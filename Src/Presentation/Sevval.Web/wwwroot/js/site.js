// Sayfa yüklendiğinde aktif ziyaretçiyi ekle
window.onload = function () {
    fetch('/api/Visitor', {
        method: 'POST',
        credentials: 'include' // Cookie'leri dahil et
    })
        .then(response => {
            if (!response.ok) {
                console.error('Ziyaretçi eklenemedi:', response.statusText);
            }
        })
        .catch(error => console.error('Hata:', error));
};

// Aktif ziyaretçi sayısını periyodik olarak kontrol et
setInterval(() => {
    fetch('/api/Visitor/active')
        .then(response => response.json())
        .then(data => {
            document.getElementById('activeVisitors').innerText = data; // Aktif ziyaretçi sayısını güncelle
        })
        .catch(error => console.error('Hata:', error));
}, 5000); // 5 saniyede bir güncelle
