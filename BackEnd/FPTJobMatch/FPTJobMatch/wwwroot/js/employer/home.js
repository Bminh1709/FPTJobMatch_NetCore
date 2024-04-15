// Jobs List page
const create_employerCVForm = $('#create_employerCVForm');
const edit_employerCVForm = $('.edit_employerCVForm');
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


const delete_employerCVFormDelete = $('.delete_employerCVFormDelete');
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

// Add New Category
$('body').on("change", "#categoryDropdown", function () {
    var newCategoryInput = $("#newCategoryInput");
    if ($(this).val() === "") {
        newCategoryInput.removeAttr("disabled");
        newCategoryInput.focus();
    } else {
        newCategoryInput.attr("disabled", "disabled");
    }
});

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


// Show Password
$('#showPassword').on("change", function () {
    const isChecked = $(this).prop('checked');
    const inputType = isChecked ? 'text' : 'password';
    $('#Password, #PasswordConfirm').each(function () {
        $(this).attr('type', inputType);
    });
});


// Validate Category for Form
$('#form_createJob').on("submit", function (event) {
    var categoryId = $('#categoryDropdown').val();
    var newCategoryName = $('#newCategoryInput').val();
    var deadlineDate = $('#deadlineInput').val();

    // Convert the deadline string to a JavaScript Date object
    var deadline = new Date(deadlineDate);
    // Get today's date
    var today = new Date();

    // Check if the deadline is not greater than today's date
    if (deadline <= today) {
        $('#deadline_errorMessage').show();
        event.preventDefault();
    } else {
        // Clear the error message
        $('#deadline_errorMessage').hide();

        // Check if category selection or entry is empty
        if ((categoryId === null || categoryId === "") && newCategoryName === "") {
            // Display an error message for category selection
            $('#category_errorMessage').text("Please select a category or enter a new category.");
            event.preventDefault(); 
        } else {
            // Clear the error message for the category
            $('#category_errorMessage').text("");
            // If both are selected or typed
            $(this).off('submit');
        }
    }
});
