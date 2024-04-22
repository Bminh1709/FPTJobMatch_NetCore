
$("#manageCvBtn").on('click', function () {
    $("#Profile_section").hide();
    $("#ManageCV_section").show();
    $(this).addClass('active');
    $('#profileBtn').removeClass('active');
});

$("#profileBtn").on('click', function () {
    $("#ManageCV_section").hide();
    $("#Profile_section").show();
    $(this).addClass('active');
    $('#manageCvBtn').removeClass('active');
});


// Information
var isSubmittedInfo = false;
$('#form_jobseekerInfo').on('submit', function (e) {
    if (isSubmittedInfo) {
        return;
        isSubmittedInfo = false
    } else {
        e.preventDefault();
        $(this).find('input, select, textarea').removeAttr('disabled');
        $('#jobseeker_about').trigger("focus");
        window.scrollTo(0, 0);
        isSubmittedInfo = true;
    }
});
$('#btn_cancelInfo').on('click', function () {
    $('#form_jobseekerInfo').find('input, select, textarea').attr('disabled', 'disabled');
    isSubmittedInfo = false; 
});


// Password Reset



// View Repsonse from Employer
const open_cvFormSubmitted = $('.open_cvFormSubmitted');
const cvFormSubmitted = $('#cvFormSubmitted');
const close_cvFormSubmitted = $('#close_cvFormSubmitted');

open_cvFormSubmitted.on('click', function () {
    cvFormSubmitted.removeClass('hidden');
});
close_cvFormSubmitted.on('click', function () {
    cvFormSubmitted.addClass('hidden');
})