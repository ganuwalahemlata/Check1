

$(document).ready(function () {

    //$("#ss1 ins").click(function () {
    //    isSubscription();
    //});
    //$("#ss2 ins").click(function () {
    //    isShipping();
    //});
    //$("#ss3 ins").click(function () {
    //    loadBalancer();
    //});
    //$("#ss5 ins").click(function () {
    //    hasRedemption();
    //});
    //$("#ss6 ins").click(function () {
    //    hasProvider();
    //});
    //$("#BillType").change(function () {
    //    billType(this.value);
    //});
    //$("#ss7 ins").click(function () {        
    //    hasPostBackUrls();
    //});
    //$("#ss4 ins").click(function () {
    //    isSelfRecurring();
    //});

    $("#IsSubscription").change(function () {
        //isSubscription();

        var $subcriptionform = $("#subcriptionform");

        if ($("#IsSubscription").is(':checked')) {
            $subcriptionform.show();
            isSelfRecurring();
        }
        else {
            $subcriptionform.hide();
        }

        //if ($subcriptionform.is(":visible")) {

        //    $('html, body').animate({
        //        scrollTop: $subcriptionform.offset().top - $subcriptionform.height() + 200,

        //    }, { duration: 1000, queue: false });

        //}
        //else
        //{
        //    $('html, body').animate({
        //        scrollTop: $(window).scrollTop() - $("#opts").height(),

        //    }, { duration: 1000, queue: false });
        //}

        

        

    });

    //var elementPosTop = $('#opts').position().top;
    //$(window).scroll(function () {
    //    var wintop = $(window).scrollTop(), docheight = $(document).height(), winheight = $(window).height();
    //    //if top of element is in view

    //    //console.log("wintop: " + wintop);
    //    //console.log("docheight: " + docheight);
    //    //console.log("winheight: " + winheight);

    //    if (wintop > elementPosTop) {
    //        //always in view
    //        $('#opts').css({ "padding-top": wintop - elementPosTop });
    //    }
    //    else {
    //        //reset back to normal viewing
    //        $('#opts').css({ "padding-top": 0 });
    //    }
    //});

   
    $("#IsTaxable").change(function () {

        var $taxableform = $("#taxableform");
        if ($("#IsTaxable").is(':checked')) {
            $taxableform.show();
        }
        else {
            $taxableform.hide();
        }
    });

    $("#IsShippable").change(function () {

        var $shippingform = $("#shippingform");
        if ($("#IsShippable").is(':checked')) {
            $shippingform.show();
        }
        else {
            $shippingform.hide();
        }
        

        //if ($shippingform.is(":visible")) {

        //    $('html, body').animate({
        //        scrollTop: $shippingform.offset().top - $shippingform.height(),

        //    }, { duration: 1000, queue: false });

        //}
        //else {
        //    $('html, body').animate({
        //        scrollTop: $(window).scrollTop() - $("#opts").height(),

        //    }, { duration: 1000, queue: false });
        //}
       
    });
    $("#LoadBalancer").change(function () {
        loadBalancer();
    });
    $("#HasRedemption").change(function () {
        hasRedemption();
    });
    $("#Provider").change(function () {
        hasProvider();
    });
    $("#BillType").change(function () {
        billType(this.value);
    });
    $("#hasurls").change(function () {
        hasPostBackUrls();
    });
    $("#selfrecurring").change(function () {
        isSelfRecurring();
    });
    isTaxable();
    isSubscription();
    isShipping();
    loadBalancer();
    hasRedemption();
    hasProvider();
    hasPostBackUrls();
    billType($("#BillType").val()); // call to the function
});

function isSelfRecurring() {

    //var pdiv = $("#ss4 > div").first();
    //if (pdiv.hasClass("checked")) {
    //    $("#RecurringProductId").prop("disabled", true);
    //}
    //else {
    //    $("#RecurringProductId").prop("disabled", false);
    //}

    if ($("#selfrecurring").is(':checked')) {
        $("#RecurringProductId").prop("disabled", true).addClass("disabled");

    }
    else {
        $("#RecurringProductId").prop("disabled", false).removeClass("disabled");
    }
}


function hasPostBackUrls() {
    
    if ($("#hasurls").is(':checked')) {
    //var pdiv = $("#ss7 > div").first();
    //if (pdiv.hasClass("checked")) {
        if (urlindex < 2) {
            $("#addurl").trigger("click");
        }
        
        $("#postbackform").show();
        //console.log(urlindex);
    }
    else {
        urlindex = 1;
        $("#pburls").html('');
        $("#postbackform").hide();
    }
}

function isSubscription() {

    //var pdiv = $("#ss1 > div").first();
    //if (pdiv.hasClass("checked")) {
    //    $("#subcriptionform").show();
    //    isSelfRecurring();
    //}
    //else {
    //    $("#subcriptionform").hide();
    //}

    if ($("#IsSubscription").is(':checked')) {
        $("#subcriptionform").show();
        isSelfRecurring();
    }
    else {
        $("#subcriptionform").hide();
    }
}

function isShipping() {

    if ($("#IsShippable").is(':checked')) {
    //var pdiv = $("#ss2 > div").first();
    //if (pdiv.hasClass("checked")) {
        $("#shippingform").show();
    }
    else {
        $("#shippingform").hide();
    }
}

function isTaxable() {

    if ($("#IsTaxable").is(':checked')) {
        $("#taxableform").show();
    }
    else {
        $("#taxableform").hide();
    }
}

function hasRedemption() {

    if ($("#HasRedemption").is(':checked')) {
    //var pdiv = $("#ss5 > div").first();
    //if (pdiv.hasClass("checked")) {
        $("#redemptionform").show();
    }
    else {
        $("#redemptionform").hide();
    }
}

function hasProvider() {
    if ($("#Provider").is(':checked')) {
    //var pdiv = $("#ss6 > div").first();
    //if (pdiv.hasClass("checked")) {
        $("#providersform").show();
    }
    else {
        $("#providersform").hide();
    }
}

function loadBalancer() {
    if ($("#LoadBalancer").is(':checked')) {
    //var pdiv = $("#ss3 > div").first();
    //if (pdiv.hasClass("checked")) {
        $("#balancerform").show();
        $("#processorform").hide();
    }
    else {
        $("#processorform").show();
        $("#balancerform").hide();
    }
}

function billType(valueSelected) {
    var inputbillvalueform = $('#inputbillvalueform');

    switch (valueSelected) {
        case 'ByCycle':
            $('#billvaluelabel').text("Days to next billing");
            $('#BillValue').attr('data-original-title', "Subscription will be billed every N days");
            if (!inputbillvalueform.is(":visible")) {
                $('#selectbillvalueform').hide();
                inputbillvalueform.show();
            }
            break;
        case 'ByDate':
            $('#billvaluelabel').text("Billing Date");
            $('#BillValue').attr('data-original-title', "Subscription will be billed on the Nth day of every month");
            if (!inputbillvalueform.is(":visible")) {
                $('#selectbillvalueform').hide();
                inputbillvalueform.show();
            }
            break;
        case 'ByDay':
            $('#billvaluelabel').text("Billing Day");
            if (inputbillvalueform.is(":visible")) {
                inputbillvalueform.hide();
                $('#selectbillvalueform').show();
            }
            break;
    }
}

$(document).on("click", ".rlink", function () {

    // remove the event from the table
    $this = $(this);
    $this.parents("tr:first").remove();
    var eid = $this.data('id');
    var ename = $this.data('name');
    var type = $this.data('type');
    var template = $this.data('template');
    var server = $this.data('server');

    var events = $("#events");
    events.append($("<option></option>")                
        .attr("id", eid)
        .data("type", type)
        .data("template", template)
        .data("server", server)
        .text(ename));

    //events.select2();
    return false;
});

$(document).on("click", ".rslink", function () {

    $this = $(this);   
    var declineid = $this.data('declineid');
   
    var $declineType = $('#salvageprofiles').find("optgroup[value=" + declineid + "]");
    $declineType.children().prop("disabled", false);
    
    $this.closest("tr").remove();
    return false;
});

//(function ($) {
//    $(document).ready(function () {
        
//    });
//})(jQuery);






