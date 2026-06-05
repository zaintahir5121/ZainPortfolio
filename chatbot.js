const KB = [
  { p: ['hi','hello','hey','morning','afternoon','howdy','hiya'], r: `Hey! 👋 I'm Zain's assistant. Ask me anything — what he builds, his experience, how to hire him, or anything else!` },
  { p: ['who is zain','about zain','who are you','introduce','zain abbas'], r: `**Zain Abbas Tahir** is an AI Technical Lead & Cloud Architect with **13+ years** of experience. Currently Technical Lead at **Aventra Group** (Kuala Lumpur). He builds intelligent, scalable enterprise systems using RAG, LLMs, Azure, .NET Core & Angular — and is open to projects worldwide.` },
  { p: ['what do you build','what can you do','services','problems','solve','help me'], r: `Zain specialises in 4 areas:\n\n🤖 **AI Products** — RAG systems, LLM chatbots, predictive analytics, Copilot Studio agents\n☁️ **Cloud Architecture** — Azure design, microservices, CI/CD, Kubernetes\n💻 **Enterprise Software** — .NET Core, Angular, SaaS platforms, APIs\n👥 **Technical Leadership** — Leading teams, delivery management, architecture consulting` },
  { p: ['skill','tech stack','technology','expertise','programming'], r: `**AI/ML:** RAG, LLMs, Azure OpenAI, Predictive AI, Sentiment Analysis, AI Foundry, Copilot Studio\n\n**Cloud:** Azure, Service Bus, Data Factory, Kubernetes, Docker, Key Vault, Cosmos DB\n\n**Dev:** .NET Core 8, Angular 17+, Blazor, React, Microservices, CQRS, REST APIs\n\n**DevOps:** CI/CD, Azure DevOps, Terraform, ARM/Bicep, SQL Server, Redis, Power BI` },
  { p: ['experience','career','worked','company','job','history'], r: `13+ years across 5 companies:\n\n🟢 **Aventra Group** — Technical Lead (Nov 2025–Present, KL)\n🔵 **DHL IT Services** — Technical Lead (2022–2025, KL)\n🔵 **Tapcheck** — Senior SWE (2019–2022, Remote US)\n🔵 **UMCH** — Senior SWE (2018–2019, KL)\n🔵 **MTBC/CareCloud + Interactive Group** — Architect/Engineer (2013–2018, Islamabad)` },
  { p: ['project','portfolio','built','created','work','example'], r: `Key projects:\n\n📁 **Document Management System** — Enterprise DMS (.NET Core, Azure, Angular)\n🤖 **AI Support Bot** — RAG + GPT-4 for 24/7 automated support\n📊 **Predictive Analytics Engine** — Azure ML forecasting platform\n👤 **Face Recognition Login** — AI-powered passwordless auth\n🔗 **URL Shortener & Analytics** — Click tracking with monetisation\n🖥️ **Point of Sale System** — Full retail POS & inventory` },
  { p: ['contact','email','reach','message','connect'], r: `📧 **Email:** zabbastahir@gmail.com\n📅 **Book a 30-min call:** calendly.com/zainabbastahir/30min\n💼 **LinkedIn:** linkedin.com/in/zainabbastahir\n🐙 **GitHub:** github.com/zainabbastahir\n💻 **Upwork:** upwork.com/freelancers/~013b69c81fcb1ae708` },
  { p: ['available','freelance','hire','remote','contract','open to work','opportunity'], r: `Yes! Zain is **open to new projects** including:\n\n✅ Freelance & contract work\n✅ Full remote positions worldwide\n✅ AI/ML development & consulting\n✅ Cloud architecture consulting\n✅ Technical leadership roles\n\nBest next step: **book a free 30-min call** → calendly.com/zainabbastahir/30min` },
  { p: ['rag','llm','ai','machine learning','chatbot','openai','gpt','neural'], r: `Zain's AI expertise:\n\n🧠 **RAG Systems** — Document Q&A, knowledge base search with Azure AI Search & vector DBs\n💬 **LLM Integration** — Azure OpenAI, GPT-4, Semantic Kernel, custom chatbots\n📈 **Predictive AI** — ML forecasting models with Azure ML\n😊 **Sentiment Analysis** — Real-time NLP text analytics\n🤖 **Copilot Studio** — Custom enterprise AI assistants\n👁️ **Computer Vision** — Face recognition, OCR` },
  { p: ['azure','cloud','kubernetes','docker','devops','infrastructure'], r: `Azure expertise:\n\n☁️ App Services, Functions & API Management\n🔄 Service Bus, Data Factory & Logic Apps\n📦 Kubernetes, Docker & containerisation\n🏗️ Terraform, ARM & Bicep (IaC)\n🔁 CI/CD with Azure DevOps\n💾 Cosmos DB, Redis & SQL Azure` },
  { p: ['.net','dotnet','c#','angular','blazor','asp.net','csharp'], r: `10+ years with .NET & Angular:\n\n⚙️ **.NET Core 8** — APIs, Web Apps, Background Services\n🅰️ **Angular 17+** — Modern SPA development\n🔥 **Blazor** — Server & WebAssembly\n🏛️ **Microservices** — CQRS, event-driven design\n📡 **REST APIs** — Clean, versioned, documented` },
  { p: ['location','where','islamabad','malaysia','kuala lumpur','pakistan','timezone'], r: `Zain is originally from **Islamabad, Pakistan** 🇵🇰 and currently working in **Kuala Lumpur, Malaysia** 🇲🇾. He works remotely with clients in the US, Europe, Middle East, and Asia across all time zones.` },
  { p: ['youtube','video','tutorial','channel'], r: `Zain runs a **YouTube channel** with tutorials on .NET, Azure, and AI development:\n\n▶️ **youtube.com/@zainabbastahir**` },
  { p: ['rate','salary','cost','price','fee','how much','charge'], r: `Rates vary depending on project scope, duration, and type. Best way: **email zabbastahir@gmail.com** or book a free call at **calendly.com/zainabbastahir/30min** to discuss.` },
  { p: ['thank','thanks','appreciate','great','awesome','helpful','perfect'], r: `Happy to help! 😊 Feel free to ask anything else. If you're ready to work with Zain, reach out at zabbastahir@gmail.com!` },
  { p: ['bye','goodbye','see you','later'], r: `Goodbye! 👋 Come back anytime. To start a project with Zain, email **zabbastahir@gmail.com** or book a call. Have a great day!` }
];

function getReply(q) {
  const low = q.toLowerCase();
  for (const item of KB) {
    if (item.p.some(p => low.includes(p))) return item.r;
  }
  return `I'm not sure about that, but I can help with:\n\n• **What Zain builds**\n• **Skills & stack**\n• **Work experience**\n• **Projects & portfolio**\n• **Availability & hiring**\n• **Contact details**\n\nOr email directly: **zabbastahir@gmail.com**`;
}

function fmt(t) {
  return t.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>').replace(/\n\n/g, '<br><br>').replace(/\n/g, '<br>');
}

/* UI */
const fab = document.getElementById('bot-fab');
const win = document.getElementById('bot-win');
const bwClose = document.getElementById('bw-close');
const msgs = document.getElementById('bw-msgs');
const inp = document.getElementById('bw-in');
const send = document.getElementById('bw-send');
const ico = document.getElementById('bot-ico');
const xIco = document.getElementById('bot-x-ico');

let open = false;
win.style.display = 'none';

function toggle() {
  open = !open;
  win.style.display = open ? 'flex' : 'none';
  if (open) win.style.flexDirection = 'column';
  ico.style.display = open ? 'none' : 'block';
  xIco.style.display = open ? 'block' : 'none';
  if (open) setTimeout(() => inp.focus(), 300);
}
fab.addEventListener('click', toggle);
bwClose.addEventListener('click', () => { open = true; toggle(); });

function addMsg(text, role) {
  const d = document.createElement('div');
  d.className = `bm ${role}`;
  const b = document.createElement('div');
  b.className = 'bb';
  b.innerHTML = fmt(text);
  d.appendChild(b);
  msgs.appendChild(d);
  msgs.scrollTop = msgs.scrollHeight;
}

function showTyping() {
  const d = document.createElement('div');
  d.className = 'bm bot'; d.id = 'btyp';
  d.innerHTML = `<div class="bb"><div class="typing-d"><span></span><span></span><span></span></div></div>`;
  msgs.appendChild(d);
  msgs.scrollTop = msgs.scrollHeight;
}
function hideTyping() { document.getElementById('btyp')?.remove(); }

function fire() {
  const txt = inp.value.trim();
  if (!txt) return;
  document.getElementById('bqr-wrap')?.remove();
  addMsg(txt, 'user');
  inp.value = '';
  showTyping();
  setTimeout(() => { hideTyping(); addMsg(getReply(txt), 'bot'); }, 600 + Math.random() * 500);
}

send.addEventListener('click', fire);
inp.addEventListener('keypress', e => { if (e.key === 'Enter') fire(); });
document.querySelectorAll('.bqr').forEach(b => b.addEventListener('click', function () { inp.value = this.dataset.q; fire(); }));
