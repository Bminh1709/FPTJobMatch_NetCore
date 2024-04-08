// Jobs List page
const create_employerCVForm = $('#create_employerCVForm');
const edit_employerCVForm = $('#edit_employerCVForm');
const employerCVForm = $('#employerCVForm');
const close_employerCVForm = $('#close_employerCVForm');

create_employerCVForm.on('click', function () {
    employerCVForm.removeClass('hidden');
});
edit_employerCVForm.on('click', function () {
    employerCVForm.removeClass('hidden');
});
close_employerCVForm.on('click', function () {
    employerCVForm.addClass('hidden');
})


const delete_employerCVFormDelete = $('#delete_employerCVFormDelete');
const employerCVFormDelete = $('#employerCVFormDelete');
const close_employerCVFormDelete = $('#close_employerCVFormDelete');
const cancel_employerCVFormDelete = $('#cancel_employerCVFormDelete');

delete_employerCVFormDelete.on('click', function () {
    employerCVFormDelete.removeClass('hidden');
});
close_employerCVFormDelete.on('click', function () {
    employerCVFormDelete.addClass('hidden');
})
cancel_employerCVFormDelete.on('click', function () {
    employerCVFormDelete.addClass('hidden');
})


// Applicants page
const view_cvFormSubmitted = $('.view_cvFormSubmitted');
const cvFormSubmitted = $('#cvFormSubmitted');
const close_cvFormSubmitted = $('#close_cvFormSubmitted');

view_cvFormSubmitted.on('click', function () {
    cvFormSubmitted.removeClass('hidden');
});

close_cvFormSubmitted.on('click', function () {
    cvFormSubmitted.addClass('hidden');
})


// Profile page
$("#companyBtn").click(function () {
    $("#Profile_section").hide();
    $("#Company_section").show();
    $(this).addClass('active');
    $('#profileBtn').removeClass('active');
});

$("#profileBtn").click(function () {
    $("#Company_section").hide();
    $("#Profile_section").show();
    $(this).addClass('active');
    $('#companyBtn').removeClass('active');
});

