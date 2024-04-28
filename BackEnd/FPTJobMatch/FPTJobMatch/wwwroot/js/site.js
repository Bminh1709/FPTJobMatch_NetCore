var mybutton = $("#backToTop-btn");

// When the user scrolls down 200px from the top of the document, show the button
$(window).on("scroll", function () {
    var mybutton = $("#backToTop-btn");
    if ($(this).scrollTop() > 200) {
        mybutton.css("display", "block");
    } else {
        mybutton.css("display", "none");
    }
});

// When the user clicks on the button, scroll to the top of the document
mybutton.on("click", function () {
    window.scrollTo(0, 0);
});

// Fade out pop up message
$('.popUpMessage').delay(2000).fadeOut(1000);


