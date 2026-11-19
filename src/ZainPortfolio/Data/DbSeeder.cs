using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Models;
using ZainPortfolio.Services;

namespace ZainPortfolio.Data;

/// <summary>
/// Applies migrations and seeds first-run content (ported from the original static site).
/// Every seed block is guarded so re-running is safe and never overwrites edited content.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IConfiguration config, ILogger logger)
    {
        // Migrations are generated against SQL Server (the production/IIS target) and cannot be
        // replayed on SQLite, so the local-dev provider builds its schema from the model instead.
        if (db.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
            await db.Database.EnsureCreatedAsync();
        else
            await db.Database.MigrateAsync();

        await SeedAdminAsync(db, config, logger);
        await SeedSettingsAsync(db);
        await SeedProjectCategoriesAsync(db);
        await SeedProjectsAsync(db);
        await SeedBlogAsync(db);
        await SeedSkillsAsync(db);
        await SeedExperienceAsync(db);

        await db.SaveChangesAsync();
    }

    private static async Task SeedAdminAsync(ApplicationDbContext db, IConfiguration config, ILogger logger)
    {
        if (await db.AdminUsers.AnyAsync()) return;

        var username = config["Admin:Username"] ?? "admin";
        var email = config["Admin:Email"] ?? "zain.tahir512@gmail.com";
        var password = config["Admin:Password"];
        var generated = false;

        if (string.IsNullOrWhiteSpace(password))
        {
            password = PasswordHasher.GenerateReadablePassword();
            generated = true;
        }

        var (hash, salt) = PasswordHasher.Hash(password);
        db.AdminUsers.Add(new AdminUser
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            DisplayName = "Zain Abbas Tahir",
            MustChangePassword = generated
        });
        await db.SaveChangesAsync();

        if (generated)
        {
            logger.LogWarning(
                "=================================================================\n" +
                " ADMIN ACCOUNT CREATED\n" +
                "   Login : /admin\n" +
                "   User  : {User}\n" +
                "   Pass  : {Pass}\n" +
                "   >> Change this immediately after first sign-in.\n" +
                "=================================================================",
                username, password);
        }
    }

    private static async Task SeedSettingsAsync(ApplicationDbContext db)
    {
        if (await db.SiteSettings.AnyAsync()) return;
        db.SiteSettings.Add(new SiteSetting
        {
            Id = 1,
            ProfileImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJ3f1yDIsYGrqA-NrCoi7LPZU6lWGuzHmcgLXo-Dqp2g&s=10",
            UpworkUrl = "https://www.upwork.com/freelancers/~013b69c81fcb1ae708",
            CalendlyUrl = "https://calendly.com/zainabbastahir/30min",
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedProjectCategoriesAsync(ApplicationDbContext db)
    {
        if (await db.ProjectCategories.AnyAsync()) return;
        db.ProjectCategories.AddRange(
            new ProjectCategory { Name = "AI / ML", Slug = "ai", SortOrder = 1 },
            new ProjectCategory { Name = "Cloud", Slug = "cloud", SortOrder = 2 },
            new ProjectCategory { Name = "Web Apps", Slug = "web", SortOrder = 3 },
            new ProjectCategory { Name = "Desktop", Slug = "desktop", SortOrder = 4 });
        await db.SaveChangesAsync();
    }

    private record Seed(string Title, string Icon, string From, string To, string Tags,
                        string? Problem, string? Solution, string? Summary,
                        string Filters, string S1i, string S1t, string S2i, string S2t, bool Featured);

    private static async Task SeedProjectsAsync(ApplicationDbContext db)
    {
        if (await db.Projects.AnyAsync()) return;

        var seeds = new List<Seed>
        {
            new("AI Compliance Review Platform", "fas fa-shield-halved", "#0891b2", "#6366f1",
                "Azure OpenAI,RAG,LLMs,.NET Core",
                "Manual compliance reviews were slow, error-prone and impossible to scale across hundreds of policy documents.",
                "Built an LLM-powered platform that auto-reads documents, flags regulatory gaps and generates structured audit reports — cutting review time by 70%.",
                "LLM platform that reads policy documents, flags regulatory gaps and generates audit-ready reports.",
                "ai cloud", "fas fa-file-contract", "Audit Ready", "fas fa-robot", "LLM Powered", true),

            new("AI Spot Report Generator", "fas fa-file-lines", "#f59e0b", "#ef4444",
                "Azure OpenAI,LLMs,Cosmos DB",
                "Operations teams spent hours manually compiling data into reports, delaying decisions and wasting analyst time.",
                "Delivered an LLM engine that ingests live operational data and auto-generates structured spot reports — reducing effort by 80%.",
                "LLM engine turning live operational data into structured reports automatically.",
                "ai cloud", "fas fa-bolt", "80% Faster", "fas fa-chart-bar", "Auto Reports", true),

            new("Document Chat Using AI", "fas fa-comments", "#10b981", "#0ea5e9",
                "RAG,AI Search,Azure OpenAI,Angular",
                "Employees wasted hours searching through thousands of documents to find answers buried in PDFs, Word files and reports.",
                "Built a RAG-powered chat UI so users simply ask a question and get exact answers from the document library in seconds.",
                "RAG chat interface over a multi-format enterprise document library with source-grounded answers.",
                "ai cloud", "fas fa-magnifying-glass", "Semantic Search", "fas fa-file-arrow-up", "Multi-format", true),

            new("Agentic AI – Internal Ecosystem Bot", "fas fa-network-wired", "#7c3aed", "#6366f1",
                "Agentic AI,Copilot Studio,Azure OpenAI",
                "Repetitive internal workflows (approvals, lookups, handoffs) required constant human coordination across multiple systems.",
                "Designed a multi-agent AI system that autonomously plans tasks, calls tools and completes cross-system workflows — freeing teams for high-value work.",
                "Multi-agent system that plans tasks, calls tools and completes cross-system workflows end to end.",
                "ai cloud", "fas fa-gears", "Multi-Agent", "fas fa-sitemap", "Orchestrated", true),

            new("AI Based Performance Optimization", "fas fa-gauge", "#f97316", "#eab308",
                "Azure ML,App Insights,Power BI",
                "Teams only discovered performance bottlenecks after users complained — reactive firefighting was costly and damaging to SLAs.",
                "Built an ML monitoring platform that proactively detects degradation patterns and auto-recommends or applies fixes before users are impacted.",
                "ML-driven monitoring that detects degradation patterns and recommends fixes proactively.",
                "ai cloud", "fas fa-chart-line", "Auto-Optimize", "fas fa-eye", "Real-time", false),

            new("AI Sales & Lead Generator", "fas fa-handshake", "#22c55e", "#0ea5e9",
                "Azure OpenAI,LLMs,CRM APIs,Angular",
                "Sales teams spent 60% of their time on unqualified leads and generic outreach, resulting in low conversion and wasted effort.",
                "Engineered an AI pipeline that scores prospects, crafts personalised outreach via LLMs and feeds qualified leads directly into the CRM.",
                "AI pipeline scoring prospects and generating personalised outreach into the CRM.",
                "ai cloud", "fas fa-user-plus", "Lead Scoring", "fas fa-envelope-open", "Auto Outreach", false),

            new("AI Customer Support Bot", "fas fa-robot", "#6366f1", "#a78bfa",
                "RAG,Azure OpenAI,LLMs",
                "Support teams were overwhelmed with repetitive queries, causing long wait times and high operational cost.",
                "Deployed a RAG + LLM chatbot that handles 70% of queries autonomously, with full context awareness and 24/7 availability.",
                "RAG + LLM chatbot handling 70% of support queries autonomously.",
                "ai cloud", "fas fa-robot", "AI Powered", "fas fa-comments", "24/7 Support", true),

            new("Predictive Analytics Engine", "fas fa-chart-line", "#0ea5e9", "#6366f1",
                "Azure ML,Data Factory,Power BI",
                "Business decisions were made on gut feeling — lack of forecasting led to inventory mismatches and missed revenue opportunities.",
                "Built an Azure ML platform that forecasts demand, predicts churn and surfaces customer behaviour patterns in real-time Power BI dashboards.",
                "Azure ML platform forecasting demand and churn through real-time Power BI dashboards.",
                "ai cloud", "fas fa-chart-line", "Forecasting", "fas fa-brain", "ML Models", false),

            new("Face Recognition Login", "fas fa-smile", "#8b5cf6", "#ec4899",
                "AI/ML,Python,.NET",
                "Password-based logins were a security liability — brute-force attacks, credential sharing and resets created constant risk and friction.",
                "Delivered a facial recognition + liveness detection system for passwordless, phish-proof enterprise authentication.",
                "Facial recognition with liveness detection for passwordless enterprise authentication.",
                "ai", "fas fa-eye", "Face Detection", "fas fa-lock", "Secure Auth", false),

            new("AI Route Optimization System", "fas fa-route", "#00d4ff", "#6366f1",
                "Azure AI,Microservices,.NET Core",
                "Manual route planning failed to account for real-time traffic and capacity — drivers followed suboptimal routes, increasing cost and delays.",
                "Architected an AI system that ingests live data and dynamically re-optimises delivery routes — cutting fuel costs and improving on-time delivery.",
                "Live-data AI system dynamically re-optimising delivery routes at global logistics scale.",
                "ai cloud", "fas fa-location-dot", "Smart Routing", "fas fa-truck", "Logistics", true),

            new("Sentiment Analysis Dashboard", "fas fa-heart-pulse", "#ec4899", "#f43f5e",
                "NLP,Python,Power BI,Azure",
                "Customer feedback was scattered across emails, reviews and social media — no way to spot trends or act on dissatisfaction quickly.",
                "Built an NLP pipeline that ingests multi-channel feedback and surfaces real-time sentiment trends in Power BI for immediate action.",
                "NLP pipeline surfacing real-time multi-channel sentiment trends in Power BI.",
                "ai cloud", "fas fa-face-smile-beam", "NLP Powered", "fas fa-chart-pie", "Real-time", false),

            new("Document Management System", "fas fa-folder-open", "#334155", "#6366f1",
                ".NET Core,Azure,Angular", null, null,
                "Enterprise-grade multi-tenant DMS with role-based access, PDF viewing, email & WhatsApp sharing and audit trails.",
                "web cloud", "fas fa-users", "Multi-tenant", "fas fa-shield-alt", "Secure", false),

            new("URL Shortener & Analytics", "fas fa-link", "#0369a1", "#0ea5e9",
                ".NET Core,MVC,Analytics", null, null,
                "Custom URL shortening service with click tracking, analytics dashboard, geographic insights and AdSense integration.",
                "web", "fas fa-link", "Short URLs", "fas fa-chart-bar", "Click Stats", false),

            new("Automated Language Translator", "fas fa-language", "#0891b2", "#10b981",
                "NLP,Python,Deep Learning",
                "Language barriers blocked effective communication between global teams and users across regions.",
                "Designed an NLP-based automated translation engine supporting multi-language pairs with context-aware accuracy.",
                "NLP translation engine supporting multi-language pairs with context-aware accuracy.",
                "ai", "fas fa-globe", "Multi-language", "fas fa-bolt", "Real-time", false),

            new("Image-to-Video & Text Classifier", "fas fa-film", "#7c3aed", "#ec4899",
                "Computer Vision,Python,TensorFlow",
                "Unstructured visual and textual content was impossible to categorise at scale without massive manual effort.",
                "Built a deep learning pipeline that classifies images, converts image sequences to video summaries and categorises text content automatically.",
                "Deep learning pipeline classifying images and generating video summaries automatically.",
                "ai", "fas fa-image", "Vision AI", "fas fa-tags", "Auto-classify", false),

            new("AI Food Detector & Classifier", "fas fa-utensils", "#f97316", "#22c55e",
                "Transfer Learning,CNN,Python",
                "Training food recognition models from scratch required huge labelled datasets and significant compute resources.",
                "Applied transfer learning on pre-trained CNN models to build a high-accuracy food detection and classification system with minimal training data.",
                "Transfer learning on pre-trained CNNs for high-accuracy food detection with minimal data.",
                "ai", "fas fa-brain", "Transfer Learning", "fas fa-camera", "Image Recognition", false),

            new("Human Disease Predictor", "fas fa-heart-pulse", "#ef4444", "#f97316",
                "ML,Python,Healthcare AI",
                "Clinicians lacked tools to identify high-risk patients early — diseases were often caught too late from fragmented medical histories.",
                "Developed an ML model that analyses patient medical history to predict disease likelihood, enabling early intervention and preventive care.",
                "ML model predicting disease likelihood from patient history for early intervention.",
                "ai", "fas fa-stethoscope", "Healthcare AI", "fas fa-chart-line", "Predictive", false),

            new("Blockchain & AI Integration", "fas fa-cube", "#f59e0b", "#6366f1",
                "Blockchain,AI,Python",
                "AI models lacked transparency and tamper-proof audit trails — enterprises couldn't trust or verify AI-driven decisions.",
                "Implemented a framework combining blockchain's immutable ledger with AI decision logging to create verifiable, trustworthy AI pipelines.",
                "Framework combining an immutable ledger with AI decision logging for verifiable AI.",
                "ai", "fas fa-link", "Immutable Audit", "fas fa-shield-halved", "Trustworthy AI", false),

            new("Association Platform Framework", "fas fa-diagram-project", "#0ea5e9", "#10b981",
                "Graph Analytics,Python,ML",
                "Hidden relationships between items, users and behaviours were invisible — businesses missed cross-sell and pattern opportunities.",
                "Built an association analysis framework using graph-based algorithms to surface item relationships, behavioural patterns and recommendation signals.",
                "Graph-based association analysis surfacing relationships and recommendation signals.",
                "ai cloud", "fas fa-diagram-project", "Graph Analysis", "fas fa-lightbulb", "Insights", false),

            new("Data Processing & Cleansing Pipeline", "fas fa-database", "#475569", "#0ea5e9",
                "Azure Data Factory,Python,SQL",
                "Dirty, inconsistent data across systems caused downstream analytics failures and unreliable business reports.",
                "Built automated ETL pipelines with validation, deduplication and cleansing rules — ensuring analytics and ML models always run on trusted data.",
                "Automated ETL pipelines with validation, deduplication and cleansing rules.",
                "ai cloud", "fas fa-filter", "Auto Cleanse", "fas fa-check-double", "Verified Data", false),

            new("Automated Anomaly Detection", "fas fa-satellite-dish", "#dc2626", "#7c3aed",
                "ML,Azure Monitor,Python,.NET",
                "Fraudulent transactions, system failures and security breaches went unnoticed until significant damage had already occurred.",
                "Developed an ML-driven anomaly detection system with continuous monitoring, auto-alerting and self-improving accuracy over time.",
                "ML anomaly detection with continuous monitoring and self-improving accuracy.",
                "ai cloud", "fas fa-bell", "Real-time Alerts", "fas fa-shield-halved", "Auto-detect", false),

            new("Point of Sale Software", "fas fa-cash-register", "#57534e", "#a78bfa",
                "C#,WinForms,SQL Server", null, null,
                "Complete POS system with inventory management, barcode support, debt tracking, and detailed sales reporting.",
                "desktop", "fas fa-barcode", "Barcode Ready", "fas fa-chart-line", "Analytics", false),
        };

        var cats = await db.ProjectCategories.ToDictionaryAsync(c => c.Slug, c => c.Id);
        var order = 0;
        foreach (var s in seeds)
        {
            var primary = s.Filters.Split(' ')[0];
            db.Projects.Add(new Project
            {
                Title = s.Title,
                Slug = SlugHelper.Generate(s.Title),
                Summary = s.Summary,
                Problem = s.Problem,
                Solution = s.Solution,
                Content = BuildProjectBody(s),
                IconClass = s.Icon,
                GradientFrom = s.From,
                GradientTo = s.To,
                Tags = s.Tags,
                FilterKeys = s.Filters,
                Stat1Icon = s.S1i, Stat1Text = s.S1t,
                Stat2Icon = s.S2i, Stat2Text = s.S2t,
                ProjectCategoryId = cats.TryGetValue(primary, out var cid) ? cid : null,
                IsFeatured = s.Featured,
                IsPublished = true,
                SortOrder = order++
            });
        }
        await db.SaveChangesAsync();
    }

    private static string BuildProjectBody(Seed s)
    {
        var tags = string.Join(", ", s.Tags.Split(','));
        var body = $"<h2>Overview</h2><p>{s.Summary}</p>";
        if (!string.IsNullOrWhiteSpace(s.Problem))
            body += $"<h2>The Challenge</h2><p>{s.Problem}</p>";
        if (!string.IsNullOrWhiteSpace(s.Solution))
            body += $"<h2>The Solution</h2><p>{s.Solution}</p>";
        body += $"<h2>Technology</h2><p>{tags}</p>";
        return body;
    }

    private static async Task SeedBlogAsync(ApplicationDbContext db)
    {
        if (await db.BlogCategories.AnyAsync()) return;

        var cats = new[]
        {
            new BlogCategory { Name = "AI Engineering", Slug = "ai-engineering", SortOrder = 1,
                Description = "RAG, LLMs, agents and everything around shipping AI to production." },
            new BlogCategory { Name = "Cloud & Azure", Slug = "cloud-azure", SortOrder = 2,
                Description = "Architecture, cost and operations on Microsoft Azure." },
            new BlogCategory { Name = "Engineering Leadership", Slug = "engineering-leadership", SortOrder = 3,
                Description = "Scaling teams, delegation and technical management." },
            new BlogCategory { Name = ".NET & Architecture", Slug = "dotnet-architecture", SortOrder = 4,
                Description = "Clean architecture, microservices and practical .NET." },
        };
        db.BlogCategories.AddRange(cats);
        await db.SaveChangesAsync();

        if (await db.BlogPosts.AnyAsync()) return;

        var posts = new[]
        {
            new BlogPost
            {
                Title = "Why Every AI Feature Should Go Through One Gateway",
                Slug = "why-every-ai-feature-should-go-through-one-gateway",
                Excerpt = "Six squads integrating Azure OpenAI directly means six implementations of credentials, guardrails, cost tracking and evaluation. Here is the architecture I used instead.",
                BlogCategoryId = cats[0].Id,
                Tags = "AI Hub,RAG,Architecture,Governance",
                ReadMinutes = 8,
                IsFeatured = true,
                Content = @"<p>When we started adding AI features across multiple squads, the obvious path was for each team to integrate Azure OpenAI directly. I pushed back — and the reason had nothing to do with the model.</p>
<h2>The problem with direct integration</h2>
<p>Every squad that calls a provider SDK directly has to solve the same hard problems independently: credential handling, guardrails, prompt versioning, cost attribution, evaluation, and redaction before data leaves your boundary. Multiply that by six teams and you get six inconsistent implementations — and you can never change providers.</p>
<h2>The AI Hub</h2>
<p>Instead we built a single internal API. Nothing calls a model provider directly. The Hub is four layers:</p>
<ul>
<li><strong>Gateway</strong> — one API. Every request carries a feature id, a correlation id and a data classification.</li>
<li><strong>Orchestration</strong> — RAG, structured extraction, and a multi-agent planner. Squads pick a capability rather than building plumbing.</li>
<li><strong>Model layer</strong> — provider abstraction over Azure OpenAI plus self-hosted open-weight models.</li>
<li><strong>Governance</strong> — prompt registry, evaluation harness, guardrails, redaction, per-feature spend limits.</li>
</ul>
<h2>The payoff</h2>
<p>Adding an AI feature became a squad-level decision instead of an architecture decision. That is what made delegation possible.</p>"
            },
            new BlogPost
            {
                Title = "Permission-Aware RAG: Filter Before the Model, Not After",
                Slug = "permission-aware-rag-filter-before-the-model",
                Excerpt = "The single most common security mistake in retrieval-augmented generation — and why post-generation filtering is already a data breach.",
                BlogCategoryId = cats[0].Id,
                Tags = "RAG,Security,Multi-tenant,Vector Search",
                ReadMinutes = 6,
                IsFeatured = true,
                Content = @"<p>If your RAG pipeline filters results by user permission <em>after</em> generation, you have already leaked the data. The model saw it. It may be in your logs, your traces and your cache.</p>
<h2>Where the filter belongs</h2>
<p>The permission and tenant filter must sit between the query and the vector search — before any content reaches the model:</p>
<p><code>query → permission + tenant filter → vector search → context → model</code></p>
<h2>What this actually requires</h2>
<p>Your tenancy and RBAC model has to extend down into the vector layer, not stop at the application. Every chunk carries its access scope as metadata captured at ingestion time, and every retrieval applies that scope as a hard pre-filter.</p>
<h2>Testing it</h2>
<p>Include negative cases in your evaluation dataset: a user who should see nothing must get nothing, and the feature should say it does not know rather than answering from model knowledge.</p>"
            },
            new BlogPost
            {
                Title = "The Decision Authority Matrix: How I Stopped Being a Bottleneck",
                Slug = "decision-authority-matrix-how-i-stopped-being-a-bottleneck",
                Excerpt = "Going from one team to six meant every technical decision routed through me. Here is the written artefact that fixed it.",
                BlogCategoryId = cats[2].Id,
                Tags = "Leadership,Delegation,Engineering Management,SOP",
                ReadMinutes = 7,
                IsFeatured = true,
                Content = @"<p>In the early stage it is workable for every design review, requirement clarification and deployment approval to route through one person. At six squads it is not — it slows delivery, creates a single point of failure, and stops tech leads growing into real owners.</p>
<h2>The fix was written, not verbal</h2>
<p>I built a decision authority matrix that states exactly which decisions a tech lead makes alone, which they recommend, and which they escalate. Three levels:</p>
<ul>
<li><strong>DECIDE</strong> — proceed, no approval needed.</li>
<li><strong>RECOMMEND</strong> — prepare the option; manager approves.</li>
<li><strong>ESCALATE</strong> — decided above you.</li>
</ul>
<h2>Deliberately weighted toward DECIDE</h2>
<p>Production releases (through gates), rollbacks, schema changes inside their own service, prompt tuning within an approved feature — all DECIDE. Service boundaries, breaking contracts, new providers — RECOMMEND. Security, tenancy, guardrails, anything disciplinary — ESCALATE.</p>
<h2>The rule that makes it work</h2>
<p>A tech lead should never be blocked waiting on me for a decision the handbook already answers. If it does not answer it, that is a gap — raise it and we add it.</p>"
            },
            new BlogPost
            {
                Title = "Cutting AI Run-Rate Without Cutting Capability",
                Slug = "cutting-ai-run-rate-without-cutting-capability",
                Excerpt = "Token spend is real operating cost. Five design decisions that kept ours flat while feature count grew.",
                BlogCategoryId = cats[1].Id,
                Tags = "Cost,LLMOps,Azure OpenAI,Architecture",
                ReadMinutes = 5,
                Content = @"<p>Most AI cost problems are design problems, not pricing problems. Five things that mattered most:</p>
<ol>
<li><strong>Start from the smallest capable model.</strong> Classification, extraction and routing rarely need a frontier model. Escalate only when evaluation proves the smaller one falls short.</li>
<li><strong>Trim the context.</strong> Sending a whole document where three chunks would do is the single largest source of avoidable spend.</li>
<li><strong>Cache aggressively.</strong> Paying twice for an identical request is a defect, not a cost of doing business.</li>
<li><strong>Attribute per feature.</strong> Without a feature id on every request you discover overruns from the invoice.</li>
<li><strong>Set hard limits.</strong> Every feature gets a spend cap and an alert before it.</li>
</ol>
<p>Combined with self-hosted open-weight models for the high-volume, low-complexity paths, this kept run-rate controlled as the portfolio grew.</p>"
            },
            new BlogPost
            {
                Title = "Clean Architecture in .NET: The Two Checks That Catch Most Drift",
                Slug = "clean-architecture-dotnet-two-checks-that-catch-most-drift",
                Excerpt = "You do not need an architecture review to find layering violations. Two ten-second checks find the majority of them.",
                BlogCategoryId = cats[3].Id,
                Tags = ".NET,Clean Architecture,Code Review,Blazor",
                ReadMinutes = 4,
                Content = @"<p>The dependency rule in Clean Architecture is simple: source code dependencies point inwards only. Nothing in an inner layer knows anything about an outer layer. Enforcing it is where teams struggle.</p>
<h2>Check one: open the Domain project file</h2>
<p>If it references any package beyond the base framework — EF Core, ASP.NET, anything — the rule is already broken. The Domain layer holds entities, value objects and business rules, and it should compile with no infrastructure at all.</p>
<h2>Check two: open a controller or a Blazor component</h2>
<p>If it contains a business rule or a database call, the rule is broken in the other direction. Presentation orchestrates; it does not decide and it does not persist.</p>
<h2>Why these two</h2>
<p>Drift almost always starts at the edges — a quick query in a component because it was faster, or a package added to Domain for a convenience type. Catch those and the middle usually holds.</p>"
            },
        };

        foreach (var (p, i) in posts.Select((p, i) => (p, i)))
        {
            p.PublishedAt = DateTime.UtcNow.AddDays(-7 * i);
            p.CreatedAt = p.PublishedAt;
            p.UpdatedAt = p.PublishedAt;
            db.BlogPosts.Add(p);
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedSkillsAsync(ApplicationDbContext db)
    {
        if (await db.SkillCategories.AnyAsync()) return;

        void Add(string name, string icon, int order, params (string n, int p)[] items)
        {
            var cat = new SkillCategory { Name = name, IconClass = icon, SortOrder = order };
            var i = 0;
            foreach (var (n, p) in items)
                cat.Skills.Add(new Skill { Name = n, Percentage = p, SortOrder = i++ });
            db.SkillCategories.Add(cat);
        }

        Add("AI & Analytics", "fas fa-brain", 1,
            ("RAG", 95), ("LLMs", 92), ("Agentic AI", 90), ("Predictive AI", 90),
            ("Sentiment Analysis", 88), ("AI Foundry", 90), ("Copilot Studio", 92));
        Add("Cloud & Infrastructure", "fas fa-cloud", 2,
            ("Microsoft Azure", 95), ("Azure Service Bus", 92), ("Data Factory", 90),
            ("Kubernetes", 88), ("Key Vault", 90), ("Entra ID & RBAC", 88));
        Add("Development & Architecture", "fas fa-code", 3,
            (".NET / .NET Core", 95), ("Angular 17+", 92), ("Blazor", 88), ("React", 85),
            ("Microservices", 93), ("CQRS / Event-Driven", 90));
        Add("DevOps & Automation", "fas fa-rocket", 4,
            ("CI/CD Pipelines", 93), ("Azure DevOps", 92), ("Terraform", 88),
            ("Docker", 90), ("TDD", 90), ("ARM / Bicep", 85));

        await db.SaveChangesAsync();
    }

    private static async Task SeedExperienceAsync(ApplicationDbContext db)
    {
        if (await db.Experiences.AnyAsync()) return;

        db.Experiences.AddRange(
            new Experience
            {
                Role = "Technical Manager / Cloud Architect", Company = "Aventra Group",
                Location = "Kuala Lumpur, Malaysia", DateRange = "Nov 2025 – Present", IsCurrent = true,
                SortOrder = 1,
                Bullets = "Lead an engineering organisation of 40+ developers across three specialist squads through embedded tech leads\n"
                        + "Act as senior technical decision-maker: target architecture, platform standards, build-vs-buy evaluations\n"
                        + "Established the AI engineering practice from the ground up — reference architectures, evaluation, security and cost guardrails\n"
                        + "Architected an LLM-powered compliance review platform, cutting manual review time by ~70%\n"
                        + "Own cloud architecture and cost posture on Azure with infrastructure delivered as code via Bicep and ARM",
                Tags = "RAG,LLMs,AI Search,.NET Core,Angular,Azure,Microservices,Copilot Studio"
            },
            new Experience
            {
                Role = "Technical Lead", Company = "DHL IT Services",
                Location = "Kuala Lumpur, Malaysia · Hybrid", DateRange = "Sep 2022 – Oct 2025", SortOrder = 2,
                Bullets = "Led cross-functional teams delivering enterprise logistics platforms for a global operation\n"
                        + "Defined and delivered an AI-based route optimisation system using live traffic and capacity data\n"
                        + "Architected a RAG-based global search capability across a large fragmented document estate\n"
                        + "Owned the Azure footprint — Service Bus, Data Factory, Logic Apps, Function Apps, Cosmos DB and Redis\n"
                        + "Raised engineering standards through code review governance, TDD adoption and architecture documentation",
                Tags = ".NET Core,Angular 18,Azure,RAG,Cosmos DB,Redis,Microservices"
            },
            new Experience
            {
                Role = "Senior Software Engineer / Lead", Company = "Tapcheck",
                Location = "Remote (US-based fintech)", DateRange = "2019 – 2022", SortOrder = 3,
                Bullets = "Led onboarding of new enterprise payroll integrations, owning the integration architecture\n"
                        + "Directed peer code review and established coding standards adopted by the wider team\n"
                        + "Built customer-facing interfaces in Angular over an ASP.NET Core and REST API backend",
                Tags = ".NET Core,Angular,REST APIs,SQL Server"
            },
            new Experience
            {
                Role = "Senior Software Engineer — AI/ML", Company = "UMCH Tech",
                Location = "Kuala Lumpur, Malaysia", DateRange = "2018 – 2019", SortOrder = 4,
                Bullets = "Delivered production AI/ML capabilities including facial and emotion recognition APIs\n"
                        + "Created intelligent chatbots and automated translation pipelines\n"
                        + "Built automated anomaly detection with continuous performance tracking",
                Tags = "Python,Machine Learning,AI/ML,NLP"
            },
            new Experience
            {
                Role = "Software Architect", Company = "MTBC (CareCloud)",
                Location = "Islamabad, Pakistan", DateRange = "2016 – 2018", SortOrder = 5,
                Bullets = "Owned architecture for regulated healthcare software with HIPAA-aligned data handling\n"
                        + "Produced impact assessments with effort and cost estimation for executive investment decisions\n"
                        + "Mentored developers and ran technical training, building internal capability",
                Tags = "ASP.NET MVC,Entity Framework,Healthcare IT,HIPAA"
            },
            new Experience
            {
                Role = "Software Engineer", Company = "Interactive Group & Moftak Solutions",
                Location = "Islamabad, Pakistan", DateRange = "2013 – 2016", SortOrder = 6,
                Bullets = "Delivered full-lifecycle web applications and backend APIs for diverse enterprise clients\n"
                        + "Built robust APIs and backend services using ASP.NET MVC, C# and SQL Server",
                Tags = "ASP.NET,C#,JavaScript,SQL Server"
            });

        await db.SaveChangesAsync();
    }
}
