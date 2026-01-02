// ==========================================
// PORTFOLIO WEBSITE - JAVASCRIPT
// ==========================================

// DOM Elements
const navbar = document.getElementById('navbar');
const navMenu = document.getElementById('nav-menu');
const navLinks = document.querySelectorAll('.nav-link');
const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
const themeToggle = document.getElementById('theme-toggle');
const cursorGlow = document.getElementById('cursor-glow');
const backToTop = document.getElementById('back-to-top');
const contactForm = document.getElementById('contact-form');

// ==========================================
// INITIALIZATION
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    initTheme();
    initNavigation();
    initScrollEffects();
    initHeroAnimations();
    initSkillsAnimation();
    initTimelineAnimation();
    initProjectsFilter();
    initTestimonialsSlider();
    initContactForm();
    initCursorGlow();
    initCounterAnimation();
});

// ==========================================
// THEME TOGGLE
// ==========================================
function initTheme() {
    // Check for saved theme preference
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    if (savedTheme) {
        document.documentElement.setAttribute('data-theme', savedTheme);
    } else if (prefersDark) {
        document.documentElement.setAttribute('data-theme', 'dark');
    }
    
    // Theme toggle click handler
    themeToggle.addEventListener('click', () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
    });
}

// ==========================================
// NAVIGATION
// ==========================================
function initNavigation() {
    // Mobile menu toggle
    mobileMenuToggle.addEventListener('click', () => {
        navMenu.classList.toggle('active');
        mobileMenuToggle.classList.toggle('active');
        document.body.classList.toggle('menu-open');
    });
    
    // Close mobile menu on link click
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            navMenu.classList.remove('active');
            mobileMenuToggle.classList.remove('active');
            document.body.classList.remove('menu-open');
        });
    });
    
    // Scroll spy for active nav links
    const sections = document.querySelectorAll('section[id]');
    
    function updateActiveNavLink() {
        const scrollPos = window.scrollY + 150;
        
        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.offsetHeight;
            const sectionId = section.getAttribute('id');
            
            if (scrollPos >= sectionTop && scrollPos < sectionTop + sectionHeight) {
                navLinks.forEach(link => {
                    link.classList.remove('active');
                    if (link.getAttribute('href') === `#${sectionId}`) {
                        link.classList.add('active');
                    }
                });
            }
        });
    }
    
    window.addEventListener('scroll', updateActiveNavLink);
}

// ==========================================
// SCROLL EFFECTS
// ==========================================
function initScrollEffects() {
    let lastScrollY = 0;
    
    window.addEventListener('scroll', () => {
        const currentScrollY = window.scrollY;
        
        // Navbar background on scroll
        if (currentScrollY > 50) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
        
        // Back to top button visibility
        if (currentScrollY > 500) {
            backToTop.classList.add('visible');
        } else {
            backToTop.classList.remove('visible');
        }
        
        lastScrollY = currentScrollY;
    });
    
    // Back to top click handler
    backToTop.addEventListener('click', () => {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
    
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });
}

// ==========================================
// HERO ANIMATIONS
// ==========================================
function initHeroAnimations() {
    // Typing effect for code window
    const codeContent = document.querySelector('.window-content code');
    if (codeContent) {
        const originalHTML = codeContent.innerHTML;
        codeContent.innerHTML = '';
        
        let charIndex = 0;
        const typeSpeed = 10;
        
        function typeCode() {
            if (charIndex < originalHTML.length) {
                // Handle HTML tags
                if (originalHTML[charIndex] === '<') {
                    const endTag = originalHTML.indexOf('>', charIndex);
                    codeContent.innerHTML += originalHTML.substring(charIndex, endTag + 1);
                    charIndex = endTag + 1;
                } else {
                    codeContent.innerHTML += originalHTML[charIndex];
                    charIndex++;
                }
                setTimeout(typeCode, typeSpeed);
            }
        }
        
        // Start typing after a delay
        setTimeout(typeCode, 500);
    }
}

// ==========================================
// COUNTER ANIMATION
// ==========================================
function initCounterAnimation() {
    const counters = document.querySelectorAll('.stat-number');
    
    const animateCounter = (counter) => {
        const target = parseInt(counter.getAttribute('data-count'));
        const duration = 2000;
        const step = target / (duration / 16);
        let current = 0;
        
        const updateCounter = () => {
            current += step;
            if (current < target) {
                counter.textContent = Math.floor(current);
                requestAnimationFrame(updateCounter);
            } else {
                counter.textContent = target;
            }
        };
        
        updateCounter();
    };
    
    // Intersection Observer for counters
    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.classList.contains('animated')) {
                entry.target.classList.add('animated');
                animateCounter(entry.target);
            }
        });
    }, { threshold: 0.5 });
    
    counters.forEach(counter => {
        counterObserver.observe(counter);
    });
}

// ==========================================
// SKILLS ANIMATION
// ==========================================
function initSkillsAnimation() {
    const skillCards = document.querySelectorAll('.skill-card');
    
    const skillObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
                const skillLevel = entry.target.getAttribute('data-skill');
                entry.target.style.setProperty('--skill-level', `${skillLevel}%`);
            }
        });
    }, { threshold: 0.5 });
    
    skillCards.forEach(card => {
        skillObserver.observe(card);
    });
}

// ==========================================
// TIMELINE ANIMATION
// ==========================================
function initTimelineAnimation() {
    const timelineItems = document.querySelectorAll('.timeline-item');
    
    const timelineObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, { threshold: 0.2 });
    
    timelineItems.forEach(item => {
        timelineObserver.observe(item);
    });
}

// ==========================================
// PROJECTS FILTER
// ==========================================
function initProjectsFilter() {
    const filterButtons = document.querySelectorAll('.filter-btn');
    const projectCards = document.querySelectorAll('.project-card');
    
    filterButtons.forEach(button => {
        button.addEventListener('click', () => {
            // Update active button
            filterButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');
            
            const filter = button.getAttribute('data-filter');
            
            // Filter projects
            projectCards.forEach(card => {
                const categories = card.getAttribute('data-category');
                
                if (filter === 'all' || (categories && categories.includes(filter))) {
                    card.style.display = 'block';
                    card.style.animation = 'fadeIn 0.5s ease forwards';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
}

// ==========================================
// TESTIMONIALS SLIDER
// ==========================================
function initTestimonialsSlider() {
    const slides = document.querySelectorAll('.testimonial-card');
    const dots = document.querySelectorAll('.nav-dots .dot');
    const prevBtn = document.querySelector('.nav-btn.prev');
    const nextBtn = document.querySelector('.nav-btn.next');
    
    let currentSlide = 0;
    let autoPlayInterval;
    
    function showSlide(index) {
        // Handle wrap around
        if (index >= slides.length) {
            currentSlide = 0;
        } else if (index < 0) {
            currentSlide = slides.length - 1;
        } else {
            currentSlide = index;
        }
        
        // Update slides
        slides.forEach((slide, i) => {
            slide.classList.remove('active');
            if (i === currentSlide) {
                slide.classList.add('active');
            }
        });
        
        // Update dots
        dots.forEach((dot, i) => {
            dot.classList.remove('active');
            if (i === currentSlide) {
                dot.classList.add('active');
            }
        });
    }
    
    function nextSlide() {
        showSlide(currentSlide + 1);
    }
    
    function prevSlide() {
        showSlide(currentSlide - 1);
    }
    
    function startAutoPlay() {
        autoPlayInterval = setInterval(nextSlide, 5000);
    }
    
    function stopAutoPlay() {
        clearInterval(autoPlayInterval);
    }
    
    // Event listeners
    if (prevBtn && nextBtn) {
        prevBtn.addEventListener('click', () => {
            stopAutoPlay();
            prevSlide();
            startAutoPlay();
        });
        
        nextBtn.addEventListener('click', () => {
            stopAutoPlay();
            nextSlide();
            startAutoPlay();
        });
    }
    
    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            stopAutoPlay();
            showSlide(index);
            startAutoPlay();
        });
    });
    
    // Start autoplay
    startAutoPlay();
    
    // Pause on hover
    const slider = document.querySelector('.testimonials-slider');
    if (slider) {
        slider.addEventListener('mouseenter', stopAutoPlay);
        slider.addEventListener('mouseleave', startAutoPlay);
    }
}

// ==========================================
// CONTACT FORM
// ==========================================
function initContactForm() {
    if (!contactForm) return;
    
    contactForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const submitBtn = contactForm.querySelector('.submit-btn');
        const originalText = submitBtn.innerHTML;
        
        // Show loading state
        submitBtn.innerHTML = '<span>Sending...</span><i class="fas fa-spinner fa-spin"></i>';
        submitBtn.disabled = true;
        
        // Simulate form submission (replace with actual API call)
        try {
            await new Promise(resolve => setTimeout(resolve, 2000));
            
            // Success
            submitBtn.innerHTML = '<span>Message Sent!</span><i class="fas fa-check"></i>';
            submitBtn.style.background = 'linear-gradient(135deg, #10b981 0%, #34d399 100%)';
            
            // Reset form
            contactForm.reset();
            
            // Reset button after delay
            setTimeout(() => {
                submitBtn.innerHTML = originalText;
                submitBtn.style.background = '';
                submitBtn.disabled = false;
            }, 3000);
            
        } catch (error) {
            // Error
            submitBtn.innerHTML = '<span>Error! Try Again</span><i class="fas fa-times"></i>';
            submitBtn.style.background = 'linear-gradient(135deg, #ef4444 0%, #f87171 100%)';
            
            setTimeout(() => {
                submitBtn.innerHTML = originalText;
                submitBtn.style.background = '';
                submitBtn.disabled = false;
            }, 3000);
        }
    });
    
    // Form field animations
    const formGroups = contactForm.querySelectorAll('.form-group');
    formGroups.forEach(group => {
        const input = group.querySelector('input, select, textarea');
        if (input) {
            input.addEventListener('focus', () => {
                group.classList.add('focused');
            });
            
            input.addEventListener('blur', () => {
                if (!input.value) {
                    group.classList.remove('focused');
                }
            });
        }
    });
}

// ==========================================
// CURSOR GLOW EFFECT
// ==========================================
function initCursorGlow() {
    if (!cursorGlow || window.matchMedia('(max-width: 768px)').matches) {
        if (cursorGlow) cursorGlow.style.display = 'none';
        return;
    }
    
    let mouseX = 0;
    let mouseY = 0;
    let glowX = 0;
    let glowY = 0;
    
    document.addEventListener('mousemove', (e) => {
        mouseX = e.clientX;
        mouseY = e.clientY;
    });
    
    function animateGlow() {
        // Smooth follow effect
        glowX += (mouseX - glowX) * 0.1;
        glowY += (mouseY - glowY) * 0.1;
        
        cursorGlow.style.left = `${glowX}px`;
        cursorGlow.style.top = `${glowY}px`;
        
        requestAnimationFrame(animateGlow);
    }
    
    animateGlow();
    
    // Hide on mouse leave
    document.addEventListener('mouseleave', () => {
        cursorGlow.style.opacity = '0';
    });
    
    document.addEventListener('mouseenter', () => {
        cursorGlow.style.opacity = '0.5';
    });
}

// ==========================================
// SCROLL REVEAL ANIMATIONS
// ==========================================
function initScrollReveal() {
    const revealElements = document.querySelectorAll('.animate-on-scroll');
    
    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });
    
    revealElements.forEach(element => {
        revealObserver.observe(element);
    });
}

// ==========================================
// PARALLAX EFFECTS
// ==========================================
window.addEventListener('scroll', () => {
    const scrolled = window.pageYOffset;
    
    // Parallax for hero shapes
    const shapes = document.querySelectorAll('.shape');
    shapes.forEach((shape, index) => {
        const speed = 0.1 + (index * 0.02);
        shape.style.transform = `translateY(${scrolled * speed}px)`;
    });
});

// ==========================================
// LAZY LOADING IMAGES
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const lazyImages = document.querySelectorAll('img[data-src]');
    
    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
                imageObserver.unobserve(img);
            }
        });
    });
    
    lazyImages.forEach(img => {
        imageObserver.observe(img);
    });
});

// ==========================================
// PRELOADER (Optional)
// ==========================================
window.addEventListener('load', () => {
    const preloader = document.getElementById('preloader');
    if (preloader) {
        preloader.style.opacity = '0';
        setTimeout(() => {
            preloader.style.display = 'none';
        }, 500);
    }
    
    // Trigger initial animations
    document.body.classList.add('loaded');
});

// ==========================================
// KEYBOARD NAVIGATION
// ==========================================
document.addEventListener('keydown', (e) => {
    // ESC to close mobile menu
    if (e.key === 'Escape') {
        navMenu.classList.remove('active');
        mobileMenuToggle.classList.remove('active');
        document.body.classList.remove('menu-open');
    }
});

// ==========================================
// RESIZE HANDLER
// ==========================================
let resizeTimer;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(() => {
        // Close mobile menu on resize to desktop
        if (window.innerWidth > 992) {
            navMenu.classList.remove('active');
            mobileMenuToggle.classList.remove('active');
            document.body.classList.remove('menu-open');
        }
    }, 250);
});

// ==========================================
// CONSOLE EASTER EGG
// ==========================================
console.log(`
%c👋 Hey there, fellow developer!
%cLooking to hire a talented engineer?
%cLet's connect: zabbastahir@gmail.com

`,
'color: #6366f1; font-size: 20px; font-weight: bold;',
'color: #a855f7; font-size: 14px;',
'color: #10b981; font-size: 12px;'
);

// ==========================================
// VISITOR TRACKING & ANALYTICS
// ==========================================
(function initVisitorStats() {
    // Simple visitor counter using localStorage
    const VISITOR_KEY = 'zat_visitor_id';
    const VISIT_COUNT_KEY = 'zat_total_visits';
    const PAGE_VIEWS_KEY = 'zat_page_views';
    
    // Generate unique visitor ID if not exists
    if (!localStorage.getItem(VISITOR_KEY)) {
        localStorage.setItem(VISITOR_KEY, 'visitor_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9));
    }
    
    // Track page view
    let pageViews = parseInt(localStorage.getItem(PAGE_VIEWS_KEY) || '0');
    pageViews++;
    localStorage.setItem(PAGE_VIEWS_KEY, pageViews.toString());
    
    // Track visit (session-based)
    if (!sessionStorage.getItem('visited')) {
        sessionStorage.setItem('visited', 'true');
        let visits = parseInt(localStorage.getItem(VISIT_COUNT_KEY) || '0');
        visits++;
        localStorage.setItem(VISIT_COUNT_KEY, visits.toString());
    }
    
    // Update display
    const visitorCountEl = document.getElementById('visitor-count');
    const pageViewsEl = document.getElementById('page-views');
    const onlineNowEl = document.getElementById('online-now');
    
    if (visitorCountEl) {
        // Animate counter
        animateCounter(visitorCountEl, parseInt(localStorage.getItem(VISIT_COUNT_KEY) || '1'));
    }
    
    if (pageViewsEl) {
        animateCounter(pageViewsEl, pageViews);
    }
    
    if (onlineNowEl) {
        // Simulate online users (1-3 for demo)
        onlineNowEl.textContent = Math.floor(Math.random() * 3) + 1;
    }
    
    function animateCounter(element, target) {
        let current = 0;
        const duration = 1500;
        const increment = target / (duration / 16);
        
        function update() {
            current += increment;
            if (current < target) {
                element.textContent = Math.floor(current);
                requestAnimationFrame(update);
            } else {
                element.textContent = target;
            }
        }
        
        // Start animation when element is in view
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    update();
                    observer.unobserve(entry.target);
                }
            });
        });
        
        observer.observe(element);
    }
    
    // Log analytics info
    console.log('%c📊 Analytics Active', 'color: #22c55e; font-weight: bold;');
    console.log('Visitor ID:', localStorage.getItem(VISITOR_KEY));
    console.log('Total Visits:', localStorage.getItem(VISIT_COUNT_KEY));
    console.log('Page Views:', pageViews);
})();

// ==========================================
// ADVANCED ANALYTICS HELPER
// ==========================================
// This sends data to Google Analytics (if configured)
function trackEvent(category, action, label) {
    if (typeof gtag !== 'undefined') {
        gtag('event', action, {
            'event_category': category,
            'event_label': label
        });
    }
}

// Track important user interactions
document.querySelectorAll('.btn-primary').forEach(btn => {
    btn.addEventListener('click', function() {
        const label = this.textContent.trim();
        trackEvent('CTA', 'click', label);
    });
});

document.querySelectorAll('.social-link, .linkedin-badge').forEach(link => {
    link.addEventListener('click', function() {
        const label = this.getAttribute('aria-label') || 'Social Link';
        trackEvent('Social', 'click', label);
    });
});

document.querySelectorAll('.project-card').forEach(card => {
    card.addEventListener('click', function() {
        const title = this.querySelector('h3')?.textContent || 'Project';
        trackEvent('Portfolio', 'view_project', title);
    });
});

// ==========================================
// AI CHATBOT - ZAIN'S ASSISTANT
// ==========================================
(function initChatbot() {
    const chatbotToggle = document.getElementById('chatbot-toggle');
    const chatbotContainer = document.getElementById('chatbot-container');
    const chatbotClose = document.getElementById('chatbot-close');
    const chatbotInput = document.getElementById('chatbot-input');
    const chatbotSend = document.getElementById('chatbot-send');
    const chatbotMessages = document.getElementById('chatbot-messages');
    const suggestionChips = document.querySelectorAll('.suggestion-chip');
    const chatNotification = document.querySelector('.chat-notification');
    
    if (!chatbotToggle) return;
    
    // Zain's Knowledge Base
    const knowledgeBase = {
        name: "Zain Abbas Tahir",
        title: "AI Technical Lead | Delivery Manager | Cloud Architect",
        experience: "13+ years",
        location: "Kuala Lumpur, Malaysia",
        email: "zabbastahir@gmail.com",
        linkedin: "https://www.linkedin.com/in/zainabbastahir/",
        github: "https://github.com/zainabbastahir",
        youtube: "https://www.youtube.com/@zainabbastahir",
        calendly: "https://calendly.com/zainabbastahir/30min",
        website: "https://zainabbastahir.com",
        
        currentRole: {
            company: "Aventra Group",
            position: "Technical Lead",
            period: "Nov 2025 - Present",
            location: "Kuala Lumpur, Malaysia"
        },
        
        previousRole: {
            company: "DHL IT Services",
            position: "Technical Lead",
            period: "Sep 2022 - Oct 2025",
            location: "Kuala Lumpur, Malaysia"
        },
        
        aiSkills: ["RAG (Retrieval-Augmented Generation)", "Large Language Models (LLMs)", "Predictive AI", "Sentiment Analysis", "AI Foundry", "Data Fabric", "Copilot Studio", "Power Apps", "AI Search", "Agent AI", "Azure OpenAI", "Computer Vision"],
        
        cloudSkills: ["Microsoft Azure", "Service Bus", "Data Factory", "App Services", "Logic Apps", "Azure Functions", "Virtual Machines", "Key Vault", "API Management", "VNET", "Active Directory & RBAC", "Kubernetes", "Azure AKS", "Cosmos DB", "Redis Cache"],
        
        devSkills: [".NET / .NET Core", "ASP.NET MVC", "Web API", "Angular 17+", "Blazor", "React", "Microservices", "CQRS", "API Integration", "C#", "TypeScript", "Python"],
        
        devOpsSkills: ["CI/CD Pipelines", "Azure DevOps", "Terraform", "ARM Templates", "Bicep", "Docker", "TDD", "Code Refactoring", "Monitoring & Metrics", "Cost Management"],
        
        dataSkills: ["SQL Server", "EF Core", "Database Migrations", "Stored Procedures", "Triggers", "Event-Driven Design", "MongoDB", "Vector DB", "Cloud Search", "SEO"],
        
        projects: [
            { name: "Document Management System", tech: ".NET Core, Azure, Angular", desc: "Enterprise-grade document management with role-based access" },
            { name: "AI Customer Support Bot", tech: "Azure OpenAI, RAG, LLMs", desc: "Intelligent chatbot with sentiment analysis" },
            { name: "Predictive Analytics Engine", tech: "Azure ML, Python, Power BI", desc: "ML-powered business forecasting system" },
            { name: "POS System", tech: ".NET Core, Angular, SQL Server", desc: "Modern point of sale with inventory management" },
            { name: "URL Shortener", tech: ".NET Core, Redis, Angular", desc: "Analytics-enabled link shortening service" },
            { name: "Face Recognition Login", tech: "Azure AI, Computer Vision", desc: "Biometric authentication system" },
            { name: "E-Commerce Platform", tech: "Microservices, Azure, Angular", desc: "Scalable multi-vendor marketplace" }
        ],
        
        services: ["Custom Software Development", "AI/ML Solutions", "Cloud Architecture", "API Integration", "Technical Consulting", "Team Leadership", "DevOps Implementation", "System Optimization"],
        
        achievements: [
            "Led teams of developers across multiple projects",
            "Built AI-based Route Optimization System",
            "Implemented RAG System for enhanced search",
            "Managed Azure infrastructure for enterprise clients",
            "Delivered 50+ successful projects"
        ]
    };
    
    // Response patterns
    const responses = {
        greeting: [
            `Hello! I'm here to tell you about Zain Abbas Tahir - an ${knowledgeBase.title} with ${knowledgeBase.experience} of experience. What would you like to know?`,
            `Hi there! 👋 I can help you learn about Zain's skills, experience, and projects. What interests you?`
        ],
        
        skills: `Zain has expertise across multiple domains:\n\n🧠 **AI & Analytics:** ${knowledgeBase.aiSkills.slice(0, 5).join(', ')} and more\n\n☁️ **Cloud:** ${knowledgeBase.cloudSkills.slice(0, 5).join(', ')}\n\n💻 **Development:** ${knowledgeBase.devSkills.slice(0, 5).join(', ')}\n\n🚀 **DevOps:** ${knowledgeBase.devOpsSkills.slice(0, 4).join(', ')}`,
        
        aiSkills: `Zain specializes in cutting-edge AI technologies:\n\n• ${knowledgeBase.aiSkills.join('\n• ')}\n\nHe builds intelligent systems using RAG, LLMs, and predictive AI to transform business operations.`,
        
        experience: `**Current Role:**\n📍 ${knowledgeBase.currentRole.position} at ${knowledgeBase.currentRole.company}\n📅 ${knowledgeBase.currentRole.period}\n\n**Previous Role:**\n📍 ${knowledgeBase.previousRole.position} at ${knowledgeBase.previousRole.company}\n📅 ${knowledgeBase.previousRole.period}\n\nZain has ${knowledgeBase.experience} of professional experience leading development teams and building enterprise solutions.`,
        
        projects: `Here are some of Zain's notable projects:\n\n${knowledgeBase.projects.map(p => `🚀 **${p.name}**\n   Tech: ${p.tech}\n   ${p.desc}`).join('\n\n')}\n\nWant to know more about any specific project?`,
        
        contact: `You can reach Zain through:\n\n📧 Email: ${knowledgeBase.email}\n💼 LinkedIn: ${knowledgeBase.linkedin}\n📅 Schedule a call: ${knowledgeBase.calendly}\n🌐 Website: ${knowledgeBase.website}\n\nHe's available for consulting and full-time opportunities!`,
        
        hire: `Great choice! Here's how to hire Zain:\n\n1️⃣ **Schedule a Call:** ${knowledgeBase.calendly}\n2️⃣ **Email:** ${knowledgeBase.email}\n3️⃣ **LinkedIn:** ${knowledgeBase.linkedin}\n\nZain offers:\n• Custom Software Development\n• AI/ML Solutions\n• Cloud Architecture\n• Technical Consulting\n• Team Leadership`,
        
        location: `Zain is based in **${knowledgeBase.location}** and works with clients globally. He's experienced in both on-site and remote collaboration.`,
        
        azure: `Zain is an Azure expert with deep experience in:\n\n${knowledgeBase.cloudSkills.map(s => `• ${s}`).join('\n')}\n\nHe has built and managed enterprise-scale Azure infrastructure for companies like DHL.`,
        
        dotnet: `Zain has extensive .NET expertise:\n\n• .NET / .NET Core\n• ASP.NET MVC & Web API\n• Blazor & Entity Framework\n• Microservices Architecture\n• CQRS Pattern\n\nHe's been working with .NET for 13+ years!`,
        
        angular: `Zain is proficient in Angular (versions 13-18+) and has built numerous enterprise applications using:\n\n• Angular 17+\n• TypeScript\n• RxJS\n• NgRx\n• Material Design\n\nHe also works with React and Blazor for frontend development.`,
        
        youtube: `Check out Zain's YouTube channel for technical tutorials:\n\n🎬 ${knowledgeBase.youtube}\n\nHe shares content on AI, .NET, Azure, and software development best practices!`,
        
        services: `Zain offers the following services:\n\n${knowledgeBase.services.map(s => `✅ ${s}`).join('\n')}\n\nInterested? Schedule a consultation: ${knowledgeBase.calendly}`,
        
        about: `**${knowledgeBase.name}**\n${knowledgeBase.title}\n\n📍 ${knowledgeBase.location}\n💼 ${knowledgeBase.experience} of experience\n\nZain builds intelligent, scalable, and predictive systems using AI, Cloud, and modern development practices. He specializes in RAG, LLMs, Azure, and .NET technologies.`,
        
        fallback: `I can help you with information about Zain Abbas Tahir. Try asking about:\n\n• His AI & technical skills\n• Work experience\n• Projects & portfolio\n• How to hire/contact him\n• Specific technologies (Azure, .NET, Angular, etc.)\n\nWhat would you like to know?`
    };
    
    // Intent detection
    function detectIntent(message) {
        const msg = message.toLowerCase();
        
        // Greetings
        if (/^(hi|hello|hey|howdy|greetings|good morning|good afternoon|good evening)/i.test(msg)) {
            return 'greeting';
        }
        
        // AI Skills
        if (/(ai|artificial intelligence|machine learning|ml|rag|llm|chatbot|nlp|sentiment|predictive|copilot|foundry)/i.test(msg)) {
            return 'aiSkills';
        }
        
        // Azure/Cloud
        if (/(azure|cloud|aws|service bus|data factory|kubernetes|k8s|docker|devops|infrastructure)/i.test(msg)) {
            return 'azure';
        }
        
        // .NET
        if (/(\.net|dotnet|c#|csharp|asp\.net|blazor|entity framework|ef core)/i.test(msg)) {
            return 'dotnet';
        }
        
        // Angular/Frontend
        if (/(angular|react|frontend|front-end|typescript|javascript|ui|ux)/i.test(msg)) {
            return 'angular';
        }
        
        // Skills general
        if (/(skill|technology|tech stack|expertise|know|proficient|capable|abilities)/i.test(msg)) {
            return 'skills';
        }
        
        // Experience
        if (/(experience|work|job|career|company|companies|worked|employment|role|position|dhl|aventra)/i.test(msg)) {
            return 'experience';
        }
        
        // Projects
        if (/(project|portfolio|work|built|developed|created|application|app|system)/i.test(msg)) {
            return 'projects';
        }
        
        // Contact
        if (/(contact|email|reach|call|phone|linkedin|connect|social)/i.test(msg)) {
            return 'contact';
        }
        
        // Hire
        if (/(hire|hiring|work with|engage|consult|freelance|available|cost|rate|price|service)/i.test(msg)) {
            return 'hire';
        }
        
        // Location
        if (/(where|location|based|live|country|city|malaysia|remote)/i.test(msg)) {
            return 'location';
        }
        
        // YouTube
        if (/(youtube|video|tutorial|channel|watch|subscribe)/i.test(msg)) {
            return 'youtube';
        }
        
        // Services
        if (/(service|offer|provide|do you do|help with|assist)/i.test(msg)) {
            return 'services';
        }
        
        // About/Who
        if (/(who|about|tell me|introduce|yourself|zain)/i.test(msg)) {
            return 'about';
        }
        
        return 'fallback';
    }
    
    // Get response
    function getResponse(message) {
        const intent = detectIntent(message);
        let response = responses[intent];
        
        if (Array.isArray(response)) {
            response = response[Math.floor(Math.random() * response.length)];
        }
        
        return response || responses.fallback;
    }
    
    // Add message to chat
    function addMessage(text, isUser = false) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${isUser ? 'user' : 'bot'}`;
        
        const avatarHtml = isUser ? '' : `
            <div class="message-avatar">
                <img src="https://media.licdn.com/dms/image/v2/D5603AQHMF2-TqJj2Tg/profile-displayphoto-scale_200_200/B56ZlsZj34I8AY-/0/1758460270237?e=1769040000&v=beta&t=TUiiTGu06kKI5KsCZJXLshEV6BZA0NozBiEf9nMywkk" alt="Zain">
            </div>
        `;
        
        // Convert markdown-style formatting to HTML
        let formattedText = text
            .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
            .replace(/\n/g, '<br>')
            .replace(/• /g, '&bull; ');
        
        messageDiv.innerHTML = `
            ${avatarHtml}
            <div class="message-content">
                <p>${formattedText}</p>
            </div>
        `;
        
        chatbotMessages.appendChild(messageDiv);
        chatbotMessages.scrollTop = chatbotMessages.scrollHeight;
    }
    
    // Show typing indicator
    function showTyping() {
        const typingDiv = document.createElement('div');
        typingDiv.className = 'chat-message bot typing-message';
        typingDiv.innerHTML = `
            <div class="message-avatar">
                <img src="https://media.licdn.com/dms/image/v2/D5603AQHMF2-TqJj2Tg/profile-displayphoto-scale_200_200/B56ZlsZj34I8AY-/0/1758460270237?e=1769040000&v=beta&t=TUiiTGu06kKI5KsCZJXLshEV6BZA0NozBiEf9nMywkk" alt="Zain">
            </div>
            <div class="message-content">
                <div class="typing-dots"><span></span><span></span><span></span></div>
            </div>
        `;
        chatbotMessages.appendChild(typingDiv);
        chatbotMessages.scrollTop = chatbotMessages.scrollHeight;
        return typingDiv;
    }
    
    // Handle send message
    function handleSend() {
        const message = chatbotInput.value.trim();
        if (!message) return;
        
        addMessage(message, true);
        chatbotInput.value = '';
        
        const typingIndicator = showTyping();
        
        // Simulate thinking time
        setTimeout(() => {
            typingIndicator.remove();
            const response = getResponse(message);
            addMessage(response);
        }, 800 + Math.random() * 700);
    }
    
    // Toggle chatbot
    chatbotToggle.addEventListener('click', () => {
        chatbotContainer.classList.toggle('active');
        if (chatNotification) chatNotification.style.display = 'none';
    });
    
    chatbotClose.addEventListener('click', () => {
        chatbotContainer.classList.remove('active');
    });
    
    // Send message
    chatbotSend.addEventListener('click', handleSend);
    chatbotInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') handleSend();
    });
    
    // Suggestion chips
    suggestionChips.forEach(chip => {
        chip.addEventListener('click', () => {
            const query = chip.getAttribute('data-query');
            chatbotInput.value = query;
            handleSend();
        });
    });
    
    // Close on outside click
    document.addEventListener('click', (e) => {
        if (!chatbotToggle.contains(e.target) && !chatbotContainer.contains(e.target)) {
            chatbotContainer.classList.remove('active');
        }
    });
})();

// ==========================================
// GALLERY LIGHTBOX
// ==========================================
(function initLightbox() {
    const lightbox = document.getElementById('lightbox');
    const lightboxImg = document.getElementById('lightbox-img');
    const lightboxClose = document.getElementById('lightbox-close');
    const lightboxPrev = document.getElementById('lightbox-prev');
    const lightboxNext = document.getElementById('lightbox-next');
    const galleryItems = document.querySelectorAll('.gallery-item');
    
    if (!lightbox || !galleryItems.length) return;
    
    let currentIndex = 0;
    const images = Array.from(galleryItems).map(item => item.querySelector('img').src);
    
    function showImage(index) {
        if (index < 0) index = images.length - 1;
        if (index >= images.length) index = 0;
        currentIndex = index;
        lightboxImg.src = images[currentIndex];
    }
    
    function openLightbox(index) {
        currentIndex = index;
        showImage(currentIndex);
        lightbox.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
    
    function closeLightbox() {
        lightbox.classList.remove('active');
        document.body.style.overflow = '';
    }
    
    // Click handlers for gallery items
    galleryItems.forEach((item, index) => {
        item.addEventListener('click', () => openLightbox(index));
    });
    
    // Lightbox controls
    if (lightboxClose) {
        lightboxClose.addEventListener('click', closeLightbox);
    }
    
    if (lightboxPrev) {
        lightboxPrev.addEventListener('click', () => showImage(currentIndex - 1));
    }
    
    if (lightboxNext) {
        lightboxNext.addEventListener('click', () => showImage(currentIndex + 1));
    }
    
    // Close on background click
    lightbox.addEventListener('click', (e) => {
        if (e.target === lightbox) closeLightbox();
    });
    
    // Keyboard navigation
    document.addEventListener('keydown', (e) => {
        if (!lightbox.classList.contains('active')) return;
        
        if (e.key === 'Escape') closeLightbox();
        if (e.key === 'ArrowLeft') showImage(currentIndex - 1);
        if (e.key === 'ArrowRight') showImage(currentIndex + 1);
    });
})();
