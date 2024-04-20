// Show Password
$('#showPassword').on("change", function () {
    const isChecked = $(this).prop('checked');
    const inputType = isChecked ? 'text' : 'password';
    $('#Password, #PasswordConfirm').each(function () {
        $(this).attr('type', inputType);
    });
});


