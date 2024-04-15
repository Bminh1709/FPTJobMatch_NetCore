const apply_cvFormSubmit = $('#apply_cvFormSubmit');
const cvFormSubmit = $('#cvFormSubmit');
const close_cvFormSubmit = $('#close_cvFormSubmit');

apply_cvFormSubmit.on('click', function () {
    cvFormSubmit.removeClass('hidden');
});

close_cvFormSubmit.on('click', function () {
    cvFormSubmit.addClass('hidden');
});

