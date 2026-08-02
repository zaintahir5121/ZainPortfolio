(function () {
  'use strict';

  function token() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  }

  document.querySelectorAll('.editor-toolbar').forEach(function (bar) {
    var editor = document.getElementById(bar.dataset.for);
    if (!editor) return;
    var hidden = document.getElementById('input-' + bar.dataset.for);

    function sync() { if (hidden) hidden.value = editor.innerHTML; }
    editor.addEventListener('input', sync);
    editor.addEventListener('blur', sync);
    sync();

    function insert(html) {
      editor.focus();
      document.execCommand('insertHTML', false, html);
      sync();
    }

    bar.querySelectorAll('button').forEach(function (btn) {
      btn.addEventListener('click', function () {
        var cmd = btn.dataset.cmd;
        var action = btn.dataset.action;

        if (cmd) {
          editor.focus();
          document.execCommand(cmd, false, btn.dataset.val || null);
          sync();
          return;
        }

        if (action === 'link') {
          var url = prompt('Link URL:', 'https://');
          if (url) { editor.focus(); document.execCommand('createLink', false, url); sync(); }
        }

        if (action === 'image') {
          openPicker('image', function (url) { insert('<img src="' + url + '" alt="" loading="lazy">'); });
        }

        if (action === 'video') {
          var v = prompt('YouTube URL or video ID:');
          if (!v) return;
          var id = v.trim();
          ['shorts/', 'embed/', 'v=', 'youtu.be/'].forEach(function (p) {
            var i = id.indexOf(p);
            if (i >= 0) { id = id.substring(i + p.length); var c = id.search(/[?&/#]/); if (c > 0) id = id.substring(0, c); }
          });
          insert('<iframe src="https://www.youtube.com/embed/' + id + '" allowfullscreen loading="lazy"></iframe>');
        }

        if (action === 'upload') {
          var input = document.createElement('input');
          input.type = 'file';
          input.accept = 'image/*';
          input.onchange = async function () {
            if (!input.files.length) return;
            var fd = new FormData();
            fd.append('file', input.files[0]);
            fd.append('__RequestVerificationToken', token());
            try {
              var res = await fetch('/admin/media/upload-ajax', { method: 'POST', body: fd });
              var json = await res.json();
              if (json.ok) insert('<img src="' + json.url + '" alt="" loading="lazy">');
              else alert(json.message || 'Upload failed.');
            } catch (e) { alert('Upload failed.'); }
          };
          input.click();
        }

        if (action === 'html') {
          var raw = prompt('Edit raw HTML:', editor.innerHTML);
          if (raw !== null) { editor.innerHTML = raw; sync(); }
        }
      });
    });

    // keep hidden field current on submit
    var form = editor.closest('form');
    if (form) form.addEventListener('submit', sync);
  });

  /* ---------------------------------------- media picker modal */
  window.openPicker = async function (kind, onPick) {
    var overlay = document.createElement('div');
    overlay.style.cssText = 'position:fixed;inset:0;background:rgba(0,0,0,.72);z-index:9999;display:grid;place-items:center;padding:2rem;';
    overlay.innerHTML =
      '<div style="background:#161a25;border:1px solid rgba(255,255,255,.09);border-radius:14px;max-width:820px;width:100%;max-height:80vh;overflow:auto;padding:1.5rem;">' +
      '<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:1rem;">' +
      '<h3 style="font-size:1rem;">Select from media library</h3>' +
      '<button type="button" id="pk-close" style="color:#8b95ab;font-size:1.2rem;">&times;</button></div>' +
      '<div id="pk-grid" class="media-grid"><p style="color:#8b95ab;">Loading…</p></div></div>';
    document.body.appendChild(overlay);

    function close() { overlay.remove(); }
    overlay.addEventListener('click', function (e) { if (e.target === overlay) close(); });
    overlay.querySelector('#pk-close').addEventListener('click', close);

    try {
      var res = await fetch('/admin/media/picker?kind=' + (kind || 'all'));
      var items = await res.json();
      var grid = overlay.querySelector('#pk-grid');
      if (!items.length) { grid.innerHTML = '<p style="color:#8b95ab;">No files yet — upload some in the Media Library.</p>'; return; }
      grid.innerHTML = '';
      items.forEach(function (m) {
        var cell = document.createElement('div');
        cell.className = 'media-item';
        cell.style.cursor = 'pointer';
        cell.innerHTML =
          '<div class="media-thumb">' +
          (m.kind === 'image'
            ? '<img src="' + m.url + '" alt="">'
            : '<i class="fas fa-' + (m.kind === 'video' ? 'film' : 'file') + '"></i>') +
          '</div><div class="media-info"><div class="nm">' + m.originalName + '</div></div>';
        cell.addEventListener('click', function () { onPick(m.url); close(); });
        grid.appendChild(cell);
      });
    } catch (e) {
      overlay.querySelector('#pk-grid').innerHTML = '<p style="color:#fca5a5;">Could not load the media library.</p>';
    }
  };

  /* ---------------------------------------- "pick image" buttons on plain fields */
  document.querySelectorAll('[data-pick-for]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var target = document.getElementById(btn.dataset.pickFor);
      openPicker(btn.dataset.pickKind || 'image', function (url) {
        if (target) {
          target.value = url;
          target.dispatchEvent(new Event('input'));
        }
      });
    });
  });

  /* ---------------------------------------- slug auto-fill */
  var titleEl = document.getElementById('Title');
  var slugEl = document.getElementById('Slug');
  if (titleEl && slugEl) {
    titleEl.addEventListener('input', function () {
      if (slugEl.dataset.touched === 'true' || slugEl.value.trim() !== '') return;
      slugEl.placeholder = titleEl.value.toLowerCase()
        .replace(/&/g, ' and ').replace(/[^a-z0-9\s-]/g, '')
        .replace(/[\s-]+/g, '-').replace(/^-|-$/g, '');
    });
    slugEl.addEventListener('input', function () { slugEl.dataset.touched = 'true'; });
  }
})();
