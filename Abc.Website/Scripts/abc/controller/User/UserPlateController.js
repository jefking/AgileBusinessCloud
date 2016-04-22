/*
 * @author Agile Business Cloud Solutions Ltd.
 * @namespace User
 * 
 * Provides responsibility over the User Plate component in the top navigation. 
 * In this case, the Email of the current user is hashed in MD5 and then
 * it slots in the css background url into the html container (span).
 */
mojo.define("abc.controller.User.UserPlateController", function ($) {
  var Controller = {
    methods: {
      Initialize: function () {
        var context = $(this.getContextElement());
        var email = context.find("span").data().email;
        var gravatar = "http://www.gravatar.com/avatar/" + MD5(email) + "?s=16";
        context.find("span").css("background", "transparent url(" + gravatar + ") center center no-repeat");
      }
    }
  };

  return Controller;
});