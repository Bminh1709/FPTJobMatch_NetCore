// Jobs List page
const create_employerCVForm = $('#create_employerCVForm');
const employerCVForm = $('#employerCVForm');
const close_employerCVForm = $('#close_employerCVForm');

create_employerCVForm.on('click', function () {
    window.scrollTo(0, 0);
    employerCVForm.removeClass('hidden');
});
close_employerCVForm.on('click', function () {
    employerCVForm.addClass('hidden');
})

// Delete Job Form
const delete_employerCVFormDelete = $('.delete_employerCVFormDelete');
const employerCVFormDelete = $('#employerCVFormDelete');
const close_employerCVFormDelete = $('#close_employerCVFormDelete');
const cancel_employerCVFormDelete = $('#cancel_employerCVFormDelete');

delete_employerCVFormDelete.on('click', function () {
    window.scrollTo(0, 0);
    employerCVFormDelete.removeClass('hidden');
});
close_employerCVFormDelete.on('click', function () {
    employerCVFormDelete.addClass('hidden');
})
cancel_employerCVFormDelete.on('click', function () {
    employerCVFormDelete.addClass('hidden');
})

// Add New Category (Employer/Home/CreateJob)
$('body').on("change", "#categoryDropdown", function () {
    var newCategoryInput = $("#newCategoryInput");
    if ($(this).val() === "") {
        newCategoryInput.removeAttr("disabled");
        newCategoryInput.trigger("focus");
    } else {
        newCategoryInput.attr("disabled", "disabled");
    }
});

// Validate Category for Form (Employer/Home/CreateJob)
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
        window.scrollTo(0, 0);
        event.preventDefault();
    } else {
        // Clear the error message
        $('#deadline_errorMessage').hide();

        // Check if category selection or entry is empty
        if ((categoryId === null || categoryId === "") && newCategoryName === "") {
            // Display an error message for category selection
            $('#category_errorMessage').text("Please select a category or enter a new category.");
            window.scrollTo(0, 0);
            event.preventDefault();
        } else {
            // Clear the error message for the category
            $('#category_errorMessage').text("");
            // If both are selected or typed
            $(this).off('submit');
        }
    }
});



// Get Job's ID to Delete Form
$('body').on('click', '.delete_employerCVFormDelete', function () {
    var tmpId = $(this).attr("data-id");
    $('#delete_employerId').val(tmpId);
});



