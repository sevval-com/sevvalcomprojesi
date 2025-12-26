document.addEventListener('DOMContentLoaded', () => {
    const map = [
        { controlId: 'IlanTuru', previewId: 'previewIlanTuru' },
        { controlId: 'Telefon', previewId: 'previewTelefon' },
        { controlId: 'IsimSoyad', previewId: 'previewIsimSoyad' },
        { controlId: 'Firma', previewId: 'previewFirma' }
    ];

    map.forEach(({ controlId, previewId }) => {
        const ctrl = document.getElementById(controlId);
        const prev = document.getElementById(previewId);

        ctrl.addEventListener('input', () => {
            let val = ctrl.value;

            if (controlId === 'Telefon') {
                // Sayılar dışındaki karakterleri temizle
                val = val.replace(/\D/g, '');
                // Başında 0 yoksa ekle
                if (!val.startsWith('0')) val = '0' + val;
                // XXXX XXX XX XX formatı uygula
                val = val.replace(/(\d{4})(\d{3})(\d{2})(\d{2}).*/, '$1 $2 $3 $4');
                ctrl.value = val;
            } else {
                // Harf ve boşluk dışını temizle
                val = val.replace(/[^A-Za-zÇÖĞÜŞİçöğüşı ]+/g, '');
                ctrl.value = val;
            }

            prev.textContent = val;
        });
    });
});