// wwwroot/js/panel-assign-task.js
document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("assignTaskModal");
    const partnerList = document.getElementById("partnerList");
    const formList = document.getElementById("formList");
    const formEl = document.getElementById("assignTaskForm");
    const btnAssign = document.getElementById("btnAssignTask");

    // Modal açıldığında partner ve form verilerini çek
    modal.addEventListener("show.bs.modal", async () => {
        // İş Ortakları
        partnerList.innerHTML = "<div class='text-muted'>Yükleniyor...</div>";
        try {
            const pRes = await fetch("/Panel/Assignments/GetPartners");
            const partners = await pRes.json();
            if (!partners.length) {
                partnerList.innerHTML = "<div class='text-danger small'>Hiç iş ortağı bulunamadı.</div>";
            } else {
                partnerList.innerHTML = "";
                partners.forEach(p => {
                    const item = document.createElement("label");
                    item.className = "list-group-item";
                    item.innerHTML = `
            <input class="form-check-input me-1" type="checkbox" name="PartnerIds" value="${p.Id}">
            <strong>${p.FullName}</strong> <br>
            <small class="text-muted">${p.Email ?? ''} • ${p.Province ?? ''} ${p.District ?? ''}</small>
          `;
                    partnerList.appendChild(item);
                });
            }
        } catch (err) {
            partnerList.innerHTML = "<div class='text-danger small'>Partner verisi alınamadı.</div>";
        }

        // Formlar
        formList.innerHTML = "<div class='text-muted'>Yükleniyor...</div>";
        try {
            const fRes = await fetch("/Panel/Assignments/GetForms");
            const forms = await fRes.json();
            if (!forms.length) {
                formList.innerHTML = "<div class='text-danger small'>Hiç form bulunamadı.</div>";
            } else {
                formList.innerHTML = "";
                forms.forEach(f => {
                    const item = document.createElement("label");
                    item.className = "list-group-item";
                    item.innerHTML = `
            <input class="form-check-input me-1" type="radio" name="FormId" value="${f.Id}">
            <strong>Form #${f.Id}</strong> • ${f.Category ?? '-'}
            <small class="text-muted">(${f.FormType ?? ''} / ${f.Status ?? ''})</small>
          `;
                    formList.appendChild(item);
                });
            }
        } catch (err) {
            formList.innerHTML = "<div class='text-danger small'>Form verisi alınamadı.</div>";
        }
    });

    // Kaydet butonu
    btnAssign.addEventListener("click", async () => {
        const selectedPartners = [...formEl.querySelectorAll('input[name="PartnerIds"]:checked')].map(p => p.value);
        const selectedForm = formEl.querySelector('input[name="FormId"]:checked')?.value;
        const title = formEl.querySelector('input[name="Title"]').value.trim();
        const description = formEl.querySelector('input[name="Description"]').value.trim();

        if (!title) { alert("Görev başlığı giriniz."); return; }
        if (!selectedPartners.length || !selectedForm) { alert("Lütfen en az bir iş ortağı ve bir form seçiniz."); return; }

        try {
            for (const partnerId of selectedPartners) {
                const fd = new FormData();
                fd.append("Title", title);
                fd.append("Description", description);
                fd.append("PartnerId", partnerId);
                fd.append("FormId", selectedForm);

                const res = await fetch("/Panel/Assignments/AssignTask", {
                    method: "POST",
                    body: fd
                });

                let resp;
                try { resp = await res.json(); } catch { resp = null; }

                if (!res.ok || !resp?.success) {
                    alert("❌ " + (resp?.message ?? "Görev atanamadı."));
                    return;
                }
            }

            alert("✅ Görev(ler) başarıyla atandı.");
            (bootstrap.Modal.getInstance(modal) || new bootstrap.Modal(modal)).hide();
            formEl.reset();
        } catch (err) {
            alert("❌ Sunucu hatası. Lütfen tekrar deneyin.");
        }
    });
});
