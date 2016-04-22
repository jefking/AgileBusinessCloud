/* 
* User Gallery Controller
* @author Agile Business Cloud Solutions Ltd.
* @namespace User
* 
* Provides an implementation silo for the gallery section of users. Just pictures
* of users on the site without exposing any information.
*/
mojo.define("abc.controller.User.UserGalleryController", function ($) {

  var Controller = {
    events: [],
    methods: {
      Gravatar: function (results) {
        var integratedResults = [];
        $(results).each(function (i, item) {
          item['Image'] = "http://www.gravatar.com/avatar/" + MD5(item.Email || "");
          integratedResults.push(item);
        });
        return integratedResults;
      },
      Initialize: function () {
        var self = this;
        var context = $(self.getContextElement());
        mojo.Messaging.publish("UI.LoadingScreen", { target: context, show: true });
        ServiceLocator.getService("getUsers").invoke(null, function (err, data) {
          var data = { Users: self.Gravatar(data) };
          mojo.Model.set("abc.User.Gallery", data);
          context.find("ul").show();
          mojo.Messaging.publish("UI.LoadingScreen", { show: false });
        });
      }
    }
  };

  return Controller;
});