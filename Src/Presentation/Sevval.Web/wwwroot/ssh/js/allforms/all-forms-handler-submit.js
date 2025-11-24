window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const { form } = window.AllFormsHandler.config;
    const { validateStep, markInvalid, clearInvalid, shake } = window.AllFormsHandler.validation;
    const { showStep } = window.AllFormsHandler.steps;

    async function submitDynamicForm(e) {
        e.preventDefault();
        console.log("✅ Submit tetiklendi");

        let hasError = false;

        // ✅ Zorunlu Alan Kontrolü (Step4 tüm required)
        if (!validateStep()) {
            hasError = true;
        }

        // ✅ Ad min 3 harf
        const nameEl = form.querySelector('input[name="Name"]');
        if (nameEl) {
            const nameVal = nameEl.value.trim();
            if (nameVal.length < 3) {
                markInvalid(nameEl, "En az 3 harf olmalı");
                shake(nameEl);
                hasError = true;
            } else {
                clearInvalid(nameEl);
            }
        }

        // ✅ Email format
        const emailEl = form.querySelector('input[name="Email"]');
        if (emailEl) {
            const email = emailEl.value.trim();
            const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!pattern.test(email)) {
                markInvalid(emailEl, "Geçerli e-posta giriniz");
                shake(emailEl);
                hasError = true;
            } else {
                clearInvalid(emailEl);
            }
        }

        // ✅ Telefon format
        const phoneEl = form.querySelector('input[name="Phone"]');
        if (phoneEl) {
            const phone = phoneEl.value.trim();
            const phonePattern = /^0\d{3}\s\d{3}\s\d{2}\s\d{2}$/;
            if (!phonePattern.test(phone)) {
                markInvalid(phoneEl, "Format 0555 555 55 55 olmalı");
                shake(phoneEl);
                hasError = true;
            } else {
                clearInvalid(phoneEl);
            }
        }

        if (hasError) {
            console.warn("❌ Submit Durduruldu → Hatalar var");
            return;
        }

        // ✅ DTO Hazırlama
        const toNum = v => v ? Number(String(v).replace(/\./g, "").replace(",", ".")) : null;
        const body = Object.fromEntries(new FormData(form));

        body.Province = document.getElementById("Province")?.value ||
            document.getElementById("ProvinceArsa")?.value || "";

        body.District = document.getElementById("District")?.value ||
            document.getElementById("DistrictArsa")?.value || "";

        body.Neighborhood = document.getElementById("Neighborhood")?.value ||
            document.getElementById("NeighborhoodArsa")?.value || "";

        body.FormKey = "Beyan";
        body.FormName = "Beyan Vermek İstiyorum";
        body.CreatedAt = new Date().toLocaleString("tr-TR", { timeZone: "Europe/Istanbul" });

        ["SquareMeter", "SquareMeterArsa", "Price", "PriceArsa"]
            .forEach(k => body[k] = toNum(body[k]));

        const formEl = document.getElementById("all-forms-beyan-form");
        body.RawPayload = Object.fromEntries(new FormData(formEl));

        // ✅ 1. API → Başarılı ise DUR
        try {
            const apiRes = await fetch("/api/forms", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(body)
            });

            if (apiRes.ok) {
                console.log("✅ API başarıyla kaydetti");
                showStep(window.AllFormsHandler.config.filteredSteps.length - 1);
                return;
            }

            console.warn("⚠️ API başarısız → MVC’ye geçiliyor...");
        } catch (err) {
            console.warn("⚠️ API erişilemedi → MVC fallback", err);
        }

        // ✅ 2. MVC FALLBACK → HER KOŞULDA KAYDEDİNCE DEVAM
        try {
            const mvcRes = await fetch("/DynamicForm/Create", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body)
            });

            if (!mvcRes.ok) {
                alert("⚠️ Form kaydedilemedi — tekrar deneyin.");
                return;
            }

            console.log("✅ MVC ile kayıt tamam!");
            showStep(window.AllFormsHandler.config.filteredSteps.length - 1);
        } catch (err) {
            console.error("🛑 MVC Submit Error:", err);
            alert("⚠️ Sunucu hatası");
        }
    }

    form.addEventListener("submit", submitDynamicForm);

    window.AllFormsHandler.submit = { submitDynamicForm };

})();
