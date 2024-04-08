// Status Form (User)
const statusForm = $('#statusForm');
const close_statusForm = $('#close_statusForm');
const cancel_statusForm = $('#cancel_statusForm');
const open_statusForm = $('#open_statusForm');

open_statusForm.on('click', function () {
    statusForm.removeClass('hidden');
});

close_statusForm.on('click', function () {
    statusForm.addClass('hidden');
});
cancel_statusForm.on('click', function () {
    statusForm.addClass('hidden');
})


// Creating Employer Form (User)
const employerFormCreated = $('#employerFormCreated');
const close_employerFormCreated = $('#employerFormCreated');
const open_employerFormCreated = $('#open_employerFormCreated');

open_employerFormCreated.on('click', function () {
    employerFormCreated.removeClass('hidden');
});
close_employerFormCreated.on('click', function () {
    employerFormCreated.addClass('hidden');
});


// Converting Btn (Category)
$("#btn_VerifiedCat").click(function () {
    $("#UnverifiedCat").hide();
    $("#VerifiedCat").show();
    $(this).addClass('active');
    $('#btn_UnverifiedCat').removeClass('active');
});

$("#btn_UnverifiedCat").click(function () {
    $("#VerifiedCat").hide();
    $("#UnverifiedCat").show();
    $(this).addClass('active');
    $('#btn_VerifiedCat').removeClass('active');
});


// Accept Form (Category)
const acceptForm = $('#acceptForm');
const open_acceptForm = $('#open_acceptForm');
const cancel_acceptForm = $('#cancel_acceptForm');

open_acceptForm.on('click', function () {
    acceptForm.removeClass('hidden');
});
cancel_acceptForm.on('click', function () {
    acceptForm.addClass('hidden');
})


// Delete Form (Category)
const deletionForm = $('#deletionForm');
const open_deletionForm = $('.open_deletionForm');
const cancel_deletionForm = $('#cancel_deletionForm');

open_deletionForm.on('click', function () {
    deletionForm.removeClass('hidden');
});
cancel_deletionForm.on('click', function () {
    deletionForm.addClass('hidden');
})