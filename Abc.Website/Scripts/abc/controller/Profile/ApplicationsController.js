/* 
* @class: Applications View Controller
* @namespace: Profile
* @author: Agile Business Cloud
* 
* Responsible fetching and rendering applications. Also handles caching / localStorage.
*/
mojo.define("abc.controller.Profile.ApplicationsController", function ($) {

  var Controller = {
    events: [
      ['context', 'ol.apps li a', 'click', 'SetDefaultApplication']
    ],
    methods: {
      Initialize: function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        ServiceLocator.getService("getUserApplications").invoke(null, this.Render);
      },
      Render: function (err, data) {
        var self = this;
        var data = { Applications: data };
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


        mojo.Model.set("abc.profile.Applications", grouped);
        mojo.Messaging.publish("UI.LoadingScreen", { show: false });
      },
      SetDefaultApplication: function (requestObj) {
        requestObj.getEvent().preventDefault();
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        ServiceLocator.getService("addUserPreference").invoke({ preference: { CurrentApplication: { Identifier: requestObj.getCaller().getAttribute('title')}} }, function (err, data) {
          window.location.reload(true);
        });
      }
    }
  };

  return Controller;
});
