# Modern Portfolio Website

A high-performance, modern portfolio website for Senior Software Engineers, AI/Cloud experts, and Full-Stack developers.

## Features

✨ **Modern Design**
- Clean, professional interface
- Smooth animations and transitions
- Dark/Light mode toggle
- Fully responsive (mobile, tablet, desktop)

🚀 **Performance Optimized**
- Lightweight and fast loading
- Lazy loading for images
- Optimized animations
- Minimal dependencies

🎯 **Key Sections**
- Hero section with animated background
- About Me with service offerings
- Skills showcase
- Work experience timeline
- Portfolio/Projects with demo placeholders
- Client testimonials carousel
- Contact form

## Quick Start

1. **Open the website**
   - Simply open `index.html` in your browser
   - Or deploy to any static hosting service

2. **Customize Content**
   - Edit the HTML directly in `index.html`
   - Update your information in each section
   - Replace placeholder images and links

3. **Deploy**
   - Upload to GitHub Pages
   - Deploy to Netlify, Vercel, or any static host
   - Or use traditional web hosting

## Customization Guide

### Change Colors
Edit the CSS variables in the `:root` section:
```css
:root {
    --primary-color: #2563eb;
    --secondary-color: #10b981;
    --accent-color: #f59e0b;
}
```

### Add Your Projects
In the Projects section, duplicate a `.project-card` and update:
- Project image/icon
- Title and description
- Tags (technologies used)
- Links to live demos or GitHub

### Add Real Demos
Replace the emoji icons in `.project-image` with:
- Screenshots: `<img src="path/to/image.jpg" alt="Project">`
- Embedded demos: `<iframe src="demo-url"></iframe>`
- Video demos: `<video>` tags

### Update Contact Form
The contact form currently shows an alert. To make it functional:
1. Add a backend endpoint (Node.js, PHP, Python, etc.)
2. Update the form submission handler in JavaScript
3. Or use a service like Formspree, Netlify Forms, or EmailJS

### Add More Sections
The structure is modular - simply duplicate a section and customize:
```html
<section id="new-section" class="section">
    <h2 class="section-title">New Section</h2>
    <!-- Your content here -->
</section>
```

## Performance Tips

1. **Optimize Images**
   - Use WebP format
   - Compress images before adding
   - Use appropriate sizes

2. **Minify Assets**
   - Minify CSS and JavaScript for production
   - Use tools like Terser or CSSNano

3. **CDN for Assets**
   - Host fonts and libraries on CDN
   - Use services like Cloudflare or jsDelivr

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## License

Free to use and modify for personal or commercial projects.

## Support

For questions or customization help, feel free to reach out!

---

**Built with ❤️ for showcasing technical expertise**