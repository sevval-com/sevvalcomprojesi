// Toplam ve aktif ziyaretçi sayısını güncelleyen fonksiyon
async function updateVisitorCounts() {
    try {
        // Aktif ziyaretçi sayısını al
        const activeResponse = await fetch('/api/visitor/active');
        const activeCount = await activeResponse.json();

        // Toplam ziyaretçi sayısını al
        const totalResponse = await fetch('/api/visitor/total');
        const totalCount = await totalResponse.json();
        
        // Sayıları HTML'de güncelle ve formatla
        // API response formatı: { Data: { ActiveVisitorCount: number, TotalVisitorCount: number } }
        const activeVisitorCount = activeCount?.Data?.ActiveVisitorCount || activeCount?.data?.activeVisitorCount || activeCount?.ActiveVisitorCount || activeCount || 0;
        const totalVisitorCount = totalCount?.Data?.TotalVisitorCount || totalCount?.data?.totalVisitorCount || totalCount?.TotalVisitorCount || totalCount || 0;

        const activeEl = document.getElementById('active-visitors-navbar');
        if (activeEl) activeEl.textContent = formatNumber(activeVisitorCount);

        const totalNavbarEl = document.getElementById('total-visitors-navbar');
        if (totalNavbarEl) totalNavbarEl.textContent = formatNumber(totalVisitorCount);

        const totalAboutEl = document.getElementById('total-visitors-about');
        if (totalAboutEl) totalAboutEl.textContent = formatNumber(totalVisitorCount);
    } catch (error) {
        console.error('Ziyaretçi sayıları güncellenirken hata oluştu:', error);
    }
}

// Sayıyı noktalarla formatlayan fonksiyon
function formatNumber(number) {
    // Sayı kontrolü ekle
    const num = typeof number === 'number' ? number : parseInt(number) || 0;
    return num.toLocaleString('tr-TR'); // Türkçe formatı kullanır
}




// Kullanıcı siteye girdiğinde ziyaretçi kaydını oluştur
async function recordVisitor() {
    try {
        await fetch('/api/visitor/enter', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
    } catch (error) {
        console.error('Ziyaretçi kaydı oluşturulurken hata oluştu:', error);
    }
}

// Kullanıcı siteyi kapattığında ziyaretçi kaydını sil
async function exitVisitor(visitorId) {
    try {
        await fetch(`/api/visitor/exit?visitorId=${visitorId}`, {
            method: 'POST'
        });
    } catch (error) {
        console.error('Ziyaretçi çıkış kaydı oluşturulurken hata oluştu:', error);
    }
}

// Sayfa yüklendiğinde ziyaretçi kaydını oluştur ve sayıları güncelle
document.addEventListener('DOMContentLoaded', () => {
    recordVisitor(); // Ziyaretçi kaydını oluştur
    updateVisitorCounts(); // İlk güncellemeyi yap

    // Her 1 dk  bir güncellemeleri tekrarla
    setInterval(updateVisitorCounts, 60000);

    // Kullanıcı siteyi kapattığında ziyaretçi kaydını sil
    window.addEventListener('beforeunload', () => {
        const visitorId = localStorage.getItem('visitorId'); // Kullanıcıya özel bir ID tutuyorsanız
        if (visitorId) {
            exitVisitor(visitorId);
        }
    });
});