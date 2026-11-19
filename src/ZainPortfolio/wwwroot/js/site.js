(function () {
  'use strict';

  /* ------------------------------------------------ theme toggle */
  const root = document.documentElement;
  const stored = localStorage.getItem('zp-theme');
  if (stored) root.setAttribute('data-theme', stored);

  function syncThemeIcon() {
    const icon = document.getElementById('theme-icon');
    if (!icon) return;
    const dark = root.getAttribute('data-theme') === 'dark';
    icon.className = dark ? 'fas fa-moon' : 'fas fa-sun';
  }
  syncThemeIcon();

  const themeBtn = document.getElementById('theme-btn');
  if (themeBtn) {
    themeBtn.addEventListener('click', function () {
      const next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
      root.setAttribute('data-theme', next);
      localStorage.setItem('zp-theme', next);
      syncThemeIcon();
    });
  }

  /* ------------------------------------------------ mobile nav */
  const hamburger = document.getElementById('hamburger');
  const navLinks = document.getElementById('nav-links');
  if (hamburger && navLinks) {
    hamburger.addEventListener('click', function () {
      const open = navLinks.classList.toggle('open');
      hamburger.classList.toggle('open', open);
      hamburger.setAttribute('aria-expanded', String(open));
    });
    navLinks.querySelectorAll('a').forEach(function (a) {
      a.addEventListener('click', function () {
        navLinks.classList.remove('open');
        hamburger.classList.remove('open');
        hamburger.setAttribute('aria-expanded', 'false');
      });
    });
  }

  /* ------------------------------------------------ scroll effects */
  const navbar = document.getElementById('navbar');
  const progress = document.getElementById('scroll-progress');
  const toTop = document.getElementById('to-top');

  function onScroll() {
    const y = window.scrollY;
    const h = document.documentElement.scrollHeight - window.innerHeight;
    if (navbar) navbar.classList.toggle('scrolled', y > 20);
    if (progress) progress.style.width = (h > 0 ? (y / h) * 100 : 0) + '%';
    if (toTop) toTop.classList.toggle('show', y > 500);
  }
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();

  if (toTop) {
    toTop.addEventListener('click', function () {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  /* ------------------------------------------------ reveal on scroll */
  const revealEls = document.querySelectorAll('.reveal');
  if ('IntersectionObserver' in window && revealEls.length) {
    const io = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) {
        if (e.isIntersecting) { e.target.classList.add('visible'); io.unobserve(e.target); }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });
    revealEls.forEach(function (el) { io.observe(el); });
  } else {
    revealEls.forEach(function (el) { el.classList.add('visible'); });
  }

  /* ------------------------------------------------ skill bars */
  const skillCards = document.querySelectorAll('.sk-card');
  if ('IntersectionObserver' in window && skillCards.length) {
    const so = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) {
        if (!e.isIntersecting) return;
        const fill = e.target.querySelector('.sk-fill');
        if (fill) fill.style.width = (e.target.dataset.pct || 0) + '%';
        so.unobserve(e.target);
      });
    }, { threshold: 0.4 });
    skillCards.forEach(function (c) { so.observe(c); });
  }

  /* ------------------------------------------------ animated counters */
  const counters = document.querySelectorAll('.stat-n[data-count]');
  if ('IntersectionObserver' in window && counters.length) {
    const co = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) {
        if (!e.isIntersecting) return;
        const el = e.target;
        const target = parseInt(el.dataset.count, 10) || 0;
        const duration = 1400;
        const start = performance.now();
        function tick(now) {
          const p = Math.min((now - start) / duration, 1);
          el.textContent = Math.floor(target * (1 - Math.pow(1 - p, 3)));
          if (p < 1) requestAnimationFrame(tick);
          else el.textContent = target;
        }
        requestAnimationFrame(tick);
        co.unobserve(el);
      });
    }, { threshold: 0.5 });
    counters.forEach(function (c) { co.observe(c); });
  }

  /* ------------------------------------------------ typewriter */
  const tw = document.getElementById('typewriter');
  if (tw && tw.dataset.roles) {
    const roles = tw.dataset.roles.split(',').map(function (r) { return r.trim(); }).filter(Boolean);
    if (roles.length) {
      let ri = 0, ci = 0, deleting = false;
      (function type() {
        const word = roles[ri];
        tw.textContent = deleting ? word.substring(0, ci--) : word.substring(0, ci++);
        let delay = deleting ? 45 : 95;
        if (!deleting && ci > word.length) { deleting = true; delay = 1600; }
        else if (deleting && ci < 0) { deleting = false; ri = (ri + 1) % roles.length; ci = 0; delay = 300; }
        setTimeout(type, delay);
      })();
    }
  }

  /* ------------------------------------------------ project filters */
  const filterBtns = document.querySelectorAll('.pf-btn');
  const projCards = document.querySelectorAll('.proj-card[data-cat]');
  if (filterBtns.length && projCards.length) {
    filterBtns.forEach(function (btn) {
      btn.addEventListener('click', function () {
        const f = btn.dataset.f;
        filterBtns.forEach(function (b) { b.classList.remove('active'); });
        btn.classList.add('active');
        projCards.forEach(function (card) {
          const show = f === 'all' || (card.dataset.cat || '').indexOf(f) !== -1;
          card.classList.toggle('hidden', !show);
        });
      });
    });
  }

  /* ------------------------------------------------ FAQ accordion */
  document.querySelectorAll('.faq-q').forEach(function (q) {
    q.addEventListener('click', function () {
      const item = q.closest('.faq-item');
      const wasOpen = item.classList.contains('open');
      document.querySelectorAll('.faq-item').forEach(function (i) { i.classList.remove('open'); });
      item.classList.toggle('open', !wasOpen);
    });
  });

  /* ------------------------------------------------ newsletter subscribe */
  document.querySelectorAll('form[data-subscribe]').forEach(function (form) {
    form.addEventListener('submit', async function (e) {
      e.preventDefault();
      const msg = form.parentElement.querySelector('.sub-msg');
      const btn = form.querySelector('button');
      const original = btn ? btn.innerHTML : '';
      if (btn) { btn.disabled = true; btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>'; }

      try {
        const data = new FormData(form);
        data.append('source', form.dataset.source || 'site');
        const res = await fetch('/subscribe', {
          method: 'POST',
          body: data,
          headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        const json = await res.json();
        if (msg) {
          msg.textContent = json.message;
          msg.className = 'sub-msg ' + (json.ok ? 'ok' : 'err');
        }
        if (json.ok) form.reset();
      } catch (err) {
        if (msg) { msg.textContent = 'Something went wrong. Please try again.'; msg.className = 'sub-msg err'; }
      } finally {
        if (btn) { btn.disabled = false; btn.innerHTML = original; }
      }
    });
  });

  /* ------------------------------------------------ active nav on scroll */
  const sections = document.querySelectorAll('section[id]');
  if (sections.length) {
    window.addEventListener('scroll', function () {
      let current = '';
      sections.forEach(function (sec) {
        if (window.scrollY >= sec.offsetTop - 140) current = sec.id;
      });
      document.querySelectorAll('.nav-a').forEach(function (a) {
        a.classList.toggle('active', a.getAttribute('href') === '/#' + current);
      });
    }, { passive: true });
  }

  /* Copy-link button on articles. */
  document.querySelectorAll('[data-copy-link]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var url = btn.getAttribute('data-copy-link');
      var done = function () {
        var icon = btn.querySelector('i');
        var previous = icon ? icon.className : null;
        btn.classList.add('copied');
        if (icon) icon.className = 'fas fa-check';
        setTimeout(function () {
          btn.classList.remove('copied');
          if (icon && previous) icon.className = previous;
        }, 1600);
      };
      if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(url).then(done).catch(function () {});
      } else {
        // http:// origins and older browsers have no clipboard API.
        var field = document.createElement('textarea');
        field.value = url;
        field.setAttribute('readonly', '');
        field.style.position = 'fixed';
        field.style.opacity = '0';
        document.body.appendChild(field);
        field.select();
        try { document.execCommand('copy'); done(); } catch (e) { /* nothing to do */ }
        document.body.removeChild(field);
      }
    });
  });

})();
