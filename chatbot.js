const KB = [
  { p:['hi','hello','hey','morning','afternoon','howdy'], r:`Hey! 👋 I'm Zain's AI assistant. Ask me anything — what he builds, his experience, skills, or how to hire him!` },
  { p:['who is zain','about zain','who are you','introduce','zain abbas'], r:`**Zain Abbas Tahir** is an AI Technical Lead & Cloud Architect with **13+ years** of experience. Currently Technical Lead at **Aventra Group** in Kuala Lumpur. Expert in RAG, LLMs, Azure Cloud, .NET Core & Angular — building intelligent enterprise systems that scale.` },
  { p:['what do you build','what can you do','services','problems','solve'], r:`Zain builds 4 types of solutions:\n\n🤖 **AI Products** — RAG pipelines, LLM chatbots, predictive analytics, Copilot Studio agents\n☁️ **Cloud Architecture** — Azure infra, microservices, CI/CD, Kubernetes\n💻 **Enterprise Software** — .NET Core, Angular SaaS platforms, REST APIs\n👥 **Technical Leadership** — Team direction, delivery, code standards` },
  { p:['skill','tech stack','technology','expertise','programming'], r:`**AI/ML:** RAG, LLMs, Azure OpenAI, Predictive AI, Sentiment Analysis, AI Foundry, Copilot Studio\n\n**Cloud:** Azure, Service Bus, Data Factory, Kubernetes, Docker, Key Vault\n\n**Dev:** .NET Core 8, Angular 17+, Blazor, React, Microservices, CQRS\n\n**DevOps:** CI/CD, Azure DevOps, Terraform, ARM/Bicep, SQL Server, Redis` },
  { p:['experience','career','worked','company','history','job'], r:`13+ years across 5 companies:\n\n🟢 **Aventra Group** — Technical Lead (Nov 2025–Present, KL)\n🔵 **DHL IT Services** — Technical Lead (2022–2025, KL)\n🔵 **Tapcheck** — Senior SWE (2019–2022, Remote US)\n🔵 **UMCH** — Senior SWE (2018–2019, KL)\n🔵 **MTBC/CareCloud** — Software Architect (2016–2018, Islamabad)` },
  { p:['project','portfolio','built','work','example'], r:`Key projects:\n\n📁 **Document Management System** — Enterprise DMS (.NET Core, Azure, Angular)\n🤖 **AI Support Bot** — RAG + GPT-4 for 24/7 automated support\n📊 **Predictive Analytics Engine** — Azure ML forecasting platform\n👤 **Face Recognition Login** — AI-powered passwordless auth\n🔗 **URL Shortener** — Analytics & monetisation platform` },
  { p:['contact','email','reach','message','connect'], r:`📧 **Email:** zabbastahir@gmail.com\n📅 **Book a call:** calendly.com/zainabbastahir/30min\n💼 **LinkedIn:** linkedin.com/in/zainabbastahir\n🐙 **GitHub:** github.com/zainabbastahir` },
  { p:['available','freelance','hire','remote','contract','open to'], r:`Yes! Zain is **open to new projects**:\n\n✅ Freelance & contract work\n✅ Full remote positions worldwide\n✅ AI/ML development consulting\n✅ Cloud architecture consulting\n✅ Technical leadership roles\n\nBook a free call → **calendly.com/zainabbastahir/30min**` },
  { p:['rag','llm','ai','machine learning','openai','gpt'], r:`Zain's AI stack:\n\n🧠 **RAG** — Document Q&A with Azure AI Search & vector DBs\n💬 **LLMs** — Azure OpenAI, GPT-4, Semantic Kernel\n📈 **Predictive AI** — Azure ML forecasting models\n😊 **Sentiment Analysis** — Real-time NLP analytics\n🤖 **Copilot Studio** — Custom enterprise AI agents` },
  { p:['azure','cloud','kubernetes','docker','devops'], r:`Azure expertise:\n\n☁️ App Services, Functions & API Management\n🔄 Service Bus, Data Factory & Logic Apps\n📦 Kubernetes, Docker containerisation\n🏗️ Terraform, ARM & Bicep IaC\n🔁 CI/CD with Azure DevOps\n💾 Cosmos DB, Redis, SQL Azure` },
  { p:['location','where','islamabad','malaysia','kuala lumpur','remote'], r:`Zain is from **Islamabad, Pakistan** 🇵🇰, currently in **Kuala Lumpur, Malaysia** 🇲🇾. Works remotely with clients in the US, Europe, Middle East, and Asia.` },
  { p:['rate','salary','cost','price','how much'], r:`Rates depend on scope and duration. Best to discuss directly — **email zabbastahir@gmail.com** or book a free call at **calendly.com/zainabbastahir/30min**.` },
  { p:['thank','thanks','great','awesome','helpful'], r:`You're welcome! 😊 If you're ready to start a project, reach out at zabbastahir@gmail.com!` },
  { p:['bye','goodbye','later'], r:`Goodbye! 👋 Come back anytime. Email **zabbastahir@gmail.com** or book a call to start working with Zain!` }
];

function getReply(q) {
  const l = q.toLowerCase();
  for (const k of KB) if (k.p.some(p => l.includes(p))) return k.r;
  return `I'm not sure about that, but I can help with:\n\n• **What Zain builds**\n• **Skills & tech stack**\n• **Experience & career**\n• **Projects & portfolio**\n• **Availability & hiring**\n\nOr email: **zabbastahir@gmail.com**`;
}

function fmt(t) {
  return t.replace(/\*\*(.*?)\*\*/g,'<strong>$1</strong>').replace(/\n\n/g,'<br><br>').replace(/\n/g,'<br>');
}

const btn = document.getElementById('bot-btn');
const win = document.getElementById('bot-win');
const bwC = document.getElementById('bw-close');
const msgs = document.getElementById('bw-msgs');
const inp = document.getElementById('bw-in');
const send = document.getElementById('bw-send');
const ico = document.getElementById('bico');
const xIco = document.getElementById('bx');
let open = false;

function toggleBot() {
  open = !open;
  win.style.display = open ? 'flex' : 'none';
  if (open) win.style.flexDirection = 'column';
  ico.style.display = open ? 'none' : 'block';
  xIco.style.display = open ? 'block' : 'none';
  if (open) setTimeout(() => inp.focus(), 300);
}
btn.addEventListener('click', toggleBot);
bwC.addEventListener('click', () => { open = true; toggleBot(); });

function addMsg(t, r) {
  const d = document.createElement('div');
  d.className = `bm ${r}`;
  const b = document.createElement('div');
  b.className = 'bb';
  b.innerHTML = fmt(t);
  d.appendChild(b);
  msgs.appendChild(d);
  msgs.scrollTop = msgs.scrollHeight;
}

function showTyping() {
  const d = document.createElement('div');
  d.className = 'bm bot'; d.id = 'btyp';
  d.innerHTML = `<div class="bb"><div class="tdots"><span></span><span></span><span></span></div></div>`;
  msgs.appendChild(d);
  msgs.scrollTop = msgs.scrollHeight;
}
function hideTyping() { document.getElementById('btyp')?.remove(); }

function fire() {
  const t = inp.value.trim();
  if (!t) return;
  document.getElementById('bqrs')?.remove();
  addMsg(t, 'user');
  inp.value = '';
  showTyping();
  setTimeout(() => { hideTyping(); addMsg(getReply(t), 'bot'); }, 600 + Math.random() * 500);
}
send.addEventListener('click', fire);
inp.addEventListener('keypress', e => { if (e.key === 'Enter') fire(); });
document.querySelectorAll('.bqr').forEach(b => b.addEventListener('click', function() { inp.value = this.dataset.q; fire(); }));
