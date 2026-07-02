/* ===========================
   AI PALM READER
   100% client-side: nothing is ever uploaded. The "reading" is a
   deterministic, seeded analysis of the photo's own pixel data, so the
   same photo always produces the same result. It's a fun UI/camera/canvas
   demo, not real palmistry or predictive science.
=========================== */

/* ---------- THEME TOGGLE ---------- */
(function () {
  const themeBtn = document.getElementById('theme-btn');
  const themeIcon = document.getElementById('theme-icon');
  const html = document.documentElement;
  const saved = localStorage.getItem('theme') || 'dark';
  html.setAttribute('data-theme', saved);
  themeIcon.className = saved === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
  themeBtn.addEventListener('click', () => {
    const next = html.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-theme', next);
    localStorage.setItem('theme', next);
    themeIcon.className = next === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
  });
})();

document.getElementById('pr-year').textContent = new Date().getFullYear();

/* ---------- ELEMENTS ---------- */
const tabs = document.querySelectorAll('.pr-tab');
const cameraWrap = document.getElementById('pr-camera-wrap');
const uploadWrap = document.getElementById('pr-upload-wrap');
const previewWrap = document.getElementById('pr-preview-wrap');

const video = document.getElementById('pr-video');
const camStart = document.getElementById('pr-cam-start');
const camCapture = document.getElementById('pr-cam-capture');
const camSwitch = document.getElementById('pr-cam-switch');
const camMsg = document.getElementById('pr-cam-msg');
const handGuide = document.getElementById('pr-hand-guide');

const dropzone = document.getElementById('pr-dropzone');
const fileInput = document.getElementById('pr-file-input');

const previewImg = document.getElementById('pr-preview-img');
const scanOverlay = document.getElementById('pr-scan-overlay');
const analyzingMsg = document.getElementById('pr-analyzing');
const previewActions = document.getElementById('pr-preview-actions');
const retakeBtn = document.getElementById('pr-retake');
const analyzeBtn = document.getElementById('pr-analyze');

const emptyCard = document.getElementById('pr-empty');
const resultsCard = document.getElementById('pr-results');
const overallEl = document.getElementById('pr-overall');
const linesEl = document.getElementById('pr-lines');
const againBtn = document.getElementById('pr-again');
const shareBtn = document.getElementById('pr-share');

const workCanvas = document.getElementById('pr-work-canvas');
const workCtx = workCanvas.getContext('2d', { willReadFrequently: true });

let currentStream = null;
let currentFacing = 'environment';
let lastImageDataUrl = null;
let lastMode = 'camera';

/* ---------- TAB SWITCHING ---------- */
tabs.forEach(tab => {
  tab.addEventListener('click', () => {
    tabs.forEach(t => t.classList.remove('active'));
    tab.classList.add('active');
    lastMode = tab.dataset.mode;
    resetToStage(lastMode);
  });
});

function resetToStage(mode) {
  stopCamera();
  previewWrap.classList.add('hidden');
  cameraWrap.classList.toggle('hidden', mode !== 'camera');
  uploadWrap.classList.toggle('hidden', mode !== 'upload');
  camCapture.classList.add('hidden');
  camSwitch.classList.add('hidden');
  camStart.classList.remove('hidden');
  camMsg.textContent = '';
}

/* ---------- CAMERA ---------- */
camStart.addEventListener('click', () => startCamera());
camSwitch.addEventListener('click', () => {
  currentFacing = currentFacing === 'environment' ? 'user' : 'environment';
  startCamera();
});
camCapture.addEventListener('click', capturePhoto);

async function startCamera() {
  camMsg.textContent = '';
  if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
    camMsg.textContent = 'Camera access is not supported in this browser. Try uploading a photo instead.';
    return;
  }
  stopCamera();
  try {
    currentStream = await navigator.mediaDevices.getUserMedia({
      video: { facingMode: currentFacing },
      audio: false
    });
    video.srcObject = currentStream;
    camStart.classList.add('hidden');
    camCapture.classList.remove('hidden');
    camSwitch.classList.remove('hidden');
  } catch (err) {
    camMsg.textContent = 'Could not access your camera (permission denied or unavailable). Try uploading a photo instead.';
  }
}

function stopCamera() {
  if (currentStream) {
    currentStream.getTracks().forEach(t => t.stop());
    currentStream = null;
  }
  video.srcObject = null;
}

function capturePhoto() {
  if (!video.videoWidth) return;
  workCanvas.width = video.videoWidth;
  workCanvas.height = video.videoHeight;
  workCtx.drawImage(video, 0, 0, workCanvas.width, workCanvas.height);
  showPreview(workCanvas.toDataURL('image/jpeg', 0.92));
  stopCamera();
}

/* ---------- UPLOAD ---------- */
dropzone.addEventListener('click', () => fileInput.click());
fileInput.addEventListener('change', () => {
  if (fileInput.files && fileInput.files[0]) handleFile(fileInput.files[0]);
});
['dragover', 'dragenter'].forEach(evt =>
  dropzone.addEventListener(evt, e => { e.preventDefault(); dropzone.classList.add('dragover'); })
);
['dragleave', 'drop'].forEach(evt =>
  dropzone.addEventListener(evt, e => { e.preventDefault(); dropzone.classList.remove('dragover'); })
);
dropzone.addEventListener('drop', e => {
  const file = e.dataTransfer.files && e.dataTransfer.files[0];
  if (file) handleFile(file);
});

function handleFile(file) {
  if (!file.type.startsWith('image/')) return;
  const reader = new FileReader();
  reader.onload = e => showPreview(e.target.result);
  reader.readAsDataURL(file);
}

/* ---------- PREVIEW / RETAKE ---------- */
function showPreview(dataUrl) {
  lastImageDataUrl = dataUrl;
  previewImg.src = dataUrl;
  cameraWrap.classList.add('hidden');
  uploadWrap.classList.add('hidden');
  previewWrap.classList.remove('hidden');
  scanOverlay.classList.add('hidden');
  analyzingMsg.classList.add('hidden');
  previewActions.classList.remove('hidden');
}

retakeBtn.addEventListener('click', () => {
  resetToStage(lastMode);
});

/* ---------- ANALYZE ---------- */
analyzeBtn.addEventListener('click', () => {
  scanOverlay.classList.remove('hidden');
  analyzingMsg.classList.remove('hidden');
  previewActions.classList.add('hidden');

  const img = new Image();
  img.onload = () => {
    const reading = analyzeImage(img);
    setTimeout(() => renderResults(reading), 2200);
  };
  img.src = lastImageDataUrl;
});

againBtn.addEventListener('click', () => {
  resultsCard.classList.add('hidden');
  emptyCard.classList.remove('hidden');
  tabs.forEach(t => t.classList.toggle('active', t.dataset.mode === 'camera'));
  lastMode = 'camera';
  resetToStage('camera');
  window.scrollTo({ top: document.getElementById('pr-capture-card').offsetTop - 100, behavior: 'smooth' });
});

shareBtn.addEventListener('click', async () => {
  const text = `I just tried the AI Palm Reader demo${overallEl.textContent ? ': "' + overallEl.textContent + '"' : ''} — check it out!`;
  if (navigator.share) {
    try { await navigator.share({ title: 'AI Palm Reader', text, url: location.href }); } catch (e) { /* user cancelled */ }
  } else {
    try {
      await navigator.clipboard.writeText(`${text} ${location.href}`);
      const original = shareBtn.innerHTML;
      shareBtn.innerHTML = '<i class="fas fa-check"></i> Copied!';
      setTimeout(() => { shareBtn.innerHTML = original; }, 1800);
    } catch (e) { /* clipboard unavailable */ }
  }
});

/* ---------- IMAGE ANALYSIS ----------
   Downsamples the photo to a small grid, hashes its pixel data to seed a
   deterministic PRNG, and measures per-region brightness/contrast. Region
   stats bias which line "traits" get picked so the reading is tied to the
   actual photo rather than being pure random noise. */
function analyzeImage(img) {
  const SIZE = 64;
  workCanvas.width = SIZE;
  workCanvas.height = SIZE;
  workCtx.drawImage(img, 0, 0, SIZE, SIZE);
  const data = workCtx.getImageData(0, 0, SIZE, SIZE).data;

  let hash = 2166136261;
  const regionSums = { tl: [], tr: [], bl: [], br: [], mid: [] };
  for (let y = 0; y < SIZE; y++) {
    for (let x = 0; x < SIZE; x++) {
      const i = (y * SIZE + x) * 4;
      const lum = 0.299 * data[i] + 0.587 * data[i + 1] + 0.114 * data[i + 2];
      hash ^= data[i] + data[i + 1] * 2 + data[i + 2] * 3 + x + y;
      hash = Math.imul(hash, 16777619);

      const half = SIZE / 2;
      if (x < half && y < half) regionSums.tl.push(lum);
      else if (x >= half && y < half) regionSums.tr.push(lum);
      else if (x < half && y >= half) regionSums.bl.push(lum);
      else regionSums.br.push(lum);
      if (x > SIZE * 0.35 && x < SIZE * 0.65) regionSums.mid.push(lum);
    }
  }

  const stats = key => {
    const arr = regionSums[key];
    const mean = arr.reduce((a, b) => a + b, 0) / arr.length;
    const variance = arr.reduce((a, b) => a + (b - mean) * (b - mean), 0) / arr.length;
    return { mean, contrast: Math.sqrt(variance) };
  };

  const seed = (hash >>> 0) || 1;
  const rand = mulberry32(seed);

  const regions = { heart: stats('tl'), head: stats('mid'), life: stats('bl'), fate: stats('tr') };
  const strength = region => {
    const norm = Math.min(1, region.contrast / 70);
    const jitter = (rand() - 0.5) * 0.25;
    return Math.round(clamp(0.42 + norm * 0.4 + jitter, 0.3, 0.97) * 100);
  };

  const lines = LINE_DEFS.map(def => {
    const pct = strength(regions[def.key]);
    const tier = pct >= 72 ? 'high' : pct >= 52 ? 'mid' : 'low';
    const variants = def.copy[tier];
    const text = variants[Math.floor(rand() * variants.length)];
    return { ...def, pct, text };
  });

  const sorted = [...lines].sort((a, b) => b.pct - a.pct);
  const overallTpl = OVERALL_TEMPLATES[Math.floor(rand() * OVERALL_TEMPLATES.length)];
  const overall = overallTpl(sorted[0], sorted[1]);

  return { lines, overall };
}

function clamp(v, min, max) { return Math.max(min, Math.min(max, v)); }

function mulberry32(a) {
  return function () {
    a |= 0; a = (a + 0x6D2B79F5) | 0;
    let t = Math.imul(a ^ (a >>> 15), 1 | a);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

/* ---------- CONTENT LIBRARY ---------- */
const LINE_DEFS = [
  {
    key: 'heart', icon: 'fa-heart', name: 'Heart Line',
    copy: {
      low: [
        'A subtle, fine heart line — you tend to guard your feelings and open up slowly, but deeply once trust is earned.',
        'Your heart line reads gentle and understated, suggesting a quiet, reserved emotional style rather than an outward one.',
        'A soft, light heart line points to someone thoughtful with affection — careful before you invest your heart.'
      ],
      mid: [
        'A balanced heart line — warm and expressive, but grounded. You care deeply without losing perspective.',
        'Your heart line shows steady emotional footing: affectionate, loyal, and fair in matters of the heart.',
        'A well-formed heart line suggests you balance empathy with healthy boundaries in relationships.'
      ],
      high: [
        'A bold, deep heart line — intensely passionate, generous with affection, and led strongly by your emotions.',
        'Your heart line is strong and pronounced, a sign of a big-hearted, expressive, all-in kind of lover and friend.',
        'A vivid heart line suggests powerful emotional depth — you feel things fully and love fiercely.'
      ]
    }
  },
  {
    key: 'head', icon: 'fa-brain', name: 'Head Line',
    copy: {
      low: [
        'A light, curved head line — imaginative and intuitive, you often trust gut feeling over pure logic.',
        'Your head line suggests a free-flowing, creative thinking style that resists rigid step-by-step reasoning.',
        'A softer head line points to a dreamer’s mind — big-picture thinking over granular detail.'
      ],
      mid: [
        'A clear, moderate head line — you think practically but stay open to creative solutions when needed.',
        'Your head line shows balanced reasoning: analytical enough to plan, flexible enough to adapt.',
        'A steady head line suggests a mind that weighs both logic and intuition before deciding.'
      ],
      high: [
        'A long, sharply etched head line — a focused, analytical mind that thrives on solving hard problems.',
        'Your head line is deep and well-defined, a sign of strong concentration and decisive, structured thinking.',
        'A strong head line points to sharp logic and ambition — you plan carefully and execute with clarity.'
      ]
    }
  },
  {
    key: 'life', icon: 'fa-seedling', name: 'Life Line',
    copy: {
      low: [
        'A slender life line — traditionally linked to a calmer, low-key pace rather than raw physical energy.',
        'Your life line reads light and close to the thumb, often associated with a measured, steady approach to life.',
        'A faint life line suggests a preference for stability and routine over constant high-energy change.'
      ],
      mid: [
        'A well-balanced life line — steady vitality paired with a healthy respect for rest and recovery.',
        'Your life line shows a good rhythm between ambition and self-care — energetic but not reckless.',
        'A moderate, even life line points to resilience built on consistency rather than bursts of intensity.'
      ],
      high: [
        'A deep, sweeping life line — traditionally tied to strong vitality, stamina, and a zest for new experiences.',
        'Your life line curves wide and bold, a sign of robust energy and an adventurous approach to living.',
        'A pronounced life line suggests resilience and drive — you bounce back fast and keep moving forward.'
      ]
    }
  },
  {
    key: 'fate', icon: 'fa-star', name: 'Fate Line',
    copy: {
      low: [
        'A faint or broken fate line — your path isn’t fixed; you shape your direction as you go rather than following one set track.',
        'Your fate line is subtle, often read as a sign of independence — you write your own story instead of following a script.',
        'A light fate line suggests flexibility — open to pivots, reinvention, and unconventional paths.'
      ],
      mid: [
        'A visible, steady fate line — a good sign of purposeful progress with room to adapt along the way.',
        'Your fate line shows a workable balance of direction and flexibility — goal-oriented but not rigid.',
        'A clear fate line suggests you’re building toward something specific, one deliberate step at a time.'
      ],
      high: [
        'A strong, unbroken fate line — traditionally read as strong ambition and a clear, driven sense of purpose.',
        'Your fate line runs deep and straight, a classic sign of focus, discipline, and long-term vision.',
        'A bold fate line suggests you’re highly goal-driven — once you commit to a direction, you follow through.'
      ]
    }
  }
];

const OVERALL_TEMPLATES = [
  (top, second) => `Your photo shows the strongest signal in your ${top.name.toLowerCase()}, hinting that ${firstLower(top.text)} Paired with a notable ${second.name.toLowerCase()}, the overall picture leans toward someone who leads with ${top.key === 'heart' ? 'emotion' : top.key === 'head' ? 'logic' : top.key === 'life' ? 'energy' : 'purpose'} and backs it up with ${second.key === 'heart' ? 'genuine warmth' : second.key === 'head' ? 'clear thinking' : second.key === 'life' ? 'steady resilience' : 'quiet ambition'}.`,
  (top, second) => `The dominant marking here is your ${top.name.toLowerCase()}. Combined with a strong ${second.name.toLowerCase()}, this reading points to a personality that’s ${top.key === 'heart' ? 'led by the heart' : top.key === 'head' ? 'led by the mind' : top.key === 'life' ? 'full of energy' : 'quietly determined'} — grounded by a good dose of ${second.key === 'heart' ? 'empathy' : second.key === 'head' ? 'reason' : second.key === 'life' ? 'stamina' : 'focus'}.`,
  (top, second) => `Your ${top.name.toLowerCase()} stands out most in this photo, and your ${second.name.toLowerCase()} reinforces it — together suggesting someone who is both ${lineTrait(top.key)} and ${lineTrait(second.key)}.`
];

function firstLower(s) { return s.charAt(0).toLowerCase() + s.slice(1); }
function lineTrait(key) {
  return { heart: 'warmly emotional', head: 'sharply analytical', life: 'full of vitality', fate: 'driven by purpose' }[key];
}

/* ---------- RENDER RESULTS ---------- */
function renderResults(reading) {
  scanOverlay.classList.add('hidden');
  analyzingMsg.classList.add('hidden');
  previewActions.classList.remove('hidden');

  emptyCard.classList.add('hidden');
  resultsCard.classList.remove('hidden');
  overallEl.textContent = reading.overall;

  linesEl.innerHTML = reading.lines.map(l => `
    <div class="pr-line-card">
      <div class="pr-line-top">
        <div class="pr-line-name"><i class="fas ${l.icon}"></i> ${l.name}</div>
        <div class="pr-line-pct">${l.pct}%</div>
      </div>
      <div class="pr-meter"><div class="pr-meter-fill" data-pct="${l.pct}"></div></div>
      <p class="pr-line-text">${l.text}</p>
    </div>
  `).join('');

  requestAnimationFrame(() => {
    linesEl.querySelectorAll('.pr-meter-fill').forEach(el => {
      el.style.width = el.dataset.pct + '%';
    });
  });

  resultsCard.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

/* ---------- CLEANUP ---------- */
window.addEventListener('pagehide', stopCamera);
document.addEventListener('visibilitychange', () => {
  if (document.hidden) stopCamera();
});
