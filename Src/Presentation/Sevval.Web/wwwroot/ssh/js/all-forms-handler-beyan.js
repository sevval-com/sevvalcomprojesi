document.addEventListener("DOMContentLoaded", () => {
    if (!window.AllFormsHandler) {
        console.error("AllFormsHandler yüklenmedi!");
        return;
    }

    // Orijinal Main Init 
    window.AllFormsHandler.initBeyanForm();
});
