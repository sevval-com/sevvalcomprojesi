// === SSH 3 Aşamalı Form === //

document.addEventListener("DOMContentLoaded", function () {
    const steps = document.querySelectorAll(".form-step");
    const nextBtns = document.querySelectorAll(".btn-next");
    const prevBtns = document.querySelectorAll(".btn-prev");
    const stepIndicators = document.querySelectorAll(".wizard-steps .step");
    const progressBar = document.getElementById("wizardProgress");

    let currentStep = 0;

    // === Fonksiyon: Adım geçişlerini güncelle ===
    function updateFormSteps() {
        // Aktif adımı göster / gizle
        steps.forEach((step, index) => {
            step.classList.toggle("active", index === currentStep);
            step.style.display = index === currentStep ? "block" : "none";
        });

        // Step başlıklarını güncelle
        stepIndicators.forEach((circle, index) => {
            circle.classList.toggle("active", index <= currentStep);
        });

        // Progress bar animasyonu
        const progress = ((currentStep + 1) / steps.length) * 100;
        progressBar.style.width = `${progress}%`;
    }

    // === İLERİ butonları ===
    nextBtns.forEach(btn => {
        btn.addEventListener("click", () => {
            if (currentStep < steps.length - 1) {
                currentStep++;
                updateFormSteps();
            }
        });
    });

    // === GERİ butonları ===
    prevBtns.forEach(btn => {
        btn.addEventListener("click", () => {
            if (currentStep > 0) {
                currentStep--;
                updateFormSteps();
            }
        });
    });

    // Sayfa yüklendiğinde ilk adımı aktif yap
    updateFormSteps();
});
