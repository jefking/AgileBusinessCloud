/* 
* @class: Method Execution Controller
* @namespace: Application
* @author: Agile Business Cloud Solutions Ltd.
* 
* Responsible plotting an aggregate of long running method calls.
*/
mojo.define("abc.controller.Application.Performance.MethodExecutionController", function ($) {

  var appId = localStorage.getItem("userAppId");
  var Controller = {
    events: [
      ["context", "a.btn-half-day", "click", "Filter"],
      ["context", "a.btn-today", "click", "Filter"],
      ["context", "a.btn-week", "click", "Filter"],
      ["context", "a.btn-three-week", "click", "Filter"],
      ["context", "a.btn-quarter", "click", "Filter"],
      ["context", "input[type=checkbox]", "click", "Toggle"]
    ],
    methods: {
      Initialize: function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        var self = this;

        var date = new Date();
        date.setDate(date.getDate());
        date.setHours(date.getHours() - 12, 0, 0, 0);
        self.Query({ from: date.toUTCString() });
        self.Activate($(self.getContextElement()).find(".btn-half-day"));

      },
      Toggle: function () {
        var model = this.param('PerformanceModel');

        var data = [];

        $(this.getContextElement()).find("input:checked").each(function () {
          var key = $(this).attr("name");
          for (var i = 0; i < model.length; i++) {
            if (key == model[i].Method) {
              data.push(model[i]);
            }
          }
        });

        if (!data.length) {
          alert("No data found.");
        }
        this.Render(data);
      },
      Query: function (params) {
        params.application = appId;
        this.GetPerformanceData(params);
      },
      Filter: function (requestObj) {
        requestObj.getEvent().preventDefault();
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });

        var caller = $(requestObj.getCaller());

        if (caller.hasClass("btn-today")) {
          this.Today(requestObj);
        } else if (caller.hasClass("btn-half-day")) {
          this.HalfDay(requestObj);
        } else if (caller.hasClass("btn-week")) {
          this.Week(requestObj);
        } else if (caller.hasClass("btn-three-week")) {
          this.ThreeWeek(requestObj);
        } else if (caller.hasClass("btn-quarter")) {
          this.Quarter(requestObj);
        }
      },
      Activate: function (target) {
        $(this.getContextElement()).find(".graph-navigation > li a").removeClass("active");
        $(target).addClass("active");
      },
      HalfDay: function (requestObj) {
        this.Activate(requestObj.getCaller());
        var date = new Date();
        date.setDate(date.getDate());
        date.setHours(date.getHours() - 12, 0, 0, 0);
        this.Query({ from: date.toUTCString() });
      },
      Today: function (requestObj) {
        this.Activate(requestObj.getCaller());
        var date = new Date();
        date.setDate(date.getDate() - 1);
        this.Query({ from: date.toUTCString() });
      },
      Week: function (requestObj) {
        this.Activate(requestObj.getCaller());
        var date = new Date();
        date.setDate(date.getDate() - 7);
        this.Query({ from: date.toUTCString() });
      },
      ThreeWeek: function (requestObj) {
        this.Activate(requestObj.getCaller());
        var date = new Date();
        date.setDate(date.getDate() - 21);
        this.Query({ from: date.toUTCString() });
      },
      Quarter: function (requestObsj) {
        var date = new Date();
        date.setMonth(date.getMonth() - 3);
        this.Query({ from: date.toUTCString() });
      },
      GetHighestValue: function (results) {
        var highest = null;

        $(results).each(function (index, item) {
          if (highest == null || highest < item.Duration.TotalMilliseconds) highest = item.Duration.TotalMilliseconds;
        });

        return highest;
      },
      Render: function (data) {
        var max = this.GetHighestValue(data);
        var padding = 5000;
        var legendContainer = $(this.getContextElement()).find(".legend-target");
        var options = { yaxis: { min: 0, max: max + padding || 10 }, legend: { container: legendContainer} };
        var view = $(".visualization", this.getContextElement());
        var graph = new abc.AggregateGraph("Method", view, data, options).Render();

      },
      ProcessMethodNames: function (key, data) {
        var self = this;
        var columns = [], fullData = [];

        for (var i = 0; i < data.length; i++) {
          columns.push(data[i][key]);
        }

        columns = _.uniq(columns);
        $(columns).each(function (index, name) {

          $(data).each(function (i, item) {
            if (item[key] == name) {
              fullData.push(item);
            }
          });

        });
        return fullData;
      },
      GetPerformanceData: function (params) {
        var self = this;
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        ServiceLocator.getService("getApplicationPerformance").invoke(params, function (err, data) {
          mojo.Messaging.publish("UI.LoadingScreen", { show: false });
          
          if (err || !data || data.length == 0) {

            $("#graph, #legend, .auxilary-commands, .graph-navigation", self.getContextElement()).hide();
            $(".error", self.getContextElement()).removeClass("hide");
            $(self.getContextElement()).removeClass("hide");
          } else {
            self.param('PerformanceModel', data);

            mojo.Model.set("abc.performance.MethodExecution", { MethodNames: self.ProcessMethodNames("Method", data) });
            self.Render(data);
            $(self.getContextElement()).removeClass("hide");
          }

        });
      }
    }
  };

  return Controller;
});