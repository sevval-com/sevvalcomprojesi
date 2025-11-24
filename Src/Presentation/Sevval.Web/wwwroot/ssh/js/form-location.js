document.addEventListener("DOMContentLoaded", () => {   
    const apiBase = `${window.location.origin}/api/location`;
    const provinceSelect = document.getElementById("Province");
    const districtSelect = document.getElementById("District");
    const neighborhoodSelect = document.getElementById("Neighborhood");


    // Yardımcı fonksiyon
    const clearSelect = (select, placeholder = "Seçiniz") => {
        if (select) select.innerHTML = `<option value="">${placeholder}</option>`;
    };

    // UTF-8 destekli JSON fetch
    const fetchJsonUtf8 = async (url) => {
        const response = await fetch(url, {
            headers: { "Accept": "application/json; charset=utf-8" },
        });
        const buffer = await response.arrayBuffer();
        const decoder = new TextDecoder("utf-8");
        const text = decoder.decode(buffer);
        return JSON.parse(text);
    };

    // İller
    fetchJsonUtf8(`${apiBase}/provinces`)
        .then(data => {
            clearSelect(provinceSelect, "İl seçiniz");
            data.forEach(p => {
                if (p && p.trim() !== "")
                    provinceSelect.insertAdjacentHTML("beforeend", `<option value="${p}">${p}</option>`);
            });
            // Hiçbir il otomatik seçili olmasın
            provinceSelect.value = "";
        })
        .catch(err => console.error("İller alınamadı:", err));

    // İl değişimi
    provinceSelect.addEventListener("change", async () => {
        const province = provinceSelect.value;
        clearSelect(districtSelect, "İlçe seçiniz");
        clearSelect(neighborhoodSelect, "Mahalle seçiniz");

        districtSelect.disabled = true;
        neighborhoodSelect.disabled = true;

        if (!province) return;

        try {
            const data = await fetchJsonUtf8(`${apiBase}/districts/${encodeURIComponent(province)}`);
            data.forEach(d => {
                districtSelect.insertAdjacentHTML("beforeend", `<option value="${d.name}">${d.name}</option>`);
            });
            districtSelect.disabled = false;
        } catch (err) {
            console.error("İlçeler alınamadı:", err);
        }
    });

    // İlçe değişimi
    districtSelect.addEventListener("change", async () => {
        const district = districtSelect.value;
        clearSelect(neighborhoodSelect, "Mahalle seçiniz");
        neighborhoodSelect.disabled = true;

        if (!district) return;

        try {
            const data = await fetchJsonUtf8(`${apiBase}/neighborhoods/${encodeURIComponent(district)}`);
            data.forEach(n => {
                neighborhoodSelect.insertAdjacentHTML("beforeend", `<option value="${n.name}">${n.name}</option>`);
            });
            neighborhoodSelect.disabled = false;
        } catch (err) {
            console.error("Mahalleler alınamadı:", err);
        }
    });
});
