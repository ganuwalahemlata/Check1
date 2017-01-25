//jQuery.validator.addMethod('requirediftrue', function (value, element, params) {
//    var checkboxId = $(element).attr('data-val-requirediftrue-boolprop');
//    var checkboxValue = $('#' + checkboxId).first().is(":checked");
//    return !checkboxValue || value;
//}, '');

//jQuery.validator.unobtrusive.adapters.add('requirediftrue', {}, function (options) {
//    options.rules['requirediftrue'] = true;
//    options.messages['requirediftrue'] = options.message;
//});

//

jQuery.validator.addMethod('requiredif', function (value, element, params) {
    console.log('here');
    var checkboxId = $(element).attr('data-val-requiredif-boolprop');
    var checkboxValue = $('#' + checkboxId).first().is(":checked");
    var expected = $(element).attr('data-val-requiredif-expected') == "True";

    console.log(checkboxValue);
    if (expected) {
        return !checkboxValue || value;
    }
    return checkboxValue || value;
    
}, '');

jQuery.validator.unobtrusive.adapters.add('requiredif', {}, function (options) {
    options.rules['requiredif'] = true;
    options.messages['requiredif'] = options.message;
});