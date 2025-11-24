// Form Doğrulama Fonksiyonu
function validateForm() {
    let isValid = true;

    // İlan Başlığı Doğrulama
    const ilanBaslik = document.getElementById('ilanBaslik');
    const ilanBaslikError = document.getElementById('ilanBaslikError');
    if (ilanBaslik.value.trim() === "") {
        ilanBaslikError.style.display = 'inline';
        isValid = false;
    } else {
        ilanBaslikError.style.display = 'none';
    }

    // İlan Açıklama Doğrulama
    const ilanAciklama = document.getElementById('ilanAciklama');
    const ilanAciklamaError = document.getElementById('ilanAciklamaError');
    if (ilanAciklama.value.trim() === "") {
        ilanAciklamaError.style.display = 'inline';
        isValid = false;
    } else {
        ilanAciklamaError.style.display = 'none';
    }

    // Fiyat Doğrulama
    const fiyat = document.getElementById('fiyat');
    const fiyatError = document.getElementById('fiyatError');
    if (fiyat.value.trim() === "") {
        fiyatError.style.display = 'inline';
        isValid = false;
    } else {
        fiyatError.style.display = 'none';
    }

    // Metrekare Doğrulama
    const metrekare = document.getElementById('metrekare');
    const metrekareError = document.getElementById('metrekareError');
    if (metrekare.value.trim() === "") {
        metrekareError.style.display = 'inline';
        isValid = false;
    } else {
        metrekareError.style.display = 'none';
    }

    // Ada No Doğrulama
    const adaNo = document.getElementById('adaNo');
    const adaNoError = document.getElementById('adaNoError');
    if (adaNo.value.trim() === "") {
        adaNoError.style.display = 'inline';
        isValid = false;
    } else {
        adaNoError.style.display = 'none';
    }

    // Parsel No Doğrulama
    const parselNo = document.getElementById('parselNo');
    const parselNoError = document.getElementById('parselNoError');
    if (parselNo.value.trim() === "") {
        parselNoError.style.display = 'inline';
        isValid = false;
    } else {
        parselNoError.style.display = 'none';
    }

    // Tapu Durumu Doğrulama
    const tapuDurumu = document.getElementById('tapuDurumu');
    const tapuDurumuError = document.getElementById('tapuDurumuError');
    if (tapuDurumu.value.trim() === "") {
        tapuDurumuError.style.display = 'inline';
        isValid = false;
    } else {
        tapuDurumuError.style.display = 'none';
    }

    // Konum Doğrulama
    const locationInput = document.getElementById('location-input');
    const locationInputError = document.getElementById('locationInputError');
    if (locationInput.value.trim() === "") {
        locationInputError.style.display = 'inline';
        isValid = false;
    } else {
        locationInputError.style.display = 'none';
    }

    if (isValid) {
        // Form geçerli ise işlemlere devam et
        console.log("Form geçerli!");
    } else {
        console.log("Form geçersiz, eksik alanları doldurun.");
    }
}

// Fiyat Girişi Formatlama Fonksiyonu
function formatPrice(input) {
    const value = input.value.replace(/[^\d]/g, ''); // Sayı olmayan karakterleri temizler
    input.value = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);
}

// Kategori Değiştirme Fonksiyonu
function kategoriDegistir() {
    const kategoriSecim = document.getElementById('kategoriSecim').value;
    const konutSecim = document.getElementById('konutSecim');
    const arsaSecim = document.getElementById('arsaSecim');

    if (kategoriSecim === 'Konut') {
        konutSecim.classList.remove('hidden');
        arsaSecim.classList.add('hidden');
    } else if (kategoriSecim === 'Arsa') {
        arsaSecim.classList.remove('hidden');
        konutSecim.classList.add('hidden');
    } else {
        konutSecim.classList.add('hidden');
        arsaSecim.classList.add('hidden');
    }
}

// Konut Durumu Değiştirme Fonksiyonu
function konutDurumuDegistir() {
    const konutDurumu = document.getElementById('konutDurumu').value;
    const mulkTipiKonut = document.getElementById('mulkTipiKonut');

    if (konutDurumu !== '') {
        mulkTipiKonut.classList.remove('hidden');
    } else {
        mulkTipiKonut.classList.add('hidden');
    }
}

// Arsa Durumu Değiştirme Fonksiyonu
function arsaDurumuDegistir() {
    const arsaDurumu = document.getElementById('arsaDurumu').value;
    const mulkTipiArsa = document.getElementById('mulkTipiArsa');

    if (arsaDurumu !== '') {
        mulkTipiArsa.classList.remove('hidden');
    } else {
        mulkTipiArsa.classList.add('hidden');
    }
}

// Google Maps API Entegrasyonu
let map;
let autocomplete;
let marker;  // Tek bir marker eklemek için

function initMap() {
    const mapElement = document.getElementById('map');
    const defaultLocation = { lat: 39.9334, lng: 32.8597 }; // Örnek konum: Ankara

    // Haritayı oluştur
    map = new google.maps.Map(mapElement, {
        center: defaultLocation,
        zoom: 15
    });

    // Autocomplete işlevi
    autocomplete = new google.maps.places.Autocomplete(document.getElementById('location-input'));
    autocomplete.bindTo('bounds', map);

    // Autocomplete işlevi ile yer seçildiğinde haritayı güncelle
    autocomplete.addListener('place_changed', function () {
        const place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }

        // Eğer yerleşim alanı varsa haritayı uyumla
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17); // Sabit zoom seviyesi
        }

        // Haritada işaretleyici ekleyelim
        addMarker(place.geometry.location);
    });

    // Haritaya tıklanma olayını ekle
    map.addListener('click', function (event) {
        addMarker(event.latLng); // Tıklanan yere marker ekle
    });
}

// İşaretleyici ekleme fonksiyonu
function addMarker(location) {
    // Eğer önceden bir marker varsa kaldır
    if (marker) {
        marker.setMap(null);
    }

    // Yeni marker oluştur ve haritaya ekle
    marker = new google.maps.Marker({
        position: location,
        map: map
    });

    // Konum bilgilerini saklamak için input alanına lat/lng değerlerini yaz
    document.getElementById('location-input').value = `${location.lat()}, ${location.lng()}`;
}

// Google Maps yükleme
google.maps.event.addDomListener(window, 'load', initMap);

function videoLinkKontrol() {
    const videoLink = document.getElementById('ilanVideo').value;
    const videoPreview = document.getElementById('videoPreview');
    const videoIframe = document.getElementById('videoIframe');

    // YouTube URL'sinin geçerli olup olmadığını kontrol et
    const youtubeRegex = /(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
    const match = videoLink.match(youtubeRegex);

    if (match) {
        const videoId = match[1];
        videoIframe.src = `https://www.youtube.com/embed/${videoId}`;
        videoPreview.classList.remove('hidden'); // Önizleme alanını göster
    } else {
        videoPreview.classList.add('hidden'); // Geçersiz URL durumunda gizle
    }
}


function validateForm() {
    // Form elemanlarını seç
    const ilanVideo = document.getElementById('ilanVideo').value;
    const videoPreview = document.getElementById('videoPreview');

    // Geçerli YouTube URL'si kontrolü
    const youtubeRegex = /(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
    const match = ilanVideo.match(youtubeRegex);

    if (!match) {
        alert("Lütfen geçerli bir YouTube linki girin.");
        return; // Geçersiz URL ise fonksiyondan çık
    }

    // Diğer form elemanlarını ve gerekli kontrolleri buraya ekleyebilirsiniz
    // ...

    // Formu başarılı bir şekilde geçerse burada ilerlemeyi sağlayın
    // Örneğin, formu gönderebilir veya başka bir sayfaya yönlendirebilirsiniz
    alert("Form başarıyla kaydedildi!"); // veya form gönderme işlemi
    // document.getElementById("yourFormId").submit(); // Formu göndermek için
}
