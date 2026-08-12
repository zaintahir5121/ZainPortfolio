/* ---------------------------------------------------------------------------
   Multi-language layer — English, Deutsch, Bahasa Melayu.

   Pure client-side so the page keeps working from file:// with no build step.
   Any element carrying data-i18n has its text replaced; data-i18n-html allows
   inline markup, and data-i18n-attr handles placeholders and aria labels.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  var DICT = {
    en: {
      /* ---- nav ---- */
      'nav.home': 'Home', 'nav.about': 'About', 'nav.skills': 'Skills',
      'nav.exp': 'Experience', 'nav.ai': 'AI Lab', 'nav.projects': 'Projects',
      'nav.contact': 'Contact', 'nav.hire': 'Hire Me',

      /* ---- hero ---- */
      'hero.badge': 'Available for new opportunities',
      'hero.r1': 'Technical Manager', 'hero.r2': 'Cloud Architect',
      'hero.h1': 'Building <span class="grad-text">AI-Powered</span><br>Systems That Scale',
      'hero.p': '13+ years designing enterprise-grade solutions with <strong>RAG</strong>, <strong>LLMs</strong>, <strong>Azure Cloud</strong>, <strong>.NET Core</strong> &amp; <strong>Angular</strong> — turning complex data into actionable intelligence.',
      'hero.s1': 'Years Exp.', 'hero.s2': 'Projects', 'hero.s3': 'Enterprise',
      'hero.cta1': 'View Work', 'hero.cta2': "Let's Talk",

      /* ---- about ---- */
      'about.tag': 'About Me',
      'about.h2': 'Turning Data Into <span class="grad-text">Intelligence</span>',
      'about.years': 'Years of<br>Excellence',
      'about.li': 'LinkedIn Profile',
      'about.lead': 'I build <strong>intelligent, scalable, and predictive systems</strong> — designing advanced AI architectures using RAG, LLMs, Predictive AI, Sentiment Analysis, and AI Foundry platforms.',
      'about.p2': 'My approach combines microservices, event-driven design, and AI-powered automation to deliver enterprise-scale, cloud-native solutions. I excel in end-to-end software delivery — from solution architecture and API integration to CI/CD pipelines, cloud infrastructure, and automated workflows.',
      'about.c1t': 'AI &amp; LLM Expert', 'about.c1d': 'RAG, LLMs, Predictive AI, Sentiment Analysis &amp; AI Foundry',
      'about.c2t': 'Azure Architect', 'about.c2d': 'Data Fabric, Service Bus, Data Factory, Copilot Studio',
      'about.c3t': 'Technical Manager', 'about.c3d': 'End-to-end software delivery &amp; technical leadership',
      'about.c4t': 'DevOps &amp; Automation', 'about.c4d': 'CI/CD, IaC (ARM, Terraform, Bicep), GitHub Codespaces',
      'about.t1': 'Custom Software Development', 'about.t2': 'API Development &amp; Integration',
      'about.t3': 'Cloud Solutions &amp; Azure', 'about.t4': 'Microservices Architecture',
      'about.t5': 'Performance Optimization', 'about.t6': 'DevOps &amp; CI/CD Setup',
      'about.t7': 'Database Optimization', 'about.t8': 'Code Review &amp; Refactoring',

      /* ---- skills ---- */
      'sk.tag': 'Technical Expertise',
      'sk.h2': 'Technologies I <span class="grad-text">Master</span>',
      'sk.desc': 'A comprehensive toolkit built over 13+ years of professional development',
      'sk.g1': 'AI &amp; Analytics', 'sk.g2': 'Cloud &amp; Infrastructure',
      'sk.g3': 'Development &amp; Architecture', 'sk.g4': 'DevOps &amp; Automation',
      'sk.g5': 'Agentic AI &amp; MCP', 'sk.g6': 'LLMOps &amp; AI Governance',
      'sk.g7': 'Enterprise Architecture', 'sk.g8': 'Delivery &amp; Governance',
      'ms.badge': 'Microsoft AI Stack',
      'ms.desc': 'The platform I build enterprise AI on day to day.',
      'ms.f': 'Unified data platform', 'ms.af': 'Model catalogue &amp; deployment',
      'ms.cp': 'Custom copilots &amp; agents', 'ms.ao': 'GPT models inside the tenant',
      'ms.sk': '.NET agent orchestration', 'ms.as': 'Vector &amp; hybrid retrieval',
      'sk.yrs': 'yrs', 'sk.yr': 'yr', 'sk.legend': 'Each badge shows hands-on years with that technology',

      /* ---- experience ---- */
      'exp.tag': 'Career Journey', 'exp.h2': 'Professional <span class="grad-text">Experience</span>',
      'exp.desc': 'Over a decade of building and leading engineering teams',

      /* ---- AI lab ---- */
      'ai.tag': 'Interactive',
      'ai.h2': 'See How I Build <span class="grad-text">AI Systems</span>',
      'ai.desc': 'Click any stage to walk through a production RAG pipeline — the same architecture behind the platforms below.',
      'ai.run': 'Run Pipeline', 'ai.reset': 'Reset', 'ai.try': 'Try a question:',
      'ai.ph': 'Ask something, e.g. "What is our refund policy?"',
      'ai.st1': 'Query', 'ai.st1d': 'The user question arrives and is normalised, spell-checked and expanded with synonyms so retrieval is not derailed by phrasing.',
      'ai.st2': 'Embed', 'ai.st2d': 'The query is converted into a vector — a list of numbers capturing meaning rather than exact words, so "refund" also matches "money back".',
      'ai.st3': 'Retrieve', 'ai.st3d': 'A vector search returns the closest document chunks. Permission filters are applied here, before the model, so users never see data they are not entitled to.',
      'ai.st4': 'Re-rank', 'ai.st4d': 'A second, sharper model reorders the candidates by true relevance and drops weak matches, which is what removes most hallucination risk.',
      'ai.st5': 'Generate', 'ai.st5d': 'The LLM answers using only the retrieved context, with the prompt constraining it to cite sources and to say "I do not know" rather than invent.',
      'ai.st6': 'Ground', 'ai.st6d': 'Every claim is checked back against the source chunks. Answers that cannot be grounded are withheld, and the citation is attached for audit.',
      'ai.hint': 'Click a stage above to see what it does',
      'ai.m1': 'Latency', 'ai.m2': 'Grounding', 'ai.m3': 'Cost / query',

      /* ---- projects ---- */
      'pj.tag': 'Featured Work',
      'pj.h2': 'Projects That <span class="grad-text">Deliver Results</span>',
      'pj.desc': 'Real solutions built for real business challenges across AI, Cloud &amp; Enterprise domains',
      'pj.f1': 'All', 'pj.f2': 'AI / ML', 'pj.f3': 'Cloud', 'pj.f4': 'Web Apps', 'pj.f5': 'Desktop',

      /* ---- contact ---- */
      'ct.tag': 'Get In Touch',
      'ct.h2': "Let's Build Something <span class=\"grad-text\">Amazing</span>",
      'ct.desc': 'Have a project in mind or want to explore collaboration opportunities?',
      'ct.info': 'Contact Information', 'ct.email': 'Email', 'ct.loc': 'Location',
      'ct.wa': 'WhatsApp', 'ct.avail': 'Availability', 'ct.availv': 'Open to remote &amp; freelance work',
      'ct.meet': 'Schedule a Meeting',
      'ct.name': 'Full Name', 'ct.emailL': 'Email Address', 'ct.subj': 'Subject', 'ct.msg': 'Message',
      'ct.send': 'Send Message',
      'ct.phName': 'John Doe', 'ct.phEmail': 'john@example.com',
      'ct.phSubj': 'Project Inquiry', 'ct.phMsg': 'Tell me about your project...',
      /* ---- impact ---- */
      'imp.tag': 'Measured Outcomes',
      'imp.h2': 'Results, Not <span class="grad-text">R\u00e9sum\u00e9 Lines</span>',
      'imp.desc': 'Every number below comes from a system that shipped and stayed in production.',
      'imp.m1l': 'Faster compliance review', 'imp.m2l': 'Less reporting effort',
      'imp.m3l': 'Engineers led', 'imp.m4l': 'Projects delivered',
      'imp.m5l': 'Teams under my lead', 'imp.m6l': 'Years in engineering',

      /* ---- architecture ---- */
      'arc.tag': 'Systems Thinking',
      'arc.h2': 'The <span class="grad-text">Enterprise AI Stack</span> I Build',
      'arc.desc': 'Five layers, one governed platform. Select a layer to see what sits inside it and why it matters.',
      'arc.hint': 'Select a layer to see what it contains',
      'arc.l1': 'Experience Layer', 'arc.l1c': 'Web \u00b7 Teams \u00b7 Voice \u00b7 API',
      'arc.l1d': 'Where people actually meet the system. One conversation contract across every surface, so a new channel is a client of the platform rather than another rewrite.',
      'arc.l2': 'Orchestration &amp; Agents', 'arc.l2c': 'Planners \u00b7 Tools \u00b7 Workflows \u00b7 Human-in-the-loop',
      'arc.l2d': 'Agents decompose a goal, call approved tools and hand control back to a human at defined checkpoints. Autonomy is bounded by policy, never by hope.',
      'arc.l3': 'AI Gateway', 'arc.l3c': 'Routing \u00b7 Guardrails \u00b7 Quotas \u00b7 Cost control',
      'arc.l3d': 'Every call goes through one gateway. It picks the cheapest model that meets the quality bar, enforces guardrails, caps spend per team and gives one place to swap providers.',
      'arc.l4': 'Knowledge &amp; Retrieval', 'arc.l4c': 'Chunking \u00b7 Embeddings \u00b7 Vector search \u00b7 Permission filter',
      'arc.l4d': 'The RAG core. Permissions are applied at retrieval, before the model sees anything \u2014 so the model can never leak what the user was not entitled to read.',
      'arc.l5': 'Governance &amp; Observability', 'arc.l5c': 'Audit trail \u00b7 Evals \u00b7 Tracing \u00b7 Drift alerts',
      'arc.l5d': 'Every prompt, retrieval and answer is traced and replayable. Automated evals run on each change, so quality regressions are caught before users meet them.',

      /* ---- recruiter ---- */
      'rec.h3': 'Recruiter Snapshot', 'rec.open': 'Open to opportunities',
      'rec.loc': 'Based in', 'rec.locv': 'Kuala Lumpur, Malaysia \u00b7 open to relocation',
      'rec.focus': 'Focus', 'rec.focusv': 'AI \u00b7 Cloud architecture \u00b7 Engineering leadership',
      'rec.lang': 'Languages', 'rec.langv': 'English \u00b7 Urdu \u00b7 Malay (working)',
      'rec.setup': 'Work setup', 'rec.setupv': 'On-site \u00b7 Hybrid \u00b7 Remote',
      'rec.resp': 'Reply time', 'rec.respv': 'Within 24 hours',
      'rec.cv': 'View Full Profile', 'rec.book': 'Book 30 Minutes',
      'nav.impact': 'Impact',
      /* ---- case study ---- */
      'cs.tag': 'Deep Dive',
      'cs.h2': 'One System, <span class="grad-text">Told Properly</span>',
      'cs.desc': 'Anyone can list technologies. This is the decision trail behind the platform every AI feature in the company now runs through.',
      'cs.kicker': 'Aventra Group \u00b7 2025 \u00b7 Technical Manager',
      'cs.title': 'The Central AI Hub',
      'cs.b1': 'The problem',
      'cs.b1d': 'Four teams had each wired their own OpenAI calls straight into their service. Four sets of keys, four prompt styles, no shared spend visibility, and no way to answer "what did the model tell that customer, and why?" when Legal asked.',
      'cs.b2': 'The constraint',
      'cs.b2d': 'I could not stop delivery to rebuild. Teams had live commitments, so whatever replaced the direct calls had to be adoptable in under a day per team, or it would simply be routed around.',
      'cs.b3': 'The decision',
      'cs.b3d': 'One gateway service, and a rule that no application talks to a model provider directly. The gateway owns routing, guardrails, per-team quotas and the audit trail. Migration was a base-URL change plus a header \u2014 deliberately trivial, because adoption friction was the real risk, not the engineering.',
      'cs.b4': 'The trade-off',
      'cs.b4d': 'A gateway is a single point of failure and adds a network hop. I accepted roughly 40 ms of added latency and paid for it with a health-checked active-active pair, because a governed platform with a hop beats four ungoverned integrations without one.',
      'cs.b5': 'The outcome',
      'cs.b5d': 'Every AI call in the company is now traceable to a user, a prompt and a retrieved source. Model spend became a line item per team instead of a surprise, and swapping a provider is a configuration change rather than four parallel refactors.',
      'cs.b6': 'What I would do differently',
      'cs.b6d': 'I built routing before evaluations. That was the wrong order \u2014 without an eval harness, the first model swap was a judgement call rather than a measurement. I would ship the eval suite alongside the gateway, not two months behind it.',

      /* ---- verification ---- */
      'vf.label': 'Verify independently',
      'vf.gh': 'GitHub', 'vf.ghs': 'Code and repositories',
      'vf.li': 'LinkedIn', 'vf.lis': 'Roles, dates and recommendations',
      'vf.yt': 'YouTube', 'vf.yts': 'Talks and walkthroughs',
      'vf.up': 'Upwork', 'vf.ups': 'Client history and reviews',


    },

    de: {
      'nav.home': 'Start', 'nav.about': 'Über mich', 'nav.skills': 'Fähigkeiten',
      'nav.exp': 'Erfahrung', 'nav.ai': 'KI-Labor', 'nav.projects': 'Projekte',
      'nav.contact': 'Kontakt', 'nav.hire': 'Anfragen',

      'hero.badge': 'Offen für neue Herausforderungen',
      'hero.r1': 'Technischer Manager', 'hero.r2': 'Cloud-Architekt',
      'hero.h1': '<span class="grad-text">KI-gestützte</span> Systeme,<br>die skalieren',
      'hero.p': 'Seit über 13 Jahren entwickle ich Unternehmenslösungen mit <strong>RAG</strong>, <strong>LLMs</strong>, <strong>Azure Cloud</strong>, <strong>.NET Core</strong> und <strong>Angular</strong> — und mache aus komplexen Daten belastbare Entscheidungen.',
      'hero.s1': 'Jahre Erf.', 'hero.s2': 'Projekte', 'hero.s3': 'Unternehmen',
      'hero.cta1': 'Projekte ansehen', 'hero.cta2': 'Gespräch starten',

      'about.tag': 'Über mich',
      'about.h2': 'Aus Daten wird <span class="grad-text">Intelligenz</span>',
      'about.years': 'Jahre<br>Erfahrung',
      'about.li': 'LinkedIn-Profil',
      'about.lead': 'Ich baue <strong>intelligente, skalierbare und vorausschauende Systeme</strong> — und entwerfe fortschrittliche KI-Architekturen mit RAG, LLMs, Predictive AI, Sentiment-Analyse und AI-Foundry-Plattformen.',
      'about.p2': 'Mein Ansatz verbindet Microservices, ereignisgesteuertes Design und KI-gestützte Automatisierung zu cloud-nativen Lösungen auf Unternehmensniveau. Ich begleite die gesamte Software-Lieferkette — von Lösungsarchitektur und API-Integration bis zu CI/CD-Pipelines, Cloud-Infrastruktur und automatisierten Abläufen.',
      'about.c1t': 'KI- &amp; LLM-Experte', 'about.c1d': 'RAG, LLMs, Predictive AI, Sentiment-Analyse &amp; AI Foundry',
      'about.c2t': 'Azure-Architekt', 'about.c2d': 'Data Fabric, Service Bus, Data Factory, Copilot Studio',
      'about.c3t': 'Technischer Manager', 'about.c3d': 'Durchgängige Software-Lieferung &amp; technische Führung',
      'about.c4t': 'DevOps &amp; Automatisierung', 'about.c4d': 'CI/CD, IaC (ARM, Terraform, Bicep), GitHub Codespaces',
      'about.t1': 'Individuelle Softwareentwicklung', 'about.t2': 'API-Entwicklung &amp; Integration',
      'about.t3': 'Cloud-Lösungen &amp; Azure', 'about.t4': 'Microservices-Architektur',
      'about.t5': 'Performance-Optimierung', 'about.t6': 'DevOps &amp; CI/CD-Aufbau',
      'about.t7': 'Datenbank-Optimierung', 'about.t8': 'Code-Review &amp; Refactoring',

      'sk.tag': 'Technische Expertise',
      'sk.h2': 'Technologien, die ich <span class="grad-text">beherrsche</span>',
      'sk.desc': 'Ein umfassendes Werkzeugset, aufgebaut in über 13 Jahren Berufspraxis',
      'sk.g1': 'KI &amp; Analytik', 'sk.g2': 'Cloud &amp; Infrastruktur',
      'sk.g3': 'Entwicklung &amp; Architektur', 'sk.g4': 'DevOps &amp; Automatisierung',
      'sk.g5': 'Agentische KI &amp; MCP', 'sk.g6': 'LLMOps &amp; KI-Governance',
      'sk.g7': 'Unternehmensarchitektur', 'sk.g8': 'Lieferung &amp; Governance',
      'ms.badge': 'Microsoft-KI-Stack',
      'ms.desc': 'Die Plattform, auf der ich t\u00e4glich Unternehmens-KI baue.',
      'ms.f': 'Einheitliche Datenplattform', 'ms.af': 'Modellkatalog &amp; Bereitstellung',
      'ms.cp': 'Eigene Copilots &amp; Agenten', 'ms.ao': 'GPT-Modelle im eigenen Tenant',
      'ms.sk': '.NET-Agenten-Orchestrierung', 'ms.as': 'Vektor- &amp; Hybridsuche',
      'sk.yrs': 'J.', 'sk.yr': 'J.', 'sk.legend': 'Jedes Abzeichen zeigt die Praxisjahre mit dieser Technologie',

      'exp.tag': 'Werdegang', 'exp.h2': 'Beruflicher <span class="grad-text">Werdegang</span>',
      'exp.desc': 'Über ein Jahrzehnt Aufbau und Führung von Entwicklungsteams',

      'ai.tag': 'Interaktiv',
      'ai.h2': 'So baue ich <span class="grad-text">KI-Systeme</span>',
      'ai.desc': 'Klicken Sie auf eine Stufe, um eine produktive RAG-Pipeline zu durchlaufen — dieselbe Architektur wie in den Projekten unten.',
      'ai.run': 'Pipeline starten', 'ai.reset': 'Zurücksetzen', 'ai.try': 'Frage ausprobieren:',
      'ai.ph': 'Fragen Sie etwas, z. B. „Wie lautet unsere Rückgaberichtlinie?“',
      'ai.st1': 'Anfrage', 'ai.st1d': 'Die Nutzerfrage wird normalisiert, rechtschreibgeprüft und um Synonyme erweitert, damit die Formulierung die Suche nicht ausbremst.',
      'ai.st2': 'Einbetten', 'ai.st2d': 'Die Anfrage wird in einen Vektor umgewandelt — eine Zahlenreihe, die Bedeutung statt exakter Wörter erfasst. So trifft „Rückgabe“ auch auf „Geld zurück“.',
      'ai.st3': 'Abrufen', 'ai.st3d': 'Eine Vektorsuche liefert die nächstgelegenen Dokumentabschnitte. Berechtigungsfilter greifen hier — vor dem Modell —, damit niemand unbefugte Daten sieht.',
      'ai.st4': 'Neu ordnen', 'ai.st4d': 'Ein zweites, präziseres Modell sortiert die Kandidaten nach echter Relevanz und verwirft schwache Treffer. Das entfernt den Großteil des Halluzinationsrisikos.',
      'ai.st5': 'Generieren', 'ai.st5d': 'Das LLM antwortet ausschließlich aus dem abgerufenen Kontext. Der Prompt zwingt es, Quellen zu nennen und lieber „Ich weiß es nicht“ zu sagen als zu erfinden.',
      'ai.st6': 'Absichern', 'ai.st6d': 'Jede Aussage wird gegen die Quellabschnitte geprüft. Nicht belegbare Antworten werden zurückgehalten, die Quellenangabe wird revisionssicher angehängt.',
      'ai.hint': 'Klicken Sie oben auf eine Stufe, um zu sehen, was sie tut',
      'ai.m1': 'Latenz', 'ai.m2': 'Beleglage', 'ai.m3': 'Kosten / Anfrage',

      'pj.tag': 'Ausgewählte Arbeiten',
      'pj.h2': 'Projekte, die <span class="grad-text">Ergebnisse liefern</span>',
      'pj.desc': 'Echte Lösungen für echte Geschäftsprobleme in den Bereichen KI, Cloud und Unternehmenssoftware',
      'pj.f1': 'Alle', 'pj.f2': 'KI / ML', 'pj.f3': 'Cloud', 'pj.f4': 'Web-Apps', 'pj.f5': 'Desktop',

      'ct.tag': 'Kontakt aufnehmen',
      'ct.h2': 'Lassen Sie uns etwas <span class="grad-text">Großartiges</span> bauen',
      'ct.desc': 'Haben Sie ein Projekt im Sinn oder möchten Sie eine Zusammenarbeit ausloten?',
      'ct.info': 'Kontaktdaten', 'ct.email': 'E-Mail', 'ct.loc': 'Standort',
      'ct.wa': 'WhatsApp', 'ct.avail': 'Verfügbarkeit', 'ct.availv': 'Offen für Remote- &amp; Freelance-Arbeit',
      'ct.meet': 'Termin vereinbaren',
      'ct.name': 'Vollständiger Name', 'ct.emailL': 'E-Mail-Adresse', 'ct.subj': 'Betreff', 'ct.msg': 'Nachricht',
      'ct.send': 'Nachricht senden',
      'ct.phName': 'Max Mustermann', 'ct.phEmail': 'max@beispiel.de',
      'ct.phSubj': 'Projektanfrage', 'ct.phMsg': 'Erzählen Sie mir von Ihrem Projekt …',
      'imp.tag': 'Messbare Ergebnisse',
      'imp.h2': 'Ergebnisse statt <span class="grad-text">Lebenslauf-Zeilen</span>',
      'imp.desc': 'Jede Zahl unten stammt aus einem System, das ausgeliefert wurde und im Betrieb geblieben ist.',
      'imp.m1l': 'Schnellere Compliance-Pr\u00fcfung', 'imp.m2l': 'Weniger Reporting-Aufwand',
      'imp.m3l': 'Gef\u00fchrte Entwickler', 'imp.m4l': 'Gelieferte Projekte',
      'imp.m5l': 'Teams unter meiner F\u00fchrung', 'imp.m6l': 'Jahre in der Entwicklung',

      'arc.tag': 'Architektonisches Denken',
      'arc.h2': 'Der <span class="grad-text">Enterprise-KI-Stack</span>, den ich baue',
      'arc.desc': 'F\u00fcnf Schichten, eine kontrollierte Plattform. W\u00e4hlen Sie eine Schicht, um zu sehen, was darin steckt und warum es z\u00e4hlt.',
      'arc.hint': 'W\u00e4hlen Sie eine Schicht, um ihren Inhalt zu sehen',
      'arc.l1': 'Erlebnisschicht', 'arc.l1c': 'Web \u00b7 Teams \u00b7 Sprache \u00b7 API',
      'arc.l1d': 'Hier begegnen Menschen dem System. Ein einheitlicher Dialogvertrag \u00fcber alle Kan\u00e4le \u2014 ein neuer Kanal ist damit nur ein Client der Plattform und kein weiteres Neuschreiben.',
      'arc.l2': 'Orchestrierung &amp; Agenten', 'arc.l2c': 'Planer \u00b7 Werkzeuge \u00b7 Abl\u00e4ufe \u00b7 Mensch im Prozess',
      'arc.l2d': 'Agenten zerlegen ein Ziel, rufen freigegebene Werkzeuge auf und geben an definierten Punkten an den Menschen zur\u00fcck. Autonomie wird durch Richtlinien begrenzt, nicht durch Hoffnung.',
      'arc.l3': 'KI-Gateway', 'arc.l3c': 'Routing \u00b7 Leitplanken \u00b7 Kontingente \u00b7 Kostensteuerung',
      'arc.l3d': 'Jeder Aufruf l\u00e4uft \u00fcber ein Gateway. Es w\u00e4hlt das g\u00fcnstigste Modell, das die Qualit\u00e4t h\u00e4lt, erzwingt Leitplanken, deckelt Ausgaben je Team und macht Anbieterwechsel zur Einstellung.',
      'arc.l4': 'Wissen &amp; Retrieval', 'arc.l4c': 'Chunking \u00b7 Embeddings \u00b7 Vektorsuche \u00b7 Berechtigungsfilter',
      'arc.l4d': 'Der RAG-Kern. Berechtigungen greifen beim Abruf, bevor das Modell irgendetwas sieht \u2014 so kann es niemals preisgeben, was der Nutzer gar nicht lesen durfte.',
      'arc.l5': 'Governance &amp; Observability', 'arc.l5c': 'Pr\u00fcfpfad \u00b7 Evaluationen \u00b7 Tracing \u00b7 Drift-Alarme',
      'arc.l5d': 'Jeder Prompt, jeder Abruf und jede Antwort ist nachvollziehbar und wiederholbar. Automatische Evaluationen laufen bei jeder \u00c4nderung \u2014 Qualit\u00e4tsr\u00fcckschritte fallen vor den Nutzern auf.',

      'rec.h3': '\u00dcberblick f\u00fcr Recruiter', 'rec.open': 'Offen f\u00fcr Angebote',
      'rec.loc': 'Standort', 'rec.locv': 'Kuala Lumpur, Malaysia \u00b7 umzugsbereit',
      'rec.focus': 'Schwerpunkt', 'rec.focusv': 'KI \u00b7 Cloud-Architektur \u00b7 Technische F\u00fchrung',
      'rec.lang': 'Sprachen', 'rec.langv': 'Englisch \u00b7 Urdu \u00b7 Malaiisch (Grundlagen)',
      'rec.setup': 'Arbeitsmodell', 'rec.setupv': 'Vor Ort \u00b7 Hybrid \u00b7 Remote',
      'rec.resp': 'Antwortzeit', 'rec.respv': 'Innerhalb von 24 Stunden',
      'rec.cv': 'Vollst\u00e4ndiges Profil', 'rec.book': '30 Minuten buchen',
      'nav.impact': 'Wirkung',
      'cs.tag': 'Tiefenanalyse',
      'cs.h2': 'Ein System, <span class="grad-text">richtig erz\u00e4hlt</span>',
      'cs.desc': 'Technologien aufz\u00e4hlen kann jeder. Dies ist die Entscheidungskette hinter der Plattform, \u00fcber die heute jede KI-Funktion des Unternehmens l\u00e4uft.',
      'cs.kicker': 'Aventra Group \u00b7 2025 \u00b7 Technischer Manager',
      'cs.title': 'Der zentrale KI-Hub',
      'cs.b1': 'Das Problem',
      'cs.b1d': 'Vier Teams hatten ihre OpenAI-Aufrufe jeweils direkt in den eigenen Dienst verdrahtet. Vier Schl\u00fcsselstapel, vier Prompt-Stile, keine gemeinsame Kosten\u00fcbersicht \u2014 und keine Antwort auf die Frage der Rechtsabteilung: \u201eWas hat das Modell diesem Kunden gesagt, und warum?\u201c',
      'cs.b2': 'Die Randbedingung',
      'cs.b2d': 'Ein Stopp der Auslieferung f\u00fcr einen Umbau kam nicht in Frage. Die Teams hatten laufende Zusagen, also musste der Ersatz in unter einem Tag pro Team einf\u00fchrbar sein \u2014 sonst w\u00e4re er schlicht umgangen worden.',
      'cs.b3': 'Die Entscheidung',
      'cs.b3d': 'Ein Gateway-Dienst und die Regel, dass keine Anwendung direkt mit einem Modellanbieter spricht. Das Gateway besitzt Routing, Leitplanken, Kontingente je Team und den Pr\u00fcfpfad. Die Migration war eine ge\u00e4nderte Basis-URL plus ein Header \u2014 bewusst trivial, denn das Risiko lag in der Akzeptanz, nicht in der Technik.',
      'cs.b4': 'Der Kompromiss',
      'cs.b4d': 'Ein Gateway ist ein Single Point of Failure und kostet einen Netzwerk-Hop. Ich habe rund 40 ms zus\u00e4tzliche Latenz akzeptiert und mit einem \u00fcberwachten Aktiv-Aktiv-Paar abgesichert \u2014 eine kontrollierte Plattform mit Hop schl\u00e4gt vier unkontrollierte Integrationen ohne.',
      'cs.b5': 'Das Ergebnis',
      'cs.b5d': 'Jeder KI-Aufruf im Unternehmen ist heute auf Nutzer, Prompt und Quelle zur\u00fcckf\u00fchrbar. Modellkosten sind eine Position je Team statt einer \u00dcberraschung, und ein Anbieterwechsel ist eine Konfigurations\u00e4nderung statt vier paralleler Refactorings.',
      'cs.b6': 'Was ich anders machen w\u00fcrde',
      'cs.b6d': 'Ich habe das Routing vor den Evaluationen gebaut. Das war die falsche Reihenfolge \u2014 ohne Eval-Harness war der erste Modellwechsel eine Ermessensfrage statt einer Messung. Ich w\u00fcrde die Eval-Suite mit dem Gateway ausliefern, nicht zwei Monate danach.',

      'vf.label': 'Unabh\u00e4ngig pr\u00fcfen',
      'vf.gh': 'GitHub', 'vf.ghs': 'Code und Repositories',
      'vf.li': 'LinkedIn', 'vf.lis': 'Rollen, Zeitr\u00e4ume und Empfehlungen',
      'vf.yt': 'YouTube', 'vf.yts': 'Vortr\u00e4ge und Walkthroughs',
      'vf.up': 'Upwork', 'vf.ups': 'Auftragshistorie und Bewertungen',


    },

    ms: {
      'nav.home': 'Utama', 'nav.about': 'Tentang', 'nav.skills': 'Kemahiran',
      'nav.exp': 'Pengalaman', 'nav.ai': 'Makmal AI', 'nav.projects': 'Projek',
      'nav.contact': 'Hubungi', 'nav.hire': 'Upah Saya',

      'hero.badge': 'Terbuka untuk peluang baharu',
      'hero.r1': 'Pengurus Teknikal', 'hero.r2': 'Arkitek Awan',
      'hero.h1': 'Membina Sistem <span class="grad-text">Berkuasa AI</span><br>Yang Boleh Berkembang',
      'hero.p': 'Lebih 13 tahun mereka bentuk penyelesaian bertaraf perusahaan dengan <strong>RAG</strong>, <strong>LLM</strong>, <strong>Azure Cloud</strong>, <strong>.NET Core</strong> &amp; <strong>Angular</strong> — menukar data kompleks kepada keputusan yang boleh dilaksanakan.',
      'hero.s1': 'Tahun Peng.', 'hero.s2': 'Projek', 'hero.s3': 'Perusahaan',
      'hero.cta1': 'Lihat Kerja', 'hero.cta2': 'Mari Berbual',

      'about.tag': 'Tentang Saya',
      'about.h2': 'Menukar Data Kepada <span class="grad-text">Kecerdasan</span>',
      'about.years': 'Tahun<br>Kecemerlangan',
      'about.li': 'Profil LinkedIn',
      'about.lead': 'Saya membina <strong>sistem yang pintar, boleh berkembang dan ramalan</strong> — mereka bentuk seni bina AI termaju menggunakan RAG, LLM, AI Ramalan, Analisis Sentimen dan platform AI Foundry.',
      'about.p2': 'Pendekatan saya menggabungkan mikroperkhidmatan, reka bentuk dipacu peristiwa dan automasi berkuasa AI untuk menyampaikan penyelesaian awan berskala perusahaan. Saya cemerlang dalam penyampaian perisian hujung ke hujung — daripada seni bina penyelesaian dan integrasi API kepada saluran CI/CD, infrastruktur awan dan aliran kerja automatik.',
      'about.c1t': 'Pakar AI &amp; LLM', 'about.c1d': 'RAG, LLM, AI Ramalan, Analisis Sentimen &amp; AI Foundry',
      'about.c2t': 'Arkitek Azure', 'about.c2d': 'Data Fabric, Service Bus, Data Factory, Copilot Studio',
      'about.c3t': 'Pengurus Teknikal', 'about.c3d': 'Penyampaian perisian hujung ke hujung &amp; kepimpinan teknikal',
      'about.c4t': 'DevOps &amp; Automasi', 'about.c4d': 'CI/CD, IaC (ARM, Terraform, Bicep), GitHub Codespaces',
      'about.t1': 'Pembangunan Perisian Tersuai', 'about.t2': 'Pembangunan &amp; Integrasi API',
      'about.t3': 'Penyelesaian Awan &amp; Azure', 'about.t4': 'Seni Bina Mikroperkhidmatan',
      'about.t5': 'Pengoptimuman Prestasi', 'about.t6': 'Persediaan DevOps &amp; CI/CD',
      'about.t7': 'Pengoptimuman Pangkalan Data', 'about.t8': 'Semakan Kod &amp; Pengstrukturan',

      'sk.tag': 'Kepakaran Teknikal',
      'sk.h2': 'Teknologi Yang Saya <span class="grad-text">Kuasai</span>',
      'sk.desc': 'Himpunan kemahiran menyeluruh yang dibina lebih 13 tahun dalam kerjaya profesional',
      'sk.g1': 'AI &amp; Analitik', 'sk.g2': 'Awan &amp; Infrastruktur',
      'sk.g3': 'Pembangunan &amp; Seni Bina', 'sk.g4': 'DevOps &amp; Automasi',
      'sk.g5': 'AI Agentik &amp; MCP', 'sk.g6': 'LLMOps &amp; Tadbir Urus AI',
      'sk.g7': 'Seni Bina Perusahaan', 'sk.g8': 'Penyampaian &amp; Tadbir Urus',
      'ms.badge': 'Timbunan AI Microsoft',
      'ms.desc': 'Platform yang saya gunakan membina AI perusahaan setiap hari.',
      'ms.f': 'Platform data bersepadu', 'ms.af': 'Katalog &amp; penggunaan model',
      'ms.cp': 'Copilot &amp; ejen tersuai', 'ms.ao': 'Model GPT dalam penyewa sendiri',
      'ms.sk': 'Orkestrasi ejen .NET', 'ms.as': 'Perolehan vektor &amp; hibrid',
      'sk.yrs': 'thn', 'sk.yr': 'thn', 'sk.legend': 'Setiap lencana menunjukkan tahun pengalaman langsung dengan teknologi itu',

      'exp.tag': 'Perjalanan Kerjaya', 'exp.h2': '<span class="grad-text">Pengalaman</span> Profesional',
      'exp.desc': 'Lebih sedekad membina dan memimpin pasukan kejuruteraan',

      'ai.tag': 'Interaktif',
      'ai.h2': 'Lihat Cara Saya Membina <span class="grad-text">Sistem AI</span>',
      'ai.desc': 'Klik mana-mana peringkat untuk meneliti saluran RAG sebenar — seni bina yang sama di sebalik platform di bawah.',
      'ai.run': 'Jalankan Saluran', 'ai.reset': 'Set Semula', 'ai.try': 'Cuba satu soalan:',
      'ai.ph': 'Tanya sesuatu, cth. "Apakah polisi bayaran balik kami?"',
      'ai.st1': 'Soalan', 'ai.st1d': 'Soalan pengguna tiba dan dinormalkan, disemak ejaan serta diperluas dengan sinonim supaya carian tidak terjejas oleh gaya ayat.',
      'ai.st2': 'Benaman', 'ai.st2d': 'Soalan ditukar menjadi vektor — satu siri nombor yang menangkap makna dan bukan perkataan tepat, jadi "bayaran balik" turut sepadan dengan "pulangkan wang".',
      'ai.st3': 'Dapatkan', 'ai.st3d': 'Carian vektor memulangkan bahagian dokumen paling hampir. Penapis kebenaran dikenakan di sini, sebelum model, supaya pengguna tidak melihat data yang bukan haknya.',
      'ai.st4': 'Susun Semula', 'ai.st4d': 'Model kedua yang lebih tajam menyusun semula calon mengikut kaitan sebenar dan membuang padanan lemah. Inilah yang menghapuskan sebahagian besar risiko halusinasi.',
      'ai.st5': 'Jana', 'ai.st5d': 'LLM menjawab hanya menggunakan konteks yang diperoleh, dengan gesaan yang mewajibkannya memetik sumber dan berkata "saya tidak tahu" dan bukannya mereka-reka.',
      'ai.st6': 'Sahkan', 'ai.st6d': 'Setiap dakwaan disemak semula terhadap sumbernya. Jawapan yang tidak dapat disahkan ditahan, dan petikan sumber dilampirkan untuk audit.',
      'ai.hint': 'Klik satu peringkat di atas untuk melihat fungsinya',
      'ai.m1': 'Kependaman', 'ai.m2': 'Pengesahan', 'ai.m3': 'Kos / soalan',

      'pj.tag': 'Kerja Pilihan',
      'pj.h2': 'Projek Yang <span class="grad-text">Memberi Hasil</span>',
      'pj.desc': 'Penyelesaian sebenar untuk cabaran perniagaan sebenar merentas domain AI, Awan &amp; Perusahaan',
      'pj.f1': 'Semua', 'pj.f2': 'AI / ML', 'pj.f3': 'Awan', 'pj.f4': 'Apl Web', 'pj.f5': 'Desktop',

      'ct.tag': 'Hubungi Saya',
      'ct.h2': 'Mari Bina Sesuatu Yang <span class="grad-text">Hebat</span>',
      'ct.desc': 'Ada projek dalam fikiran atau ingin meneroka peluang kerjasama?',
      'ct.info': 'Maklumat Hubungan', 'ct.email': 'E-mel', 'ct.loc': 'Lokasi',
      'ct.wa': 'WhatsApp', 'ct.avail': 'Ketersediaan', 'ct.availv': 'Terbuka untuk kerja jarak jauh &amp; bebas',
      'ct.meet': 'Tetapkan Perjumpaan',
      'ct.name': 'Nama Penuh', 'ct.emailL': 'Alamat E-mel', 'ct.subj': 'Subjek', 'ct.msg': 'Mesej',
      'ct.send': 'Hantar Mesej',
      'ct.phName': 'Ahmad bin Ali', 'ct.phEmail': 'ahmad@contoh.com',
      'ct.phSubj': 'Pertanyaan Projek', 'ct.phMsg': 'Ceritakan tentang projek anda...',
      'imp.tag': 'Hasil Yang Diukur',
      'imp.h2': 'Hasil, Bukan <span class="grad-text">Baris Resume</span>',
      'imp.desc': 'Setiap nombor di bawah datang daripada sistem yang benar-benar dihantar dan kekal dalam produksi.',
      'imp.m1l': 'Semakan pematuhan lebih pantas', 'imp.m2l': 'Usaha pelaporan berkurang',
      'imp.m3l': 'Jurutera dipimpin', 'imp.m4l': 'Projek disiapkan',
      'imp.m5l': 'Pasukan di bawah pimpinan', 'imp.m6l': 'Tahun dalam kejuruteraan',

      'arc.tag': 'Pemikiran Sistem',
      'arc.h2': '<span class="grad-text">Timbunan AI Perusahaan</span> Yang Saya Bina',
      'arc.desc': 'Lima lapisan, satu platform bertadbir. Pilih satu lapisan untuk melihat isinya dan mengapa ia penting.',
      'arc.hint': 'Pilih satu lapisan untuk melihat kandungannya',
      'arc.l1': 'Lapisan Pengalaman', 'arc.l1c': 'Web \u00b7 Teams \u00b7 Suara \u00b7 API',
      'arc.l1d': 'Di sinilah pengguna benar-benar bertemu sistem. Satu kontrak perbualan merentas semua saluran, jadi saluran baharu hanyalah klien platform dan bukan penulisan semula.',
      'arc.l2': 'Orkestrasi &amp; Ejen', 'arc.l2c': 'Perancang \u00b7 Alat \u00b7 Aliran kerja \u00b7 Manusia dalam gelung',
      'arc.l2d': 'Ejen memecahkan matlamat, memanggil alat yang diluluskan dan menyerahkan kembali kawalan kepada manusia pada titik yang ditetapkan. Autonomi dibatasi oleh polisi, bukan harapan.',
      'arc.l3': 'Get Laluan AI', 'arc.l3c': 'Penghalaan \u00b7 Pagar keselamatan \u00b7 Kuota \u00b7 Kawalan kos',
      'arc.l3d': 'Setiap panggilan melalui satu get laluan. Ia memilih model termurah yang memenuhi mutu, menguatkuasakan pagar keselamatan, mengehadkan perbelanjaan setiap pasukan dan memudahkan penukaran pembekal.',
      'arc.l4': 'Pengetahuan &amp; Perolehan', 'arc.l4c': 'Pembahagian \u00b7 Benaman \u00b7 Carian vektor \u00b7 Penapis kebenaran',
      'arc.l4d': 'Teras RAG. Kebenaran dikenakan semasa perolehan, sebelum model melihat apa-apa \u2014 jadi model tidak mungkin membocorkan apa yang pengguna tiada hak membacanya.',
      'arc.l5': 'Tadbir Urus &amp; Pemerhatian', 'arc.l5c': 'Jejak audit \u00b7 Penilaian \u00b7 Pengesanan \u00b7 Amaran hanyutan',
      'arc.l5d': 'Setiap gesaan, perolehan dan jawapan dijejak dan boleh dimainkan semula. Penilaian automatik berjalan pada setiap perubahan, jadi kemerosotan mutu ditangkap sebelum pengguna menemuinya.',

      'rec.h3': 'Ringkasan Untuk Perekrut', 'rec.open': 'Terbuka untuk peluang',
      'rec.loc': 'Berpangkalan di', 'rec.locv': 'Kuala Lumpur, Malaysia \u00b7 sedia berpindah',
      'rec.focus': 'Fokus', 'rec.focusv': 'AI \u00b7 Seni bina awan \u00b7 Kepimpinan kejuruteraan',
      'rec.lang': 'Bahasa', 'rec.langv': 'Inggeris \u00b7 Urdu \u00b7 Melayu (asas)',
      'rec.setup': 'Cara bekerja', 'rec.setupv': 'Di pejabat \u00b7 Hibrid \u00b7 Jarak jauh',
      'rec.resp': 'Masa balasan', 'rec.respv': 'Dalam masa 24 jam',
      'rec.cv': 'Lihat Profil Penuh', 'rec.book': 'Tempah 30 Minit',
      'nav.impact': 'Impak',
      'cs.tag': 'Kajian Mendalam',
      'cs.h2': 'Satu Sistem, <span class="grad-text">Diceritakan Dengan Betul</span>',
      'cs.desc': 'Sesiapa pun boleh menyenaraikan teknologi. Ini ialah jejak keputusan di sebalik platform yang kini digunakan oleh setiap ciri AI dalam syarikat.',
      'cs.kicker': 'Aventra Group \u00b7 2025 \u00b7 Pengurus Teknikal',
      'cs.title': 'Hab AI Pusat',
      'cs.b1': 'Masalahnya',
      'cs.b1d': 'Empat pasukan masing-masing menyambung panggilan OpenAI terus ke dalam perkhidmatan sendiri. Empat set kunci, empat gaya gesaan, tiada pandangan perbelanjaan bersama, dan tiada cara menjawab soalan pihak undang-undang: "Apa yang model beritahu pelanggan itu, dan mengapa?"',
      'cs.b2': 'Kekangannya',
      'cs.b2d': 'Saya tidak boleh menghentikan penghantaran untuk membina semula. Pasukan mempunyai komitmen yang sedang berjalan, jadi pengganti panggilan terus itu mesti boleh diguna pakai dalam masa kurang sehari bagi setiap pasukan \u2014 jika tidak, ia akan dipintas begitu sahaja.',
      'cs.b3': 'Keputusannya',
      'cs.b3d': 'Satu perkhidmatan get laluan, dan satu peraturan: tiada aplikasi bercakap terus dengan pembekal model. Get laluan memiliki penghalaan, pagar keselamatan, kuota setiap pasukan dan jejak audit. Migrasi hanyalah penukaran URL asas dan satu pengepala \u2014 sengaja dibuat remeh, kerana risiko sebenar ialah penerimaan, bukan kejuruteraan.',
      'cs.b4': 'Pertukaran nilainya',
      'cs.b4d': 'Get laluan ialah titik kegagalan tunggal dan menambah satu lompatan rangkaian. Saya menerima kira-kira 40 ms kependaman tambahan dan membayarnya dengan pasangan aktif-aktif yang dipantau \u2014 kerana platform bertadbir dengan satu lompatan lebih baik daripada empat integrasi tanpa tadbir urus.',
      'cs.b5': 'Hasilnya',
      'cs.b5d': 'Setiap panggilan AI dalam syarikat kini boleh dijejak kepada pengguna, gesaan dan sumber yang diperoleh. Perbelanjaan model menjadi butiran setiap pasukan dan bukan kejutan, dan menukar pembekal ialah perubahan konfigurasi, bukan empat pengstrukturan selari.',
      'cs.b6': 'Apa yang saya akan ubah',
      'cs.b6d': 'Saya membina penghalaan sebelum penilaian. Itu susunan yang salah \u2014 tanpa rangka penilaian, penukaran model pertama ialah pertimbangan rasa, bukan ukuran. Saya akan menghantar suite penilaian bersama get laluan, bukan dua bulan selepasnya.',

      'vf.label': 'Sahkan secara bebas',
      'vf.gh': 'GitHub', 'vf.ghs': 'Kod dan repositori',
      'vf.li': 'LinkedIn', 'vf.lis': 'Peranan, tarikh dan cadangan',
      'vf.yt': 'YouTube', 'vf.yts': 'Ceramah dan panduan',
      'vf.up': 'Upwork', 'vf.ups': 'Sejarah klien dan ulasan',


    }
  };

  /* Project cards are injected separately to keep this file readable. */
  if (window.PROJECT_I18N) {
    ['en', 'de', 'ms'].forEach(function (lang) {
      var src = window.PROJECT_I18N[lang] || {};
      Object.keys(src).forEach(function (k) { DICT[lang][k] = src[k]; });
    });
  }

  var LANGS = { en: 'EN', de: 'DE', ms: 'MS' };
  var HTML_LANG = { en: 'en', de: 'de', ms: 'ms' };
  var STORE = 'zat-lang';
  var current = 'en';

  function pick(lang, key) {
    var table = DICT[lang] || DICT.en;
    // Fall back to English rather than showing a raw key if a string is missing.
    return table[key] !== undefined ? table[key] : DICT.en[key];
  }

  var decoder = document.createElement('textarea');

  /* Dictionary values are written as HTML source, so they carry entities like
     &amp;. textContent does not decode those, which would print the entity
     verbatim — decode before assigning to a text node. */
  function decode(value) {
    if (value.indexOf('&') === -1) return value;
    decoder.innerHTML = value;
    return decoder.value;
  }

  /* Translates one subtree. Fires no events, so callers that rebuild markup in
     response to a language change can use it without re-entering apply(). */
  function translate(root) {
    var scope = root || document;

    scope.querySelectorAll('[data-i18n]').forEach(function (el) {
      var v = pick(current, el.getAttribute('data-i18n'));
      if (v !== undefined) el.textContent = decode(v);
    });

    scope.querySelectorAll('[data-i18n-html]').forEach(function (el) {
      var v = pick(current, el.getAttribute('data-i18n-html'));
      if (v !== undefined) el.innerHTML = v;
    });

    // "data-i18n-attr" holds pairs like: placeholder:ct.phName,aria-label:nav.home
    scope.querySelectorAll('[data-i18n-attr]').forEach(function (el) {
      el.getAttribute('data-i18n-attr').split(',').forEach(function (pair) {
        var bits = pair.split(':');
        if (bits.length !== 2) return;
        var v = pick(current, bits[1].trim());
        if (v !== undefined) el.setAttribute(bits[0].trim(), decode(v));
      });
    });

    // Years badges keep their number but swap the unit word, singular included.
    var many = pick(current, 'sk.yrs');
    var one = pick(current, 'sk.yr');
    scope.querySelectorAll('.sk-years').forEach(function (el) {
      var n = el.getAttribute('data-years');
      el.textContent = n + ' ' + (n === '1' ? one : many);
    });
  }

  function apply(lang) {
    current = DICT[lang] ? lang : 'en';
    translate(document);

    document.documentElement.setAttribute('lang', HTML_LANG[current]);
    document.querySelectorAll('[data-lang-btn]').forEach(function (b) {
      b.classList.toggle('active', b.getAttribute('data-lang-btn') === current);
    });

    try { localStorage.setItem(STORE, current); } catch (e) { /* private mode */ }
    document.dispatchEvent(new CustomEvent('languagechange', { detail: { lang: current } }));
  }

  function initial() {
    var saved = null;
    try { saved = localStorage.getItem(STORE); } catch (e) { /* ignore */ }
    if (saved && DICT[saved]) return saved;
    var nav = (navigator.language || 'en').slice(0, 2).toLowerCase();
    if (nav === 'de') return 'de';
    if (nav === 'ms' || nav === 'id') return 'ms';   // Indonesian readers get Malay
    return 'en';
  }

  function buildSwitcher() {
    var host = document.getElementById('lang-switch');
    if (!host) return;
    Object.keys(LANGS).forEach(function (code) {
      var b = document.createElement('button');
      b.type = 'button';
      b.className = 'lang-btn';
      b.textContent = LANGS[code];
      b.setAttribute('data-lang-btn', code);
      b.setAttribute('aria-label', 'Switch language to ' + LANGS[code]);
      b.addEventListener('click', function () { apply(code); });
      host.appendChild(b);
    });
  }

  /* The markup ships in English, so harvest it as the English dictionary before
     anything is translated. Without this, keys that only exist in de/ms (every
     project card) would have no English entry, and switching back to EN would
     silently leave the previous language on screen. */
  function harvestEnglish() {
    document.querySelectorAll('[data-i18n]').forEach(function (el) {
      var k = el.getAttribute('data-i18n');
      if (DICT.en[k] === undefined) DICT.en[k] = el.textContent;
    });
    document.querySelectorAll('[data-i18n-html]').forEach(function (el) {
      var k = el.getAttribute('data-i18n-html');
      if (DICT.en[k] === undefined) DICT.en[k] = el.innerHTML;
    });
    document.querySelectorAll('[data-i18n-attr]').forEach(function (el) {
      el.getAttribute('data-i18n-attr').split(',').forEach(function (pair) {
        var bits = pair.split(':');
        if (bits.length !== 2) return;
        var k = bits[1].trim();
        if (DICT.en[k] === undefined) DICT.en[k] = el.getAttribute(bits[0].trim());
      });
    });
  }

  function start() {
    harvestEnglish();
    buildSwitcher();
    apply(initial());
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start);
  else start();

  window.ZatI18n = {
    apply: apply,
    translate: translate,
    get lang() { return current; }
  };
})();
