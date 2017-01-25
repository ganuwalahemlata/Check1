$('#extra').change(function () {
	addextras();
});

function addextras() {
	var container = $('#extracontent');

	$("#extra option").each(function () {
		$option = $(this);

		//console.log($option);

		switch ($option.val()) {
			case 'Name':
			    //console.log($option.is(':selected'));
				$fgroup = $('#extracontent #fname');
				if ($option.is(':selected')) {
					if ($fgroup.length) { // if element exist
						$fgroup.show(); // show
					} else {  // else append
						container.append('<div class="form-group" id="fname"><label class = "col-sm-3 col-md-3 col-lg-2 control-label">Name</label><div class="col-sm-2 col-md-2"> <input id="Name" class="form-control input-sm" type="text" value="' + $option.data("value") +  '" name="Name" data-val-required="The Name field is required." data-val="true"> </div> <div class="col-sm-6"><span class="field-validation-valid" data-valmsg-replace="true" data-valmsg-for="Name"></span></div></div>');
					}
				}
				else {
					if ($fgroup.is(':visible')) {
						$fgroup.hide();
					}
				}

				break;
			case 'Description':

				$fgroup = $('#extracontent #fdescription');
				if ($option.is(':selected')) {
					if ($fgroup.length) { // if element exist
						$fgroup.show(); // show
					} else {  // else append
					    container.append('<div class="form-group" id="fdescription"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="Description">Description</label><div class="col-sm-9 col-md-9 col-lg-10"><textarea id="Description" class="form-control" rows="3" name="Description" cols="20">' + $option.data("value") + '</textarea></div></div>');
					}
				}
				else {
					if ($fgroup.is(':visible')) {
						$fgroup.hide();
					}
				}

				break;


			case 'Taxable':

				$fgroup = $('#extracontent #ftaxable');
				if ($option.is(':selected')) {
					if ($fgroup.length) { // if element exist
						$fgroup.show(); // show
					} else {  // else append

					    var checked = "";
					    if ($option.data("value") != "") {
					        checked = "checked='checked'";
					    }

						container.append('<div class="form-group" id="ftaxable"><label class="col-sm-3 col-md-3 col-lg-2 control-label"></label><div class="col-sm-9 col-md-9 col-lg-10"><label><input id="Taxable" type="checkbox" value="true" '+ checked +' name="Taxable" data-val-required="The Taxable field is required." data-val="true"> Taxable</label></div></div>').find(':checkbox').iCheck({
							checkboxClass: 'icheckbox_flat-blue',
							radioClass: 'iradio_flat-blue'
						});
					}
				}
				else {
					if ($fgroup.is(':visible')) {
						$fgroup.hide();
					}
				}

				break;

		    case 'ShipValue':

		        $fgroup = $('#extracontent #fshipvalue');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append
		                container.append('<div class="form-group" id="fshipvalue"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="ShipValue">ShipValue</label><div class="col-sm-1"><input class="form-control input-sm" data-val="true" data-val-number="The field ShipValue must be a number." id="ShipValue" name="ShipValue" type="text" value="' + $option.data("value") + '" /></div><div class="col-sm-6"><span class="field-validation-valid" data-valmsg-for="ShipValue" data-valmsg-replace="true"></span></div></div>');
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'Weight':

		        $fgroup = $('#extracontent #fweight');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append
		                container.append('<div class="form-group" id="fweight"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="Weight">Weight</label><div class="col-sm-1"><input class="form-control input-sm" data-val="true" data-val-number="The field Weight must be a number." id="Weight" name="Weight" type="text" value="' + $option.data("value") + '" /></div><div class="col-sm-6"><span class="field-validation-valid" data-valmsg-for="Weight" data-valmsg-replace="true"></span></div></div>');
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'FulfillmentProvider':

		        $fgroup = $('#extracontent #fulfillmentprovider');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append
		                
		                var opts = '<option value=""></option>';
		                var value = $option.data("value");
		                

		                // call to get the populate the ddl
		                $.getJSON('/helper/fulfillmentprovidersddl', function (data) {
		                   
		                    $.each(data, function (i, obj) {
		                       
		                        if (obj.Id == value) {
		                            opts += '<option selected value="' + obj.Id + '">' + obj.Name + '</option>';
		                        } else {
		                            opts += '<option value="' + obj.Id + '">' + obj.Name + '</option>';
		                        }
		                    });
		                   
		                    container.append('<div class="form-group" id="fulfillmentprovider"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="FulfillmentProvider">FulfillmentProvider</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-number="The field FulfillmentProviderId must be a number." id="FulfillmentProviderId" name="FulfillmentProvider">' + opts + '</select><span class="field-validation-valid" data-valmsg-for="FulfillmentProviderId" data-valmsg-replace="true"></span></div></div>').find('select').select2();

		                });

		                
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'SignatureConfirmation':

		        $fgroup = $('#extracontent #fsignatureconfirmation');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var checked = "";
		                if ($option.data("value") != "") {
		                    checked = "checked='checked'";
		                }

		                container.append('<div class="form-group" id="fsignatureconfirmation"><label class="col-sm-3 col-md-3 col-lg-2 control-label"></label><div class="col-sm-9 col-md-9 col-lg-10"><label><input id="SignatureConfirmation" type="checkbox" value="true" ' + checked + ' name="SignatureConfirmation" data-val-required="The SignatureConfirmation field is required." data-val="true"> SignatureConfirmation</label></div></div>').find(':checkbox').iCheck({
		                    checkboxClass: 'icheckbox_flat-blue',
		                    radioClass: 'iradio_flat-blue'
		                });
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'DeliveryConfirmation':

		        $fgroup = $('#extracontent #fdeliveryconfirmation');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var checked = "";
		                if ($option.data("value") != "") {
		                    checked = "checked='checked'";
		                }

		                container.append('<div class="form-group" id="fdeliveryconfirmation"><label class="col-sm-3 col-md-3 col-lg-2 control-label"></label><div class="col-sm-9 col-md-9 col-lg-10"><label><input id="DeliveryConfirmation" type="checkbox" value="true" ' + checked + ' name="DeliveryConfirmation" data-val-required="The DeliveryConfirmation field is required." data-val="true"> DeliveryConfirmation</label></div></div>').find(':checkbox').iCheck({
		                    checkboxClass: 'icheckbox_flat-blue',
		                    radioClass: 'iradio_flat-blue'
		                });
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'RecurringProductId':

		        $fgroup = $('#extracontent #frecurringproductid');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var opts = '<option value=""></option>';
		                var value = $option.data("value");


		                // call to get the populate the ddl
		                $.getJSON('/helper/recurringproductidddl', function (data) {

		                    $.each(data, function (i, obj) {

		                        if (obj.Id == value) {
		                            opts += '<option selected value="' + obj.Id + '">' + obj.Name + '</option>';
		                        } else {
		                            opts += '<option value="' + obj.Id + '">' + obj.Name + '</option>';
		                        }
		                    });

		                    container.append('<div class="form-group" id="frecurringproductid"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="RecurringProductId">RecurringProductId</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-number="The field RecurringProductId must be a number." id="RecurringProductId" name="RecurringProductId">' + opts + '</select><span class="field-validation-valid" data-valmsg-for="RecurringProductId" data-valmsg-replace="true"></span></div></div>').find('select').select2();

		                });


		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'BillType':

		        $fgroup = $('#extracontent #fbilltype');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var value = $option.data("value");
		                
		                var opts = '<option value="ByCycle"' + ((value == "ByCycle") ? 'selected' : '') + '>By cycle</option>'
                                 + '<option value="ByDate"' + ((value == "ByDate")? 'selected' : '') + '>By date</option>'
                                 + '<option value="ByDay"' + ((value == "ByDay") ? 'selected' : '') + '>By day</option>';
		                
		                container.append('<div class="form-group" id="fbilltype"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="BillType">BillType</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-required="The BillType field is required." id="BillType" name="BillType">' + opts + '</select>').find('select').select2();
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'BillValue':
		        
		        $fgroup = $('#extracontent #fbillvalue');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var billtypevalue = $('#extracontent #fbilltype').find(':selected').val();
		                //console.log(billtypevalue);

		                if (typeof billtypevalue === 'undefined')
		                {
                            // get the billtype from the product
		                }
		                container.append('<div class="form-group" id="fbillvalue"><label class = "col-sm-3 col-md-3 col-lg-2 control-label">BillValue</label><div class="col-sm-2 col-md-2"> <input id="BillValue" class="form-control input-sm" type="text" value="' + $option.data("value") + '" name="BillValue" data-val-required="The BillValue field is required." data-val="true"> </div> <div class="col-sm-6"><span class="field-validation-valid" data-valmsg-replace="true" data-valmsg-for="BillValue"></span></div></div>');
		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'Processor':

		        $fgroup = $('#extracontent #fprocessor');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var opts = '<option value=""></option>';
		                var value = $option.data("value");


		                // call to get the populate the ddl
		                $.getJSON('/helper/processorsddl', function (data) {

		                    $.each(data, function (i, obj) {

		                        if (obj.Id == value) {
		                            opts += '<option selected value="' + obj.Id + '">' + obj.Name + '</option>';
		                        } else {
		                            opts += '<option value="' + obj.Id + '">' + obj.Name + '</option>';
		                        }
		                    });

		                    //container.append('<div class="form-group" id="processor"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="Processor">Processor</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-number="The field FulfillmentProviderId must be a number." id="FulfillmentProviderId" name="FulfillmentProvider">' + opts + '</select><span class="field-validation-valid" data-valmsg-for="FulfillmentProviderId" data-valmsg-replace="true"></span></div></div>').find('select').select2();
		                    container.append('<div class="form-group" id="fprocessor"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="Processor">Processor</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-number="The field Processor must be a number." id="Processor" name="Processor">' + opts + '</select></div></div>').find('select').select2();
		                });


		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		    case 'Balancer':

		        $fgroup = $('#extracontent #fbalancer');
		        if ($option.is(':selected')) {
		            if ($fgroup.length) { // if element exist
		                $fgroup.show(); // show
		            } else {  // else append

		                var opts = '<option value=""></option>';
		                var value = $option.data("value");


		                // call to get the populate the ddl
		                $.getJSON('/helper/balancersddl', function (data) {

		                    $.each(data, function (i, obj) {

		                        if (obj.Id == value) {
		                            opts += '<option selected value="' + obj.Id + '">' + obj.Name + '</option>';
		                        } else {
		                            opts += '<option value="' + obj.Id + '">' + obj.Name + '</option>';
		                        }
		                    });

		                    container.append('<div class="form-group" id="fbalancer"><label class="col-sm-3 col-md-3 col-lg-2 control-label" for="Balancer">Balancer</label><div class="col-sm-9 col-md-9 col-lg-10"><select data-val="true" data-val-number="The field Balancer must be a number." id="Balancer" name="Balancer">'+ opts +'</select></div></div>').find('select').select2();
		                });


		            }
		        }
		        else {
		            if ($fgroup.is(':visible')) {
		                $fgroup.hide();
		            }
		        }

		        break;

		}

	});

	var form = $("#mform");
	form.removeData('validator');
	form.removeData('unobtrusiveValidation');
	$.validator.unobtrusive.parse(form);
}
