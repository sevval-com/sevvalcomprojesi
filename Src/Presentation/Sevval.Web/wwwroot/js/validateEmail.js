document.getElementById("email").addEventListener("input", function () {
    var emailInput = document.getElementById("email");
    var errorMessage = document.getElementById("error-message");

    // Email doğrulaması
    var emailValue = emailInput.value;

    if (!emailValue.includes("@")) {
        emailInput.classList.add("invalid"); // Kırmızı sınır ve arka plan
        errorMessage.textContent = "Geçersiz email adresi! Lütfen '@' sembolü ekleyin.";
    } else {
        emailInput.classList.remove("invalid");
        errorMessage.textContent = "";
    }
});
