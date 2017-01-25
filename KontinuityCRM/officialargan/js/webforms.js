var AJAX_LOCATION = 'https://www.transactsafely.com/ajax.php';
//MUST be off in production
//
var DebugWebForms = false;

function Debug(message)
{
    if(DebugWebForms)
    {
        LimeDbg(message);
    }
}

function SetCountryValue(stateLabel,intlState2,mericanState,country,stateOuter,stateOuter2)
{
    el_1 = document.getElementById(stateOuter);
    el_2 = document.getElementById(stateOuter2);
    var country_val = document.getElementById(country).selectedIndex;
    var select_val = document.getElementById(country).options[country_val].text;
    if (country.search(new RegExp("billing", "gi")) == 0 )
    {
        var billShip = 'Billing ';
    }
    else
    {
        var billShip = 'Shipping ';
    }
    if(select_val == "United States")
    {
        if (document.getElementById(stateLabel))
        {
            document.getElementById(stateLabel).innerHTML = billShip + 'State:';
        }
        document.getElementById(mericanState).selectedIndex = 0;
        document.getElementById(intlState2).value = '';
        el_1.style.display = "inline";
        el_2.style.display = "none";
    }
    else
    {
        if (document.getElementById(stateLabel))
        {
            document.getElementById(stateLabel).innerHTML = billShip + 'Region:';
        }
        document.getElementById(mericanState).selectedIndex = 0;
        el_2.style.display = "inline";
        el_1.style.display = "none";
    }
}

function SetShippingValue()
{
    var totalObj = document.getElementById('total_amount');
    var tempStr = totalObj.innerHTML;
    tempStr = tempStr.ltrim();

    //remove the currency sign
    var totalAmountString = tempStr.substr(1);

    //get the previous shipping amount
    var previousShippingStr = document.getElementById('shipping_price').innerHTML;

    //remove the currency symbol
    var trimmed = previousShippingStr.replace(/^\s+|\s+$/g, '') ;
    var previousShippingStr = trimmed.substr(1);

    //so take new shipping amount and subtract the old shipping amount, this is how much total_amount needs to move
    var ship_price = document.getElementById('shipping').options[document.getElementById('shipping').selectedIndex].title;
    var delta = parseFloat(ship_price) - parseFloat(previousShippingStr);

    document.getElementById('shipping_price').innerHTML = currencySymbol  + ship_price;

    var total_amount = parseFloat(totalAmountString) + parseFloat(delta);

    total_amount = total_amount.toFixed(2);
    document.getElementById('total_amount').innerHTML = currencySymbol + total_amount;

}

function SetCountryValue2()
{

}


function copyToState2()
{
    var state_value = document.forms['opt_in_form'].fields_state.options[document.forms['opt_in_form'].fields_state.selectedIndex].text;
    document.forms['opt_in_form'].fields_state2.value = state_value;
}

function isValidEmail(str)
{
    var at="@";
    var dot=".";
    var lat=str.indexOf(at);
    var lldot = str.lastIndexOf(dot);
    var lstr=str.length;
    var ldot=str.indexOf(dot);

    if (str.indexOf(at)==-1){
        alert("Invalid E-mail Address");
        return false;
    }

    if (str.indexOf(at)==-1 || str.indexOf(at)==0 || str.indexOf(at)==(lstr-1) || str.indexOf(at)==(lstr-3))
    {
        alert("Invalid Email Address");
        return false;
    }

    if (lldot==lstr-1 || lldot==lstr-2 || (lldot==ldot && lldot < lat))
    {
        alert("Invalid Email Address");
        return false;
    }

    if (str.indexOf(dot)==-1 || str.indexOf(dot)==0 || str.indexOf(dot)==(lstr-1))
    {
        alert("Invalid Email Address");
        return false;
    }

    if (str.indexOf(at,(lat+1))!=-1)
    {
        alert("Invalid Email Address");
        return false;
    }

    if (str.substring(lat-1,lat)==dot || str.substring(lat+1,lat+2)==dot)
    {
        alert("Invalid Email Address");
        return false;
    }

    if (str.indexOf(dot,(lat+2))==-1)
    {
        alert("Invalid Email Address");
        return false;
    }

    if (str.indexOf(" ")!=-1)
    {
        alert("Invalid Email Address");
        return false;
    }

    var isDoubleByte = false;
    for(var i=0; i<str.length; i++)
    {
        if (str.charCodeAt(i) > 127)
        {
            isDoubleByte = true;
            break;
        }
    }

    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    if(!isDoubleByte && reg.test(str) == false)
    {
        alert("Invalid Email Address");
        return false;
    }
    return true;
}

function getcitystate(zip)
{
	return false;
    var country;
    $ = jQuery.noConflict();
    if(zip.value==''){
        return false;
    }
    $.ajax({
        url: 'includes/zip.php?zip=' + zip.value, 
        method:'get',
        dataType:'json',
        contentType: "application/json;charset=utf-8",
        async:'false',
        success: function(res)
        {
            if(res.isFound){
                country = $("#country").val();
                if(country==res.country){
                    $("#fields_city").val(res.city);
                    $("#fields_state").val(res.state);
                    SetStateHid(document.getElementById("fields_state"));
                }
//                }else{
//                    $("#country").val(res.country);
//                    $("#country").trigger("change");
//                }
//                
            }
        //document.getElementById("fields_phone").value = jQuery(res).find("AREA_CODE").text();
        }, 
        error: function(xhr, errorString, ex)
        {
        //
        }
    });
}

function allValidChars(email)
{
    var parsed = true;
    var validchars = "abcdefghijklmnopqrstuvwxyz0123456789@.-_";
    for (var i=0; i < email.length; i++)
    {
        var letter = email.charAt(i).toLowerCase();
        if (validchars.indexOf(letter) != -1)
            continue;
        parsed = false;
        break;
    }
    return parsed;
}

function update_phone_field(field_name)
{
    phone1 = document.getElementById(field_name + "_phone1").value;
    phone2 = document.getElementById(field_name + "_phone2").value;
    phone3 = document.getElementById(field_name + "_phone3").value;
    document.getElementById(field_name).value = phone1 + phone2 + phone3;
}

function update_expire()
{
    var month = document.getElementById("fields_expmonth");
    var month_value = month.options[month.selectedIndex].value;
    var year = document.getElementById("fields_expyear");
    var year_value = year.options[year.selectedIndex].value;
    if ((month_value != '' ) && (year_value != ''))
    {
        document.getElementById("cc_expires").value = month_value +  year_value;
    }
    else
    {
        document.getElementById("cc_expires").value = '';
    }
}

var taborder = Array();
taborder['postal'] = 'phone1';
taborder['phone1'] = 'phone2';
taborder['phone2'] = 'phone3';
function Key13handler(keyCode, sender)
{
    go_next = 0;
    if (keyCode == 13) go_next = 1;
    if (go_next)
    {
        nextname = taborder[sender.name];
        objEl = document.forms['opt_in_form'][nextname];
        if (objEl != null)
            objEl.focus();
    }
}

function onPhoneKeyUp(keyCode, sender)
{
    go_next = 0;
    if ((keyCode != 13) && (sender.name == 'phone1') && (sender.value.length > 2)) go_next = 1;
    if ((keyCode != 13) && (sender.name == 'phone2') && (sender.value.length > 2)) go_next = 1;
    if (go_next)
    {
        nextname = taborder[sender.name];
        objEl = document.forms['opt_in_form'][nextname];
        if (objEl != null)
            objEl.focus();
    }
}



function validateMilitary(pointerState,pointerCity)
{
    if(document.getElementById(pointerState) && document.getElementById(pointerState).value != '')
    {
        var val = document.getElementById(pointerState).value;

        var militaryState = (
            val.indexOf('Armed Forces') !== -1 ||
            val == 'AA' ||
            val == 'AP' ||
            val == 'AE' ||
            val == 'AE-A' ||
            val == 'AE-E' ||
            val == 'AE-M' ||
            val == 'AE-C'
            );

        var militaryCity  = (
            document.getElementById(pointerCity).value == 'APO' ||
            document.getElementById(pointerCity).value == 'FPO' ||
            document.getElementById(pointerCity).value == 'DPO'
            );

        if (militaryState == true && militaryCity == false)
        {
            return false;
        }
        return true;
    }
    return true;
}

function payment_change(obj)
{
    switch (obj.value)
    {
        case '' :
            break;
        case 'checking' :
            ll('#routing_number_tr').show();
            ll('#account_number_tr').show();
        case 'offline' :
            ll('#cc_expires_tr').hide().val('');
            ll('#cc_cvv_tr').hide().val('');
            ll('#cc_number_tr').hide().val('');
            document.getElementById('fields_expmonth').value ='';
            document.getElementById('fields_expyear').value ='';
            break;
        default :
            ll('#cc_expires_tr').show();
            ll('#cc_cvv_tr').show();
            ll('#cc_number_tr').show();
            ll('#routing_number_tr').hide().val('');
            ll('#account_number_tr').hide().val('');

            switch (obj.value)
            {
                case 'solo' :
                case 'switch' :
                case 'maestro' :
                    ll('#cc_number').attr('maxLength', '19');
                    //ll('#cc_cvv_tr label[for="cc_cvv"]').text('Issue Id:');
                    ll('#cc_cvv').attr('maxLength', '2');
                    ll('#cc_cvv').val(ll('#cc_cvv').val().substr(0,2));
                    break;
                default :
                    ll('#cc_number').attr('maxLength', '16');
                    ll('#cc_number').val(ll('#cc_number').val().substr(0,16));
                    //ll('#cc_cvv_tr label[for="cc_cvv"]').text('Security Code:');
                    ll('#cc_cvv').attr('maxLength', '4');
                    break;
            }
            break;
    }
}

String.prototype.ltrim = function()
{
    return this.replace(/^\s+/,"");
}

// -- dont remove these globals
var required_fields=new Array();
var required_fields_label=new Array();

function toggleBillingAddress(radioButtonObj)
{
    var billingDiv = document.getElementById('billingDiv');
    if (radioButtonObj.value == "no")
    {
        billingDiv.style.display = 'inline';
    }
    else
    {
        billingDiv.style.display = 'none';
    }
}

function onlyNumbers(e,type)
{
    var keynum;
    var keychar;
    var numcheck;
    if(window.event) // IE
    {
        keynum = e.keyCode;
    }
    else if(e.which) // Netscape/Firefox/Opera
    {
        keynum = e.which;
    }
    keychar = String.fromCharCode(keynum);
    numcheck = /\d/;

    switch (keynum)
    {
        case 8:    //backspace
        case 9:    //tab
        case 35:   //end
        case 36:   //home
        case 37:   //left arrow
        case 38:   //right arrow
        case 39:   //insert
        case 45:   //delete
        case 46:   //0
        case 48:   //1
        case 49:   //2
        case 50:   //3
        case 51:   //4
        case 52:   //5
        case 54:   //6
        case 55:   //7
        case 56:   //8
        case 57:   //9
        case 96:   //0
        case 97:   //1
        case 98:   //2
        case 99:   //3
        case 100:  //4
        case 101:  //5
        case 102:  //6
        case 103:  //7
        case 104:  //8
        case 105:  //9
            result2 = true;
            break;
        case 109: // dash -
            if (type == 'phone')
            {
                result2 = true;
            }
            else
            {
                result2 = false;
            }
            break;
        default:
            result2 = numcheck.test(keychar);
            break;
    }

    return result2;
}

/*function GetTaxMessage()
{
   ll.ajax({
      dataType: 'jsonp',
      jsonp: 'webFormAjax',
      url: AJAX_LOCATION + '?m=1&' + ll('#opt_in_form').serialize(),
      success: function (response) {
         if(response['status'] > 0)
         {
            ll('#cc_number').before(response['content']);
         }
      }
   });
}*/

if (typeof(ll) != 'undefined')
{
    // -- here we have the body loaded on all traffic on all new web forms.  Add everything you need here.
    ll(document).ready(function()
    {
        if (document.getElementById('isWebForm'))
        {
            // -- all web form body onloads come through here now
            if (document.getElementById('cc_type') && document.getElementById('cc_type_tr'))
            {
                document.getElementById('cc_type').onchange();
            }
            if (document.getElementById('routing_number_tr'))
            {
                ll('#routing_number_tr').hide();
            }
            if (document.getElementById('account_number_tr'))
            {
                ll('#account_number_tr').hide();
            }

            if (ll('#opt_in_form').length > 0)
            {
                ll('#opt_in_form').attr('accept-charset','ISO-8859-1').attr('enctype','application/x-www-form-urlencoded;charset=ISO-8859-1');
                if (ll('#limelight_charset').length == 0)
                {
                    ll('#opt_in_form').append("<input type='hidden' name='limelight_charset' id='limelight_charset' value='ISO-8859-1'/>");
                }
            }
            else if (ll('form[name="opt_in_form"]').length > 0)
            {
                ll('form[name="opt_in_form"]').attr('accept-charset','ISO-8859-1').attr('enctype','application/x-www-form-urlencoded;charset=ISO-8859-1');
                if (ll('#limelight_charset').length == 0)
                {
                    ll('form[name="opt_in_form"]').append("<input type='hidden' name='limelight_charset' id='limelight_charset' value='ISO-8859-1'/>");
                }
            }
            else if (ll('#opt_in_form2').length > 0)
            {
                ll('#opt_in_form2').attr('accept-charset','ISO-8859-1').attr('enctype','application/x-www-form-urlencoded;charset=ISO-8859-1');
                if (ll('#limelight_charset').length == 0)
                {
                    ll('#opt_in_form2').append("<input type='hidden' name='limelight_charset' id='limelight_charset' value='ISO-8859-1'/>");
                }
            }
            else if (ll('form').length > 0)
            {
                ll('form').attr('accept-charset','ISO-8859-1').attr('enctype','application/x-www-form-urlencoded;charset=ISO-8859-1');
                if (ll('#limelight_charset').length == 0)
                {
                    ll('form').append("<input type='hidden' name='limelight_charset' id='limelight_charset' value='ISO-8859-1'/>");
                }
            }
        }
    });
}

function CheckProductCode(baseProductId, codeMap)
{
    var promotxt       = ll("#product_promo"),
    foundCodeMatch = false;

    ll.each(codeMap, function(code, promoProductId) {
        if (promotxt.val().toUpperCase() == code.toUpperCase())
        {
            var allProducts = ll('input[name="extra"]');
            foundCodeMatch = true;

            ll('#div_product_' + baseProductId).hide();
            ll('#div_product_' + promoProductId).show();
            ll('#product_' + promoProductId).click();
            ll("#div_product_promo").hide();

            return false;
        }
    });

    if (! foundCodeMatch)
    {
        alert("Please enter a valid promo code.");
        promotxt.select();
        window.setTimeout(function(){
            promotxt.focus()
            },20);
    }
}
/**
* GetProductQuantity()
*
* Obtains a specific products quantity and max as well as validates them
*/
function GetProductQuantity(productId)
{
    var quantity = 1;
    var maxQty   = 1;

    if(typeof(productId) != 'undefined' && productId != '')
    {
        var element = 'productQty['+productId+']';

        if(document.getElementById(element))
        {
            quantity = parseInt(document.getElementById(element).value);

            //Obtain the max
            //
            maxQtyTmp = parseInt(document.getElementById(element).className);

            if(typeof(maxQtyTmp) != 'undefined' && maxQtyTmp > 0)
            {
                maxQty = maxQtyTmp;
            }

            //Ensure that the entered quantity doesnt exceed the max
            //
            if(quantity > maxQty)
            {
                quantity = maxQty;
                document.getElementById(element).value = quantity;
            }
        }
    }

    if(! quantity || typeof(quantity) == 'undefined' || ! (quantity > 0))
    {
        quantity = 1;
    }

    return quantity;
}

function UpdateProductQuantity(basePrice, productId)
{
    UpdateDisplayTotal();
}

/**
* UpdateDisplayTotal()
*
* Updates interface totals only. Nothing of value is actually posted and used.
*
* @return void
*/
function UpdateDisplayTotal()
{
    var mainProductTotal = GetMainProductTotal();   //Current main product total
    var upsellTotal      = GetUpsellProductTotal(); //Total of selected upsells
    var shippingPrice    = GetShippingTotal()       //Shipping price selected

    document.getElementById('p_price').innerHTML = '<span class="priceDisplay">' + currencySymbol + mainProductTotal.toFixed(2) + '</span>'

    var total_amount = (shippingPrice + mainProductTotal + upsellTotal);

    Debug('total_amount = shippingPrice: ' + shippingPrice + ' + mainProductTotal: ' + mainProductTotal + ' + upsellTotal: ' + upsellTotal);

    total_amount = currencySymbol + total_amount.toFixed(2);

    document.getElementById('total_amount').innerHTML = total_amount;
}

/**
* GetUpsellProductTotal()
*
* Replaces legacy 10k loop. Totals all currently selected upsells with quantities included
*
* @replaces GetUpsellProductTotalLegacy()
* @return float
*/
function GetUpsellProductTotal()
{
    upsellTotal = 0.00;

    if(typeof(ll) != 'undefined' && ll('.upsell_price_x').length)
    {
        ll('.upsell_price_x').each(function(i)
        {
            var upsellIdTmp = ll(this).attr('id');
            var upsellId    = upsellIdTmp.split('_');
            upsellId        = upsellId[2];

            if(ll('#upsell_' + upsellId).attr('checked'))
            {
                var upsellPrice = parseFloat(ll(this).attr('value'));
                upsellQty       = GetProductQuantity(upsellId);

                upsellTotal += (upsellPrice * upsellQty);
            }
        });
    }
    else
    {
        GetUpsellProductTotalLegacy();
    }

    Debug('upsellTotal new ' + upsellTotal);

    return parseFloat(upsellTotal);
}

/**
* GetUpsellProductTotalLegacy()
*
* Do not call this function directly, instead, call GetUpsellProductTotal()
*
* Legacy 10k loop. Totals all currently selected upsells with quantities included.
*
* @return float
*/
function GetUpsellProductTotalLegacy()
{
    upsellTotal = 0.00;

    for (i = 0; i <10000; i++)
    {
        var elementName = "upsell_price_"+i;
        if (document.getElementById(elementName))
        {
            var checkboxName   = 'upsell_'+i;
            var priceObj       = document.getElementById(elementName);
            var checkboxObject = document.getElementById(checkboxName);

            if (checkboxObject.checked)
            {
                upsellQty    = GetProductQuantity(i);
                upsellTmp    = parseFloat(priceObj.value);
                upsellTotal += (upsellTmp * upsellQty);
            }
        }
    }

    Debug('upsellTotal legacy ' + upsellTotal);

    return parseFloat(upsellTotal);
}

/**
* GetMainProductTotal()
*
* Gets current main product and applies quantities
* custom_product exists for all campaign types post quantities. If it doesnt exist then neither does quantities
*
* @return float
*/
function GetMainProductTotal()
{
    //Check for a main product id first
    //
    if(document.getElementById('custom_product'))
    {
        var mainProductId    = document.getElementById('custom_product').value;      //Current main product id
        var mainProductPrice = document.getElementById('custom_product_price').value //Current main product price
        var mainProductQty   = GetProductQuantity(mainProductId);                    //Current main product quantity
    }
    else
    {
        var mainProductPrice = GetMainProductTotalLegacy();
        var mainProductQty   = GetProductQuantity();
    }

    Debug('mainProductId ' + mainProductId);
    Debug('mainProductPrice ' + mainProductPrice);
    Debug('mainProductQty ' + mainProductQty);

    //Apply quantities to main product
    //
    mainProductPrice = (mainProductPrice * mainProductQty);

    Debug(mainProductPrice +' * '+mainProductQty);
    Debug('mainProductPrice ' + mainProductPrice);

    return parseFloat(mainProductPrice);
}

/**
* GetMainProductTotalLegacy()
*
* Gets current main product via legacy parsing methods
*
* @return float
*/
function GetMainProductTotalLegacy()
{
    if(document.getElementById('custom_product_price'))
    {
        var mainProductPrice = document.getElementById('custom_product_price').value;
    }
    else if(document.getElementById('p_price'))
    {
        var mainProductPrice = ParseMainProductTotal()
    }

    Debug('GetMainProductTotalLegacy() mainProductPrice ' + mainProductPrice);

    return parseFloat(mainProductPrice);
}

/**
* ParseMainProductTotal()
*
* Fail-safe/Legacy method for retrieving the main product amount
*
* @return float
*/
function ParseMainProductTotal()
{
    //Main element like: <div id="p_price"><font color="#ff0000">$1.00</font></div>
    //
    var mainPricetmp = document.getElementById('p_price').innerHTML;

    mainPricetmp = mainPricetmp.replace(/<\/?[^>]+(>|$)/g, "");
    mainPricetmp = mainPricetmp.replace(/^\s+|\s+$/g,"");

    //Remove the currency sign
    //
    var mainProductPrice = parseFloat(mainPricetmp.substr(1));

    Debug('ParseMainProductTotal() mainProductPrice ' + mainProductPrice);

    return mainProductPrice;
}

/**
* GetShippingTotal()
*
* Gets current shipping price
*
* @return float
*/
function GetShippingTotal()
{
    if(document.getElementById('shipping'))
    {
        var shippingPrice = document.getElementById('shipping').options[document.getElementById('shipping').selectedIndex].title;
    }
    else if(document.getElementById('shipping_price'))
    {
        var shippingPrice = ParseShippingTotal();
    }

    Debug('GetShippingTotal() shippingPrice ' + shippingPrice);

    return parseFloat(shippingPrice);
}

/**
* ParseShippingTotal()
*
* Fail-safe/Legacy method for retrieving the shipping amount
*
* @return float
*/
function ParseShippingTotal()
{
    //Main element like: <div color="#FF0000" id="shipping_price"> $1.00 </div>
    //
    var shippingPriceTmp = document.getElementById('shipping_price').innerHTML;

    shippingPriceTmp = shippingPriceTmp.replace(/<\/?[^>]+(>|$)/g, "");
    shippingPriceTmp = shippingPriceTmp.replace(/^\s+|\s+$/g,"");

    //Remove the currency sign
    //
    var shippingPrice = parseFloat(shippingPriceTmp.substr(1));

    Debug('ParseShippingTotal() shippingPrice ' + shippingPrice);

    return shippingPrice;
}

/**
* clickCheckbox()
*
* Upsell Toggle - legacy support
* User selected or deselected an upsell. Just updates the displayed order total
*
*/
function clickCheckbox(checkboxObj,amountToUse,currencySymbol)
{
    Debug('clickCheckbox');
    UpdateDisplayTotal();
}

/**
* ProductShippingToggle()
*
* Product / Shipping method toggle
* If a mapping function exists, use the results to toggle shipping methods based on the selected product
*
* Map format is
* map = {
*    product_id_x : [shipping_id_a,shipping_id_b],
*    product_id_y : [shipping_id_c]
* }
*
*/
function ProductShippingToggle(productId)
{
    if (typeof ProductShippingMap == 'function')
    {
        if (! productId)
        {
            productId = ll('#custom_product').val();
        }

        map = ProductShippingMap();

        if (map[productId])
        {
            var shippingIds = map[productId],
            selected    = false;

            if (! ll.isArray(shippingIds))
            {
                shippingIds = [shippingIds];
            }

            ll.each(ll('option', ll('#shipping')), function() {

                var shippingId = parseInt(ll(this).val());

                if (ll.inArray(shippingId, shippingIds) == -1)
                {
                    ll(this).hide();
                }
                else
                {
                    ll(this).show();
                    if (! selected)
                    {
                        selected = true;
                        ll(this).attr('selected','selected');
                        SetShippingValue();
                    }

                }
            });
        }
    }
}

/**
* change_products()
*
* Main Product toggle - legacy support
* Updates form elements to the currently selected main product
*
*/
function change_products(price,id)
{
    Debug('price ' + price);
    Debug('id ' + id);

    if(document.getElementById('custom_product'))
    {
        document.getElementById('custom_product').value = id;
    }
    else
    {
        if(document.getElementById('custom_product_price'))
        {
            var inputProductElement = document.getElementById('custom_product_price');
        }
        else if(document.getElementById('p_price'))
        {
            var inputProductElement = document.getElementById('p_price');
        }
        else
        {
            var inputProductElement = document.getElementsByTagName('body')[0];
        }

        var inputProductId = document.createElement('input');
        inputProductId.setAttribute('type', 'hidden');
        inputProductId.setAttribute('id', 'custom_product');

        inputProductElement.appendChild(inputProductId);

        document.getElementById('custom_product').value = id;

        Debug('document.getElementById(custom_product) ' + document.getElementById('custom_product'));
        Debug(id);
    }

    if(document.getElementById('custom_product_price'))
    {
        document.getElementById('custom_product_price').value = price.toFixed(2);
    }

    //Check the radio buttong if it exists
    //
    var mainProductRadioButton = 'product_' + id;

    if(document.getElementById(mainProductRadioButton))
    {
        document.getElementById(mainProductRadioButton).checked = true;
    }

    Debug('change_products: price: ' + price + ' id: ' + id);

    UpdateDisplayTotal();

    ProductShippingToggle(id);
}

/**
* AJAX request to dynamically change state choices based on country
*
* @param obj       country object
* @param targetObj state object
* @param sel       selected state value
* @param id        state object id
* @param spinner   bool show spinner
*/
function ChangeCountry(list)
{
    var obj       = ll(list.obj),
    targetObj = ll(list.targetObj),
    sel       = ((typeof list.sel != 'undefined') ? list.sel : ''),
    id        = ((typeof list.id != 'undefined') ? list.id : ''),
    spinner   = ((typeof list.spinner != 'undefined') ? list.spinner : false),
    focus     = ((typeof list.focus != 'undefined') ? list.focus : true),
    url       = AJAX_LOCATION + '?m=2&country=' + obj.val() + '&sel=' + sel;

    if (id)
    {
        url += '&id=' + id;
    }

    ll('#fields_state', targetObj).val('').change();

    if (spinner)
    {
        targetObj.html('<img src="images/limeload.gif" alt="Loading" />')
    }

    ll.ajax({
        dataType: 'jsonp',
        jsonp: 'webFormAjax',
        url: url,
        success: function (response) {
            if(response['status'] > 0)
            {
                targetObj.html(response['content']);
                ll('#fields_state option:first').remove();

                if (focus)
                {
                    if (ll('#' + id))
                    {
                        ll('#' + id).focus();
                    }
                }
            }
        }
    });
}

/**
* AJAX request to dynamically change the state label based on country
*
* @param obj       country object
* @param targetObj state label object
* @param template  label template
*/
function ChangeStateLabel(list)
{
    var obj       = ll(list.obj),
    targetObj = ll(list.targetObj),
    template  = list.template;

    ll.ajax({
        dataType: 'jsonp',
        jsonp: 'webFormAjax',
        url: AJAX_LOCATION + '?m=3&country=' + obj.val() + '&template=' + template,
        success: function (response) {
            if(response['status'] > 0)
            {
                targetObj.html(response['content']);
            }
        }
    });
}

/**
* Two in one
*
* @param obj          country object
* @param stateObj     state object
* @param sel selected state value
* @param id generated state id
* @param labelObj     state label object
* @param template     state label template
*/
function ChangeCountryAndStateLabel(countryList, stateList)
{
    ChangeCountry(countryList);
    ChangeStateLabel(stateList);
}

/**
* Set a hidden element for regenerating AJAX after using browser back button
*
* @param el state element
*/
function SetStateHid(el)
{
    ll('#' + ll(el).attr('id') + '_hid').val(ll(el).val());
}
