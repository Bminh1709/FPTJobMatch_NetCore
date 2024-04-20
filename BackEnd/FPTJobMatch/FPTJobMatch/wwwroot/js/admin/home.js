// Status Form (User)
const statusForm = $('.statusForm');
const close_statusForm = $('.close_statusForm');
const cancel_statusForm = $('.cancel_statusForm');
const open_statusForm = $('.open_statusForm');

open_statusForm.on('click', function () {
    window.scrollTo(0, 0);
    statusForm.removeClass('hidden');
});

close_statusForm.on('click', function () {
    statusForm.addClass('hidden');
});
cancel_statusForm.on('click', function () {
    statusForm.addClass('hidden');
})


// Reset Password Form (User)
const resetPassForm = $('.resetPassForm');
const close_resetPassForm = $('.close_resetPassForm');
const cancel_resetPassForm = $('.cancel_resetPassForm');
const open_resetPassForm = $('.open_resetPassForm');

open_resetPassForm.on('click', function () {
    window.scrollTo(0, 0);
    resetPassForm.removeClass('hidden');
});

close_resetPassForm.on('click', function () {
    resetPassForm.addClass('hidden');
});
cancel_resetPassForm.on('click', function () {
    resetPassForm.addClass('hidden');
})


// Filter (Admin/)
function updateUserType(userType) {
    $('#filter_userType').val(userType);
    $('#form_filter').trigger("submit");
}
function updateSortType(sortType) {
    $('#filter_sortType').val(sortType);
    $('#form_filter').trigger("submit");
}
$('#btn_submitFilter').on('click', function () {
    $('#filter_keyword').val($('#keywordInput').val());
});


// Creating Employer Form (User)
const employerFormCreated = $('#employerFormCreated');
const close_employerFormCreated = $('#close_employerFormCreated');
const open_employerFormCreated = $('#open_employerFormCreated');

open_employerFormCreated.on('click', function () {
    employerFormCreated.removeClass('hidden');
    window.scrollTo(0, 0);
});
close_employerFormCreated.on('click', function () {
    employerFormCreated.addClass('hidden');
});


// Get User's ID to Status Form
$('body').on('click', '.btn_modifyUser', function () {
    var tmpId = $(this).data("id");
    $('#id_modifyUser').val(tmpId);
});


// Get User's ID to Password Reset Form
$('body').on('click', '.btn_resetPass', function () {
    var tmpId = $(this).data("id");
    $('#id_resetPass').val(tmpId);
});
