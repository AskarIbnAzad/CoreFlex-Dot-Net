document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const sidebar = document.querySelector('.sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const toggleBtn = document.querySelector('.toggle-btn');
    const menuItems = document.querySelectorAll('.menu-item');
    const mobileLogo = document.querySelector('.mobile-logo-container');

    // Toggle sidebar on button click
    sidebarToggle.addEventListener('click', function (e) {
        e.stopPropagation();
        sidebar.classList.toggle('show');
    });

    // Close sidebar when close button is clicked (mobile)
    if (toggleBtn) {
        toggleBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            sidebar.classList.remove('show');
        });
    }

    // Close sidebar when clicking outside (mobile)
    document.addEventListener('click', function (event) {
        const isClickInsideSidebar = sidebar.contains(event.target);
        const isClickOnToggle = sidebarToggle.contains(event.target);

        if (!isClickInsideSidebar && !isClickOnToggle && window.innerWidth <= 992) {
            sidebar.classList.remove('show');
        }
    });

    // Add active class to clicked menu item
    menuItems.forEach(item => {
        item.addEventListener('click', function () {
            menuItems.forEach(i => i.classList.remove('active'));
            this.classList.add('active');

            // Close sidebar on mobile after selection
            if (window.innerWidth <= 992) {
                sidebar.classList.remove('show');
            }
        });
    });

    // Handle logo visibility on resize
    function handleLogoVisibility() {
        if (window.innerWidth <= 992) {
            if (sidebar.classList.contains('show')) {
                mobileLogo.style.display = 'none';
            } else {
                mobileLogo.style.display = 'block';
            }
        }
    }

    // Initial check
    handleLogoVisibility();

    // Update on window resize
    window.addEventListener('resize', function () {
        handleLogoVisibility();

        // Ensure sidebar is visible on desktop
        if (window.innerWidth > 992) {
            sidebar.classList.remove('show');
        }
    });

    // Prevent sidebar from closing when clicking inside
    sidebar.addEventListener('click', function (e) {
        e.stopPropagation();
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });
});