/// <reference path="~/Web/Scripts/jquery-2.1.4.min.js" />
/// <reference path="~/Web/Scripts/hammer.min.js" />

$(function () {
    var hammer = new window.Hammer($("html").get(0));
    hammer.on("swiperight", function (e) {
        window.external.notify("goback");
    });

    //document.onfullscreenchange = function () {
    //};
});

$(document).on("fullscreenchange", function () {
    var isFullScreen = document.webkitIsFullScreen;
    if (isFullScreen) {
        window.external.notify("enterFullScreen");
    } else {
        window.external.notify("exitFullScreen");
    }
});