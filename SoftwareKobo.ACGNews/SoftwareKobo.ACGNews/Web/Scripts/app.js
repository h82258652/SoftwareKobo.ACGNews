/// <reference path="~/Web/Scripts/jquery-2.1.4.min.js" />
/// <reference path="~/Web/Scripts/hammer.min.js" />

(function () {
    var rawSrcSetter = HTMLScriptElement.prototype.__lookupSetter__("src");
    HTMLScriptElement.prototype.__defineSetter__("src", function (src) {
        if (src.indexOf("//") === 0) {
            src = "http:" + src;
        }
        rawSrcSetter.call(this, src);
    });

    var rawHrefSetter = HTMLLinkElement.prototype.__lookupSetter__("src");
    HTMLLinkElement.prototype.__defineSetter__("href", function (href) {
        if (href.indexOf("//") === 0) {
            href = "http:" + href;
        }
        rawHrefSetter.call(this, href);
    });
})();

$(function () {
    var hammer = new window.Hammer($("html").get(0));
    hammer.on("swiperight", function (e) {
        window.external.notify("goback");
    });
});

$(document).on("fullscreenchange", function () {
    var isFullScreen = document.webkitIsFullScreen;
    if (isFullScreen) {
        window.external.notify("enterFullScreen");
    } else {
        window.external.notify("exitFullScreen");
    }
});