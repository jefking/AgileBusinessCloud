/* 
 * An aggregate graph for ABC
 */
(function($) {

  var AggregateGraph = function AggregateGraph(property, viewObj, dataObj, options) {

    if ('undefined' == typeof _ || !_) throw new Error("_ library missing.");
    if ('undefined' == typeof jQuery.plot || !jQuery.plot) throw new Error("plot library missing.");
    if (!property) throw new Error("property parameter is required.");
    if ('string' != typeof property) throw new Error("property parameter must be a string.");
    if (!viewObj) throw new Error("view parameter is required.");
    if (!dataObj) throw new Error("data parameter is required.");
    if (options && 'object' != typeof options) throw new Error("configuration object must be an object.");

    this.property = property;
    this.data = dataObj;
    this.view = $(viewObj);

    var defaults = {
      series: {
        points: { show: true }
      },
      grid: { hoverable: true, clickable: true },
      xaxis: { mode: "time", timeformat: "%d/%m/%y - %H:%M:%S" },
      //Set Min to 250, as that is the minimum threshold we support with logging
      yaxis: { min: 250, max: 50000 },
      legend: {
        show: true,
        backgroundColor: "#fff"
      }
    };

    this.options = $.extend({}, defaults, options);

    return this;
  };
  AggregateGraph.prototype.ConvertDate = function ConvertDate(value) {
    var ts = value.match(/\d/g).join("");
    var date = new Date();
    date.setTime(ts);
    return date;
  };
  AggregateGraph.prototype.Unique = function Unique(data) {
    var self = this;
    var columns = []
      , fullData = [];

    for (var i = 0; i < data.length; i++) {
      columns.push(data[i][self.property]);
    }

    columns = _.uniq(columns);
    $(columns).each(function (index, name) {

      var dataItem = { label: name };
      dataItem.data = [];
      $(data).each(function (i, item) {
        if (item[self.property] == name) {


          dataItem.label = item.Class + "<br>&rarr;" + name;
          var csharpdate = item.OccurredOn.match(/\d/g).join("");
          var itemDate = new Date();
          itemDate.setTime(csharpdate);

          dataItem.data.push([itemDate.getTime(), item.Duration.TotalMilliseconds]);
        }
      });
      fullData.push(dataItem);
    });

    return fullData;
  };
  AggregateGraph.prototype.Render = function Render() {
    var data = this.Unique(this.data);
    $.plot(this.view, data, this.options);
    return this;
  };

  if (!window.abc) window.abc = {};

  window.abc.AggregateGraph = AggregateGraph;

})(jQuery);