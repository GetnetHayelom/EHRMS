$(document).ready(function () {
    $('#filter-company').change(function () {
        var companyID = $(this).val();
        var $dept = $('#filter-department');

        $dept.empty().append('<option value="">-- Loading --</option>');

        if (companyID) {
            $.getJSON('/api/core/DepartmentsByCompany/' + companyID, function (data) {
                $dept.empty().append('<option value="">All</option>');

                $.each(data, function (i, dept) {
                    $dept.append($('<option>', {
                        value: dept.departmentID,
                        text: dept.departmentName
                    }));
                });

                // Select "All" and trigger change event
                //$dept.val("0").trigger("change");
            });
        } else {
            $dept.empty().append('<option value="">All</option>').val("0").trigger("change");
        }
    });
});
