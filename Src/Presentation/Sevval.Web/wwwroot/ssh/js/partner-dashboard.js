document.addEventListener("DOMContentLoaded", async () => {
    const tableBody = document.querySelector("#formsTable tbody");

    async function loadForms() {
        const res = await fetch("/Panel/Forms/Filter");
        const data = await res.json();

        tableBody.innerHTML = data.map(f => `
            <tr>
                <td>${f.id}</td>
                <td>${f.name} ${f.surname}</td>
                <td>${f.email}</td>
                <td>${f.phone || '-'}</td>
                <td>${f.province || ''} / ${f.district || ''}</td>
                <td>${f.category || '-'}</td>
                <td>${f.status}</td>
                <td>${f.price?.toLocaleString() || '-'}</td>
                <td>${new Date(f.createdAt).toLocaleDateString()}</td>
                <td><button class="btn btn-sm btn-outline-brown">Detay</button></td>
            </tr>
        `).join("");
    }

    loadForms();
});
