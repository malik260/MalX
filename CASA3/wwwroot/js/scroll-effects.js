/**
 * Perfect Scroll Effects - Smooth scroll animations for client pages
 * Uses Intersection Observer API for optimal performance
 */

(function() {
    'use strict';

    // Configuration
    const config = {
        // Animation thresholds
        threshold: 0.1, // Trigger when 10% of element is visible
        rootMargin: '0px 0px -50px 0px', // Trigger slightly before element enters viewport
        
        // Animation durations (in milliseconds)
        duration: {
            fast: 400,
            normal: 600,
            slow: 800
        },
        
        // Stagger delays (in milliseconds)
        stagger: {
            fast: 50,
            normal: 100,
            slow: 150
        }
    };

    // CSS classes for animations
    const animationClasses = {
        // Fade animations
        fadeIn: 'scroll-fade-in',
        fadeInUp: 'scroll-fade-in-up',
        fadeInDown: 'scroll-fade-in-down',
        fadeInLeft: 'scroll-fade-in-left',
        fadeInRight: 'scroll-fade-in-right',
        
        // Slide animations
        slideUp: 'scroll-slide-up',
        slideDown: 'scroll-slide-down',
        slideLeft: 'scroll-slide-left',
        slideRight: 'scroll-slide-right',
        
        // Scale animations
        scaleIn: 'scroll-scale-in',
        scaleUp: 'scroll-scale-up',
        
        // Rotate animations
        rotateIn: 'scroll-rotate-in',
        
        // Combined animations
        fadeScale: 'scroll-fade-scale',
        fadeSlideUp: 'scroll-fade-slide-up',
        
        // Stagger animations (for lists)
        stagger: 'scroll-stagger',
        
        // Active state
        active: 'scroll-animated'
    };

    // Initialize scroll effects
    function initScrollEffects() {
        // Enable smooth scrolling for anchor links
        enableSmoothScrolling();
        
        // Initialize Intersection Observer
        const observer = new IntersectionObserver(handleIntersection, {
            threshold: config.threshold,
            rootMargin: config.rootMargin
        });

        // Observe all elements with scroll animation classes
        const animatedElements = document.querySelectorAll([
            '.' + animationClasses.fadeIn,
            '.' + animationClasses.fadeInUp,
            '.' + animationClasses.fadeInDown,
            '.' + animationClasses.fadeInLeft,
            '.' + animationClasses.fadeInRight,
            '.' + animationClasses.slideUp,
            '.' + animationClasses.slideDown,
            '.' + animationClasses.slideLeft,
            '.' + animationClasses.slideRight,
            '.' + animationClasses.scaleIn,
            '.' + animationClasses.scaleUp,
            '.' + animationClasses.rotateIn,
            '.' + animationClasses.fadeScale,
            '.' + animationClasses.fadeSlideUp,
            '[class*="scroll-"]' // Catch any other scroll- prefixed classes
        ].join(', '));

        animatedElements.forEach(element => {
            observer.observe(element);
        });

        // Handle stagger animations for child elements
        initStaggerAnimations(observer);
        
        // Auto-apply animations to common elements if not already applied
        autoApplyAnimations(observer);
    }

    // Handle intersection events
    function handleIntersection(entries, observer) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const element = entry.target;
                
                // Add active class to trigger animation
                element.classList.add(animationClasses.active);
                
                // Stop observing once animated (for performance)
                observer.unobserve(element);
            }
        });
    }

    // Initialize stagger animations for lists
    function initStaggerAnimations(observer) {
        const staggerContainers = document.querySelectorAll('.' + animationClasses.stagger);
        
        staggerContainers.forEach(container => {
            const children = container.children;
            const staggerDelay = container.dataset.staggerDelay || config.stagger.normal;
            
            Array.from(children).forEach((child, index) => {
                // Add animation class if not present
                if (!child.classList.toString().includes('scroll-')) {
                    child.classList.add(animationClasses.fadeInUp);
                }
                
                // Set custom delay for stagger effect
                child.style.setProperty('--stagger-delay', `${index * staggerDelay}ms`);
                child.style.animationDelay = `${index * staggerDelay}ms`;
                
                // Observe each child
                observer.observe(child);
            });
        });
    }

    // Auto-apply animations to common page elements
    function autoApplyAnimations(observer) {
        // Sections
        const sections = document.querySelectorAll('main > section:not([class*="scroll-"])');
        sections.forEach((section, index) => {
            if (index > 0) { // Skip first section (usually hero/banner)
                section.classList.add(animationClasses.fadeInUp);
                observer.observe(section);
            }
        });

        // Cards
        const cards = document.querySelectorAll('.card, [class*="card"], [class*="Card"]');
        cards.forEach(card => {
            if (!card.classList.toString().includes('scroll-')) {
                card.classList.add(animationClasses.fadeScale);
                observer.observe(card);
            }
        });

        // Headings (h2, h3)
        const headings = document.querySelectorAll('main h2:not([class*="scroll-"]), main h3:not([class*="scroll-"])');
        headings.forEach((heading, index) => {
            if (index > 0) { // Skip first heading
                heading.classList.add(animationClasses.fadeInUp);
                observer.observe(heading);
            }
        });

        // Images
        const images = document.querySelectorAll('main img:not([class*="scroll-"])');
        images.forEach(img => {
            img.classList.add(animationClasses.fadeScale);
            observer.observe(img);
        });

        // Lists
        const lists = document.querySelectorAll('main ul:not([class*="scroll-"]), main ol:not([class*="scroll-"])');
        lists.forEach(list => {
            list.classList.add(animationClasses.stagger);
            list.setAttribute('data-stagger-delay', config.stagger.normal);
            initStaggerAnimations(observer);
        });
    }

    // Enable smooth scrolling for anchor links
    function enableSmoothScrolling() {
        // Add smooth scroll behavior to html
        if (!document.documentElement.style.scrollBehavior) {
            document.documentElement.style.scrollBehavior = 'smooth';
        }

        // Handle anchor link clicks with smooth scroll
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#' || href === '#!') return;
                
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // Add parallax effect to elements with data-parallax attribute
    function initParallax() {
        const parallaxElements = document.querySelectorAll('[data-parallax]');
        
        if (parallaxElements.length === 0) return;

        let ticking = false;

        function updateParallax() {
            const scrollY = window.pageYOffset;
            
            parallaxElements.forEach(element => {
                const speed = parseFloat(element.dataset.parallax) || 0.5;
                const yPos = -(scrollY * speed);
                element.style.transform = `translate3d(0, ${yPos}px, 0)`;
            });

            ticking = false;
        }

        function requestTick() {
            if (!ticking) {
                window.requestAnimationFrame(updateParallax);
                ticking = true;
            }
        }

        window.addEventListener('scroll', requestTick, { passive: true });
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            initScrollEffects();
            initParallax();
        });
    } else {
        initScrollEffects();
        initParallax();
    }

    // Re-initialize on dynamic content load (for AJAX-loaded content)
    if (typeof jQuery !== 'undefined') {
        $(document).on('ajaxComplete', function() {
            setTimeout(initScrollEffects, 100);
        });
    }

    // Expose API for manual triggering
    window.ScrollEffects = {
        init: initScrollEffects,
        observe: function(element) {
            const observer = new IntersectionObserver(handleIntersection, {
                threshold: config.threshold,
                rootMargin: config.rootMargin
            });
            observer.observe(element);
        }
    };
})();

