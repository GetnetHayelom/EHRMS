//function printDiv(divId) {
//    // Get the content of the div
//    var content = document.getElementById(divId).innerHTML;
//    // Get the current date
//    var currentDate = new Date().toLocaleDateString('en-US', {
//        year: 'numeric', month: 'long', day: 'numeric'
//    });
//    // Open a new window for printing
//    var printWindow = window.open('', '', 'height=600,width=800');

//    // Write the HTML structure to the print window
//    printWindow.document.write('<html><head><title>PIS2</title>');

//    // You can include your own styles or link to external stylesheets

//    printWindow.document.write('<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />');
//    printWindow.document.write('<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />');
//    printWindow.document.write('<link rel="stylesheet" href="~/PIS2.styles.css" asp-append-version="true" />');

//    printWindow.document.write('</head><body>');
//    printWindow.document.write(`
//            <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; width: 100%;">
//                <!-- Left Logo -->
//                <div style="width: 15%; text-align: left;">
//                    <img src="/assets/mie_logo.png" alt="Logo" style="width: 100%; max-height: 80px;">
//                        <p>Form No. WF/HRM/11</p>
//                </div>

//                <!-- Centered Company Info -->
//                <div style="width: 70%; text-align: center;">
//                    <h3 style="margin: 0;">መስፍን ኢንዳስትሪያል ኢንጂነሪንግ ሓ/የተ/የግ/ኩባንያ</h3>
//                    <h2 style="margin: 0;">Mefin Industrial Engineering PLC</h2>
//                    <p style="margin: 0;">Mekelle, Ethiopia</p>
//                    <p style="margin: 0;">Phone: +123 456 789 | Email: contact@company.com</p>
//                </div>

//                <!-- Right Logo -->
//                <div style="width: 15%; text-align: right;">
//                    <img src="/assets/mie_logo.png" alt="Logo" style="width: 100%; max-height: 80px;">
//                    <p style="margin: 10px 0; font-weight: bold;">Date: ${currentDate}</p>
//                </div>
//            </div>
//            <hr>
//                `);


//    // Insert the content of the div into the print window
//    printWindow.document.write(content);



//    printWindow.document.close(); // Close the document for writing
//    //printWindow.focus(); // Focus the new window
//    //printWindow.print(); // Print the content
//    // Wait for the stylesheets to load before printing
//    printWindow.onload = function () {
//        printWindow.focus();
//        printWindow.print();
//    };
//}
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
    countElement.textContent = `Filter result= ${visibleRows}`;
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
