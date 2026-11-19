/* ---------------------------------------------------------------------------
   Project card translations. Kept apart from i18n.js so the main dictionary
   stays readable; i18n.js merges this in at load time.

   Keys: p{n}.t = title, p{n}.p = problem, p{n}.s = solution,
         p{n}.d = plain description (cards without a problem/solution split).
--------------------------------------------------------------------------- */
window.PROJECT_I18N = {

  en: {}, /* English lives in the markup already — nothing to override. */

  de: {
    'p1.t': 'KI-Plattform für Compliance-Prüfung',
    'p1.p': 'Manuelle Compliance-Prüfungen waren langsam, fehleranfällig und über Hunderte von Richtliniendokumenten nicht skalierbar.',
    'p1.s': 'Eine LLM-gestützte Plattform, die Dokumente automatisch liest, regulatorische Lücken markiert und strukturierte Prüfberichte erzeugt — 70 % weniger Prüfzeit.',

    'p2.t': 'KI-Generator für Lageberichte',
    'p2.p': 'Operative Teams verbrachten Stunden damit, Daten manuell zu Berichten zusammenzustellen — das verzögerte Entscheidungen und band Analysten.',
    'p2.s': 'Eine LLM-Engine, die operative Live-Daten aufnimmt und strukturierte Lageberichte automatisch erzeugt — 80 % weniger Aufwand.',

    'p3.t': 'Dokumenten-Chat mit KI',
    'p3.p': 'Mitarbeitende verloren Stunden bei der Suche in Tausenden Dokumenten nach Antworten, die in PDFs, Word-Dateien und Berichten vergraben waren.',
    'p3.s': 'Eine RAG-gestützte Chat-Oberfläche: Nutzer stellen einfach eine Frage und erhalten in Sekunden die exakte Antwort aus der Dokumentenbibliothek.',

    'p4.t': 'Agentische KI – Interner Ökosystem-Bot',
    'p4.p': 'Wiederkehrende interne Abläufe (Freigaben, Abfragen, Übergaben) erforderten ständige menschliche Koordination über mehrere Systeme hinweg.',
    'p4.s': 'Ein Multi-Agenten-System, das Aufgaben eigenständig plant, Werkzeuge aufruft und systemübergreifende Abläufe abschließt — das Team gewinnt Zeit für Wertschöpfung.',

    'p5.t': 'KI-basierte Performance-Optimierung',
    'p5.p': 'Engpässe fielen erst auf, wenn Nutzer sich beschwerten — reaktives Feuerlöschen war teuer und schädigte die SLAs.',
    'p5.s': 'Eine ML-Monitoring-Plattform, die Verschlechterungsmuster proaktiv erkennt und Korrekturen empfiehlt oder anwendet, bevor Nutzer etwas merken.',

    'p6.t': 'KI für Vertrieb &amp; Leadgenerierung',
    'p6.p': 'Vertriebsteams verbrachten 60 % ihrer Zeit mit unqualifizierten Leads und generischer Ansprache — mit entsprechend schwacher Conversion.',
    'p6.s': 'Eine KI-Pipeline, die Interessenten bewertet, personalisierte Ansprache per LLM verfasst und qualifizierte Leads direkt ins CRM einspeist.',

    'p7.t': 'KI-Kundenservice-Bot',
    'p7.p': 'Der Support war von wiederkehrenden Anfragen überlastet — lange Wartezeiten und hohe Betriebskosten waren die Folge.',
    'p7.s': 'Ein RAG-+-LLM-Chatbot, der 70 % der Anfragen eigenständig löst, den Gesprächskontext behält und rund um die Uhr verfügbar ist.',

    'p8.t': 'Predictive-Analytics-Engine',
    'p8.p': 'Geschäftsentscheidungen beruhten auf Bauchgefühl — fehlende Prognosen führten zu falschen Beständen und verpasstem Umsatz.',
    'p8.s': 'Eine Azure-ML-Plattform, die Nachfrage prognostiziert, Abwanderung vorhersagt und Kundenverhalten in Echtzeit-Power-BI-Dashboards sichtbar macht.',

    'p9.t': 'Anmeldung per Gesichtserkennung',
    'p9.p': 'Passwörter waren ein Sicherheitsrisiko — Brute-Force-Angriffe, geteilte Zugangsdaten und Zurücksetzungen erzeugten dauerhaft Risiko und Reibung.',
    'p9.s': 'Ein System aus Gesichtserkennung und Lebenderkennung für passwortlose, phishing-sichere Unternehmensauthentifizierung.',

    'p10.t': 'KI-System zur Routenoptimierung',
    'p10.p': 'Die manuelle Tourenplanung bei DHL berücksichtigte weder Echtzeitverkehr noch Kapazitäten — Fahrer fuhren suboptimale Routen, Kosten und Verzögerungen stiegen.',
    'p10.s': 'Ein KI-System, das Live-Daten aufnimmt und Lieferrouten dynamisch neu optimiert — geringere Spritkosten und mehr pünktliche Zustellungen.',

    'p11.t': 'Dashboard für Sentiment-Analyse',
    'p11.p': 'Kundenfeedback verteilte sich über E-Mails, Bewertungen und soziale Medien — Trends waren nicht erkennbar, Unzufriedenheit blieb unbeantwortet.',
    'p11.s': 'Eine NLP-Pipeline, die Feedback aus allen Kanälen bündelt und Stimmungstrends in Echtzeit in Power BI sichtbar macht.',

    'p12.t': 'Dokumentenmanagement-System',
    'p12.d': 'Mandantenfähiges DMS auf Unternehmensniveau mit rollenbasiertem Zugriff, PDF-Ansicht, Freigabe per E-Mail und WhatsApp sowie lückenloser Protokollierung.',

    'p13.t': 'URL-Kürzer &amp; Analytics',
    'p13.d': 'Eigener URL-Kürzungsdienst mit Klick-Tracking, Analyse-Dashboard, geografischen Auswertungen und AdSense-Anbindung.',

    'p14.t': 'Automatischer Sprachübersetzer',
    'p14.p': 'Sprachbarrieren behinderten die Verständigung zwischen globalen Teams und Nutzern in verschiedenen Regionen.',
    'p14.s': 'Eine NLP-basierte Übersetzungs-Engine für viele Sprachpaare — mit kontextbewusster Genauigkeit statt Wort-für-Wort.',

    'p15.t': 'Bild-zu-Video &amp; Textklassifizierung',
    'p15.p': 'Unstrukturierte Bild- und Textinhalte ließen sich ohne enormen manuellen Aufwand nicht in großem Maßstab kategorisieren.',
    'p15.s': 'Eine Deep-Learning-Pipeline, die Bilder klassifiziert, Bildfolgen in Videozusammenfassungen überführt und Textinhalte automatisch einordnet.',

    'p16.t': 'KI-Lebensmittelerkennung &amp; -klassifizierung',
    'p16.p': 'Modelle zur Lebensmittelerkennung von Grund auf zu trainieren, erforderte riesige annotierte Datensätze und erhebliche Rechenleistung.',
    'p16.s': 'Transfer Learning auf vortrainierten CNN-Modellen lieferte ein hochgenaues Erkennungs- und Klassifizierungssystem mit minimalen Trainingsdaten.',

    'p17.t': 'Krankheitsprognose für Patienten',
    'p17.p': 'Ärzten fehlten Werkzeuge, um Risikopatienten früh zu erkennen — Erkrankungen wurden aus fragmentierten Krankenakten oft zu spät entdeckt.',
    'p17.s': 'Ein ML-Modell, das die Krankengeschichte auswertet und Erkrankungswahrscheinlichkeiten vorhersagt — für frühe Intervention und Vorsorge.',

    'p18.t': 'Blockchain- &amp; KI-Integration',
    'p18.p': 'KI-Modellen fehlten Transparenz und fälschungssichere Prüfpfade — Unternehmen konnten KI-Entscheidungen weder vertrauen noch nachweisen.',
    'p18.s': 'Ein Framework, das das unveränderliche Blockchain-Ledger mit KI-Entscheidungsprotokollen verbindet und so nachprüfbare, vertrauenswürdige KI-Pipelines schafft.',

    'p19.t': 'Framework für Assoziationsanalyse',
    'p19.p': 'Verborgene Zusammenhänge zwischen Artikeln, Nutzern und Verhalten blieben unsichtbar — Cross-Selling-Chancen und Muster gingen verloren.',
    'p19.s': 'Ein Assoziations-Framework auf Basis graphbasierter Algorithmen, das Artikelbeziehungen, Verhaltensmuster und Empfehlungssignale sichtbar macht.',

    'p20.t': 'Pipeline für Datenaufbereitung &amp; -bereinigung',
    'p20.p': 'Verschmutzte, widersprüchliche Daten über Systeme hinweg ließen nachgelagerte Analysen scheitern und machten Berichte unzuverlässig.',
    'p20.s': 'Automatisierte ETL-Pipelines mit Validierung, Dublettenabgleich und Bereinigungsregeln — Analytik und ML-Modelle laufen stets auf geprüften Daten.',

    'p21.t': 'Automatische Anomalieerkennung',
    'p21.p': 'Betrügerische Transaktionen, Systemausfälle und Sicherheitsvorfälle blieben unbemerkt, bis erheblicher Schaden entstanden war.',
    'p21.s': 'Ein ML-gestütztes Anomalieerkennungssystem mit laufender Überwachung, automatischer Alarmierung und Genauigkeit, die sich über die Zeit selbst verbessert.',

    'p22.t': 'Kassensoftware (POS)',
    'p22.d': 'Vollständiges Kassensystem mit Lagerverwaltung, Barcode-Unterstützung, Verwaltung offener Posten und detaillierter Umsatzauswertung.',
    'p23.t': 'Mehrsprachiger Sprachassistent',
    'p23.p': 'Mitarbeitende an der Front in drei Ländern konnten die internen Wissenswerkzeuge nicht nutzen — es gab sie nur auf Englisch.',
    'p23.s': 'Ein Sprach-zu-Sprach-Assistent für Englisch, Malaiisch und Deutsch: transkribiert, antwortet aus demselben RAG-Index und spricht in der Sprache des Anrufers.',

    'p24.t': 'Engine zur Rechnungsauslesung',
    'p24.p': 'Die Buchhaltung erfasste monatlich Tausende Lieferantenrechnungen von Hand — Tippfehler fielen erst beim Abgleich auf.',
    'p24.s': 'Eine Vision-+-LLM-Pipeline, die jedes Rechnungslayout liest, Positionen als strukturiertes JSON extrahiert und Abweichungen vor der Buchung meldet — 92 % Dunkelverarbeitung.',

    'p25.t': 'Adaptive Lernplattform',
    'p25.p': 'Alle Lernenden erhielten denselben starren Kurs — Starke langweilten sich, Schwächere fielen unbemerkt zurück.',
    'p25.s': 'Eine Empfehlungs-Engine, die jede Antwort bewertet, den Lernstand modelliert und das nächste Modul in Echtzeit neu anordnet — 38 % mehr Kursabschlüsse.',

    'p26.t': 'Echtzeit-IoT-Telemetrie-Hub',
    'p26.p': 'Sensordaten von Tausenden Feldgeräten trafen in Schüben ein und überforderten den nächtlichen Batch-Lauf des Altsystems.',
    'p26.s': 'Ein ereignisgesteuerter Ingestion-Hub auf Azure Event Hubs und Service Bus, der in Zeitreihenspeicher streamt — mit Live-Dashboards und Alarmen im Sekundenbruchteil.',

    'p27.t': 'Analysator für Vertragsrisiken',
    'p27.p': 'Die juristische Prüfung von Lieferantenverträgen dauerte Tage pro Dokument — und unter Zeitdruck wurden riskante Klauseln trotzdem übersehen.',
    'p27.s': 'Ein LLM-Analysator, der jede Klausel gegen das freigegebene Playbook prüft, das Abweichungsrisiko bewertet und Änderungsvorschläge zur Freigabe durch Juristen entwirft.',

    'p28.t': 'Portfolio-Dashboard für Delivery',
    'p28.p': 'Der Führung fehlte ein einheitlicher Blick auf den Lieferstatus — er stammte aus alle zwei Wochen handgebauten Foliensätzen.',
    'p28.s': 'Ein Portfolio-Dashboard mit Live-Daten aus Azure DevOps und Git, das Durchsatz, Risiken und Abhängigkeitskonflikte über sechs Teams zeigt — ganz ohne Tabellenpflege.'
  },


  ms: {
    'p1.t': 'Platform Semakan Pematuhan AI',
    'p1.p': 'Semakan pematuhan secara manual adalah perlahan, mudah tersilap dan mustahil untuk diperluas merentas ratusan dokumen polisi.',
    'p1.s': 'Membina platform berkuasa LLM yang membaca dokumen secara automatik, menandakan jurang pengawalseliaan dan menjana laporan audit berstruktur — memotong masa semakan sebanyak 70%.',

    'p2.t': 'Penjana Laporan Serta-Merta AI',
    'p2.p': 'Pasukan operasi menghabiskan berjam-jam menyusun data ke dalam laporan secara manual, melambatkan keputusan dan membazirkan masa penganalisis.',
    'p2.s': 'Menyampaikan enjin LLM yang menyerap data operasi langsung dan menjana laporan berstruktur secara automatik — mengurangkan usaha sebanyak 80%.',

    'p3.t': 'Sembang Dokumen Menggunakan AI',
    'p3.p': 'Pekerja membazirkan berjam-jam mencari dalam ribuan dokumen untuk jawapan yang tersembunyi dalam fail PDF, Word dan laporan.',
    'p3.s': 'Membina antara muka sembang berkuasa RAG supaya pengguna hanya bertanya dan terus mendapat jawapan tepat daripada perpustakaan dokumen dalam beberapa saat.',

    'p4.t': 'AI Agentik – Bot Ekosistem Dalaman',
    'p4.p': 'Aliran kerja dalaman berulang (kelulusan, carian, penyerahan) memerlukan penyelarasan manusia berterusan merentas pelbagai sistem.',
    'p4.s': 'Mereka bentuk sistem AI berbilang ejen yang merancang tugas secara autonomi, memanggil alat dan melengkapkan aliran kerja merentas sistem — membebaskan pasukan untuk kerja bernilai tinggi.',

    'p5.t': 'Pengoptimuman Prestasi Berasaskan AI',
    'p5.p': 'Pasukan hanya menemui kesesakan prestasi selepas pengguna mengadu — tindakan reaktif ini mahal dan menjejaskan SLA.',
    'p5.s': 'Membina platform pemantauan ML yang mengesan corak kemerosotan secara proaktif dan mencadang atau melaksana pembetulan sebelum pengguna terjejas.',

    'p6.t': 'Penjana Jualan &amp; Prospek AI',
    'p6.p': 'Pasukan jualan menghabiskan 60% masa pada prospek tidak berkelayakan dan pendekatan umum, menyebabkan penukaran rendah dan usaha terbazir.',
    'p6.s': 'Membina saluran AI yang menilai prospek, menyusun pendekatan peribadi melalui LLM dan menyalurkan prospek berkelayakan terus ke dalam CRM.',

    'p7.t': 'Bot Sokongan Pelanggan AI',
    'p7.p': 'Pasukan sokongan dibanjiri pertanyaan berulang, menyebabkan masa menunggu yang panjang dan kos operasi yang tinggi.',
    'p7.s': 'Melaksanakan chatbot RAG + LLM yang mengendalikan 70% pertanyaan secara autonomi, dengan kesedaran konteks penuh dan ketersediaan 24/7.',

    'p8.t': 'Enjin Analitik Ramalan',
    'p8.p': 'Keputusan perniagaan dibuat berdasarkan naluri — ketiadaan ramalan menyebabkan ketidakpadanan inventori dan peluang hasil terlepas.',
    'p8.s': 'Membina platform Azure ML yang meramal permintaan, menjangka kehilangan pelanggan dan memaparkan corak tingkah laku dalam papan pemuka Power BI masa nyata.',

    'p9.t': 'Log Masuk Pengecaman Wajah',
    'p9.p': 'Log masuk berasaskan kata laluan adalah liabiliti keselamatan — serangan brute-force, perkongsian kelayakan dan set semula mencipta risiko dan geseran berterusan.',
    'p9.s': 'Menyampaikan sistem pengecaman wajah dan pengesanan hidup untuk pengesahan perusahaan tanpa kata laluan dan kalis pancingan data.',

    'p10.t': 'Sistem Pengoptimuman Laluan AI',
    'p10.p': 'Perancangan laluan manual DHL gagal mengambil kira trafik dan kapasiti masa nyata — pemandu mengikut laluan tidak optimum, meningkatkan kos dan kelewatan.',
    'p10.s': 'Merangka sistem AI yang menyerap data langsung dan mengoptimumkan semula laluan penghantaran secara dinamik — mengurangkan kos bahan api dan meningkatkan penghantaran tepat masa.',

    'p11.t': 'Papan Pemuka Analisis Sentimen',
    'p11.p': 'Maklum balas pelanggan bertaburan merentas e-mel, ulasan dan media sosial — tiada cara untuk mengesan aliran atau bertindak pantas terhadap rasa tidak puas hati.',
    'p11.s': 'Membina saluran NLP yang menyerap maklum balas pelbagai saluran dan memaparkan aliran sentimen masa nyata dalam Power BI untuk tindakan segera.',

    'p12.t': 'Sistem Pengurusan Dokumen',
    'p12.d': 'DMS bertaraf perusahaan berbilang penyewa dengan akses berasaskan peranan, paparan PDF, perkongsian e-mel &amp; WhatsApp serta jejak audit.',

    'p13.t': 'Pemendek URL &amp; Analitik',
    'p13.d': 'Perkhidmatan pemendekan URL tersuai dengan penjejakan klik, papan pemuka analitik, cerapan geografi dan integrasi AdSense.',

    'p14.t': 'Penterjemah Bahasa Automatik',
    'p14.p': 'Halangan bahasa menyekat komunikasi berkesan antara pasukan global dan pengguna merentas wilayah.',
    'p14.s': 'Mereka bentuk enjin terjemahan automatik berasaskan NLP yang menyokong pelbagai pasangan bahasa dengan ketepatan sedar konteks.',

    'p15.t': 'Imej-ke-Video &amp; Pengelas Teks',
    'p15.p': 'Kandungan visual dan teks tidak berstruktur mustahil dikategorikan secara besar-besaran tanpa usaha manual yang sangat besar.',
    'p15.s': 'Membina saluran pembelajaran mendalam yang mengelaskan imej, menukar urutan imej kepada ringkasan video dan mengkategorikan kandungan teks secara automatik.',

    'p16.t': 'Pengesan &amp; Pengelas Makanan AI',
    'p16.p': 'Melatih model pengecaman makanan dari awal memerlukan set data berlabel yang sangat besar dan sumber pengiraan yang ketara.',
    'p16.s': 'Menggunakan pembelajaran pindahan pada model CNN pra-latih untuk membina sistem pengesanan dan pengelasan makanan berketepatan tinggi dengan data latihan minimum.',

    'p17.t': 'Peramal Penyakit Manusia',
    'p17.p': 'Pengamal perubatan kekurangan alat untuk mengenal pasti pesakit berisiko tinggi lebih awal — penyakit sering dikesan terlalu lewat daripada sejarah perubatan yang berselerak.',
    'p17.s': 'Membangunkan model ML yang menganalisis sejarah perubatan pesakit untuk meramal kebarangkalian penyakit, membolehkan campur tangan awal dan penjagaan pencegahan.',

    'p18.t': 'Integrasi Blockchain &amp; AI',
    'p18.p': 'Model AI kekurangan ketelusan dan jejak audit kalis usik — perusahaan tidak dapat mempercayai atau mengesahkan keputusan yang dipacu AI.',
    'p18.s': 'Menyiasat dan melaksanakan rangka kerja yang menggabungkan lejar kekal blockchain dengan pengelogan keputusan AI bagi mencipta saluran AI yang boleh disahkan dan dipercayai.',

    'p19.t': 'Rangka Kerja Platform Perkaitan',
    'p19.p': 'Hubungan tersembunyi antara item, pengguna dan tingkah laku tidak kelihatan — perniagaan terlepas peluang jualan silang dan corak berguna.',
    'p19.s': 'Membina rangka kerja analisis perkaitan menggunakan algoritma berasaskan graf untuk memaparkan hubungan item, corak tingkah laku dan isyarat cadangan.',

    'p20.t': 'Saluran Pemprosesan &amp; Pembersihan Data',
    'p20.p': 'Data kotor dan tidak konsisten merentas sistem menyebabkan kegagalan analitik hiliran dan laporan perniagaan yang tidak boleh dipercayai.',
    'p20.s': 'Membina saluran ETL automatik dengan pengesahan, penyingkiran pendua dan peraturan pembersihan — memastikan analitik dan model ML sentiasa berjalan atas data yang disahkan.',

    'p21.t': 'Pengesanan Anomali Automatik',
    'p21.p': 'Transaksi penipuan, kegagalan sistem dan pelanggaran keselamatan tidak disedari sehingga kerosakan besar telah pun berlaku.',
    'p21.s': 'Membangunkan sistem pengesanan anomali dipacu ML dengan pemantauan berterusan, amaran automatik dan ketepatan yang menambah baik diri dari semasa ke semasa.',

    'p22.t': 'Perisian Tempat Jualan (POS)',
    'p22.d': 'Sistem POS lengkap dengan pengurusan inventori, sokongan kod bar, penjejakan hutang dan pelaporan jualan terperinci.',
    'p23.t': 'Pembantu Suara Pelbagai Bahasa',
    'p23.p': 'Kakitangan barisan hadapan di tiga negara tidak dapat menggunakan alat pengetahuan dalaman kerana ia wujud dalam bahasa Inggeris sahaja.',
    'p23.s': 'Membina pembantu suara-ke-suara yang mengendalikan bahasa Inggeris, Melayu dan Jerman — menyalin pertuturan, menjawab daripada indeks RAG yang sama dan membalas dalam bahasa pemanggil.',

    'p24.t': 'Enjin Pengekstrakan Invois',
    'p24.p': 'Bahagian kewangan memasukkan ribuan invois pembekal secara manual setiap bulan, dan kesilapan taip hanya muncul semasa penyesuaian akaun.',
    'p24.s': 'Menyampaikan saluran penglihatan + LLM yang membaca sebarang susun atur invois, mengekstrak butiran ke JSON berstruktur dan menandakan ketidakpadanan sebelum pos — 92% pemprosesan terus.',

    'p25.t': 'Platform Pembelajaran Adaptif',
    'p25.p': 'Setiap pelajar menerima kursus tetap yang sama, jadi pelajar cemerlang berasa bosan manakala yang lemah ketinggalan tanpa disedari.',
    'p25.s': 'Membina enjin cadangan yang menilai setiap jawapan, memodelkan pelajar dan menyusun semula modul seterusnya secara masa nyata — meningkatkan kadar tamat kursus sebanyak 38%.',

    'p26.t': 'Hab Telemetri IoT Masa Nyata',
    'p26.p': 'Data penderia daripada ribuan peranti lapangan tiba secara mendadak sehingga membebankan kerja kelompok malam sistem lama.',
    'p26.s': 'Merangka hab penyerapan dipacu peristiwa pada Azure Event Hubs dan Service Bus, menstrim ke storan siri masa dengan papan pemuka langsung dan amaran bawah sesaat.',

    'p27.t': 'Penganalisis Risiko Kontrak',
    'p27.p': 'Semakan undang-undang bagi kontrak vendor mengambil masa berhari-hari setiap dokumen, dan klausa berisiko masih terlepas pandang apabila tarikh akhir mendesak.',
    'p27.s': 'Membina penganalisis LLM yang membandingkan setiap klausa dengan buku panduan diluluskan, menilai risiko penyimpangan dan merangka cadangan pindaan untuk keputusan peguam.',

    'p28.t': 'Papan Pemuka Portfolio Penyampaian',
    'p28.p': 'Pihak pengurusan tiada pandangan tunggal tentang kesihatan penyampaian — status datang daripada slaid yang disusun secara manual setiap dua minggu.',
    'p28.s': 'Membina papan pemuka portfolio yang menarik data langsung daripada Azure DevOps dan Git, memaparkan hasil kerja, risiko dan pertembungan kebergantungan merentas enam pasukan tanpa sesiapa mengemas kini hamparan.'
  }

};
