/// <reference path="~/Scripts/lib/jquery.js" />
/// <reference path="~/Scripts/lib/knockout.js" />

define(["knockout"], function (ko) {

  var context = $(".welcome-tribes-component")[0];
  var props = $(context).data();

  $(context).delegate("a.m-btn.big.blue", "click", function (event) {

    $.get("/api/AddUserToTribe", {
      tribe: event.target.innerText
    }, function (response) {
      console.log(response);
    });

    return false;
  });
});