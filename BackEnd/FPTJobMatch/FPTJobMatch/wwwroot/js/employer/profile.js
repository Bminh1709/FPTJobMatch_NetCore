// Profile page
$("#companyBtn").on("click", function () {
    $("#Profile_section").hide();
    $("#Company_section").show();
    $(this).addClass('active');
    $('#profileBtn').removeClass('active');
});

$("#profileBtn").on("click", function () {
    $("#Company_section").hide();
    $("#Profile_section").show();
    $(this).addClass('active');
    $('#companyBtn').removeClass('active');
});


// Update Company
let isSubmittedComp = false;
$('#form_updateCompany').on('submit', function (e) {
    if (isSubmittedComp) {
        return; 
        isSubmittedComp = false
    } else {
        e.preventDefault();
        $(this).find('input, select, textarea').removeAttr('disabled');
        $('#about_updateCompany').trigger("focus");
        isSubmittedComp = true; 
    }
});

$('#btn_cancelUpdateCompany').on('click', function () {
    isSubmittedComp = false;
    $('#form_updateCompany').find('input, select, textarea').attr('disabled', true);
});

// Update User
let isSubmittedUser = false;
$('#form_updateUser').on('submit', function (e) {
    if (isSubmittedUser) {
        return;
        isSubmittedUser = false
    } else {
        e.preventDefault();
        $(this).find('input').removeAttr('disabled');
        $('#name_updateUser').trigger("focus");
        isSubmittedUser = true;
    }
});
$('#btn_cancelUpdateUser').on('click', function () {
    isSubmittedUser = false;
    $('#form_updateUser').find('input').attr('disabled', true);
});






