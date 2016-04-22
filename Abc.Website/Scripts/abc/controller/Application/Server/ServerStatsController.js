/* 
* @class: Server Stats Controller
* @namespace: Application
* @author: Agile Business Cloud Solutions Ltd.
* 
* Responsible for displaying data for Server Stats
*/
mojo.define("abc.controller.Application.Server.ServerStatsController", function ($) {

  var appId = localStorage.getItem("userAppId");
  var Controller = {
    events: [],
    methods: {
      Initialize: function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        var self = this;
        var date = new Date();
        this.Query({ application: appId });
      },
      Query: function (params) {
        params.application = params.application;
        var date = new Date();
        date.setDate(date.getDate());
        date.setHours(date.getHours() - 24, 0, 0, 0);
        params.from = date;
        ServiceLocator.getService("getServerStatistics").invoke(params, this.Filter, this);
      },
      Filter: function (err, data) {
        var self = this;
        this.RenderTimelines(data);
      },
      GroupByMachine: function (data) {
        var grouped = [];
        var groups = {};
        for (var i = 0; i < data.length; i++) {
          groups[data[i].MachineName] = [];
        }

        _.keys(groups).forEach(function (k, index) {
          for (var i = 0; i < data.length; i++) {
            if (k == data[i].MachineName) {
              groups[k].push(data[i]);
            }
          }
        });
        return groups;
      },
      RenderTimelines: function (data) {

        var groupedData = this.GroupByMachine(data);
        for (guid in groupedData) {
          var vals = (function () {

            var values = {
              cpu: [], mem: [], io: []
            };
            for (var i = 0; i < groupedData[guid].length; i++) {
              values.cpu.push(groupedData[guid][i].CpuUsagePercentage);
              values.mem.push(groupedData[guid][i].MemoryUsagePercentage);
              values.io.push(groupedData[guid][i].PhysicalDiskUsagePercentage);
            }
            return values;
          })();
          var graphWidth = 675, max = 100, min = 0;
          var graphContainer = $("#" + guid);
          graphContainer.find("span.graph-cpu").text(vals.cpu.join(","));
          graphContainer.find("span.graph-cpu").peity("line", { width: graphWidth, colour: "#ccc", max: max, min: min });

          graphContainer.find("span.graph-mem").text(vals.mem.join(","));
          graphContainer.find("span.graph-mem").peity("line", { width: graphWidth, colour: "#ccc", max: max, min: min });

          graphContainer.find("span.graph-io").text(vals.io.join(","));
          graphContainer.find("span.graph-io").peity("line", { width: graphWidth, colour: "#ccc", max: max, min: min });

          mojo.Messaging.publish("UI.LoadingScreen", { show: false, target: this.getContextElement() });
        }
      }
    }
  };

  return Controller;
});