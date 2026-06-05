/* NAV SCROLL */
const hdr = document.getElementById('hdr');
window.addEventListener('scroll', () => hdr.classList.toggle('stuck', scrollY > 50), { passive: true });

/* MOBILE MENU */
const mobBtn = document.getElementById('mob-btn');
const mobMenu = document.getElementById('mob-menu');
mobBtn.addEventListener('click', () => {
  mobBtn.classList.toggle('on');
  mobMenu.classList.toggle('open');
});
mobMenu.querySelectorAll('a').forEach(a => a.addEventListener('click', () => {
  mobBtn.classList.remove('on');
  mobMenu.classList.remove('open');
}));

/* COUNTER */
function countUp(el) {
  const target = +el.dataset.n, dur = 1800, start = performance.now();
  const tick = now => {
    const t = Math.min((now - start) / dur, 1);
    el.textContent = Math.round((1 - (1 - t) ** 3) * target);
    if (t < 1) requestAnimationFrame(tick);
  };
  requestAnimationFrame(tick);
}
const cObs = new IntersectionObserver(es => {
  es.forEach(e => { if (e.isIntersecting) { countUp(e.target); cObs.unobserve(e.target); } });
}, { threshold: 0.7 });
document.querySelectorAll('.count').forEach(el => cObs.observe(el));

/* REVEAL */
const rObs = new IntersectionObserver(es => {
  es.forEach(e => { if (e.isIntersecting) { e.target.classList.add('vis'); rObs.unobserve(e.target); } });
}, { threshold: 0.1 });
document.querySelectorAll('.reveal').forEach(el => rObs.observe(el));

/* WORK FILTER */
document.querySelectorAll('.wf').forEach(btn => {
  btn.addEventListener('click', function () {
    document.querySelectorAll('.wf').forEach(b => b.classList.remove('active'));
    this.classList.add('active');
    const f = this.dataset.f;
    document.querySelectorAll('.proj-item').forEach(card => {
      card.classList.toggle('hidden', f !== 'all' && !(card.dataset.c || '').includes(f));
    });
  });
});

/* CONTACT FORM */
document.getElementById('cf').addEventListener('submit', function (e) {
  e.preventDefault();
  const ok = document.getElementById('cf-ok');
  const btn = this.querySelector('button[type=submit]');
  btn.disabled = true;
  btn.textContent = 'Sending…';
  setTimeout(() => {
    ok.textContent = '✓ Message sent! Zain will be in touch shortly.';
    this.reset();
    btn.disabled = false;
    btn.textContent = 'Send Message →';
    setTimeout(() => ok.textContent = '', 5000);
  }, 1200);
});
