/* 
* @class: Application Details Controller
* @namespace: Application
* @author: Agile Business Cloud
* 
* Responsible for handling the Application Details Form, including
* editing and creating applications.
*/
mojo.define("abc.controller.Application.ApplicationDetailsController", {
  events: [
    ['context', 'a.btn-save', 'click', 'SaveApplication'],
    ['context', '.generated-app-settings textarea', 'click', 'SelectAll']
  ],
  methods: {
    SelectAll: function (requestObj) {
      requestObj.getCaller().select();
    },
    SaveApplication: function (requestObj) {
      requestObj.getEvent().preventDefault();
      localStorage.clear();

      var messageBar = $(".bar-notice", this.getContextElement());
      if (!$("#Name").val().length) {
        messageBar.removeClass("notice-sucess").addClass("notice-error");
        messageBar.text("Application name is required.");
        messageBar.show();
        return;
      } else {
        messageBar.hide();
        messageBar.text("Successfully created application!");
        messageBar.addClass("notice-sucess").removeClass("notice-error");
      }

      var inputs = $("input[type=text], input[type=hidden], textarea, select", this.getContextElement());

      var params = { value: {} };

      $(inputs).each(function (index, element) {
        var pair = {};
        if (!$(element).attr("name")) return false;
        params.value[$(element).attr("name")] = $(element).val() || "";
      });

      params.value['Active'] = $("#Active").length ? $("#Active")[0].checked : true;
      params.value['IsValid'] = $("#IsValid").length ? $("#IsValid")[0].checked : true;
      params.value['Deleted'] = $("#Deleted").length ? $("#Deleted")[0].checked : false;

      mojo.Messaging.publish("UI.LoadingScreen", { target: $(".application-details-page"), show: true });

      ServiceLocator.getService("addApplication").invoke(params, function (err, data) {
        $(".notice-sucess").show();
        var appId = $("input[name='ApplicationId']").val();
        ServiceLocator.getService("addUserPreference").invoke({ preference: { CurrentApplication: { Identifier: appId}} }, function (err, data) {
          mojo.Messaging.publish("UI.LoadingScreen", { show: false });
          $(".application-details-page .generated-app-settings").fadeIn();
        });
      });
    }
  }
});