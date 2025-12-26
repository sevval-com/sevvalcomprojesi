document.addEventListener("DOMContentLoaded", () => {

    const btnFilter = document.getElementById("btnFilter");
    const btnReset = document.getElementById("btnReset");
    const btnExportPDF = document.getElementById("btnExportPDF");
    const pageSizeSelect = document.getElementById("pageSizeSelect");
    const tableBody = document.querySelector("#formsTable tbody");
    const totalRecordsBox = document.getElementById("totalRecords");
    const paginationContainer = document.getElementById("pagination");

    const safe = (val) => val ?? "-";

    let fullData = [];
    let currentPage = 1;
    let pageSize = parseInt(pageSizeSelect?.value || 10);

    async function loadFiltered() {
        const params = new URLSearchParams({
            name: filterName.value.trim(),
            surname: filterSurname.value.trim(),
            email: filterEmail.value.trim(),
            phone: filterPhone.value.trim(),
            province: filterProvince.value.trim(),
            district: filterDistrict.value.trim(),
            category: filterCategory.value.trim(),
            formType: filterFormType.value.trim(),
            minPrice: filterMinPrice.value.trim(),
            maxPrice: filterMaxPrice.value.trim(),
            startDate: filterStartDate.value,
            endDate: filterEndDate.value
        });

        const res = await fetch(`${window.location.origin}/Panel/Forms/Filter?${params}`);
        fullData = await res.json();
        currentPage = 1;

        renderTable();
        renderPagination();
    }

    function renderTable() {
        tableBody.innerHTML = "";

        if (!fullData.length) {
            totalRecordsBox.textContent = "0 kayıt bulundu";
            return tableBody.innerHTML = `
                <tr><td colspan="10" class="text-muted text-center py-4">Hiç kayıt yok.</td></tr>`;
        }

        totalRecordsBox.textContent = `${fullData.length} kayıt bulundu`;

        const start = (currentPage - 1) * pageSize;
        const items = fullData.slice(start, start + pageSize);

        // tablonun her satırını oluşturduğun yerde:
        items.forEach(f => {
            const row = document.createElement("tr");
            row.innerHTML = `
            <td data-label="ID">${safe(f.Id)}</td>
            <td data-label="Ad Soyad">${safe(f.Name)} ${safe(f.Surname)}</td>
            <td data-label="TC">${safe(f.Tc)}</td>
            <td data-label="E-posta">${safe(f.Email)}</td>
            <td data-label="Telefon">${safe(f.Phone)}</td>
            <td data-label="İl / İlçe">${safe(f.Province)} / ${safe(f.District)}</td>
           <!-- <td data-label="Kategori">${safe(f.Category)}</td> -->
            <td data-label="Nitelik Durumu">${safe(f.NitelikDurumu)}</td>
            <td data-label="Tapu Durumu">${safe(f.TapuDurumu)}</td>
           <!-- <td data-label="Yevmiye Tarihi">${f.YevmiyeTarihi ? new Date(f.YevmiyeTarihi).toLocaleDateString("tr-TR") : "-"}</td> -->
            <td data-label="Tapu Fotoğrafı" class="text-center">
                ${f.TapuFotografi
                            ? `<button class="btn btn-sm btn-outline-info tapu-btn" data-img="${f.TapuFotografi}">
                         <i class="bi bi-image"></i> Görüntüle
                       </button>`
                            : "-"}
            </td>
            <td data-label="Oluşturulma">${f.CreatedAt ? new Date(f.CreatedAt).toLocaleString("tr-TR") : "-"}</td>
            <td data-label="İşlemler" class="text-center">
                <button class="btn btn-sm btn-outline-primary me-1 edit-btn"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-outline-danger me-1 delete-btn"><i class="bi bi-trash"></i></button>
                <button class="btn btn-sm btn-outline-secondary pdf-btn"><i class="bi bi-filetype-pdf"></i></button>
            </td>`;

            const pdfButton = row.querySelector(".pdf-btn");
            if (pdfButton) {
                pdfButton.addEventListener("click", (e) => {
                    e.stopPropagation();
                    generatePDF(f);
                });
            }

            // Tapu button
            const tapuBtn = row.querySelector(".tapu-btn");
            if (tapuBtn) {
                tapuBtn.addEventListener("click", (e) => {
                    e.stopPropagation();
                    const imgSrc = tapuBtn.getAttribute("data-img");
                    const imgElement = document.getElementById("tapuImagePreview");
                    imgElement.src = imgSrc;
                    new bootstrap.Modal(document.getElementById("tapuImageModal")).show();
                });
            }

            // Modal detay 
            row.addEventListener("click", (e) => {
                if (e.target.closest(".edit-btn,.delete-btn,.pdf-btn")) return;
                document.getElementById("formDetailBody").innerHTML = `
                <p><strong>Ad Soyad:</strong> ${safe(f.Name)} ${safe(f.Surname)}</p>
                <p><strong>TC:</strong> ${safe(f.Tc)}</p>
                <p><strong>E-posta:</strong> ${safe(f.Email)}</p>
                <p><strong>Telefon:</strong> ${safe(f.Phone)}</p>
                <p><strong>İl / İlçe:</strong> ${safe(f.Province)} / ${safe(f.District)}</p>
                <p><strong>Kategori:</strong> ${safe(f.Category)}</p>
                <p><strong>Nitelik Durumu:</strong> ${safe(f.NitelikDurumu)}</p>
                <p><strong>Tapu Durumu:</strong> ${safe(f.TapuDurumu)}</p>
                <p><strong>Yevmiye Tarihi:</strong> ${f.YevmiyeTarihi ? new Date(f.YevmiyeTarihi).toLocaleDateString("tr-TR") : "-"}</p>
                <p><strong>Tapu Fotoğrafı:</strong> ${f.TapuFotografi
                                    ? `<a href="${f.TapuFotografi}" target="_blank" class="text-primary text-decoration-underline">Görüntüle</a>`
                                    : "-"
                                }</p>                
                <p><strong>Oluşturulma:</strong> ${f.CreatedAt ? new Date(f.CreatedAt).toLocaleString("tr-TR") : "-"}</p>`;
                new bootstrap.Modal(document.getElementById("formDetailModal")).show();
            });

            tableBody.appendChild(row);
        });
    }

    function renderPagination() {
        const totalPages = Math.ceil(fullData.length / pageSize);
        if (totalPages <= 1) return paginationContainer.innerHTML = "";

        let html = `
        <button onclick="goToPage(1)" class="btn btn-sm btn-outline-primary">«</button>
        <button onclick="goToPage(${currentPage - 1})" class="btn btn-sm btn-outline-primary"><</button>`;

        for (let i = 1; i <= totalPages; i++) {
            html += `<button onclick="goToPage(${i})" class="btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'} me-1">${i}</button>`;
        }

        html += `
        <button onclick="goToPage(${currentPage + 1})" class="btn btn-sm btn-outline-primary">></button>
        <button onclick="goToPage(${totalPages})" class="btn btn-sm btn-outline-primary">»</button>`;

        paginationContainer.innerHTML = html;
    }

    window.goToPage = (page) => {
        if (page < 1) return;
        const totalPages = Math.ceil(fullData.length / pageSize);
        if (page > totalPages) return;
        currentPage = page;
        renderTable();
        renderPagination();
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    //function generatePDF(f) {
    //    const { jsPDF } = window.jspdf;
    //    const doc = new jsPDF({ orientation: "portrait", unit: "mm", format: "a4" });

    //    // ✅ Türkçe karakter düzeltici
    //    const fixTR = (text) =>
    //        text?.toString()
    //            .replace(/İ/g, "I")
    //            .replace(/ı/g, "i")
    //            .replace(/Ğ/g, "G")
    //            .replace(/ğ/g, "g")
    //            .replace(/Ş/g, "S")
    //            .replace(/ş/g, "s")
    //            .replace(/Ö/g, "O")
    //            .replace(/ö/g, "o")
    //            .replace(/Ü/g, "U")
    //            .replace(/ü/g, "u")
    //            .replace(/Ç/g, "C")
    //            .replace(/ç/g, "c") || "-";

    //    doc.setFont("Poppins", "normal");

    //    const title = fixTR(f.FormName) || "Beyan Formu";
    //    doc.setFontSize(16);
    //    doc.text(title, 14, 15);

    //    doc.setFontSize(11);
    //    doc.text(`Form No: ${fixTR(f.Id)}`, 14, 25);
    //    doc.text(
    //        f.CreatedAt
    //            ? `Olusturulma Tarihi: ${new Date(f.CreatedAt).toLocaleString("tr-TR")}`
    //            : "Olusturulma Tarihi: -",
    //        14,
    //        32
    //    );

    //    let commonFields = [
    //        ["Ad Soyad", `${fixTR(f.Name)} ${fixTR(f.Surname)}`],
    //        ["E-posta", fixTR(f.Email)],
    //        ["Telefon", fixTR(f.Phone)],
    //        ["Il", fixTR(f.Province)],
    //        ["Ilce", fixTR(f.District)],
    //        ["Kategori", fixTR(f.Category)],
    //        ["Durum", fixTR(f.Status)],
    //        [
    //            "Fiyat",
    //            `${Number(f.Price ?? f.PriceArsa ?? 0).toLocaleString("tr-TR")} TL`,
    //        ]
    //    ];

    //    let type1Fields = [
    //        ["Oda Sayisi", fixTR(f.RoomCount)],
    //        ["Metrekare", fixTR(f.SquareMeter)],
    //        ["Bina Yasi", fixTR(f.BuildingAge)],
    //        ["Kat", fixTR(f.Floor)],
    //        ["Isinma", fixTR(f.Heating)],
    //        ["Banyo", fixTR(f.BathCount)],
    //        ["Balkon", fixTR(f.BalconyCount)]
    //    ];

    //    let type2Fields = [
    //        ["Ada", fixTR(f.Ada)],
    //        ["Parsel", fixTR(f.Parsel)],
    //        ["Meyil Durumu", fixTR(f.MeyilDurumu)],
    //        ["Yol Durumu", fixTR(f.YolDurumu)],
    //        ["Yerlesim Uzaklik", fixTR(f.YerlesimUzaklik)],
    //        ["Imar Durumu", fixTR(f.ImarDurumu)],
    //        ["Metrekare (Arsa)", fixTR(f.SquareMeterArsa)]
    //    ];

    //    let finalBody = [
    //        ...commonFields,
    //        ...(f.FormType?.toLowerCase() === "type1" ? type1Fields : type2Fields)
    //    ];

    //    doc.autoTable({
    //        startY: 40,
    //        head: [["Alan", "Bilgi"]],
    //        body: finalBody,
    //        styles: { halign: "left", font: "Poppins", fontSize: 10 },
    //        headStyles: {
    //            fillColor: [0, 86, 179],
    //            textColor: 255,
    //            fontStyle: "bold",
    //            font: "Poppins"
    //        },
    //        alternateRowStyles: { fillColor: [245, 245, 245] }
    //    });

    //    doc.save(`${fixTR(f.FormName)}_${fixTR(f.Id)}.pdf`);
    //}      

    function generatePDF(f) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ orientation: "portrait", unit: "mm", format: "a4" });           

        // Turkish character ASCII
        const fixTR = (text) =>
            text?.toString()
                .replace(/İ/g, "I")
                .replace(/ı/g, "i")
                .replace(/Ğ/g, "G")
                .replace(/ğ/g, "g")
                .replace(/Ü/g, "U")
                .replace(/ü/g, "u")
                .replace(/Ş/g, "S")
                .replace(/ş/g, "s")
                .replace(/Ö/g, "O")
                .replace(/ö/g, "o")
                .replace(/Ç/g, "C")
                .replace(/ç/g, "c")
                .trim() || "-";

        doc.setFont("Poppins", "normal"); 
        const pageWidth = doc.internal.pageSize.getWidth();

        // === Logo ===
        const logoPath = "/images/sevval_logo.webp";
        doc.addImage(logoPath, "JPEG", 15, 10, 30, 20);

        // === Head ===
        doc.setFontSize(16);
        doc.text(fixTR("TALEBINIZ ALINMISTIR"), pageWidth / 2, 25, { align: "center" });

        // === Explanation ===
        doc.setFontSize(11);
        doc.text(
            fixTR("Sevval.com sistemine ilettiginiz form basariyla alinmistir.\n\nAsagida talebinize iliskin detaylar yer almaktadir."),
            pageWidth / 2,
            35,
            { align: "center", maxWidth: 170 }
        );

        let y = 55;
        doc.setFontSize(11);

        // === Form Data ===
        const fields = [
            ["Ad Soyad", `${fixTR(f.Name)} ${fixTR(f.Surname)}`],
            ["E-posta", fixTR(f.Email)],
            ["Telefon", fixTR(f.Phone)],
            ["Il / Ilce", `${fixTR(f.Province)} / ${fixTR(f.District)}`],
            ["Kategori", fixTR(f.Category)],
            ["TC", fixTR(f.Tc)],
            ["Nitelik Durumu", fixTR(f.NitelikDurumu)],
            ["Tapu Durumu", fixTR(f.TapuDurumu)],
            ["Yevmiye Tarihi", f.YevmiyeTarihi ? new Date(f.YevmiyeTarihi).toLocaleDateString("tr-TR") : "-"],
            ["Tapu Fotografi", f.TapuFotografi ? fixTR(f.TapuFotografi) : "-"],
            ...(f.FormType?.toLowerCase() === "type1"
                ? [
                    //["Oda Sayisi", fixTR(f.RoomCount)],
                    ["Metrekare", fixTR(f.SquareMeter)],
                    ["Bina Yasi", fixTR(f.BuildingAge)],
                    ["Kat", fixTR(f.Floor)],
                    //["Isinma", fixTR(f.Heating)],
                    //["Banyo", fixTR(f.BathCount)],
                    //["Balkon", fixTR(f.BalconyCount)],
                ]
                : [
                    ["Ada", fixTR(f.Ada)],
                    ["Parsel", fixTR(f.Parsel)],
                   /* ["Meyil Durumu", fixTR(f.MeyilDurumu)],*/
                    //["Yol Durumu", fixTR(f.YolDurumu)],
                    //["Yerlesim Uzaklik", fixTR(f.YerlesimUzaklik)],
                    //["Imar Durumu", fixTR(f.ImarDurumu)],
                    ["Metrekare (Arsa)", fixTR(f.SquareMeterArsa)],
                ])
        ];

        fields.forEach(([label, value]) => {
            doc.setFont("Poppins", "bold");
            doc.text(fixTR(`${label}:`), 20, y);
            doc.setFont("Poppins", "normal");
            doc.text(fixTR(value), 60, y, { maxWidth: 120 });
            y += 8;
            if (y > 260) {
                doc.addPage();
                doc.setFont("Poppins", "normal");
                y = 30;
            }
        });

        y += 12;

        // === Message ===
        const messageYStart = y;
        doc.setFillColor(0, 123, 255);
        doc.rect(15, messageYStart - 10, pageWidth - 30, 40, "F");
        doc.setTextColor(255, 255, 255);

        const message = [
            fixTR("Talebiniz en kisa surede ilgili departmanimiza iletilecektir."),
            fixTR("Degerlendirme surecinde sizinle iletisime gecilecektir."),
            fixTR("Bu surecte Sevval.com hesabinizdan durumunuzu takip edebilirsiniz."),
            fixTR("Bizi tercih ettiginiz icin tesekkur ederiz.")
        ];

        doc.setFont("Poppins", "normal");
        doc.setFontSize(11);
        message.forEach((line) => {
            doc.text(line, 20, y, { maxWidth: 170 });
            y += 7;
        });

        y += 12;

        // === Signature ===
        doc.setFont("Poppins", "bold");
        doc.text(fixTR("Saygilarimizla,"), 20, y);
        y += 7;
        doc.text(fixTR("Sevval.com Ekibi"), 20, y);

        // === Footer ===
        doc.setFont("Poppins", "normal");
        doc.setDrawColor(180);
        doc.line(15, 285, pageWidth - 15, 285);
        doc.setFontSize(9);
        doc.setTextColor(120);
        doc.text(fixTR(`Olusturulma: ${new Date(f.CreatedAt).toLocaleString("tr-TR")}`), 15, 292);
        doc.text(fixTR("© Sevval.com"), pageWidth - 40, 292);

        doc.save(`${fixTR(f.FormName)}_${fixTR(f.Id)}.pdf`);
    }

    // EVENTLER 
    pageSizeSelect.addEventListener("change", () => {
        pageSize = parseInt(pageSizeSelect.value);
        currentPage = 1;
        renderTable();
        renderPagination();
    });

    btnFilter.addEventListener("click", e => { e.preventDefault(); loadFiltered(); });
    btnReset.addEventListener("click", e => {
        e.preventDefault();
        document.querySelectorAll(".card input,.card select").forEach(el => el.value = "");
        loadFiltered();
    });

    btnExportPDF?.addEventListener("click", () => {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ orientation: "landscape" });
        const fixTR = (text) =>
            text?.toString()
                .replace(/İ/g, "I")
                .replace(/ı/g, "i")
                .replace(/Ğ/g, "G")
                .replace(/ğ/g, "g")
                .replace(/Ü/g, "U")
                .replace(/ü/g, "u")
                .replace(/Ş/g, "S")
                .replace(/ş/g, "s")
                .replace(/Ö/g, "O")
                .replace(/ö/g, "o")
                .replace(/Ç/g, "C")
                .replace(/ç/g, "c")
                .trim() || "-";

        doc.setFont("Poppins", "normal");
        doc.setFontSize(14);
        doc.text(fixTR("TÜM FORMLAR LİSTESİ"), 14, 15);

        doc.autoTable({
            html: "#formsTable",
            startY: 25,
            styles: { font: "Poppins", fontSize: 9, cellPadding: 2 },
            headStyles: {
                fillColor: [0, 86, 179],
                textColor: 255,
                fontStyle: "bold",
                font: "Poppins"
            },
            bodyStyles: {
                font: "Poppins",
                textColor: [20, 20, 20]
            },
            didParseCell: (data) => {
                if (typeof data.cell.text[0] === "string") {
                    data.cell.text[0] = fixTR(data.cell.text[0]);
                }
            }
        });

        doc.save(fixTR("Tum_Formlar.pdf"));
    });

    loadFiltered();
});
