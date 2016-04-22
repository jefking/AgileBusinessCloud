/*
* @class LoadingScreenController
* @context .component
* 
* Provides any component the ability to have a UI blocker for both data access and/or behavioural
* interaction. A typical use case for this would be when a user decides to reload a set of data.
* When this occurs, we want to notify the user of the application fetching data, as well as,
* prevent them from changing the data while in mid-request/response.
*/
mojo.define("abc.controller.UI.LoadingScreenController", function ($) {

  var Controller = {
    methods: {
      Initialize: function () {
        mojo.Messaging.subscribe("UI.LoadingScreen", function (event, params) {

          if (params.target) {
            var blocker = jQuery(params.target).find(".UIBlock");
            if (!blocker.length) {
              blocker = jQuery(params.target).append("<div class='UIBlock'></div>");
              blocker = jQuery(params.target).find(".UIBlock");
            }
            if (params.show) {
              blocker.addClass("show");
            } else {
              blocker.removeClass("show");
            }
          } else {
            jQuery(document.body).find(".UIBlock").removeClass("show");
          }
        });
      }
    }
  };

  return Controller;

});