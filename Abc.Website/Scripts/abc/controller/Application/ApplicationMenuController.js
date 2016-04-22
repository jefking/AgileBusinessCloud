/* 
* @class: Application Menu Controller
* @namespace: Application
* @author: Agile Business Cloud
* 
* Responsible fetching and rendering applications. Also handles caching / localStorage.
*/
mojo.define("abc.controller.Application.ApplicationMenuController", function ($) {

  var Controller = {
    events: [
      ['context', '.apps li.app a', 'click', 'SetDefaultApplication'],
      ['context', '.btn-refresh span', 'click', 'Refresh'],
      ['dom', 'li.btn-logout', 'click', 'Logout']
    ],
    methods: {
      Initialize: function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        ServiceLocator.getService("getUserApplications").invoke(null, this.Render, this);
      },
      Logout: function () {
        localStorage.clear();
      },
      Render: function (err, data) {
        var self = this;
        var activeData = function (records) {
          var arr = [];
          $(records).each(function (index, item) {
            if (item.Active && !item.Deleted) {

              if (!item.Name) {
                item.Name = item.Identifier.split('-')[0];
              }
              if (item.Name.length > 20) {
                item.Name = item.Name.substring(0, 20) + '...';
              }
              arr.push(item);
            }
          });

          return arr;
        } (data);
        var data = { Applications: activeData };

        var envs = ["Development", "Testing", "Staging", "Production"];

        var grouped = (function scan(results) {

          var Map = { Environments: [] };
          for (var i = 0; i < envs.length; i++) {
            var env = envs[i];
            var EnvironmentModel = { EnvironmentName: env, Apps: [] };
            $(results.Applications).each(function (index, app) {

              if (app.Environment === env) {
                EnvironmentModel.Apps.push(app);
              }
            });

            if (EnvironmentModel.Apps.length > 0) {
              Map.Environments.push(EnvironmentModel);
            }
          }
          return Map;
        })(data);

        localStorage.setItem('abc.menu.LastUpdate', new Date().getTime());
        localStorage.setItem('abc.menu.Applications', JSON.stringify(grouped));

        mojo.Model.set("abc.menu.Applications", grouped);
        mojo.Messaging.publish("UI.LoadingScreen", { show: false, target: self.getContextElement() });
      },
      Refresh: function (requestObj) {
        requestObj.getEvent().preventDefault();
        localStorage.clear();
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        ServiceLocator.getService("getUserApplications").invoke(null, this.Render, this);
      },
      SetDefaultApplication: function (requestObj) {
        requestObj.getEvent().preventDefault();
        var PublicKey = $(requestObj.getCaller()).data().publickey;
        console.log("Set Public Key: " + PublicKey);
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        localStorage.setItem("userAppId", requestObj.getCaller().getAttribute('title'));
        localStorage.setItem("appPublicKey", PublicKey);
        ServiceLocator.getService("addUserPreference").invoke({ preference: { CurrentApplication: { Identifier: requestObj.getCaller().getAttribute('title')}} }, function (err, data) {
          window.location.reload(true);
        });
      }
    }
  };

  return Controller;
});
