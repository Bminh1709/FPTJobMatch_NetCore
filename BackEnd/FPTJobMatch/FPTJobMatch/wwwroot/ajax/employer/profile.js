// Reset Password
let isSubmittedReset = false;
$('#btn_resetPassword').on('click', function (e) {
    e.preventDefault();

    if (isSubmittedReset) {
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

    } else {
        $('#form_resetPassword').find('input').removeAttr('disabled');
        $('#old_resetPassword').trigger("focus");
        isSubmittedReset = true;
    }

});

$('#btn_cancelResetPassword').on('click', function () {
    isSubmittedReset = false;
    $('#form_resetPassword').find('input').prop('disabled', true);
});
