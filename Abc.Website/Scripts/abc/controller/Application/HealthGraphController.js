/* 
* Health Graph - Real Time Controller
*
* @author Agile Business Cloud Solutions Ltd.
* @namespace Application
*
* Provides an implementation silo for streaming data into the Smoothie graph.
*/
mojo.define("abc.controller.Application.HealthGraphController", function ($) {
  var Controller = {
    events: [],
    methods: {
      Initialize: function () {
        var context = $(this.getContextElement());
        var self = this;

        mojo.Messaging.publish("UI.RealTimeGraphLoadingScreen", { show: true, target: context });

        var abcHealthGraph = new SmoothieChart({
          grid: { strokeStyle: '#808080', lineWidth: 0, fillStyle: '#272727', millisPerLine: 1499, verticalSections: 2 },
          labels: { fillStyle: '#808080' }
        });

        abcHealthGraph.streamTo(document.getElementById("application-graph-data"));

        // Data
        var messagesLine = new TimeSeries()
        , errorsLine = new TimeSeries()
        , throughputLine = new TimeSeries()
        , performanceLine = new TimeSeries()
        , serverLine = new TimeSeries()
        , dataSets = [throughputLine, messagesLine, errorsLine, performanceLine, serverLine];

        // Display Console
        var messagesSpeed = context.find("li.messages span.speed")
        , errorsSpeed = context.find("li.errors span.speed")
        , throughputSpeed = context.find("li.throughput span.speed")
        , performanceSpeed = context.find("li.performance span.speed")
        , serverSpeed = context.find("li.eventlog span.speed");

        // Add to SmoothieChart
        abcHealthGraph.addTimeSeries(throughputLine, { strokeStyle: '#fff', lineWidth: 2 });
        abcHealthGraph.addTimeSeries(messagesLine, { strokeStyle: '#fa9c20', lineWidth: 1 });
        abcHealthGraph.addTimeSeries(errorsLine, { strokeStyle: '#ff0000', lineWidth: 1 });
        abcHealthGraph.addTimeSeries(performanceLine, { strokeStyle: '#608539', lineWidth: 1 });
        abcHealthGraph.addTimeSeries(serverLine, { strokeStyle: '#298db7', lineWidth: 1 });

        // Prime that Health Graph with Local Storage
        self.Prepare(dataSets);

        var streamed = [];

        setInterval(function () {
          if ($(window).scrollTop() > 250) {
            abcHealthGraph.stop();
            return;
          } else {
            abcHealthGraph.start();
          }
          ServiceLocator.getService("getCurrentThroughput").invoke({}, function (err, response) {
            mojo.Messaging.publish("UI.RealTimeGraphLoadingScreen", { show: false });
            if (!err) {
              var d = new Date().getTime(),
                messages = response.Messages,
                exceptions = response.Exceptions,
                performance = response.Performance,
                server = response.ServerStatistics,
                total = parseInt(messages, 10) + parseInt(exceptions, 10) + parseInt(performance, 10) + parseInt(server, 10) || 0;

              messagesLine.append(d, messages);
              errorsLine.append(d, exceptions);
              throughputLine.append(d, total);
              performanceLine.append(d, performance);
              serverLine.append(d, server);

              var instantaneous = [];
              instantaneous.push(messages);
              instantaneous.push(exceptions);
              instantaneous.push(total);
              instantaneous.push(performance);
              instantaneous.push(server);

              streamed.push(instantaneous);

              self.setLocalStorage(streamed);
            }
          });
        }, 1000);
      },
      getLocalStorage: function () {

        if (Modernizr.localstorage) {
          var timelines = $.parseJSON(localStorage.getItem("abc.HealthGraphData"));

          if (!timelines) timelines = [];

          return timelines;
        }
        return [];
      },
      setLocalStorage: function (json) {

        if (Modernizr.localstorage) {

          var queue = this.getLocalStorage();

          if (queue.length > 20) {
            localStorage.removeItem("abc.HealthGraphData");
          }
          if (queue.length == 20) {
            queue.shift();
          }

          queue.push(json);

          localStorage.setItem("abc.HealthGraphData", JSON.stringify(queue));
        }
        return false;
      },
      Prepare: function (dataSets) {
        var now = new Date().getTime();

        // Our queue is 20 data points containing 4 arrays (timelines)
        // With these 20 data points, we want to push these as
        // default values into the Health Graph

        for (var t = now - 1000 * 20; t <= now; t += 1000) {
          this.Fake(t, dataSets);
        }
      },
      Fake: function (time, dataSets) {
        for (var i = 0; i < dataSets.length; i++) {
          dataSets[i].append(time, 0);
        }
      }
    }
  }

  return Controller;
});