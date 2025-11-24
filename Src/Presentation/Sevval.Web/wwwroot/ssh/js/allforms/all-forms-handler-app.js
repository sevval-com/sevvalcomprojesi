window.AllFormsHandler = window.AllFormsHandler || {};

window.AllFormsHandler.initBeyanForm = () => {

    const cfg = window.AllFormsHandler.config;
    const steps = window.AllFormsHandler.steps;
    const validate = window.AllFormsHandler.validation.validateStep;

    if (!cfg.form) {
        console.error("FORM bulunamadı!");
        return;
    }

    // Step filtering + first view
    steps.rebuildSteps();
    cfg.steps.forEach(s => s.style.display = "none");
    steps.showStep(0);

    // First Next Button: Category → FormType
    const firstNext = cfg.form.querySelector(
        '.all-forms-step-section[data-step="1"] .all-forms-next-btn'
    );
    firstNext?.addEventListener("click", () => {
        if (!validate()) return;
        const cSel = cfg.form.querySelector('input[name="Category"]:checked');
        if (!cSel) return;
        cfg.selectedCategory = cSel.value;
        const type = cfg.formTypeMap[cSel.value];
        document.getElementById("SelectedCategory").value = cfg.selectedCategory;
        document.getElementById("FormType").value = type;
        steps.setActiveFormType(type);
    });

    // Next / Previous general
    cfg.form.querySelectorAll(".all-forms-next-btn")
        .forEach(btn =>
            btn.addEventListener("click", () => validate() && steps.showStep(cfg.currentStep + 1))
        );

    cfg.form.querySelectorAll(".all-forms-prev-btn")
        .forEach(btn => btn.addEventListener("click", () => steps.showStep(cfg.currentStep - 1)));
};
