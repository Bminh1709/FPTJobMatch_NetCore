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
const acceptForm = $('.acceptForm');
const open_acceptForm = $('.open_acceptForm');
const cancel_acceptForm = $('.cancel_acceptForm');

open_acceptForm.on('click', function () {
    window.scrollTo(0, 0);
    acceptForm.removeClass('hidden');
});
cancel_acceptForm.on('click', function () {
    acceptForm.addClass('hidden');
})


// Delete Form (Category)
const deletionForm = $('.deletionForm');
const open_deletionForm = $('.open_deletionForm');
const cancel_deletionForm = $('.cancel_deletionForm');

open_deletionForm.on('click', function () {
    window.scrollTo(0, 0);
    deletionForm.removeClass('hidden');
});
cancel_deletionForm.on('click', function () {
    deletionForm.addClass('hidden');
})


// Get Category's ID to Form Accept
$('body').on('click', '.open_acceptForm', function () {
    var tmpId = $(this).data("id");
    $('#id_approveCategory').val(tmpId);
});

// Get Category's ID to Form Delete
$('body').on('click', '.open_deletionForm', function () {
    var tmpId = $(this).data("id");
    $('#id_deleteCategory').val(tmpId);
});