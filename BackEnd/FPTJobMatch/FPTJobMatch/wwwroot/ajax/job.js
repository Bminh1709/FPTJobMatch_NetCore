// Display job
$('body').on('click', '.job_item', function () {
    $('#noJobClick_message').hide();

    var tmpId = $(this).data("id");
    var curJob = $('#job_id').val();
    if (curJob != tmpId) {
        // Remove active class from all job_item elements
        $('.job_item').removeClass('active');

        // Add active class to the clicked job_item
        $(this).addClass('active');


        $.ajax({
            url: '/Jobs/GetJob',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    console.log(rs.data)
                    $('#company_logo').attr('src', '/images/companyLogo/' + (rs.data.logo ?? 'noLogoCompany.jpg')).attr('alt', rs.data.title);
                    $('#job_id').attr('value', rs.data.id);
                    $('#job_title').text(rs.data.title);
                    $('#company_name_small').text(rs.data.company.name + ",");
                    $('#company_city').text(rs.data.company.city.name + " City");
                    $('#job_date').text(getTimeAgoString(rs.data.createdAt));
                    $('#job_address').text(rs.data.address);
                    $('#job_type').text(rs.data.jobType.name);
                    $('#job_deadline').text(rs.data.deadline);
                    $('#job_category').text(rs.data.category.name);
                    $('#job_description').text(rs.data.description);
                    $('#job_responsibilities').text(rs.data.responsibility);
                    $('#job_experience').text(rs.data.experience);
                    $('#job_additionalInfo').text(rs.data.additionalDetail);
                    $('#company_name').text(rs.data.company.name);
                    $('#company_size').text(rs.data.company.size);
                    $('#company_location').text(rs.data.company.city.name + " City");

                    // Show the #rightTab element
                    $('#jobDisplay_tab').show();
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

function getTimeAgoString(utcDateTimeString) {
    var createdAt = new Date(utcDateTimeString);
    var currentTime = new Date();

    var timeElapsed = currentTime - createdAt;
    var seconds = Math.floor(timeElapsed / 1000);
    var minutes = Math.floor(seconds / 60);
    var hours = Math.floor(minutes / 60);
    var days = Math.floor(hours / 24);

    if (seconds < 60) {
        return "Just now";
    } else if (minutes < 60) {
        return minutes + " minutes ago";
    } else if (hours < 24) {
        return hours + " hours ago";
    } else {
        return days + " days ago";
    }
}


// Submit CV for Job
$('body').on('click', '#submitCV_btn', function (e) {
    e.preventDefault();

    // Get the job ID
    var jobId = $('#job_id').val();

    // Get the file input element
    var fileInput = $('#fileCV_input')[0];

    // Check if a file is selected
    if (fileInput.files.length === 0) {
        $('#fileCV_message').text('Please choose a file');
        return; // Exit the function to prevent form submission
    }

    // Create a new FormData object
    var formData = new FormData();

    // Append the job ID to the FormData object
    formData.append('jobId', jobId);

    // Append the file to the FormData object
    formData.append('fileCV', fileInput.files[0]);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    // Send the FormData object via AJAX
    $.ajax({
        url: '/Jobs/SubmitCV',
        type: 'POST',
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Prevent jQuery from setting contentType
        success: function (response) {
            location.reload();
        },
        error: function (xhr, status, error) {
            // Handle error
        }
    });
});



