$('body').on('click', '.btn_modifyUser', function () {
    var tmpId = $(this).data("id");
    $('#id_modifyUser').val(tmpId);
});