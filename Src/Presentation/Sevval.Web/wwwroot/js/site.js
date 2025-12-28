// Sayfa yüklendiğinde aktif ziyaretçiyi ekle
window.onload = function () {
    fetch('/api/Visitor/enter', {
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

// Not: Visitor count güncelleme işlemi artık layout.js dosyasında yapılıyor
