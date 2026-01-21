
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
            document.getElementById(hiddenId).dispatchEvent(new Event('change', { bubbles: true }));
        }
    });
    if (!found) {
        document.getElementById(hiddenId).value = '';
    }
}

/**
 * Fetches Job Step based on a Job Grade ID and populates a target input.
 * @param {string} jobStepInput - The jQuery selector for the input to be updated (e.g., '#jobStepID').
 */
function getJobSteps(jobGrade, jobStepInput) {
    var $target = $(jobStepInput);

    $target.empty().append('<option value="">-- Loading --</option>');

    var gradeID = (typeof jobGrade === 'object') ? jobGrade.value : jobGrade;

    if (gradeID) {
        
        $.getJSON('/api/core/GetJobSteps/' + gradeID, function (data) {
            $target.empty().append('<option value="">-- Select Step --</option>');

            // 3. Loop through the returned data and append to the target
            $.each(data, function (i, item) {
                $target.append($('<option>', {
                    value: item.jobStepID, 
                    text: item.jobStepName  
                })).trigger('change');
            });
        }).fail(function () {
            $target.empty().append('<option value="">Error loading data</option>');
        });
    } else {
        // 4. Reset if no jobID is provided
        $target.empty().append('<option value="">-- Select Job Grade First --</option>');
    }
}

/**
 * Fetches Job Salary based on a Job Step ID and populates a target input.
 * @param {string} jobSalaryInput - The jQuery selector for the input to be updated (e.g., '#jobSalary').
 */
function getJobSalary(jobStep, jobSalaryInput) {

    var $target = $(jobSalaryInput);

    var stepID = (typeof jobStep === 'object') ? jobStep.value : jobStep;

    if (stepID) {
        $.getJSON('/api/core/GetJobSalary/' + stepID, function (data) {
            $target.val(data);
        }).fail(function () {
            console.error("Could not fetch salary for ID: " + stepID);
        });
    }
}

/**
 * Fetches Job Grades based on a Job ID and selects the returned value in the target dropdown.
 * @param {string} jobGradeInput - The jQuery selector for the dropdown to be updated (e.g., '#jobGradeID').
 */
function getJobGrade(job, jobGradeInput) {
    var $target = $(jobGradeInput);

    var jobID = (typeof job === 'object') ? job.value : job;

    if (jobID) {
        $.getJSON('/api/core/GetJobGrade/' + jobID, function (data) {
            
            $target.val(String(data)).trigger('change');
        }).fail(function () {
            
            console.error("Could not fetch Job Grade for ID: " + jobID);
        });
    }
}


    /**
     * Reusable function to load departments based on a Company ID.
     * @param {string} sourceSelector - The ID of the Company dropdown.
     * @param {string} targetSelector - The ID of the Department dropdown.
     */
    function loadDepartments(sourceSelector = '#filter-company', targetSelector = '#filter-department') {
        const $source = $(sourceSelector);
        const $target = $(targetSelector);
        const companyID = $source.val();

        // 1. Visual Feedback: Show loading and disable to prevent race conditions
        $target.empty()
            .append('<option value="">-- Loading --</option>')
            .prop('disabled', true);

        if (companyID) {
            $.getJSON('/api/core/DepartmentsByCompany/' + companyID)
                .done(function (data) {
                    // 2. Clear and populate
                    $target.empty().append('<option value="" selected>All</option>');

                    $.each(data, function (i, dept) {
                        $target.append($('<option>', {
                            value: dept.departmentID,
                            text: dept.departmentName
                        }));
                    });
                })
                .fail(function () {
                    // 3. Error Handling
                    $target.empty().append('<option value="">Error loading data</option>');
                })
                .always(function () {
                    // 4. Re-enable control
                    $target.prop('disabled', false);
                });
        } else {
            // 5. Reset state if no company is selected
            $target.empty()
                .append('<option value="">All</option>')
                .val("")
                .prop('disabled', false)
                .trigger('change');
        }
    }

    // Wiring up the event listener using the defaults
    $('#filter-company').on('change', function () {
        loadDepartments();
    });

    // To call it manually for different IDs elsewhere:
    // loadDepartments('#manual-company-id', '#manual-dept-id');



function bindUserOnlyChange(controlId, callback, ...controls) {

    let userAction = false;
    const selector = `#${controlId}`;

    // Detect real user interaction
    $(document).on('mousedown keydown touchstart', selector, function () {
        userAction = true;
    });

    // Fire change ONLY if caused by user
    $(document).on('change', selector, function () {
        if (!userAction) return;

        callback(this, controls);

        // reset after firing
        userAction = false;
    });
}

function showToast(type, message) {
    const toastEl = document.getElementById('actionToast');
    const titleEl = document.getElementById('toastTitle');
    const bodyEl = document.getElementById('toastBody');

    const isSuccess = type === 'Success';

    toastEl.classList.remove('bg-success', 'bg-danger');
    toastEl.querySelector('.toast-header')
        .classList.remove('bg-success', 'bg-danger');

    toastEl.classList.add(isSuccess ? 'bg-success' : 'bg-danger');
    toastEl.querySelector('.toast-header')
        .classList.add(isSuccess ? 'bg-success' : 'bg-danger');

    titleEl.textContent = `${isSuccess ? '✔' : '⚠'} ${type}`;
    bodyEl.textContent = message;

    new bootstrap.Toast(toastEl).show();
}


