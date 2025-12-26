<script>
    // Pop-up açma işlemi
    document.getElementById("floatingLogo").onclick = function() {
        document.getElementById("myPopup").style.display = "block";
    }

    // Pop-up kapama işlemi
    document.getElementById("closePopup").onclick = function() {
        document.getElementById("myPopup").style.display = "none";
    }

    // Kategori butonuna tıklama
    document.getElementById("categoryBtn").onclick = function() {
        document.getElementById("step1").style.display = "none";
    document.getElementById("step2").style.display = "block";
    }

    // WhatsApp linkini doldurma
    document.getElementById("step5").onclick = function() {
        const whatsappNumber = "1234567890"; // Buraya gerçek numarayı yazın
    document.getElementById("whatsappLink").href = "https://wa.me/" + whatsappNumber + "?text=" + encodeURIComponent(getFormData());
    }

    function getFormData() {
        return `Mülk Tipi: ${document.getElementById("propertyType").value}, Oda Sayısı: ${document.getElementById("roomCount").value}, Metrekare: ${document.getElementById("area").value}, Yaş: ${document.getElementById("age").value}, Kat: ${document.getElementById("floor").value}, Toplam Kat: ${document.getElementById("totalFloors").value}`;
    }
</script>
