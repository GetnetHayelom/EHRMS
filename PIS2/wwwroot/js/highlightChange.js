// highlight-history.js

document.addEventListener("DOMContentLoaded", () => {
    const tables = document.querySelectorAll(".historyTable");

    tables.forEach(table => {
        const rows = table.rows;

        for (let i = 2; i < rows.length; i++) {
            const currentRow = rows[i];
            const prevRow = rows[i - 1];

            for (let j = 0; j < currentRow.cells.length - 2; j++) {
                const currCell = currentRow.cells[j];
                const prevCell = prevRow.cells[j];

                if (currCell.textContent.trim() !== prevCell.textContent.trim()) {
                    currCell.classList.add("bg-warning");
                }
            }
        }
    });
});
