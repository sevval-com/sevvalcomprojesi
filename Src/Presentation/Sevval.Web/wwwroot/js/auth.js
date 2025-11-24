document.addEventListener('DOMContentLoaded', () => {
    // E-Devlet'ten gelen yetki kontrolü
    checkAuthorization();

    // Form submit olayını yakala
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', (e) => {
            if (!isAuthorized()) {
                e.preventDefault();
                showUnauthorizedMessage();
            }
        });
    }
});

function checkAuthorization() {
    // E-Devlet'ten gelen yetki token'ı kontrolü
    const authToken = getAuthTokenFromEDevlet();

    if (isValidAuthToken(authToken)) {
        showListingForm();
    } else {
        showAuthSection();
    }
}

function isValidAuthToken(token) {
    // Token var mı kontrol et
    if (!token || !token.kullaniciKodu || !token.yetkilendirmeToken) {
        return false;
    }

    // Backend'e token doğrulama isteği gönder
    return fetch('/api/dogrula-edevlet-token', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(token)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Token geçerli ise session'a kaydet
                sessionStorage.setItem('eDevletAuth', JSON.stringify(token));
                return true;
            }
            return false;
        })
        .catch(error => {
            console.error('Token doğrulama hatası:', error);
            return false;
        });
}

function getAuthTokenFromEDevlet() {
    // URL'den gelen parametreleri al
    const urlParams = new URLSearchParams(window.location.search);

    // E-Devlet'ten dönecek parametreler
    const kullaniciKodu = urlParams.get('kullaniciKodu');
    const yetkilendirmeToken = urlParams.get('yetkilendirmeToken');
    const tcKimlikNo = urlParams.get('tcKimlikNo');
    const vergiNo = urlParams.get('vergiNo'); // Tüzel kişiler için

    // Token bilgilerini bir objede topla
    return {
        kullaniciKodu,
        yetkilendirmeToken,
        tcKimlikNo,
        vergiNo
    };
}

function showListingForm() {
    document.getElementById('authSection').style.display = 'none';
    document.getElementById('unauthorizedMessage').style.display = 'none';
    document.getElementById('listingFormSection').style.display = 'block';
}

function showAuthSection() {
    document.getElementById('authSection').style.display = 'block';
    document.getElementById('listingFormSection').style.display = 'none';
    document.getElementById('unauthorizedMessage').style.display = 'none';
}

function showUnauthorizedMessage() {
    document.getElementById('unauthorizedMessage').style.display = 'block';
    // Sayfayı yukarı kaydır
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function isAuthorized() {
    // E-Devlet yetki kontrolü
    return false; // Şu an için hep false dönüyor
}

function startEDevletAuth() {
    const eDevletUrl = 'https://www.turkiye.gov.tr/ticaret-eids-tasinmaz-ilani-yetkilendirme-islemleri?' + new URLSearchParams({
        firmaKodu: '4100907',
        returnUrl: 'https://sevvalemlak.com.tr/api/EDevlet/callback'
    }).toString();

    window.open(eDevletUrl, '_blank');
} 