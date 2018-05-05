
jQuery(document).ready(function () {

    //Function that triggers on login button
    jQuery(document).on('click', "#login", function () {
        
    });


    //Function that trigger on triange bottom icon on dashboard, will toggle logout div
    jQuery(document).on('click', '.triangle-bottom', function () {
        jQuery('.logout-div').toggle();
    });



});