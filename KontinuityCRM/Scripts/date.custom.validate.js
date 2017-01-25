//all the fallowing variant works!

jQuery(function ($) {
    //$.validator.addMethod('date', function (value, element) {
    //    console.log('here');
    //    if (this.optional(element)) {
    //        return true;
    //    }

    //    var ok = true;
    //    try {
    //        $.datepicker.parseDate('mm-dd-yy', value);
    //    }
    //    catch (err) {
    //        ok = false;
    //    }
    //    return ok;
    //});

    //$.validator.methods.date = function (value, element) {
    //    var s = value;
    //    s = value.replace(/\-/g, '/');

    //    return this.optional(element) || !/Invalid|NaN/.test(new Date(s));
    //};
});

//$(function () {
//    $.validator.addMethod(
//    "date",
//    function (value, element) {
//        var s = value;
//        s = value.replace(/\-/g, '/');
//        console.log('here');

//        // Chrome requires tolocaledatestring conversion, otherwise just use the slashed format
//        var d = new Date();
//        return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value))) || !/Invalid|NaN/.test(new Date(s));
//    },
//    ""
//    );
//});

//$.validator.unobtrusive.parse();