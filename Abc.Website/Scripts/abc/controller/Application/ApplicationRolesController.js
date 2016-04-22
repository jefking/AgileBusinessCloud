mojo.define("abc.controller.Application.ApplicationRolesController", function () {
  return {
    events: [
      ['context', 'li input[type=checkbox]', 'click', 'AddRole']
    ],
    methods: {
      AddRole: function (requestObj) {

        if (window.confirm("Are you sure you want to set this role?")) {
          var userId = $(requestObj.getCaller()).parent().parent().parent().attr('userId');
          ServiceLocator.getService("addUserRole").invoke({ userRole: { RoleName: requestObj.getCaller().value, UserIdentifier: userId} }, function (err, response) {
          });
          requestObj.getCaller().checked = true;
        } else {
          requestObj.getCaller().checked = false;
        }


      },
      IntegrateGravatar: function (results) {
        var integratedResults = [];
        $(results).each(function (i, item) {

          item['Image'] = "http://www.gravatar.com/avatar/" + MD5(item.Email || "");
          integratedResults.push(item);

        });
        return integratedResults;
      }
    },
    after: {
      Start: function () {
        var self = this;
        var context = $(this.getContextElement());
        mojo.Messaging.publish("UI.LoadingScreen", { target: context, show: true });
        ServiceLocator.getService("getUsers").invoke(null, function (err, data) {
          var data = { Users: self.IntegrateGravatar(data) };
          mojo.Model.set("abc.Application.Users.Roles", data);
          context.find("ul").show();
          mojo.Messaging.publish("UI.LoadingScreen", { show: false });
        });
      }
    }
  }
});