/// <reference path="~/Web/Scripts/jquery-2.1.4.min.js" />
/// <reference path="~/Web/Scripts/hammer.min.js" />

function setContent(content) {
    $("#content").html(content);
}

$(function () {
    var hammer = new window.Hammer($("html").get(0));
    hammer.on("swiperight", function (e) {
        window.external.notify("goback");
    });
});