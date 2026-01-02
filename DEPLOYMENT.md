# Deployment Guide

Quick guide to deploy your portfolio website to various platforms.

## Option 1: GitHub Pages (Free & Easy)

1. **Create a GitHub repository**
   ```bash
   git init
   git add .
   git commit -m "Initial portfolio website"
   git branch -M main
   git remote add origin https://github.com/yourusername/portfolio.git
   git push -u origin main
   ```

2. **Enable GitHub Pages**
   - Go to repository Settings → Pages
   - Select `main` branch as source
   - Your site will be live at `https://yourusername.github.io/portfolio`

## Option 2: Netlify (Recommended - Free)

1. **Drag & Drop Method**
   - Go to [netlify.com](https://netlify.com)
   - Sign up/login
   - Drag your project folder to Netlify dashboard
   - Site is live instantly!

2. **Git Integration**
   - Connect your GitHub repository
   - Netlify auto-deploys on every push
   - Free SSL certificate included

## Option 3: Vercel (Free)

1. **Install Vercel CLI**
   ```bash
   npm i -g vercel
   ```

2. **Deploy**
   ```bash
   vercel
   ```
   - Follow the prompts
   - Your site is live!

## Option 4: Traditional Web Hosting

1. **Upload files via FTP**
   - Upload `index.html` and all assets
   - Ensure `index.html` is in root directory
   - Access via your domain

## Option 5: Azure Static Web Apps (Free Tier)

1. **Create Static Web App**
   - Go to Azure Portal
   - Create Static Web App resource
   - Connect GitHub repository
   - Auto-deploys on push

## Custom Domain Setup

### For GitHub Pages:
1. Add `CNAME` file with your domain name
2. Update DNS records:
   - Type: `CNAME`
   - Name: `www` (or `@`)
   - Value: `yourusername.github.io`

### For Netlify/Vercel:
1. Go to Domain Settings
2. Add your custom domain
3. Follow DNS configuration instructions

## Performance Optimization

### Before Deploying:

1. **Minify CSS & JavaScript**
   ```bash
   # Using npm packages
   npm install -g clean-css-cli terser
   clean-css-cli -o style.min.css style.css
   terser script.js -o script.min.js
   ```

2. **Optimize Images**
   - Use WebP format
   - Compress with tools like TinyPNG
   - Use appropriate sizes

3. **Enable Compression**
   - Most hosting platforms do this automatically
   - For custom servers, enable gzip/brotli

## Analytics Integration

### Google Analytics:
Add before `</head>`:
```html
<!-- Google tag (gtag.js) -->
<script async src="https://www.googletagmanager.com/gtag/js?id=GA_MEASUREMENT_ID"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'GA_MEASUREMENT_ID');
</script>
```

### Plausible Analytics (Privacy-friendly):
```html
<script defer data-domain="yourdomain.com" src="https://plausible.io/js/script.js"></script>
```

## SEO Optimization

1. **Update meta tags** in `<head>`:
```html
<meta name="description" content="Your professional description">
<meta name="keywords" content="software engineer, full-stack, cloud, AI">
<meta property="og:title" content="Your Name - Portfolio">
<meta property="og:description" content="Your description">
<meta property="og:image" content="https://yourdomain.com/og-image.jpg">
```

2. **Add structured data** (JSON-LD):
```html
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "Person",
  "name": "Your Name",
  "jobTitle": "Senior Software Engineer",
  "url": "https://yourdomain.com"
}
</script>
```

## Security Headers

Add to `.htaccess` (Apache) or server config:
```
Header set X-Content-Type-Options "nosniff"
Header set X-Frame-Options "DENY"
Header set X-XSS-Protection "1; mode=block"
Header set Referrer-Policy "strict-origin-when-cross-origin"
```

## Testing Checklist

- [ ] Test on mobile devices
- [ ] Test on different browsers (Chrome, Firefox, Safari, Edge)
- [ ] Check all links work
- [ ] Verify contact form (if implemented)
- [ ] Test dark/light mode toggle
- [ ] Check page load speed (aim for < 3 seconds)
- [ ] Validate HTML/CSS
- [ ] Test accessibility (keyboard navigation, screen readers)

## Continuous Deployment

### GitHub Actions Example:
Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy
on:
  push:
    branches: [ main ]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Deploy to Netlify
        uses: netlify/actions/cli@master
        with:
          args: deploy --prod
        env:
          NETLIFY_AUTH_TOKEN: ${{ secrets.NETLIFY_AUTH_TOKEN }}
          NETLIFY_SITE_ID: ${{ secrets.NETLIFY_SITE_ID }}
```

## Monitoring

Set up monitoring for:
- Uptime (UptimeRobot, Pingdom)
- Performance (Google PageSpeed Insights)
- Errors (Sentry, LogRocket)

## Backup

- Keep code in Git repository
- Regular backups of any backend/database
- Document custom configurations

---

**Need help?** Most hosting platforms have excellent documentation and support!