$('body').on('click', '.edit_helpCategory', function () {

    var tmpId = $(this).data("id");

    var curId = $('#id_type').val();

    if (tmpId != curId) {
        $.ajax({
            url: '/Admin/Help/GetHelpType',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    console.log(rs.data)
                    $('#id_type').val(tmpId);
                    $('#name_type').val(rs.data.name);
                    $('#description_type').val(rs.data.description);
                }
            },
            error: function (xhr, status, error) {
                // Display error message
                console.log(xhr);
                console.log(status);
                console.log(error);
                alert('Error: ' + error);
            }
        });
    }


});