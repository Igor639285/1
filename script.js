const canvas = document.getElementById('starfield');
const ctx = canvas.getContext('2d');

const stars = [];
const STAR_COUNT = 200;
const bursts = [];

function resizeCanvas() {
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
}

function rand(min, max) {
  return Math.random() * (max - min) + min;
}

function createStar() {
  return {
    x: rand(0, canvas.width),
    y: rand(0, canvas.height),
    vx: rand(-0.15, 0.15),
    vy: rand(0.05, 0.45),
    radius: rand(0.5, 2.2),
    alpha: rand(0.35, 0.95),
  };
}

function initStars() {
  stars.length = 0;
  for (let i = 0; i < STAR_COUNT; i += 1) {
    stars.push(createStar());
  }
}

function spawnBurst(x, y) {
  for (let i = 0; i < 25; i += 1) {
    const angle = rand(0, Math.PI * 2);
    const speed = rand(1.3, 3.8);
    bursts.push({
      x,
      y,
      vx: Math.cos(angle) * speed,
      vy: Math.sin(angle) * speed,
      radius: rand(1.2, 2.8),
      life: 1,
    });
  }
}

function update() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  for (const star of stars) {
    star.x += star.vx;
    star.y += star.vy;

    if (star.y > canvas.height + 4) {
      star.y = -4;
      star.x = rand(0, canvas.width);
    }
    if (star.x < -4) star.x = canvas.width + 4;
    if (star.x > canvas.width + 4) star.x = -4;

    ctx.beginPath();
    ctx.fillStyle = `rgba(255,255,255,${star.alpha})`;
    ctx.arc(star.x, star.y, star.radius, 0, Math.PI * 2);
    ctx.fill();
  }

  for (let i = bursts.length - 1; i >= 0; i -= 1) {
    const b = bursts[i];
    b.x += b.vx;
    b.y += b.vy;
    b.vx *= 0.98;
    b.vy *= 0.98;
    b.life -= 0.02;

    ctx.beginPath();
    ctx.fillStyle = `rgba(122,162,255,${Math.max(0, b.life)})`;
    ctx.arc(b.x, b.y, b.radius, 0, Math.PI * 2);
    ctx.fill();

    if (b.life <= 0) bursts.splice(i, 1);
  }

  requestAnimationFrame(update);
}

function handlePointer(event) {
  const point = event.touches ? event.touches[0] : event;
  spawnBurst(point.clientX, point.clientY);
}

window.addEventListener('resize', () => {
  resizeCanvas();
  initStars();
});

window.addEventListener('click', handlePointer);
window.addEventListener('touchstart', handlePointer, { passive: true });

resizeCanvas();
initStars();
update();
