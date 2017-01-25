/**
 * Unicorn Admin Template
 * Version 2.2.0
 * Diablo9983 -> diablo9983@gmail.com
**/
$(document).ready(function(){
	
	//$('input[type=checkbox],input[type=radio]').iCheck({
    //	checkboxClass: 'icheckbox_flat-blue',
    //	radioClass: 'iradio_flat-blue'
	//});
	
	//$('select').select2({
	//    allowClear: true,
	//    placeholder: ""
	//});
    $('.colorpicker').colorpicker();
    $('.datepicker').datepicker();
    $('.spinner').spinner();

    // njhones here!!
    //$('.datepicker').each(function () {

    //    var $this = $(this);
    //    var dataDateFormat = $this.attr('data-dateformat') || 'mm/dd/yy'; //'dd.mm.yy'; njhones change datepicker format

    //    $this.datepicker({
            
    //        dateFormat: dataDateFormat,
    //        prevText: '<i class="fa fa-chevron-left"></i>',
    //        nextText: '<i class="fa fa-chevron-right"></i>',
    //    });
    //});

});
