'use strict';

let currentUser = null;
let allLogs     = [];

/* ══════════════════════════════════════
   Navigation
══════════════════════════════════════ */
function showSection(name) {
  document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
  document.getElementById(`section-${name}`).classList.add('active');
}

/* ══════════════════════════════════════
   Toast
══════════════════════════════════════ */
let _toastTimer = null;
function toast(msg, type = 'ok') {
  const el = document.getElementById('toast');
  el.textContent = msg;
  el.className   = `toast${type === 'err' ? ' err' : ''}`;
  clearTimeout(_toastTimer);
  _toastTimer = setTimeout(() => el.classList.add('hidden'), 3500);
}

/* ══════════════════════════════════════
   API helper
══════════════════════════════════════ */
function token()     { return localStorage.getItem('wl_token'); }
function saveToken(t){ localStorage.setItem('wl_token', t); }
function clearToken(){ localStorage.removeItem('wl_token'); }

async function api(path, opts = {}) {
  const t   = token();
  const res = await fetch(path, {
    ...opts,
    headers: {
      'Content-Type': 'application/json',
      ...(t ? { Authorization: `Bearer ${t}` } : {}),
      ...opts.headers,
    },
  });
  const data = await res.json();
  if (!res.ok) throw new Error(data.error || 'Request failed');
  return data;
}

/* ══════════════════════════════════════
   Auth
══════════════════════════════════════ */
async function handleLogin(e) {
  e.preventDefault();
  const username = document.getElementById('l-user').value.trim();
  const password = document.getElementById('l-pass').value;
  const errEl    = document.getElementById('login-err');
  const btn      = document.getElementById('login-submit');

  errEl.classList.add('hidden');
  btn.disabled    = true;
  btn.innerHTML   = '<i class="fas fa-spinner spin"></i> Signing in…';

  try {
    const { token: t, user } = await api('/api/login', {
      method: 'POST',
      body: JSON.stringify({ username, password }),
    });
    saveToken(t);
    currentUser = user;
    initDashboard();
    showSection('dashboard');
  } catch (err) {
    errEl.textContent = err.message;
    errEl.classList.remove('hidden');
  } finally {
    btn.disabled  = false;
    btn.innerHTML = 'Sign In <i class="fas fa-sign-in-alt"></i>';
  }
}

async function handleLogout() {
  try { await api('/api/logout', { method: 'POST' }); } catch (_) {}
  clearToken();
  currentUser = null;
  allLogs     = [];
  showSection('landing');
}

function fillDemo(u, p) {
  document.getElementById('l-user').value = u;
  document.getElementById('l-pass').value = p;
}

/* ══════════════════════════════════════
   Dashboard bootstrap
══════════════════════════════════════ */
function initDashboard() {
  const nameEl = document.getElementById('d-user-name');
  const badge  = document.getElementById('d-user-role');
  nameEl.textContent = currentUser.name;
  badge.textContent  = currentUser.role;
  badge.className    = `user-badge ${currentUser.role}`;
  document.getElementById('f-date').value = todayStr();
  fetchLogs();
}

function todayStr() {
  const d = new Date();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${d.getFullYear()}-${mm}-${dd}`;
}

/* ══════════════════════════════════════
   Logs — fetch & render
══════════════════════════════════════ */
async function fetchLogs() {
  try {
    allLogs = await api('/api/logs');
    renderLogs(allLogs);
    updateStats(allLogs);
  } catch (err) {
    toast('Failed to load logs: ' + err.message, 'err');
  }
}

function updateStats(logs) {
  const t         = todayStr();
  const todayLogs = logs.filter(l => l.date === t);
  const todayHrs  = todayLogs.reduce((s, l) => s + l.hours, 0);

  const weekStart = new Date();
  weekStart.setDate(weekStart.getDate() - weekStart.getDay());
  weekStart.setHours(0, 0, 0, 0);
  const weekLogs  = logs.filter(l => new Date(l.date + 'T00:00:00') >= weekStart);

  const projects  = new Set(logs.map(l => l.project));

  document.getElementById('s-today').textContent    = todayHrs ? `${todayHrs}h` : '0h';
  document.getElementById('s-week').textContent     = weekLogs.length;
  document.getElementById('s-projects').textContent = projects.size;
}

function renderLogs(logs) {
  const el = document.getElementById('logs-list');
  if (!logs.length) {
    el.innerHTML = `<div class="empty-state">
      <i class="fas fa-clipboard-list"></i>
      <p>No logs found. Add your first work entry!</p>
    </div>`;
    return;
  }

  const isAdmin = currentUser.role === 'admin';
  el.innerHTML  = logs.map(log => {
    const canDel  = currentUser.id === log.userId || isAdmin;
    const tagsHtml = (log.tags || []).length
      ? `<div class="log-tags">${log.tags.map(t => `<span class="tag">${esc(t)}</span>`).join('')}</div>`
      : '';

    return `<div class="log-item" data-id="${log.id}">
      <div class="log-top">
        <div class="log-meta">
          <div class="log-project">${esc(log.project)}</div>
          ${isAdmin ? `<div class="log-user"><i class="fas fa-user" style="font-size:9px;margin-right:4px"></i>${esc(log.userName)}</div>` : ''}
        </div>
        <div class="log-side">
          <div>
            <div class="log-hours">${log.hours}h</div>
            <div class="log-date">${fmtDate(log.date)}</div>
          </div>
          ${canDel ? `<button class="del-btn" onclick="deleteLog('${log.id}')" title="Delete entry">
            <i class="fas fa-trash"></i>
          </button>` : ''}
        </div>
      </div>
      <div class="log-desc">${esc(log.description)}</div>
      ${tagsHtml}
    </div>`;
  }).join('');
}

function filterLogs(query) {
  const period = document.getElementById('log-period').value;
  const t      = todayStr();

  let filtered = allLogs.filter(log => {
    const d = new Date(log.date + 'T00:00:00');
    if (period === 'today') {
      return log.date === t;
    } else if (period === 'week') {
      const ws = new Date(); ws.setDate(ws.getDate() - ws.getDay()); ws.setHours(0,0,0,0);
      return d >= ws;
    } else if (period === 'month') {
      const ms = new Date(); ms.setDate(1); ms.setHours(0,0,0,0);
      return d >= ms;
    }
    return true;
  });

  if (query.trim()) {
    const q = query.toLowerCase();
    filtered = filtered.filter(l =>
      l.project.toLowerCase().includes(q) ||
      l.description.toLowerCase().includes(q) ||
      (l.tags || []).some(tg => tg.toLowerCase().includes(q))
    );
  }

  renderLogs(filtered);
}

/* ══════════════════════════════════════
   Add / delete log
══════════════════════════════════════ */
async function handleAddLog(e) {
  e.preventDefault();
  const errEl = document.getElementById('log-err');
  errEl.classList.add('hidden');

  const body = {
    date:        document.getElementById('f-date').value,
    hours:       document.getElementById('f-hours').value,
    project:     document.getElementById('f-project').value.trim(),
    description: document.getElementById('f-desc').value.trim(),
    tags:        document.getElementById('f-tags').value.trim(),
  };

  try {
    const log = await api('/api/logs', { method: 'POST', body: JSON.stringify(body) });
    allLogs.unshift(log);
    renderLogs(allLogs);
    updateStats(allLogs);
    document.getElementById('log-form').reset();
    document.getElementById('f-date').value = todayStr();
    toast('Work log added!');
  } catch (err) {
    errEl.textContent = err.message;
    errEl.classList.remove('hidden');
  }
}

async function deleteLog(id) {
  if (!confirm('Delete this log entry?')) return;
  try {
    await api(`/api/logs/${id}`, { method: 'DELETE' });
    allLogs = allLogs.filter(l => l.id !== id);
    renderLogs(allLogs);
    updateStats(allLogs);
    toast('Log deleted');
  } catch (err) {
    toast('Delete failed: ' + err.message, 'err');
  }
}

/* ══════════════════════════════════════
   AI features
══════════════════════════════════════ */
async function improveDesc() {
  const ta   = document.getElementById('f-desc');
  const btn  = document.getElementById('ai-improve-btn');
  const text = ta.value.trim();
  if (!text) { toast('Write a description first', 'err'); return; }

  btn.disabled  = true;
  btn.innerHTML = '<i class="fas fa-spinner spin"></i>';

  try {
    const { result } = await api('/api/ai/improve', {
      method: 'POST', body: JSON.stringify({ text }),
    });
    ta.value = result;
    toast('Description improved by AI!');
  } catch (err) {
    toast(err.message, 'err');
  } finally {
    btn.disabled  = false;
    btn.innerHTML = '<i class="fas fa-magic"></i> AI';
  }
}

async function genAiSummary() {
  const t         = todayStr();
  const todayLogs = allLogs.filter(l => l.date === t);
  if (!todayLogs.length) { toast("No logs for today yet", 'err'); return; }

  const btn = document.getElementById('ai-sum-btn');
  btn.disabled  = true;
  btn.innerHTML = '<i class="fas fa-spinner spin"></i> Generating…';

  try {
    const { result } = await api('/api/ai/summary', {
      method: 'POST', body: JSON.stringify({ logs: todayLogs }),
    });
    document.getElementById('ai-box-body').textContent = result;
    const box = document.getElementById('ai-box');
    box.classList.remove('hidden');
    box.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  } catch (err) {
    toast(err.message, 'err');
  } finally {
    btn.disabled  = false;
    btn.innerHTML = '<i class="fas fa-magic"></i> AI Summary';
  }
}

/* ══════════════════════════════════════
   Utilities
══════════════════════════════════════ */
function esc(str) {
  const d = document.createElement('div');
  d.appendChild(document.createTextNode(String(str)));
  return d.innerHTML;
}

function fmtDate(ds) {
  return new Date(ds + 'T00:00:00').toLocaleDateString('en-US',
    { month: 'short', day: 'numeric', year: 'numeric' });
}

/* ══════════════════════════════════════
   Boot — restore session on refresh
══════════════════════════════════════ */
(function boot() {
  if (!token()) { showSection('landing'); return; }

  api('/api/me')
    .then(user => {
      currentUser = user;
      initDashboard();
      showSection('dashboard');
    })
    .catch(() => {
      clearToken();
      showSection('landing');
    });
})();
