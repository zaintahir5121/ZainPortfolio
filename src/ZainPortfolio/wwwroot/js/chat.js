(function () {
  'use strict';
  var widget = document.getElementById('chat-widget');
  if (!widget) return;

  var fab = document.getElementById('chat-fab');
  var closeBtn = document.getElementById('cw-close');
  var msgs = document.getElementById('cw-messages');
  var input = document.getElementById('cw-input');
  var send = document.getElementById('cw-send');
  var busy = false;

  function token() {
    var el = widget.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  }
  function scroll() { msgs.scrollTop = msgs.scrollHeight; }

  function bubble(text, who) {
    var wrap = document.createElement('div');
    wrap.className = 'cw-msg ' + who;
    var b = document.createElement('div');
    b.className = 'cw-bubble';
    // only **bold** is interpreted; everything else stays literal text
    var parts = String(text).split(/\*\*(.+?)\*\*/g);
    parts.forEach(function (p, i) {
      if (i % 2 === 1) { var s = document.createElement('strong'); s.textContent = p; b.appendChild(s); }
      else if (p) { b.appendChild(document.createTextNode(p)); }
    });
    wrap.appendChild(b);
    msgs.appendChild(wrap);
    scroll();
  }

  function links(list) {
    if (!list || !list.length) return;
    var box = document.createElement('div');
    box.className = 'cw-links';
    list.forEach(function (l) {
      var a = document.createElement('a');
      a.className = 'cw-link';
      a.href = l.url;
      if (/^https?:|^mailto:/.test(l.url)) { a.target = '_blank'; a.rel = 'noopener'; }
      a.innerHTML = '<i class="fas fa-arrow-right"></i>';
      a.appendChild(document.createTextNode(l.text));
      box.appendChild(a);
    });
    msgs.appendChild(box);
    scroll();
  }

  function quick(list) {
    document.querySelectorAll('.cw-quick-replies').forEach(function (n) { n.remove(); });
    if (!list || !list.length) return;
    var box = document.createElement('div');
    box.className = 'cw-quick-replies';
    list.forEach(function (q) {
      var b = document.createElement('button');
      b.type = 'button';
      b.className = 'cw-qr';
      b.textContent = q;
      b.addEventListener('click', function () { ask(q); });
      box.appendChild(b);
    });
    msgs.appendChild(box);
    scroll();
  }

  function typing(on) {
    var t = document.getElementById('cw-typing');
    if (on) {
      if (t) return;
      var wrap = document.createElement('div');
      wrap.className = 'cw-msg bot';
      wrap.id = 'cw-typing';
      wrap.innerHTML = '<div class="cw-bubble cw-typing"><span></span><span></span><span></span></div>';
      msgs.appendChild(wrap);
      scroll();
    } else if (t) { t.remove(); }
  }

  async function ask(question) {
    if (busy || !question || !question.trim()) return;
    busy = true;
    if (send) send.disabled = true;
    document.querySelectorAll('.cw-quick-replies').forEach(function (n) { n.remove(); });
    bubble(question, 'user');
    input.value = '';
    typing(true);

    try {
      var body = new URLSearchParams();
      body.append('question', question);
      body.append('__RequestVerificationToken', token());
      var res = await fetch('/chat/ask', {
        method: 'POST',
        body: body,
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
      });
      var data = await res.json();
      await new Promise(function (r) { setTimeout(r, 350); });   // brief pause reads as thinking
      typing(false);
      bubble(data.message, 'bot');
      links(data.links);
      quick(data.suggestions);
    } catch (e) {
      typing(false);
      bubble("Sorry — I couldn't reach the server. Please try again, or use the contact form.", 'bot');
    } finally {
      busy = false;
      if (send) send.disabled = false;
      input.focus();
    }
  }

  fab && fab.addEventListener('click', function () {
    var open = widget.classList.toggle('open');
    fab.setAttribute('aria-expanded', String(open));
    if (open) setTimeout(function () { input.focus(); }, 250);
  });
  closeBtn && closeBtn.addEventListener('click', function () {
    widget.classList.remove('open');
    fab.setAttribute('aria-expanded', 'false');
  });
  send && send.addEventListener('click', function () { ask(input.value); });
  input && input.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') { e.preventDefault(); ask(input.value); }
  });
  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && widget.classList.contains('open')) widget.classList.remove('open');
  });

  document.querySelectorAll('#cw-quick .cw-qr').forEach(function (b) {
    b.addEventListener('click', function () { ask(b.dataset.q || b.textContent); });
  });
})();
