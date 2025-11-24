let selectedCategory = "";
let selectedType = "";
let selectedSubcategory = "";

function selectCategory(category) {
    selectedCategory = category;
    document.getElementById("categoryInput").value = selectedCategory; // Kategori değerini güncelle
    resetSelections(); // Seçimleri sıfırla
    document.getElementById("step2").style.display = "block";
}

function selectType(type) {
    selectedType = type;

    const subcategoryList = document.getElementById("subcategoryList");
    subcategoryList.innerHTML = ""; // Önceki alt kategori öğelerini temizle

    // Seçilen kategoriye göre alt kategorileri oluştur
    let subcategories = [];
    switch (selectedCategory) {
        case 'Konut (Yaşam Alanı)':
            addSubcategoryOptions([
                'Daire', 'Müstakil Ev', 'Köy Evi', 'Eski, Kargir Ev',
                'Dağ Evi', 'Kulübe', 'Konteyner', 'Prefabrik', 'Bungalow',
                'Tiny House', 'Loft Daire', 'Bina', 'Devremülk',
                'Çiftlik Evi', 'Kooperatif', 'Villa', 'Köşk',
                'Konak', 'Yalı', 'Kerpiç Ev', 'Ev ve Ahır', 'Samanlık', 'Rezidans', 'Diğer'
            ]);
            break;
        case 'Arsa':
            addSubcategoryOptions([
                'Konut İmarlı', 'Sanayi İmarlı', 'Ticaret İmarlı',
                'Karma İmarlı', 'Tarım İmarlı', 'Yeşil Alan İmarlı',
                'Kamu Alanı İmarlı', 'Turizm İmarlı', 'Sağlık İmarlı',
                'Eğitim İmarlı', 'A-Lejantlı Arsa', 'Bahçe İmarlı',
                'Çiftlik İmarlı', 'Depo Ve Antrepo İmarlı',
                'Hastane İmarlı', 'Petrol Lejantı İmarlı',
                'Konut + Ticaret İmarlı', 'Muhtelif Arsa', 'Özel Kullanım',
                'Sit Alanı', 'Toplu Konut İmarlı', 'Turizm Arsa',
                'Enerji Depolama', 'Sera İmarlı', 'Spor Alanı İmarlı', 'Diğer'
            ]);
            ]);
            break;
        case 'Bahçe':
            addSubcategoryOptions([
                'Elma Bahçesi', 'Armut Bahçesi', 'Kiraz Bahçesi',
                'Üzüm Bahçesi', , 'Çilek Bahçesi', 'Nar Bahçesi',
                'Kivi Bahçesi', 'Limon Bahçesi', 'Şeftali Bahçesi',
                'Ahududu Bahçesi', 'Böğürtlen Bahçesi', 'Greyfurt Bahçesi',
                'Portakal Bahçesi', 'Mandalina Bahçesi', 'Avokado Bahçesi',
                'Muz Bahçesi', 'Erik Bahçesi', 'Kayısı Bahçesi',
                'Dut Bahçesi', 'Fındık Bahçesi', 'Ceviz Bahçesi',
                'Lavanta Bahçesi', 'Zeytin Bahçesi', 'Ayva Bahçesi',
                'Muşmula Bahçesi', 'İğde Bahçesi', 'Kızılcık Bahçesi',
                'Badem Bahçesi', 'Kestane Bahçesi', 'Vişne Bahçesi',
                'Hurma Bahçesi', 'Karışık', 'Diğer'
            ]);
            break;

        case 'Tarla':
            addSubcategoryOptions([
                'Buğday Tarlası', 'Mısır Tarlası', 'Patates Tarlası',
                'Arpa Tarlası', 'Soğan Tarlası', 'Havuç Tarlası',
                'Pamuk Tarlası', 'Nohut Tarlası', 'Mercimek Tarlası',
                'Fasulye Tarlası', 'Şeker Pancarı Tarlası', 'Yulaf Tarlası',
                'Çavdar Tarlası', 'Soya Fasulyesi Tarlası', 'Karpuz Tarlası',
                'Kavun Tarlası', 'Aspir Tarlası', 
                'Çeltik Tarlası', 'Keten Tarlası',
                'Turp Tarlası', 'Domates Tarlası', 'Biber Tarlası', 'Diğer'
                
            ]);
            break;
        case 'İş Yeri':
            addSubcategoryOptions([
                'Akaryakıt İstasyonu', 'Apartman Dairesi', 'Atölye',
                'AVM', 'Büfe', 'Büro & Ofis', 'Cafe & Bar',
                'Çiftlik', 'Depo & Antrepo', 'Düğün Salonu',
                'Dükkan & Mağaza', 'Enerji Santrali', 'Fabrika & Üretim Tesisi',
                'Garaj & Park Yeri', 'İmalathane', 'İş Hanı Katı & Ofisi',
                'Kantin', 'Kır & Kahvaltı Bahçesi', 'Kıraathane',
                'Komple Bina', 'Maden Ocağı', 'Otopark', 'Pastane',
                'Fırın & Tatlıcı', 'Pazar Yeri', 'Plaza',
                'Plaza Katı & Ofisi', 'Radyo İstasyonu & TV Kanalı',
                'Restoran & Lokanta', 'Rezidans Katı & Ofisi',
                'Sağlık Merkezi', 'Sinema & Konferans Salonu',
                'SPA', 'Hamam & Sauna', 'Spor Tesisi', 'Villa',
                'Yurt', 'Diğer'
            ]);
            break;
        case 'Turistik Tesis':
            addSubcategoryOptions([
                'Bungalov', 'Dağ Evi', 'Kamp Alanı',
                'Otel', 'Yazlık', 'Diğer'
            ]);
            break;
        default:
            break;
    }

    document.getElementById("step3").style.display = "block";
}

function addSubcategoryOptions(options) {
    const subcategoryList = document.getElementById("subcategoryList");
    options.forEach(option => {
        const li = document.createElement("li");
        li.textContent = option;
        li.onclick = function () { selectSubcategory(option); };
        subcategoryList.appendChild(li);
    });
}

function selectSubcategory(subcategory) {
    selectedSubcategory = subcategory;
    document.getElementById("step4").style.display = "block";

    // Seçim detaylarını güncelle
    document.getElementById("confirmation-text").textContent =
        `${selectedCategory} / ${selectedType} / ${selectedSubcategory}`;

    // Gizli alanlara seçimleri yerleştir
    document.getElementById("hidden-category").value = selectedCategory;
    document.getElementById("hidden-type").value = selectedType;
    document.getElementById("hidden-subcategory").value = selectedSubcategory;
}

function validateSelections() {
    if (!selectedCategory || !selectedType || !selectedSubcategory) {
        alert("Lütfen tüm seçimleri yapın.");
        return false; // Form gönderimini durdur
    }
    return true; // Form gönderimini onayla
}

function resetSelections() {
    selectedType = ""; // Durumu sıfırla
    selectedSubcategory = ""; // Alt kategoriyi sıfırla
    document.getElementById("step3").style.display = "none"; // Alt kategori adımını gizle
    document.getElementById("step4").style.display = "none"; // Onay adımını gizle
    document.getElementById("confirmation-text").textContent = ""; // Onay metnini sıfırla
    const subcategoryList = document.getElementById("subcategoryList");
    subcategoryList.innerHTML = ""; // Alt kategori listesini temizle
}

function redirect() {
    window.location.href = '/ilan/ver'; // Yönlendirmek istediğiniz URL
}

document.addEventListener("DOMContentLoaded", function () {
    const categorySelect = document.getElementById("category-select");
    const steps = document.querySelectorAll(".step");
    const confirmationText = document.getElementById("confirmation-text");
    const buttons = document.querySelectorAll("button");

    const categories = {
        "Kategori 1": {
            durum: ["Durum 1.1", "Durum 1.2", "Durum 1.3"],
            altKategori: ["Alt Kategori 1.1", "Alt Kategori 1.2"],
        },
        "Kategori 2": {
            durum: ["Durum 2.1", "Durum 2.2", "Durum 2.3"],
            altKategori: ["Alt Kategori 2.1", "Alt Kategori 2.2"],
        },
        "Kategori 3": {
            durum: ["Durum 3.1", "Durum 3.2", "Durum 3.3"],
            altKategori: ["Alt Kategori 3.1", "Alt Kategori 3.2"],
        },
    };

    function resetSteps() {
        steps.forEach(step => {
            step.classList.add("hidden");
            step.querySelector("ul").innerHTML = ""; // Mevcut durumları temizle
            confirmationText.textContent = ""; // Onay metnini temizle
        });
    }

    function updateSteps() {
        resetSteps();

        const selectedCategory = categorySelect.value;

        if (selectedCategory && categories[selectedCategory]) {
            const selectedStep = document.getElementById(selectedCategory.replace(/\s/g, '-')); // Hedef pencereyi bul
            selectedStep.classList.remove("hidden");

            // Durumları ekle
            const durumList = selectedStep.querySelector("ul");
            categories[selectedCategory].durum.forEach(durum => {
                const li = document.createElement("li");
                li.textContent = durum;
                li.onclick = () => {
                    confirmationText.textContent = `Seçilen Durum: ${durum}`;
                };
                durumList.appendChild(li);
            });

            // Alt Kategorileri ekle
            const altKategoriList = document.getElementById("alt-kategori");
            altKategoriList.innerHTML = ""; // Mevcut alt kategorileri temizlez
            categories[selectedCategory].altKategori.forEach(alt => {
                const li = document.createElement("li");
                li.textContent = alt;
                altKategoriList.appendChild(li);
            });
        }
    }

    categorySelect.addEventListener("change", updateSteps);

    // İlk açılışta güncelle
    updateSteps();
});
