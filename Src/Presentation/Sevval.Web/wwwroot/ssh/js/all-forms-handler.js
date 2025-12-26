document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("all-forms-beyan-form");
    if (!form) return;

    /* === STEP2 DEVRE DIŞI === */
    //const step2Section = document.querySelector('.all-forms-step-section[data-step="2"][data-form="common"]');
    //const step2Indicator = document.querySelector('.all-forms-step[data-step="2"]');

    //if (step2Section) step2Section.remove(); 
    //if (step2Indicator) step2Indicator.remove(); 

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
            if (ind.style.display === "none") return; 
            ind.classList.toggle("active", i === index);
            ind.classList.toggle("completed", i < index);
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
        currentStep = index;
    }

    function setActiveFormType(type) {
        activeFormType = type;
        rebuildSteps();

        const step2El = document.querySelector('.all-forms-step-section[data-step="2"][data-form="common"]');
        const step2Ind = document.querySelector('.all-forms-step[data-step="2"]');
        if (step2El) step2El.style.display = "none";
        if (step2Ind) step2Ind.style.display = "none";

        filteredSteps.forEach(s => s.classList.remove("active"));
        currentStep = 2;
        showStep(currentStep);
        updateStepTitles();

        form.querySelectorAll(".invalid, .af-mini-hint").forEach(el => {
            el.classList.remove("invalid");
            if (el.classList.contains("af-mini-hint")) el.remove();
        });
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
            const step2El = document.querySelector('.all-forms-step-section[data-step="2"][data-form="common"]');
            if (!step2El || step2El.style.display === "none") return true;
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

            const step2El = document.querySelector('.all-forms-step-section[data-step="2"][data-form="common"]');
            const step2Indicator = document.querySelector('.all-forms-step[data-step="2"]');
            if (step2El) step2El.style.display = "none";
            if (step2Indicator) step2Indicator.style.display = "none";
           
            setTimeout(() => {
                form.querySelectorAll(".invalid").forEach(el => el.classList.remove("invalid"));
                form.querySelectorAll(".af-mini-hint").forEach(el => el.remove());
            }, 100);
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
            const formType = document.getElementById("FormType")?.value?.toLowerCase();

            const prevStep = filteredSteps[currentStep - 1];
            const commonStep2 = document.querySelector('.all-forms-step-section[data-step="2"][data-form="common"]');
            const type2Step2 = document.querySelector('.all-forms-step-section[data-step="2"][data-form="type2"]');

            const isHiddenCommon = commonStep2 && commonStep2.style.display === "none";
            const isHiddenType2 = type2Step2 && type2Step2.style.display === "none";

            if (
                (prevStep && prevStep.dataset.step === "2" && prevStep.style.display === "none") ||
                (formType === "type2" && isHiddenType2) ||
                (formType === "type1" && isHiddenCommon)
            ) {
                showStep(currentStep - 2);
            } else {
                showStep(currentStep - 1);
            }
        });
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
        /* ==== Yaşadığınız Şehir (City) ==== */
        (() => {
            const sel = document.getElementById("City");
            if (!sel) return;

            sel.innerHTML = `<option value="">Seçiniz</option>`;

            fetch("/api/location/provinces")
                .then(r => r.json())
                .then(list => {
                    list.forEach(p => {
                        sel.insertAdjacentHTML("beforeend", `<option value="${p}">${p}</option>`);
                    });
                })
                .catch(err => console.error("Şehir yükleme hatası:", err));
        })();
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

    /* =================== RAW DEĞER SAKLAMA (DB FIX) =================== */
    function storeRawValue(input) {
        if (!input) return;
        const raw = input.value.replace(/[^\d]/g, "");
        input.dataset.raw = raw || "";
    }

    form.querySelectorAll('input[name="SquareMeter"], input[name="SquareMeterArsa"]').forEach(inp => {
        inp.addEventListener("input", () => storeRawValue(inp));
        inp.addEventListener("blur", () => storeRawValue(inp));
    });

    form.querySelectorAll('input[name="Price"], input[name="PriceArsa"]').forEach(inp => {
        inp.addEventListener("input", () => storeRawValue(inp));
        inp.addEventListener("blur", () => storeRawValue(inp));
    });

    /* =================== EXTRA NUMERIC GÜVENLİK =================== */

    // TYPE1 + TYPE2 — Metrekare 
    form.querySelectorAll('input[name="SquareMeter"], input[name="SquareMeterArsa"]').forEach(inp => {
        inp.addEventListener("input", () => {
            // Sadece rakamları al → max 5 hane
            let raw = inp.value.replace(/[^\d]/g, "").slice(0, 5);

            inp.dataset.raw = raw;

            // Görsel biçimlendirme → 12.345
            inp.value = raw.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        });
        inp.addEventListener("blur", () => storeRawValue(inp));
    });

    // ADA — sadece rakam, max 5
    document.querySelectorAll('input[name="Ada"]').forEach(inp => {
        inp.addEventListener("input", () => {
            let raw = inp.value.replace(/[^\d]/g, "");
            if (raw.length > 5) {
                raw = raw.slice(0, 5);
                shake(inp);
            }
            inp.dataset.raw = raw;
            inp.value = raw; 
        });
    });

    // PARSEL — sadece rakam, max 7
    document.querySelectorAll('input[name="Parsel"]').forEach(inp => {
        inp.addEventListener("input", () => {
            let raw = inp.value.replace(/[^\d]/g, "");
            if (raw.length > 7) {
                raw = raw.slice(0, 7);
                shake(inp);
            }
            inp.dataset.raw = raw;
            inp.value = raw; 
        });
    });

    /* =================== TC =================== */
    form.querySelectorAll('input[name="Tc_Common"]').forEach(inp => {
        // Yazarken/paste ederken temizle ve 11 hanede kes
        inp.addEventListener("input", () => {
            let raw = inp.value.replace(/\D/g, "").slice(0, 11);
            inp.value = raw;
        });
        // Harf basımını engelle (kontrol tuşlarına izin)
        inp.addEventListener("keydown", (e) => {
            if (e.key.length === 1 && !/[0-9]/.test(e.key)) e.preventDefault();
        });
        // Mobil sayısal klavye
        inp.setAttribute("inputmode", "numeric");
        inp.setAttribute("maxlength", "11");
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

    /* ===================  FINAL SECURE SUBMIT ✅ =================== */
    (async () => {

        const toNum = v =>
            v ? Number(String(v).replace(/\./g, "").replace(",", ".")) : null;

        form.addEventListener("submit", async (e) => {
            e.preventDefault();
            console.log("🚀 FINAL SUBMIT tetiklendi");

            // Tapu fotoğraf
            let tapuFotoBase64 = "";
            const formType = document.getElementById("FormType")?.value?.toLowerCase();
            const tapuFile = formType === "type2"
                ? document.querySelector('input[name="TapuFotografi_Type2"]')?.files?.[0]
                : document.querySelector('input[name="TapuFotografi_Type1"]')?.files?.[0];

            if (tapuFile) {
                tapuFotoBase64 = await new Promise((resolve) => {
                    const reader = new FileReader();
                    reader.onload = e => resolve(e.target.result);
                    reader.readAsDataURL(tapuFile);
                });
            }           
            
            function normalize(val) {
                return (val && val.trim() !== "") ? val.trim() : "";
            }

            /*  Ad kontrolü */
            const nameEl = form.querySelector('input[name="Name"]');
            if (nameEl) {
                const v = nameEl.value.trim();
                if (v.length < 3) {
                    markInvalid(nameEl, "En az 3 harf olmalı");
                    shake(nameEl);
                    return;
                } else {
                    clearInvalid(nameEl);
                }
            }

            /*  Telefon kontrolü */
            const phoneEl = form.querySelector('input[name="Phone"]');
            if (phoneEl) {
                const v = phoneEl.value.trim();
                const regex = /^0\d{3}\s\d{3}\s\d{2}\s\d{2}$/;
                if (!regex.test(v)) {
                    markInvalid(phoneEl, "Format 0555 555 55 55 olmalı");
                    shake(phoneEl);
                    return;
                } else {
                    clearInvalid(phoneEl);
                }
            }

            /*  Email kontrolü */
            const emailEl = form.querySelector('input[name="Email"]');
            if (emailEl) {
                const v = emailEl.value.trim();
                const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!regex.test(v)) {
                    markInvalid(emailEl, "Geçerli e-posta giriniz");
                    shake(emailEl);
                    return;
                } else {
                    clearInvalid(emailEl);
                }
            }            

            if (!validateStep()) {
                console.warn("❌ Step validation fail!");
                return;
            }

            const body = {
                FormKey: "Beyan",
                FormName: "Beyan Vermek İstiyorum",
                FormType: document.getElementById("FormType")?.value || "type1",

                Category: document.querySelector('input[name="Category"]:checked')?.value || "",
                Status: document.querySelector('input[name="Status"]:checked')?.value || "",

                Province: document.getElementById("Province")?.value ||
                    document.getElementById("ProvinceArsa")?.value || "",
                District: document.getElementById("District")?.value ||
                    document.getElementById("DistrictArsa")?.value || "",
                Neighborhood: document.getElementById("Neighborhood")?.value ||
                    document.getElementById("NeighborhoodArsa")?.value || "",

                // === Yeni eklenen alanlar ===
                //NitelikDurumu: document.querySelector(`[name^="NitelikDurumu_"]`)?.value || "",
                //TapuDurumu: document.querySelector(`[name^="TapuDurumu_"]`)?.value || "",
                //YevmiyeTarihi: document.querySelector(`[name^="YevmiyeTarihi_"]`)?.value || "",
                //Tc: document.querySelector('[name="Tc_Common"]')?.value || "",
                //TapuFotografi: tapuFotoBase64 || "",

                // === Yeni eklenen alanlar ===
                //NitelikDurumu: document.querySelector(`[name^="NitelikDurumu_"]`)?.value ||
                //    document.querySelector(`[name="NitelikDurumu_Type2"]`)?.value || "",
                //TapuDurumu: document.querySelector(`[name^="TapuDurumu_"]`)?.value ||
                //    document.querySelector(`[name="TapuDurumu_Type2"]`)?.value || "",
                //YevmiyeTarihi: document.querySelector(`[name^="YevmiyeTarihi_"]`)?.value ||
                //    document.querySelector(`[name="YevmiyeTarihi_Type2"]`)?.value || "",
                //Tc: document.querySelector('[name="Tc_Common"]')?.value || "",
                //TapuFotografi: tapuFotoBase64 || "",

                // === Yeni eklenen alanlar ===
                NitelikDurumu: document.querySelector('[name="NitelikDurumu_Type1"]')?.value ||
                    document.querySelector('[name="NitelikDurumu_Type2"]')?.value || "",

                TapuDurumu: document.querySelector('[name="TapuDurumu_Type1"]')?.value ||
                    document.querySelector('[name="TapuDurumu_Type2"]')?.value || "",

                YevmiyeTarihi: (document.querySelector('[name="YevmiyeTarihi_Type1"]')?.value ||
                    document.querySelector('[name="YevmiyeTarihi_Type2"]')?.value || "") || undefined,

                Tc: document.querySelector('[name="Tc_Common"]')?.value || "",

                TapuFotografi: tapuFotoBase64 || "",
                TapuFotografi_Type2: tapuFotoBase64 || "",

                // TYPE1
                RoomCount: document.querySelector('select[name="RoomCount"]')?.value || "",
                SquareMeter: toNum(document.querySelector('input[name="SquareMeter"]')?.dataset.raw),
                BuildingAge: document.querySelector('select[name="BuildingAge"]')?.value || "",
                Floor: document.querySelector('select[name="Floor"]')?.value || "",
                Heating: document.querySelector('select[name="Heating"]')?.value || "",
                BathCount: document.querySelector('select[name="BathCount"]')?.value || "",
                BalconyCount: document.querySelector('select[name="BalconyCount"]')?.value || "",
                Price: toNum(document.querySelector('input[name="Price"]')?.dataset.raw),

                // TYPE2
                Ada: document.querySelector('input[name="Ada"]')?.value || "",
                Parsel: document.querySelector('input[name="Parsel"]')?.value || "",
                MeyilDurumu: document.querySelector('select[name="MeyilDurumu"]')?.value || "",
                YolDurumu: document.querySelector('select[name="YolDurumu"]')?.value || "",
                YerlesimUzaklik: document.querySelector('select[name="YerlesimUzaklik"]')?.value || "",
                ImarDurumu: document.querySelector('select[name="ImarDurumu"]')?.value || "",
                SquareMeterArsa: toNum(document.querySelector('input[name="SquareMeterArsa"]')?.dataset.raw),
                PriceArsa: toNum(document.querySelector('input[name="PriceArsa"]')?.dataset.raw),

                // PERSONAL
                Name: nameEl?.value?.trim() || "",
                Surname: document.querySelector('input[name="Surname"]')?.value || "",
                Phone: phoneEl?.value?.trim() || "",
                Email: emailEl?.value?.trim() || "",
                City: document.querySelector('select[name="City"]')?.value || "",

                /*FilePath: "",*/
                RawPayload: Object.fromEntries(new FormData(form)),
                CreatedAt: new Date().toISOString()
            };

            // === TYPE1 Tapu Fotoğrafı ===
            if ((document.getElementById("FormType")?.value || "").toLowerCase() === "type1") {
                body.TapuFotografi_Type1 = tapuFotoBase64 || "";
            }

            // === TYPE2 Tapu Fotoğrafı ===
            if ((document.getElementById("FormType")?.value || "").toLowerCase() === "type2") {
                body.TapuFotografi_Type2 = tapuFotoBase64 || "";
            }

            console.log("📤 API BODY:", body);

            try {
                const res = await fetch("/api/forms", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(body)
                });

                if (!res.ok) {
                    const errText = await res.text();
                    console.error("❌ Server error:", errText);
                    alert("⚠️ Form kaydedilemedi!");
                    return;
                }

                const data = await res.json();
                console.log("✅ API OK:", data);

                showStep(filteredSteps.length - 1);
            } catch (err) {
                console.error("❌ Fetch error:", err);
                alert("⚠️ Sunucuya ulaşılamadı!");
            }
        });
    })();
});