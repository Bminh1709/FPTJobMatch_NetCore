$(document).ready(function () {
    var mybutton = $("#backToTop-btn");

    // When the user scrolls down 200px from the top of the document, show the button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            mybutton.css("display", "block");
        } else {
            mybutton.css("display", "none");
        }
    });

    // When the user clicks on the button, scroll to the top of the document
    mybutton.click(function () {
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });
});

