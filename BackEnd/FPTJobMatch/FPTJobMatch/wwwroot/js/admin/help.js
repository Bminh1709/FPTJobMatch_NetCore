// Delete Form (Help/Index)
const deletionForm = $('#deletionForm');
const cancel_deletionForm = $('#cancel_deletionForm');
const open_deletionForm = $('.open_deletionForm');

open_deletionForm.on('click', function () {
    window.scrollTo(0, 0);
    deletionForm.removeClass('hidden');
});

cancel_deletionForm.on('click', function () {
    deletionForm.addClass('hidden');
});


// Add - Update Form
const form_helpCategory = $('#form_helpCategory');
const close_helpCategory = $('#close_helpCategory');
const edit_helpCategory = $('.edit_helpCategory');
const create_helpCategory = $('#create_helpCategory');

create_helpCategory.on('click', function () {
    window.scrollTo(0, 0);
    form_helpCategory.removeClass('hidden');
});

edit_helpCategory.on('click', function () {
    window.scrollTo(0, 0);
    form_helpCategory.removeClass('hidden');
});

close_helpCategory.on('click', function () {
    form_helpCategory.addClass('hidden');
})


// Sorting dropdown click event handler
$('.sort-option').on('click', function () {
    var sortValue = $(this).data('value');
    $('#selectedSortType').val(sortValue);
    $('#dropdown_sort').text(sortValue !== '' ? sortValue : 'Sorting');
});


// Empty the id when adding (Form adding type)
$('#create_helpCategory').on('click', function () {
    $('#id_type').val('');
    $('#name_type').val('');
    $('#description_type').val('');
});


// Get HelpType's id on Form Delete
$('.open_deletionForm').on('click', function () {
    var tmpId = $(this).data('id');
    $('#id_typeDelete').val(tmpId);
});


// Filter
$('.sort-option').on('click', function () {
    var sortValue = $(this).data('value');
    $('#selectedSortType').val(sortValue);
    $('#dropdown_sort').text(sortValue !== '' ? sortValue : 'Sorting');
});



// Delete Form (Help/Article)
const deletionForm_article = $('#deletionForm_article');
const cancel_deletionForm_article = $('#cancel_deletionForm_article');
const open_deletionForm_article = $('#open_deletionForm_article');

open_deletionForm_article.on('click', function () {
    window.scrollTo(0, 0);
    deletionForm_article.removeClass('hidden');
});

cancel_deletionForm_article.on('click', function () {
    deletionForm_article.addClass('hidden');
});