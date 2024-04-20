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


// Filter Candidates 
$('.status-option').on('click', function () {
    var statusValue = $(this).data('value');
    $('#selectedStatus').val(statusValue);
    $('#dropdown_status').text(statusValue !== '' ? statusValue : 'Status');
});

// Sorting dropdown click event handler
$('.sort-option').on('click', function () {
    var sortValue = $(this).data('value');
    $('#selectedSortType').val(sortValue);
    $('#dropdown_sort').text(sortValue !== '' ? sortValue : 'Sorting');
});





