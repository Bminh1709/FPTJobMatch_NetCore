// Display job
$('body').on('click', '.open_cvFormSubmitted', function () {

    var tmpId = $(this).data("id");
    var curCVId = $('#applicantCV_id').val();
    if (tmpId != curCVId) {
        $.ajax({
            url: '/JobSeeker/Profile/GetCV',
            type: 'GET',
            data: { cvId: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    console.log(rs.data)
                    $('#applicantCV_id').val(rs.data.id);
                    $('#applicantCV_response').text(rs.data.responseMessage);
                    $('#applicantCV_file').attr('href', '/filecv/' + rs.data.fileCV);
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

// Reset Password
var formSubmitted = false;
$('#btn_resetPassword').on('click', function (e) {
    e.preventDefault();

    if (formSubmitted) {
        var oldPassword = $('#old_resetPassword').val();
        var newPassword = $('#new_resetPassword').val();
        var confirmNewPassword = $('#confirm_resetPassword').val();

        // Check if any of the fields are empty
        if (oldPassword === '' || newPassword === '' || confirmNewPassword === '') {
            $('#message_resetPassword').text('Please fill in all fields');
            return;
        }

        // Check if new password and confirm new password match
        if (newPassword !== confirmNewPassword) {
            $('#message_resetPassword').text('New password and confirm new password do not match');
            return;
        }

        $.ajax({
            url: '/Access/ResetPassword',
            type: 'POST',
            data: {
                oldPassword: oldPassword,
                newPassword: newPassword,
                ConfirmNewPassword: confirmNewPassword
            },
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    $('#message_resetPassword').text(response.error);
                    // Enable form inputs
                    toggleFormInputs(false);
                }
            },
            error: function (xhr, status, error) {
                // Display error message
                console.log(xhr);
                console.log(status);
                console.log(error);
                alert('Error: ' + error);
                // Enable form inputs
                toggleFormInputs(false);
            }
        });

    } else {
        // Disable form inputs
        toggleFormInputs(false);
        $('#old_resetPassword').trigger("focus");
        formSubmitted = true;
    }


});

// Function to enable/disable form inputs
function toggleFormInputs(disabled) {
    $('#form_resetPassword').find('input').prop('disabled', disabled);
}

// Event handler for the "Cancel" button
$('#btn_cancelResetPassword').on('click', function () {
    // Clear any error message
    $('#message_resetPassword').text('');
    // Disable form inputs
    toggleFormInputs(true);
    formSubmitted = false
});


