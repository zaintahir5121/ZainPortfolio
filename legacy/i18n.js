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
      'hero.r1': 'AI Technical Lead', 'hero.r2': 'Cloud Architect', 'hero.r3': 'Delivery Manager',
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
      'about.c3t': 'Delivery Manager', 'about.c3d': 'End-to-end software delivery &amp; technical leadership',
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
      'sk.yrs': 'yrs', 'sk.legend': 'Each badge shows hands-on years with that technology',

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
      'ct.phSubj': 'Project Inquiry', 'ct.phMsg': 'Tell me about your project...'
    },

    de: {
      'nav.home': 'Start', 'nav.about': 'Über mich', 'nav.skills': 'Fähigkeiten',
      'nav.exp': 'Erfahrung', 'nav.ai': 'KI-Labor', 'nav.projects': 'Projekte',
      'nav.contact': 'Kontakt', 'nav.hire': 'Anfragen',

      'hero.badge': 'Offen für neue Herausforderungen',
      'hero.r1': 'Technischer KI-Leiter', 'hero.r2': 'Cloud-Architekt', 'hero.r3': 'Delivery Manager',
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
      'about.c3t': 'Delivery Manager', 'about.c3d': 'Durchgängige Software-Lieferung &amp; technische Führung',
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
      'sk.yrs': 'J.', 'sk.legend': 'Jedes Abzeichen zeigt die Praxisjahre mit dieser Technologie',

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
      'ct.phSubj': 'Projektanfrage', 'ct.phMsg': 'Erzählen Sie mir von Ihrem Projekt …'
    },

    ms: {
      'nav.home': 'Utama', 'nav.about': 'Tentang', 'nav.skills': 'Kemahiran',
      'nav.exp': 'Pengalaman', 'nav.ai': 'Makmal AI', 'nav.projects': 'Projek',
      'nav.contact': 'Hubungi', 'nav.hire': 'Upah Saya',

      'hero.badge': 'Terbuka untuk peluang baharu',
      'hero.r1': 'Ketua Teknikal AI', 'hero.r2': 'Arkitek Awan', 'hero.r3': 'Pengurus Penyampaian',
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
      'about.c3t': 'Pengurus Penyampaian', 'about.c3d': 'Penyampaian perisian hujung ke hujung &amp; kepimpinan teknikal',
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
      'sk.yrs': 'thn', 'sk.legend': 'Setiap lencana menunjukkan tahun pengalaman langsung dengan teknologi itu',

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
      'ct.phSubj': 'Pertanyaan Projek', 'ct.phMsg': 'Ceritakan tentang projek anda...'
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

  /* Translates one subtree. Fires no events, so callers that rebuild markup in
     response to a language change can use it without re-entering apply(). */
  function translate(root) {
    var scope = root || document;

    scope.querySelectorAll('[data-i18n]').forEach(function (el) {
      var v = pick(current, el.getAttribute('data-i18n'));
      if (v !== undefined) el.textContent = v;
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
        if (v !== undefined) el.setAttribute(bits[0].trim(), v);
      });
    });

    // Years badges keep their number but swap the unit word.
    var unit = pick(current, 'sk.yrs');
    scope.querySelectorAll('.sk-years').forEach(function (el) {
      el.textContent = el.getAttribute('data-years') + ' ' + unit;
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
