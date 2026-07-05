// Regenerates assets/og-image.jpg (LinkedIn/Twitter/Facebook share preview)
// from og/og-config.json. Run manually with `npm run generate:og`, or let
// the "Generate OG image" GitHub Action run it automatically on push.
const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const ROOT = path.join(__dirname, "..");
const config = JSON.parse(
  fs.readFileSync(path.join(ROOT, "og", "og-config.json"), "utf8")
);

function escapeHtml(str) {
  return String(str)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

function renderCardBody(card) {
  switch (card.kind) {
    case "chat":
      return `
        <div class="bubble ai">${escapeHtml(card.aiMessage)}</div>
        <div class="bubble user">${escapeHtml(card.userMessage)}</div>
        <div class="bubble ai typing"><i></i><i></i><i></i></div>`;
    case "analytics": {
      const bars = card.bars && card.bars.length ? card.bars : [38, 55, 44, 72, 60, 88, 68, 95];
      return `
        <div class="kpi-row">
          <div class="kpi"><b>${escapeHtml(card.kpi1Value)}</b><span>${escapeHtml(card.kpi1Label)}</span></div>
          <div class="kpi"><b>${escapeHtml(card.kpi2Value)}</b><span>${escapeHtml(card.kpi2Label)}</span></div>
        </div>
        <div class="bars">${bars.map((h) => `<div style="height:${h}%"></div>`).join("")}</div>`;
    }
    case "scan":
      return `
        <div class="scan-wrap">
          <div class="scan-frame">
            <div class="face"></div>
            <div class="corner tl"></div><div class="corner tr"></div>
            <div class="corner bl"></div><div class="corner br"></div>
          </div>
          <div class="granted"><i></i>${escapeHtml(card.statusText || "Access Granted")}</div>
        </div>`;
    default:
      return `
        <div class="generic-card">
          <div class="generic-icon">${escapeHtml((card.title || card.tag || "?").charAt(0).toUpperCase())}</div>
          <div class="generic-title">${escapeHtml(card.title || card.tag)}</div>
        </div>`;
  }
}

const cardSlots = ["a", "b", "c"];
const cardsHtml = config.cards
  .slice(0, 3)
  .map(
    (card, i) => `
      <div class="card card-${cardSlots[i]}">
        <div class="card-tag">${escapeHtml(card.tag)}</div>
        <div class="card-bar"><span class="c c1"></span><span class="c c2"></span><span class="c c3"></span></div>
        <div class="card-body">${renderCardBody(card)}</div>
      </div>`
  )
  .join("");

const pillsHtml = config.pills.map((p) => `<span class="pill">${escapeHtml(p)}</span>`).join("");

const html = `<!doctype html>
<html>
<head>
<meta charset="UTF-8">
<style>
  *{ margin:0; padding:0; box-sizing:border-box; }
  html,body{ width:1200px; height:630px; overflow:hidden; }
  body{
    font-family: -apple-system, "Segoe UI", Arial, sans-serif;
    background: #0d0f14;
    position: relative;
    color: #e2e8f0;
  }
  .grid{
    position:absolute; inset:0;
    background-image: linear-gradient(rgba(255,255,255,0.05) 1px, transparent 1px),
      linear-gradient(90deg, rgba(255,255,255,0.05) 1px, transparent 1px);
    background-size: 42px 42px;
    -webkit-mask-image: radial-gradient(ellipse 900px 500px at 30% 50%, #000 40%, transparent 85%);
    mask-image: radial-gradient(ellipse 900px 500px at 30% 50%, #000 40%, transparent 85%);
  }
  .orb{ position:absolute; border-radius:50%; filter: blur(90px); opacity:0.55; }
  .orb1{ width:480px; height:480px; background: radial-gradient(circle, #6366f1, transparent 70%); top:-180px; left:-140px; }
  .orb2{ width:420px; height:420px; background: radial-gradient(circle, #818cf8, transparent 70%); bottom:-200px; left:420px; opacity:0.35;}

  .wrap{ position:relative; z-index:2; width:100%; height:100%; display:flex; align-items:center; padding: 0 64px; }
  .left{ width: 460px; flex-shrink:0; }
  .brand{ display:flex; align-items:center; gap:10px; margin-bottom:34px; }
  .brand-mark{ font-weight:800; font-size:22px; letter-spacing:-0.03em; color:#e2e8f0; }
  .brand-mark span{ color:#818cf8; }
  .brand-url{ font-family:monospace; font-size:13px; color:rgba(226,232,240,0.45); }

  .eyebrow{
    display:inline-flex; align-items:center; gap:8px;
    font-family:monospace; font-size:12.5px; letter-spacing:0.06em; text-transform:uppercase;
    color:#a5b4fc; background: rgba(99,102,241,0.12); border:1px solid rgba(129,140,248,0.35);
    padding:6px 12px; margin-bottom:22px;
  }
  .dot{ width:6px; height:6px; border-radius:50%; background:#818cf8; }

  h1{ font-size:46px; font-weight:800; letter-spacing:-0.02em; line-height:1.08; color:#f4f6fb; margin-bottom:18px; }
  h1 .grad{
    background: linear-gradient(90deg,#818cf8,#c4b5fd);
    -webkit-background-clip:text; background-clip:text; color:transparent;
  }
  .sub{ font-size:17px; line-height:1.55; color:#9aa6b8; max-width:400px; margin-bottom:26px;}
  .stack{ display:flex; flex-wrap:wrap; gap:8px; }
  .pill{
    font-family:monospace; font-size:12.5px; color:#c7d0e0;
    border:1px solid rgba(255,255,255,0.12); background:rgba(255,255,255,0.03);
    padding:5px 11px;
  }

  .right{ position:relative; flex:1; height:520px; }
  .card{
    position:absolute; width:400px; border-radius:10px; overflow:hidden;
    background:#12151c; border:1px solid rgba(255,255,255,0.09);
    box-shadow: 0 30px 60px -15px rgba(0,0,0,0.6);
  }
  .card-bar{
    height:30px; display:flex; align-items:center; gap:6px; padding:0 12px;
    background:#181c26; border-bottom:1px solid rgba(255,255,255,0.07);
  }
  .card-bar .c{ width:8px; height:8px; border-radius:50%; }
  .card-bar .c1{ background:#ef4444; } .card-bar .c2{ background:#f59e0b; } .card-bar .c3{ background:#22c55e; }
  .card-tag{
    position:absolute; top:38px; left:12px; z-index:3;
    font-family:monospace; font-size:11px; font-weight:500; letter-spacing:0.03em;
    color:#c7d2fe; background:rgba(30,27,60,0.85); border:1px solid rgba(129,140,248,0.4);
    padding:3px 9px; border-radius:3px;
  }
  .card-body{ height:172px; position:relative; padding:26px 18px 16px; }

  .card-a{ top:0; right:0px; z-index:3; transform: rotate(2deg); }
  .card-b{ top:178px; right:210px; z-index:2; transform: rotate(-3deg); }
  .card-c{ top:356px; right:30px; z-index:1; transform: rotate(4deg); }

  .bubble{ max-width:78%; padding:8px 12px; font-size:12.5px; line-height:1.4; margin-bottom:9px; border-radius:9px; }
  .bubble.ai{ background:rgba(129,140,248,0.16); border:1px solid rgba(129,140,248,0.3); color:#dbe0ff; border-bottom-left-radius:2px; }
  .bubble.user{ background:rgba(255,255,255,0.06); border:1px solid rgba(255,255,255,0.08); color:#c7d0e0; margin-left:auto; border-bottom-right-radius:2px; }
  .typing{ display:flex; gap:4px; padding:9px 12px; width:fit-content; }
  .typing i{ width:5px; height:5px; border-radius:50%; background:#818cf8; opacity:0.8; }

  .kpi-row{ display:flex; gap:18px; margin-bottom:12px; }
  .kpi b{ display:block; font-size:19px; font-weight:800; color:#f4f6fb; }
  .kpi span{ font-family:monospace; font-size:10px; color:#8b96ab; text-transform:uppercase; letter-spacing:0.04em; }
  .bars{ display:flex; align-items:flex-end; gap:7px; height:66px; }
  .bars div{ flex:1; border-radius:3px 3px 0 0; background: linear-gradient(180deg,#a5b4fc,#6366f1); }

  .scan-wrap{ display:flex; align-items:center; justify-content:center; height:100%; gap:20px; }
  .scan-frame{ position:relative; width:96px; height:96px; }
  .scan-frame .corner{ position:absolute; width:18px; height:18px; border-color:#818cf8; border-style:solid; }
  .scan-frame .tl{ top:0; left:0; border-width:3px 0 0 3px; }
  .scan-frame .tr{ top:0; right:0; border-width:3px 3px 0 0; }
  .scan-frame .bl{ bottom:0; left:0; border-width:0 0 3px 3px; }
  .scan-frame .br{ bottom:0; right:0; border-width:0 3px 3px 0; }
  .scan-frame .face{ position:absolute; inset:14px; border-radius:50%; background:rgba(129,140,248,0.14); border:1px solid rgba(129,140,248,0.35); }
  .granted{ font-family:monospace; font-size:12px; color:#86efac; background:rgba(34,197,94,0.12); border:1px solid rgba(34,197,94,0.35); padding:6px 12px; display:flex; align-items:center; gap:7px; }
  .granted i{ width:6px; height:6px; border-radius:50%; background:#4ade80; }

  .generic-card{ display:flex; flex-direction:column; align-items:center; justify-content:center; height:100%; gap:14px; }
  .generic-icon{
    width:52px; height:52px; border-radius:12px; display:flex; align-items:center; justify-content:center;
    background:rgba(129,140,248,0.14); border:1px solid rgba(129,140,248,0.35);
    font-size:22px; font-weight:800; color:#a5b4fc;
  }
  .generic-title{ font-size:14px; font-weight:600; color:#e2e8f0; text-align:center; padding:0 12px; }
</style>
</head>
<body>
  <div class="grid"></div>
  <div class="orb orb1"></div>
  <div class="orb orb2"></div>

  <div class="wrap">
    <div class="left">
      <div class="brand">
        <span class="brand-mark">${escapeHtml(config.brandMark)}<span>.</span></span>
        <span class="brand-url">${escapeHtml(config.brandUrl)}</span>
      </div>
      <div class="eyebrow"><span class="dot"></span> ${escapeHtml(config.eyebrow)}</div>
      <h1>${escapeHtml(config.headline.plain)}<span class="grad">${escapeHtml(config.headline.accent)}</span><br>${escapeHtml(config.headline.rest)}</h1>
      <p class="sub">${escapeHtml(config.subhead)}</p>
      <div class="stack">${pillsHtml}</div>
    </div>

    <div class="right">
      ${cardsHtml}
    </div>
  </div>
</body>
</html>`;

(async () => {
  const browser = await chromium.launch();
  try {
    const page = await browser.newPage({ viewport: { width: 1200, height: 630 } });
    await page.setContent(html, { waitUntil: "networkidle" });
    const outPath = path.join(ROOT, "assets", "og-image.jpg");
    fs.mkdirSync(path.dirname(outPath), { recursive: true });
    await page.screenshot({ path: outPath, type: "jpeg", quality: 92 });
    console.log("OG image written to", path.relative(ROOT, outPath));
  } finally {
    await browser.close();
  }
})().catch((err) => {
  console.error(err);
  process.exit(1);
});
