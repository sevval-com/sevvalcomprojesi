window.AllFormsHandler = window.AllFormsHandler || {};

(function () {

    const form = document.getElementById("all-forms-beyan-form");
    if (!form) return;

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

    window.AllFormsHandler.config = {
        form, formTypeMap,
        steps, indicators,
        activeFormType,
        filteredSteps,
        currentStep,
        selectedCategory
    };

})();
