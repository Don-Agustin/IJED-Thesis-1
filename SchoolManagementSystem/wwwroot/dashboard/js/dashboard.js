/* =================================================================
   dashboard.js  —  IJED Dashboard Interactions
   ================================================================= */

(function () {
    'use strict';

    // ── Sidebar toggle (desktop collapse / mobile slide) ─────────────
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const contentWrapper = document.getElementById('contentWrapper');
    const topbar = document.querySelector('.dash-topbar');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function () {
            const isMobile = window.innerWidth <= 768;

            if (isMobile) {
                sidebar.classList.toggle('mobile-open');
                let backdrop = document.getElementById('sidebarBackdrop');
                if (!backdrop) {
                    backdrop = document.createElement('div');
                    backdrop.id = 'sidebarBackdrop';
                    backdrop.className = 'sidebar-backdrop';
                    document.body.appendChild(backdrop);
                    backdrop.addEventListener('click', closeMobileSidebar);
                }
                backdrop.classList.toggle('show', sidebar.classList.contains('mobile-open'));
            } else {
                sidebar.classList.toggle('collapsed');
                // Sync topbar and content-wrapper left offset via CSS class
                if (topbar) topbar.style.left = sidebar.classList.contains('collapsed') ? 'var(--sidebar-collapsed-w)' : 'var(--sidebar-width)';
                if (contentWrapper) contentWrapper.style.marginLeft = sidebar.classList.contains('collapsed') ? 'var(--sidebar-collapsed-w)' : 'var(--sidebar-width)';
            }
        });
    }

    function closeMobileSidebar() {
        if (sidebar) sidebar.classList.remove('mobile-open');
        const backdrop = document.getElementById('sidebarBackdrop');
        if (backdrop) backdrop.classList.remove('show');
    }

    // ── Active sidebar link highlight ──────────────────────────────────
    // Ensures the correct link is marked active based on current URL
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar-link').forEach(function (link) {
        const href = link.getAttribute('href');
        if (!href) return;
        const linkPath = href.toLowerCase().split('?')[0];
        // Mark active if the URL starts with this link's path (handles /Students/Edit/5 → /Students)
        if (linkPath !== '/' && currentPath.startsWith(linkPath)) {
            // Remove any existing active
            document.querySelectorAll('.sidebar-link.active').forEach(function (a) {
                a.classList.remove('active');
                a.style.paddingLeft = '';
            });
            link.classList.add('active');
        }
    });

    // ── Card entrance animation ────────────────────────────────────────
    document.querySelectorAll('.card').forEach(function (card) {
        card.classList.add('animate__animated', 'animate__fadeInUp');
    });

    // ── Alert/notification bell shake on load ──────────────────────────
    var bellIcon = document.querySelector('.topbar-notif .fa-bell, .topbar-notif i');
    if (bellIcon) {
        bellIcon.classList.add('animate__animated', 'animate__shakeX');
        setTimeout(function () {
            bellIcon.classList.remove('animate__shakeX');
        }, 1000);
    }

    // ── Responsive: re-evaluate on window resize ───────────────────────
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            closeMobileSidebar();
        }
    });

})();
/* =================================================================
   dashboard.js  —  IJED Dashboard Interactions
   ================================================================= */

(function () {
    'use strict';

    // ── Sidebar toggle (desktop collapse / mobile slide) ─────────────
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const contentWrapper = document.getElementById('contentWrapper');
    const topbar = document.querySelector('.dash-topbar');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function () {
            const isMobile = window.innerWidth <= 768;

            if (isMobile) {
                sidebar.classList.toggle('mobile-open');
                let backdrop = document.getElementById('sidebarBackdrop');
                if (!backdrop) {
                    backdrop = document.createElement('div');
                    backdrop.id = 'sidebarBackdrop';
                    backdrop.className = 'sidebar-backdrop';
                    document.body.appendChild(backdrop);
                    backdrop.addEventListener('click', closeMobileSidebar);
                }
                backdrop.classList.toggle('show', sidebar.classList.contains('mobile-open'));
            } else {
                sidebar.classList.toggle('collapsed');
                // Sync topbar and content-wrapper left offset via CSS class
                if (topbar) topbar.style.left = sidebar.classList.contains('collapsed') ? 'var(--sidebar-collapsed-w)' : 'var(--sidebar-width)';
                if (contentWrapper) contentWrapper.style.marginLeft = sidebar.classList.contains('collapsed') ? 'var(--sidebar-collapsed-w)' : 'var(--sidebar-width)';
            }
        });
    }

    function closeMobileSidebar() {
        if (sidebar) sidebar.classList.remove('mobile-open');
        const backdrop = document.getElementById('sidebarBackdrop');
        if (backdrop) backdrop.classList.remove('show');
    }

    // ── Active sidebar link highlight ──────────────────────────────────
    // Ensures the correct link is marked active based on current URL
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar-link').forEach(function (link) {
        const href = link.getAttribute('href');
        if (!href) return;
        const linkPath = href.toLowerCase().split('?')[0];
        // Mark active if the URL starts with this link's path (handles /Students/Edit/5 → /Students)
        if (linkPath !== '/' && currentPath.startsWith(linkPath)) {
            // Remove any existing active
            document.querySelectorAll('.sidebar-link.active').forEach(function (a) {
                a.classList.remove('active');
                a.style.paddingLeft = '';
            });
            link.classList.add('active');
        }
    });

    // ── Card entrance animation ────────────────────────────────────────
    document.querySelectorAll('.card').forEach(function (card) {
        card.classList.add('animate__animated', 'animate__fadeInUp');
    });

    // ── Alert/notification bell shake on load ──────────────────────────
    var bellIcon = document.querySelector('.topbar-notif .fa-bell, .topbar-notif i');
    if (bellIcon) {
        bellIcon.classList.add('animate__animated', 'animate__shakeX');
        setTimeout(function () {
            bellIcon.classList.remove('animate__shakeX');
        }, 1000);
    }

    // ── Responsive: re-evaluate on window resize ───────────────────────
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            closeMobileSidebar();
        }
    });

})();
