/* 
* @class: Method Execution Controller
* @namespace: Application
* @author: Agile Business Cloud Solutions Ltd.
* 
* Responsible plotting an aggregate of long running method calls.
*/
mojo.define("abc.controller.Application.Performance.ClassListController", function ($) {

  var appId = localStorage.getItem("userAppId");
  var PublicKey = localStorage.getItem("appPublicKey");
  var Controller = {
    events: [
      ["context", ".class-title", "click", "Toggle"]
    ],
    methods: {
      Initialize: function () {
        var date = new Date();
        date.setDate(date.getDate());
        date.setHours(date.getHours() - 24, 0, 0, 0);
        this.Query({ from: date.toUTCString() });
      },
      Toggle: function (requestObj) {

        var container = $(requestObj.getCaller()).parent();
        container.find(".methods-wrapper").toggleClass("open");
      },
      Query: function (params) {
        params.application = appId;
        params.PublicKey = PublicKey;
        this.GetPerformanceData(params);
      },
      Render: function (data) {
        var self = this;
        var theBigList = self.param("PerformanceData");

        var view = (function (data) {
          for (var i = 0; i < data.length; i++) {
            var programmaticClassName = data[i].Class;
            var methods = [];
            data[i].ClassName = data[i].Class.replace(/\./g, "-");
            data[i].Total = (function (list) {
              var count = 0;

              for (var i = 0, len = list.length; i < len; i++) {
                if (programmaticClassName == list[i].Class) {

                  count++;

                  methods.push({
                    Name: list[i].Method,
                    RelativeDate: prettyDate(list[i].OccurredOn, true),
                    Duration: Math.round(list[i].Duration.TotalMilliseconds)
                  });
                }
              }
              return count;
            })(theBigList);

            data[i].Methods = methods;

          }
          return data;
        })(data);

        mojo.Model.set("abc.performance.ClassList", { Classes: view });

        (function () {
          for (var j = 0; j < data.length; j++) {

            var className = data[j].Class;
            var tmp = [];
            for (var i = 0; i < theBigList.length; i++) {

              if (theBigList[i].Class == className) {
                tmp.push(theBigList[i].Duration.TotalMilliseconds);
              }
            }
            className = className.replace(/\./g, "-");
            $("." + className).find("span.graph").text(tmp.join(","));
            $("." + className).find("span.graph").peity("line", { width: 425 });
          }
        })();

      },
      Process: function (key, data) {
        var self = this;
        var columns = [], fullData = [];

        for (var i = 0; i < data.length; i++) {
          columns.push(data[i][key]);
        }

        columns = _.uniq(columns);

        $(columns).each(function (index, item) {
          fullData.push({ Class: item });
        });
        return fullData;
      },
      GetPerformanceData: function (params) {
        var self = this;

        window.renderOccurrences = function (data) {
          if (data.Occurrences.length == 0) {
            data = [];
          } else {
            data = data.Occurrences;

          }


          var clonedResponse = _.clone(data);
          self.param("PerformanceData", clonedResponse);
          var d = self.Process("Class", data);
          self.Render(d);
        };

        ServiceLocator.getService("getApplicationPerformance").invoke(params, function (err, data) {
          mojo.Messaging.publish("UI.LoadingScreen", { show: false, target: self.getContextElement() });
        });
      }
    }
  };

  return Controller;
});