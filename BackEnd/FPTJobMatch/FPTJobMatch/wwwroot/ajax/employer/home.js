
// Display job
$('body').on('click', '.job_item', function () {
    $('#noJobClick_message').hide();

    var tmpId = $(this).data("id");
    var curJob = $('#job_id').val();
    if (curJob != tmpId) {
        $.ajax({
            url: '/Employer/Home/GetJob',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    console.log(rs.data)
                    $('#job_id').attr('value', rs.data.job.id);
                    $('#job_title').text(rs.data.job.title);
                    $('#company_city').text(rs.data.job.company.city.name + " City");
                    $('#applicantCV_new').text(rs.data.numOfNewCVs + " CVs New");
                    $('#applicantCV_total').text(rs.data.numOfCVs + " CVs Submitted");
                    $('#job_address').text(rs.data.job.address);
                    $('#job_type').text(rs.data.job.jobType.name);
                    $('#job_category').text(rs.data.job.category.name);
                    $('#job_description').text(rs.data.job.description);
                    $('#job_responsibilities').text(rs.data.job.responsibility);
                    $('#job_experience').text(rs.data.job.experience);
                    $('#job_additionalInfo').text(rs.data.job.additionalDetail);
                    $('#company_name').text(rs.data.job.company.name);
                    $('#company_size').text(rs.data.job.company.size);
                    $('#company_location').text(rs.data.job.company.city.name + " City");

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