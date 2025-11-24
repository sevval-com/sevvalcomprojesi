window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const { form } = window.AllFormsHandler.config;

    function markInvalid(el, extraMsg = "") {
        if (!el) return;
        el.classList.add("invalid");

        const wrap = el.closest("div");
        if (!wrap) return;

        const oldHint = wrap.querySelector(".af-mini-hint");
        if (oldHint) oldHint.remove();

        const small = document.createElement("small");
        small.className = "af-mini-hint";

        let baseMsg = "* Zorunlu Alan";
        if (extraMsg) baseMsg += " – " + extraMsg;

        small.textContent = baseMsg;
        wrap.appendChild(small);
    }

    function clearInvalid(el) {
        if (!el) return;
        el.classList.remove("invalid");

        const wrap = el.closest("div");
        if (!wrap) return;

        const hint = wrap.querySelector(".af-mini-hint");
        if (hint) hint.remove();
    }

    function shake(elOrSelector) {
        const el = typeof elOrSelector === "string" ? document.querySelector(elOrSelector) : elOrSelector;
        if (!el) return;
        el.classList.add("shake");
        setTimeout(() => el.classList.remove("shake"), 400);
    }

    function validateStep() {
        const { currentStep, activeFormType } = window.AllFormsHandler.config;
        let rules = [];

        if (currentStep === 0) rules = ['radio:Category'];
        else if (currentStep === 1) rules = ['radio:Status'];
        else if (currentStep === 2) {
            rules = activeFormType === "type2" ?
                ['#ProvinceArsa', '#DistrictArsa', '#NeighborhoodArsa',
                    'input[name="Ada"]', 'input[name="Parsel"]',
                    'input[name="SquareMeterArsa"]', 'select[name="MeyilDurumu"]',
                    'select[name="YolDurumu"]', 'select[name="YerlesimUzaklik"]',
                    'select[name="ImarDurumu"]', 'input[name="PriceArsa"]']
                :
                ['#Province', '#District', '#Neighborhood',
                    'select[name="RoomCount"]', 'input[name="SquareMeter"]',
                    'select[name="BuildingAge"]', 'select[name="Floor"]',
                    'select[name="Heating"]', 'select[name="BathCount"]',
                    'select[name="BalconyCount"]', 'input[name="Price"]'];
        }
        else if (currentStep === 3)
            rules = ['input[name="Name"]', 'input[name="Surname"]',
                'input[name="Phone"]', 'input[name="Email"]',
                'input[name="City"]'];

        let ok = true;

        for (const rule of rules) {
            if (rule.startsWith("radio:")) {
                const name = rule.split(":")[1];
                const checked = form.querySelector(`input[name="${name}"]:checked`);
                if (!checked) ok = false;
                continue;
            }

            const el = form.querySelector(rule);
            if (el && el.offsetParent !== null) {
                if (!(el.value || "").trim()) ok = false;
            }
        }

        if (!ok) {
            for (const rule of rules) {
                if (!rule.startsWith("radio:")) {
                    const el = form.querySelector(rule);
                    if (el && !(el.value || "").trim()) markInvalid(el);
                }
            }
        }

        return ok;
    }

    window.AllFormsHandler.validation = { markInvalid, clearInvalid, shake, validateStep };

})();
