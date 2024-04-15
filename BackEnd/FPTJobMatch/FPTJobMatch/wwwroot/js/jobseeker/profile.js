$(document).ready(function () {
    $("#manageCvBtn").click(function () {
        $("#Profile_section").hide();
        $("#ManageCV_section").show();
        $(this).addClass('active');
        $('#profileBtn').removeClass('active');
    });

    $("#profileBtn").click(function () {
        $("#ManageCV_section").hide();
        $("#Profile_section").show();
        $(this).addClass('active');
        $('#manageCvBtn').removeClass('active');
    });
});


var formSubmitted = false;
$('#form_jobseekerInfo').on('submit', function (e) {
    e.preventDefault();

    if (!formSubmitted) {
        $(this).find('input, select, textarea').removeAttr('disabled');
        $('#jobseeker_about').focus();
        formSubmitted = true; // Set the flag to true 
    } else {
        var phoneNumber = $("#jobseeker_phone").val();
        if (phoneNumber != null) {

        }
        $(this).off('submit').submit();
    }
});

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