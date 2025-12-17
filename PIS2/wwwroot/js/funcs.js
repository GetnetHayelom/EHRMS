
function printDiv(divId) {
    var printWin = window.open(window.location.href, "_self");
    
    printWin.print();
    printWin.close();

}
//
//
//
//
//
//
//
//
//

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




function highlightDifferences() {
    const table = document.getElementById("historyTable");
    if (!table) return;

    const rows = table.rows;

    for (let i = 2; i < rows.length; i++) {
        const currentRow = rows[i];
        const prevRow = rows[i - 1];

        const columnsToCheck = currentRow.cells.length - 2;

        for (let j = 0; j < columnsToCheck; j++) {
            const currCell = currentRow.cells[j];
            const prevCell = prevRow.cells[j];

            if (currCell.textContent.trim() !== prevCell.textContent.trim()) {
                currCell.classList.add("bg-warning");
            }
        }
    }
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

// Filter by column
let filters = [];

function filterVisibleRows(inputId, columnIndex) {
    let filterInput = document.getElementById(inputId).value.toLowerCase().trim();
    let visibleRows = document.querySelectorAll("#dataTable tbody tr");

    // Store the filter value in the global array
    filters[columnIndex] = filterInput;
 
    visibleRows.forEach(row => {
        let isVisible = true; // Assume row should be shown

        filters.forEach((filterValue, index) => {
            if (filterValue) { // Ignore empty filters
                let cell = row.cells[index];
                let cellText = cell ? cell.textContent.toLowerCase().trim() : "";

                if (!cellText.includes(filterValue)) {
                    isVisible = false; // If any filter fails, hide the row
                }
            }
        });

        row.style.display = isVisible ? "" : "none";
    });

    updateRowCount("countId"); // Call this once after filtering
}

// Function to count total and visible rows
function updateRowCount(countId) {
    let totalRows = document.querySelectorAll("#dataTable tbody tr").length;
    let visibleRows = document.querySelectorAll("#dataTable tbody tr:not([style*='display: none'])").length;
    let countElement = document.getElementById(countId);
    //countElement.textContent = `Filter result= ${visibleRows}`;
}

// Run on page load to set initial row count
document.addEventListener("DOMContentLoaded", function () {
    updateRowCount("countId");
});

//sorting


function enableTableSorting(tableId) {
    let table = document.getElementById(tableId);
    if (!table) {
        console.error(`Table with ID "${tableId}" not found.`);
        return;
    }

    table.querySelectorAll("th").forEach((header, columnIndex) => {
        header.style.cursor = "pointer";
        header.addEventListener("click", function () {
            sortTable(table, columnIndex);
        });
    });
}

let sortStates = {}; // Store sorting state per table

function sortTable(table, columnIndex) {
    let tbody = table.querySelector("tbody");
    let rows = Array.from(tbody.rows);

    // Initialize sort state if not set
    if (!sortStates[table.id]) {
        sortStates[table.id] = { column: columnIndex, direction: 1 };
    }

    let sortState = sortStates[table.id];

    // Toggle sort direction if clicking the same column
    if (sortState.column === columnIndex) {
        sortState.direction *= -1;
    } else {
        sortState.column = columnIndex;
        sortState.direction = 1;
    }

    // Sort rows
    rows.sort((rowA, rowB) => {
        let cellA = rowA.cells[columnIndex].innerText.trim();
        let cellB = rowB.cells[columnIndex].innerText.trim();

        // Handle numeric sorting
        if (!isNaN(Date.parse(cellA)) && !isNaN(Date.parse(cellB))) {
            return (new Date(cellA) - new Date(cellB)) * sortState.direction;
        } else if (!isNaN(cellA) && !isNaN(cellB)) {
            return (parseFloat(cellA) - parseFloat(cellB)) * sortState.direction;
        }

        // String sorting (case insensitive)
        return cellA.localeCompare(cellB) * sortState.direction;
    });

    // Clear and re-add sorted rows
    tbody.innerHTML = "";
    rows.forEach(row => tbody.appendChild(row));

    // Update sorting icons
    updateSortingIcons(table, columnIndex, sortState.direction);
}

function updateSortingIcons(table, columnIndex, direction) {
    table.querySelectorAll("th").forEach((th, index) => {
        let icon = th.querySelector("span.sort-icon");
        if (!icon) {
            icon = document.createElement("span");
            icon.classList.add("sort-icon");
            th.appendChild(icon);
        }
        icon.innerText = index === columnIndex ? (direction === 1 ? " 🔼" : " 🔽") : "";
    });
}
//
//
//
//
//EXPORT TO EXCEL
function exportTableToExcel(tableID, filename = 'export.xlsx') {
    const table = document.getElementById(tableID);
    if (!table) {
        console.error("Table not found: " + tableID);
        return;
    }

    // Convert table to workbook
    const workbook = XLSX.utils.table_to_book(table, { sheet: "Sheet1" });
    const worksheet = workbook.Sheets["Sheet1"];

    // ----- AUTO COLUMN WIDTH -----
    const sheetData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
    let colWidths = [];

    sheetData.forEach(row => {
        row.forEach((cell, colIndex) => {
            const cellValue = cell ? cell.toString() : "";
            const width = cellValue.length + 2; // +2 for padding
            colWidths[colIndex] = Math.max(colWidths[colIndex] || 10, width);
        });
    });

    worksheet['!cols'] = colWidths.map(w => ({ wch: w }));

    // ----- HEADER FORMATTING -----
    const range = XLSX.utils.decode_range(worksheet['!ref']); // get the worksheet range
    for (let C = range.s.c; C <= range.e.c; ++C) {
        const cellAddress = XLSX.utils.encode_cell({ r: 0, c: C }); // first row = header
        if (!worksheet[cellAddress]) continue;

        worksheet[cellAddress].s = {
            font: { bold: true, color: { rgb: "FFFFFF" } },
            fill: { fgColor: { rgb: "4F81BD" } }, // header background color
            alignment: { horizontal: "center", vertical: "center" }
        };
    }

    // Export Excel file
    XLSX.writeFile(workbook, filename, { bookType: 'xlsx', cellStyles: true });
}


// Match typed text to datalist and set hidden input
/**
 * Maps a visible input value to a hidden ID based on a datalist match.
 * @param {string} hiddenId - The ID of the hidden input to store the ID.
 
 */
// Match typed text to datalist and set hidden input
function selector(input, hiddenId) {
    const value = input.value;
    const datalist = input.getAttribute('list');
    const options = document.querySelectorAll(`#${datalist} option`);
    let found = false;
    options.forEach(opt => {
        if (opt.value === value) {
            document.getElementById(hiddenId).value = opt.dataset.id;
            found = true;
        }
    });
    if (!found) {
        document.getElementById(hiddenId).value = '';
    }
}

