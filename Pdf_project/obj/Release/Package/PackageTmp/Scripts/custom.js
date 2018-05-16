
jQuery(document).ready(function () {

    //Function that triggers on login button
    jQuery(document).on('click', "#login", function () {
        //Sending data from login form to Home controller (Login ActionResult), that will check if data exists in db and return true or false
        jQuery.post("/home/login",

            //Passing username and password to Home controller
            {
                Username: jQuery('#username').val(),
                Password: jQuery('#password').val()
            },

            //Returned result from controller
            function (data) {

                //If result is True redirect to Dashboard, else show error message
                if (data.Result == 'True') {

                    //Set error message to invisible              
                    jQuery('.error-message').css('display', 'none');

                    //Set localstorage for current user
                    localStorage["zip"] = data.UserZip;
                    localStorage["serialno"] = data.UserSerialNo;

                    //Redirect to dashboard
                    window.location.href = '/dashboard?zip=' + data.UserZip + '&email=' + data.UserEmail + '&serialno=' + data.UserSerialNo;
                }
                else {
                    //Set error message visible
                    jQuery('.error-message').css('display', 'block');
                }




            });
    });


    //Function that trigger on triange bottom icon on dashboard, will toggle logout div
    jQuery(document).on('click', '.triangle-bottom', function () {
        jQuery('.logout-div').toggle();
    });

    //Function that will trigger when checkbox change state 
    jQuery('#agreementCheckbox').change(function () {

        //Get agreement button 
        var agreementButton = jQuery('#agreementButton');

        //If checkbox is checked remove default btn class and add btn-primaru class, enable button
        if (jQuery(this).is(":checked")) {

            

                //Remove disabled attribute 
                $('#agreementButton').prop("disabled", false);

               
            

        }

        //Else remove primary btn class and add btn-default class, disable button 
        else {

            //Add disabled attribute 
            $('#agreementButton').prop("disabled", true);

        }
    });

    //Function that will trigger on agreementButton click
    jQuery(document).on('click', '#agreementButton', function () {

        //Set loading screen 
        jQuery(".loader-over-page").css("display", "block");

        //Post data to controller action to create word and pdf
        jQuery.post("/dashboard/createdocumentsfromtemplate",
            //Passing parametres
            {
                Zip: localStorage["zip"],
                SerialNo: localStorage["serialno"]               
            },
            //Returned result from controller
            function (data) {
                console.log('Data:', data);

                //Remove loading screen 
                jQuery(".loader-over-page").css("display", "none");

                if (data.Result == "true") {

                    //Show download-preview section 
                    jQuery('.contract-section').css("display", "none");
                    jQuery('.download-preview-contract').css("display", "block");

                    //Set href values for download and preview 
                    jQuery('.download-pdf').attr("href", "/Dashboard/DownloadPdf?name=" + data.Name);
                    jQuery('.preview-pdf').attr("href", "Pdf/" + data.Name + ".pdf");

                    //Scroll to top
                    jQuery("html, body").animate({ scrollTop: 0 }, 0);


                    window.open("Pdf/" + data.Name + ".pdf", "_blank");
                }


            });

    });


});