/* NAV SCROLL */
const nav = document.getElementById('nav');
window.addEventListener('scroll', () => nav.classList.toggle('stuck', scrollY > 40));

/* MOBILE MENU */
const burger = document.getElementById('burger');
const navList = document.getElementById('nav-list');
burger.addEventListener('click', () => {
  burger.classList.toggle('on');
  navList.classList.toggle('open');
});
navList.querySelectorAll('a').forEach(a => a.addEventListener('click', () => {
  burger.classList.remove('on');
  navList.classList.remove('open');
}));
document.addEventListener('click', e => {
  if (!nav.contains(e.target)) { burger.classList.remove('on'); navList.classList.remove('open'); }
});

/* COUNTER */
function runCount(el) {
  const n = +el.dataset.n, d = 1600, s = performance.now();
  const tick = t => {
    const p = Math.min((t - s) / d, 1);
    el.textContent = Math.round((1 - Math.pow(1 - p, 3)) * n);
    if (p < 1) requestAnimationFrame(tick);
  };
  requestAnimationFrame(tick);
}
new IntersectionObserver((es, ob) => {
  es.forEach(e => { if (e.isIntersecting) { runCount(e.target); ob.unobserve(e.target); } });
}, { threshold: .6 }).observe(document.querySelector('.trust-bar'));
document.querySelectorAll('.tnum').forEach(el => {
  new IntersectionObserver((es, ob) => {
    es.forEach(e => { if (e.isIntersecting) { runCount(e.target); ob.unobserve(e.target); } });
  }, { threshold: .6 }).observe(el);
});

/* REVEAL */
new IntersectionObserver((es) => {
  es.forEach(e => { if (e.isIntersecting) e.target.classList.add('vis'); });
}, { threshold: .1 }).observe(document.body);

const revObs = new IntersectionObserver(es => {
  es.forEach(e => { if (e.isIntersecting) { e.target.classList.add('vis'); revObs.unobserve(e.target); } });
}, { threshold: .1 });
document.querySelectorAll('.reveal').forEach(el => revObs.observe(el));

/* PROJECT FILTER */
document.querySelectorAll('.wf').forEach(btn => {
  btn.addEventListener('click', function() {
    document.querySelectorAll('.wf').forEach(b => b.classList.remove('active'));
    this.classList.add('active');
    const f = this.dataset.f;
    document.querySelectorAll('.work-card').forEach(card => {
      card.classList.toggle('hidden', f !== 'all' && !(card.dataset.c || '').includes(f));
    });
  });
});

/* CONTACT FORM */
document.getElementById('cf').addEventListener('submit', function(e) {
  e.preventDefault();
  const msg = document.getElementById('cf-msg');
  const btn = this.querySelector('button[type=submit]');
  btn.disabled = true;
  btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending…';
  setTimeout(() => {
    msg.style.color = '#86efac';
    msg.textContent = '✓ Message sent! Zain will get back to you soon.';
    this.reset();
    btn.disabled = false;
    btn.innerHTML = 'Send Message <i class="fas fa-paper-plane"></i>';
    setTimeout(() => msg.textContent = '', 5000);
  }, 1200);
});
