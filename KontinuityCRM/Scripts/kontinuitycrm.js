$(function () {
    setNavigation();
});

function setNavigation() {
    var url = window.location.pathname;
    url = url.replace(/\/$/, "");
    url = decodeURIComponent(url);

    var urlparts = url.split('/'); // controller urlparts[1]

    //console.log(url);

    //remove ".active" class from all (previously) ".active" elements
    $('.nav-list li.active').removeClass('active');

    if (url == '') {
        $('.nav-list li').first().addClass('active');
        return;
    }
    
    var new_active;
    var _href;
    $('#sidebar a').each(function () {

        var $this = $(this);
        _href = $this.attr('href');

        //console.log(url);
        

        if (_href != '/' && url.substr(0, _href.length) == _href) {
        //if (_href != '/' && urlparts[1] == _href.substring(1, _href.length)){
            //url.substring(0, href.length) === href) { // if href is not '/' because in that case is always true
            //console.log(_href.substring(1, _href.length));
            //$this.closest('li.submenu').addClass('active open');
            //$(this).parent().addClass('active');          
            new_active = $this;
            $this.addClass('active').parents('.nav-list li').addClass('active open');//.addClass("");
            $this.parent('.nav-list li').removeClass("open");

            return;
            
        }
    });

    //you can also update breadcrumbs:
    var breadcrumb_items = [];
    //$(this) is a reference to our clicked/selected element
    new_active.parents('.nav-list li')        
        .each(function () {
        var link = $(this).find('> a');
        var text = link.text();
        var href = link.attr('href');

        if (href != '#') {
            breadcrumb_items.push({ 'text': text, 'href': href });
        }
        
    });

    //console.log(breadcrumb_items);

    $('ul.breadcrumb li').remove();

   //console.log(urlparts);

    // add home item
    //var breadcrumb_html = '<li><i class="ace-icon fa fa-home home-icon"></i> <a href="/">Home</a></li>';
    $('ul.breadcrumb').append('<li><i class="ace-icon fa fa-home home-icon"></i> <a href="/">Home</a></li>'); //.html('<i class="ace-icon fa fa-home home-icon"></i> <a href="/">Home</a>');

    var i = 0;
    if (urlparts.length > 2) {
    //if (urlparts.length > 2) {
        for (i = 0; i < breadcrumb_items.length - 1; i += 1) {
            //document.writeln(myArray[i]);
            breadcrumb_html += '<li><a href="' + breadcrumb_items[i].href + '">' + breadcrumb_items[i].text + '</a></li>';
        }
        $('ul.breadcrumb').append('<li><a href="' + breadcrumb_items[i].href + '">' + breadcrumb_items[i].text + '</a></li>');
        $('ul.breadcrumb').append('<li class="active capitalize">' + urlparts[2] + '</li>');
        //breadcrumb_html += '<li class="active">' + urlparts[urlparts.length - 1] + '<li>';
    }
    else
    {
        $('ul.breadcrumb').append('<li class="active">' + breadcrumb_items[i].text + '</li>');
        //breadcrumb_html += '<li class="active">' + "ds" + '<li>';
    }
    
    // add last item
    //breadcrumb_html += '<li class="active">' + breadcrumb_items[i].text + '<li>';

    //console.log(breadcrumb_html);
    //update the breadcrumb
    //$('ul.breadcrumb').html(breadcrumb_html);

    //$('ul.breadcrumb').append(breadcrumb_html);

   
    
    
}

function loadScript(scriptName, callback) {

    if (!jsArray[scriptName]) {
        jsArray[scriptName] = true;

        // adding the script tag to the head as suggested before
        var body = document.getElementsByTagName('body')[0];
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = scriptName;

        // then bind the event to the callback function
        // there are several events for cross browser compatibility
        //script.onreadystatechange = callback;
        script.onload = callback;

        // fire the loading
        body.appendChild(script);

    } else if (callback) {// changed else to else if(callback)
        // console.log("JS file already added!");
        //execute function
        callback();
    }

}