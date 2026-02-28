const mediaInput = document.getElementById('mediaInput');
const uploadBtn = document.getElementById('uploadBtn');
const resultBox = document.getElementById('resultBox');
const errorBox = document.getElementById('errorBox');
const configInfo = document.getElementById('configInfo');

async function loadConfig() {
  const res = await fetch('/api/config');
  const data = await res.json();
  const maxMb = (data.maxFileSizeBytes / (1024 * 1024)).toFixed(0);
  configInfo.textContent = `LAN IP: ${data.lanIp} | Порт: ${data.port} | TTL: ${(data.ttlMs / (1000 * 60 * 60)).toFixed(0)}ч | Лимит: ${maxMb}MB`;
}

function showError(message) {
  errorBox.style.display = 'block';
  errorBox.textContent = message;
}

function resetMessages() {
  errorBox.style.display = 'none';
  errorBox.textContent = '';
  resultBox.style.display = 'none';
  resultBox.textContent = '';
}

uploadBtn.addEventListener('click', async () => {
  resetMessages();

  const file = mediaInput.files?.[0];
  if (!file) {
    showError('Выберите файл перед загрузкой.');
    return;
  }

  uploadBtn.disabled = true;
  uploadBtn.textContent = 'Загрузка...';

  try {
    const formData = new FormData();
    formData.append('media', file);

    const uploadRes = await fetch('/api/media', {
      method: 'POST',
      body: formData
    });
    const uploadData = await uploadRes.json();

    if (!uploadRes.ok) {
      throw new Error(uploadData.error || 'Ошибка загрузки');
    }

    const shareRes = await fetch(`/api/share/${uploadData.id}?token=${encodeURIComponent(uploadData.token)}`);
    const shareData = await shareRes.json();
    if (!shareRes.ok) {
      throw new Error(shareData.error || 'Ошибка создания ссылки');
    }

    resultBox.style.display = 'block';
    const expires = new Date(shareData.expiresAt).toLocaleString();
    resultBox.innerHTML = `
      <strong>Ссылка для просмотра:</strong><br/>
      <a href="${shareData.url}" target="_blank" rel="noreferrer">${shareData.url}</a>
      <br/><small>Действует до: ${expires}</small>
    `;
  } catch (err) {
    showError(err.message || 'Неизвестная ошибка');
  } finally {
    uploadBtn.disabled = false;
    uploadBtn.textContent = 'Загрузить и получить ссылку';
  }
});

loadConfig().catch((err) => {
  showError(`Не удалось загрузить конфигурацию: ${err.message}`);
  configInfo.textContent = 'Не удалось определить LAN IP.';
});
