window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const { form } = window.AllFormsHandler.config;
    if (!form) return;

    const apiBase = "/api/location";

    const sets = [
        { prov: "Province", dist: "District", neigh: "Neighborhood" },
        { prov: "ProvinceArsa", dist: "DistrictArsa", neigh: "NeighborhoodArsa" }
    ];

    const fetchJsonUtf8 = async (url) => {
        try {
            const res = await fetch(url, {
                headers: { "Accept": "application/json; charset=utf-8" }
            });
            if (!res.ok) return [];
            const buf = await res.arrayBuffer();
            const text = new TextDecoder("utf-8").decode(buf);
            return JSON.parse(text);
        } catch (err) {
            console.error("Fetch Hatası:", url, err);
            return [];
        }
    };

    const clearSelect = (sel, placeholder = "Seçiniz", disable = true) => {
        if (!sel) return;
        sel.innerHTML = `<option value="">${placeholder}</option>`;
        sel.disabled = disable;
    };

    sets.forEach(({ prov, dist, neigh }) => {

        const provinceSelect = document.getElementById(prov);
        const districtSelect = document.getElementById(dist);
        const neighborhoodSelect = document.getElementById(neigh);

        if (!provinceSelect) return;

        clearSelect(provinceSelect, "İl seçiniz", false);
        clearSelect(districtSelect, "İlçe seçiniz");
        clearSelect(neighborhoodSelect, "Mahalle seçiniz");

        // İLLER
        fetchJsonUtf8(`${apiBase}/provinces`)
            .then(provinces => {
                provinces.forEach(p => {
                    const val = typeof p === "string" ? p : p.name;
                    provinceSelect.insertAdjacentHTML("beforeend",
                        `<option value="${val}">${val}</option>`);
                });
            });

        // İLÇE
        provinceSelect.addEventListener("change", async () => {

            const province = provinceSelect.value;
            clearSelect(districtSelect, "İlçe seçiniz");
            clearSelect(neighborhoodSelect, "Mahalle seçiniz");

            if (!province) return;

            const districts = await fetchJsonUtf8(`${apiBase}/districts/${encodeURIComponent(province)}`);
            districts.forEach(d => {
                const val = typeof d === "string" ? d : d.name;
                districtSelect.insertAdjacentHTML("beforeend",
                    `<option value="${val}">${val}</option>`);
            });
            districtSelect.disabled = false;
        });

        // MAHALLE
        districtSelect?.addEventListener("change", async () => {

            const district = districtSelect.value;
            clearSelect(neighborhoodSelect, "Mahalle seçiniz");

            if (!district) return;

            const neighborhoods = await fetchJsonUtf8(`${apiBase}/neighborhoods/${encodeURIComponent(district)}`);
            neighborhoods.forEach(n => {
                const val = typeof n === "string" ? n : n.name;
                neighborhoodSelect.insertAdjacentHTML("beforeend",
                    `<option value="${val}">${val}</option>`);
            });
            neighborhoodSelect.disabled = false;
        });
    });

})();
