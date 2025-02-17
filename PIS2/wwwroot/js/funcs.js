function filterTable() {
    let input = document.getElementById("searchBox").value.toLowerCase();
    if (!input) {
        console.warn("searchBox not found!");
        return;
    }
    let rows = document.querySelectorAll("#dataTable tbody tr");
    
    rows.forEach(row => {
        let text = row.textContent.toLowerCase();
        row.style.display = text.includes(input) ? "" : "none";
    });
}
// Attach event listener when the DOM is fully loaded
document.addEventListener("DOMContentLoaded", function () {
    let input = document.getElementById("searchBox");
    if (input) {
        input.addEventListener("keyup", filterTable);
    } else {
        console.warn("searchBox not bound!");
    }
});