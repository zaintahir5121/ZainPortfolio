'use strict';

const express = require('express');
const cors    = require('cors');
const crypto  = require('crypto');
const fs      = require('fs');
const path    = require('path');

const app  = express();
const PORT  = process.env.PORT         || 3001;
const OLLAMA_URL   = process.env.OLLAMA_URL   || 'http://localhost:11434';
const OLLAMA_MODEL = process.env.OLLAMA_MODEL || 'llama3.2';

const DATA_DIR   = path.join(__dirname, 'data');
const LOGS_FILE  = path.join(DATA_DIR, 'logs.json');
const USERS_FILE = path.join(DATA_DIR, 'users.json');

if (!fs.existsSync(DATA_DIR)) fs.mkdirSync(DATA_DIR, { recursive: true });

if (!fs.existsSync(USERS_FILE)) {
  fs.writeFileSync(USERS_FILE, JSON.stringify([
    { id: 1, username: 'admin',  password: 'admin123', name: 'Admin',     role: 'admin' },
    { id: 2, username: 'john',   password: 'pass123',  name: 'John Doe',  role: 'employee' },
    { id: 3, username: 'sara',   password: 'pass123',  name: 'Sara Khan', role: 'employee' },
  ], null, 2));
}

if (!fs.existsSync(LOGS_FILE)) {
  fs.writeFileSync(LOGS_FILE, JSON.stringify([], null, 2));
}

const sessions = new Map();

app.use(cors());
app.use(express.json());
app.use(express.static(__dirname));
app.get('/', (_, res) => res.sendFile(path.join(__dirname, 'index.html')));

const rj = (file)       => JSON.parse(fs.readFileSync(file, 'utf8'));
const wj = (file, data) => fs.writeFileSync(file, JSON.stringify(data, null, 2));

function auth(req, res, next) {
  const token = req.headers.authorization?.split(' ')[1];
  if (!token || !sessions.has(token)) return res.status(401).json({ error: 'Unauthorized' });
  req.user = sessions.get(token);
  next();
}

/* ── Auth ── */
app.post('/api/login', (req, res) => {
  const { username, password } = req.body || {};
  if (!username || !password) return res.status(400).json({ error: 'Missing credentials' });

  const user = rj(USERS_FILE).find(u => u.username === username && u.password === password);
  if (!user) return res.status(401).json({ error: 'Invalid username or password' });

  const token = crypto.randomBytes(32).toString('hex');
  const session = { id: user.id, username: user.username, name: user.name, role: user.role };
  sessions.set(token, session);
  res.json({ token, user: session });
});

app.post('/api/logout', auth, (req, res) => {
  sessions.delete(req.headers.authorization.split(' ')[1]);
  res.json({ ok: true });
});

app.get('/api/me', auth, (req, res) => res.json(req.user));

/* ── Logs ── */
app.get('/api/logs', auth, (req, res) => {
  const all = rj(LOGS_FILE);
  const logs = req.user.role === 'admin' ? all : all.filter(l => l.userId === req.user.id);
  res.json(logs.sort((a, b) => new Date(b.date) - new Date(a.date)));
});

app.post('/api/logs', auth, (req, res) => {
  const { date, project, description, hours, tags } = req.body || {};
  if (!date || !project || !description || !hours)
    return res.status(400).json({ error: 'date, project, description and hours are required' });

  const log = {
    id:          Date.now().toString(),
    userId:      req.user.id,
    userName:    req.user.name,
    date,
    project:     project.trim(),
    description: description.trim(),
    hours:       parseFloat(hours),
    tags:        tags ? tags.split(',').map(t => t.trim()).filter(Boolean) : [],
    createdAt:   new Date().toISOString(),
  };

  const logs = rj(LOGS_FILE);
  logs.push(log);
  wj(LOGS_FILE, logs);
  res.status(201).json(log);
});

app.delete('/api/logs/:id', auth, (req, res) => {
  const logs = rj(LOGS_FILE);
  const idx  = logs.findIndex(l => l.id === req.params.id);
  if (idx === -1) return res.status(404).json({ error: 'Not found' });

  const log = logs[idx];
  if (log.userId !== req.user.id && req.user.role !== 'admin')
    return res.status(403).json({ error: 'Forbidden' });

  logs.splice(idx, 1);
  wj(LOGS_FILE, logs);
  res.json({ ok: true });
});

/* ── Ollama AI ── */
async function ollama(prompt, timeoutMs = 40000) {
  const resp = await fetch(`${OLLAMA_URL}/api/generate`, {
    method:  'POST',
    headers: { 'Content-Type': 'application/json' },
    body:    JSON.stringify({ model: OLLAMA_MODEL, prompt, stream: false }),
    signal:  AbortSignal.timeout(timeoutMs),
  });
  if (!resp.ok) throw new Error(`Ollama HTTP ${resp.status}`);
  const data = await resp.json();
  return data.response?.trim();
}

app.post('/api/ai/improve', auth, async (req, res) => {
  const { text } = req.body || {};
  if (!text) return res.status(400).json({ error: 'text required' });

  const prompt = `You are a professional work-log assistant. Rewrite the following work description to be clear, concise, and professional. Return ONLY the improved text with no extra explanation or quotes:\n\n${text}`;

  try {
    res.json({ result: await ollama(prompt) });
  } catch {
    res.status(503).json({ error: 'AI unavailable — make sure Ollama is running and the model is pulled.' });
  }
});

app.post('/api/ai/summary', auth, async (req, res) => {
  const { logs } = req.body || {};
  if (!logs?.length) return res.status(400).json({ error: 'logs required' });

  const list = logs.map(l => `• [${l.project}] ${l.description} (${l.hours}h)`).join('\n');
  const prompt = `Write a brief, professional end-of-day work summary (3–4 sentences) based on these log entries:\n\n${list}`;

  try {
    res.json({ result: await ollama(prompt, 60000) });
  } catch {
    res.status(503).json({ error: 'AI unavailable — make sure Ollama is running and the model is pulled.' });
  }
});

app.listen(PORT, () => {
  console.log(`\n  WorkLog  →  http://localhost:${PORT}`);
  console.log(`  Ollama   →  ${OLLAMA_URL}  (model: ${OLLAMA_MODEL})\n`);
});
