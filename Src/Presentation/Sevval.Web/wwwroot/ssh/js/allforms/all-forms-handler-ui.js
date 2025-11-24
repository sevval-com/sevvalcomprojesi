window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const { form } = window.AllFormsHandler.config;
    const { clearInvalid, shake } = window.AllFormsHandler.validation;

    // STYLE INJECT
    (function () {
        const css = `
        .invalid{border:2px solid #ff4d4f!important;background:#fff8f8;}
        .shake{animation:shake .3s ease;}
        @keyframes shake{0%,100%{transform:translateX(0)}25%{transform:translateX(-5px)}50%{transform:translateX(5px)}75%{transform:translateX(-5px)}}
        .af-mini-hint{margin-top:4px;font-size:12px;line-height:1;color:#dc3545;}
        .all-forms-category-grid label.selected,
        .all-forms-status-grid label.selected{border:2px solid #0d6efd;background:#e8f1ff;}
        `;
        const style = document.createElement("style");
        style.textContent = css;
        document.head.appendChild(style);
    })();

    form.querySelectorAll("input, select").forEach(el => {
        el.addEventListener("input", () => clearInvalid(el));
        el.addEventListener("change", () => clearInvalid(el));
    });

    // Numeric Formatters
    function formatTR(v) {
        v = v.replace(/[^\d,]/g, "");
        const parts = v.split(",");
        let i = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        return parts.length > 1 ? `${i},${parts[1].slice(0, 1)}` : i;
    }

    form.querySelectorAll("input[data-numeric]").forEach(inp =>
        inp.addEventListener("input", e => e.target.value = formatTR(e.target.value))
    );

    form.querySelectorAll('input[name="Price"], input[name="PriceArsa"]')
        .forEach(inp => inp.addEventListener("input", e => {
            let v = e.target.value.replace(/[^\d]/g, "");
            if (v.length > 12) {
                e.target.value = formatTR(v.slice(0, 12));
                shake(e.target);
            } else {
                e.target.value = formatTR(v);
            }
        }));

    const phoneInput = form.querySelector('input[name="Phone"]');
    phoneInput?.addEventListener("input", e => {
        let v = e.target.value.replace(/[^\d]/g, "");
        if (v.length > 11) v = v.slice(0, 11);
        const p = [];
        if (v.length > 0) p.push(v.substring(0, 4));
        if (v.length > 4) p.push(v.substring(4, 7));
        if (v.length > 7) p.push(v.substring(7, 9));
        if (v.length > 9) p.push(v.substring(9, 11));
        e.target.value = p.join(" ");
    });

    // Radio highlight
    function wireRadio(group, cont) {
        const radios = form.querySelectorAll(`input[name="${group}"]`);
        const c = form.querySelector(cont);
        radios.forEach(r => r.addEventListener("change", () => {
            c.querySelectorAll("label").forEach(l => l.classList.remove("selected"));
            r.closest("label")?.classList.add("selected");
        }));
    }
    wireRadio("Category", ".all-forms-category-grid");
    wireRadio("Status", ".all-forms-status-grid");

    window.AllFormsHandler.ui = { formatTR };

    // ========== NUMERIC MASKS & m² UNIT (TYPE1 & TYPE2) ==========
    (function () {
        const form = window.AllFormsHandler?.config?.form;
        if (!form) return;

        // Ada / Parsel — sadece rakam
        ["Ada", "Parsel"].forEach(name => {
            const el = form.querySelector(`input[name="${name}"]`);
            if (!el) return;
            el.addEventListener("input", () => {
                // Sadece rakam kalsın
                const digits = (el.value || "").replace(/\D/g, "");
                el.value = digits;
            });
        });

        // m² birimi ekle (sağ tarafa)
        //function attachM2Unit(input) {
        //    if (!input) return;
        //    const wrap = input.closest("div");
        //    if (!wrap) return;
        //    if (wrap.querySelector(".af-unit-m2")) return; 

        //    const unit = document.createElement("span");
        //    unit.className = "af-unit-m2";
        //    unit.innerHTML = 'm<sup>2</sup>';
        //    unit.style.position = "absolute";
        //    unit.style.right = "10px";
        //    unit.style.top = "50%";
        //    unit.style.transform = "translateY(-50%)";
        //    unit.style.pointerEvents = "none";
        //    unit.style.color = "#6c757d";
        //    unit.style.fontSize = "12px";

        //    const style = getComputedStyle(wrap);
        //    if (style.position === "static") wrap.style.position = "relative";

        //    const p = parseInt(getComputedStyle(input).paddingRight || "0", 10);
        //    input.style.paddingRight = (p + 28) + "px";

        //    wrap.appendChild(unit);
        //}

        // TR sayısal format: 5 haneli tamsayı (maks), 1 ondalık (virgüllü), binlikte nokta
        function formatTRLimited(v) {
            // sadece rakam ve virgül
            v = (v || "").replace(/[^\d,]/g, "");

            // parçala
            let [intPart, decPart] = v.split(",");
            intPart = (intPart || "").replace(/\D/g, "");

            // maksimum 5 hane (tamsayı kısmı)
            intPart = intPart.slice(0, 5);

            // binlik noktalama
            intPart = intPart.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

            // ondalık kısmı 1 hane ile sınırla (opsiyonel)
            if (decPart) decPart = decPart.replace(/\D/g, "").slice(0, 1);

            return decPart ? `${intPart},${decPart}` : intPart;
        }

        // Type1 & Type2 metrekare alanları: SquareMeter / SquareMeterArsa
        const sqmInputs = [
            form.querySelector('input[name="SquareMeter"]'),
            form.querySelector('input[name="SquareMeterArsa"]')
        ].filter(Boolean);

        sqmInputs.forEach(inp => {
            attachM2Unit(inp);
            inp.addEventListener("input", (e) => {
                const cur = e.target.value;
                const formatted = formatTRLimited(cur);
                if (cur !== formatted) e.target.value = formatted;
            });
        });
    })();
})();
