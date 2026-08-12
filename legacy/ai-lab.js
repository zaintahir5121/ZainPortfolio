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
