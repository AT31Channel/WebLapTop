// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Fix z-index issues with footer
document.addEventListener('DOMContentLoaded', function() {
    // Đảm bảo main content có z-index cao hơn footer
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.style.position = 'relative';
        mainContent.style.zIndex = '20';
    }
    
    // Đảm bảo footer không che khuất nội dung
    const footer = document.querySelector('footer');
    if (footer) {
        footer.style.position = 'relative';
        footer.style.zIndex = '10';
    }
});
