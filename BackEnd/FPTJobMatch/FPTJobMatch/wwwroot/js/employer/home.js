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


// Validate Category for Form
$('#form_createJob').submit(function (event) {
    var categoryId = $('#categoryDropdown').val();
    var newCategoryName = $('#newCategoryInput').val();

    if ((categoryId === null || categoryId === "") && newCategoryName === "") {
        // Display an error message
        $('#category_errorMessage').text("Please select a category or enter a new category.");
        event.preventDefault();
    } else {
        $('#category_errorMessage').text("");
        // If both are selected or typed
        $(this).off('submit');
    }
});
