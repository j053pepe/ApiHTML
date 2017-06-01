$(function init() {
    $('#Menu').load('../Menu/Menu.html', function () {
        var s = document.createElement("script");
        s.type = "text/javascript";
        s.src = "../Menu/Menu.js";
        // Use any selector
        $("head").append(s);
    });
});