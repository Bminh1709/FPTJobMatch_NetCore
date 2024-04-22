// Display job
$('body').on('click', '.view_cvFormSubmitted', function () {

    $('#applicant_response').val('');

    var applicantId = $(this).data("id");
    var jobId = $('#job_id').val();
    var curApplicantId = $('#applicant_id').val();

    if (applicantId != curApplicantId) {
        $.ajax({
            url: '/Employer/Applicant/GetCV',
            type: 'GET',
            data: { applicantId: applicantId, jobId: jobId },
            success: function (rs) {
                console.log(rs)
                if (rs.applicant != null) {
                    console.log(rs.applicant)
                    $('#applicant_id').attr('value', rs.applicant.id);
                    $('#CV_id').attr('value', rs.applicant.cvId);
                    $('#applicant_name').attr('value', rs.applicant.name);
                    $('#applicant_phone').attr('value', rs.applicant.phoneNumber);
                    $('#applicant_email').attr('value', rs.applicant.email);
                    $('#applicant_response').val(rs.applicant.responseMessage);
                    $('#applicant_cv').attr('href', '/filecv/' + rs.applicant.fileCV);
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


// Submit form
$('#responseMessage_form').on('submit', function (e) {
    var responseMessage = $('#applicant_response').val();

    if (!responseMessage.trim()) {
        // Prevent form submission if responseMessage is empty
        e.preventDefault();

        // Display error message
        $('#responseMessage_errorMessage').text('Please enter a message before submitting');
    }
});