/* ---------------------------------------------------------------------------
   AI Lab — an interactive walkthrough of a production RAG pipeline.

   Everything runs locally: no network, no keys, no build step. The "answer"
   is assembled from the visitor's own question so the demo feels responsive
   without pretending to be a real model.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var pipe   = document.getElementById('ai-pipe');
  if (!pipe) return;

  var input  = document.getElementById('ai-input');
  var runBtn = document.getElementById('ai-run');
  var resetBtn = document.getElementById('ai-reset');
  var detail = document.getElementById('ai-detail');
  var answer = document.getElementById('ai-answer');
  var stages = Array.prototype.slice.call(pipe.querySelectorAll('.ai-stage'));

  var timers = [];
  var running = false;

  function showStage(n) {
    stages.forEach(function (s) {
      s.classList.toggle('active', +s.getAttribute('data-stage') === n);
    });
    detail.classList.remove('is-hint');
    detail.innerHTML =
      '<h4 data-i18n="ai.st' + n + '">…</h4>' +
      '<p data-i18n="ai.st' + n + 'd">…</p>';
    if (window.ZatI18n) window.ZatI18n.translate(detail);
  }

  function clearTimers() {
    timers.forEach(clearTimeout);
    timers = [];
  }

  function reset() {
    clearTimers();
    running = false;
    runBtn.disabled = false;
    stages.forEach(function (s) { s.classList.remove('done', 'running', 'active'); });
    detail.classList.add('is-hint');
    detail.innerHTML = '<p data-i18n="ai.hint">Click a stage above to see what it does</p>';
    answer.innerHTML = '';
    ['ai-m1', 'ai-m2', 'ai-m3'].forEach(function (id) {
      document.getElementById(id).textContent = '—';
    });
    if (window.ZatI18n) window.ZatI18n.translate(detail);
  }

  function countUp(el, to, suffix, decimals, prefix) {
    var steps = 24, i = 0;
    var tick = setInterval(function () {
      i++;
      var v = (to * i) / steps;
      el.textContent = (prefix || '') + (decimals ? v.toFixed(decimals) : Math.round(v)) + suffix;
      if (i >= steps) clearInterval(tick);
    }, 22);
    timers.push(tick);
  }

  function finish(question) {
    var q = (question || '').trim();
    var safe = q.replace(/[<>&]/g, function (c) {
      return { '<': '&lt;', '>': '&gt;', '&': '&amp;' }[c];
    });

    countUp(document.getElementById('ai-m1'), 840, ' ms', 0);
    countUp(document.getElementById('ai-m2'), 100, ' %', 0);
    countUp(document.getElementById('ai-m3'), 0.004, '', 3, '$');

    answer.innerHTML =
      '<div class="ai-answer">' +
      '<strong><i class="fas fa-circle-check"></i> Grounded answer</strong><br>' +
      'Retrieved <strong>4 chunks</strong> from 2 documents for ' +
      (safe ? '&ldquo;' + safe + '&rdquo;' : 'your question') +
      ', re-ranked them, and answered using only that context. ' +
      'Nothing outside the retrieved passages was used.' +
      '<span class="ai-cite"><i class="fas fa-file-lines"></i> ' +
      'policy-handbook.pdf &middot; p.12  |  faq-internal.docx &middot; §4.2</span>' +
      '</div>';
  }

  function run() {
    if (running) return;
    running = true;
    runBtn.disabled = true;
    clearTimers();
    answer.innerHTML = '';
    stages.forEach(function (s) { s.classList.remove('done', 'running', 'active'); });

    var question = input.value;
    var step = 0;

    function next() {
      if (step > 0) {
        stages[step - 1].classList.remove('running');
        stages[step - 1].classList.add('done');
      }
      if (step >= stages.length) {
        running = false;
        runBtn.disabled = false;
        finish(question);
        return;
      }
      stages[step].classList.add('running');
      showStage(step + 1);
      step++;
      timers.push(setTimeout(next, 780));
    }
    next();
  }

  stages.forEach(function (s) {
    s.addEventListener('click', function () {
      if (running) return;
      showStage(+s.getAttribute('data-stage'));
    });
  });

  runBtn.addEventListener('click', run);
  resetBtn.addEventListener('click', reset);
  input.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') { e.preventDefault(); run(); }
  });

  // Re-render the open stage when the visitor switches language.
  document.addEventListener('languagechange', function () {
    var open = pipe.querySelector('.ai-stage.active');
    if (open && !running) showStage(+open.getAttribute('data-stage'));
  });
})();

/* ---------------------------------------------------------------------------
   Architecture explorer — click a layer, read what lives in it.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var stack = document.getElementById('arc-stack');
  if (!stack) return;
  var detail = document.getElementById('arc-detail');
  var layers = Array.prototype.slice.call(stack.querySelectorAll('.arc-layer'));

  function open(n) {
    layers.forEach(function (l) {
      l.classList.toggle('active', +l.getAttribute('data-layer') === n);
    });
    detail.classList.remove('is-hint');
    detail.innerHTML =
      '<span class="arc-detail-kicker" data-i18n="arc.l' + n + 'c">…</span>' +
      '<h4 data-i18n-html="arc.l' + n + '">…</h4>' +
      '<p data-i18n="arc.l' + n + 'd">…</p>';
    if (window.ZatI18n) window.ZatI18n.translate(detail);
  }

  layers.forEach(function (l) {
    l.addEventListener('click', function () { open(+l.getAttribute('data-layer')); });
  });

  document.addEventListener('languagechange', function () {
    var cur = stack.querySelector('.arc-layer.active');
    if (cur) open(+cur.getAttribute('data-layer'));
  });
})();

/* ---------------------------------------------------------------------------
   Neural-network backdrop. Decorative only, so it is skipped entirely when the
   visitor has asked for reduced motion.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var canvas = document.getElementById('neural-bg');
  if (!canvas) return;
  if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    canvas.style.display = 'none';
    return;
  }

  var ctx = canvas.getContext('2d');
  var nodes = [], raf = null, w = 0, h = 0;
  var LINK = 150;

  function size() {
    var host = canvas.parentElement;
    w = canvas.width = host.offsetWidth;
    h = canvas.height = host.offsetHeight;
    var count = Math.min(46, Math.max(16, Math.round((w * h) / 26000)));
    nodes = [];
    for (var i = 0; i < count; i++) {
      nodes.push({
        x: Math.random() * w, y: Math.random() * h,
        vx: (Math.random() - 0.5) * 0.28, vy: (Math.random() - 0.5) * 0.28,
        r: 1.4 + Math.random() * 1.8
      });
    }
  }

  function frame() {
    ctx.clearRect(0, 0, w, h);

    for (var i = 0; i < nodes.length; i++) {
      var a = nodes[i];
      a.x += a.vx; a.y += a.vy;
      if (a.x < 0 || a.x > w) a.vx *= -1;
      if (a.y < 0 || a.y > h) a.vy *= -1;

      for (var j = i + 1; j < nodes.length; j++) {
        var b = nodes[j];
        var dx = a.x - b.x, dy = a.y - b.y;
        var d = Math.sqrt(dx * dx + dy * dy);
        if (d < LINK) {
          ctx.strokeStyle = 'rgba(124, 108, 240, ' + (0.16 * (1 - d / LINK)).toFixed(3) + ')';
          ctx.lineWidth = 1;
          ctx.beginPath();
          ctx.moveTo(a.x, a.y);
          ctx.lineTo(b.x, b.y);
          ctx.stroke();
        }
      }

      ctx.fillStyle = 'rgba(124, 108, 240, 0.5)';
      ctx.beginPath();
      ctx.arc(a.x, a.y, a.r, 0, Math.PI * 2);
      ctx.fill();
    }
    raf = requestAnimationFrame(frame);
  }

  // Only animate while the section is on screen — no cost when scrolled away.
  var io = new IntersectionObserver(function (entries) {
    entries.forEach(function (e) {
      if (e.isIntersecting && !raf) { size(); frame(); }
      else if (!e.isIntersecting && raf) { cancelAnimationFrame(raf); raf = null; }
    });
  }, { threshold: 0.01 });
  io.observe(canvas.parentElement);

  var resizeT;
  window.addEventListener('resize', function () {
    clearTimeout(resizeT);
    resizeT = setTimeout(function () { if (raf) size(); }, 200);
  });
})();

/* ---------------------------------------------------------------------------
   Impact counters — count up once, when the band first scrolls into view.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var grid = document.querySelector('.imp-grid');
  if (!grid) return;

  function run(el) {
    var raw = el.getAttribute('data-target') || el.textContent;
    var num = parseFloat(raw.replace(/[^0-9.]/g, ''));
    if (isNaN(num)) return;
    var suffix = raw.replace(/[0-9.]/g, '');   // keeps % and +
    var steps = 34, i = 0;
    var tick = setInterval(function () {
      i++;
      el.textContent = Math.round((num * i) / steps) + suffix;
      if (i >= steps) { clearInterval(tick); el.textContent = raw; }
    }, 26);
  }

  var io = new IntersectionObserver(function (entries) {
    entries.forEach(function (e) {
      if (!e.isIntersecting) return;
      grid.querySelectorAll('.imp-v').forEach(run);
      io.disconnect();
    });
  }, { threshold: 0.3 });
  io.observe(grid);
})();

/* ---------------------------------------------------------------------------
   Spend calculator. Deliberately simple and legible arithmetic — the point is
   to show where the money goes, not to quote anyone a price.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var q = document.getElementById('c-q');
  if (!q) return;
  var t = document.getElementById('c-t');
  var team = document.getElementById('c-team');

  // Published list prices per 1M tokens, blended input/output.
  var PREMIUM = 7.5;
  var SMALL = 0.45;

  // What a gateway actually buys you, and roughly what each lever is worth.
  var ROUTED_TO_SMALL = 0.62;   // simple queries a small model handles fine
  var CACHE_HIT = 0.18;         // repeated questions served without a call
  var PROMPT_TRIM = 0.12;       // bloat removed by a shared prompt library

  function money(n) {
    if (n >= 1000000) return '$' + (n / 1000000).toFixed(2) + 'M';
    if (n >= 1000) return '$' + Math.round(n / 1000) + 'k';
    return '$' + Math.round(n);
  }

  function render() {
    var queries = +q.value, tokens = +t.value, teams = +team.value;

    document.getElementById('c-q-o').textContent = queries.toLocaleString('en-US');
    document.getElementById('c-t-o').textContent = tokens.toLocaleString('en-US');
    document.getElementById('c-team-o').textContent = teams;

    var monthlyTokens = (queries * tokens * 30) / 1000000;

    // Ungoverned: premium model for everything, plus the duplicated effort of
    // each team solving the same problems in isolation.
    var before = monthlyTokens * PREMIUM * (1 + (teams - 1) * 0.03);

    // Governed: fewer calls, cheaper calls, smaller calls.
    var billable = monthlyTokens * (1 - CACHE_HIT) * (1 - PROMPT_TRIM);
    var after = billable * (ROUTED_TO_SMALL * SMALL + (1 - ROUTED_TO_SMALL) * PREMIUM);

    document.getElementById('c-before').textContent = money(before);
    document.getElementById('c-after').textContent = money(after);
    document.getElementById('c-save').textContent =
      Math.round(((before - after) / before) * 100) + '%';
  }

  [q, t, team].forEach(function (el) {
    el.addEventListener('input', render);
  });
  render();
})();
