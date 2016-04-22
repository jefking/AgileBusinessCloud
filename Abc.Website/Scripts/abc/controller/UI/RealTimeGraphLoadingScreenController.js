/*
* @class RealTimeGraphLoadingScreenController
* @context .component
* 
* Provides a slicker UI interaction for the Real Time Graph on header
*/
mojo.define("abc.controller.UI.RealTimeGraphLoadingScreenController", function ($) {

  var Controller = {
    methods: {
      Initialize: function () {
        mojo.Messaging.subscribe("UI.RealTimeGraphLoadingScreen", function (event, params) {

          if (params.target) {
            var blocker = jQuery(params.target).find(".UI-RealTime-Block");
            if (!blocker.length) {
              blocker = jQuery(params.target).append("<div class='UI-RealTime-Block'></div>");
              blocker = jQuery(params.target).find(".UI-RealTime-Block");
            }
            if (params.show) {
              blocker.addClass("show");
            } else {
              blocker.removeClass("show");
            }
          } else {
            jQuery(document.body).find(".UI-RealTime-Block").removeClass("show");
          }
        });
      }
    }
  };

  return Controller;

});