/* ===== ZAIN'S AI CHATBOT ===== */

const KNOWLEDGE_BASE = {
  greetings: {
    patterns: ['hi', 'hello', 'hey', 'good morning', 'good afternoon', 'good evening', 'howdy', 'sup', 'hiya'],
    response: () => `Hi there! 👋 I'm Zain's AI assistant. I can tell you all about Zain's skills, experience, projects, and more. What would you like to know?`
  },
  name: {
    patterns: ['who are you', 'who is zain', 'about zain', 'tell me about yourself', 'introduce', 'zain abbas'],
    response: () => `Zain Abbas Tahir is an **AI Technical Lead & Cloud Architect** based in Islamabad, Pakistan. With **13+ years** of experience, he specializes in building enterprise-grade AI systems using RAG, LLMs, Azure Cloud, .NET Core, and Angular. Currently working as Technical Lead at **Aventra Group** in Kuala Lumpur, Malaysia.`
  },
  skills: {
    patterns: ['skill', 'technology', 'tech stack', 'what can you do', 'expertise', 'what do you know', 'programming', 'languages', 'tools', 'capabilities'],
    response: () => `Zain's key skills span multiple domains:\n\n**AI & ML:** RAG Systems, LLMs, Predictive AI, Sentiment Analysis, AI Foundry, Copilot Studio\n\n**Cloud:** Microsoft Azure, Service Bus, Data Factory, Kubernetes, Azure Functions\n\n**Development:** .NET Core, Angular 17+, Blazor, React, Microservices, CQRS\n\n**DevOps:** CI/CD Pipelines, Azure DevOps, Docker, Terraform, ARM/Bicep\n\n**Data:** SQL Server, EF Core, Cosmos DB, Redis Cache`
  },
  experience: {
    patterns: ['experience', 'work history', 'career', 'where have you worked', 'previous job', 'company', 'employment', 'worked at', 'job'],
    response: () => `Zain has **13+ years** of professional experience:\n\n🟢 **Aventra Group** (Nov 2025–Present) – Technical Lead, Kuala Lumpur\n🔵 **DHL IT Services** (2022–2025) – Technical Lead, Kuala Lumpur\n🔵 **Tapcheck** (2019–2022) – Senior Software Engineer, Remote (US)\n🔵 **UMCH** (2018–2019) – Senior Software Engineer, Kuala Lumpur\n🔵 **MTBC/CareCloud** (2016–2018) – Software Architect, Islamabad\n🔵 **Interactive Group & Moftak** (2013–2016) – Software Engineer, Islamabad`
  },
  projects: {
    patterns: ['project', 'portfolio', 'built', 'created', 'developed', 'work samples', 'examples', 'show me'],
    response: () => `Here are some of Zain's key projects:\n\n📁 **Document Management System** – Enterprise DMS with role-based access, PDF viewing, email & WhatsApp sharing (.NET Core, Angular, Azure)\n\n🤖 **AI Customer Support Bot** – Intelligent chatbot using RAG & LLMs for context-aware responses (Azure OpenAI)\n\n📊 **Predictive Analytics Engine** – ML platform for sales forecasting using Azure ML & Power BI\n\n👤 **Face Recognition Login** – AI-powered passwordless authentication (Python, AI/ML)\n\n🔗 **URL Shortener & Analytics** – Click tracking & analytics dashboard (.NET Core MVC)`
  },
  contact: {
    patterns: ['contact', 'email', 'reach', 'how to get in touch', 'hire', 'get in touch', 'phone', 'message', 'connect'],
    response: () => `You can reach Zain through:\n\n📧 **Email:** zain.tahir512@gmail.com
💬 **WhatsApp:** +60-10-3635-921\n💼 **LinkedIn:** linkedin.com/in/zainabbastahir\n🐙 **GitHub:** github.com/zainabbastahir\n💻 **Upwork:** upwork.com/freelancers/~013b69c81fcb1ae708\n📅 **Schedule a meeting:** calendly.com/zainabbastahir/30min`
  },
  availability: {
    patterns: ['available', 'freelance', 'hire', 'open to work', 'remote', 'contract', 'full time', 'part time', 'opportunity'],
    response: () => `Yes! Zain is **open to new opportunities** including:\n\n✅ Freelance & contract projects\n✅ Remote positions worldwide\n✅ AI/ML consulting\n✅ Cloud architecture consulting\n✅ Technical leadership roles\n\nHe's particularly interested in challenging AI and cloud projects. Feel free to reach out at **zain.tahir512@gmail.com** or schedule a call via Calendly!`
  },
  ai: {
    patterns: ['rag', 'llm', 'artificial intelligence', 'machine learning', 'ai system', 'chatbot', 'neural', 'deep learning', 'nlp', 'language model', 'openai', 'gpt'],
    response: () => `Zain is an **AI specialist** with deep expertise in:\n\n🧠 **RAG Systems** – Retrieval-Augmented Generation with Azure AI Search & vector databases\n💬 **LLM Integration** – Custom chatbots using Azure OpenAI, GPT-4, Semantic Kernel\n📈 **Predictive AI** – ML forecasting models with Azure ML\n😊 **Sentiment Analysis** – NLP-based customer feedback & social media monitoring\n🤖 **Copilot Studio** – Custom AI assistants for enterprise workflows\n👁️ **Computer Vision** – Face recognition, OCR, document processing`
  },
  azure: {
    patterns: ['azure', 'cloud', 'microsoft', 'aws', 'infrastructure', 'devops', 'kubernetes', 'docker', 'deployment'],
    response: () => `Zain is an **Azure Expert** with hands-on experience in:\n\n☁️ Azure App Services & Azure Functions\n🔄 Service Bus, Data Factory & Logic Apps\n🔐 Key Vault, AD & RBAC\n📦 Kubernetes, Docker & containerization\n🏗️ Infrastructure as Code (Terraform, ARM, Bicep)\n🔁 CI/CD Pipelines with Azure DevOps\n💾 Cosmos DB, Redis Cache & SQL Azure`
  },
  dotnet: {
    patterns: ['.net', 'dotnet', 'c#', 'asp.net', 'mvc', 'blazor', 'web api', 'entity framework', 'csharp'],
    response: () => `Zain has **10+ years** with the .NET ecosystem:\n\n⚙️ **.NET Core / .NET 8** – Full-stack web apps & APIs\n🅰️ **Angular 17+** – Modern SPA development\n🔥 **Blazor** – Server & WebAssembly apps\n🏛️ **Microservices** – CQRS, event-driven architecture\n📡 **REST & GraphQL APIs** – Clean API design\n🗄️ **EF Core** – Database-first & code-first approaches`
  },
  location: {
    patterns: ['location', 'where are you', 'based', 'country', 'city', 'pakistan', 'islamabad', 'malaysia', 'kuala lumpur', 'timezone'],
    response: () => `Zain is originally from **Islamabad, Pakistan** 🇵🇰 and is currently working in **Kuala Lumpur, Malaysia** 🇲🇾 at Aventra Group. He's fully comfortable working remotely across different time zones and has experience with US, European, and Asian clients.`
  },
  education: {
    patterns: ['education', 'degree', 'university', 'college', 'study', 'qualification', 'academic'],
    response: () => `Zain holds a strong academic background in Computer Science and has been continuously expanding his knowledge through professional certifications in Azure and AI. He's also a **content creator** on YouTube sharing tutorials on .NET, Azure, and AI development.`
  },
  youtube: {
    patterns: ['youtube', 'video', 'tutorial', 'channel', 'content', 'watch'],
    response: () => `Zain runs a **YouTube channel** where he shares:\n\n🎬 Technical tutorials on .NET, Azure & AI\n🎬 Demo videos of projects he's built\n🎬 Tips for enterprise software development\n\n🔗 Subscribe at: **youtube.com/@zainabbastahir**`
  },
  salary: {
    patterns: ['salary', 'rate', 'cost', 'price', 'charge', 'fee', 'how much', 'hourly'],
    response: () => `For freelance rates or salary expectations, it's best to discuss directly with Zain as it depends on the project scope, duration, and requirements. You can reach him at **zain.tahir512@gmail.com** or schedule a consultation via **Calendly**.`
  },
  thanks: {
    patterns: ['thank', 'thanks', 'appreciate', 'great', 'awesome', 'perfect', 'helpful'],
    response: () => `You're very welcome! 😊 Feel free to ask anything else about Zain. If you'd like to discuss a project, don't hesitate to reach out at zain.tahir512@gmail.com!`
  },
  bye: {
    patterns: ['bye', 'goodbye', 'see you', 'cya', 'later', 'take care'],
    response: () => `Goodbye! 👋 Feel free to come back anytime. If you're interested in working with Zain, send an email to **zain.tahir512@gmail.com** or visit his LinkedIn profile!`
  }
};

function getResponse(input) {
  const lower = input.toLowerCase().trim();
  for (const key in KNOWLEDGE_BASE) {
    const entry = KNOWLEDGE_BASE[key];
    if (entry.patterns.some(p => lower.includes(p))) {
      return entry.response();
    }
  }
  return `I'm not sure about that, but I'd love to help! You can ask me about Zain's:\n\n• **Skills & technologies**\n• **Work experience**\n• **Projects & portfolio**\n• **Availability & contact**\n• **AI & cloud expertise**\n\nOr email him directly at **zain.tahir512@gmail.com** 📧`;
}

/* ===== CHAT UI ===== */
const chatWidget = document.getElementById('chat-widget');
const chatFab = document.getElementById('chat-fab');
const chatWindow = document.getElementById('chat-window');
const cwClose = document.getElementById('cw-close');
const cwMessages = document.getElementById('cw-messages');
const cwInput = document.getElementById('cw-input');
const cwSend = document.getElementById('cw-send');

chatFab.addEventListener('click', () => {
  chatWidget.classList.toggle('open');
  if (chatWidget.classList.contains('open')) {
    setTimeout(() => cwInput.focus(), 350);
  }
});

cwClose.addEventListener('click', () => chatWidget.classList.remove('open'));

function appendMsg(text, role) {
  const msgEl = document.createElement('div');
  msgEl.className = `cw-msg ${role}`;
  const bubble = document.createElement('div');
  bubble.className = 'cw-bubble';
  bubble.innerHTML = formatResponse(text);
  msgEl.appendChild(bubble);
  cwMessages.appendChild(msgEl);
  cwMessages.scrollTop = cwMessages.scrollHeight;
}

function formatResponse(text) {
  return text
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\n\n/g, '<br><br>')
    .replace(/\n/g, '<br>')
    .replace(/🟢|🔵|📁|🤖|📊|👤|🔗|📧|💼|🐙|💻|📅|✅|🧠|💬|📈|😊|🔄|☁️|🔐|📦|🏗️|🔁|💾|⚙️|🅰️|🔥|🏛️|📡|🗄️|🇵🇰|🇲🇾|🎬|👋|😊|📢/g, m => `<span>${m}</span>`);
}

function showTyping() {
  const typingEl = document.createElement('div');
  typingEl.className = 'cw-msg bot';
  typingEl.id = 'cw-typing-indicator';
  typingEl.innerHTML = `<div class="cw-bubble cw-typing"><span></span><span></span><span></span></div>`;
  cwMessages.appendChild(typingEl);
  cwMessages.scrollTop = cwMessages.scrollHeight;
}

function hideTyping() {
  const el = document.getElementById('cw-typing-indicator');
  if (el) el.remove();
}

function sendMessage() {
  const text = cwInput.value.trim();
  if (!text) return;
  const qr = document.getElementById('cw-quick');
  if (qr) qr.remove();
  appendMsg(text, 'user');
  cwInput.value = '';
  showTyping();
  setTimeout(() => {
    hideTyping();
    const reply = getResponse(text);
    appendMsg(reply, 'bot');
  }, 700 + Math.random() * 600);
}

cwSend.addEventListener('click', sendMessage);
cwInput.addEventListener('keypress', (e) => { if (e.key === 'Enter') sendMessage(); });

document.querySelectorAll('.cw-qr').forEach(btn => {
  btn.addEventListener('click', function() {
    cwInput.value = this.dataset.q;
    sendMessage();
  });
});
