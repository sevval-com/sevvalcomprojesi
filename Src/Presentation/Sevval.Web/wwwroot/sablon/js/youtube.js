const videos = document.querySelectorAll('video-item');

videos.forEach(video => {
    video.addEventListener('click', () => {
        const modal = document.querySelector('.modal');
        const modalContent = document.querySelector('.modal-content');
        modal.style.display = 'block';
        modalContent.innerHTML = video.outerHTML; // Tıklanan videoyu modal içine kopyala
    });
});