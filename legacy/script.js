/* ===== THEME TOGGLE ===== */
const themeBtn = document.getElementById('theme-btn');
const themeIcon = document.getElementById('theme-icon');
const html = document.documentElement;

const savedTheme = localStorage.getItem('theme') || 'dark';
html.setAttribute('data-theme', savedTheme);
updateThemeIcon(savedTheme);

themeBtn.addEventListener('click', () => {
  const current = html.getAttribute('data-theme');
  const next = current === 'dark' ? 'light' : 'dark';
  html.setAttribute('data-theme', next);
  localStorage.setItem('theme', next);
  updateThemeIcon(next);
});

function updateThemeIcon(theme) {
  themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
}

/* ===== NAVBAR SCROLL ===== */
const navbar = document.getElementById('navbar');
window.addEventListener('scroll', () => {
  navbar.classList.toggle('scrolled', window.scrollY > 50);
  updateActiveNav();
});

/* ===== MOBILE MENU ===== */
const hamburger = document.getElementById('hamburger');
const navLinks = document.getElementById('nav-links');

hamburger.addEventListener('click', () => {
  hamburger.classList.toggle('open');
  navLinks.classList.toggle('open');
});

navLinks.querySelectorAll('.nav-a').forEach(link => {
  link.addEventListener('click', () => {
    hamburger.classList.remove('open');
    navLinks.classList.remove('open');
  });
});

document.addEventListener('click', (e) => {
  if (!navbar.contains(e.target)) {
    hamburger.classList.remove('open');
    navLinks.classList.remove('open');
  }
});

/* ===== SMOOTH SCROLL ===== */
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function(e) {
    const target = document.querySelector(this.getAttribute('href'));
    if (!target) return;
    e.preventDefault();
    const offset = 80;
    window.scrollTo({ top: target.offsetTop - offset, behavior: 'smooth' });
  });
});

/* ===== ACTIVE NAV ===== */
function updateActiveNav() {
  const sections = document.querySelectorAll('section[id]');
  const scrollPos = window.scrollY + 120;
  sections.forEach(sec => {
    const link = document.querySelector(`.nav-a[href="#${sec.id}"]`);
    if (!link) return;
    const inView = sec.offsetTop <= scrollPos && sec.offsetTop + sec.offsetHeight > scrollPos;
    link.classList.toggle('active', inView);
  });
}

/* ===== COUNTER ANIMATION ===== */
function animateCounter(el) {
  const target = parseInt(el.dataset.count);
  const duration = 1500;
  const start = performance.now();
  const update = (now) => {
    const elapsed = now - start;
    const progress = Math.min(elapsed / duration, 1);
    const ease = 1 - Math.pow(1 - progress, 3);
    el.textContent = Math.round(ease * target);
    if (progress < 1) requestAnimationFrame(update);
  };
  requestAnimationFrame(update);
}

/* ===== SKILL BAR ANIMATION ===== */
function animateSkillBars(entries) {
  entries.forEach(entry => {
    if (!entry.isIntersecting) return;
    entry.target.querySelectorAll('.sk-card').forEach(card => {
      const fill = card.querySelector('.sk-fill');
      const pct = card.dataset.pct;
      if (fill && pct) fill.style.width = pct + '%';
    });
    skillObs.unobserve(entry.target);
  });
}

const skillObs = new IntersectionObserver(animateSkillBars, { threshold: 0.1 });
document.querySelectorAll('.skills-wrap').forEach(el => skillObs.observe(el));

/* ===== REVEAL ANIMATIONS ===== */
const revealObs = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add('visible');
      revealObs.unobserve(entry.target);
    }
  });
}, { threshold: 0.1 });
document.querySelectorAll('.reveal').forEach(el => revealObs.observe(el));

/* ===== COUNTER TRIGGER ===== */
const counterObs = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.querySelectorAll('.stat-n').forEach(animateCounter);
      counterObs.unobserve(entry.target);
    }
  });
}, { threshold: 0.5 });
const heroStats = document.querySelector('.hero-stats');
if (heroStats) counterObs.observe(heroStats);

/* ===== PROJECT FILTER ===== */
document.querySelectorAll('.pf-btn').forEach(btn => {
  btn.addEventListener('click', function() {
    document.querySelectorAll('.pf-btn').forEach(b => b.classList.remove('active'));
    this.classList.add('active');
    const filter = this.dataset.f;
    document.querySelectorAll('.proj-card').forEach(card => {
      const cats = card.dataset.cat || '';
      card.classList.toggle('hidden', filter !== 'all' && !cats.includes(filter));
    });
  });
});

/* ===== CONTACT FORM ===== */
document.getElementById('contact-form').addEventListener('submit', function(e) {
  e.preventDefault();
  const status = document.getElementById('cf-status');
  const btn = this.querySelector('button[type=submit]');
  btn.disabled = true;
  btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending...';
  setTimeout(() => {
    status.textContent = 'Thanks! Your message has been sent. I\'ll get back to you soon.';
    status.className = 'cf-status ok';
    this.reset();
    btn.disabled = false;
    btn.innerHTML = 'Send Message <i class="fas fa-paper-plane"></i>';
    setTimeout(() => { status.textContent = ''; status.className = 'cf-status'; }, 5000);
  }, 1200);
});
