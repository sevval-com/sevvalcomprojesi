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
    fetch('/api/visitor/active')
        .then(response => response.json())
        .then(data => {
            // API response formatı: { Data: { ActiveVisitorCount: number } }
            const visitorCount = data?.Data?.ActiveVisitorCount || data?.data?.activeVisitorCount || data?.ActiveVisitorCount || data || 0;
            const activeVisitorsElement = document.getElementById('activeVisitors');
            if (activeVisitorsElement) {
                activeVisitorsElement.innerText = visitorCount;
            }
        })
        .catch(error => console.error('Hata:', error));
}, 5000); // 5 saniyede bir güncelle
