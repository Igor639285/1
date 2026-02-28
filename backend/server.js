const express = require('express');
const cors = require('cors');
const multer = require('multer');
const fs = require('fs');
const fsp = require('fs/promises');
const path = require('path');
const os = require('os');
const crypto = require('crypto');
const mime = require('mime-types');

const app = express();
const BASE_PORT = Number(process.env.PORT || 3000);
const ROOT_DIR = path.join(__dirname, '..');
const STORAGE_DIR = path.join(ROOT_DIR, 'storage');
const FRONTEND_DIR = path.join(ROOT_DIR, 'frontend');
const TTL_MS = Number(process.env.SHARE_TTL_MS || 24 * 60 * 60 * 1000);
const CLEANUP_INTERVAL_MS = Number(process.env.CLEANUP_INTERVAL_MS || 60 * 60 * 1000);
const MAX_FILE_SIZE_BYTES = Number(process.env.MAX_FILE_SIZE_BYTES || 500 * 1024 * 1024);

const mediaStore = new Map();
let activePort = BASE_PORT;

function ensureStorageDir() {
  if (!fs.existsSync(STORAGE_DIR)) {
    fs.mkdirSync(STORAGE_DIR, { recursive: true });
  }
}

function getLanIp() {
  const interfaces = os.networkInterfaces();
  let fallback = '127.0.0.1';

  for (const entries of Object.values(interfaces)) {
    for (const entry of entries || []) {
      if (entry.internal || entry.family !== 'IPv4') {
        continue;
      }

      if (
        entry.address.startsWith('192.168.') ||
        entry.address.startsWith('10.') ||
        /^172\.(1[6-9]|2\d|3[01])\./.test(entry.address)
      ) {
        return entry.address;
      }

      fallback = entry.address;
    }
  }

  return fallback;
}

function createToken() {
  return crypto.randomBytes(24).toString('hex');
}

function createId() {
  return crypto.randomUUID();
}

function isSupportedMime(mimeType) {
  return /^video\//.test(mimeType) || /^image\//.test(mimeType);
}

function cleanupExpired() {
  const now = Date.now();
  for (const [id, item] of mediaStore.entries()) {
    if (item.expiresAt <= now) {
      mediaStore.delete(id);
      fsp.unlink(item.path).catch(() => {});
    }
  }
}

function getValidMedia(id, token) {
  const media = mediaStore.get(id);
  if (!media) {
    return { error: 404, message: 'Not found' };
  }

  if (media.expiresAt <= Date.now()) {
    mediaStore.delete(id);
    fsp.unlink(media.path).catch(() => {});
    return { error: 410, message: 'Link expired' };
  }

  if (!token) {
    return { error: 401, message: 'Token required' };
  }

  const provided = Buffer.from(token);
  const expected = Buffer.from(media.secret);
  if (provided.length !== expected.length || !crypto.timingSafeEqual(provided, expected)) {
    return { error: 403, message: 'Invalid token' };
  }

  return { media };
}

ensureStorageDir();

const storage = multer.diskStorage({
  destination: (_req, _file, cb) => cb(null, STORAGE_DIR),
  filename: (_req, file, cb) => {
    const ext = mime.extension(file.mimetype) || path.extname(file.originalname).replace('.', '') || 'bin';
    cb(null, `${createId()}.${ext}`);
  }
});

const upload = multer({
  storage,
  limits: { fileSize: MAX_FILE_SIZE_BYTES },
  fileFilter: (_req, file, cb) => {
    if (!isSupportedMime(file.mimetype)) {
      cb(new Error('Only image/* and video/* files are allowed'));
      return;
    }
    cb(null, true);
  }
});

app.use(cors());
app.use(express.json());
app.use(express.static(FRONTEND_DIR));

app.get('/api/config', (_req, res) => {
  res.json({
    lanIp: getLanIp(),
    port: activePort,
    ttlMs: TTL_MS,
    maxFileSizeBytes: MAX_FILE_SIZE_BYTES
  });
});

app.post('/api/media', (req, res) => {
  upload.single('media')(req, res, (err) => {
    if (err) {
      if (err instanceof multer.MulterError && err.code === 'LIMIT_FILE_SIZE') {
        return res.status(413).json({ error: `File is too large (max ${MAX_FILE_SIZE_BYTES} bytes)` });
      }
      return res.status(400).json({ error: err.message || 'Upload failed' });
    }

    if (!req.file) {
      return res.status(400).json({ error: 'File is required in field "media"' });
    }

    const id = createId();
    const secret = createToken();
    const now = Date.now();
    const expiresAt = now + TTL_MS;

    mediaStore.set(id, {
      id,
      secret,
      path: req.file.path,
      mimeType: req.file.mimetype,
      originalName: req.file.originalname,
      createdAt: now,
      expiresAt,
      size: req.file.size
    });

    res.status(201).json({
      id,
      token: secret,
      expiresAt,
      mimeType: req.file.mimetype,
      size: req.file.size,
      originalName: req.file.originalname
    });
  });
});

app.get('/api/share/:id', (req, res) => {
  const { id } = req.params;
  const token = req.query.token;
  const { media, error, message } = getValidMedia(id, typeof token === 'string' ? token : '');

  if (error) {
    return res.status(error).json({ error: message });
  }

  const lanIp = getLanIp();
  const url = `http://${lanIp}:${activePort}/view/${id}?token=${encodeURIComponent(media.secret)}`;

  res.json({
    url,
    expiresAt: media.expiresAt,
    mimeType: media.mimeType
  });
});

app.get('/view/:id', (req, res) => {
  const token = typeof req.query.token === 'string' ? req.query.token : '';
  const { media, error, message } = getValidMedia(req.params.id, token);

  if (error) {
    return res.status(error).send(`<h2>${message}</h2>`);
  }

  const isVideo = media.mimeType.startsWith('video/');
  const content = isVideo
    ? `<video controls style="max-width: 100%; max-height: 90vh;" src="/media/${media.id}?token=${encodeURIComponent(token)}"></video>`
    : `<img style="max-width: 100%; max-height: 90vh; object-fit: contain;" src="/media/${media.id}?token=${encodeURIComponent(token)}" alt="Shared media"/>`;

  res.type('html').send(`<!doctype html>
<html>
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>LAN Media Viewer</title>
  </head>
  <body style="margin:0;min-height:100vh;display:flex;align-items:center;justify-content:center;background:#0f172a;color:#f8fafc;">
    <main style="padding:16px;text-align:center;">
      <h2 style="font-family:Arial,sans-serif;">${media.originalName}</h2>
      ${content}
    </main>
  </body>
</html>`);
});

app.get('/media/:id', async (req, res) => {
  const token = typeof req.query.token === 'string' ? req.query.token : '';
  const { media, error, message } = getValidMedia(req.params.id, token);

  if (error) {
    return res.status(error).json({ error: message });
  }

  try {
    const stat = await fsp.stat(media.path);
    const total = stat.size;

    res.setHeader('Content-Type', media.mimeType);
    res.setHeader('Accept-Ranges', 'bytes');
    res.setHeader('Cache-Control', 'no-store');

    const range = req.headers.range;
    if (range) {
      const matches = range.match(/bytes=(\d*)-(\d*)/);
      if (!matches) {
        return res.status(416).send('Invalid range');
      }

      let start = matches[1] ? Number(matches[1]) : 0;
      let end = matches[2] ? Number(matches[2]) : total - 1;
      if (Number.isNaN(start) || Number.isNaN(end) || start > end || start >= total) {
        return res.status(416).send('Range not satisfiable');
      }

      if (end >= total) {
        end = total - 1;
      }

      res.status(206);
      res.setHeader('Content-Range', `bytes ${start}-${end}/${total}`);
      res.setHeader('Content-Length', end - start + 1);

      fs.createReadStream(media.path, { start, end }).pipe(res);
      return;
    }

    res.setHeader('Content-Length', total);
    fs.createReadStream(media.path).pipe(res);
  } catch (_err) {
    mediaStore.delete(req.params.id);
    res.status(404).json({ error: 'Media file missing' });
  }
});

setInterval(cleanupExpired, CLEANUP_INTERVAL_MS).unref();

function startServer(port, attemptsLeft = 20) {
  const server = app.listen(port, '0.0.0.0', () => {
    activePort = port;
    const lanIp = getLanIp();
    console.log(`LAN Media Viewer backend running on http://0.0.0.0:${activePort}`);
    console.log(`LAN access URL: http://${lanIp}:${activePort}`);
  });

  server.on('error', (err) => {
    if (err && err.code === 'EADDRINUSE' && attemptsLeft > 0) {
      console.warn(`Port ${port} is busy, trying ${port + 1}...`);
      startServer(port + 1, attemptsLeft - 1);
      return;
    }

    console.error('Failed to start server:', err.message);
    process.exit(1);
  });
}

startServer(BASE_PORT);
