# Zain Portfolio CMS

A full portfolio + blog + project showcase with a built-in content management system.
One ASP.NET Core 8 MVC project, SQL Server, deployable to IIS.

Everything on the public site — content, images, video, theme colours, logo, SEO — is
editable from `/admin`. No code changes or redeploys needed to update the site.

---

## Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC (single project) |
| Data | Entity Framework Core 8 + SQL Server |
| Auth | Cookie authentication, PBKDF2-SHA256 (210k iterations) |
| Editor | Custom rich text editor with media picker |
| Sanitisation | HtmlSanitizer (whitelist-based) |
| Hosting | IIS (in-process) — `web.config` included |

---

## Features

### Public site
- Dynamic home page: hero, about, skills, experience timeline, featured projects, latest posts, FAQ, contact
- **Projects** — filterable grid plus a full case-study page per project at `/projects/{slug}`
- **Blog** — categories, tags, search, pagination, featured post, related posts, prev/next
- Newsletter subscription (home, blog, article and footer)
- Contact form writing to the database
- Dark / light theme toggle, fully responsive, reduced-motion aware

### CMS (`/admin`)
- **Dashboard** — visitors, page views, subscribers, messages, traffic chart, top pages, devices, referrers, content performance
- **Projects** — full CRUD with rich text, problem/solution fields, gallery, gradient or image covers
- **Blog** — full CRUD with rich text, cover image, video embed, per-post SEO fields
- **Media library** — drag-in upload for images, video and PDF; picker built into every editor
- **Site & Theme** — brand, colours, fonts, radius, hero, about, contact, SEO, feature toggles
- **Skills & Experience** — inline editing of the home page timeline and skill bars
- **Subscribers** — list, activate/deactivate, CSV export
- **Messages** — inbox for contact form submissions

### SEO (targeted at Malaysia)
- XML sitemap at `/sitemap.xml`, generated from the database
- `/robots.txt` and RSS feed at `/rss.xml`
- Canonical URLs, Open Graph, Twitter cards, hreflang
- Geo meta targeting Kuala Lumpur (`geo.region`, `geo.position`, `ICBM`)
- JSON-LD structured data: Person, WebSite, ProfessionalService, BlogPosting, CreativeWork, BreadcrumbList, FAQPage
- Long-tail keyword defaults — easier to rank than generic head terms
- Per-post meta title and description overrides

### Built-in analytics
First-party page-view tracking with bot filtering, cookie-based unique visitors,
device and referrer breakdown, and hashed IPs (no raw IP is stored).

---

## Running locally

The app runs on SQLite in Development so you don't need SQL Server installed:

```bash
cd src/ZainPortfolio
dotnet run
```

Then open `http://localhost:5088` and sign in at `/admin` with the credentials in
`appsettings.Development.json` (`admin` / `Admin@12345`).

To run against SQL Server locally, set `Database:Provider` to `SqlServer` and update
the connection string.

---

## Deploying to IIS

1. **Publish**

   ```bash
   dotnet publish src/ZainPortfolio/ZainPortfolio.csproj -c Release -o ./publish
   ```

2. **Create the database.** Point `ConnectionStrings:DefaultConnection` at your SQL Server
   instance. Migrations run automatically on first start, and seed data is inserted only
   when the tables are empty.

3. **Configure `appsettings.Production.json`** on the server (this file is git-ignored):

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ZainPortfolio;User Id=...;Password=...;TrustServerCertificate=True"
     },
     "Database": { "Provider": "SqlServer" },
     "Admin": {
       "Username": "admin",
       "Email": "you@example.com",
       "Password": "SET-A-STRONG-PASSWORD"
     }
   }
   ```

   If `Admin:Password` is left blank, a strong password is generated on first run and
   written **once** to the application log. Change it immediately at `/admin/account/password`.

4. **IIS setup**
   - Install the [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) on the server
   - Create a site pointing at the publish folder
   - Set the application pool to **No Managed Code**
   - Give the app pool identity **write access to `wwwroot/uploads`** — uploads fail without it

5. **After go-live**
   - Set the canonical base URL in **Admin → Settings → SEO** (required for correct sitemap and Open Graph URLs)
   - Submit `/sitemap.xml` to Google Search Console and Bing Webmaster Tools
   - Add your Google Analytics ID and site verification codes in the same tab

---

## Project structure

```
src/ZainPortfolio/
├── Areas/Admin/          CMS controllers and views
├── Controllers/          Public site + sitemap/robots/RSS + subscribe
├── Data/                 DbContext, migrations, seeder
├── Models/               Entities and view models
├── Services/             Settings cache, SEO, analytics, media, sanitiser, hashing
├── Views/                Public Razor views
├── wwwroot/              CSS, JS, uploads
└── web.config            IIS configuration
```

`legacy/` holds the original static HTML site, kept for reference.

---

## Security notes

- Admin area is `noindex` and disallowed in `robots.txt`
- All rich text passes through a whitelist sanitiser before storage
- File uploads are validated by extension, size-capped at 64 MB, and stored with generated names
- Anti-forgery tokens on every state-changing form
- Analytics stores a salted hash of the IP address, never the address itself
