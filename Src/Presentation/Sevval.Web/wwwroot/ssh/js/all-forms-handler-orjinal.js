document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("all-forms-beyan-form");
    if (!form) return;

    /* =================== KATEGORİ - FORM TİP MAPS =================== */
    const formTypeMap = {
        "Konut": "type1",
        "Turistik Tesis": "type1",
        "İş Yeri": "type1",
        "Arsa": "type2",
        "Tarla": "type2",
        "Bahçe": "type2"
    };

    const steps = Array.from(document.querySelectorAll(".all-forms-step-section"));
    const indicators = Array.from(document.querySelectorAll(".all-forms-beyan-steps .all-forms-step"));
    let activeFormType = "type1";
    let filteredSteps = [];
    let currentStep = 0;
    let selectedCategory = "";

    /* =================== STYLE =================== */
    (function injectStyles() {
        const css = `
        .invalid{border:2px solid #ff4d4f!important;background:#fff8f8;}
        .shake{animation:shake .3s ease;}
        @keyframes shake{0%,100%{transform:translateX(0)}25%{transform:translateX(-5px)}50%{transform:translateX(5px)}75%{transform:translateX(-5px)}}
        .af-mini-hint{display:block;margin-top:4px;font-size:12px;line-height:1;color:#dc3545;}
        .all-forms-category-grid label.selected, .all-forms-status-grid label.selected{
            border:2px solid #0d6efd;background:#e8f1ff;box-shadow:0 6px 14px rgba(13,110,253,.18);
        }
        .all-forms-category-grid label.selected i, .all-forms-status-grid label.selected i,
        .all-forms-category-grid label.selected span, .all-forms-status-grid label.selected span{color:#0d6efd!important;}
        `;
        const style = document.createElement("style");
        style.textContent = css;
        document.head.appendChild(style);
    })();

    /* =================== HATA İŞLEMLERİ =================== */
    function markInvalid(el, extraMsg = "") {
        if (!el) return;
        el.classList.add("invalid");

        const wrap = el.closest("div");
        if (wrap) {
            const oldHint = wrap.querySelector(".af-mini-hint");
            if (oldHint) oldHint.remove();

            const small = document.createElement("small");
            small.className = "af-mini-hint";

            let baseMsg = "* Zorunlu Alan";

            if (extraMsg && extraMsg.trim() !== "") {
                baseMsg += " – " + extraMsg.trim();
            }

            small.textContent = baseMsg;
            wrap.appendChild(small);
        }
    }

    function clearInvalid(el) {
        if (!el) return;
        el.classList.remove("invalid");
        const wrap = el.closest("div");
        const hint = wrap?.querySelector(".af-mini-hint");
        if (hint) hint.remove();
    }

    function shake(selector) {
        const el = typeof selector === "string" ? document.querySelector(selector) : selector;
        if (!el) return;
        el.classList.add("shake");
        setTimeout(() => el.classList.remove("shake"), 400);
    }

    /* =================== ADIM YÖNETİMİ =================== */
    function rebuildSteps() {
        filteredSteps = steps.filter(s => s.dataset.form === "common" || s.dataset.form === activeFormType);
    }

    function showStep(index) {
        if (index < 0) index = 0;
        if (index >= filteredSteps.length) index = filteredSteps.length - 1;

        filteredSteps.forEach((s, i) => {
            s.style.display = i === index ? "block" : "none";
            s.classList.toggle("active", i === index);
        });

        indicators.forEach((ind, i) => {
            ind.classList.toggle("active", i === index);
            ind.classList.toggle("completed", i < index);
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
        currentStep = index;
    }

    function setActiveFormType(type) {
        activeFormType = type;
        rebuildSteps();
        currentStep = 0;
        showStep(1);
        updateStepTitles();
    }

    /* =================== BAŞLIKLARI GÜNCELLE =================== */
    function updateStepTitles() {
        const titles = document.querySelectorAll('[data-dynamic-title]');
        titles.forEach(t => {
            if (selectedCategory) {
                t.textContent = `Mülk Bilgileri (${selectedCategory})`;
            } else {
                t.textContent = `Mülk Bilgileri`;
            }
        });
    }

    /* =================== DOĞRULAMA =================== */
    function validateStep() {
        let rules = [];

        if (currentStep === 0) {
            rules = ['radio:Category'];
        }
        else if (currentStep === 1) {
            rules = ['radio:Status'];
        }
        else if (currentStep === 2) {
            if (activeFormType === "type2") {
                rules = [
                    '#ProvinceArsa', '#DistrictArsa', '#NeighborhoodArsa',
                    'input[name="Ada"]',
                    'input[name="Parsel"]',
                    'input[name="SquareMeterArsa"]',
                    'select[name="MeyilDurumu"]',
                    'select[name="YolDurumu"]',
                    'select[name="YerlesimUzaklik"]',
                    'select[name="ImarDurumu"]',
                    'input[name="PriceArsa"]'
                ];
            } else {
                rules = [
                    '#Province', '#District', '#Neighborhood',
                    'select[name="RoomCount"]',
                    'input[name="SquareMeter"]',
                    'select[name="BuildingAge"]',
                    'select[name="Floor"]',
                    'select[name="Heating"]',
                    'select[name="BathCount"]',
                    'select[name="BalconyCount"]',
                    'input[name="Price"]'
                ];
            }
        }
        else if (currentStep === 3) {
            rules = [
                'input[name="Name"]',
                'input[name="Surname"]',
                'input[name="Phone"]',
                'input[name="Email"]',
                'input[name="City"]'
            ];
        }

        let ok = true;

        for (const rule of rules) {
            if (rule.startsWith("radio:")) {
                const name = rule.split(":")[1];
                const checked = form.querySelector(`input[name="${name}"]:checked`);
                if (!checked) {
                    shake(name === "Category" ? ".all-forms-category-grid" : ".all-forms-status-grid");
                    ok = false;
                }
                continue;
            }

            const el = form.querySelector(rule);
            if (el && el.offsetParent !== null) {
                const val = (el.value || '').trim();
                if (!val) {
                    markInvalid(el);
                    ok = false;
                } else {
                    clearInvalid(el);
                }
            }
        }

        return ok;
    }

    /* =================== EVENT =================== */
    form.querySelectorAll("input, select").forEach(el => {
        el.addEventListener("input", () => clearInvalid(el));
        el.addEventListener("change", () => clearInvalid(el));
    });

    const firstNext = form.querySelector('.all-forms-step-section[data-step="1"] .all-forms-next-btn');
    if (firstNext) {
        firstNext.addEventListener("click", () => {
            if (!validateStep()) return;
            const selected = form.querySelector('input[name="Category"]:checked');
            if (!selected) return;
            selectedCategory = selected.value;
            const chosenType = formTypeMap[selectedCategory];
            document.getElementById("SelectedCategory").value = selectedCategory;
            document.getElementById("FormType").value = chosenType;
            setActiveFormType(chosenType);
        });
    }

    form.querySelectorAll(".all-forms-next-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            if (!validateStep()) return;
            showStep(currentStep + 1);
        });
    });

    form.querySelectorAll(".all-forms-prev-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            showStep(currentStep - 1);
        });
    });

    /* =================== SUBMIT =================== */
    async function submitDynamicForm() {
        const toNum = v => v ? Number(String(v).replace(/\./g, "").replace(",", ".")) : null;

        const formKey = "Beyan";
        const formName = "Beyan Vermek İstiyorum";
        const formType = document.getElementById("FormType")?.value || "type1";
        const category = document.querySelector('input[name="Category"]:checked')?.value
            || document.getElementById("SelectedCategory")?.value || "";
        const status = document.querySelector('input[name="Status"]:checked')?.value || null;

        const province = document.getElementById("Province")?.value || document.getElementById("ProvinceArsa")?.value || null;
        const district = document.getElementById("District")?.value || document.getElementById("DistrictArsa")?.value || null;
        const neighborhood = document.getElementById("Neighborhood")?.value || document.getElementById("NeighborhoodArsa")?.value || null;

        const roomCount = document.querySelector('select[name="RoomCount"]')?.value || null;
        const squareMeter = toNum(document.querySelector('input[name="SquareMeter"]')?.value);
        const buildingAge = document.querySelector('select[name="BuildingAge"]')?.value || null;
        const floor = document.querySelector('select[name="Floor"]')?.value || null;
        const heating = document.querySelector('select[name="Heating"]')?.value || null;
        const bathCount = document.querySelector('select[name="BathCount"]')?.value || null;
        const balconyCount = document.querySelector('select[name="BalconyCount"]')?.value || null;
        const price = toNum(document.querySelector('input[name="Price"]')?.value);

        const ada = document.querySelector('input[name="Ada"]')?.value || null;
        const parsel = document.querySelector('input[name="Parsel"]')?.value || null;
        const meyilDurumu = document.querySelector('select[name="MeyilDurumu"]')?.value || null;
        const yolDurumu = document.querySelector('select[name="YolDurumu"]')?.value || null;
        const yerlesimUzaklik = document.querySelector('select[name="YerlesimUzaklik"]')?.value || null;
        const imarDurumu = document.querySelector('select[name="ImarDurumu"]')?.value || null;
        const squareMeterArsa = toNum(document.querySelector('input[name="SquareMeterArsa"]')?.value);
        const priceArsa = toNum(document.querySelector('input[name="PriceArsa"]')?.value);

        const name = document.querySelector('input[name="Name"]')?.value || null;
        const surname = document.querySelector('input[name="Surname"]')?.value || null;
        const phone = document.querySelector('input[name="Phone"]')?.value || null;
        const email = document.querySelector('input[name="Email"]')?.value || null;
        const city = document.querySelector('input[name="City"]')?.value || null;

        // rawPayload backend Dictionary<string, object> 
        const rawPayload = {};
        document.querySelectorAll('#all-forms-beyan-form input, #all-forms-beyan-form select').forEach(el => {
            const key = el.name || el.id;
            if (!key) return;
            const v = (el.type === "radio") ? (el.checked ? el.value : null) : el.value;
            rawPayload[key] = v ?? "";
        });

        // backend DTO JSON
        const body = {
            formKey,
            formName,
            formType,
            category,
            status,
            province,
            district,
            neighborhood,
            roomCount,
            squareMeter,
            buildingAge,
            floor,
            heating,
            bathCount,
            balconyCount,
            price,
            ada,
            parsel,
            meyilDurumu,
            yolDurumu,
            yerlesimUzaklik,
            imarDurumu,
            squareMeterArsa,
            priceArsa,
            name,
            surname,
            phone,
            email,
            city,
            filePath: "", 
            rawPayload // Dictionary 
        };

        try {
            const res = await fetch("/DynamicForm/Create", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body)
            });

            if (!res.ok) {
                const t = await res.text();
                console.error("🧩 Form kaydı başarısız:", t);
                alert("⚠️ Form kaydedilemedi, lütfen tekrar deneyin.");
                return;
            }

            console.log("✅ Form başarıyla kaydedildi");
            if (typeof showStep === "function") {
                showStep(filteredSteps.length - 1); 
            } else {
                alert("✅ Form başarıyla kaydedildi!");
            }

        } catch (err) {
            console.error("🧩 Fetch hatası:", err);
            alert("⚠️ Sunucuya bağlanılamadı, lütfen internet bağlantınızı kontrol edin.");
        }
    }

    form.addEventListener("submit", async e => {
        e.preventDefault();
        console.log("✅ Submit event tetiklendi"); // debug 
        if (!validateStep()) return;
        await submitDynamicForm();
    });

    /* =================== RADIO Color =================== */
    function wireRadioHighlight(group, container) {
        const radios = form.querySelectorAll(`input[name="${group}"]`);
        const cont = form.querySelector(container);
        radios.forEach(r => {
            r.addEventListener("change", () => {
                cont.querySelectorAll("label").forEach(l => l.classList.remove("selected"));
                r.closest("label")?.classList.add("selected");
            });
        });
    }
    wireRadioHighlight("Category", ".all-forms-category-grid");
    wireRadioHighlight("Status", ".all-forms-status-grid");

    /* =================== LOKASYON YÜKLEME =================== */
    (function () {
        const apiBase = "/api/location";
        const locationSets = [
            { prov: "Province", dist: "District", neigh: "Neighborhood" },
            { prov: "ProvinceArsa", dist: "DistrictArsa", neigh: "NeighborhoodArsa" }
        ];
        const fetchJsonUtf8 = async (url) => {
            try {
                const res = await fetch(url, { headers: { "Accept": "application/json; charset=utf-8" } });
                if (!res.ok) return [];
                const buffer = await res.arrayBuffer();
                const decoder = new TextDecoder("utf-8");
                const text = decoder.decode(buffer);
                return JSON.parse(text);
            } catch (err) {
                console.error("Fetch hatası:", url, err);
                return [];
            }
        };
        const clearSelect = (sel, placeholder = "Seçiniz", disable = true) => {
            if (!sel) return;
            sel.innerHTML = `<option value="">${placeholder}</option>`;
            sel.disabled = disable;
        };
        locationSets.forEach(({ prov, dist, neigh }) => {
            const provinceSelect = document.getElementById(prov);
            const districtSelect = document.getElementById(dist);
            const neighborhoodSelect = document.getElementById(neigh);
            if (!provinceSelect) return;

            clearSelect(provinceSelect, "İl seçiniz", false);
            clearSelect(districtSelect, "İlçe seçiniz");
            clearSelect(neighborhoodSelect, "Mahalle seçiniz");

            fetchJsonUtf8(`${apiBase}/provinces`).then(provinces => {
                provinces.forEach(p => {
                    if (p && p.trim() !== "")
                        provinceSelect.insertAdjacentHTML("beforeend", `<option value="${p}">${p}</option>`);
                });
            });

            provinceSelect.addEventListener("change", async () => {
                const province = provinceSelect.value;
                clearSelect(districtSelect, "İlçe seçiniz");
                clearSelect(neighborhoodSelect, "Mahalle seçiniz");
                if (!province) return;
                const districts = await fetchJsonUtf8(`${apiBase}/districts/${encodeURIComponent(province)}`);
                districts.forEach(d => {
                    const val = typeof d === "string" ? d : d.name;
                    districtSelect.insertAdjacentHTML("beforeend", `<option value="${val}">${val}</option>`);
                });
                districtSelect.disabled = false;
            });

            districtSelect?.addEventListener("change", async () => {
                const district = districtSelect.value;
                clearSelect(neighborhoodSelect, "Mahalle seçiniz");
                if (!district) return;
                const neighborhoods = await fetchJsonUtf8(`${apiBase}/neighborhoods/${encodeURIComponent(district)}`);
                neighborhoods.forEach(n => {
                    const val = typeof n === "string" ? n : n.name;
                    neighborhoodSelect.insertAdjacentHTML("beforeend", `<option value="${val}">${val}</option>`);
                });
                neighborhoodSelect.disabled = false;
            });
        });
    })();

    /* =================== SAYISAL =================== */
    function formatTR(v) {
        v = v.replace(/[^\d,]/g, "");
        const parts = v.split(",");
        let intPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        return parts.length > 1 ? `${intPart},${parts[1].slice(0, 1)}` : intPart;
    }
    form.querySelectorAll("input[data-numeric]").forEach(inp => {
        inp.addEventListener("input", e => e.target.value = formatTR(e.target.value));
    });

    /* =================== FİYAT LİMİTİ =================== */
    form.querySelectorAll('input[name="Price"], input[name="PriceArsa"]').forEach(inp => {
        inp.addEventListener("input", e => {
            let v = e.target.value.replace(/[^\d]/g, "");
            if (v.length > 12) {
                v = v.slice(0, 12); // max 12 hane
                shake(e.target);
            }
            e.target.value = formatTR(v);
        });
    });

    /* =================== TELEFON MASK =================== */
    const phoneInput = form.querySelector('input[name="Phone"]');
    if (phoneInput) {
        phoneInput.addEventListener("input", (e) => {
            let v = e.target.value.replace(/[^\d]/g, "");
            if (v.length > 11) v = v.slice(0, 11);
            const parts = [];
            if (v.length > 0) parts.push(v.substring(0, 4));
            if (v.length > 4) parts.push(v.substring(4, 7));
            if (v.length > 7) parts.push(v.substring(7, 9));
            if (v.length > 9) parts.push(v.substring(9, 11));
            e.target.value = parts.join(" ");
        });
    }

    /* =================== BAŞLANGIÇ =================== */
    rebuildSteps();
    steps.forEach(s => s.style.display = "none");
    showStep(0);

    /* =================== PATCH: Güvenli Submit =================== */
    form.querySelectorAll('button[type="submit"], .all-forms-submit-btn').forEach(btn => {
        btn.addEventListener("click", async (e) => {
            e.preventDefault();
            console.log("🧩 PATCH: Submit butonu tetiklendi");

            // Doğrulama

            // Ad kontrolü
            const nameEl = form.querySelector('input[name="Name"]');
            if (nameEl) {
                const nameVal = nameEl.value.trim();
                if (nameVal.length < 3) {
                    markInvalid(nameEl, "En az 3 harf olmalı");
                    shake(nameEl);
                    return;
                } else {
                    clearInvalid(nameEl);
                }
            }      

            // E-posta kontrolü
            const emailEl = form.querySelector('input[name="Email"]');
            if (emailEl) {
                const email = emailEl.value.trim();
                const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (email && !emailPattern.test(email)) {
                    markInvalid(emailEl, "Geçerli e-posta giriniz");
                    shake(emailEl);
                    return;
                } else {
                    clearInvalid(emailEl);
                }
            }         

            // Telefon kontrolü
            const phoneEl = form.querySelector('input[name="Phone"]');
            if (phoneEl) {
                const phone = phoneEl.value.trim();
                const phonePattern = /^0\d{3}\s\d{3}\s\d{2}\s\d{2}$/;
                if (phone && !phonePattern.test(phone)) {
                    markInvalid(phoneEl, "Format 0555 555 55 55 olmalı");
                    shake(phoneEl);
                    return;
                } else {
                    clearInvalid(phoneEl);
                }
            }

            if (!validateStep()) {
                console.warn("🧩 PATCH: Validation failed");
                return;
            }

            try {
                const formEl = document.getElementById("all-forms-beyan-form");

                // Sayısal  
                const toNum = v => {
                    if (!v) return null;
                    const n = Number(String(v).replace(/\./g, "").replace(",", "."));
                    return isNaN(n) ? null : n;
                };

                // Body (API DTO property)
                const body = {
                    FormKey: "Beyan",
                    FormName: "Beyan Vermek İstiyorum",
                    FormType: document.getElementById("FormType")?.value || "type1",
                    Category: document.querySelector('input[name="Category"]:checked')?.value || "",
                    Status: document.querySelector('input[name="Status"]:checked')?.value || "",
                    Province: document.getElementById("Province")?.value || document.getElementById("ProvinceArsa")?.value || "",
                    District: document.getElementById("District")?.value || document.getElementById("DistrictArsa")?.value || "",
                    Neighborhood: document.getElementById("Neighborhood")?.value || document.getElementById("NeighborhoodArsa")?.value || "",

                    // type1
                    RoomCount: document.querySelector('select[name="RoomCount"]')?.value || "",
                    SquareMeter: toNum(document.querySelector('input[name="SquareMeter"]')?.value),
                    BuildingAge: document.querySelector('select[name="BuildingAge"]')?.value || "",
                    Floor: document.querySelector('select[name="Floor"]')?.value || "",
                    Heating: document.querySelector('select[name="Heating"]')?.value || "",
                    BathCount: document.querySelector('select[name="BathCount"]')?.value || "",
                    BalconyCount: document.querySelector('select[name="BalconyCount"]')?.value || "",
                    Price: toNum(document.querySelector('input[name="Price"]')?.value),

                    // type2
                    Ada: document.querySelector('input[name="Ada"]')?.value || "",
                    Parsel: document.querySelector('input[name="Parsel"]')?.value || "",
                    MeyilDurumu: document.querySelector('select[name="MeyilDurumu"]')?.value || "",
                    YolDurumu: document.querySelector('select[name="YolDurumu"]')?.value || "",
                    YerlesimUzaklik: document.querySelector('select[name="YerlesimUzaklik"]')?.value || "",
                    ImarDurumu: document.querySelector('select[name="ImarDurumu"]')?.value || "",
                    SquareMeterArsa: toNum(document.querySelector('input[name="SquareMeterArsa"]')?.value),
                    PriceArsa: toNum(document.querySelector('input[name="PriceArsa"]')?.value),

                    // personal
                    Name: document.querySelector('input[name="Name"]')?.value || "",
                    Surname: document.querySelector('input[name="Surname"]')?.value || "",
                    Phone: document.querySelector('input[name="Phone"]')?.value || "",
                    Email: document.querySelector('input[name="Email"]')?.value || "",
                    City: document.querySelector('input[name="City"]')?.value || "",

                    // extra
                    FilePath: "",
                    RawPayload: Object.fromEntries(new FormData(formEl)),
                    CreatedAt: new Date().toLocaleString("tr-TR", { timeZone: "Europe/Istanbul" })
                };

                // API çağrısı (MVC + API)
                const res = await fetch("/api/forms", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify(body)
                });

                // Hata yönetimi
                if (!res.ok) {
                    const t = await res.text();
                    console.error("PATCH: Sunucu hatası →", t);
                    alert("Form kaydedilemedi, lütfen tekrar deneyin.");
                    return;
                }

                console.log("PATCH: Form başarıyla kaydedildi.");
                const data = await res.json();
                console.log("API Yanıtı:", data);

                // Başarı sonrası Step 5’e geç
                if (typeof showStep === "function") {
                    showStep(filteredSteps.length - 1);
                } else {
                    alert("Form başarıyla kaydedildi!");
                }

            } catch (err) {
                console.error("🧩 PATCH: Fetch hatası", err);
                alert("Sunucuya ulaşılamadı veya beklenmedik hata oluştu.");
            }
        });
    });
});
