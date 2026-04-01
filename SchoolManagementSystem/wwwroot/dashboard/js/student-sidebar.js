/* =================================================================
   student-sidebar.js  —  Student sidebar collapse / expand
   Persists state to localStorage so preference survives page loads.
   Works alongside existing dashboard.js (does not conflict).
   ================================================================= */

(function () {
    'use strict';

    const STORAGE_KEY = 'ijed_student_sidebar_collapsed';

    const sidebar       = document.getElementById('sidebar');
    const collapseBtn   = document.getElementById('studentSidebarCollapseBtn');
    const collapseIcon  = document.getElementById('collapseIcon');
    const contentWrapper = document.getElementById('contentWrapper');
    const topbar        = document.getElementById('studentTopbar');

    if (!sidebar || !collapseBtn) return;

    // ── Apply collapsed state ───────────────────────────────────────
    function applyCollapsed(collapsed, animate) {
        if (!animate) sidebar.style.transition = 'none';

        if (collapsed) {
            sidebar.classList.add('collapsed');
        } else {
            sidebar.classList.remove('collapsed');
        }

        syncLayoutOffsets(collapsed);

        if (!animate) {
            // Force reflow then re-enable transitions
            sidebar.offsetHeight; // eslint-disable-line no-unused-expressions
            sidebar.style.transition = '';
        }
    }

    function syncLayoutOffsets(collapsed) {
        const w = collapsed
            ? 'var(--sidebar-collapsed-w)'
            : 'var(--sidebar-width)';

        if (topbar)         topbar.style.left       = w;
        if (contentWrapper) contentWrapper.style.marginLeft = w;
    }

    // ── Toggle on button click ──────────────────────────────────────
    collapseBtn.addEventListener('click', function () {
        const isMobile = window.innerWidth <= 768;

        if (isMobile) {
            // On mobile, let dashboard.js handle the slide-over behaviour;
            // just mirror the mobile-open class.
            sidebar.classList.toggle('mobile-open');
            return;
        }

        const willCollapse = !sidebar.classList.contains('collapsed');
        applyCollapsed(willCollapse, true);
        localStorage.setItem(STORAGE_KEY, willCollapse ? '1' : '0');
    });

    // ── Restore persisted preference on load ───────────────────────
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === '1') {
        applyCollapsed(true, false);   // no animation on initial load
    } else {
        syncLayoutOffsets(false);      // ensure offsets are correct on fresh load
    }

    // ── Responsive: reset on resize back to desktop ─────────────────
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            const collapsed = sidebar.classList.contains('collapsed');
            syncLayoutOffsets(collapsed);
        }
    });

})();
