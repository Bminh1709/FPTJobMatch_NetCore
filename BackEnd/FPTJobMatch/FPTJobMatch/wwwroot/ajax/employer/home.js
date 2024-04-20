
// Display job
$('body').on('click', '.job_item', function () {
    $('#noJobClick_message').hide();

    var tmpId = $(this).data("id"); // Job on left tab
    var curJob = $('#job_id').val(); // job is displaying
    if (curJob != tmpId) {
        // Remove active class from all job_item elements
        $('.job_item').removeClass('active');

        // Add active class to the clicked job_item
        $(this).addClass('active');


        $.ajax({
            url: '/Employer/Home/GetJob',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    console.log(rs.data)
                    $('#edit_employerCVFormId').attr('data-id', rs.data.job.id);
                    $('#delete_employerCVFormDeleteId').attr('data-id', rs.data.job.id);
                    $('#job_id').attr('value', rs.data.job.id);
                    $('#job_title').text(rs.data.job.title);
                    $('#applicantCV_new').text(rs.data.numOfNewCVs + " CVs New");
                    $('#applicantCV_total').text(rs.data.numOfCVs + " CVs Submitted");
                    $('#job_address').text(rs.data.job.address);
                    $('#job_type').text(rs.data.job.jobType.name);
                    $('#job_deadline').text(rs.data.job.deadline);
                    $('#job_category').text(rs.data.job.category.name);
                    $('#job_description').text(rs.data.job.description);
                    $('#job_responsibilities').text(rs.data.job.responsibility);
                    $('#job_experience').text(rs.data.job.experience);
                    $('#job_additionalInfo').text(rs.data.job.additionalDetail);


                    $('#viewApplicants_page').attr('href', '/Employer/Applicant/Index?jobId=' + rs.data.job.id);

                    // Show the #rightTab element
                    $('#jobDisplay_tab').show();
                }
            },
            error: function (xhr, status, error) {
                // Display error message
                console.log(xhr);
                console.log(status);
                console.log(error);
                alert('Error: ' + error);
            }
        });
    }

});


// Display job (Edit Form)
$('body').on('click', '.edit_employerCVForm', function () {

    var tmpId = $(this).data("id"); // Job is displaying

    $.ajax({
        url: '/Employer/Home/GetJob',
        type: 'GET',
        data: { id: tmpId },
        success: function (rs) {
            if (rs.data != null) {
                console.log(rs.data)
                $('#delete_employerCVFormDeleteId').attr('data-id', rs.data.job.id);
                $('#job_id').attr('value', rs.data.job.id);
                $('#job_title').text(rs.data.job.title);
                $('#applicantCV_new').text(rs.data.numOfNewCVs + " CVs New");
                $('#applicantCV_total').text(rs.data.numOfCVs + " CVs Submitted");
                $('#job_address').text(rs.data.job.address);
                $('#job_type').text(rs.data.job.jobType.name);
                $('#job_deadline').text(rs.data.job.deadline);
                $('#job_category').text(rs.data.job.category.name);
                $('#job_description').text(rs.data.job.description);
                $('#job_responsibilities').text(rs.data.job.responsibility);
                $('#job_experience').text(rs.data.job.experience);
                $('#job_additionalInfo').text(rs.data.job.additionalDetail);


                $('#viewApplicants_page').attr('href', '/Employer/Applicant/Index?jobId=' + rs.data.job.id);

                // Show the #rightTab element
                $('#jobDisplay_tab').show();
            }
        },
        error: function (xhr, status, error) {
            // Display error message
            console.log(xhr);
            console.log(status);
            console.log(error);
            alert('Error: ' + error);
        }
    });

});