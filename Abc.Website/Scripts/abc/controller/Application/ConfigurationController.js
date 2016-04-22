/*globals ServiceLocator,mojo,Modernizr*/
/* 
* @class: Application Configuration Controller
* @namespace: Application
* @author: Agile Business Cloud
* @context: .component-application-settings
* 
* Responsible for handling cascading application configurations/settings.
*/
mojo.define("abc.controller.Application.ConfigurationController", {
  events: [
    ['context', 'a.btn-save', 'click', 'SaveSettings'],
    ['context', 'a.btn-key', 'click', 'SetEditFocus']
  ],
  methods: {
    SetEditFocus: function (requestObj) {
      requestObj.getEvent().preventDefault();


    },
    SaveSettings: function (requestObj) {
      var context = $(this.getContextElement());
      requestObj.getEvent().preventDefault();

      mojo.Messaging.publish("UI.LoadingScreen", {
        target: this.getContextElement(),
        show: true
      });

      var data = this.RetrieveSettings();

      //TODO: Switch to DOM lookup from backend
      var userSelectedAppId = localStorage.getItem("userAppId");

      if (!userSelectedAppId) {
        throw new Error("User has not selected an app id");
      }

      for (key in data) {
        var param = { Key: key, Value: data[key] };
        param.Token = { applicationId: userSelectedAppId };

        ServiceLocator.getService("addApplicationConfiguration").invoke({ configuration: param }, function (err, data) {
          var notice = context.find(".bar-notice");
          if (!err) {
            notice.addClass("notice-sucess");
            //use server message later!
            notice.text("Successfully applied new application configuration.");
          } else {
            notice.addClass("notice-error");
            notice.text("Failed to apply application configuration.");
          }
        });

      }

      setTimeout(function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: false });
      }, 2000);

    },
    RetrieveSettings: function () {
      var dataObj = {};

      $("div.config-info input", this.getContextElement()).each(function (index, input) {
        dataObj[$(input).attr('name')] = $(input).val();
      });
      return dataObj;
    },
    ParseXML: function (input) {
      var settings = $.xml2json(input);
      return settings;
    },
    Render: function (model) {
      mojo.Model.set("abc.application.settings", model);
      $(this.getContextElement()).find(".mojoTemplate").removeClass("mojoTemplate");
    },
    BuildConfig: function (arr) {

      var returnedObj = [];
      $(arr).each(function (i, obj) {
        var data = { Key: obj.name, Value: obj.value };
        returnedObj.push(data);

      });
      return returnedObj;
    },
    FixKeys: function (arr) {
      var obj = [];
      $(arr).each(function (i, item) {
        obj.push({ Key: item.key, Value: item.value });
      });

      return obj;
    },
    HandleFileSelect: function (event) {

      event.stopPropagation();
      event.preventDefault();

      var self = this;
      var context = $(event.target.parentNode.parentNode);
      var file = event.dataTransfer.files[0];


      if (file.type == "application/xml" || file.fileName.match(/.cscfg/gi)) {
        var isAzure = file.fileName.match(/.cscfg/gi);

        var reader = new FileReader();

        // Closure to capture the file information.
        reader.onload = (function (theFile) {
          return function (e) {

            var settings = self.ParseXML(e.target.result);

            if (isAzure) {
              settings.Configs = self.BuildConfig(settings.Role.ConfigurationSettings.Setting);
              settings.Configs.push({ Key: "Instances", Value: settings.Role.Instances.count });
            } else {

              settings.Configs = self.FixKeys(settings.appSettings.add);

            }

            $(".drop-zone-container", self.getContextElement()).fadeOut();
            $("textarea", self.getContextElement()).text(e.target.result).fadeIn();
            self.Render(settings);

          };
        })(file);

        reader.readAsText(file);
      } else {
        throw new Error("Invalid file format. Should be XML.");
      }

    },
    HandleDragOver: function (event) {
      event.stopPropagation();
      event.preventDefault();
    }
  },
  after: {
    Start: function () {
      var self = this;
      if (!Modernizr.draganddrop) {
        throw new Error("Drag and Drop is unsupported.");
      }
      var context = $(this.getContextElement());
      // Setup the dnd listeners.
      var dropZone = context.find(".drop-zone");
      dropZone[0].addEventListener('dragover', this.HandleDragOver.bind(this), false);
      dropZone[0].addEventListener('drop', this.HandleFileSelect.bind(this), false);


      ServiceLocator.getService("addGetApplicationConfiguration").invoke({ application: { Identifier: localStorage.getItem("userAppId")} }, function (err, response) {
        if (!err && response.length) {
          self.Render({ Configs: response });
        }
      });
    }
  }
});