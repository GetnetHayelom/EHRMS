$(document).ready(function () {
    $('#filter-jobClassID').change(function () {
        var $jobClassID = $(this).val();
        var $job = $('#jobTitleOptions');

        $job.empty().append('<option value="">-- Loading --</option>');

        if ($jobClassID) {
            $.getJSON('/api/core/JobsByJobClass/' + $jobClassID, function (data) {
                $job.empty().append('<option value="">All</option>');

                $.each(data, function (i, job) {
                    $job.append($('<option>', {
                        value: job.jobTitle,
                        text: job.jobTitle
                    }));
                });

                // Select "All" and trigger change event
                //$dept.val("0").trigger("change");
            });
        } else {
            $job.empty().append('<option value="">All</option>').val("0").trigger("change");
        }
    });
});
