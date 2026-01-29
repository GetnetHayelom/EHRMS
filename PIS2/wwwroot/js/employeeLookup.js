
console.log("Function invoked for employeeId:");
/**
 * Fetches employee data using jQuery AJAX based on the Employee ID.
 * The data is retrieved from the Razor Page handler OnGetEmployeeLookup.
 * @param {string} employeeId - The ID of the employee to look up.
 * 
 */
function fetchEmployeeData(employeeId, empName, comp, dep) {
    const $status = $('#statusMessage');
    $status.addClass('d-none').removeClass('alert-success alert-danger');

   
    if (!employeeId) {
        $status.text('Enter Employee ID to load.')
            .removeClass('d-none alert-success')
            .addClass('alert-danger');
        console.log("No employee Id provided");
        return;
    }

    console.log("Calling API");

    $.getJSON(`/api/core/EmployeeLookup/${employeeId}`, function (data) {

        console.log("API response:", data);
        
        if (data.isFound) {
            // Loop through each property in the JSON object
            $.each(data, function (key, value) {
                let targetId = key; // default mapping

                if (key === 'employeeName' && empName) {
                    targetId = empName;   // e.g. employeeName2
                }
                if (key === 'company' && comp) {
                    targetId = comp;
                }
                if (key === 'department' && dep) {
                    targetId = dep;
                }

                let ctrl = $("#" + targetId);

                if (ctrl.length) {
                    console.log("Setting:", targetId, "→ value:", value);
                    ctrl.val(value ?? "null");
                } else {
                    console.warn("No element found for:", targetId);
                    return { success: false };
                }
            });
            $status.text('Employee data loaded successfully.')
                .removeClass('d-none alert-danger')
                .addClass('alert-success');
        } else {
            $status.text(`Employee ID ${employeeId} not found or no active job placement.`)
                .removeClass('d-none alert-success')
                .addClass('alert-danger');
        }
    }).fail(function () {
        $status.text('Error fetching employee data.')
            .removeClass('d-none alert-success')
            .addClass('alert-danger');
    });
}
