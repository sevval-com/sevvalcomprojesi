window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const { steps, indicators, form, formTypeMap } = window.AllFormsHandler.config;

    function rebuildSteps() {
        const cfg = window.AllFormsHandler.config;
        cfg.filteredSteps = steps.filter(s => s.dataset.form === "common" || s.dataset.form === cfg.activeFormType);
    }

    function showStep(index) {
        const cfg = window.AllFormsHandler.config;
        if (index < 0) index = 0;
        if (index >= cfg.filteredSteps.length) index = cfg.filteredSteps.length - 1;

        cfg.filteredSteps.forEach((s, i) => {
            s.style.display = i === index ? "block" : "none";
            s.classList.toggle("active", i === index);
        });

        indicators.forEach((ind, i) => {
            ind.classList.toggle("active", i === index);
            ind.classList.toggle("completed", i < index);
        });

        cfg.currentStep = index;
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    function setActiveFormType(type) {
        const cfg = window.AllFormsHandler.config;
        cfg.activeFormType = type;
        rebuildSteps();
        cfg.currentStep = 0;
        showStep(1);
        updateStepTitles();
    }

    function updateStepTitles() {
        const { selectedCategory } = window.AllFormsHandler.config;
        document.querySelectorAll('[data-dynamic-title]')
            .forEach(t => t.textContent = selectedCategory
                ? `Mülk Bilgileri (${selectedCategory})`
                : "Mülk Bilgileri");
    }

    window.AllFormsHandler.steps = { rebuildSteps, showStep, setActiveFormType, updateStepTitles };

})();
