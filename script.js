/* ============================================================
   PORTFOLIO – script.js
   Features: particles · custom cursor · typewriter · tilt cards
             magnetic buttons · scroll progress · all original fx
   ============================================================ */

/* ── SCROLL PROGRESS ─────────────────────────────────────── */
const scrollBar = document.getElementById('scroll-progress');
window.addEventListener('scroll', () => {
  const pct = window.scrollY / (document.body.scrollHeight - window.innerHeight) * 100;
  if (scrollBar) scrollBar.style.width = pct + '%';
}, { passive: true });

/* ── CUSTOM CURSOR ───────────────────────────────────────── */
const dot  = document.getElementById('cursor-dot');
const ring = document.getElementById('cursor-ring');
let mouseX = 0, mouseY = 0, ringX = 0, ringY = 0;

window.addEventListener('mousemove', e => {
  mouseX = e.clientX; mouseY = e.clientY;
  if (dot)  { dot.style.left  = mouseX + 'px'; dot.style.top  = mouseY + 'px'; }
}, { passive: true });

(function animateRing() {
  ringX += (mouseX - ringX) * 0.12;
  ringY += (mouseY - ringY) * 0.12;
  if (ring) { ring.style.left = ringX + 'px'; ring.style.top  = ringY + 'px'; }
  requestAnimationFrame(animateRing);
})();

document.addEventListener('mouseleave', () => {
  if (dot)  dot.style.opacity  = '0';
  if (ring) ring.style.opacity = '0';
});
document.addEventListener('mouseenter', () => {
  if (dot)  dot.style.opacity  = '1';
  if (ring) ring.style.opacity = '1';
});

/* ── PARTICLES CANVAS ────────────────────────────────────── */
(function initParticles() {
  const canvas = document.getElementById('particles-canvas');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');

  function resize() {
    canvas.width  = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
  }
  resize();
  window.addEventListener('resize', resize, { passive: true });

  const COUNT = 70;
  const particles = Array.from({ length: COUNT }, () => ({
    x: Math.random() * canvas.width,
    y: Math.random() * canvas.height,
    vx: (Math.random() - 0.5) * 0.4,
    vy: (Math.random() - 0.5) * 0.4,
    r: Math.random() * 1.5 + 0.5,
  }));

  function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const MAX_DIST = 140;

    for (let i = 0; i < particles.length; i++) {
      for (let j = i + 1; j < particles.length; j++) {
        const dx = particles[i].x - particles[j].x;
        const dy = particles[i].y - particles[j].y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        if (dist < MAX_DIST) {
          const alpha = (1 - dist / MAX_DIST) * 0.2;
          ctx.beginPath();
          ctx.moveTo(particles[i].x, particles[i].y);
          ctx.lineTo(particles[j].x, particles[j].y);
          ctx.strokeStyle = `rgba(99,102,241,${alpha})`;
          ctx.lineWidth = 0.8;
          ctx.stroke();
        }
      }
    }

    particles.forEach(p => {
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
      ctx.fillStyle = 'rgba(129,140,248,0.6)';
      ctx.fill();
      p.x += p.vx; p.y += p.vy;
      if (p.x < 0 || p.x > canvas.width)  p.vx *= -1;
      if (p.y < 0 || p.y > canvas.height) p.vy *= -1;
    });

    requestAnimationFrame(draw);
  }
  draw();
})();

/* ── TYPEWRITER ──────────────────────────────────────────── */
(function initTypewriter() {
  const el = document.getElementById('typewriter');
  if (!el) return;
  const words = ['AI Technical Lead', 'Cloud Architect', 'Delivery Manager', 'RAG & LLM Expert'];
  let wi = 0, ci = 0, deleting = false;

  function tick() {
    const word = words[wi];
    el.textContent = deleting ? word.slice(0, ci--) : word.slice(0, ci++);
    let delay = deleting ? 55 : 100;
    if (!deleting && ci > word.length)  { delay = 1800; deleting = true; }
    else if (deleting && ci < 0)        { deleting = false; wi = (wi + 1) % words.length; ci = 0; delay = 400; }
    setTimeout(tick, delay);
  }
  tick();
})();

/* ── THEME TOGGLE ────────────────────────────────────────── */
const themeBtn  = document.getElementById('theme-btn');
const themeIcon = document.getElementById('theme-icon');
const html      = document.documentElement;

const savedTheme = localStorage.getItem('theme') || 'dark';
html.setAttribute('data-theme', savedTheme);
if (themeIcon) themeIcon.className = savedTheme === 'dark' ? 'fas fa-moon' : 'fas fa-sun';

if (themeBtn) {
  themeBtn.addEventListener('click', () => {
    const next = html.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-theme', next);
    localStorage.setItem('theme', next);
    if (themeIcon) themeIcon.className = next === 'dark' ? 'fas fa-moon' : 'fas fa-sun';
  });
}

/* ── NAVBAR SCROLL ───────────────────────────────────────── */
const navbar   = document.getElementById('navbar');
const navLinks = document.querySelectorAll('.nav-a');
const sections = document.querySelectorAll('section[id]');

function updateActiveNav() {
  const scrollPos = window.scrollY + 120;
  sections.forEach(sec => {
    const link = document.querySelector(`.nav-a[href="#${sec.id}"]`);
    if (!link) return;
    link.classList.toggle('active', sec.offsetTop <= scrollPos && sec.offsetTop + sec.offsetHeight > scrollPos);
  });
}

window.addEventListener('scroll', () => {
  if (navbar) navbar.classList.toggle('scrolled', window.scrollY > 50);
  updateActiveNav();
}, { passive: true });

/* ── HAMBURGER MENU ──────────────────────────────────────── */
const hamburger = document.getElementById('hamburger');
const navMenu   = document.getElementById('nav-links');
if (hamburger && navMenu) {
  hamburger.addEventListener('click', () => {
    hamburger.classList.toggle('open');
    navMenu.classList.toggle('open');
  });
  navMenu.querySelectorAll('.nav-a').forEach(link => {
    link.addEventListener('click', () => {
      hamburger.classList.remove('open');
      navMenu.classList.remove('open');
    });
  });
}

/* ── SMOOTH SCROLL ───────────────────────────────────────── */
document.querySelectorAll('a[href^="#"]').forEach(a => {
  a.addEventListener('click', e => {
    const target = document.querySelector(a.getAttribute('href'));
    if (!target) return;
    e.preventDefault();
    window.scrollTo({ top: target.offsetTop - 80, behavior: 'smooth' });
  });
});

/* ── INTERSECTION OBSERVER (reveal + counters + skill bars) ─ */
const io = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (!entry.isIntersecting) return;
    const el = entry.target;

    if (el.classList.contains('reveal'))      el.classList.add('visible');
    if (el.classList.contains('stat-n'))      animateCounter(el);
    if (el.classList.contains('skill-group')) animateSkillGroup(el);

    io.unobserve(el);
  });
}, { threshold: 0.15 });

document.querySelectorAll('.reveal, .stat-n, .skill-group').forEach(el => io.observe(el));

function easeOut(t) { return 1 - Math.pow(2, -10 * t); }
function animateCounter(el) {
  const target = +el.dataset.count, dur = 1600, start = performance.now();
  (function step(now) {
    const p = Math.min((now - start) / dur, 1);
    el.textContent = Math.round(easeOut(p) * target);
    if (p < 1) requestAnimationFrame(step);
  })(start);
}
function animateSkillGroup(group) {
  group.querySelectorAll('.sk-card').forEach((card, i) => {
    setTimeout(() => {
      const fill = card.querySelector('.sk-fill');
      if (fill) fill.style.width = card.dataset.pct + '%';
    }, i * 90);
  });
}

/* ── PROJECT FILTER ──────────────────────────────────────── */
document.querySelectorAll('.pf-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    document.querySelectorAll('.pf-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    const f = btn.dataset.f;
    document.querySelectorAll('.tilt-card, .proj-card').forEach(card => {
      const cats = card.dataset.cat || '';
      card.classList.toggle('hidden', f !== 'all' && !cats.includes(f));
    });
  });
});

/* ── 3D CARD TILT ────────────────────────────────────────── */
document.querySelectorAll('.tilt-card').forEach(card => {
  card.addEventListener('mouseenter', () => {
    card.style.transition = 'transform 0.1s ease, box-shadow 0.3s, border-color 0.3s';
  });
  card.addEventListener('mousemove', e => {
    const rect = card.getBoundingClientRect();
    const cx = rect.left + rect.width  / 2;
    const cy = rect.top  + rect.height / 2;
    const rx = ((e.clientY - cy) / (rect.height / 2)) * -8;
    const ry = ((e.clientX - cx) / (rect.width  / 2)) *  8;
    card.style.transform = `perspective(900px) rotateX(${rx}deg) rotateY(${ry}deg) translateZ(8px)`;
  });
  card.addEventListener('mouseleave', () => {
    card.style.transition = 'transform 0.5s ease, box-shadow 0.3s, border-color 0.3s';
    card.style.transform  = 'perspective(900px) rotateX(0) rotateY(0) translateZ(0)';
  });
});

/* ── MAGNETIC BUTTONS ────────────────────────────────────── */
document.querySelectorAll('.magnetic').forEach(btn => {
  btn.addEventListener('mouseenter', () => {
    btn.style.transition = 'transform 0.1s ease, box-shadow 0.2s, opacity 0.2s';
  });
  btn.addEventListener('mousemove', e => {
    const rect = btn.getBoundingClientRect();
    const dx = e.clientX - (rect.left + rect.width  / 2);
    const dy = e.clientY - (rect.top  + rect.height / 2);
    btn.style.transform = `translate(${dx * 0.28}px, ${dy * 0.28}px)`;
  });
  btn.addEventListener('mouseleave', () => {
    btn.style.transition = 'transform 0.45s ease, box-shadow 0.2s, opacity 0.2s';
    btn.style.transform  = '';
  });
});

/* ── CONTACT FORM ────────────────────────────────────────── */
const form   = document.getElementById('contact-form');
const status = document.getElementById('cf-status');
if (form) {
  form.addEventListener('submit', e => {
    e.preventDefault();
    const btn = form.querySelector('button[type="submit"]');
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending…';
    btn.disabled = true;
    setTimeout(() => {
      btn.innerHTML = 'Send Message <i class="fas fa-paper-plane"></i>';
      btn.disabled  = false;
      status.textContent = '✓ Message sent! I\'ll get back to you soon.';
      status.className   = 'cf-status ok';
      form.reset();
      setTimeout(() => { status.textContent = ''; status.className = 'cf-status'; }, 5000);
    }, 1500);
  });
}
