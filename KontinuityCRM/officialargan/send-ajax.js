$ = jQuery.noConflict();
$(document).ready(function(){
//    $.ajax({
//        url:'getuserdata.php',
//        method:'post',
//        dataType:'json',
//        contentType: "application/json;charset=utf-8",
//        async:'false',
//        success:function(res){
//            if(res.gotResponse){
//                $("#fields_fname").val(res.shipping_first_name);
//                $("#fields_lname").val(res.shipping_last_name);
//                $("#fields_address1").val(res.shipping_street_address);
//                $("#fields_address2").val(res.shipping_street_address2);
//                $("#fields_city").val(res.shipping_city);
//                $("#country").val(res.shipping_country);
//                $("#fields_state").val(res.shipping_state);
//                $("#fields_zip").val(res.shipping_postcode);
//                $("#fields_phone").val(res.customers_telephone);
//                $("#fields_email").val(res.email_address);
//            }
//        },
//        error:function(err){
//            console.log(err);
//        }
//    });
     
    $("#country").live("change",function(){
        var geo = $(this).val().toUpperCase();
        if(geo=='UK'){
            geo='GB';
        }
        getStates(geo);
    });
    $(".country-select").live("click",function(){
        var country = $(this).find(".country").val().toLowerCase();
        window.onbeforeunload = UnPopIt;
       window.location.href=document.location.protocol+"//"+document.location.host+"/"+country+"/"+document.location.search; 
    });
});

