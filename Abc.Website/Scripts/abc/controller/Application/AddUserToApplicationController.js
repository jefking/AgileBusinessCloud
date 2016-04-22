/*globals ServiceLocator,mojo,Modernizr*/
/* 
* @class: Add User to Application Controller
* @namespace: Application
* @author: Agile Business Cloud
* 
* Responsible for handling and managing how users are added/removed to applications.
*/
mojo.define("abc.controller.Application.AddUserToApplicationController", function () {
  return {
    events: [
      ['context', 'li a', 'click', 'AddUser']
    ],
    methods: {
      AddUser: function (requestObj) {
        requestObj.getEvent().preventDefault();
        ServiceLocator.getService("addUserToApplication").invoke({ user: $(requestObj.getCaller()).attr('userId'), application: $("#ApplicationId").val() }, function (err, data) {
          $(requestObj.getCaller()).find("span").removeClass("ui-icon-circle-plus").addClass("ui-icon-check");
        });

      }
    },
    after: {
      Start: function () {
        ServiceLocator.getService("getUsers").invoke(null, function (err, data) {
          var data = { Users: data };
          mojo.Model.set('abc.Application.Users', data);
          $(".list-application-users").removeClass("mojoTemplate");
        });
      }
    }

  }
});