
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

        if ((jQuery("#contactEmail").val() && String(jQuery("#contactEmail").val()).trim() != "") && (jQuery("#contractUser").val() && String(jQuery("#contractUser").val()).trim() != "")) {

            //If contains error class remove it 
            if (jQuery("#contactEmail").hasClass('empty-email')) {
                jQuery("#contactEmail").removeClass('empty-email');
            }

            if (jQuery("#contractUser").hasClass('empty-email')) {
                jQuery("#contractUser").removeClass('empty-email');
            }

            //Set error message to none
            jQuery('.error-field-message').css("display", "none");


            //Set loading screen 
            jQuery(".loader-over-page").css("display", "block");

            //Post data to controller action to create word and pdf
            jQuery.post("/dashboard/createdocumentsfromtemplate",
                //Passing parametres
                {
                    
                    SerialNo: localStorage["serialno"],
                    Name1: jQuery("#name1").val(),
                    Name2: jQuery("#name2").val(),
                    Street: jQuery("#street").val(),
                    Zip: jQuery("#zip").val(),
                    UserZip: localStorage["zip"],
                    City: jQuery("#city").val(),
                    Country: jQuery("#country").val(),
                    Email: jQuery("#contactEmail").val(),
                    ContractUser: jQuery("#contractUser").val()

                },
                //Returned result from controller
                function (data) {
                    console.log('Data:', data);

                    //Remove loading screen 
                    jQuery(".loader-over-page").css("display", "none");

                    if (data.Result == "true") {

                        //Send data to web service
                        $.ajax({
                            url: "http://crm.hope.software/apiv1/customer/setDSVGO_ADV/" + localStorage["serialno"] + "/true",
                            type: "POST",
                            crossDomain: true,
                            
                            dataType: "json",
                            success: function (response) {
                                console.log(success)
                            },
                            error: function (xhr, status) {
                                console.log(xhr);
                            }
                        });

                        

                        

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
        }

        else {
            if (String(jQuery("#contactEmail").val()).trim() == "") {
                jQuery("#contactEmail").addClass("empty-email");
            }

            else if (jQuery("#contactEmail").hasClass("empty-email")) {
                jQuery("#contactEmail").removeClass("empty-email");
            }

            if (String(jQuery("#contractUser").val()).trim() == "") {
                jQuery("#contractUser").addClass("empty-email");
            }
            else if (jQuery("#contractUser").hasClass("empty-email")) {
                jQuery("#contractUser").removeClass("empty-email");
            }
            
            
            //Set error message to block
            jQuery('.error-field-message').css("display", "block");

            //Scroll to top
            //Scroll to top
            jQuery("html, body").animate({ scrollTop: 0 }, 500);
        }



        

    });


});