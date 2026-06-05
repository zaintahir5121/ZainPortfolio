/* ZAIN'S CHATBOT */
const KB = {
  hi: {
    p: ['hi','hello','hey','good morning','good afternoon','howdy','hiya','sup'],
    r: `Hi there! 👋 I'm Zain's assistant. Ask me anything about what he builds, his experience, or how to hire him!`
  },
  who: {
    p: ['who are you','who is zain','about zain','tell me about','introduce','zain abbas'],
    r: `**Zain Abbas Tahir** is an AI Technical Lead & Cloud Architect with **13+ years** of experience. He's currently Technical Lead at **Aventra Group** in Kuala Lumpur. He specialises in building AI products (RAG, LLMs), Azure cloud infrastructure, and enterprise .NET/Angular apps.`
  },
  solve: {
    p: ['what do you solve','what can you build','problems','challenge','help me with','what do you do','services'],
    r: `Zain solves 4 main categories of problems:\n\n🤖 **AI Products** — RAG systems, LLM chatbots, predictive AI, sentiment dashboards\n☁️ **Cloud Architecture** — Azure design, microservices, DevOps, CI/CD\n💻 **Enterprise Software** — .NET Core backends, Angular frontends, APIs, SaaS platforms\n👥 **Technical Leadership** — Leading dev teams, delivery management, code quality`
  },
  skills: {
    p: ['skill','technology','tech stack','expertise','programming','tools'],
    r: `**AI/ML:** RAG, LLMs, Azure OpenAI, Predictive AI, Sentiment Analysis, Copilot Studio\n\n**Cloud:** Azure, Service Bus, Data Factory, Kubernetes, Docker, Key Vault\n\n**Dev:** .NET Core 8, Angular 17+, Blazor, React, Microservices, CQRS\n\n**DevOps:** CI/CD, Azure DevOps, Terraform, ARM/Bicep, SQL Server, Redis`
  },
  experience: {
    p: ['experience','career','worked','company','employment','job','history'],
    r: `13+ years across:\n\n🟢 **Aventra Group** — Technical Lead (Nov 2025–Present, KL)\n🔵 **DHL IT Services** — Technical Lead (2022–2025, KL)\n🔵 **Tapcheck** — Senior SWE (2019–2022, Remote US)\n🔵 **UMCH** — Senior SWE (2018–2019, KL)\n🔵 **MTBC/CareCloud** — Software Architect (2016–2018, Islamabad)\n🔵 **Interactive Group** — Software Engineer (2013–2016, Islamabad)`
  },
  projects: {
    p: ['project','portfolio','built','created','examples','work'],
    r: `Notable projects:\n\n📁 **Document Management System** — Enterprise DMS (.NET, Azure, Angular)\n🤖 **AI Customer Support Bot** — RAG + LLMs for 24/7 automated support\n📊 **Predictive Analytics Engine** — Azure ML forecasting platform\n👤 **Face Recognition Login** — AI-powered passwordless auth\n🔗 **URL Shortener** — Click analytics platform\n🖥️ **Point of Sale System** — Full retail POS with inventory`
  },
  contact: {
    p: ['contact','email','reach','get in touch','message','connect','phone'],
    r: `📧 **Email:** zabbastahir@gmail.com\n📅 **Book a Call:** calendly.com/zainabbastahir/30min\n💼 **LinkedIn:** linkedin.com/in/zainabbastahir\n🐙 **GitHub:** github.com/zainabbastahir\n💻 **Upwork:** upwork.com/freelancers/~013b69c81fcb1ae708`
  },
  available: {
    p: ['available','freelance','hire','remote','contract','opportunity','open to work'],
    r: `Yes! Zain is **open to new opportunities** including:\n\n✅ Freelance & contract projects\n✅ Full remote positions worldwide\n✅ AI/ML consulting\n✅ Cloud architecture consulting\n✅ Technical leadership roles\n\nBest way to start: **book a free 30-min call** at calendly.com/zainabbastahir/30min`
  },
  ai: {
    p: ['rag','llm','artificial intelligence','machine learning','chatbot','neural','nlp','openai','gpt','ai system'],
    r: `Zain's AI expertise includes:\n\n🧠 **RAG Systems** — Document Q&A, knowledge base search (Azure AI Search + Vector DB)\n💬 **LLM Integration** — Custom chatbots, Azure OpenAI, GPT-4, Semantic Kernel\n📈 **Predictive AI** — ML forecasting with Azure ML\n😊 **Sentiment Analysis** — Real-time NLP text analytics\n🤖 **Copilot Studio** — Custom enterprise AI assistants\n👁️ **Computer Vision** — Face recognition, OCR`
  },
  azure: {
    p: ['azure','cloud','microsoft','kubernetes','docker','devops','infrastructure','deployment'],
    r: `Zain is an **Azure Expert** with hands-on experience in:\n\n☁️ App Services, Functions & API Management\n🔄 Service Bus, Data Factory & Logic Apps\n📦 Kubernetes, Docker & containerisation\n🏗️ Terraform, ARM templates & Bicep (IaC)\n🔁 CI/CD Pipelines with Azure DevOps\n💾 Cosmos DB, Redis & SQL Azure`
  },
  dotnet: {
    p: ['.net','dotnet','c#','angular','asp.net','blazor','web api','csharp'],
    r: `Zain has **10+ years** with .NET & Angular:\n\n⚙️ **.NET Core 8** — APIs, Web Apps, Background Services\n🅰️ **Angular 17+** — Modern SPA with RxJS & NgRx\n🔥 **Blazor** — Server & WebAssembly\n🏛️ **Microservices** — CQRS, event-driven architecture\n📡 **REST APIs** — Clean, versioned, documented`
  },
  location: {
    p: ['location','where','based','pakistan','islamabad','malaysia','kuala lumpur','timezone','country'],
    r: `Zain is from **Islamabad, Pakistan** 🇵🇰 and currently works in **Kuala Lumpur, Malaysia** 🇲🇾. He's fully comfortable working remotely with US, European, and Asian clients across all time zones.`
  },
  youtube: {
    p: ['youtube','video','tutorial','channel','watch'],
    r: `Zain has a **YouTube channel** with tutorials on:\n🎬 .NET Core, Azure & AI development\n🎬 Demo walkthroughs of projects\n🎬 Architecture deep-dives\n\n▶️ **youtube.com/@zainabbastahir**`
  },
  rate: {
    p: ['salary','rate','cost','price','charge','fee','how much','hourly'],
    r: `Rates depend on project scope, duration, and type of engagement. The best way to discuss is to **book a free 30-min call** at calendly.com/zainabbastahir/30min or email zabbastahir@gmail.com directly.`
  },
  thanks: {
    p: ['thank','thanks','appreciate','great','awesome','perfect','helpful','cool'],
    r: `You're welcome! 😊 Feel free to ask anything else. If you'd like to work with Zain, send an email or book a call — he's always happy to chat!`
  },
  bye: {
    p: ['bye','goodbye','see you','later','take care'],
    r: `Goodbye! 👋 If you ever want to discuss a project, reach Zain at **zabbastahir@gmail.com** or book a call at Calendly. Have a great day!`
  }
};

function getReply(input) {
  const q = input.toLowerCase().trim();
  for (const k in KB) {
    if (KB[k].p.some(p => q.includes(p))) return KB[k].r;
  }
  return `I'm not sure about that, but here's what I can help with:\n\n• **What Zain builds / solves**\n• **Skills & tech stack**\n• **Work experience**\n• **Projects & portfolio**\n• **Availability & hiring**\n• **Contact info**\n\nOr just email him: **zabbastahir@gmail.com** 📧`;
}

function fmt(t) {
  return t
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\n\n/g, '<br><br>')
    .replace(/\n/g, '<br>');
}

/* UI */
const fab = document.getElementById('bot-fab');
const win = document.getElementById('bot-win');
const bx = document.getElementById('bot-x');
const msgs = document.getElementById('bot-msgs');
const inp = document.getElementById('bot-in');
const send = document.getElementById('bot-send');

fab.addEventListener('click', () => {
  const open = win.style.display === 'flex';
  win.style.display = open ? 'none' : 'flex';
  win.style.flexDirection = 'column';
  const icon = fab.querySelector('.bot-open');
  const xi = fab.querySelector('.bot-close');
  icon.style.display = open ? 'block' : 'none';
  xi.style.display = open ? 'none' : 'block';
  if (!open) setTimeout(() => inp.focus(), 300);
});
bx.addEventListener('click', () => {
  win.style.display = 'none';
  fab.querySelector('.bot-open').style.display = 'block';
  fab.querySelector('.bot-close').style.display = 'none';
});

function addMsg(text, role) {
  const div = document.createElement('div');
  div.className = `bmsg ${role}`;
  const bbl = document.createElement('div');
  bbl.className = 'bbl';
  bbl.innerHTML = fmt(text);
  div.appendChild(bbl);
  msgs.appendChild(div);
  msgs.scrollTop = msgs.scrollHeight;
}

function showTyping() {
  const div = document.createElement('div');
  div.className = 'bmsg bot bot-typing';
  div.id = 'typing';
  div.innerHTML = `<div class="bbl"><div class="typing-dots"><span></span><span></span><span></span></div></div>`;
  msgs.appendChild(div);
  msgs.scrollTop = msgs.scrollHeight;
}
function hideTyping() { document.getElementById('typing')?.remove(); }

function fire() {
  const txt = inp.value.trim();
  if (!txt) return;
  document.getElementById('bot-qr')?.remove();
  addMsg(txt, 'user');
  inp.value = '';
  showTyping();
  setTimeout(() => {
    hideTyping();
    addMsg(getReply(txt), 'bot');
  }, 600 + Math.random() * 600);
}

send.addEventListener('click', fire);
inp.addEventListener('keypress', e => { if (e.key === 'Enter') fire(); });
document.querySelectorAll('.bqr').forEach(b => b.addEventListener('click', function() {
  inp.value = this.dataset.q;
  fire();
}));
