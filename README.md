# Zain Abbas Tahir - Professional Portfolio Website

A modern, high-performance personal portfolio website for a Senior Software / AI / Cloud Engineer.

## 🚀 Features

### Design & UI
- **Modern Glassmorphism Design** - Beautiful frosted glass effects and layered UI
- **Dark/Light Mode** - System-aware theme with manual toggle
- **Responsive Layout** - Optimized for all devices (mobile, tablet, desktop)
- **Smooth Animations** - Scroll-triggered animations, hover effects, and transitions
- **Interactive Elements** - Cursor glow effect, parallax scrolling, and more

### Sections
1. **Hero Section**
   - Animated code window with typing effect
   - Tech orbit animation showing expertise
   - Animated statistics counters
   - Social media links

2. **About Section**
   - Professional photo with decorative elements
   - Experience highlights with icons
   - Services offered with tag cloud

3. **Skills Section**
   - Categorized skill display (Backend, Frontend, Cloud, Database)
   - Animated skill progress bars
   - Tech stack icons from CDN

4. **Experience Timeline**
   - Interactive timeline with scroll animations
   - Technology tags for each role
   - Company logos and descriptions

5. **Projects Portfolio**
   - Filterable project grid (All, Web, Cloud, Desktop, AI)
   - Featured project highlighting
   - Hover overlay with project links
   - Project statistics

6. **Testimonials Slider**
   - Auto-playing testimonial carousel
   - Navigation dots and arrows
   - Author avatars and ratings

7. **Contact Section**
   - Animated contact form
   - Multiple contact methods
   - Hire platform links (LinkedIn, Upwork, Freelancer)
   - Calendly integration for scheduling

### Performance
- Minimal dependencies (only Font Awesome icons)
- CSS animations using GPU-accelerated transforms
- Intersection Observer for lazy loading animations
- Optimized for Core Web Vitals

## 📁 Project Structure

```
/workspace/
├── index.html      # Main HTML structure
├── styles.css      # All styles with CSS variables
├── script.js       # Interactive functionality
└── README.md       # Documentation
```

## 🛠️ Technologies Used

- **HTML5** - Semantic markup with accessibility features
- **CSS3** - Custom properties, Grid, Flexbox, animations
- **JavaScript** - ES6+, Intersection Observer API
- **Font Awesome 6** - Icon library
- **Google Fonts** - Inter & JetBrains Mono

## 🚦 Getting Started

### Quick Start

1. **Clone/Download** the repository
2. **Open** `index.html` in your browser
3. That's it! No build process required.

### Local Development

For local development with live reload:

```bash
# Using Python 3
python -m http.server 8000

# Using Node.js (with npx)
npx serve

# Using PHP
php -S localhost:8000
```

Then open `http://localhost:8000` in your browser.

### Deployment

This is a static website that can be deployed to:
- **GitHub Pages** - Free hosting
- **Netlify** - Free with CI/CD
- **Vercel** - Free with CI/CD
- **Any web server** - Apache, Nginx, etc.

## ⚙️ Customization

### Theme Colors

Edit the CSS variables in `styles.css`:

```css
:root {
    --accent-primary: #6366f1;
    --accent-secondary: #8b5cf6;
    --accent-tertiary: #a855f7;
}
```

### Content

Update the content in `index.html`:
- Replace placeholder text with your information
- Update image sources
- Modify social media links
- Add your own projects

### Contact Form

The contact form currently simulates submission. To make it functional:

1. **Use Formspree** (recommended):
   ```html
   <form action="https://formspree.io/f/YOUR_ID" method="POST">
   ```

2. **Use Netlify Forms**:
   ```html
   <form netlify data-netlify="true">
   ```

3. **Custom Backend**:
   Modify the form submission in `script.js`

## 📱 Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers (iOS Safari, Chrome for Android)

## 🎨 Design Highlights

- **Cursor Glow Effect** - Follows mouse movement for immersive feel
- **Floating Shapes** - Background elements with parallax effect
- **Code Window Animation** - Live typing effect in hero section
- **Tech Orbit** - Animated technology icons orbiting
- **Skill Bars** - Progress bars that animate on scroll
- **Timeline Animation** - Fade-in effect for experience items

## 📄 License

This project is open source and available under the MIT License.

## 📞 Contact

- **Email**: zabbastahir@gmail.com
- **LinkedIn**: [zainabbastahir](https://www.linkedin.com/in/zainabbastahir/)
- **Website**: [zainabbastahir.com](https://zainabbastahir.com)

---

Built with ❤️ and lots of ☕ by Zain Abbas Tahir
