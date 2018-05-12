﻿
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

                    //Redirect to dashboard
                    window.location.href = '/dashboard?zip=' + data.UserZip + '&email=' + data.UserEmail;
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
            
            if (agreementButton.hasClass('btn-default')) {

                //Remove disabled attribute 
                $('#agreementButton').prop("disabled", false);

                agreementButton.removeClass('btn-default');
                agreementButton.addClass('btn-primary');
            }

        }

        //Else remove primary btn class and add btn-default class, disable button 
        else {

            //Add disabled attribute 
            $('#agreementButton').prop("disabled", true);

            if (agreementButton.hasClass('btn-primary')) {
                agreementButton.removeClass('btn-primary');
                agreementButton.addClass('btn-default');
            }

        }
    });



});