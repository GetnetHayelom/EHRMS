
/**
 * Fetches employee data using jQuery AJAX based on the Employee ID.
 * The data is retrieved from the Razor Page handler OnGetEmployeeLookup.
 * @param {string} employeeId - The ID of the employee to look up.
 */
function fetchEmployeeData(employeeId) {
    const $status = $('#statusMessage');
    $status.addClass('d-none').removeClass('alert-success alert-danger');

    if (!employeeId) return;

    $.getJSON(`/api/core/EmployeeLookup/${employeeId}`, function (data) {
        function setIfExists(id, value) {
            const $el = $("#"+id);
            if ($el.length) $el.val(value || '');
        }
        
        if (data.isFound) {
            setIfExists("EmploymentID", data.employmentID);
            setIfExists("EmployeeName", data.employeeName);
            setIfExists("JobTitle", data.jobTitle);
            setIfExists("Department", data.department);
            setIfExists("Company", data.company);
            setIfExists("WorkLocation", data.workLocation);
            console.log("################");
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
