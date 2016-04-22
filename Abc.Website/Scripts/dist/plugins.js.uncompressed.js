// MIT License:
//
// Copyright (c) 2010-2011, Joe Walnes
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/**
* Smoothie Charts - http://smoothiecharts.org/
* (c) 2010, Joe Walnes
*
* v1.0: Main charting library, by Joe Walnes
* v1.1: Auto scaling of axis, by Neil Dunn
* v1.2: fps (frames per second) option, by Mathias Petterson
* v1.3: Fix for divide by zero, by Paul Nikitochkin
* v1.4: Set minimum, top-scale padding, remove timeseries, add optional timer to reset bounds, by Kelley Reynolds
* v1.5: Set default frames per second to 50... smoother.
*       .start(), .stop() methods for conserving CPU, by Dmitry Vyal
*       options.iterpolation = 'bezier' or 'line', by Dmitry Vyal
*       options.maxValue to fix scale, by Dmitry Vyal
*/
(function() {
  function TimeSeries(options) {
      options = options || {};
      options.resetBoundsInterval = options.resetBoundsInterval || 3000; // Reset the max/min bounds after this many milliseconds
      options.resetBounds = options.resetBounds || true; // Enable or disable the resetBounds timer
      this.options = options;
      this.data = [];

      this.maxValue = Number.NaN; // The maximum value ever seen in this time series.
      this.minValue = Number.NaN; // The minimum value ever seen in this time series.

      // Start a resetBounds Interval timer desired
      if (options.resetBounds) {
        this.boundsTimer = setInterval(function (thisObj) {
          try { thisObj.resetBounds(); } catch (e) { }
        }, options.resetBoundsInterval, this);
      }
  }

  // Reset the min and max for this timeseries so the graph rescales itself
  TimeSeries.prototype.resetBounds = function () {
      this.maxValue = Number.NaN;
      this.minValue = Number.NaN;
      for (var i = 0; i < this.data.length; i++) {
          this.maxValue = !isNaN(this.maxValue) ? Math.max(this.maxValue, this.data[i][1]) : this.data[i][1];
          this.minValue = !isNaN(this.minValue) ? Math.min(this.minValue, this.data[i][1]) : this.data[i][1];
      }
  };

  TimeSeries.prototype.append = function (timestamp, value) {
      this.data.push([timestamp, value]);
      this.maxValue = !isNaN(this.maxValue) ? Math.max(this.maxValue, value) : value;
      this.minValue = !isNaN(this.minValue) ? Math.min(this.minValue, value) : value;
  };

  function SmoothieChart(options) {
      // Defaults
      options = options || {};
      options.grid = options.grid || { fillStyle: '#000000', strokeStyle: '#777777', lineWidth: 1, millisPerLine: 1000, verticalSections: 2 };
      options.millisPerPixel = options.millisPerPixel || 20;
      options.fps = options.fps || 50;
      options.maxValueScale = options.maxValueScale || 1;
      options.minValue = options.minValue;
      options.maxValue = options.maxValue;
      options.labels = options.labels || { fillStyle: '#ffffff' };
      options.interpolation = options.interpolation || "bezier";
      this.options = options;
      this.seriesSet = [];
  }

  SmoothieChart.prototype.addTimeSeries = function (timeSeries, options) {
      this.seriesSet.push({ timeSeries: timeSeries, options: options || {} });
  };

  SmoothieChart.prototype.removeTimeSeries = function (timeSeries) {
      this.seriesSet.splice(this.seriesSet.indexOf(timeSeries), 1);
  };

  SmoothieChart.prototype.streamTo = function (canvas, delay) {
      var self = this;
      this.render_on_tick = function () {
          self.render(canvas, new Date().getTime() - (delay || 0));
      };

      this.start();
  };

  SmoothieChart.prototype.start = function () {
      if (!this.timer)
          this.timer = setInterval(this.render_on_tick, 1000 / this.options.fps);
  };

  SmoothieChart.prototype.stop = function () {
      if (this.timer) {
          clearInterval(this.timer);
          this.timer = undefined;
      }
  };

  SmoothieChart.prototype.render = function (canvas, time) {
      var canvasContext = canvas.getContext("2d");
      var options = this.options;
      var dimensions = { top: 0, left: 0, width: canvas.clientWidth, height: canvas.clientHeight };

      // Save the state of the canvas context, any transformations applied in this method
      // will get removed from the stack at the end of this method when .restore() is called.
      canvasContext.save();

      // Round time down to pixel granularity, so motion appears smoother.
      time = time - time % options.millisPerPixel;

      // Move the origin.
      canvasContext.translate(dimensions.left, dimensions.top);

      // Create a clipped rectangle - anything we draw will be constrained to this rectangle.
      // This prevents the occasional pixels from curves near the edges overrunning and creating
      // screen cheese (that phrase should neeed no explanation).
      canvasContext.beginPath();
      canvasContext.rect(0, 0, dimensions.width, dimensions.height);
      canvasContext.clip();

      // Clear the working area.
      canvasContext.save();
      canvasContext.fillStyle = options.grid.fillStyle;
      canvasContext.fillRect(0, 0, dimensions.width, dimensions.height);
      canvasContext.restore();

      // Grid lines....
      canvasContext.save();
      canvasContext.lineWidth = options.grid.lineWidth || 1;
      canvasContext.strokeStyle = options.grid.strokeStyle || '#ffffff';
      // Vertical (time) dividers.
      if (options.grid.millisPerLine > 0) {
          for (var t = time - (time % options.grid.millisPerLine); t >= time - (dimensions.width * options.millisPerPixel); t -= options.grid.millisPerLine) {
              canvasContext.beginPath();
              var gx = Math.round(dimensions.width - ((time - t) / options.millisPerPixel));
              canvasContext.moveTo(gx, 0);
              canvasContext.lineTo(gx, dimensions.height);
              canvasContext.stroke();
              canvasContext.closePath();
          }
      }

      // Horizontal (value) dividers.
      for (var v = 1; v < options.grid.verticalSections; v++) {
          var gy = Math.round(v * dimensions.height / options.grid.verticalSections);
          canvasContext.beginPath();
          canvasContext.moveTo(0, gy);
          canvasContext.lineTo(dimensions.width, gy);
          canvasContext.stroke();
          canvasContext.closePath();
      }
      // Bounding rectangle.
      canvasContext.beginPath();
      canvasContext.strokeRect(0, 0, dimensions.width, dimensions.height);
      canvasContext.closePath();
      canvasContext.restore();

      // Calculate the current scale of the chart, from all time series.
      var maxValue = Number.NaN;
      var minValue = Number.NaN;

      for (var d = 0; d < this.seriesSet.length; d++) {
          // TODO(ndunn): We could calculate / track these values as they stream in.
          var timeSeries = this.seriesSet[d].timeSeries;
          if (!isNaN(timeSeries.maxValue)) {
              maxValue = !isNaN(maxValue) ? Math.max(maxValue, timeSeries.maxValue) : timeSeries.maxValue;
          }

          if (!isNaN(timeSeries.minValue)) {
              minValue = !isNaN(minValue) ? Math.min(minValue, timeSeries.minValue) : timeSeries.minValue;
          }
      }

      if (isNaN(maxValue) && isNaN(minValue)) {
          return;
      }

      // Scale the maxValue to add padding at the top if required
      if (options.maxValue != null)
          maxValue = options.maxValue;
      else
          maxValue = maxValue * options.maxValueScale;
      // Set the minimum if we've specified one
      if (options.minValue != null)
          minValue = options.minValue;
      var valueRange = maxValue - minValue;

      // For each data set...
      for (var d = 0; d < this.seriesSet.length; d++) {
          canvasContext.save();
          var timeSeries = this.seriesSet[d].timeSeries;
          var dataSet = timeSeries.data;
          var seriesOptions = this.seriesSet[d].options;

          // Delete old data that's moved off the left of the chart.
          // We must always keep the last expired data point as we need this to draw the
          // line that comes into the chart, but any points prior to that can be removed.
          while (dataSet.length >= 2 && dataSet[1][0] < time - (dimensions.width * options.millisPerPixel)) {
              dataSet.splice(0, 1);
          }

          // Set style for this dataSet.
          canvasContext.lineWidth = seriesOptions.lineWidth || 1;
          canvasContext.fillStyle = seriesOptions.fillStyle;
          canvasContext.strokeStyle = seriesOptions.strokeStyle || '#ffffff';
          // Draw the line...
          canvasContext.beginPath();
          // Retain lastX, lastY for calculating the control points of bezier curves.
          var firstX = 0, lastX = 0, lastY = 0;
          for (var i = 0; i < dataSet.length; i++) {
              // TODO: Deal with dataSet.length < 2.
              var x = Math.round(dimensions.width - ((time - dataSet[i][0]) / options.millisPerPixel));
              var value = dataSet[i][1];
              var offset = maxValue - value;
              var scaledValue = valueRange ? Math.round((offset / valueRange) * dimensions.height) : 0;
              var y = Math.max(Math.min(scaledValue, dimensions.height - 1), 1); // Ensure line is always on chart.

              if (i == 0) {
                  firstX = x;
                  canvasContext.moveTo(x, y);
              }
              // Great explanation of Bezier curves: http://en.wikipedia.org/wiki/B�zier_curve#Quadratic_curves
              //
              // Assuming A was the last point in the line plotted and B is the new point,
              // we draw a curve with control points P and Q as below.
              //
              // A---P
              //     |
              //     |
              //     |
              //     Q---B
              //
              // Importantly, A and P are at the same y coordinate, as are B and Q. This is
              // so adjacent curves appear to flow as one.
              //
              else {
                  switch (options.interpolation) {
                      case "line":
                          canvasContext.lineTo(x, y);
                          break;
                      case "bezier":
                      default:
                          canvasContext.bezierCurveTo( // startPoint (A) is implicit from last iteration of loop
              Math.round((lastX + x) / 2), lastY, // controlPoint1 (P)
              Math.round((lastX + x)) / 2, y, // controlPoint2 (Q)
              x, y); // endPoint (B)
                          break;
                  }
              }

              lastX = x, lastY = y;
          }
          if (dataSet.length > 0 && seriesOptions.fillStyle) {
              // Close up the fill region.
              canvasContext.lineTo(dimensions.width + seriesOptions.lineWidth + 1, lastY);
              canvasContext.lineTo(dimensions.width + seriesOptions.lineWidth + 1, dimensions.height + seriesOptions.lineWidth + 1);
              canvasContext.lineTo(firstX, dimensions.height + seriesOptions.lineWidth);
              canvasContext.fill();
          }
          canvasContext.stroke();
          canvasContext.closePath();
          canvasContext.restore();
      }

      // Draw the axis values on the chart.
      if (!options.labels.disabled) {
          canvasContext.fillStyle = options.labels.fillStyle;
          var maxValueString = maxValue.toFixed(2);
          var minValueString = minValue.toFixed(2);
          canvasContext.fillText(maxValueString, dimensions.width - canvasContext.measureText(maxValueString).width - 2, 10);
          canvasContext.fillText(minValueString, dimensions.width - canvasContext.measureText(minValueString).width - 2, dimensions.height - 2);
      }

      canvasContext.restore(); // See .save() above.
  }
  
  window.TimeSeries = TimeSeries;
  window.SmoothieChart = SmoothieChart;
  
})();

// Peity jQuery plugin version 0.6.0
// (c) 2011 Ben Pickles
//
// http://benpickles.github.com/peity/
//
// Released under MIT license.
(function($, document) {
  var peity = $.fn.peity = function(type, options) {
    if (document.createElement("canvas").getContext) {
      this.each(function() {
        $(this).change(function() {
          var opts = $.extend({}, options)
          var self = this

          $.each(opts, function(name, value) {
            if ($.isFunction(value)) opts[name] = value.call(self)
          })

          var value = $(this).html();
          peity.graphers[type].call(this, $.extend({}, peity.defaults[type], opts));
          $(this).trigger("chart:changed", value);
        }).trigger("change");
      });
    }

    return this;
  };

  peity.graphers = {};
  peity.defaults = {};

  peity.add = function(type, defaults, grapher){
    peity.graphers[type] = grapher;
    peity.defaults[type] = defaults;
  };

  var devicePixelRatio = window.devicePixelRatio || 1

  function createCanvas(width, height) {
    var canvas = document.createElement("canvas")
    canvas.setAttribute("width", width * devicePixelRatio)
    canvas.setAttribute("height", height * devicePixelRatio)

    if (devicePixelRatio != 1) {
      var style = "width:" + width + "px;height:" + height + "px"
      canvas.setAttribute("style", style)
    }

    return canvas
  }

  peity.add(
    'pie',
    {
      colours: ['#FFF4DD', '#FF9900'],
      delimeter: '/',
      diameter: 16
    },
    function(opts) {
      var $this = $(this)
      var values = $this.text().split(opts.delimeter)
      var v1 = parseFloat(values[0]);
      var v2 = parseFloat(values[1]);
      var adjust = -Math.PI / 2;
      var slice = (v1 / v2) * Math.PI * 2;

      var canvas = createCanvas(opts.diameter, opts.diameter)
      var context = canvas.getContext("2d");
      var centre = canvas.width / 2;

      // Plate.
      context.beginPath();
      context.moveTo(centre, centre);
      context.arc(centre, centre, centre, slice + adjust, (slice == 0) ? Math.PI * 2 : adjust, false);
      context.fillStyle = opts.colours[0];
      context.fill();

      // Slice of pie.
      context.beginPath();
      context.moveTo(centre, centre);
      context.arc(centre, centre, centre, adjust, slice + adjust, false);
      context.fillStyle = opts.colours[1];
      context.fill();

      $this.wrapInner($("<span>").hide()).append(canvas)
  });

  peity.add(
    "line",
    {
      colour: "#c6d9fd",
      strokeColour: "#4d89f9",
      strokeWidth: 1,
      delimeter: ",",
      height: 16,
      max: null,
      min: 0,
      width: 32
    },
    function(opts) {
      var $this = $(this)
      var canvas = createCanvas(opts.width, opts.height)
      var values = $this.text().split(opts.delimeter)
      if (values.length == 1) values.push(values[0])
      var max = Math.max.apply(Math, values.concat([opts.max]));
      var min = Math.min.apply(Math, values.concat([opts.min]))

      var context = canvas.getContext("2d");
      var width = canvas.width
      var height = canvas.height
      var xQuotient = width / (values.length - 1)
      var yQuotient = height / (max - min)

      var coords = [];
      var i;

      context.beginPath();
      context.moveTo(0, height + (min * yQuotient))

      for (i = 0; i < values.length; i++) {
        var x = i * xQuotient
        var y = height - (yQuotient * (values[i] - min))

        coords.push({ x: x, y: y });
        context.lineTo(x, y);
      }

      context.lineTo(width, height + (min * yQuotient))
      context.fillStyle = opts.colour;
      context.fill();

      if (opts.strokeWidth) {
        context.beginPath();
        context.moveTo(0, coords[0].y);
        for (i = 0; i < coords.length; i++) {
          context.lineTo(coords[i].x, coords[i].y);
        }
        context.lineWidth = opts.strokeWidth * devicePixelRatio;
        context.strokeStyle = opts.strokeColour;
        context.stroke();
      }

      $this.wrapInner($("<span>").hide()).append(canvas)
    }
  );

  peity.add(
    'bar',
    {
      colour: "#4D89F9",
      delimeter: ",",
      height: 16,
      max: null,
      min: 0,
      width: 32
    },
    function(opts) {
      var $this = $(this)
      var values = $this.text().split(opts.delimeter)
      var max = Math.max.apply(Math, values.concat([opts.max]));
      var min = Math.min.apply(Math, values.concat([opts.min]))

      var canvas = createCanvas(opts.width, opts.height)
      var context = canvas.getContext("2d");

      var width = canvas.width
      var height = canvas.height
      var yQuotient = height / (max - min)
      var space = devicePixelRatio / 2
      var xQuotient = (width + space) / values.length

      context.fillStyle = opts.colour;

      for (var i = 0; i < values.length; i++) {
        var x = i * xQuotient
        var y = height - (yQuotient * (values[i] - min))

        context.fillRect(x, y, xQuotient - space, yQuotient * values[i])
      }

      $this.wrapInner($("<span>").hide()).append(canvas)
    }
  );
})(jQuery, document);

/*
 * zClip :: jQuery ZeroClipboard v1.1.1
 * http://steamdev.com/zclip
 *
 * Copyright 2011, SteamDev
 * Released under the MIT license.
 * http://www.opensource.org/licenses/mit-license.php
 *
 * Date: Wed Jun 01, 2011
 */


(function ($) {

    $.fn.zclip = function (params) {

        if (typeof params == "object" && !params.length) {

            var settings = $.extend({

                path: 'ZeroClipboard.swf',
                copy: null,
                beforeCopy: null,
                afterCopy: null,
                clickAfter: true,
                setHandCursor: true,
                setCSSEffects: true

            }, params);
			

            return this.each(function () {

                var o = $(this);

                if (o.is(':visible') && (typeof settings.copy == 'string' || $.isFunction(settings.copy))) {

                    ZeroClipboard.setMoviePath(settings.path);
                    var clip = new ZeroClipboard.Client();
                    
                    if($.isFunction(settings.copy)){
                    	o.bind('zClip_copy',settings.copy);
                    }
                    if($.isFunction(settings.beforeCopy)){
                    	o.bind('zClip_beforeCopy',settings.beforeCopy);
                    }
                    if($.isFunction(settings.afterCopy)){
                    	o.bind('zClip_afterCopy',settings.afterCopy);
                    }                    

                    clip.setHandCursor(settings.setHandCursor);
                    clip.setCSSEffects(settings.setCSSEffects);
                    clip.addEventListener('mouseOver', function (client) {
                        o.trigger('mouseenter');
                    });
                    clip.addEventListener('mouseOut', function (client) {
                        o.trigger('mouseleave');
                    });
                    clip.addEventListener('mouseDown', function (client) {

                        o.trigger('mousedown');
                        
			if(!$.isFunction(settings.copy)){
			   clip.setText(settings.copy);
			} else {
			   clip.setText(o.triggerHandler('zClip_copy'));
			}                        
                        
                        if ($.isFunction(settings.beforeCopy)) {
                            o.trigger('zClip_beforeCopy');                            
                        }

                    });

                    clip.addEventListener('complete', function (client, text) {

                        if ($.isFunction(settings.afterCopy)) {
                            
                            o.trigger('zClip_afterCopy');

                        } else {
                            if (text.length > 500) {
                                text = text.substr(0, 500) + "...\n\n(" + (text.length - 500) + " characters not shown)";
                            }
							
			    o.removeClass('hover');
                            alert("Copied text to clipboard:\n\n " + text);
                        }

                        if (settings.clickAfter) {
                            o.trigger('click');
                        }

                    });

					
                    clip.glue(o[0], o.parent()[0]);
					
		    $(window).bind('load resize',function(){clip.reposition();});
					

                }

            });

        } else if (typeof params == "string") {

            return this.each(function () {

                var o = $(this);

                params = params.toLowerCase();
                var zclipId = o.data('zclipId');
                var clipElm = $('#' + zclipId + '.zclip');

                if (params == "remove") {

                    clipElm.remove();
                    o.removeClass('active hover');

                } else if (params == "hide") {

                    clipElm.hide();
                    o.removeClass('active hover');

                } else if (params == "show") {

                    clipElm.show();

                }

            });

        }

    }	
	
	

})(jQuery);







// ZeroClipboard
// Simple Set Clipboard System
// Author: Joseph Huckaby
var ZeroClipboard = {

    version: "1.0.7",
    clients: {},
    // registered upload clients on page, indexed by id
    moviePath: 'ZeroClipboard.swf',
    // URL to movie
    nextId: 1,
    // ID of next movie
    $: function (thingy) {
        // simple DOM lookup utility function
        if (typeof(thingy) == 'string') thingy = document.getElementById(thingy);
        if (!thingy.addClass) {
            // extend element with a few useful methods
            thingy.hide = function () {
                this.style.display = 'none';
            };
            thingy.show = function () {
                this.style.display = '';
            };
            thingy.addClass = function (name) {
                this.removeClass(name);
                this.className += ' ' + name;
            };
            thingy.removeClass = function (name) {
                var classes = this.className.split(/\s+/);
                var idx = -1;
                for (var k = 0; k < classes.length; k++) {
                    if (classes[k] == name) {
                        idx = k;
                        k = classes.length;
                    }
                }
                if (idx > -1) {
                    classes.splice(idx, 1);
                    this.className = classes.join(' ');
                }
                return this;
            };
            thingy.hasClass = function (name) {
                return !!this.className.match(new RegExp("\\s*" + name + "\\s*"));
            };
        }
        return thingy;
    },

    setMoviePath: function (path) {
        // set path to ZeroClipboard.swf
        this.moviePath = path;
    },

    dispatch: function (id, eventName, args) {
        // receive event from flash movie, send to client		
        var client = this.clients[id];
        if (client) {
            client.receiveEvent(eventName, args);
        }
    },

    register: function (id, client) {
        // register new client to receive events
        this.clients[id] = client;
    },

    getDOMObjectPosition: function (obj, stopObj) {
        // get absolute coordinates for dom element
        var info = {
            left: 0,
            top: 0,
            width: obj.width ? obj.width : obj.offsetWidth,
            height: obj.height ? obj.height : obj.offsetHeight
        };

        if (obj && (obj != stopObj)) {
			info.left += obj.offsetLeft;
            info.top += obj.offsetTop;
        }

        return info;
    },

    Client: function (elem) {
        // constructor for new simple upload client
        this.handlers = {};

        // unique ID
        this.id = ZeroClipboard.nextId++;
        this.movieId = 'ZeroClipboardMovie_' + this.id;

        // register client with singleton to receive flash events
        ZeroClipboard.register(this.id, this);

        // create movie
        if (elem) this.glue(elem);
    }
};

ZeroClipboard.Client.prototype = {

    id: 0,
    // unique ID for us
    ready: false,
    // whether movie is ready to receive events or not
    movie: null,
    // reference to movie object
    clipText: '',
    // text to copy to clipboard
    handCursorEnabled: true,
    // whether to show hand cursor, or default pointer cursor
    cssEffects: true,
    // enable CSS mouse effects on dom container
    handlers: null,
    // user event handlers
    glue: function (elem, appendElem, stylesToAdd) {
        // glue to DOM element
        // elem can be ID or actual DOM element object
        this.domElement = ZeroClipboard.$(elem);

        // float just above object, or zIndex 99 if dom element isn't set
        var zIndex = 99;
        if (this.domElement.style.zIndex) {
            zIndex = parseInt(this.domElement.style.zIndex, 10) + 1;
        }

        if (typeof(appendElem) == 'string') {
            appendElem = ZeroClipboard.$(appendElem);
        } else if (typeof(appendElem) == 'undefined') {
            appendElem = document.getElementsByTagName('body')[0];
        }

        // find X/Y position of domElement
        var box = ZeroClipboard.getDOMObjectPosition(this.domElement, appendElem);

        // create floating DIV above element
        this.div = document.createElement('div');
        this.div.className = "zclip";
        this.div.id = "zclip-" + this.movieId;
        $(this.domElement).data('zclipId', 'zclip-' + this.movieId);
        var style = this.div.style;
        style.position = 'absolute';
        style.left = '' + box.left + 'px';
        style.top = '' + box.top + 'px';
        style.width = '' + box.width + 'px';
        style.height = '' + box.height + 'px';
        style.zIndex = zIndex;

        if (typeof(stylesToAdd) == 'object') {
            for (addedStyle in stylesToAdd) {
                style[addedStyle] = stylesToAdd[addedStyle];
            }
        }

        // style.backgroundColor = '#f00'; // debug
        appendElem.appendChild(this.div);

        this.div.innerHTML = this.getHTML(box.width, box.height);
    },

    getHTML: function (width, height) {
        // return HTML for movie
        var html = '';
        var flashvars = 'id=' + this.id + '&width=' + width + '&height=' + height;

        if (navigator.userAgent.match(/MSIE/)) {
            // IE gets an OBJECT tag
            var protocol = location.href.match(/^https/i) ? 'https://' : 'http://';
            html += '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="' + protocol + 'download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" width="' + width + '" height="' + height + '" id="' + this.movieId + '" align="middle"><param name="allowScriptAccess" value="always" /><param name="allowFullScreen" value="false" /><param name="movie" value="' + ZeroClipboard.moviePath + '" /><param name="loop" value="false" /><param name="menu" value="false" /><param name="quality" value="best" /><param name="bgcolor" value="#ffffff" /><param name="flashvars" value="' + flashvars + '"/><param name="wmode" value="transparent"/></object>';
        } else {
            // all other browsers get an EMBED tag
            html += '<embed id="' + this.movieId + '" src="' + ZeroClipboard.moviePath + '" loop="false" menu="false" quality="best" bgcolor="#ffffff" width="' + width + '" height="' + height + '" name="' + this.movieId + '" align="middle" allowScriptAccess="always" allowFullScreen="false" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" flashvars="' + flashvars + '" wmode="transparent" />';
        }
        return html;
    },

    hide: function () {
        // temporarily hide floater offscreen
        if (this.div) {
            this.div.style.left = '-2000px';
        }
    },

    show: function () {
        // show ourselves after a call to hide()
        this.reposition();
    },

    destroy: function () {
        // destroy control and floater
        if (this.domElement && this.div) {
            this.hide();
            this.div.innerHTML = '';

            var body = document.getElementsByTagName('body')[0];
            try {
                body.removeChild(this.div);
            } catch (e) {;
            }

            this.domElement = null;
            this.div = null;
        }
    },

    reposition: function (elem) {
        // reposition our floating div, optionally to new container
        // warning: container CANNOT change size, only position
        if (elem) {
            this.domElement = ZeroClipboard.$(elem);
            if (!this.domElement) this.hide();
        }

        if (this.domElement && this.div) {
            var box = ZeroClipboard.getDOMObjectPosition(this.domElement);
            var style = this.div.style;
            style.left = '' + box.left + 'px';
            style.top = '' + box.top + 'px';
        }
    },

    setText: function (newText) {
        // set text to be copied to clipboard
        this.clipText = newText;
        if (this.ready) {
            this.movie.setText(newText);
        }
    },

    addEventListener: function (eventName, func) {
        // add user event listener for event
        // event types: load, queueStart, fileStart, fileComplete, queueComplete, progress, error, cancel
        eventName = eventName.toString().toLowerCase().replace(/^on/, '');
        if (!this.handlers[eventName]) {
            this.handlers[eventName] = [];
        }
        this.handlers[eventName].push(func);
    },

    setHandCursor: function (enabled) {
        // enable hand cursor (true), or default arrow cursor (false)
        this.handCursorEnabled = enabled;
        if (this.ready) {
            this.movie.setHandCursor(enabled);
        }
    },

    setCSSEffects: function (enabled) {
        // enable or disable CSS effects on DOM container
        this.cssEffects = !! enabled;
    },

    receiveEvent: function (eventName, args) {
        // receive event from flash
        eventName = eventName.toString().toLowerCase().replace(/^on/, '');

        // special behavior for certain events
        switch (eventName) {
        case 'load':
            // movie claims it is ready, but in IE this isn't always the case...
            // bug fix: Cannot extend EMBED DOM elements in Firefox, must use traditional function
            this.movie = document.getElementById(this.movieId);
            if (!this.movie) {
                var self = this;
                setTimeout(function () {
                    self.receiveEvent('load', null);
                }, 1);
                return;
            }

            // firefox on pc needs a "kick" in order to set these in certain cases
            if (!this.ready && navigator.userAgent.match(/Firefox/) && navigator.userAgent.match(/Windows/)) {
                var self = this;
                setTimeout(function () {
                    self.receiveEvent('load', null);
                }, 100);
                this.ready = true;
                return;
            }

            this.ready = true;
            try {
                this.movie.setText(this.clipText);
            } catch (e) {}
            try {
                this.movie.setHandCursor(this.handCursorEnabled);
            } catch (e) {}
            break;

        case 'mouseover':
            if (this.domElement && this.cssEffects) {
                this.domElement.addClass('hover');
                if (this.recoverActive) {
                    this.domElement.addClass('active');
                }


            }


            break;

        case 'mouseout':
            if (this.domElement && this.cssEffects) {
                this.recoverActive = false;
                if (this.domElement.hasClass('active')) {
                    this.domElement.removeClass('active');
                    this.recoverActive = true;
                }
                this.domElement.removeClass('hover');

            }
            break;

        case 'mousedown':
            if (this.domElement && this.cssEffects) {
                this.domElement.addClass('active');
            }
            break;

        case 'mouseup':
            if (this.domElement && this.cssEffects) {
                this.domElement.removeClass('active');
                this.recoverActive = false;
            }
            break;
        } // switch eventName
        if (this.handlers[eventName]) {
            for (var idx = 0, len = this.handlers[eventName].length; idx < len; idx++) {
                var func = this.handlers[eventName][idx];

                if (typeof(func) == 'function') {
                    // actual function reference
                    func(this, args);
                } else if ((typeof(func) == 'object') && (func.length == 2)) {
                    // PHP style object + method, i.e. [myObject, 'myMethod']
                    func[0][func[1]](this, args);
                } else if (typeof(func) == 'string') {
                    // name of function
                    window[func](this, args);
                }
            } // foreach event handler defined
        } // user defined handler for event
    }

};	



/*
* JavaScript Pretty Date
* Copyright (c) 2008 John Resig (jquery.com)
* Licensed under the MIT license.
* Modified for UTC
*/
function prettyDate(date, fix) {
  // duckpunch for C# date times to human relative dates
  if (fix) {
    date = eval('new ' + date.replace(/\//g, ''));
  }

  //modification to prettyDate function
  //- assume: param 'date' is now utc from backend
  //- wrap better checking around nulls

  if ('undefined' == typeof date || !date) return "";

  var now = new Date();
  var utc = new Date(now.getUTCFullYear(),
                      now.getUTCMonth(),
                      now.getUTCDate(),
                      now.getUTCHours(),
                      now.getUTCMinutes(),
                      now.getUTCSeconds());

  //dateOffset = 0;
  dateOffset = date.getTimezoneOffset() * 60000;
  utcOffset = 0;
  //utcOffset = utc.getTimezoneOffset() * 60000;
  diff = (((utc.getTime() + utcOffset) - (date.getTime() + dateOffset)) / 1000),
  day_diff = Math.floor(diff / 86400);

  if (isNaN(day_diff) || day_diff < 0 || day_diff >= 365) {
    return "recently";
  }

  return day_diff == 0 && (
			diff < 60 && "just now" ||
			diff < 120 && "1 minute ago" ||
			diff < 3600 && Math.floor(diff / 60) + " minutes ago" ||
			diff < 7200 && "1 hour ago" ||
			diff < 86400 && Math.floor(diff / 3600) + " hours ago") ||
		day_diff == 1 && "yesterday" ||
		day_diff < 7 && day_diff + " days ago" ||
        day_diff < 14 && "1 week ago" ||
		day_diff < 365 && Math.ceil(day_diff / 7) + " weeks ago";
}


/*! Javascript plotting library for jQuery, v. 0.7.
* 
* Released under the MIT license by IOLA, December 2007.
*
*/

// first an inline dependency, jquery.colorhelpers.js, we inline it here
// for convenience

/* Plugin for jQuery for working with colors.
* 
* Version 1.1.
* 
* Inspiration from jQuery color animation plugin by John Resig.
*
* Released under the MIT license by Ole Laursen, October 2009.
*
* Examples:
*
*   $.color.parse("#fff").scale('rgb', 0.25).add('a', -0.5).toString()
*   var c = $.color.extract($("#mydiv"), 'background-color');
*   console.log(c.r, c.g, c.b, c.a);
*   $.color.make(100, 50, 25, 0.4).toString() // returns "rgba(100,50,25,0.4)"
*
* Note that .scale() and .add() return the same modified object
* instead of making a new one.
*
* V. 1.1: Fix error handling so e.g. parsing an empty string does
* produce a color rather than just crashing.
*/
(function (B) { B.color = {}; B.color.make = function (F, E, C, D) { var G = {}; G.r = F || 0; G.g = E || 0; G.b = C || 0; G.a = D != null ? D : 1; G.add = function (J, I) { for (var H = 0; H < J.length; ++H) { G[J.charAt(H)] += I } return G.normalize() }; G.scale = function (J, I) { for (var H = 0; H < J.length; ++H) { G[J.charAt(H)] *= I } return G.normalize() }; G.toString = function () { if (G.a >= 1) { return "rgb(" + [G.r, G.g, G.b].join(",") + ")" } else { return "rgba(" + [G.r, G.g, G.b, G.a].join(",") + ")" } }; G.normalize = function () { function H(J, K, I) { return K < J ? J : (K > I ? I : K) } G.r = H(0, parseInt(G.r), 255); G.g = H(0, parseInt(G.g), 255); G.b = H(0, parseInt(G.b), 255); G.a = H(0, G.a, 1); return G }; G.clone = function () { return B.color.make(G.r, G.b, G.g, G.a) }; return G.normalize() }; B.color.extract = function (D, C) { var E; do { E = D.css(C).toLowerCase(); if (E != "" && E != "transparent") { break } D = D.parent() } while (!B.nodeName(D.get(0), "body")); if (E == "rgba(0, 0, 0, 0)") { E = "transparent" } return B.color.parse(E) }; B.color.parse = function (F) { var E, C = B.color.make; if (E = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(F)) { return C(parseInt(E[1], 10), parseInt(E[2], 10), parseInt(E[3], 10)) } if (E = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]+(?:\.[0-9]+)?)\s*\)/.exec(F)) { return C(parseInt(E[1], 10), parseInt(E[2], 10), parseInt(E[3], 10), parseFloat(E[4])) } if (E = /rgb\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*\)/.exec(F)) { return C(parseFloat(E[1]) * 2.55, parseFloat(E[2]) * 2.55, parseFloat(E[3]) * 2.55) } if (E = /rgba\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\s*\)/.exec(F)) { return C(parseFloat(E[1]) * 2.55, parseFloat(E[2]) * 2.55, parseFloat(E[3]) * 2.55, parseFloat(E[4])) } if (E = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/.exec(F)) { return C(parseInt(E[1], 16), parseInt(E[2], 16), parseInt(E[3], 16)) } if (E = /#([a-fA-F0-9])([a-fA-F0-9])([a-fA-F0-9])/.exec(F)) { return C(parseInt(E[1] + E[1], 16), parseInt(E[2] + E[2], 16), parseInt(E[3] + E[3], 16)) } var D = B.trim(F).toLowerCase(); if (D == "transparent") { return C(255, 255, 255, 0) } else { E = A[D] || [0, 0, 0]; return C(E[0], E[1], E[2]) } }; var A = { aqua: [0, 255, 255], azure: [240, 255, 255], beige: [245, 245, 220], black: [0, 0, 0], blue: [0, 0, 255], brown: [165, 42, 42], cyan: [0, 255, 255], darkblue: [0, 0, 139], darkcyan: [0, 139, 139], darkgrey: [169, 169, 169], darkgreen: [0, 100, 0], darkkhaki: [189, 183, 107], darkmagenta: [139, 0, 139], darkolivegreen: [85, 107, 47], darkorange: [255, 140, 0], darkorchid: [153, 50, 204], darkred: [139, 0, 0], darksalmon: [233, 150, 122], darkviolet: [148, 0, 211], fuchsia: [255, 0, 255], gold: [255, 215, 0], green: [0, 128, 0], indigo: [75, 0, 130], khaki: [240, 230, 140], lightblue: [173, 216, 230], lightcyan: [224, 255, 255], lightgreen: [144, 238, 144], lightgrey: [211, 211, 211], lightpink: [255, 182, 193], lightyellow: [255, 255, 224], lime: [0, 255, 0], magenta: [255, 0, 255], maroon: [128, 0, 0], navy: [0, 0, 128], olive: [128, 128, 0], orange: [255, 165, 0], pink: [255, 192, 203], purple: [128, 0, 128], violet: [128, 0, 128], red: [255, 0, 0], silver: [192, 192, 192], white: [255, 255, 255], yellow: [255, 255, 0]} })(jQuery);

// the actual Flot code
(function ($) {
  function Plot(placeholder, data_, options_, plugins) {
    // data is on the form:
    //   [ series1, series2 ... ]
    // where series is either just the data as [ [x1, y1], [x2, y2], ... ]
    // or { data: [ [x1, y1], [x2, y2], ... ], label: "some label", ... }

    var series = [],
            options = {
              // the color theme used for graphs
              colors: ["#edc240", "#afd8f8", "#cb4b4b", "#4da74d", "#9440ed"],
              legend: {
                show: true,
                noColumns: 1, // number of colums in legend table
                labelFormatter: null, // fn: string -> string
                labelBoxBorderColor: "#ccc", // border color for the little label boxes
                container: null, // container (as jQuery object) to put legend in, null means default on top of graph
                position: "ne", // position of default legend container within plot
                margin: 5, // distance from grid edge to default legend container within plot
                backgroundColor: null, // null means auto-detect
                backgroundOpacity: 0.85 // set to 0 to avoid background
              },
              xaxis: {
                show: null, // null = auto-detect, true = always, false = never
                position: "bottom", // or "top"
                mode: null, // null or "time"
                color: null, // base color, labels, ticks
                tickColor: null, // possibly different color of ticks, e.g. "rgba(0,0,0,0.15)"
                transform: null, // null or f: number -> number to transform axis
                inverseTransform: null, // if transform is set, this should be the inverse function
                min: null, // min. value to show, null means set automatically
                max: null, // max. value to show, null means set automatically
                autoscaleMargin: null, // margin in % to add if auto-setting min/max
                ticks: null, // either [1, 3] or [[1, "a"], 3] or (fn: axis info -> ticks) or app. number of ticks for auto-ticks
                tickFormatter: null, // fn: number -> string
                labelWidth: null, // size of tick labels in pixels
                labelHeight: null,
                reserveSpace: null, // whether to reserve space even if axis isn't shown
                tickLength: null, // size in pixels of ticks, or "full" for whole line
                alignTicksWithAxis: null, // axis number or null for no sync

                // mode specific options
                tickDecimals: null, // no. of decimals, null means auto
                tickSize: null, // number or [number, "unit"]
                minTickSize: null, // number or [number, "unit"]
                monthNames: null, // list of names of months
                timeformat: null, // format string to use
                twelveHourClock: false // 12 or 24 time in time mode
              },
              yaxis: {
                autoscaleMargin: 0.02,
                position: "left" // or "right"
              },
              xaxes: [],
              yaxes: [],
              series: {
                points: {
                  show: false,
                  radius: 3,
                  lineWidth: 2, // in pixels
                  fill: true,
                  fillColor: "#ffffff",
                  symbol: "circle" // or callback
                },
                lines: {
                  // we don't put in show: false so we can see
                  // whether lines were actively disabled 
                  lineWidth: 2, // in pixels
                  fill: false,
                  fillColor: null,
                  steps: false
                },
                bars: {
                  show: false,
                  lineWidth: 2, // in pixels
                  barWidth: 1, // in units of the x axis
                  fill: true,
                  fillColor: null,
                  align: "left", // or "center" 
                  horizontal: false
                },
                shadowSize: 3
              },
              grid: {
                show: true,
                aboveData: false,
                color: "#545454", // primary color used for outline and labels
                backgroundColor: null, // null for transparent, else color
                borderColor: null, // set if different from the grid color
                tickColor: null, // color for the ticks, e.g. "rgba(0,0,0,0.15)"
                labelMargin: 5, // in pixels
                axisMargin: 8, // in pixels
                borderWidth: 2, // in pixels
                minBorderMargin: null, // in pixels, null means taken from points radius
                markings: null, // array of ranges or fn: axes -> array of ranges
                markingsColor: "#f4f4f4",
                markingsLineWidth: 2,
                // interactive stuff
                clickable: false,
                hoverable: false,
                autoHighlight: true, // highlight in case mouse is near
                mouseActiveRadius: 10 // how far the mouse can be away to activate an item
              },
              hooks: {}
            },
        canvas = null,      // the canvas for the plot itself
        overlay = null,     // canvas for interactive stuff on top of plot
        eventHolder = null, // jQuery object that events should be bound to
        ctx = null, octx = null,
        xaxes = [], yaxes = [],
        plotOffset = { left: 0, right: 0, top: 0, bottom: 0 },
        canvasWidth = 0, canvasHeight = 0,
        plotWidth = 0, plotHeight = 0,
        hooks = {
          processOptions: [],
          processRawData: [],
          processDatapoints: [],
          drawSeries: [],
          draw: [],
          bindEvents: [],
          drawOverlay: [],
          shutdown: []
        },
        plot = this;

    // public functions
    plot.setData = setData;
    plot.setupGrid = setupGrid;
    plot.draw = draw;
    plot.getPlaceholder = function () { return placeholder; };
    plot.getCanvas = function () { return canvas; };
    plot.getPlotOffset = function () { return plotOffset; };
    plot.width = function () { return plotWidth; };
    plot.height = function () { return plotHeight; };
    plot.offset = function () {
      var o = eventHolder.offset();
      o.left += plotOffset.left;
      o.top += plotOffset.top;
      return o;
    };
    plot.getData = function () { return series; };
    plot.getAxes = function () {
      var res = {}, i;
      $.each(xaxes.concat(yaxes), function (_, axis) {
        if (axis)
          res[axis.direction + (axis.n != 1 ? axis.n : "") + "axis"] = axis;
      });
      return res;
    };
    plot.getXAxes = function () { return xaxes; };
    plot.getYAxes = function () { return yaxes; };
    plot.c2p = canvasToAxisCoords;
    plot.p2c = axisToCanvasCoords;
    plot.getOptions = function () { return options; };
    plot.highlight = highlight;
    plot.unhighlight = unhighlight;
    plot.triggerRedrawOverlay = triggerRedrawOverlay;
    plot.pointOffset = function (point) {
      return {
        left: parseInt(xaxes[axisNumber(point, "x") - 1].p2c(+point.x) + plotOffset.left),
        top: parseInt(yaxes[axisNumber(point, "y") - 1].p2c(+point.y) + plotOffset.top)
      };
    };
    plot.shutdown = shutdown;
    plot.resize = function () {
      getCanvasDimensions();
      resizeCanvas(canvas);
      resizeCanvas(overlay);
    };

    // public attributes
    plot.hooks = hooks;

    // initialize
    initPlugins(plot);
    parseOptions(options_);
    setupCanvases();
    setData(data_);
    setupGrid();
    draw();
    bindEvents();


    function executeHooks(hook, args) {
      args = [plot].concat(args);
      for (var i = 0; i < hook.length; ++i)
        hook[i].apply(this, args);
    }

    function initPlugins() {
      for (var i = 0; i < plugins.length; ++i) {
        var p = plugins[i];
        p.init(plot);
        if (p.options)
          $.extend(true, options, p.options);
      }
    }

    function parseOptions(opts) {
      var i;

      $.extend(true, options, opts);

      if (options.xaxis.color == null)
        options.xaxis.color = options.grid.color;
      if (options.yaxis.color == null)
        options.yaxis.color = options.grid.color;

      if (options.xaxis.tickColor == null) // backwards-compatibility
        options.xaxis.tickColor = options.grid.tickColor;
      if (options.yaxis.tickColor == null) // backwards-compatibility
        options.yaxis.tickColor = options.grid.tickColor;

      if (options.grid.borderColor == null)
        options.grid.borderColor = options.grid.color;
      if (options.grid.tickColor == null)
        options.grid.tickColor = $.color.parse(options.grid.color).scale('a', 0.22).toString();

      // fill in defaults in axes, copy at least always the
      // first as the rest of the code assumes it'll be there
      for (i = 0; i < Math.max(1, options.xaxes.length); ++i)
        options.xaxes[i] = $.extend(true, {}, options.xaxis, options.xaxes[i]);
      for (i = 0; i < Math.max(1, options.yaxes.length); ++i)
        options.yaxes[i] = $.extend(true, {}, options.yaxis, options.yaxes[i]);

      // backwards compatibility, to be removed in future
      if (options.xaxis.noTicks && options.xaxis.ticks == null)
        options.xaxis.ticks = options.xaxis.noTicks;
      if (options.yaxis.noTicks && options.yaxis.ticks == null)
        options.yaxis.ticks = options.yaxis.noTicks;
      if (options.x2axis) {
        options.xaxes[1] = $.extend(true, {}, options.xaxis, options.x2axis);
        options.xaxes[1].position = "top";
      }
      if (options.y2axis) {
        options.yaxes[1] = $.extend(true, {}, options.yaxis, options.y2axis);
        options.yaxes[1].position = "right";
      }
      if (options.grid.coloredAreas)
        options.grid.markings = options.grid.coloredAreas;
      if (options.grid.coloredAreasColor)
        options.grid.markingsColor = options.grid.coloredAreasColor;
      if (options.lines)
        $.extend(true, options.series.lines, options.lines);
      if (options.points)
        $.extend(true, options.series.points, options.points);
      if (options.bars)
        $.extend(true, options.series.bars, options.bars);
      if (options.shadowSize != null)
        options.series.shadowSize = options.shadowSize;

      // save options on axes for future reference
      for (i = 0; i < options.xaxes.length; ++i)
        getOrCreateAxis(xaxes, i + 1).options = options.xaxes[i];
      for (i = 0; i < options.yaxes.length; ++i)
        getOrCreateAxis(yaxes, i + 1).options = options.yaxes[i];

      // add hooks from options
      for (var n in hooks)
        if (options.hooks[n] && options.hooks[n].length)
          hooks[n] = hooks[n].concat(options.hooks[n]);

        executeHooks(hooks.processOptions, [options]);
      }

      function setData(d) {
        series = parseData(d);
        fillInSeriesOptions();
        processData();
      }

      function parseData(d) {
        var res = [];
        for (var i = 0; i < d.length; ++i) {
          var s = $.extend(true, {}, options.series);

          if (d[i].data != null) {
            s.data = d[i].data; // move the data instead of deep-copy
            delete d[i].data;

            $.extend(true, s, d[i]);

            d[i].data = s.data;
          }
          else
            s.data = d[i];
          res.push(s);
        }

        return res;
      }

      function axisNumber(obj, coord) {
        var a = obj[coord + "axis"];
        if (typeof a == "object") // if we got a real axis, extract number
          a = a.n;
        if (typeof a != "number")
          a = 1; // default to first axis
        return a;
      }

      function allAxes() {
        // return flat array without annoying null entries
        return $.grep(xaxes.concat(yaxes), function (a) { return a; });
      }

      function canvasToAxisCoords(pos) {
        // return an object with x/y corresponding to all used axes 
        var res = {}, i, axis;
        for (i = 0; i < xaxes.length; ++i) {
          axis = xaxes[i];
          if (axis && axis.used)
            res["x" + axis.n] = axis.c2p(pos.left);
        }

        for (i = 0; i < yaxes.length; ++i) {
          axis = yaxes[i];
          if (axis && axis.used)
            res["y" + axis.n] = axis.c2p(pos.top);
        }

        if (res.x1 !== undefined)
          res.x = res.x1;
        if (res.y1 !== undefined)
          res.y = res.y1;

        return res;
      }

      function axisToCanvasCoords(pos) {
        // get canvas coords from the first pair of x/y found in pos
        var res = {}, i, axis, key;

        for (i = 0; i < xaxes.length; ++i) {
          axis = xaxes[i];
          if (axis && axis.used) {
            key = "x" + axis.n;
            if (pos[key] == null && axis.n == 1)
              key = "x";

            if (pos[key] != null) {
              res.left = axis.p2c(pos[key]);
              break;
            }
          }
        }

        for (i = 0; i < yaxes.length; ++i) {
          axis = yaxes[i];
          if (axis && axis.used) {
            key = "y" + axis.n;
            if (pos[key] == null && axis.n == 1)
              key = "y";

            if (pos[key] != null) {
              res.top = axis.p2c(pos[key]);
              break;
            }
          }
        }

        return res;
      }

      function getOrCreateAxis(axes, number) {
        if (!axes[number - 1])
          axes[number - 1] = {
            n: number, // save the number for future reference
            direction: axes == xaxes ? "x" : "y",
            options: $.extend(true, {}, axes == xaxes ? options.xaxis : options.yaxis)
          };

        return axes[number - 1];
      }

      function fillInSeriesOptions() {
        var i;

        // collect what we already got of colors
        var neededColors = series.length,
                usedColors = [],
                assignedColors = [];
        for (i = 0; i < series.length; ++i) {
          var sc = series[i].color;
          if (sc != null) {
            --neededColors;
            if (typeof sc == "number")
              assignedColors.push(sc);
            else
              usedColors.push($.color.parse(series[i].color));
          }
        }

        // we might need to generate more colors if higher indices
        // are assigned
        for (i = 0; i < assignedColors.length; ++i) {
          neededColors = Math.max(neededColors, assignedColors[i] + 1);
        }

        // produce colors as needed
        var colors = [], variation = 0;
        i = 0;
        while (colors.length < neededColors) {
          var c;
          if (options.colors.length == i) // check degenerate case
            c = $.color.make(100, 100, 100);
          else
            c = $.color.parse(options.colors[i]);

          // vary color if needed
          var sign = variation % 2 == 1 ? -1 : 1;
          c.scale('rgb', 1 + sign * Math.ceil(variation / 2) * 0.2)

          // FIXME: if we're getting to close to something else,
          // we should probably skip this one
          colors.push(c);

          ++i;
          if (i >= options.colors.length) {
            i = 0;
            ++variation;
          }
        }

        // fill in the options
        var colori = 0, s;
        for (i = 0; i < series.length; ++i) {
          s = series[i];

          // assign colors
          if (s.color == null) {
            s.color = colors[colori].toString();
            ++colori;
          }
          else if (typeof s.color == "number")
            s.color = colors[s.color].toString();

          // turn on lines automatically in case nothing is set
          if (s.lines.show == null) {
            var v, show = true;
            for (v in s)
              if (s[v] && s[v].show) {
                show = false;
                break;
              }
            if (show)
              s.lines.show = true;
          }

          // setup axes
          s.xaxis = getOrCreateAxis(xaxes, axisNumber(s, "x"));
          s.yaxis = getOrCreateAxis(yaxes, axisNumber(s, "y"));
        }
      }

      function processData() {
        var topSentry = Number.POSITIVE_INFINITY,
                bottomSentry = Number.NEGATIVE_INFINITY,
                fakeInfinity = Number.MAX_VALUE,
                i, j, k, m, length,
                s, points, ps, x, y, axis, val, f, p;

        function updateAxis(axis, min, max) {
          if (min < axis.datamin && min != -fakeInfinity)
            axis.datamin = min;
          if (max > axis.datamax && max != fakeInfinity)
            axis.datamax = max;
        }

        $.each(allAxes(), function (_, axis) {
          // init axis
          axis.datamin = topSentry;
          axis.datamax = bottomSentry;
          axis.used = false;
        });

        for (i = 0; i < series.length; ++i) {
          s = series[i];
          s.datapoints = { points: [] };

          executeHooks(hooks.processRawData, [s, s.data, s.datapoints]);
        }

        // first pass: clean and copy data
        for (i = 0; i < series.length; ++i) {
          s = series[i];

          var data = s.data, format = s.datapoints.format;

          if (!format) {
            format = [];
            // find out how to copy
            format.push({ x: true, number: true, required: true });
            format.push({ y: true, number: true, required: true });

            if (s.bars.show || (s.lines.show && s.lines.fill)) {
              format.push({ y: true, number: true, required: false, defaultValue: 0 });
              if (s.bars.horizontal) {
                delete format[format.length - 1].y;
                format[format.length - 1].x = true;
              }
            }

            s.datapoints.format = format;
          }

          if (s.datapoints.pointsize != null)
            continue; // already filled in

          s.datapoints.pointsize = format.length;

          ps = s.datapoints.pointsize;
          points = s.datapoints.points;

          insertSteps = s.lines.show && s.lines.steps;
          s.xaxis.used = s.yaxis.used = true;

          for (j = k = 0; j < data.length; ++j, k += ps) {
            p = data[j];

            var nullify = p == null;
            if (!nullify) {
              for (m = 0; m < ps; ++m) {
                val = p[m];
                f = format[m];

                if (f) {
                  if (f.number && val != null) {
                    val = +val; // convert to number
                    if (isNaN(val))
                      val = null;
                    else if (val == Infinity)
                      val = fakeInfinity;
                    else if (val == -Infinity)
                      val = -fakeInfinity;
                  }

                  if (val == null) {
                    if (f.required)
                      nullify = true;

                    if (f.defaultValue != null)
                      val = f.defaultValue;
                  }
                }

                points[k + m] = val;
              }
            }

            if (nullify) {
              for (m = 0; m < ps; ++m) {
                val = points[k + m];
                if (val != null) {
                  f = format[m];
                  // extract min/max info
                  if (f.x)
                    updateAxis(s.xaxis, val, val);
                  if (f.y)
                    updateAxis(s.yaxis, val, val);
                }
                points[k + m] = null;
              }
            }
            else {
              // a little bit of line specific stuff that
              // perhaps shouldn't be here, but lacking
              // better means...
              if (insertSteps && k > 0
                            && points[k - ps] != null
                            && points[k - ps] != points[k]
                            && points[k - ps + 1] != points[k + 1]) {
                // copy the point to make room for a middle point
                for (m = 0; m < ps; ++m)
                  points[k + ps + m] = points[k + m];

                // middle point has same y
                points[k + 1] = points[k - ps + 1];

                // we've added a point, better reflect that
                k += ps;
              }
            }
          }
        }

        // give the hooks a chance to run
        for (i = 0; i < series.length; ++i) {
          s = series[i];

          executeHooks(hooks.processDatapoints, [s, s.datapoints]);
        }

        // second pass: find datamax/datamin for auto-scaling
        for (i = 0; i < series.length; ++i) {
          s = series[i];
          points = s.datapoints.points,
                ps = s.datapoints.pointsize;

          var xmin = topSentry, ymin = topSentry,
                    xmax = bottomSentry, ymax = bottomSentry;

          for (j = 0; j < points.length; j += ps) {
            if (points[j] == null)
              continue;

            for (m = 0; m < ps; ++m) {
              val = points[j + m];
              f = format[m];
              if (!f || val == fakeInfinity || val == -fakeInfinity)
                continue;

              if (f.x) {
                if (val < xmin)
                  xmin = val;
                if (val > xmax)
                  xmax = val;
              }
              if (f.y) {
                if (val < ymin)
                  ymin = val;
                if (val > ymax)
                  ymax = val;
              }
            }
          }

          if (s.bars.show) {
            // make sure we got room for the bar on the dancing floor
            var delta = s.bars.align == "left" ? 0 : -s.bars.barWidth / 2;
            if (s.bars.horizontal) {
              ymin += delta;
              ymax += delta + s.bars.barWidth;
            }
            else {
              xmin += delta;
              xmax += delta + s.bars.barWidth;
            }
          }

          updateAxis(s.xaxis, xmin, xmax);
          updateAxis(s.yaxis, ymin, ymax);
        }

        $.each(allAxes(), function (_, axis) {
          if (axis.datamin == topSentry)
            axis.datamin = null;
          if (axis.datamax == bottomSentry)
            axis.datamax = null;
        });
      }

      function makeCanvas(skipPositioning, cls) {
        var c = document.createElement('canvas');
        c.className = cls;
        c.width = canvasWidth;
        c.height = canvasHeight;

        if (!skipPositioning)
          $(c).css({ position: 'absolute', left: 0, top: 0 });

        $(c).appendTo(placeholder);

        if (!c.getContext) // excanvas hack
          c = window.G_vmlCanvasManager.initElement(c);

        // used for resetting in case we get replotted
        c.getContext("2d").save();

        return c;
      }

      function getCanvasDimensions() {
        canvasWidth = placeholder.width();
        canvasHeight = placeholder.height();

        if (canvasWidth <= 0 || canvasHeight <= 0)
          throw "Invalid dimensions for plot, width = " + canvasWidth + ", height = " + canvasHeight;
      }

      function resizeCanvas(c) {
        // resizing should reset the state (excanvas seems to be
        // buggy though)
        if (c.width != canvasWidth)
          c.width = canvasWidth;

        if (c.height != canvasHeight)
          c.height = canvasHeight;

        // so try to get back to the initial state (even if it's
        // gone now, this should be safe according to the spec)
        var cctx = c.getContext("2d");
        cctx.restore();

        // and save again
        cctx.save();
      }

      function setupCanvases() {
        var reused,
                existingCanvas = placeholder.children("canvas.base"),
                existingOverlay = placeholder.children("canvas.overlay");

        if (existingCanvas.length == 0 || existingOverlay == 0) {
          // init everything

          placeholder.html(""); // make sure placeholder is clear

          placeholder.css({ padding: 0 }); // padding messes up the positioning

          if (placeholder.css("position") == 'static')
            placeholder.css("position", "relative"); // for positioning labels and overlay

          getCanvasDimensions();

          canvas = makeCanvas(true, "base");
          overlay = makeCanvas(false, "overlay"); // overlay canvas for interactive features

          reused = false;
        }
        else {
          // reuse existing elements

          canvas = existingCanvas.get(0);
          overlay = existingOverlay.get(0);

          reused = true;
        }

        ctx = canvas.getContext("2d");
        octx = overlay.getContext("2d");

        // we include the canvas in the event holder too, because IE 7
        // sometimes has trouble with the stacking order
        eventHolder = $([overlay, canvas]);

        if (reused) {
          // run shutdown in the old plot object
          placeholder.data("plot").shutdown();

          // reset reused canvases
          plot.resize();

          // make sure overlay pixels are cleared (canvas is cleared when we redraw)
          octx.clearRect(0, 0, canvasWidth, canvasHeight);

          // then whack any remaining obvious garbage left
          eventHolder.unbind();
          placeholder.children().not([canvas, overlay]).remove();
        }

        // save in case we get replotted
        placeholder.data("plot", plot);
      }

      function bindEvents() {
        // bind events
        if (options.grid.hoverable) {
          eventHolder.mousemove(onMouseMove);
          eventHolder.mouseleave(onMouseLeave);
        }

        if (options.grid.clickable)
          eventHolder.click(onClick);

        executeHooks(hooks.bindEvents, [eventHolder]);
      }

      function shutdown() {
        if (redrawTimeout)
          clearTimeout(redrawTimeout);

        eventHolder.unbind("mousemove", onMouseMove);
        eventHolder.unbind("mouseleave", onMouseLeave);
        eventHolder.unbind("click", onClick);

        executeHooks(hooks.shutdown, [eventHolder]);
      }

      function setTransformationHelpers(axis) {
        // set helper functions on the axis, assumes plot area
        // has been computed already

        function identity(x) { return x; }

        var s, m, t = axis.options.transform || identity,
                it = axis.options.inverseTransform;

        // precompute how much the axis is scaling a point
        // in canvas space
        if (axis.direction == "x") {
          s = axis.scale = plotWidth / Math.abs(t(axis.max) - t(axis.min));
          m = Math.min(t(axis.max), t(axis.min));
        }
        else {
          s = axis.scale = plotHeight / Math.abs(t(axis.max) - t(axis.min));
          s = -s;
          m = Math.max(t(axis.max), t(axis.min));
        }

        // data point to canvas coordinate
        if (t == identity) // slight optimization
          axis.p2c = function (p) { return (p - m) * s; };
        else
          axis.p2c = function (p) { return (t(p) - m) * s; };
        // canvas coordinate to data point
        if (!it)
          axis.c2p = function (c) { return m + c / s; };
        else
          axis.c2p = function (c) { return it(m + c / s); };
      }

      function measureTickLabels(axis) {
        var opts = axis.options, i, ticks = axis.ticks || [], labels = [],
                l, w = opts.labelWidth, h = opts.labelHeight, dummyDiv;

        function makeDummyDiv(labels, width) {
          return $('<div style="position:absolute;top:-10000px;' + width + 'font-size:smaller">' +
                         '<div class="' + axis.direction + 'Axis ' + axis.direction + axis.n + 'Axis">'
                         + labels.join("") + '</div></div>')
                    .appendTo(placeholder);
        }

        if (axis.direction == "x") {
          // to avoid measuring the widths of the labels (it's slow), we
          // construct fixed-size boxes and put the labels inside
          // them, we don't need the exact figures and the
          // fixed-size box content is easy to center
          if (w == null)
            w = Math.floor(canvasWidth / (ticks.length > 0 ? ticks.length : 1));

          // measure x label heights
          if (h == null) {
            labels = [];
            for (i = 0; i < ticks.length; ++i) {
              l = ticks[i].label;
              if (l)
                labels.push('<div class="tickLabel" style="float:left;width:' + w + 'px">' + l + '</div>');
            }

            if (labels.length > 0) {
              // stick them all in the same div and measure
              // collective height
              labels.push('<div style="clear:left"></div>');
              dummyDiv = makeDummyDiv(labels, "width:10000px;");
              h = dummyDiv.height();
              dummyDiv.remove();
            }
          }
        }
        else if (w == null || h == null) {
          // calculate y label dimensions
          for (i = 0; i < ticks.length; ++i) {
            l = ticks[i].label;
            if (l)
              labels.push('<div class="tickLabel">' + l + '</div>');
          }

          if (labels.length > 0) {
            dummyDiv = makeDummyDiv(labels, "");
            if (w == null)
              w = dummyDiv.children().width();
            if (h == null)
              h = dummyDiv.find("div.tickLabel").height();
            dummyDiv.remove();
          }
        }

        if (w == null)
          w = 0;
        if (h == null)
          h = 0;

        axis.labelWidth = w;
        axis.labelHeight = h;
      }

      function allocateAxisBoxFirstPhase(axis) {
        // find the bounding box of the axis by looking at label
        // widths/heights and ticks, make room by diminishing the
        // plotOffset

        var lw = axis.labelWidth,
                lh = axis.labelHeight,
                pos = axis.options.position,
                tickLength = axis.options.tickLength,
                axismargin = options.grid.axisMargin,
                padding = options.grid.labelMargin,
                all = axis.direction == "x" ? xaxes : yaxes,
                index;

        // determine axis margin
        var samePosition = $.grep(all, function (a) {
          return a && a.options.position == pos && a.reserveSpace;
        });
        if ($.inArray(axis, samePosition) == samePosition.length - 1)
          axismargin = 0; // outermost

        // determine tick length - if we're innermost, we can use "full"
        if (tickLength == null)
          tickLength = "full";

        var sameDirection = $.grep(all, function (a) {
          return a && a.reserveSpace;
        });

        var innermost = $.inArray(axis, sameDirection) == 0;
        if (!innermost && tickLength == "full")
          tickLength = 5;

        if (!isNaN(+tickLength))
          padding += +tickLength;

        // compute box
        if (axis.direction == "x") {
          lh += padding;

          if (pos == "bottom") {
            plotOffset.bottom += lh + axismargin;
            axis.box = { top: canvasHeight - plotOffset.bottom, height: lh };
          }
          else {
            axis.box = { top: plotOffset.top + axismargin, height: lh };
            plotOffset.top += lh + axismargin;
          }
        }
        else {
          lw += padding;

          if (pos == "left") {
            axis.box = { left: plotOffset.left + axismargin, width: lw };
            plotOffset.left += lw + axismargin;
          }
          else {
            plotOffset.right += lw + axismargin;
            axis.box = { left: canvasWidth - plotOffset.right, width: lw };
          }
        }

        // save for future reference
        axis.position = pos;
        axis.tickLength = tickLength;
        axis.box.padding = padding;
        axis.innermost = innermost;
      }

      function allocateAxisBoxSecondPhase(axis) {
        // set remaining bounding box coordinates
        if (axis.direction == "x") {
          axis.box.left = plotOffset.left;
          axis.box.width = plotWidth;
        }
        else {
          axis.box.top = plotOffset.top;
          axis.box.height = plotHeight;
        }
      }

      function setupGrid() {
        var i, axes = allAxes();

        // first calculate the plot and axis box dimensions

        $.each(axes, function (_, axis) {
          axis.show = axis.options.show;
          if (axis.show == null)
            axis.show = axis.used; // by default an axis is visible if it's got data

          axis.reserveSpace = axis.show || axis.options.reserveSpace;

          setRange(axis);
        });

        allocatedAxes = $.grep(axes, function (axis) { return axis.reserveSpace; });

        plotOffset.left = plotOffset.right = plotOffset.top = plotOffset.bottom = 0;
        if (options.grid.show) {
          $.each(allocatedAxes, function (_, axis) {
            // make the ticks
            setupTickGeneration(axis);
            setTicks(axis);
            snapRangeToTicks(axis, axis.ticks);

            // find labelWidth/Height for axis
            measureTickLabels(axis);
          });

          // with all dimensions in house, we can compute the
          // axis boxes, start from the outside (reverse order)
          for (i = allocatedAxes.length - 1; i >= 0; --i)
            allocateAxisBoxFirstPhase(allocatedAxes[i]);

          // make sure we've got enough space for things that
          // might stick out
          var minMargin = options.grid.minBorderMargin;
          if (minMargin == null) {
            minMargin = 0;
            for (i = 0; i < series.length; ++i)
              minMargin = Math.max(minMargin, series[i].points.radius + series[i].points.lineWidth / 2);
          }

          for (var a in plotOffset) {
            plotOffset[a] += options.grid.borderWidth;
            plotOffset[a] = Math.max(minMargin, plotOffset[a]);
          }
        }

        plotWidth = canvasWidth - plotOffset.left - plotOffset.right;
        plotHeight = canvasHeight - plotOffset.bottom - plotOffset.top;

        // now we got the proper plotWidth/Height, we can compute the scaling
        $.each(axes, function (_, axis) {
          setTransformationHelpers(axis);
        });

        if (options.grid.show) {
          $.each(allocatedAxes, function (_, axis) {
            allocateAxisBoxSecondPhase(axis);
          });

          insertAxisLabels();
        }

        insertLegend();
      }

      function setRange(axis) {
        var opts = axis.options,
                min = +(opts.min != null ? opts.min : axis.datamin),
                max = +(opts.max != null ? opts.max : axis.datamax),
                delta = max - min;

        if (delta == 0.0) {
          // degenerate case
          var widen = max == 0 ? 1 : 0.01;

          if (opts.min == null)
            min -= widen;
          // always widen max if we couldn't widen min to ensure we
          // don't fall into min == max which doesn't work
          if (opts.max == null || opts.min != null)
            max += widen;
        }
        else {
          // consider autoscaling
          var margin = opts.autoscaleMargin;
          if (margin != null) {
            if (opts.min == null) {
              min -= delta * margin;
              // make sure we don't go below zero if all values
              // are positive
              if (min < 0 && axis.datamin != null && axis.datamin >= 0)
                min = 0;
            }
            if (opts.max == null) {
              max += delta * margin;
              if (max > 0 && axis.datamax != null && axis.datamax <= 0)
                max = 0;
            }
          }
        }
        axis.min = min;
        axis.max = max;
      }

      function setupTickGeneration(axis) {
        var opts = axis.options;

        // estimate number of ticks
        var noTicks;
        if (typeof opts.ticks == "number" && opts.ticks > 0)
          noTicks = opts.ticks;
        else
        // heuristic based on the model a*sqrt(x) fitted to
        // some data points that seemed reasonable
          noTicks = 0.3 * Math.sqrt(axis.direction == "x" ? canvasWidth : canvasHeight);

        var delta = (axis.max - axis.min) / noTicks,
                size, generator, unit, formatter, i, magn, norm;

        if (opts.mode == "time") {
          // pretty handling of time

          // map of app. size of time units in milliseconds
          var timeUnitSize = {
            "second": 1000,
            "minute": 60 * 1000,
            "hour": 60 * 60 * 1000,
            "day": 24 * 60 * 60 * 1000,
            "month": 30 * 24 * 60 * 60 * 1000,
            "year": 365.2425 * 24 * 60 * 60 * 1000
          };


          // the allowed tick sizes, after 1 year we use
          // an integer algorithm
          var spec = [
                    [1, "second"], [2, "second"], [5, "second"], [10, "second"],
                    [30, "second"],
                    [1, "minute"], [2, "minute"], [5, "minute"], [10, "minute"],
                    [30, "minute"],
                    [1, "hour"], [2, "hour"], [4, "hour"],
                    [8, "hour"], [12, "hour"],
                    [1, "day"], [2, "day"], [3, "day"],
                    [0.25, "month"], [0.5, "month"], [1, "month"],
                    [2, "month"], [3, "month"], [6, "month"],
                    [1, "year"]
                ];

          var minSize = 0;
          if (opts.minTickSize != null) {
            if (typeof opts.tickSize == "number")
              minSize = opts.tickSize;
            else
              minSize = opts.minTickSize[0] * timeUnitSize[opts.minTickSize[1]];
          }

          for (var i = 0; i < spec.length - 1; ++i)
            if (delta < (spec[i][0] * timeUnitSize[spec[i][1]]
                                 + spec[i + 1][0] * timeUnitSize[spec[i + 1][1]]) / 2
                       && spec[i][0] * timeUnitSize[spec[i][1]] >= minSize)
              break;
          size = spec[i][0];
          unit = spec[i][1];

          // special-case the possibility of several years
          if (unit == "year") {
            magn = Math.pow(10, Math.floor(Math.log(delta / timeUnitSize.year) / Math.LN10));
            norm = (delta / timeUnitSize.year) / magn;
            if (norm < 1.5)
              size = 1;
            else if (norm < 3)
              size = 2;
            else if (norm < 7.5)
              size = 5;
            else
              size = 10;

            size *= magn;
          }

          axis.tickSize = opts.tickSize || [size, unit];

          generator = function (axis) {
            var ticks = [],
                        tickSize = axis.tickSize[0], unit = axis.tickSize[1],
                        d = new Date(axis.min);

            var step = tickSize * timeUnitSize[unit];

            if (unit == "second")
              d.setUTCSeconds(floorInBase(d.getUTCSeconds(), tickSize));
            if (unit == "minute")
              d.setUTCMinutes(floorInBase(d.getUTCMinutes(), tickSize));
            if (unit == "hour")
              d.setUTCHours(floorInBase(d.getUTCHours(), tickSize));
            if (unit == "month")
              d.setUTCMonth(floorInBase(d.getUTCMonth(), tickSize));
            if (unit == "year")
              d.setUTCFullYear(floorInBase(d.getUTCFullYear(), tickSize));

            // reset smaller components
            d.setUTCMilliseconds(0);
            if (step >= timeUnitSize.minute)
              d.setUTCSeconds(0);
            if (step >= timeUnitSize.hour)
              d.setUTCMinutes(0);
            if (step >= timeUnitSize.day)
              d.setUTCHours(0);
            if (step >= timeUnitSize.day * 4)
              d.setUTCDate(1);
            if (step >= timeUnitSize.year)
              d.setUTCMonth(0);


            var carry = 0, v = Number.NaN, prev;
            do {
              prev = v;
              v = d.getTime();
              ticks.push(v);
              if (unit == "month") {
                if (tickSize < 1) {
                  // a bit complicated - we'll divide the month
                  // up but we need to take care of fractions
                  // so we don't end up in the middle of a day
                  d.setUTCDate(1);
                  var start = d.getTime();
                  d.setUTCMonth(d.getUTCMonth() + 1);
                  var end = d.getTime();
                  d.setTime(v + carry * timeUnitSize.hour + (end - start) * tickSize);
                  carry = d.getUTCHours();
                  d.setUTCHours(0);
                }
                else
                  d.setUTCMonth(d.getUTCMonth() + tickSize);
              }
              else if (unit == "year") {
                d.setUTCFullYear(d.getUTCFullYear() + tickSize);
              }
              else
                d.setTime(v + step);
            } while (v < axis.max && v != prev);

            return ticks;
          };

          formatter = function (v, axis) {
            var d = new Date(v);

            // first check global format
            if (opts.timeformat != null)
              return $.plot.formatDate(d, opts.timeformat, opts.monthNames);

            var t = axis.tickSize[0] * timeUnitSize[axis.tickSize[1]];
            var span = axis.max - axis.min;
            var suffix = (opts.twelveHourClock) ? " %p" : "";

            if (t < timeUnitSize.minute)
              fmt = "%h:%M:%S" + suffix;
            else if (t < timeUnitSize.day) {
              if (span < 2 * timeUnitSize.day)
                fmt = "%h:%M" + suffix;
              else
                fmt = "%b %d %h:%M" + suffix;
            }
            else if (t < timeUnitSize.month)
              fmt = "%b %d";
            else if (t < timeUnitSize.year) {
              if (span < timeUnitSize.year)
                fmt = "%b";
              else
                fmt = "%b %y";
            }
            else
              fmt = "%y";

            return $.plot.formatDate(d, fmt, opts.monthNames);
          };
        }
        else {
          // pretty rounding of base-10 numbers
          var maxDec = opts.tickDecimals;
          var dec = -Math.floor(Math.log(delta) / Math.LN10);
          if (maxDec != null && dec > maxDec)
            dec = maxDec;

          magn = Math.pow(10, -dec);
          norm = delta / magn; // norm is between 1.0 and 10.0

          if (norm < 1.5)
            size = 1;
          else if (norm < 3) {
            size = 2;
            // special case for 2.5, requires an extra decimal
            if (norm > 2.25 && (maxDec == null || dec + 1 <= maxDec)) {
              size = 2.5;
              ++dec;
            }
          }
          else if (norm < 7.5)
            size = 5;
          else
            size = 10;

          size *= magn;

          if (opts.minTickSize != null && size < opts.minTickSize)
            size = opts.minTickSize;

          axis.tickDecimals = Math.max(0, maxDec != null ? maxDec : dec);
          axis.tickSize = opts.tickSize || size;

          generator = function (axis) {
            var ticks = [];

            // spew out all possible ticks
            var start = floorInBase(axis.min, axis.tickSize),
                        i = 0, v = Number.NaN, prev;
            do {
              prev = v;
              v = start + i * axis.tickSize;
              ticks.push(v);
              ++i;
            } while (v < axis.max && v != prev);
            return ticks;
          };

          formatter = function (v, axis) {
            return v.toFixed(axis.tickDecimals);
          };
        }

        if (opts.alignTicksWithAxis != null) {
          var otherAxis = (axis.direction == "x" ? xaxes : yaxes)[opts.alignTicksWithAxis - 1];
          if (otherAxis && otherAxis.used && otherAxis != axis) {
            // consider snapping min/max to outermost nice ticks
            var niceTicks = generator(axis);
            if (niceTicks.length > 0) {
              if (opts.min == null)
                axis.min = Math.min(axis.min, niceTicks[0]);
              if (opts.max == null && niceTicks.length > 1)
                axis.max = Math.max(axis.max, niceTicks[niceTicks.length - 1]);
            }

            generator = function (axis) {
              // copy ticks, scaled to this axis
              var ticks = [], v, i;
              for (i = 0; i < otherAxis.ticks.length; ++i) {
                v = (otherAxis.ticks[i].v - otherAxis.min) / (otherAxis.max - otherAxis.min);
                v = axis.min + v * (axis.max - axis.min);
                ticks.push(v);
              }
              return ticks;
            };

            // we might need an extra decimal since forced
            // ticks don't necessarily fit naturally
            if (axis.mode != "time" && opts.tickDecimals == null) {
              var extraDec = Math.max(0, -Math.floor(Math.log(delta) / Math.LN10) + 1),
                            ts = generator(axis);

              // only proceed if the tick interval rounded
              // with an extra decimal doesn't give us a
              // zero at end
              if (!(ts.length > 1 && /\..*0$/.test((ts[1] - ts[0]).toFixed(extraDec))))
                axis.tickDecimals = extraDec;
            }
          }
        }

        axis.tickGenerator = generator;
        if ($.isFunction(opts.tickFormatter))
          axis.tickFormatter = function (v, axis) { return "" + opts.tickFormatter(v, axis); };
        else
          axis.tickFormatter = formatter;
      }

      function setTicks(axis) {
        var oticks = axis.options.ticks, ticks = [];
        if (oticks == null || (typeof oticks == "number" && oticks > 0))
          ticks = axis.tickGenerator(axis);
        else if (oticks) {
          if ($.isFunction(oticks))
          // generate the ticks
            ticks = oticks({ min: axis.min, max: axis.max });
          else
            ticks = oticks;
        }

        // clean up/labelify the supplied ticks, copy them over
        var i, v;
        axis.ticks = [];
        for (i = 0; i < ticks.length; ++i) {
          var label = null;
          var t = ticks[i];
          if (typeof t == "object") {
            v = +t[0];
            if (t.length > 1)
              label = t[1];
          }
          else
            v = +t;
          if (label == null)
            label = axis.tickFormatter(v, axis);
          if (!isNaN(v))
            axis.ticks.push({ v: v, label: label });
        }
      }

      function snapRangeToTicks(axis, ticks) {
        if (axis.options.autoscaleMargin && ticks.length > 0) {
          // snap to ticks
          if (axis.options.min == null)
            axis.min = Math.min(axis.min, ticks[0].v);
          if (axis.options.max == null && ticks.length > 1)
            axis.max = Math.max(axis.max, ticks[ticks.length - 1].v);
        }
      }

      function draw() {
        ctx.clearRect(0, 0, canvasWidth, canvasHeight);

        var grid = options.grid;

        // draw background, if any
        if (grid.show && grid.backgroundColor)
          drawBackground();

        if (grid.show && !grid.aboveData)
          drawGrid();

        for (var i = 0; i < series.length; ++i) {
          executeHooks(hooks.drawSeries, [ctx, series[i]]);
          drawSeries(series[i]);
        }

        executeHooks(hooks.draw, [ctx]);

        if (grid.show && grid.aboveData)
          drawGrid();
      }

      function extractRange(ranges, coord) {
        var axis, from, to, key, axes = allAxes();

        for (i = 0; i < axes.length; ++i) {
          axis = axes[i];
          if (axis.direction == coord) {
            key = coord + axis.n + "axis";
            if (!ranges[key] && axis.n == 1)
              key = coord + "axis"; // support x1axis as xaxis
            if (ranges[key]) {
              from = ranges[key].from;
              to = ranges[key].to;
              break;
            }
          }
        }

        // backwards-compat stuff - to be removed in future
        if (!ranges[key]) {
          axis = coord == "x" ? xaxes[0] : yaxes[0];
          from = ranges[coord + "1"];
          to = ranges[coord + "2"];
        }

        // auto-reverse as an added bonus
        if (from != null && to != null && from > to) {
          var tmp = from;
          from = to;
          to = tmp;
        }

        return { from: from, to: to, axis: axis };
      }

      function drawBackground() {
        ctx.save();
        ctx.translate(plotOffset.left, plotOffset.top);

        ctx.fillStyle = getColorOrGradient(options.grid.backgroundColor, plotHeight, 0, "rgba(255, 255, 255, 0)");
        ctx.fillRect(0, 0, plotWidth, plotHeight);
        ctx.restore();
      }

      function drawGrid() {
        var i;

        ctx.save();
        ctx.translate(plotOffset.left, plotOffset.top);

        // draw markings
        var markings = options.grid.markings;
        if (markings) {
          if ($.isFunction(markings)) {
            var axes = plot.getAxes();
            // xmin etc. is backwards compatibility, to be
            // removed in the future
            axes.xmin = axes.xaxis.min;
            axes.xmax = axes.xaxis.max;
            axes.ymin = axes.yaxis.min;
            axes.ymax = axes.yaxis.max;

            markings = markings(axes);
          }

          for (i = 0; i < markings.length; ++i) {
            var m = markings[i],
                        xrange = extractRange(m, "x"),
                        yrange = extractRange(m, "y");

            // fill in missing
            if (xrange.from == null)
              xrange.from = xrange.axis.min;
            if (xrange.to == null)
              xrange.to = xrange.axis.max;
            if (yrange.from == null)
              yrange.from = yrange.axis.min;
            if (yrange.to == null)
              yrange.to = yrange.axis.max;

            // clip
            if (xrange.to < xrange.axis.min || xrange.from > xrange.axis.max ||
                        yrange.to < yrange.axis.min || yrange.from > yrange.axis.max)
              continue;

            xrange.from = Math.max(xrange.from, xrange.axis.min);
            xrange.to = Math.min(xrange.to, xrange.axis.max);
            yrange.from = Math.max(yrange.from, yrange.axis.min);
            yrange.to = Math.min(yrange.to, yrange.axis.max);

            if (xrange.from == xrange.to && yrange.from == yrange.to)
              continue;

            // then draw
            xrange.from = xrange.axis.p2c(xrange.from);
            xrange.to = xrange.axis.p2c(xrange.to);
            yrange.from = yrange.axis.p2c(yrange.from);
            yrange.to = yrange.axis.p2c(yrange.to);

            if (xrange.from == xrange.to || yrange.from == yrange.to) {
              // draw line
              ctx.beginPath();
              ctx.strokeStyle = m.color || options.grid.markingsColor;
              ctx.lineWidth = m.lineWidth || options.grid.markingsLineWidth;
              ctx.moveTo(xrange.from, yrange.from);
              ctx.lineTo(xrange.to, yrange.to);
              ctx.stroke();
            }
            else {
              // fill area
              ctx.fillStyle = m.color || options.grid.markingsColor;
              ctx.fillRect(xrange.from, yrange.to,
                                     xrange.to - xrange.from,
                                     yrange.from - yrange.to);
            }
          }
        }

        // draw the ticks
        var axes = allAxes(), bw = options.grid.borderWidth;

        for (var j = 0; j < axes.length; ++j) {
          var axis = axes[j], box = axis.box,
                    t = axis.tickLength, x, y, xoff, yoff;
          if (!axis.show || axis.ticks.length == 0)
            continue

          ctx.strokeStyle = axis.options.tickColor || $.color.parse(axis.options.color).scale('a', 0.22).toString();
          ctx.lineWidth = 1;

          // find the edges
          if (axis.direction == "x") {
            x = 0;
            if (t == "full")
              y = (axis.position == "top" ? 0 : plotHeight);
            else
              y = box.top - plotOffset.top + (axis.position == "top" ? box.height : 0);
          }
          else {
            y = 0;
            if (t == "full")
              x = (axis.position == "left" ? 0 : plotWidth);
            else
              x = box.left - plotOffset.left + (axis.position == "left" ? box.width : 0);
          }

          // draw tick bar
          if (!axis.innermost) {
            ctx.beginPath();
            xoff = yoff = 0;
            if (axis.direction == "x")
              xoff = plotWidth;
            else
              yoff = plotHeight;

            if (ctx.lineWidth == 1) {
              x = Math.floor(x) + 0.5;
              y = Math.floor(y) + 0.5;
            }

            ctx.moveTo(x, y);
            ctx.lineTo(x + xoff, y + yoff);
            ctx.stroke();
          }

          // draw ticks
          ctx.beginPath();
          for (i = 0; i < axis.ticks.length; ++i) {
            var v = axis.ticks[i].v;

            xoff = yoff = 0;

            if (v < axis.min || v > axis.max
            // skip those lying on the axes if we got a border
                        || (t == "full" && bw > 0
                            && (v == axis.min || v == axis.max)))
              continue;

            if (axis.direction == "x") {
              x = axis.p2c(v);
              yoff = t == "full" ? -plotHeight : t;

              if (axis.position == "top")
                yoff = -yoff;
            }
            else {
              y = axis.p2c(v);
              xoff = t == "full" ? -plotWidth : t;

              if (axis.position == "left")
                xoff = -xoff;
            }

            if (ctx.lineWidth == 1) {
              if (axis.direction == "x")
                x = Math.floor(x) + 0.5;
              else
                y = Math.floor(y) + 0.5;
            }

            ctx.moveTo(x, y);
            ctx.lineTo(x + xoff, y + yoff);
          }

          ctx.stroke();
        }


        // draw border
        if (bw) {
          ctx.lineWidth = bw;
          ctx.strokeStyle = options.grid.borderColor;
          ctx.strokeRect(-bw / 2, -bw / 2, plotWidth + bw, plotHeight + bw);
        }

        ctx.restore();
      }

      function insertAxisLabels() {
        placeholder.find(".tickLabels").remove();

        var html = ['<div class="tickLabels" style="font-size:smaller">'];

        var axes = allAxes();
        for (var j = 0; j < axes.length; ++j) {
          var axis = axes[j], box = axis.box;
          if (!axis.show)
            continue;
          //debug: html.push('<div style="position:absolute;opacity:0.10;background-color:red;left:' + box.left + 'px;top:' + box.top + 'px;width:' + box.width +  'px;height:' + box.height + 'px"></div>')
          html.push('<div class="' + axis.direction + 'Axis ' + axis.direction + axis.n + 'Axis" style="color:' + axis.options.color + '">');
          for (var i = 0; i < axis.ticks.length; ++i) {
            var tick = axis.ticks[i];
            if (!tick.label || tick.v < axis.min || tick.v > axis.max)
              continue;

            var pos = {}, align;

            if (axis.direction == "x") {
              align = "center";
              pos.left = Math.round(plotOffset.left + axis.p2c(tick.v) - axis.labelWidth / 2);
              if (axis.position == "bottom")
                pos.top = box.top + box.padding;
              else
                pos.bottom = canvasHeight - (box.top + box.height - box.padding);
            }
            else {
              pos.top = Math.round(plotOffset.top + axis.p2c(tick.v) - axis.labelHeight / 2);
              if (axis.position == "left") {
                pos.right = canvasWidth - (box.left + box.width - box.padding)
                align = "right";
              }
              else {
                pos.left = box.left + box.padding;
                align = "left";
              }
            }

            pos.width = axis.labelWidth;

            var style = ["position:absolute", "text-align:" + align];
            for (var a in pos)
              style.push(a + ":" + pos[a] + "px")

            html.push('<div class="tickLabel" style="' + style.join(';') + '">' + tick.label + '</div>');
          }
          html.push('</div>');
        }

        html.push('</div>');

        placeholder.append(html.join(""));
      }

      function drawSeries(series) {
        if (series.lines.show)
          drawSeriesLines(series);
        if (series.bars.show)
          drawSeriesBars(series);
        if (series.points.show)
          drawSeriesPoints(series);
      }

      function drawSeriesLines(series) {
        function plotLine(datapoints, xoffset, yoffset, axisx, axisy) {
          var points = datapoints.points,
                    ps = datapoints.pointsize,
                    prevx = null, prevy = null;

          ctx.beginPath();
          for (var i = ps; i < points.length; i += ps) {
            var x1 = points[i - ps], y1 = points[i - ps + 1],
                        x2 = points[i], y2 = points[i + 1];

            if (x1 == null || x2 == null)
              continue;

            // clip with ymin
            if (y1 <= y2 && y1 < axisy.min) {
              if (y2 < axisy.min)
                continue;   // line segment is outside
              // compute new intersection point
              x1 = (axisy.min - y1) / (y2 - y1) * (x2 - x1) + x1;
              y1 = axisy.min;
            }
            else if (y2 <= y1 && y2 < axisy.min) {
              if (y1 < axisy.min)
                continue;
              x2 = (axisy.min - y1) / (y2 - y1) * (x2 - x1) + x1;
              y2 = axisy.min;
            }

            // clip with ymax
            if (y1 >= y2 && y1 > axisy.max) {
              if (y2 > axisy.max)
                continue;
              x1 = (axisy.max - y1) / (y2 - y1) * (x2 - x1) + x1;
              y1 = axisy.max;
            }
            else if (y2 >= y1 && y2 > axisy.max) {
              if (y1 > axisy.max)
                continue;
              x2 = (axisy.max - y1) / (y2 - y1) * (x2 - x1) + x1;
              y2 = axisy.max;
            }

            // clip with xmin
            if (x1 <= x2 && x1 < axisx.min) {
              if (x2 < axisx.min)
                continue;
              y1 = (axisx.min - x1) / (x2 - x1) * (y2 - y1) + y1;
              x1 = axisx.min;
            }
            else if (x2 <= x1 && x2 < axisx.min) {
              if (x1 < axisx.min)
                continue;
              y2 = (axisx.min - x1) / (x2 - x1) * (y2 - y1) + y1;
              x2 = axisx.min;
            }

            // clip with xmax
            if (x1 >= x2 && x1 > axisx.max) {
              if (x2 > axisx.max)
                continue;
              y1 = (axisx.max - x1) / (x2 - x1) * (y2 - y1) + y1;
              x1 = axisx.max;
            }
            else if (x2 >= x1 && x2 > axisx.max) {
              if (x1 > axisx.max)
                continue;
              y2 = (axisx.max - x1) / (x2 - x1) * (y2 - y1) + y1;
              x2 = axisx.max;
            }

            if (x1 != prevx || y1 != prevy)
              ctx.moveTo(axisx.p2c(x1) + xoffset, axisy.p2c(y1) + yoffset);

            prevx = x2;
            prevy = y2;
            ctx.lineTo(axisx.p2c(x2) + xoffset, axisy.p2c(y2) + yoffset);
          }
          ctx.stroke();
        }

        function plotLineArea(datapoints, axisx, axisy) {
          var points = datapoints.points,
                    ps = datapoints.pointsize,
                    bottom = Math.min(Math.max(0, axisy.min), axisy.max),
                    i = 0, top, areaOpen = false,
                    ypos = 1, segmentStart = 0, segmentEnd = 0;

          // we process each segment in two turns, first forward
          // direction to sketch out top, then once we hit the
          // end we go backwards to sketch the bottom
          while (true) {
            if (ps > 0 && i > points.length + ps)
              break;

            i += ps; // ps is negative if going backwards

            var x1 = points[i - ps],
                        y1 = points[i - ps + ypos],
                        x2 = points[i], y2 = points[i + ypos];

            if (areaOpen) {
              if (ps > 0 && x1 != null && x2 == null) {
                // at turning point
                segmentEnd = i;
                ps = -ps;
                ypos = 2;
                continue;
              }

              if (ps < 0 && i == segmentStart + ps) {
                // done with the reverse sweep
                ctx.fill();
                areaOpen = false;
                ps = -ps;
                ypos = 1;
                i = segmentStart = segmentEnd + ps;
                continue;
              }
            }

            if (x1 == null || x2 == null)
              continue;

            // clip x values

            // clip with xmin
            if (x1 <= x2 && x1 < axisx.min) {
              if (x2 < axisx.min)
                continue;
              y1 = (axisx.min - x1) / (x2 - x1) * (y2 - y1) + y1;
              x1 = axisx.min;
            }
            else if (x2 <= x1 && x2 < axisx.min) {
              if (x1 < axisx.min)
                continue;
              y2 = (axisx.min - x1) / (x2 - x1) * (y2 - y1) + y1;
              x2 = axisx.min;
            }

            // clip with xmax
            if (x1 >= x2 && x1 > axisx.max) {
              if (x2 > axisx.max)
                continue;
              y1 = (axisx.max - x1) / (x2 - x1) * (y2 - y1) + y1;
              x1 = axisx.max;
            }
            else if (x2 >= x1 && x2 > axisx.max) {
              if (x1 > axisx.max)
                continue;
              y2 = (axisx.max - x1) / (x2 - x1) * (y2 - y1) + y1;
              x2 = axisx.max;
            }

            if (!areaOpen) {
              // open area
              ctx.beginPath();
              ctx.moveTo(axisx.p2c(x1), axisy.p2c(bottom));
              areaOpen = true;
            }

            // now first check the case where both is outside
            if (y1 >= axisy.max && y2 >= axisy.max) {
              ctx.lineTo(axisx.p2c(x1), axisy.p2c(axisy.max));
              ctx.lineTo(axisx.p2c(x2), axisy.p2c(axisy.max));
              continue;
            }
            else if (y1 <= axisy.min && y2 <= axisy.min) {
              ctx.lineTo(axisx.p2c(x1), axisy.p2c(axisy.min));
              ctx.lineTo(axisx.p2c(x2), axisy.p2c(axisy.min));
              continue;
            }

            // else it's a bit more complicated, there might
            // be a flat maxed out rectangle first, then a
            // triangular cutout or reverse; to find these
            // keep track of the current x values
            var x1old = x1, x2old = x2;

            // clip the y values, without shortcutting, we
            // go through all cases in turn

            // clip with ymin
            if (y1 <= y2 && y1 < axisy.min && y2 >= axisy.min) {
              x1 = (axisy.min - y1) / (y2 - y1) * (x2 - x1) + x1;
              y1 = axisy.min;
            }
            else if (y2 <= y1 && y2 < axisy.min && y1 >= axisy.min) {
              x2 = (axisy.min - y1) / (y2 - y1) * (x2 - x1) + x1;
              y2 = axisy.min;
            }

            // clip with ymax
            if (y1 >= y2 && y1 > axisy.max && y2 <= axisy.max) {
              x1 = (axisy.max - y1) / (y2 - y1) * (x2 - x1) + x1;
              y1 = axisy.max;
            }
            else if (y2 >= y1 && y2 > axisy.max && y1 <= axisy.max) {
              x2 = (axisy.max - y1) / (y2 - y1) * (x2 - x1) + x1;
              y2 = axisy.max;
            }

            // if the x value was changed we got a rectangle
            // to fill
            if (x1 != x1old) {
              ctx.lineTo(axisx.p2c(x1old), axisy.p2c(y1));
              // it goes to (x1, y1), but we fill that below
            }

            // fill triangular section, this sometimes result
            // in redundant points if (x1, y1) hasn't changed
            // from previous line to, but we just ignore that
            ctx.lineTo(axisx.p2c(x1), axisy.p2c(y1));
            ctx.lineTo(axisx.p2c(x2), axisy.p2c(y2));

            // fill the other rectangle if it's there
            if (x2 != x2old) {
              ctx.lineTo(axisx.p2c(x2), axisy.p2c(y2));
              ctx.lineTo(axisx.p2c(x2old), axisy.p2c(y2));
            }
          }
        }

        ctx.save();
        ctx.translate(plotOffset.left, plotOffset.top);
        ctx.lineJoin = "round";

        var lw = series.lines.lineWidth,
                sw = series.shadowSize;
        // FIXME: consider another form of shadow when filling is turned on
        if (lw > 0 && sw > 0) {
          // draw shadow as a thick and thin line with transparency
          ctx.lineWidth = sw;
          ctx.strokeStyle = "rgba(0,0,0,0.1)";
          // position shadow at angle from the mid of line
          var angle = Math.PI / 18;
          plotLine(series.datapoints, Math.sin(angle) * (lw / 2 + sw / 2), Math.cos(angle) * (lw / 2 + sw / 2), series.xaxis, series.yaxis);
          ctx.lineWidth = sw / 2;
          plotLine(series.datapoints, Math.sin(angle) * (lw / 2 + sw / 4), Math.cos(angle) * (lw / 2 + sw / 4), series.xaxis, series.yaxis);
        }

        ctx.lineWidth = lw;
        ctx.strokeStyle = series.color;
        var fillStyle = getFillStyle(series.lines, series.color, 0, plotHeight);
        if (fillStyle) {
          ctx.fillStyle = fillStyle;
          plotLineArea(series.datapoints, series.xaxis, series.yaxis);
        }

        if (lw > 0)
          plotLine(series.datapoints, 0, 0, series.xaxis, series.yaxis);
        ctx.restore();
      }

      function drawSeriesPoints(series) {
        function plotPoints(datapoints, radius, fillStyle, offset, shadow, axisx, axisy, symbol) {
          var points = datapoints.points, ps = datapoints.pointsize;

          for (var i = 0; i < points.length; i += ps) {
            var x = points[i], y = points[i + 1];
            if (x == null || x < axisx.min || x > axisx.max || y < axisy.min || y > axisy.max)
              continue;

            ctx.beginPath();
            x = axisx.p2c(x);
            y = axisy.p2c(y) + offset;
            if (symbol == "circle")
              ctx.arc(x, y, radius, 0, shadow ? Math.PI : Math.PI * 2, false);
            else
              symbol(ctx, x, y, radius, shadow);
            ctx.closePath();

            if (fillStyle) {
              ctx.fillStyle = fillStyle;
              ctx.fill();
            }
            ctx.stroke();
          }
        }

        ctx.save();
        ctx.translate(plotOffset.left, plotOffset.top);

        var lw = series.points.lineWidth,
                sw = series.shadowSize,
                radius = series.points.radius,
                symbol = series.points.symbol;
        if (lw > 0 && sw > 0) {
          // draw shadow in two steps
          var w = sw / 2;
          ctx.lineWidth = w;
          ctx.strokeStyle = "rgba(0,0,0,0.1)";
          plotPoints(series.datapoints, radius, null, w + w / 2, true,
                           series.xaxis, series.yaxis, symbol);

          ctx.strokeStyle = "rgba(0,0,0,0.2)";
          plotPoints(series.datapoints, radius, null, w / 2, true,
                           series.xaxis, series.yaxis, symbol);
        }

        ctx.lineWidth = lw;
        ctx.strokeStyle = series.color;
        plotPoints(series.datapoints, radius,
                       getFillStyle(series.points, series.color), 0, false,
                       series.xaxis, series.yaxis, symbol);
        ctx.restore();
      }

      function drawBar(x, y, b, barLeft, barRight, offset, fillStyleCallback, axisx, axisy, c, horizontal, lineWidth) {
        var left, right, bottom, top,
                drawLeft, drawRight, drawTop, drawBottom,
                tmp;

        // in horizontal mode, we start the bar from the left
        // instead of from the bottom so it appears to be
        // horizontal rather than vertical
        if (horizontal) {
          drawBottom = drawRight = drawTop = true;
          drawLeft = false;
          left = b;
          right = x;
          top = y + barLeft;
          bottom = y + barRight;

          // account for negative bars
          if (right < left) {
            tmp = right;
            right = left;
            left = tmp;
            drawLeft = true;
            drawRight = false;
          }
        }
        else {
          drawLeft = drawRight = drawTop = true;
          drawBottom = false;
          left = x + barLeft;
          right = x + barRight;
          bottom = b;
          top = y;

          // account for negative bars
          if (top < bottom) {
            tmp = top;
            top = bottom;
            bottom = tmp;
            drawBottom = true;
            drawTop = false;
          }
        }

        // clip
        if (right < axisx.min || left > axisx.max ||
                top < axisy.min || bottom > axisy.max)
          return;

        if (left < axisx.min) {
          left = axisx.min;
          drawLeft = false;
        }

        if (right > axisx.max) {
          right = axisx.max;
          drawRight = false;
        }

        if (bottom < axisy.min) {
          bottom = axisy.min;
          drawBottom = false;
        }

        if (top > axisy.max) {
          top = axisy.max;
          drawTop = false;
        }

        left = axisx.p2c(left);
        bottom = axisy.p2c(bottom);
        right = axisx.p2c(right);
        top = axisy.p2c(top);

        // fill the bar
        if (fillStyleCallback) {
          c.beginPath();
          c.moveTo(left, bottom);
          c.lineTo(left, top);
          c.lineTo(right, top);
          c.lineTo(right, bottom);
          c.fillStyle = fillStyleCallback(bottom, top);
          c.fill();
        }

        // draw outline
        if (lineWidth > 0 && (drawLeft || drawRight || drawTop || drawBottom)) {
          c.beginPath();

          // FIXME: inline moveTo is buggy with excanvas
          c.moveTo(left, bottom + offset);
          if (drawLeft)
            c.lineTo(left, top + offset);
          else
            c.moveTo(left, top + offset);
          if (drawTop)
            c.lineTo(right, top + offset);
          else
            c.moveTo(right, top + offset);
          if (drawRight)
            c.lineTo(right, bottom + offset);
          else
            c.moveTo(right, bottom + offset);
          if (drawBottom)
            c.lineTo(left, bottom + offset);
          else
            c.moveTo(left, bottom + offset);
          c.stroke();
        }
      }

      function drawSeriesBars(series) {
        function plotBars(datapoints, barLeft, barRight, offset, fillStyleCallback, axisx, axisy) {
          var points = datapoints.points, ps = datapoints.pointsize;

          for (var i = 0; i < points.length; i += ps) {
            if (points[i] == null)
              continue;
            drawBar(points[i], points[i + 1], points[i + 2], barLeft, barRight, offset, fillStyleCallback, axisx, axisy, ctx, series.bars.horizontal, series.bars.lineWidth);
          }
        }

        ctx.save();
        ctx.translate(plotOffset.left, plotOffset.top);

        // FIXME: figure out a way to add shadows (for instance along the right edge)
        ctx.lineWidth = series.bars.lineWidth;
        ctx.strokeStyle = series.color;
        var barLeft = series.bars.align == "left" ? 0 : -series.bars.barWidth / 2;
        var fillStyleCallback = series.bars.fill ? function (bottom, top) { return getFillStyle(series.bars, series.color, bottom, top); } : null;
        plotBars(series.datapoints, barLeft, barLeft + series.bars.barWidth, 0, fillStyleCallback, series.xaxis, series.yaxis);
        ctx.restore();
      }

      function getFillStyle(filloptions, seriesColor, bottom, top) {
        var fill = filloptions.fill;
        if (!fill)
          return null;

        if (filloptions.fillColor)
          return getColorOrGradient(filloptions.fillColor, bottom, top, seriesColor);

        var c = $.color.parse(seriesColor);
        c.a = typeof fill == "number" ? fill : 0.4;
        c.normalize();
        return c.toString();
      }

      function insertLegend() {
        placeholder.find(".legend").remove();

        if (!options.legend.show)
          return;

        var fragments = [], rowStarted = false,
                lf = options.legend.labelFormatter, s, label;
        for (var i = 0; i < series.length; ++i) {
          s = series[i];
          label = s.label;
          if (!label)
            continue;

          if (i % options.legend.noColumns == 0) {
            if (rowStarted)
              fragments.push('</tr>');
            fragments.push('<tr>');
            rowStarted = true;
          }

          if (lf)
            label = lf(label, s);

          fragments.push(
                    '<td class="legendColorBox"><div style="border:1px solid ' + options.legend.labelBoxBorderColor + ';padding:1px"><div style="width:4px;height:0;border:5px solid ' + s.color + ';overflow:hidden"></div></div></td>' +
                    '<td class="legendLabel">' + label + '</td>');
        }
        if (rowStarted)
          fragments.push('</tr>');

        if (fragments.length == 0)
          return;

        var table = '<table style="font-size:smaller;color:' + options.grid.color + '">' + fragments.join("") + '</table>';
        if (options.legend.container != null)
          $(options.legend.container).html(table);
        else {
          var pos = "",
                    p = options.legend.position,
                    m = options.legend.margin;
          if (m[0] == null)
            m = [m, m];
          if (p.charAt(0) == "n")
            pos += 'top:' + (m[1] + plotOffset.top) + 'px;';
          else if (p.charAt(0) == "s")
            pos += 'bottom:' + (m[1] + plotOffset.bottom) + 'px;';
          if (p.charAt(1) == "e")
            pos += 'right:' + (m[0] + plotOffset.right) + 'px;';
          else if (p.charAt(1) == "w")
            pos += 'left:' + (m[0] + plotOffset.left) + 'px;';
          var legend = $('<div class="legend">' + table.replace('style="', 'style="position:absolute;' + pos + ';') + '</div>').appendTo(placeholder);
          if (options.legend.backgroundOpacity != 0.0) {
            // put in the transparent background
            // separately to avoid blended labels and
            // label boxes
            var c = options.legend.backgroundColor;
            if (c == null) {
              c = options.grid.backgroundColor;
              if (c && typeof c == "string")
                c = $.color.parse(c);
              else
                c = $.color.extract(legend, 'background-color');
              c.a = 1;
              c = c.toString();
            }
            var div = legend.children();
            $('<div style="position:absolute;width:' + div.width() + 'px;height:' + div.height() + 'px;' + pos + 'background-color:' + c + ';"> </div>').prependTo(legend).css('opacity', options.legend.backgroundOpacity);
          }
        }
      }


      // interactive features

      var highlights = [],
            redrawTimeout = null;

      // returns the data item the mouse is over, or null if none is found
      function findNearbyItem(mouseX, mouseY, seriesFilter) {
        var maxDistance = options.grid.mouseActiveRadius,
                smallestDistance = maxDistance * maxDistance + 1,
                item = null, foundPoint = false, i, j;

        for (i = series.length - 1; i >= 0; --i) {
          if (!seriesFilter(series[i]))
            continue;

          var s = series[i],
                    axisx = s.xaxis,
                    axisy = s.yaxis,
                    points = s.datapoints.points,
                    ps = s.datapoints.pointsize,
                    mx = axisx.c2p(mouseX), // precompute some stuff to make the loop faster
                    my = axisy.c2p(mouseY),
                    maxx = maxDistance / axisx.scale,
                    maxy = maxDistance / axisy.scale;

          // with inverse transforms, we can't use the maxx/maxy
          // optimization, sadly
          if (axisx.options.inverseTransform)
            maxx = Number.MAX_VALUE;
          if (axisy.options.inverseTransform)
            maxy = Number.MAX_VALUE;

          if (s.lines.show || s.points.show) {
            for (j = 0; j < points.length; j += ps) {
              var x = points[j], y = points[j + 1];
              if (x == null)
                continue;

              // For points and lines, the cursor must be within a
              // certain distance to the data point
              if (x - mx > maxx || x - mx < -maxx ||
                            y - my > maxy || y - my < -maxy)
                continue;

              // We have to calculate distances in pixels, not in
              // data units, because the scales of the axes may be different
              var dx = Math.abs(axisx.p2c(x) - mouseX),
                            dy = Math.abs(axisy.p2c(y) - mouseY),
                            dist = dx * dx + dy * dy; // we save the sqrt

              // use <= to ensure last point takes precedence
              // (last generally means on top of)
              if (dist < smallestDistance) {
                smallestDistance = dist;
                item = [i, j / ps];
              }
            }
          }

          if (s.bars.show && !item) { // no other point can be nearby
            var barLeft = s.bars.align == "left" ? 0 : -s.bars.barWidth / 2,
                        barRight = barLeft + s.bars.barWidth;

            for (j = 0; j < points.length; j += ps) {
              var x = points[j], y = points[j + 1], b = points[j + 2];
              if (x == null)
                continue;

              // for a bar graph, the cursor must be inside the bar
              if (series[i].bars.horizontal ?
                            (mx <= Math.max(b, x) && mx >= Math.min(b, x) &&
                             my >= y + barLeft && my <= y + barRight) :
                            (mx >= x + barLeft && mx <= x + barRight &&
                             my >= Math.min(b, y) && my <= Math.max(b, y)))
                item = [i, j / ps];
            }
          }
        }

        if (item) {
          i = item[0];
          j = item[1];
          ps = series[i].datapoints.pointsize;

          return { datapoint: series[i].datapoints.points.slice(j * ps, (j + 1) * ps),
            dataIndex: j,
            series: series[i],
            seriesIndex: i
          };
        }

        return null;
      }

      function onMouseMove(e) {
        if (options.grid.hoverable)
          triggerClickHoverEvent("plothover", e,
                                       function (s) { return s["hoverable"] != false; });
      }

      function onMouseLeave(e) {
        if (options.grid.hoverable)
          triggerClickHoverEvent("plothover", e,
                                       function (s) { return false; });
      }

      function onClick(e) {
        triggerClickHoverEvent("plotclick", e,
                                   function (s) { return s["clickable"] != false; });
      }

      // trigger click or hover event (they send the same parameters
      // so we share their code)
      function triggerClickHoverEvent(eventname, event, seriesFilter) {
        var offset = eventHolder.offset(),
                canvasX = event.pageX - offset.left - plotOffset.left,
                canvasY = event.pageY - offset.top - plotOffset.top,
            pos = canvasToAxisCoords({ left: canvasX, top: canvasY });

        pos.pageX = event.pageX;
        pos.pageY = event.pageY;

        var item = findNearbyItem(canvasX, canvasY, seriesFilter);

        if (item) {
          // fill in mouse pos for any listeners out there
          item.pageX = parseInt(item.series.xaxis.p2c(item.datapoint[0]) + offset.left + plotOffset.left);
          item.pageY = parseInt(item.series.yaxis.p2c(item.datapoint[1]) + offset.top + plotOffset.top);
        }

        if (options.grid.autoHighlight) {
          // clear auto-highlights
          for (var i = 0; i < highlights.length; ++i) {
            var h = highlights[i];
            if (h.auto == eventname &&
                        !(item && h.series == item.series &&
                          h.point[0] == item.datapoint[0] &&
                          h.point[1] == item.datapoint[1]))
              unhighlight(h.series, h.point);
          }

          if (item)
            highlight(item.series, item.datapoint, eventname);
        }

        placeholder.trigger(eventname, [pos, item]);
      }

      function triggerRedrawOverlay() {
        if (!redrawTimeout)
          redrawTimeout = setTimeout(drawOverlay, 30);
      }

      function drawOverlay() {
        redrawTimeout = null;

        // draw highlights
        octx.save();
        octx.clearRect(0, 0, canvasWidth, canvasHeight);
        octx.translate(plotOffset.left, plotOffset.top);

        var i, hi;
        for (i = 0; i < highlights.length; ++i) {
          hi = highlights[i];

          if (hi.series.bars.show)
            drawBarHighlight(hi.series, hi.point);
          else
            drawPointHighlight(hi.series, hi.point);
        }
        octx.restore();

        executeHooks(hooks.drawOverlay, [octx]);
      }

      function highlight(s, point, auto) {
        if (typeof s == "number")
          s = series[s];

        if (typeof point == "number") {
          var ps = s.datapoints.pointsize;
          point = s.datapoints.points.slice(ps * point, ps * (point + 1));
        }

        var i = indexOfHighlight(s, point);
        if (i == -1) {
          highlights.push({ series: s, point: point, auto: auto });

          triggerRedrawOverlay();
        }
        else if (!auto)
          highlights[i].auto = false;
      }

      function unhighlight(s, point) {
        if (s == null && point == null) {
          highlights = [];
          triggerRedrawOverlay();
        }

        if (typeof s == "number")
          s = series[s];

        if (typeof point == "number")
          point = s.data[point];

        var i = indexOfHighlight(s, point);
        if (i != -1) {
          highlights.splice(i, 1);

          triggerRedrawOverlay();
        }
      }

      function indexOfHighlight(s, p) {
        for (var i = 0; i < highlights.length; ++i) {
          var h = highlights[i];
          if (h.series == s && h.point[0] == p[0]
                    && h.point[1] == p[1])
            return i;
        }
        return -1;
      }

      function drawPointHighlight(series, point) {
        var x = point[0], y = point[1],
                axisx = series.xaxis, axisy = series.yaxis;

        if (x < axisx.min || x > axisx.max || y < axisy.min || y > axisy.max)
          return;

        var pointRadius = series.points.radius + series.points.lineWidth / 2;
        octx.lineWidth = pointRadius;
        octx.strokeStyle = $.color.parse(series.color).scale('a', 0.5).toString();
        var radius = 1.5 * pointRadius,
                x = axisx.p2c(x),
                y = axisy.p2c(y);

        octx.beginPath();
        if (series.points.symbol == "circle")
          octx.arc(x, y, radius, 0, 2 * Math.PI, false);
        else
          series.points.symbol(octx, x, y, radius, false);
        octx.closePath();
        octx.stroke();
      }

      function drawBarHighlight(series, point) {
        octx.lineWidth = series.bars.lineWidth;
        octx.strokeStyle = $.color.parse(series.color).scale('a', 0.5).toString();
        var fillStyle = $.color.parse(series.color).scale('a', 0.5).toString();
        var barLeft = series.bars.align == "left" ? 0 : -series.bars.barWidth / 2;
        drawBar(point[0], point[1], point[2] || 0, barLeft, barLeft + series.bars.barWidth,
                    0, function () { return fillStyle; }, series.xaxis, series.yaxis, octx, series.bars.horizontal, series.bars.lineWidth);
      }

      function getColorOrGradient(spec, bottom, top, defaultColor) {
        if (typeof spec == "string")
          return spec;
        else {
          // assume this is a gradient spec; IE currently only
          // supports a simple vertical gradient properly, so that's
          // what we support too
          var gradient = ctx.createLinearGradient(0, top, 0, bottom);

          for (var i = 0, l = spec.colors.length; i < l; ++i) {
            var c = spec.colors[i];
            if (typeof c != "string") {
              var co = $.color.parse(defaultColor);
              if (c.brightness != null)
                co = co.scale('rgb', c.brightness)
              if (c.opacity != null)
                co.a *= c.opacity;
              c = co.toString();
            }
            gradient.addColorStop(i / (l - 1), c);
          }

          return gradient;
        }
      }
    }

    $.plot = function (placeholder, data, options) {
      //var t0 = new Date();
      var plot = new Plot($(placeholder), data, options, $.plot.plugins);
      //(window.console ? console.log : alert)("time used (msecs): " + ((new Date()).getTime() - t0.getTime()));
      return plot;
    };

    $.plot.version = "0.7";

    $.plot.plugins = [];

    // returns a string with the date d formatted according to fmt
    $.plot.formatDate = function (d, fmt, monthNames) {
      var leftPad = function (n) {
        n = "" + n;
        return n.length == 1 ? "0" + n : n;
      };

      var r = [];
      var escape = false, padNext = false;
      var hours = d.getUTCHours();
      var isAM = hours < 12;
      if (monthNames == null)
        monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

      if (fmt.search(/%p|%P/) != -1) {
        if (hours > 12) {
          hours = hours - 12;
        } else if (hours == 0) {
          hours = 12;
        }
      }
      for (var i = 0; i < fmt.length; ++i) {
        var c = fmt.charAt(i);

        if (escape) {
          switch (c) {
            case 'h': c = "" + hours; break;
            case 'H': c = leftPad(hours); break;
            case 'M': c = leftPad(d.getUTCMinutes()); break;
            case 'S': c = leftPad(d.getUTCSeconds()); break;
            case 'd': c = "" + d.getUTCDate(); break;
            case 'm': c = "" + (d.getUTCMonth() + 1); break;
            case 'y': c = "" + d.getUTCFullYear(); break;
            case 'b': c = "" + monthNames[d.getUTCMonth()]; break;
            case 'p': c = (isAM) ? ("" + "am") : ("" + "pm"); break;
            case 'P': c = (isAM) ? ("" + "AM") : ("" + "PM"); break;
            case '0': c = ""; padNext = true; break;
          }
          if (c && padNext) {
            c = leftPad(c);
            padNext = false;
          }
          r.push(c);
          if (!padNext)
            escape = false;
        }
        else {
          if (c == "%")
            escape = true;
          else
            r.push(c);
        }
      }
      return r.join("");
    };

    // round to nearby lower multiple of base
    function floorInBase(n, base) {
      return base * Math.floor(n / base);
    }

  })(jQuery);

/*!
* jQuery Cycle Plugin (with Transition Definitions)
* Examples and documentation at: http://jquery.malsup.com/cycle/
* Copyright (c) 2007-2010 M. Alsup
* Version: 2.9997 (13-OCT-2011)
* Dual licensed under the MIT and GPL licenses.
* http://jquery.malsup.com/license.html
* Requires: jQuery v1.3.2 or later
*/
; (function ($) {

  var ver = '2.9997';

  // if $.support is not defined (pre jQuery 1.3) add what I need
  if ($.support == undefined) {
    $.support = {
      opacity: !($.browser.msie)
    };
  }

  function debug(s) {
    $.fn.cycle.debug && log(s);
  }
  function log() {
    window.console && console.log && console.log('[cycle] ' + Array.prototype.join.call(arguments, ' '));
  }
  $.expr[':'].paused = function (el) {
    return el.cyclePause;
  }


  // the options arg can be...
  //   a number  - indicates an immediate transition should occur to the given slide index
  //   a string  - 'pause', 'resume', 'toggle', 'next', 'prev', 'stop', 'destroy' or the name of a transition effect (ie, 'fade', 'zoom', etc)
  //   an object - properties to control the slideshow
  //
  // the arg2 arg can be...
  //   the name of an fx (only used in conjunction with a numeric value for 'options')
  //   the value true (only used in first arg == 'resume') and indicates
  //	 that the resume should occur immediately (not wait for next timeout)

  $.fn.cycle = function (options, arg2) {
    var o = { s: this.selector, c: this.context };

    // in 1.3+ we can fix mistakes with the ready state
    if (this.length === 0 && options != 'stop') {
      if (!$.isReady && o.s) {
        log('DOM not ready, queuing slideshow');
        $(function () {
          $(o.s, o.c).cycle(options, arg2);
        });
        return this;
      }
      // is your DOM ready?  http://docs.jquery.com/Tutorials:Introducing_$(document).ready()
      log('terminating; zero elements found by selector' + ($.isReady ? '' : ' (DOM not ready)'));
      return this;
    }

    // iterate the matched nodeset
    return this.each(function () {
      var opts = handleArguments(this, options, arg2);
      if (opts === false)
        return;

      opts.updateActivePagerLink = opts.updateActivePagerLink || $.fn.cycle.updateActivePagerLink;

      // stop existing slideshow for this container (if there is one)
      if (this.cycleTimeout)
        clearTimeout(this.cycleTimeout);
      this.cycleTimeout = this.cyclePause = 0;

      var $cont = $(this);
      var $slides = opts.slideExpr ? $(opts.slideExpr, this) : $cont.children();
      var els = $slides.get();

      var opts2 = buildOptions($cont, $slides, els, opts, o);
      if (opts2 === false)
        return;

      if (els.length < 2) {
        log('terminating; too few slides: ' + els.length);
        return;
      }

      var startTime = opts2.continuous ? 10 : getTimeout(els[opts2.currSlide], els[opts2.nextSlide], opts2, !opts2.backwards);

      // if it's an auto slideshow, kick it off
      if (startTime) {
        startTime += (opts2.delay || 0);
        if (startTime < 10)
          startTime = 10;
        debug('first timeout: ' + startTime);
        this.cycleTimeout = setTimeout(function () { go(els, opts2, 0, !opts.backwards) }, startTime);
      }
    });
  };

  function triggerPause(cont, byHover, onPager) {
    var opts = $(cont).data('cycle.opts');
    var paused = !!cont.cyclePause;
    if (paused && opts.paused)
      opts.paused(cont, opts, byHover, onPager);
    else if (!paused && opts.resumed)
      opts.resumed(cont, opts, byHover, onPager);
  }

  // process the args that were passed to the plugin fn
  function handleArguments(cont, options, arg2) {
    if (cont.cycleStop == undefined)
      cont.cycleStop = 0;
    if (options === undefined || options === null)
      options = {};
    if (options.constructor == String) {
      switch (options) {
        case 'destroy':
        case 'stop':
          var opts = $(cont).data('cycle.opts');
          if (!opts)
            return false;
          cont.cycleStop++; // callbacks look for change
          if (cont.cycleTimeout)
            clearTimeout(cont.cycleTimeout);
          cont.cycleTimeout = 0;
          opts.elements && $(opts.elements).stop();
          $(cont).removeData('cycle.opts');
          if (options == 'destroy')
            destroy(opts);
          return false;
        case 'toggle':
          cont.cyclePause = (cont.cyclePause === 1) ? 0 : 1;
          checkInstantResume(cont.cyclePause, arg2, cont);
          triggerPause(cont);
          return false;
        case 'pause':
          cont.cyclePause = 1;
          triggerPause(cont);
          return false;
        case 'resume':
          cont.cyclePause = 0;
          checkInstantResume(false, arg2, cont);
          triggerPause(cont);
          return false;
        case 'prev':
        case 'next':
          var opts = $(cont).data('cycle.opts');
          if (!opts) {
            log('options not found, "prev/next" ignored');
            return false;
          }
          $.fn.cycle[options](opts);
          return false;
        default:
          options = { fx: options };
      };
      return options;
    }
    else if (options.constructor == Number) {
      // go to the requested slide
      var num = options;
      options = $(cont).data('cycle.opts');
      if (!options) {
        log('options not found, can not advance slide');
        return false;
      }
      if (num < 0 || num >= options.elements.length) {
        log('invalid slide index: ' + num);
        return false;
      }
      options.nextSlide = num;
      if (cont.cycleTimeout) {
        clearTimeout(cont.cycleTimeout);
        cont.cycleTimeout = 0;
      }
      if (typeof arg2 == 'string')
        options.oneTimeFx = arg2;
      go(options.elements, options, 1, num >= options.currSlide);
      return false;
    }
    return options;

    function checkInstantResume(isPaused, arg2, cont) {
      if (!isPaused && arg2 === true) { // resume now!
        var options = $(cont).data('cycle.opts');
        if (!options) {
          log('options not found, can not resume');
          return false;
        }
        if (cont.cycleTimeout) {
          clearTimeout(cont.cycleTimeout);
          cont.cycleTimeout = 0;
        }
        go(options.elements, options, 1, !options.backwards);
      }
    }
  };

  function removeFilter(el, opts) {
    if (!$.support.opacity && opts.cleartype && el.style.filter) {
      try { el.style.removeAttribute('filter'); }
      catch (smother) { } // handle old opera versions
    }
  };

  // unbind event handlers
  function destroy(opts) {
    if (opts.next)
      $(opts.next).unbind(opts.prevNextEvent);
    if (opts.prev)
      $(opts.prev).unbind(opts.prevNextEvent);

    if (opts.pager || opts.pagerAnchorBuilder)
      $.each(opts.pagerAnchors || [], function () {
        this.unbind().remove();
      });
    opts.pagerAnchors = null;
    if (opts.destroy) // callback
      opts.destroy(opts);
  };

  // one-time initialization
  function buildOptions($cont, $slides, els, options, o) {
    // support metadata plugin (v1.0 and v2.0)
    var opts = $.extend({}, $.fn.cycle.defaults, options || {}, $.metadata ? $cont.metadata() : $.meta ? $cont.data() : {});
    var meta = $.isFunction($cont.data) ? $cont.data(opts.metaAttr) : null;
    if (meta)
      opts = $.extend(opts, meta);
    if (opts.autostop)
      opts.countdown = opts.autostopCount || els.length;

    var cont = $cont[0];
    $cont.data('cycle.opts', opts);
    opts.$cont = $cont;
    opts.stopCount = cont.cycleStop;
    opts.elements = els;
    opts.before = opts.before ? [opts.before] : [];
    opts.after = opts.after ? [opts.after] : [];

    // push some after callbacks
    if (!$.support.opacity && opts.cleartype)
      opts.after.push(function () { removeFilter(this, opts); });
    if (opts.continuous)
      opts.after.push(function () { go(els, opts, 0, !opts.backwards); });

    saveOriginalOpts(opts);

    // clearType corrections
    if (!$.support.opacity && opts.cleartype && !opts.cleartypeNoBg)
      clearTypeFix($slides);

    // container requires non-static position so that slides can be position within
    if ($cont.css('position') == 'static')
      $cont.css('position', 'relative');
    if (opts.width)
      $cont.width(opts.width);
    if (opts.height && opts.height != 'auto')
      $cont.height(opts.height);

    if (opts.startingSlide)
      opts.startingSlide = parseInt(opts.startingSlide, 10);
    else if (opts.backwards)
      opts.startingSlide = els.length - 1;

    // if random, mix up the slide array
    if (opts.random) {
      opts.randomMap = [];
      for (var i = 0; i < els.length; i++)
        opts.randomMap.push(i);
      opts.randomMap.sort(function (a, b) { return Math.random() - 0.5; });
      opts.randomIndex = 1;
      opts.startingSlide = opts.randomMap[1];
    }
    else if (opts.startingSlide >= els.length)
      opts.startingSlide = 0; // catch bogus input
    opts.currSlide = opts.startingSlide || 0;
    var first = opts.startingSlide;

    // set position and zIndex on all the slides
    $slides.css({ position: 'absolute', top: 0, left: 0 }).hide().each(function (i) {
      var z;
      if (opts.backwards)
        z = first ? i <= first ? els.length + (i - first) : first - i : els.length - i;
      else
        z = first ? i >= first ? els.length - (i - first) : first - i : els.length - i;
      $(this).css('z-index', z)
    });

    // make sure first slide is visible
    $(els[first]).css('opacity', 1).show(); // opacity bit needed to handle restart use case
    removeFilter(els[first], opts);

    // stretch slides
    if (opts.fit) {
      if (!opts.aspect) {
        if (opts.width)
          $slides.width(opts.width);
        if (opts.height && opts.height != 'auto')
          $slides.height(opts.height);
      } else {
        $slides.each(function () {
          var $slide = $(this);
          var ratio = (opts.aspect === true) ? $slide.width() / $slide.height() : opts.aspect;
          if (opts.width && $slide.width() != opts.width) {
            $slide.width(opts.width);
            $slide.height(opts.width / ratio);
          }

          if (opts.height && $slide.height() < opts.height) {
            $slide.height(opts.height);
            $slide.width(opts.height * ratio);
          }
        });
      }
    }

    if (opts.center && ((!opts.fit) || opts.aspect)) {
      $slides.each(function () {
        var $slide = $(this);
        $slide.css({
          "margin-left": opts.width ?
				((opts.width - $slide.width()) / 2) + "px" :
				0,
          "margin-top": opts.height ?
				((opts.height - $slide.height()) / 2) + "px" :
				0
        });
      });
    }

    if (opts.center && !opts.fit && !opts.slideResize) {
      $slides.each(function () {
        var $slide = $(this);
        $slide.css({
          "margin-left": opts.width ? ((opts.width - $slide.width()) / 2) + "px" : 0,
          "margin-top": opts.height ? ((opts.height - $slide.height()) / 2) + "px" : 0
        });
      });
    }

    // stretch container
    var reshape = opts.containerResize && !$cont.innerHeight();
    if (reshape) { // do this only if container has no size http://tinyurl.com/da2oa9
      var maxw = 0, maxh = 0;
      for (var j = 0; j < els.length; j++) {
        var $e = $(els[j]), e = $e[0], w = $e.outerWidth(), h = $e.outerHeight();
        if (!w) w = e.offsetWidth || e.width || $e.attr('width');
        if (!h) h = e.offsetHeight || e.height || $e.attr('height');
        maxw = w > maxw ? w : maxw;
        maxh = h > maxh ? h : maxh;
      }
      if (maxw > 0 && maxh > 0)
        $cont.css({ width: maxw + 'px', height: maxh + 'px' });
    }

    var pauseFlag = false;  // https://github.com/malsup/cycle/issues/44
    if (opts.pause)
      $cont.hover(
		function () {
		  pauseFlag = true;
		  this.cyclePause++;
		  triggerPause(cont, true);
		},
		function () {
		  pauseFlag && this.cyclePause--;
		  triggerPause(cont, true);
		}
	);

    if (supportMultiTransitions(opts) === false)
      return false;

    // apparently a lot of people use image slideshows without height/width attributes on the images.
    // Cycle 2.50+ requires the sizing info for every slide; this block tries to deal with that.
    var requeue = false;
    options.requeueAttempts = options.requeueAttempts || 0;
    $slides.each(function () {
      // try to get height/width of each slide
      var $el = $(this);
      this.cycleH = (opts.fit && opts.height) ? opts.height : ($el.height() || this.offsetHeight || this.height || $el.attr('height') || 0);
      this.cycleW = (opts.fit && opts.width) ? opts.width : ($el.width() || this.offsetWidth || this.width || $el.attr('width') || 0);

      if ($el.is('img')) {
        // sigh..  sniffing, hacking, shrugging...  this crappy hack tries to account for what browsers do when
        // an image is being downloaded and the markup did not include sizing info (height/width attributes);
        // there seems to be some "default" sizes used in this situation
        var loadingIE = ($.browser.msie && this.cycleW == 28 && this.cycleH == 30 && !this.complete);
        var loadingFF = ($.browser.mozilla && this.cycleW == 34 && this.cycleH == 19 && !this.complete);
        var loadingOp = ($.browser.opera && ((this.cycleW == 42 && this.cycleH == 19) || (this.cycleW == 37 && this.cycleH == 17)) && !this.complete);
        var loadingOther = (this.cycleH == 0 && this.cycleW == 0 && !this.complete);
        // don't requeue for images that are still loading but have a valid size
        if (loadingIE || loadingFF || loadingOp || loadingOther) {
          if (o.s && opts.requeueOnImageNotLoaded && ++options.requeueAttempts < 100) { // track retry count so we don't loop forever
            log(options.requeueAttempts, ' - img slide not loaded, requeuing slideshow: ', this.src, this.cycleW, this.cycleH);
            setTimeout(function () { $(o.s, o.c).cycle(options) }, opts.requeueTimeout);
            requeue = true;
            return false; // break each loop
          }
          else {
            log('could not determine size of image: ' + this.src, this.cycleW, this.cycleH);
          }
        }
      }
      return true;
    });

    if (requeue)
      return false;

    opts.cssBefore = opts.cssBefore || {};
    opts.cssAfter = opts.cssAfter || {};
    opts.cssFirst = opts.cssFirst || {};
    opts.animIn = opts.animIn || {};
    opts.animOut = opts.animOut || {};

    $slides.not(':eq(' + first + ')').css(opts.cssBefore);
    $($slides[first]).css(opts.cssFirst);

    if (opts.timeout) {
      opts.timeout = parseInt(opts.timeout, 10);
      // ensure that timeout and speed settings are sane
      if (opts.speed.constructor == String)
        opts.speed = $.fx.speeds[opts.speed] || parseInt(opts.speed, 10);
      if (!opts.sync)
        opts.speed = opts.speed / 2;

      var buffer = opts.fx == 'none' ? 0 : opts.fx == 'shuffle' ? 500 : 250;
      while ((opts.timeout - opts.speed) < buffer) // sanitize timeout
        opts.timeout += opts.speed;
    }
    if (opts.easing)
      opts.easeIn = opts.easeOut = opts.easing;
    if (!opts.speedIn)
      opts.speedIn = opts.speed;
    if (!opts.speedOut)
      opts.speedOut = opts.speed;

    opts.slideCount = els.length;
    opts.currSlide = opts.lastSlide = first;
    if (opts.random) {
      if (++opts.randomIndex == els.length)
        opts.randomIndex = 0;
      opts.nextSlide = opts.randomMap[opts.randomIndex];
    }
    else if (opts.backwards)
      opts.nextSlide = opts.startingSlide == 0 ? (els.length - 1) : opts.startingSlide - 1;
    else
      opts.nextSlide = opts.startingSlide >= (els.length - 1) ? 0 : opts.startingSlide + 1;

    // run transition init fn
    if (!opts.multiFx) {
      var init = $.fn.cycle.transitions[opts.fx];
      if ($.isFunction(init))
        init($cont, $slides, opts);
      else if (opts.fx != 'custom' && !opts.multiFx) {
        log('unknown transition: ' + opts.fx, '; slideshow terminating');
        return false;
      }
    }

    // fire artificial events
    var e0 = $slides[first];
    if (!opts.skipInitializationCallbacks) {
      if (opts.before.length)
        opts.before[0].apply(e0, [e0, e0, opts, true]);
      if (opts.after.length)
        opts.after[0].apply(e0, [e0, e0, opts, true]);
    }
    if (opts.next)
      $(opts.next).bind(opts.prevNextEvent, function () { return advance(opts, 1) });
    if (opts.prev)
      $(opts.prev).bind(opts.prevNextEvent, function () { return advance(opts, 0) });
    if (opts.pager || opts.pagerAnchorBuilder)
      buildPager(els, opts);

    exposeAddSlide(opts, els);

    return opts;
  };

  // save off original opts so we can restore after clearing state
  function saveOriginalOpts(opts) {
    opts.original = { before: [], after: [] };
    opts.original.cssBefore = $.extend({}, opts.cssBefore);
    opts.original.cssAfter = $.extend({}, opts.cssAfter);
    opts.original.animIn = $.extend({}, opts.animIn);
    opts.original.animOut = $.extend({}, opts.animOut);
    $.each(opts.before, function () { opts.original.before.push(this); });
    $.each(opts.after, function () { opts.original.after.push(this); });
  };

  function supportMultiTransitions(opts) {
    var i, tx, txs = $.fn.cycle.transitions;
    // look for multiple effects
    if (opts.fx.indexOf(',') > 0) {
      opts.multiFx = true;
      opts.fxs = opts.fx.replace(/\s*/g, '').split(',');
      // discard any bogus effect names
      for (i = 0; i < opts.fxs.length; i++) {
        var fx = opts.fxs[i];
        tx = txs[fx];
        if (!tx || !txs.hasOwnProperty(fx) || !$.isFunction(tx)) {
          log('discarding unknown transition: ', fx);
          opts.fxs.splice(i, 1);
          i--;
        }
      }
      // if we have an empty list then we threw everything away!
      if (!opts.fxs.length) {
        log('No valid transitions named; slideshow terminating.');
        return false;
      }
    }
    else if (opts.fx == 'all') {  // auto-gen the list of transitions
      opts.multiFx = true;
      opts.fxs = [];
      for (p in txs) {
        tx = txs[p];
        if (txs.hasOwnProperty(p) && $.isFunction(tx))
          opts.fxs.push(p);
      }
    }
    if (opts.multiFx && opts.randomizeEffects) {
      // munge the fxs array to make effect selection random
      var r1 = Math.floor(Math.random() * 20) + 30;
      for (i = 0; i < r1; i++) {
        var r2 = Math.floor(Math.random() * opts.fxs.length);
        opts.fxs.push(opts.fxs.splice(r2, 1)[0]);
      }
      debug('randomized fx sequence: ', opts.fxs);
    }
    return true;
  };

  // provide a mechanism for adding slides after the slideshow has started
  function exposeAddSlide(opts, els) {
    opts.addSlide = function (newSlide, prepend) {
      var $s = $(newSlide), s = $s[0];
      if (!opts.autostopCount)
        opts.countdown++;
      els[prepend ? 'unshift' : 'push'](s);
      if (opts.els)
        opts.els[prepend ? 'unshift' : 'push'](s); // shuffle needs this
      opts.slideCount = els.length;

      $s.css('position', 'absolute');
      $s[prepend ? 'prependTo' : 'appendTo'](opts.$cont);

      if (prepend) {
        opts.currSlide++;
        opts.nextSlide++;
      }

      if (!$.support.opacity && opts.cleartype && !opts.cleartypeNoBg)
        clearTypeFix($s);

      if (opts.fit && opts.width)
        $s.width(opts.width);
      if (opts.fit && opts.height && opts.height != 'auto')
        $s.height(opts.height);
      s.cycleH = (opts.fit && opts.height) ? opts.height : $s.height();
      s.cycleW = (opts.fit && opts.width) ? opts.width : $s.width();

      $s.css(opts.cssBefore);

      if (opts.pager || opts.pagerAnchorBuilder)
        $.fn.cycle.createPagerAnchor(els.length - 1, s, $(opts.pager), els, opts);

      if ($.isFunction(opts.onAddSlide))
        opts.onAddSlide($s);
      else
        $s.hide(); // default behavior
    };
  }

  // reset internal state; we do this on every pass in order to support multiple effects
  $.fn.cycle.resetState = function (opts, fx) {
    fx = fx || opts.fx;
    opts.before = []; opts.after = [];
    opts.cssBefore = $.extend({}, opts.original.cssBefore);
    opts.cssAfter = $.extend({}, opts.original.cssAfter);
    opts.animIn = $.extend({}, opts.original.animIn);
    opts.animOut = $.extend({}, opts.original.animOut);
    opts.fxFn = null;
    $.each(opts.original.before, function () { opts.before.push(this); });
    $.each(opts.original.after, function () { opts.after.push(this); });

    // re-init
    var init = $.fn.cycle.transitions[fx];
    if ($.isFunction(init))
      init(opts.$cont, $(opts.elements), opts);
  };

  // this is the main engine fn, it handles the timeouts, callbacks and slide index mgmt
  function go(els, opts, manual, fwd) {
    // opts.busy is true if we're in the middle of an animation
    if (manual && opts.busy && opts.manualTrump) {
      // let manual transitions requests trump active ones
      debug('manualTrump in go(), stopping active transition');
      $(els).stop(true, true);
      opts.busy = 0;
    }
    // don't begin another timeout-based transition if there is one active
    if (opts.busy) {
      debug('transition active, ignoring new tx request');
      return;
    }

    var p = opts.$cont[0], curr = els[opts.currSlide], next = els[opts.nextSlide];

    // stop cycling if we have an outstanding stop request
    if (p.cycleStop != opts.stopCount || p.cycleTimeout === 0 && !manual)
      return;

    // check to see if we should stop cycling based on autostop options
    if (!manual && !p.cyclePause && !opts.bounce &&
	((opts.autostop && (--opts.countdown <= 0)) ||
	(opts.nowrap && !opts.random && opts.nextSlide < opts.currSlide))) {
      if (opts.end)
        opts.end(opts);
      return;
    }

    // if slideshow is paused, only transition on a manual trigger
    var changed = false;
    if ((manual || !p.cyclePause) && (opts.nextSlide != opts.currSlide)) {
      changed = true;
      var fx = opts.fx;
      // keep trying to get the slide size if we don't have it yet
      curr.cycleH = curr.cycleH || $(curr).height();
      curr.cycleW = curr.cycleW || $(curr).width();
      next.cycleH = next.cycleH || $(next).height();
      next.cycleW = next.cycleW || $(next).width();

      // support multiple transition types
      if (opts.multiFx) {
        if (fwd && (opts.lastFx == undefined || ++opts.lastFx >= opts.fxs.length))
          opts.lastFx = 0;
        else if (!fwd && (opts.lastFx == undefined || --opts.lastFx < 0))
          opts.lastFx = opts.fxs.length - 1;
        fx = opts.fxs[opts.lastFx];
      }

      // one-time fx overrides apply to:  $('div').cycle(3,'zoom');
      if (opts.oneTimeFx) {
        fx = opts.oneTimeFx;
        opts.oneTimeFx = null;
      }

      $.fn.cycle.resetState(opts, fx);

      // run the before callbacks
      if (opts.before.length)
        $.each(opts.before, function (i, o) {
          if (p.cycleStop != opts.stopCount) return;
          o.apply(next, [curr, next, opts, fwd]);
        });

      // stage the after callacks
      var after = function () {
        opts.busy = 0;
        $.each(opts.after, function (i, o) {
          if (p.cycleStop != opts.stopCount) return;
          o.apply(next, [curr, next, opts, fwd]);
        });
        if (!p.cycleStop) {
          // queue next transition
          queueNext();
        }
      };

      debug('tx firing(' + fx + '); currSlide: ' + opts.currSlide + '; nextSlide: ' + opts.nextSlide);

      // get ready to perform the transition
      opts.busy = 1;
      if (opts.fxFn) // fx function provided?
        opts.fxFn(curr, next, opts, after, fwd, manual && opts.fastOnEvent);
      else if ($.isFunction($.fn.cycle[opts.fx])) // fx plugin ?
        $.fn.cycle[opts.fx](curr, next, opts, after, fwd, manual && opts.fastOnEvent);
      else
        $.fn.cycle.custom(curr, next, opts, after, fwd, manual && opts.fastOnEvent);
    }
    else {
      queueNext();
    }

    if (changed || opts.nextSlide == opts.currSlide) {
      // calculate the next slide
      opts.lastSlide = opts.currSlide;
      if (opts.random) {
        opts.currSlide = opts.nextSlide;
        if (++opts.randomIndex == els.length)
          opts.randomIndex = 0;
        opts.nextSlide = opts.randomMap[opts.randomIndex];
        if (opts.nextSlide == opts.currSlide)
          opts.nextSlide = (opts.currSlide == opts.slideCount - 1) ? 0 : opts.currSlide + 1;
      }
      else if (opts.backwards) {
        var roll = (opts.nextSlide - 1) < 0;
        if (roll && opts.bounce) {
          opts.backwards = !opts.backwards;
          opts.nextSlide = 1;
          opts.currSlide = 0;
        }
        else {
          opts.nextSlide = roll ? (els.length - 1) : opts.nextSlide - 1;
          opts.currSlide = roll ? 0 : opts.nextSlide + 1;
        }
      }
      else { // sequence
        var roll = (opts.nextSlide + 1) == els.length;
        if (roll && opts.bounce) {
          opts.backwards = !opts.backwards;
          opts.nextSlide = els.length - 2;
          opts.currSlide = els.length - 1;
        }
        else {
          opts.nextSlide = roll ? 0 : opts.nextSlide + 1;
          opts.currSlide = roll ? els.length - 1 : opts.nextSlide - 1;
        }
      }
    }
    if (changed && opts.pager)
      opts.updateActivePagerLink(opts.pager, opts.currSlide, opts.activePagerClass);

    function queueNext() {
      // stage the next transition
      var ms = 0, timeout = opts.timeout;
      if (opts.timeout && !opts.continuous) {
        ms = getTimeout(els[opts.currSlide], els[opts.nextSlide], opts, fwd);
        if (opts.fx == 'shuffle')
          ms -= opts.speedOut;
      }
      else if (opts.continuous && p.cyclePause) // continuous shows work off an after callback, not this timer logic
        ms = 10;
      if (ms > 0)
        p.cycleTimeout = setTimeout(function () { go(els, opts, 0, !opts.backwards) }, ms);
    }
  };

  // invoked after transition
  $.fn.cycle.updateActivePagerLink = function (pager, currSlide, clsName) {
    $(pager).each(function () {
      $(this).children().removeClass(clsName).eq(currSlide).addClass(clsName);
    });
  };

  // calculate timeout value for current transition
  function getTimeout(curr, next, opts, fwd) {
    if (opts.timeoutFn) {
      // call user provided calc fn
      var t = opts.timeoutFn.call(curr, curr, next, opts, fwd);
      while (opts.fx != 'none' && (t - opts.speed) < 250) // sanitize timeout
        t += opts.speed;
      debug('calculated timeout: ' + t + '; speed: ' + opts.speed);
      if (t !== false)
        return t;
    }
    return opts.timeout;
  };

  // expose next/prev function, caller must pass in state
  $.fn.cycle.next = function (opts) { advance(opts, 1); };
  $.fn.cycle.prev = function (opts) { advance(opts, 0); };

  // advance slide forward or back
  function advance(opts, moveForward) {
    var val = moveForward ? 1 : -1;
    var els = opts.elements;
    var p = opts.$cont[0], timeout = p.cycleTimeout;
    if (timeout) {
      clearTimeout(timeout);
      p.cycleTimeout = 0;
    }
    if (opts.random && val < 0) {
      // move back to the previously display slide
      opts.randomIndex--;
      if (--opts.randomIndex == -2)
        opts.randomIndex = els.length - 2;
      else if (opts.randomIndex == -1)
        opts.randomIndex = els.length - 1;
      opts.nextSlide = opts.randomMap[opts.randomIndex];
    }
    else if (opts.random) {
      opts.nextSlide = opts.randomMap[opts.randomIndex];
    }
    else {
      opts.nextSlide = opts.currSlide + val;
      if (opts.nextSlide < 0) {
        if (opts.nowrap) return false;
        opts.nextSlide = els.length - 1;
      }
      else if (opts.nextSlide >= els.length) {
        if (opts.nowrap) return false;
        opts.nextSlide = 0;
      }
    }

    var cb = opts.onPrevNextEvent || opts.prevNextClick; // prevNextClick is deprecated
    if ($.isFunction(cb))
      cb(val > 0, opts.nextSlide, els[opts.nextSlide]);
    go(els, opts, 1, moveForward);
    return false;
  };

  function buildPager(els, opts) {
    var $p = $(opts.pager);
    $.each(els, function (i, o) {
      $.fn.cycle.createPagerAnchor(i, o, $p, els, opts);
    });
    opts.updateActivePagerLink(opts.pager, opts.startingSlide, opts.activePagerClass);
  };

  $.fn.cycle.createPagerAnchor = function (i, el, $p, els, opts) {
    var a;
    if ($.isFunction(opts.pagerAnchorBuilder)) {
      a = opts.pagerAnchorBuilder(i, el);
      debug('pagerAnchorBuilder(' + i + ', el) returned: ' + a);
    }
    else
      a = '<a href="#">' + (i + 1) + '</a>';

    if (!a)
      return;
    var $a = $(a);
    // don't reparent if anchor is in the dom
    if ($a.parents('body').length === 0) {
      var arr = [];
      if ($p.length > 1) {
        $p.each(function () {
          var $clone = $a.clone(true);
          $(this).append($clone);
          arr.push($clone[0]);
        });
        $a = $(arr);
      }
      else {
        $a.appendTo($p);
      }
    }

    opts.pagerAnchors = opts.pagerAnchors || [];
    opts.pagerAnchors.push($a);

    var pagerFn = function (e) {
      e.preventDefault();
      opts.nextSlide = i;
      var p = opts.$cont[0], timeout = p.cycleTimeout;
      if (timeout) {
        clearTimeout(timeout);
        p.cycleTimeout = 0;
      }
      var cb = opts.onPagerEvent || opts.pagerClick; // pagerClick is deprecated
      if ($.isFunction(cb))
        cb(opts.nextSlide, els[opts.nextSlide]);
      go(els, opts, 1, opts.currSlide < i); // trigger the trans
      //		return false; // <== allow bubble
    }

    if (/mouseenter|mouseover/i.test(opts.pagerEvent)) {
      $a.hover(pagerFn, function () { /* no-op */ });
    }
    else {
      $a.bind(opts.pagerEvent, pagerFn);
    }

    if (!/^click/.test(opts.pagerEvent) && !opts.allowPagerClickBubble)
      $a.bind('click.cycle', function () { return false; }); // suppress click

    var cont = opts.$cont[0];
    var pauseFlag = false; // https://github.com/malsup/cycle/issues/44
    if (opts.pauseOnPagerHover) {
      $a.hover(
		function () {
		  pauseFlag = true;
		  cont.cyclePause++;
		  triggerPause(cont, true, true);
		}, function () {
		  pauseFlag && cont.cyclePause--;
		  triggerPause(cont, true, true);
		}
	);
    }
  };

  // helper fn to calculate the number of slides between the current and the next
  $.fn.cycle.hopsFromLast = function (opts, fwd) {
    var hops, l = opts.lastSlide, c = opts.currSlide;
    if (fwd)
      hops = c > l ? c - l : opts.slideCount - l;
    else
      hops = c < l ? l - c : l + opts.slideCount - c;
    return hops;
  };

  // fix clearType problems in ie6 by setting an explicit bg color
  // (otherwise text slides look horrible during a fade transition)
  function clearTypeFix($slides) {
    debug('applying clearType background-color hack');
    function hex(s) {
      s = parseInt(s, 10).toString(16);
      return s.length < 2 ? '0' + s : s;
    };
    function getBg(e) {
      for (; e && e.nodeName.toLowerCase() != 'html'; e = e.parentNode) {
        var v = $.css(e, 'background-color');
        if (v && v.indexOf('rgb') >= 0) {
          var rgb = v.match(/\d+/g);
          return '#' + hex(rgb[0]) + hex(rgb[1]) + hex(rgb[2]);
        }
        if (v && v != 'transparent')
          return v;
      }
      return '#ffffff';
    };
    $slides.each(function () { $(this).css('background-color', getBg(this)); });
  };

  // reset common props before the next transition
  $.fn.cycle.commonReset = function (curr, next, opts, w, h, rev) {
    $(opts.elements).not(curr).hide();
    if (typeof opts.cssBefore.opacity == 'undefined')
      opts.cssBefore.opacity = 1;
    opts.cssBefore.display = 'block';
    if (opts.slideResize && w !== false && next.cycleW > 0)
      opts.cssBefore.width = next.cycleW;
    if (opts.slideResize && h !== false && next.cycleH > 0)
      opts.cssBefore.height = next.cycleH;
    opts.cssAfter = opts.cssAfter || {};
    opts.cssAfter.display = 'none';
    $(curr).css('zIndex', opts.slideCount + (rev === true ? 1 : 0));
    $(next).css('zIndex', opts.slideCount + (rev === true ? 0 : 1));
  };

  // the actual fn for effecting a transition
  $.fn.cycle.custom = function (curr, next, opts, cb, fwd, speedOverride) {
    var $l = $(curr), $n = $(next);
    var speedIn = opts.speedIn, speedOut = opts.speedOut, easeIn = opts.easeIn, easeOut = opts.easeOut;
    $n.css(opts.cssBefore);
    if (speedOverride) {
      if (typeof speedOverride == 'number')
        speedIn = speedOut = speedOverride;
      else
        speedIn = speedOut = 1;
      easeIn = easeOut = null;
    }
    var fn = function () {
      $n.animate(opts.animIn, speedIn, easeIn, function () {
        cb();
      });
    };
    $l.animate(opts.animOut, speedOut, easeOut, function () {
      $l.css(opts.cssAfter);
      if (!opts.sync)
        fn();
    });
    if (opts.sync) fn();
  };

  // transition definitions - only fade is defined here, transition pack defines the rest
  $.fn.cycle.transitions = {
    fade: function ($cont, $slides, opts) {
      $slides.not(':eq(' + opts.currSlide + ')').css('opacity', 0);
      opts.before.push(function (curr, next, opts) {
        $.fn.cycle.commonReset(curr, next, opts);
        opts.cssBefore.opacity = 0;
      });
      opts.animIn = { opacity: 1 };
      opts.animOut = { opacity: 0 };
      opts.cssBefore = { top: 0, left: 0 };
    }
  };

  $.fn.cycle.ver = function () { return ver; };

  // override these globally if you like (they are all optional)
  $.fn.cycle.defaults = {
    activePagerClass: 'activeSlide', // class name used for the active pager link
    after: null,  // transition callback (scope set to element that was shown):  function(currSlideElement, nextSlideElement, options, forwardFlag)
    allowPagerClickBubble: false, // allows or prevents click event on pager anchors from bubbling
    animIn: null,  // properties that define how the slide animates in
    animOut: null,  // properties that define how the slide animates out
    aspect: false,  // preserve aspect ratio during fit resizing, cropping if necessary (must be used with fit option)
    autostop: 0,   // true to end slideshow after X transitions (where X == slide count)
    autostopCount: 0,   // number of transitions (optionally used with autostop to define X)
    backwards: false, // true to start slideshow at last slide and move backwards through the stack
    before: null,  // transition callback (scope set to element to be shown):	 function(currSlideElement, nextSlideElement, options, forwardFlag)
    center: null,  // set to true to have cycle add top/left margin to each slide (use with width and height options)
    cleartype: !$.support.opacity,  // true if clearType corrections should be applied (for IE)
    cleartypeNoBg: false, // set to true to disable extra cleartype fixing (leave false to force background color setting on slides)
    containerResize: 1,   // resize container to fit largest slide
    continuous: 0,   // true to start next transition immediately after current one completes
    cssAfter: null,  // properties that defined the state of the slide after transitioning out
    cssBefore: null,  // properties that define the initial state of the slide before transitioning in
    delay: 0,   // additional delay (in ms) for first transition (hint: can be negative)
    easeIn: null,  // easing for "in" transition
    easeOut: null,  // easing for "out" transition
    easing: null,  // easing method for both in and out transitions
    end: null,  // callback invoked when the slideshow terminates (use with autostop or nowrap options): function(options)
    fastOnEvent: 0,   // force fast transitions when triggered manually (via pager or prev/next); value == time in ms
    fit: 0,   // force slides to fit container
    fx: 'fade', // name of transition effect (or comma separated names, ex: 'fade,scrollUp,shuffle')
    fxFn: null,  // function used to control the transition: function(currSlideElement, nextSlideElement, options, afterCalback, forwardFlag)
    height: 'auto', // container height (if the 'fit' option is true, the slides will be set to this height as well)
    manualTrump: true,  // causes manual transition to stop an active transition instead of being ignored
    metaAttr: 'cycle', // data- attribute that holds the option data for the slideshow
    next: null,  // element, jQuery object, or jQuery selector string for the element to use as event trigger for next slide
    nowrap: 0,   // true to prevent slideshow from wrapping
    onPagerEvent: null,  // callback fn for pager events: function(zeroBasedSlideIndex, slideElement)
    onPrevNextEvent: null, // callback fn for prev/next events: function(isNext, zeroBasedSlideIndex, slideElement)
    pager: null,  // element, jQuery object, or jQuery selector string for the element to use as pager container
    pagerAnchorBuilder: null, // callback fn for building anchor links:  function(index, DOMelement)
    pagerEvent: 'click.cycle', // name of event which drives the pager navigation
    pause: 0,   // true to enable "pause on hover"
    pauseOnPagerHover: 0, // true to pause when hovering over pager link
    prev: null,  // element, jQuery object, or jQuery selector string for the element to use as event trigger for previous slide
    prevNextEvent: 'click.cycle', // event which drives the manual transition to the previous or next slide
    random: 0,   // true for random, false for sequence (not applicable to shuffle fx)
    randomizeEffects: 1,  // valid when multiple effects are used; true to make the effect sequence random
    requeueOnImageNotLoaded: true, // requeue the slideshow if any image slides are not yet loaded
    requeueTimeout: 250,  // ms delay for requeue
    rev: 0,   // causes animations to transition in reverse (for effects that support it such as scrollHorz/scrollVert/shuffle)
    shuffle: null,  // coords for shuffle animation, ex: { top:15, left: 200 }
    skipInitializationCallbacks: false, // set to true to disable the first before/after callback that occurs prior to any transition
    slideExpr: null,  // expression for selecting slides (if something other than all children is required)
    slideResize: 1,     // force slide width/height to fixed size before every transition
    speed: 1000,  // speed of the transition (any valid fx speed value)
    speedIn: null,  // speed of the 'in' transition
    speedOut: null,  // speed of the 'out' transition
    startingSlide: 0,   // zero-based index of the first slide to be displayed
    sync: 1,   // true if in/out transitions should occur simultaneously
    timeout: 4000,  // milliseconds between slide transitions (0 to disable auto advance)
    timeoutFn: null,  // callback for determining per-slide timeout value:  function(currSlideElement, nextSlideElement, options, forwardFlag)
    updateActivePagerLink: null, // callback fn invoked to update the active pager link (adds/removes activePagerClass style)
    width: null   // container width (if the 'fit' option is true, the slides will be set to this width as well)
  };

})(jQuery);

/*!
* jQuery Cycle Plugin Transition Definitions
* This script is a plugin for the jQuery Cycle Plugin
* Examples and documentation at: http://malsup.com/jquery/cycle/
* Copyright (c) 2007-2010 M. Alsup
* Version:	 2.73
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*/
(function ($) {

  //
  // These functions define slide initialization and properties for the named
  // transitions. To save file size feel free to remove any of these that you
  // don't need.
  //
  $.fn.cycle.transitions.none = function ($cont, $slides, opts) {
    opts.fxFn = function (curr, next, opts, after) {
      $(next).show();
      $(curr).hide();
      after();
    };
  };

  // not a cross-fade, fadeout only fades out the top slide
  $.fn.cycle.transitions.fadeout = function ($cont, $slides, opts) {
    $slides.not(':eq(' + opts.currSlide + ')').css({ display: 'block', 'opacity': 1 });
    opts.before.push(function (curr, next, opts, w, h, rev) {
      $(curr).css('zIndex', opts.slideCount + (!rev === true ? 1 : 0));
      $(next).css('zIndex', opts.slideCount + (!rev === true ? 0 : 1));
    });
    opts.animIn.opacity = 1;
    opts.animOut.opacity = 0;
    opts.cssBefore.opacity = 1;
    opts.cssBefore.display = 'block';
    opts.cssAfter.zIndex = 0;
  };

  // scrollUp/Down/Left/Right
  $.fn.cycle.transitions.scrollUp = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden');
    opts.before.push($.fn.cycle.commonReset);
    var h = $cont.height();
    opts.cssBefore.top = h;
    opts.cssBefore.left = 0;
    opts.cssFirst.top = 0;
    opts.animIn.top = 0;
    opts.animOut.top = -h;
  };
  $.fn.cycle.transitions.scrollDown = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden');
    opts.before.push($.fn.cycle.commonReset);
    var h = $cont.height();
    opts.cssFirst.top = 0;
    opts.cssBefore.top = -h;
    opts.cssBefore.left = 0;
    opts.animIn.top = 0;
    opts.animOut.top = h;
  };
  $.fn.cycle.transitions.scrollLeft = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden');
    opts.before.push($.fn.cycle.commonReset);
    var w = $cont.width();
    opts.cssFirst.left = 0;
    opts.cssBefore.left = w;
    opts.cssBefore.top = 0;
    opts.animIn.left = 0;
    opts.animOut.left = 0 - w;
  };
  $.fn.cycle.transitions.scrollRight = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden');
    opts.before.push($.fn.cycle.commonReset);
    var w = $cont.width();
    opts.cssFirst.left = 0;
    opts.cssBefore.left = -w;
    opts.cssBefore.top = 0;
    opts.animIn.left = 0;
    opts.animOut.left = w;
  };
  $.fn.cycle.transitions.scrollHorz = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden').width();
    opts.before.push(function (curr, next, opts, fwd) {
      if (opts.rev)
        fwd = !fwd;
      $.fn.cycle.commonReset(curr, next, opts);
      opts.cssBefore.left = fwd ? (next.cycleW - 1) : (1 - next.cycleW);
      opts.animOut.left = fwd ? -curr.cycleW : curr.cycleW;
    });
    opts.cssFirst.left = 0;
    opts.cssBefore.top = 0;
    opts.animIn.left = 0;
    opts.animOut.top = 0;
  };
  $.fn.cycle.transitions.scrollVert = function ($cont, $slides, opts) {
    $cont.css('overflow', 'hidden');
    opts.before.push(function (curr, next, opts, fwd) {
      if (opts.rev)
        fwd = !fwd;
      $.fn.cycle.commonReset(curr, next, opts);
      opts.cssBefore.top = fwd ? (1 - next.cycleH) : (next.cycleH - 1);
      opts.animOut.top = fwd ? curr.cycleH : -curr.cycleH;
    });
    opts.cssFirst.top = 0;
    opts.cssBefore.left = 0;
    opts.animIn.top = 0;
    opts.animOut.left = 0;
  };

  // slideX/slideY
  $.fn.cycle.transitions.slideX = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $(opts.elements).not(curr).hide();
      $.fn.cycle.commonReset(curr, next, opts, false, true);
      opts.animIn.width = next.cycleW;
    });
    opts.cssBefore.left = 0;
    opts.cssBefore.top = 0;
    opts.cssBefore.width = 0;
    opts.animIn.width = 'show';
    opts.animOut.width = 0;
  };
  $.fn.cycle.transitions.slideY = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $(opts.elements).not(curr).hide();
      $.fn.cycle.commonReset(curr, next, opts, true, false);
      opts.animIn.height = next.cycleH;
    });
    opts.cssBefore.left = 0;
    opts.cssBefore.top = 0;
    opts.cssBefore.height = 0;
    opts.animIn.height = 'show';
    opts.animOut.height = 0;
  };

  // shuffle
  $.fn.cycle.transitions.shuffle = function ($cont, $slides, opts) {
    var i, w = $cont.css('overflow', 'visible').width();
    $slides.css({ left: 0, top: 0 });
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, true, true);
    });
    // only adjust speed once!
    if (!opts.speedAdjusted) {
      opts.speed = opts.speed / 2; // shuffle has 2 transitions
      opts.speedAdjusted = true;
    }
    opts.random = 0;
    opts.shuffle = opts.shuffle || { left: -w, top: 15 };
    opts.els = [];
    for (i = 0; i < $slides.length; i++)
      opts.els.push($slides[i]);

    for (i = 0; i < opts.currSlide; i++)
      opts.els.push(opts.els.shift());

    // custom transition fn (hat tip to Benjamin Sterling for this bit of sweetness!)
    opts.fxFn = function (curr, next, opts, cb, fwd) {
      if (opts.rev)
        fwd = !fwd;
      var $el = fwd ? $(curr) : $(next);
      $(next).css(opts.cssBefore);
      var count = opts.slideCount;
      $el.animate(opts.shuffle, opts.speedIn, opts.easeIn, function () {
        var hops = $.fn.cycle.hopsFromLast(opts, fwd);
        for (var k = 0; k < hops; k++)
          fwd ? opts.els.push(opts.els.shift()) : opts.els.unshift(opts.els.pop());
        if (fwd) {
          for (var i = 0, len = opts.els.length; i < len; i++)
            $(opts.els[i]).css('z-index', len - i + count);
        }
        else {
          var z = $(curr).css('z-index');
          $el.css('z-index', parseInt(z, 10) + 1 + count);
        }
        $el.animate({ left: 0, top: 0 }, opts.speedOut, opts.easeOut, function () {
          $(fwd ? this : curr).hide();
          if (cb) cb();
        });
      });
    };
    $.extend(opts.cssBefore, { display: 'block', opacity: 1, top: 0, left: 0 });
  };

  // turnUp/Down/Left/Right
  $.fn.cycle.transitions.turnUp = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, false);
      opts.cssBefore.top = next.cycleH;
      opts.animIn.height = next.cycleH;
      opts.animOut.width = next.cycleW;
    });
    opts.cssFirst.top = 0;
    opts.cssBefore.left = 0;
    opts.cssBefore.height = 0;
    opts.animIn.top = 0;
    opts.animOut.height = 0;
  };
  $.fn.cycle.transitions.turnDown = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, false);
      opts.animIn.height = next.cycleH;
      opts.animOut.top = curr.cycleH;
    });
    opts.cssFirst.top = 0;
    opts.cssBefore.left = 0;
    opts.cssBefore.top = 0;
    opts.cssBefore.height = 0;
    opts.animOut.height = 0;
  };
  $.fn.cycle.transitions.turnLeft = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, true);
      opts.cssBefore.left = next.cycleW;
      opts.animIn.width = next.cycleW;
    });
    opts.cssBefore.top = 0;
    opts.cssBefore.width = 0;
    opts.animIn.left = 0;
    opts.animOut.width = 0;
  };
  $.fn.cycle.transitions.turnRight = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, true);
      opts.animIn.width = next.cycleW;
      opts.animOut.left = curr.cycleW;
    });
    $.extend(opts.cssBefore, { top: 0, left: 0, width: 0 });
    opts.animIn.left = 0;
    opts.animOut.width = 0;
  };

  // zoom
  $.fn.cycle.transitions.zoom = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, false, true);
      opts.cssBefore.top = next.cycleH / 2;
      opts.cssBefore.left = next.cycleW / 2;
      $.extend(opts.animIn, { top: 0, left: 0, width: next.cycleW, height: next.cycleH });
      $.extend(opts.animOut, { width: 0, height: 0, top: curr.cycleH / 2, left: curr.cycleW / 2 });
    });
    opts.cssFirst.top = 0;
    opts.cssFirst.left = 0;
    opts.cssBefore.width = 0;
    opts.cssBefore.height = 0;
  };

  // fadeZoom
  $.fn.cycle.transitions.fadeZoom = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, false);
      opts.cssBefore.left = next.cycleW / 2;
      opts.cssBefore.top = next.cycleH / 2;
      $.extend(opts.animIn, { top: 0, left: 0, width: next.cycleW, height: next.cycleH });
    });
    opts.cssBefore.width = 0;
    opts.cssBefore.height = 0;
    opts.animOut.opacity = 0;
  };

  // blindX
  $.fn.cycle.transitions.blindX = function ($cont, $slides, opts) {
    var w = $cont.css('overflow', 'hidden').width();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts);
      opts.animIn.width = next.cycleW;
      opts.animOut.left = curr.cycleW;
    });
    opts.cssBefore.left = w;
    opts.cssBefore.top = 0;
    opts.animIn.left = 0;
    opts.animOut.left = w;
  };
  // blindY
  $.fn.cycle.transitions.blindY = function ($cont, $slides, opts) {
    var h = $cont.css('overflow', 'hidden').height();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts);
      opts.animIn.height = next.cycleH;
      opts.animOut.top = curr.cycleH;
    });
    opts.cssBefore.top = h;
    opts.cssBefore.left = 0;
    opts.animIn.top = 0;
    opts.animOut.top = h;
  };
  // blindZ
  $.fn.cycle.transitions.blindZ = function ($cont, $slides, opts) {
    var h = $cont.css('overflow', 'hidden').height();
    var w = $cont.width();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts);
      opts.animIn.height = next.cycleH;
      opts.animOut.top = curr.cycleH;
    });
    opts.cssBefore.top = h;
    opts.cssBefore.left = w;
    opts.animIn.top = 0;
    opts.animIn.left = 0;
    opts.animOut.top = h;
    opts.animOut.left = w;
  };

  // growX - grow horizontally from centered 0 width
  $.fn.cycle.transitions.growX = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, true);
      opts.cssBefore.left = this.cycleW / 2;
      opts.animIn.left = 0;
      opts.animIn.width = this.cycleW;
      opts.animOut.left = 0;
    });
    opts.cssBefore.top = 0;
    opts.cssBefore.width = 0;
  };
  // growY - grow vertically from centered 0 height
  $.fn.cycle.transitions.growY = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, false);
      opts.cssBefore.top = this.cycleH / 2;
      opts.animIn.top = 0;
      opts.animIn.height = this.cycleH;
      opts.animOut.top = 0;
    });
    opts.cssBefore.height = 0;
    opts.cssBefore.left = 0;
  };

  // curtainX - squeeze in both edges horizontally
  $.fn.cycle.transitions.curtainX = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, false, true, true);
      opts.cssBefore.left = next.cycleW / 2;
      opts.animIn.left = 0;
      opts.animIn.width = this.cycleW;
      opts.animOut.left = curr.cycleW / 2;
      opts.animOut.width = 0;
    });
    opts.cssBefore.top = 0;
    opts.cssBefore.width = 0;
  };
  // curtainY - squeeze in both edges vertically
  $.fn.cycle.transitions.curtainY = function ($cont, $slides, opts) {
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, false, true);
      opts.cssBefore.top = next.cycleH / 2;
      opts.animIn.top = 0;
      opts.animIn.height = next.cycleH;
      opts.animOut.top = curr.cycleH / 2;
      opts.animOut.height = 0;
    });
    opts.cssBefore.height = 0;
    opts.cssBefore.left = 0;
  };

  // cover - curr slide covered by next slide
  $.fn.cycle.transitions.cover = function ($cont, $slides, opts) {
    var d = opts.direction || 'left';
    var w = $cont.css('overflow', 'hidden').width();
    var h = $cont.height();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts);
      if (d == 'right')
        opts.cssBefore.left = -w;
      else if (d == 'up')
        opts.cssBefore.top = h;
      else if (d == 'down')
        opts.cssBefore.top = -h;
      else
        opts.cssBefore.left = w;
    });
    opts.animIn.left = 0;
    opts.animIn.top = 0;
    opts.cssBefore.top = 0;
    opts.cssBefore.left = 0;
  };

  // uncover - curr slide moves off next slide
  $.fn.cycle.transitions.uncover = function ($cont, $slides, opts) {
    var d = opts.direction || 'left';
    var w = $cont.css('overflow', 'hidden').width();
    var h = $cont.height();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, true, true);
      if (d == 'right')
        opts.animOut.left = w;
      else if (d == 'up')
        opts.animOut.top = -h;
      else if (d == 'down')
        opts.animOut.top = h;
      else
        opts.animOut.left = -w;
    });
    opts.animIn.left = 0;
    opts.animIn.top = 0;
    opts.cssBefore.top = 0;
    opts.cssBefore.left = 0;
  };

  // toss - move top slide and fade away
  $.fn.cycle.transitions.toss = function ($cont, $slides, opts) {
    var w = $cont.css('overflow', 'visible').width();
    var h = $cont.height();
    opts.before.push(function (curr, next, opts) {
      $.fn.cycle.commonReset(curr, next, opts, true, true, true);
      // provide default toss settings if animOut not provided
      if (!opts.animOut.left && !opts.animOut.top)
        $.extend(opts.animOut, { left: w * 2, top: -h / 2, opacity: 0 });
      else
        opts.animOut.opacity = 0;
    });
    opts.cssBefore.left = 0;
    opts.cssBefore.top = 0;
    opts.animIn.left = 0;
  };

  // wipe - clip animation
  $.fn.cycle.transitions.wipe = function ($cont, $slides, opts) {
    var w = $cont.css('overflow', 'hidden').width();
    var h = $cont.height();
    opts.cssBefore = opts.cssBefore || {};
    var clip;
    if (opts.clip) {
      if (/l2r/.test(opts.clip))
        clip = 'rect(0px 0px ' + h + 'px 0px)';
      else if (/r2l/.test(opts.clip))
        clip = 'rect(0px ' + w + 'px ' + h + 'px ' + w + 'px)';
      else if (/t2b/.test(opts.clip))
        clip = 'rect(0px ' + w + 'px 0px 0px)';
      else if (/b2t/.test(opts.clip))
        clip = 'rect(' + h + 'px ' + w + 'px ' + h + 'px 0px)';
      else if (/zoom/.test(opts.clip)) {
        var top = parseInt(h / 2, 10);
        var left = parseInt(w / 2, 10);
        clip = 'rect(' + top + 'px ' + left + 'px ' + top + 'px ' + left + 'px)';
      }
    }

    opts.cssBefore.clip = opts.cssBefore.clip || clip || 'rect(0px 0px 0px 0px)';

    var d = opts.cssBefore.clip.match(/(\d+)/g);
    var t = parseInt(d[0], 10), r = parseInt(d[1], 10), b = parseInt(d[2], 10), l = parseInt(d[3], 10);

    opts.before.push(function (curr, next, opts) {
      if (curr == next) return;
      var $curr = $(curr), $next = $(next);
      $.fn.cycle.commonReset(curr, next, opts, true, true, false);
      opts.cssAfter.display = 'block';

      var step = 1, count = parseInt((opts.speedIn / 13), 10) - 1;
      (function f() {
        var tt = t ? t - parseInt(step * (t / count), 10) : 0;
        var ll = l ? l - parseInt(step * (l / count), 10) : 0;
        var bb = b < h ? b + parseInt(step * ((h - b) / count || 1), 10) : h;
        var rr = r < w ? r + parseInt(step * ((w - r) / count || 1), 10) : w;
        $next.css({ clip: 'rect(' + tt + 'px ' + rr + 'px ' + bb + 'px ' + ll + 'px)' });
        (step++ <= count) ? setTimeout(f, 13) : $curr.css('display', 'none');
      })();
    });
    $.extend(opts.cssBefore, { display: 'block', opacity: 1, top: 0, left: 0 });
    opts.animIn = { left: 0 };
    opts.animOut = { left: 0 };
  };

})(jQuery);

/**
* @summary     DataTables
* @description Paginate, search and sort HTML tables
* @version     1.9.3
* @file        jquery.dataTables.js
* @author      Allan Jardine (www.sprymedia.co.uk)
* @contact     www.sprymedia.co.uk/contact
*
* @copyright Copyright 2008-2012 Allan Jardine, all rights reserved.
*
* This source file is free software, under either the GPL v2 license or a
* BSD style license, available at:
*   http://datatables.net/license_gpl2
*   http://datatables.net/license_bsd
* 
* This source file is distributed in the hope that it will be useful, but 
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
* or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
* 
* For details please refer to: http://www.datatables.net
*/

/*jslint evil: true, undef: true, browser: true */
/*globals $, jQuery,_fnExternApiFunc,_fnInitialise,_fnInitComplete,_fnLanguageCompat,_fnAddColumn,_fnColumnOptions,_fnAddData,_fnCreateTr,_fnGatherData,_fnBuildHead,_fnDrawHead,_fnDraw,_fnReDraw,_fnAjaxUpdate,_fnAjaxParameters,_fnAjaxUpdateDraw,_fnServerParams,_fnAddOptionsHtml,_fnFeatureHtmlTable,_fnScrollDraw,_fnAdjustColumnSizing,_fnFeatureHtmlFilter,_fnFilterComplete,_fnFilterCustom,_fnFilterColumn,_fnFilter,_fnBuildSearchArray,_fnBuildSearchRow,_fnFilterCreateSearch,_fnDataToSearch,_fnSort,_fnSortAttachListener,_fnSortingClasses,_fnFeatureHtmlPaginate,_fnPageChange,_fnFeatureHtmlInfo,_fnUpdateInfo,_fnFeatureHtmlLength,_fnFeatureHtmlProcessing,_fnProcessingDisplay,_fnVisibleToColumnIndex,_fnColumnIndexToVisible,_fnNodeToDataIndex,_fnVisbleColumns,_fnCalculateEnd,_fnConvertToWidth,_fnCalculateColumnWidths,_fnScrollingWidthAdjust,_fnGetWidestNode,_fnGetMaxLenString,_fnStringToCss,_fnDetectType,_fnSettingsFromNode,_fnGetDataMaster,_fnGetTrNodes,_fnGetTdNodes,_fnEscapeRegex,_fnDeleteIndex,_fnReOrderIndex,_fnColumnOrdering,_fnLog,_fnClearTable,_fnSaveState,_fnLoadState,_fnCreateCookie,_fnReadCookie,_fnDetectHeader,_fnGetUniqueThs,_fnScrollBarWidth,_fnApplyToChildren,_fnMap,_fnGetRowData,_fnGetCellData,_fnSetCellData,_fnGetObjectDataFn,_fnSetObjectDataFn,_fnApplyColumnDefs,_fnBindAction,_fnCallbackReg,_fnCallbackFire,_fnJsonString,_fnRender,_fnNodeToColumnIndex,_fnInfoMacros,_fnBrowserDetect,_fnGetColumns*/

( /** @lends <global> */
function ($, window, document, undefined) {
  /** 
  * DataTables is a plug-in for the jQuery Javascript library. It is a 
  * highly flexible tool, based upon the foundations of progressive 
  * enhancement, which will add advanced interaction controls to any 
  * HTML table. For a full list of features please refer to
  * <a href="http://datatables.net">DataTables.net</a>.
  *
  * Note that the <i>DataTable</i> object is not a global variable but is
  * aliased to <i>jQuery.fn.DataTable</i> and <i>jQuery.fn.dataTable</i> through which 
  * it may be  accessed.
  *
  *  @class
  *  @param {object} [oInit={}] Configuration object for DataTables. Options
  *    are defined by {@link DataTable.defaults}
  *  @requires jQuery 1.3+
  * 
  *  @example
  *    // Basic initialisation
  *    $(document).ready( function {
  *      $('#example').dataTable();
  *    } );
  *  
  *  @example
  *    // Initialisation with configuration options - in this case, disable
  *    // pagination and sorting.
  *    $(document).ready( function {
  *      $('#example').dataTable( {
  *        "bPaginate": false,
  *        "bSort": false 
  *      } );
  *    } );
  */
  var DataTable = function (oInit) {


    /**
    * Add a column to the list used for the table with default values
    *  @param {object} oSettings dataTables settings object
    *  @param {node} nTh The th element for this column
    *  @memberof DataTable#oApi
    */
    function _fnAddColumn(oSettings, nTh) {
      var oDefaults = DataTable.defaults.columns;
      var iCol = oSettings.aoColumns.length;
      var oCol = $.extend({}, DataTable.models.oColumn, oDefaults, {
        "sSortingClass": oSettings.oClasses.sSortable,
        "sSortingClassJUI": oSettings.oClasses.sSortJUI,
        "nTh": nTh ? nTh : document.createElement('th'),
        "sTitle": oDefaults.sTitle ? oDefaults.sTitle : nTh ? nTh.innerHTML : '',
        "aDataSort": oDefaults.aDataSort ? oDefaults.aDataSort : [iCol],
        "mData": oDefaults.mData ? oDefaults.oDefaults : iCol
      });
      oSettings.aoColumns.push(oCol);

      /* Add a column specific filter */
      if (oSettings.aoPreSearchCols[iCol] === undefined || oSettings.aoPreSearchCols[iCol] === null) {
        oSettings.aoPreSearchCols[iCol] = $.extend({}, DataTable.models.oSearch);
      } else {
        var oPre = oSettings.aoPreSearchCols[iCol];

        /* Don't require that the user must specify bRegex, bSmart or bCaseInsensitive */
        if (oPre.bRegex === undefined) {
          oPre.bRegex = true;
        }

        if (oPre.bSmart === undefined) {
          oPre.bSmart = true;
        }

        if (oPre.bCaseInsensitive === undefined) {
          oPre.bCaseInsensitive = true;
        }
      }

      /* Use the column options function to initialise classes etc */
      _fnColumnOptions(oSettings, iCol, null);
    }


    /**
    * Apply options for a column
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iCol column index to consider
    *  @param {object} oOptions object with sType, bVisible and bSearchable etc
    *  @memberof DataTable#oApi
    */
    function _fnColumnOptions(oSettings, iCol, oOptions) {
      var oCol = oSettings.aoColumns[iCol];

      /* User specified column options */
      if (oOptions !== undefined && oOptions !== null) {
        /* Backwards compatibility for mDataProp */
        if (oOptions.mDataProp && !oOptions.mData) {
          oOptions.mData = oOptions.mDataProp;
        }

        if (oOptions.sType !== undefined) {
          oCol.sType = oOptions.sType;
          oCol._bAutoType = false;
        }

        $.extend(oCol, oOptions);
        _fnMap(oCol, oOptions, "sWidth", "sWidthOrig");

        /* iDataSort to be applied (backwards compatibility), but aDataSort will take
        * priority if defined
        */
        if (oOptions.iDataSort !== undefined) {
          oCol.aDataSort = [oOptions.iDataSort];
        }
        _fnMap(oCol, oOptions, "aDataSort");
      }

      /* Cache the data get and set functions for speed */
      var mRender = oCol.mRender ? _fnGetObjectDataFn(oCol.mRender) : null;
      var mData = _fnGetObjectDataFn(oCol.mData);

      oCol.fnGetData = function (oData, sSpecific) {
        var innerData = mData(oData, sSpecific);

        if (oCol.mRender && (sSpecific && sSpecific !== '')) {
          return mRender(innerData, sSpecific, oData);
        }
        return innerData;
      };
      oCol.fnSetData = _fnSetObjectDataFn(oCol.mData);

      /* Feature sorting overrides column specific when off */
      if (!oSettings.oFeatures.bSort) {
        oCol.bSortable = false;
      }

      /* Check that the class assignment is correct for sorting */
      if (!oCol.bSortable || ($.inArray('asc', oCol.asSorting) == -1 && $.inArray('desc', oCol.asSorting) == -1)) {
        oCol.sSortingClass = oSettings.oClasses.sSortableNone;
        oCol.sSortingClassJUI = "";
      } else if (oCol.bSortable || ($.inArray('asc', oCol.asSorting) == -1 && $.inArray('desc', oCol.asSorting) == -1)) {
        oCol.sSortingClass = oSettings.oClasses.sSortable;
        oCol.sSortingClassJUI = oSettings.oClasses.sSortJUI;
      } else if ($.inArray('asc', oCol.asSorting) != -1 && $.inArray('desc', oCol.asSorting) == -1) {
        oCol.sSortingClass = oSettings.oClasses.sSortableAsc;
        oCol.sSortingClassJUI = oSettings.oClasses.sSortJUIAscAllowed;
      } else if ($.inArray('asc', oCol.asSorting) == -1 && $.inArray('desc', oCol.asSorting) != -1) {
        oCol.sSortingClass = oSettings.oClasses.sSortableDesc;
        oCol.sSortingClassJUI = oSettings.oClasses.sSortJUIDescAllowed;
      }
    }


    /**
    * Adjust the table column widths for new data. Note: you would probably want to 
    * do a redraw after calling this function!
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnAdjustColumnSizing(oSettings) {
      /* Not interested in doing column width calculation if auto-width is disabled */
      if (oSettings.oFeatures.bAutoWidth === false) {
        return false;
      }

      _fnCalculateColumnWidths(oSettings);
      for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        oSettings.aoColumns[i].nTh.style.width = oSettings.aoColumns[i].sWidth;
      }
    }


    /**
    * Covert the index of a visible column to the index in the data array (take account
    * of hidden columns)
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iMatch Visible column index to lookup
    *  @returns {int} i the data index
    *  @memberof DataTable#oApi
    */
    function _fnVisibleToColumnIndex(oSettings, iMatch) {
      var aiVis = _fnGetColumns(oSettings, 'bVisible');

      return typeof aiVis[iMatch] === 'number' ? aiVis[iMatch] : null;
    }


    /**
    * Covert the index of an index in the data array and convert it to the visible
    *   column index (take account of hidden columns)
    *  @param {int} iMatch Column index to lookup
    *  @param {object} oSettings dataTables settings object
    *  @returns {int} i the data index
    *  @memberof DataTable#oApi
    */
    function _fnColumnIndexToVisible(oSettings, iMatch) {
      var aiVis = _fnGetColumns(oSettings, 'bVisible');
      var iPos = $.inArray(iMatch, aiVis);

      return iPos !== -1 ? iPos : null;
    }


    /**
    * Get the number of visible columns
    *  @param {object} oSettings dataTables settings object
    *  @returns {int} i the number of visible columns
    *  @memberof DataTable#oApi
    */
    function _fnVisbleColumns(oSettings) {
      return _fnGetColumns(oSettings, 'bVisible').length;
    }


    /**
    * Get an array of column indexes that match a given property
    *  @param {object} oSettings dataTables settings object
    *  @param {string} sParam Parameter in aoColumns to look for - typically 
    *    bVisible or bSearchable
    *  @returns {array} Array of indexes with matched properties
    *  @memberof DataTable#oApi
    */
    function _fnGetColumns(oSettings, sParam) {
      var a = [];

      $.map(oSettings.aoColumns, function (val, i) {
        if (val[sParam]) {
          a.push(i);
        }
      });

      return a;
    }


    /**
    * Get the sort type based on an input string
    *  @param {string} sData data we wish to know the type of
    *  @returns {string} type (defaults to 'string' if no type can be detected)
    *  @memberof DataTable#oApi
    */
    function _fnDetectType(sData) {
      var aTypes = DataTable.ext.aTypes;
      var iLen = aTypes.length;

      for (var i = 0; i < iLen; i++) {
        var sType = aTypes[i](sData);
        if (sType !== null) {
          return sType;
        }
      }

      return 'string';
    }


    /**
    * Figure out how to reorder a display list
    *  @param {object} oSettings dataTables settings object
    *  @returns array {int} aiReturn index list for reordering
    *  @memberof DataTable#oApi
    */
    function _fnReOrderIndex(oSettings, sColumns) {
      var aColumns = sColumns.split(',');
      var aiReturn = [];

      for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        for (var j = 0; j < iLen; j++) {
          if (oSettings.aoColumns[i].sName == aColumns[j]) {
            aiReturn.push(j);
            break;
          }
        }
      }

      return aiReturn;
    }


    /**
    * Get the column ordering that DataTables expects
    *  @param {object} oSettings dataTables settings object
    *  @returns {string} comma separated list of names
    *  @memberof DataTable#oApi
    */
    function _fnColumnOrdering(oSettings) {
      var sNames = '';
      for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        sNames += oSettings.aoColumns[i].sName + ',';
      }
      if (sNames.length == iLen) {
        return "";
      }
      return sNames.slice(0, -1);
    }


    /**
    * Take the column definitions and static columns arrays and calculate how
    * they relate to column indexes. The callback function will then apply the
    * definition found for a column to a suitable configuration object.
    *  @param {object} oSettings dataTables settings object
    *  @param {array} aoColDefs The aoColumnDefs array that is to be applied
    *  @param {array} aoCols The aoColumns array that defines columns individually
    *  @param {function} fn Callback function - takes two parameters, the calculated
    *    column index and the definition for that column.
    *  @memberof DataTable#oApi
    */
    function _fnApplyColumnDefs(oSettings, aoColDefs, aoCols, fn) {
      var i, iLen, j, jLen, k, kLen;

      // Column definitions with aTargets
      if (aoColDefs) {
        /* Loop over the definitions array - loop in reverse so first instance has priority */
        for (i = aoColDefs.length - 1; i >= 0; i--) {
          /* Each definition can target multiple columns, as it is an array */
          var aTargets = aoColDefs[i].aTargets;
          if (!$.isArray(aTargets)) {
            _fnLog(oSettings, 1, 'aTargets must be an array of targets, not a ' + (typeof aTargets));
          }

          for (j = 0, jLen = aTargets.length; j < jLen; j++) {
            if (typeof aTargets[j] === 'number' && aTargets[j] >= 0) {
              /* Add columns that we don't yet know about */
              while (oSettings.aoColumns.length <= aTargets[j]) {
                _fnAddColumn(oSettings);
              }

              /* Integer, basic index */
              fn(aTargets[j], aoColDefs[i]);
            } else if (typeof aTargets[j] === 'number' && aTargets[j] < 0) {
              /* Negative integer, right to left column counting */
              fn(oSettings.aoColumns.length + aTargets[j], aoColDefs[i]);
            } else if (typeof aTargets[j] === 'string') {
              /* Class name matching on TH element */
              for (k = 0, kLen = oSettings.aoColumns.length; k < kLen; k++) {
                if (aTargets[j] == "_all" || $(oSettings.aoColumns[k].nTh).hasClass(aTargets[j])) {
                  fn(k, aoColDefs[i]);
                }
              }
            }
          }
        }
      }

      // Statically defined columns array
      if (aoCols) {
        for (i = 0, iLen = aoCols.length; i < iLen; i++) {
          fn(i, aoCols[i]);
        }
      }
    }



    /**
    * Add a data array to the table, creating DOM node etc. This is the parallel to 
    * _fnGatherData, but for adding rows from a Javascript source, rather than a
    * DOM source.
    *  @param {object} oSettings dataTables settings object
    *  @param {array} aData data array to be added
    *  @returns {int} >=0 if successful (index of new aoData entry), -1 if failed
    *  @memberof DataTable#oApi
    */
    function _fnAddData(oSettings, aDataSupplied) {
      var oCol;

      /* Take an independent copy of the data source so we can bash it about as we wish */
      var aDataIn = ($.isArray(aDataSupplied)) ? aDataSupplied.slice() : $.extend(true, {}, aDataSupplied);

      /* Create the object for storing information about this new row */
      var iRow = oSettings.aoData.length;
      var oData = $.extend(true, {}, DataTable.models.oRow);
      oData._aData = aDataIn;
      oSettings.aoData.push(oData);

      /* Create the cells */
      var nTd, sThisType;
      for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        oCol = oSettings.aoColumns[i];

        /* Use rendered data for filtering / sorting */
        if (typeof oCol.fnRender === 'function' && oCol.bUseRendered && oCol.mData !== null) {
          _fnSetCellData(oSettings, iRow, i, _fnRender(oSettings, iRow, i));
        } else {
          _fnSetCellData(oSettings, iRow, i, _fnGetCellData(oSettings, iRow, i));
        }

        /* See if we should auto-detect the column type */
        if (oCol._bAutoType && oCol.sType != 'string') {
          /* Attempt to auto detect the type - same as _fnGatherData() */
          var sVarType = _fnGetCellData(oSettings, iRow, i, 'type');
          if (sVarType !== null && sVarType !== '') {
            sThisType = _fnDetectType(sVarType);
            if (oCol.sType === null) {
              oCol.sType = sThisType;
            } else if (oCol.sType != sThisType && oCol.sType != "html") {
              /* String is always the 'fallback' option */
              oCol.sType = 'string';
            }
          }
        }
      }

      /* Add to the display array */
      oSettings.aiDisplayMaster.push(iRow);

      /* Create the DOM imformation */
      if (!oSettings.oFeatures.bDeferRender) {
        _fnCreateTr(oSettings, iRow);
      }

      return iRow;
    }


    /**
    * Read in the data from the target table from the DOM
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnGatherData(oSettings) {
      var iLoop, i, iLen, j, jLen, jInner, nTds, nTrs, nTd, aLocalData, iThisIndex, iRow, iRows, iColumn, iColumns, sNodeName, oCol, oData;

      /*
      * Process by row first
      * Add the data object for the whole table - storing the tr node. Note - no point in getting
      * DOM based data if we are going to go and replace it with Ajax source data.
      */
      if (oSettings.bDeferLoading || oSettings.sAjaxSource === null) {
        nTrs = oSettings.nTBody.childNodes;
        for (i = 0, iLen = nTrs.length; i < iLen; i++) {
          if (nTrs[i].nodeName.toUpperCase() == "TR") {
            iThisIndex = oSettings.aoData.length;
            nTrs[i]._DT_RowIndex = iThisIndex;
            oSettings.aoData.push($.extend(true, {}, DataTable.models.oRow, {
              "nTr": nTrs[i]
            }));

            oSettings.aiDisplayMaster.push(iThisIndex);
            nTds = nTrs[i].childNodes;
            jInner = 0;

            for (j = 0, jLen = nTds.length; j < jLen; j++) {
              sNodeName = nTds[j].nodeName.toUpperCase();
              if (sNodeName == "TD" || sNodeName == "TH") {
                _fnSetCellData(oSettings, iThisIndex, jInner, $.trim(nTds[j].innerHTML));
                jInner++;
              }
            }
          }
        }
      }

      /* Gather in the TD elements of the Table - note that this is basically the same as
      * fnGetTdNodes, but that function takes account of hidden columns, which we haven't yet
      * setup!
      */
      nTrs = _fnGetTrNodes(oSettings);
      nTds = [];
      for (i = 0, iLen = nTrs.length; i < iLen; i++) {
        for (j = 0, jLen = nTrs[i].childNodes.length; j < jLen; j++) {
          nTd = nTrs[i].childNodes[j];
          sNodeName = nTd.nodeName.toUpperCase();
          if (sNodeName == "TD" || sNodeName == "TH") {
            nTds.push(nTd);
          }
        }
      }

      /* Now process by column */
      for (iColumn = 0, iColumns = oSettings.aoColumns.length; iColumn < iColumns; iColumn++) {
        oCol = oSettings.aoColumns[iColumn];

        /* Get the title of the column - unless there is a user set one */
        if (oCol.sTitle === null) {
          oCol.sTitle = oCol.nTh.innerHTML;
        }

        var 
          bAutoType = oCol._bAutoType,
            bRender = typeof oCol.fnRender === 'function',
            bClass = oCol.sClass !== null,
            bVisible = oCol.bVisible,
            nCell, sThisType, sRendered, sValType;

        /* A single loop to rule them all (and be more efficient) */
        if (bAutoType || bRender || bClass || !bVisible) {
          for (iRow = 0, iRows = oSettings.aoData.length; iRow < iRows; iRow++) {
            oData = oSettings.aoData[iRow];
            nCell = nTds[(iRow * iColumns) + iColumn];

            /* Type detection */
            if (bAutoType && oCol.sType != 'string') {
              sValType = _fnGetCellData(oSettings, iRow, iColumn, 'type');
              if (sValType !== '') {
                sThisType = _fnDetectType(sValType);
                if (oCol.sType === null) {
                  oCol.sType = sThisType;
                } else if (oCol.sType != sThisType && oCol.sType != "html") {
                  /* String is always the 'fallback' option */
                  oCol.sType = 'string';
                }
              }
            }

            if (typeof oCol.mData === 'function') {
              nCell.innerHTML = _fnGetCellData(oSettings, iRow, iColumn, 'display');
            }

            /* Rendering */
            if (bRender) {
              sRendered = _fnRender(oSettings, iRow, iColumn);
              nCell.innerHTML = sRendered;
              if (oCol.bUseRendered) {
                /* Use the rendered data for filtering / sorting */
                _fnSetCellData(oSettings, iRow, iColumn, sRendered);
              }
            }

            /* Classes */
            if (bClass) {
              nCell.className += ' ' + oCol.sClass;
            }

            /* Column visibility */
            if (!bVisible) {
              oData._anHidden[iColumn] = nCell;
              nCell.parentNode.removeChild(nCell);
            } else {
              oData._anHidden[iColumn] = null;
            }

            if (oCol.fnCreatedCell) {
              oCol.fnCreatedCell.call(oSettings.oInstance, nCell, _fnGetCellData(oSettings, iRow, iColumn, 'display'), oData._aData, iRow, iColumn);
            }
          }
        }
      }

      /* Row created callbacks */
      if (oSettings.aoRowCreatedCallback.length !== 0) {
        for (i = 0, iLen = oSettings.aoData.length; i < iLen; i++) {
          oData = oSettings.aoData[i];
          _fnCallbackFire(oSettings, 'aoRowCreatedCallback', null, [oData.nTr, oData._aData, i]);
        }
      }
    }


    /**
    * Take a TR element and convert it to an index in aoData
    *  @param {object} oSettings dataTables settings object
    *  @param {node} n the TR element to find
    *  @returns {int} index if the node is found, null if not
    *  @memberof DataTable#oApi
    */
    function _fnNodeToDataIndex(oSettings, n) {
      return (n._DT_RowIndex !== undefined) ? n._DT_RowIndex : null;
    }


    /**
    * Take a TD element and convert it into a column data index (not the visible index)
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow The row number the TD/TH can be found in
    *  @param {node} n The TD/TH element to find
    *  @returns {int} index if the node is found, -1 if not
    *  @memberof DataTable#oApi
    */
    function _fnNodeToColumnIndex(oSettings, iRow, n) {
      var anCells = _fnGetTdNodes(oSettings, iRow);

      for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        if (anCells[i] === n) {
          return i;
        }
      }
      return -1;
    }


    /**
    * Get an array of data for a given row from the internal data cache
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow aoData row id
    *  @param {string} sSpecific data get type ('type' 'filter' 'sort')
    *  @param {array} aiColumns Array of column indexes to get data from
    *  @returns {array} Data array
    *  @memberof DataTable#oApi
    */
    function _fnGetRowData(oSettings, iRow, sSpecific, aiColumns) {
      var out = [];
      for (var i = 0, iLen = aiColumns.length; i < iLen; i++) {
        out.push(_fnGetCellData(oSettings, iRow, aiColumns[i], sSpecific));
      }
      return out;
    }


    /**
    * Get the data for a given cell from the internal cache, taking into account data mapping
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow aoData row id
    *  @param {int} iCol Column index
    *  @param {string} sSpecific data get type ('display', 'type' 'filter' 'sort')
    *  @returns {*} Cell data
    *  @memberof DataTable#oApi
    */
    function _fnGetCellData(oSettings, iRow, iCol, sSpecific) {
      var sData;
      var oCol = oSettings.aoColumns[iCol];
      var oData = oSettings.aoData[iRow]._aData;

      if ((sData = oCol.fnGetData(oData, sSpecific)) === undefined) {
        if (oSettings.iDrawError != oSettings.iDraw && oCol.sDefaultContent === null) {
          _fnLog(oSettings, 0, "Requested unknown parameter " + (typeof oCol.mData == 'function' ? '{mData function}' : "'" + oCol.mData + "'") + " from the data source for row " + iRow);
          oSettings.iDrawError = oSettings.iDraw;
        }
        return oCol.sDefaultContent;
      }

      /* When the data source is null, we can use default column data */
      if (sData === null && oCol.sDefaultContent !== null) {
        sData = oCol.sDefaultContent;
      } else if (typeof sData === 'function') {
        /* If the data source is a function, then we run it and use the return */
        return sData();
      }

      if (sSpecific == 'display' && sData === null) {
        return '';
      }
      return sData;
    }


    /**
    * Set the value for a specific cell, into the internal data cache
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow aoData row id
    *  @param {int} iCol Column index
    *  @param {*} val Value to set
    *  @memberof DataTable#oApi
    */
    function _fnSetCellData(oSettings, iRow, iCol, val) {
      var oCol = oSettings.aoColumns[iCol];
      var oData = oSettings.aoData[iRow]._aData;

      oCol.fnSetData(oData, val);
    }


    // Private variable that is used to match array syntax in the data property object
    var __reArray = /\[.*?\]$/;

    /**
    * Return a function that can be used to get data from a source object, taking
    * into account the ability to use nested objects as a source
    *  @param {string|int|function} mSource The data source for the object
    *  @returns {function} Data get function
    *  @memberof DataTable#oApi
    */
    function _fnGetObjectDataFn(mSource) {
      if (mSource === null) {
        /* Give an empty string for rendering / sorting etc */
        return function (data, type) {
          return null;
        };
      } else if (typeof mSource === 'function') {
        return function (data, type, extra) {
          return mSource(data, type, extra);
        };
      } else if (typeof mSource === 'string' && (mSource.indexOf('.') !== -1 || mSource.indexOf('[') !== -1)) {
        /* If there is a . in the source string then the data source is in a 
        * nested object so we loop over the data for each level to get the next
        * level down. On each loop we test for undefined, and if found immediately
        * return. This allows entire objects to be missing and sDefaultContent to
        * be used if defined, rather than throwing an error
        */
        var fetchData = function (data, type, src) {
          var a = src.split('.');
          var arrayNotation, out, innerSrc;

          if (src !== "") {
            for (var i = 0, iLen = a.length; i < iLen; i++) {
              // Check if we are dealing with an array notation request
              arrayNotation = a[i].match(__reArray);

              if (arrayNotation) {
                a[i] = a[i].replace(__reArray, '');

                // Condition allows simply [] to be passed in
                if (a[i] !== "") {
                  data = data[a[i]];
                }
                out = [];

                // Get the remainder of the nested object to get
                a.splice(0, i + 1);
                innerSrc = a.join('.');

                // Traverse each entry in the array getting the properties requested
                for (var j = 0, jLen = data.length; j < jLen; j++) {
                  out.push(fetchData(data[j], type, innerSrc));
                }

                // If a string is given in between the array notation indicators, that
                // is used to join the strings together, otherwise an array is returned
                var join = arrayNotation[0].substring(1, arrayNotation[0].length - 1);
                data = (join === "") ? out : out.join(join);

                // The inner call to fetchData has already traversed through the remainder
                // of the source requested, so we exit from the loop
                break;
              }

              if (data === null || data[a[i]] === undefined) {
                return undefined;
              }
              data = data[a[i]];
            }
          }

          return data;
        };

        return function (data, type) {
          return fetchData(data, type, mSource);
        };
      } else {
        /* Array or flat object mapping */
        return function (data, type) {
          return data[mSource];
        };
      }
    }


    /**
    * Return a function that can be used to set data from a source object, taking
    * into account the ability to use nested objects as a source
    *  @param {string|int|function} mSource The data source for the object
    *  @returns {function} Data set function
    *  @memberof DataTable#oApi
    */
    function _fnSetObjectDataFn(mSource) {
      if (mSource === null) {
        /* Nothing to do when the data source is null */
        return function (data, val) { };
      } else if (typeof mSource === 'function') {
        return function (data, val) {
          mSource(data, 'set', val);
        };
      } else if (typeof mSource === 'string' && (mSource.indexOf('.') !== -1 || mSource.indexOf('[') !== -1)) {
        /* Like the get, we need to get data from a nested object */
        var setData = function (data, val, src) {
          var a = src.split('.'),
                b;
          var arrayNotation, o, innerSrc;

          for (var i = 0, iLen = a.length - 1; i < iLen; i++) {
            // Check if we are dealing with an array notation request
            arrayNotation = a[i].match(__reArray);

            if (arrayNotation) {
              a[i] = a[i].replace(__reArray, '');
              data[a[i]] = [];

              // Get the remainder of the nested object to set so we can recurse
              b = a.slice();
              b.splice(0, i + 1);
              innerSrc = b.join('.');

              // Traverse each entry in the array setting the properties requested
              for (var j = 0, jLen = val.length; j < jLen; j++) {
                o = {};
                setData(o, val[j], innerSrc);
                data[a[i]].push(o);
              }

              // The inner call to setData has already traversed through the remainder
              // of the source and has set the data, thus we can exit here
              return;
            }

            // If the nested object doesn't currently exist - since we are
            // trying to set the value - create it
            if (data[a[i]] === null || data[a[i]] === undefined) {
              data[a[i]] = {};
            }
            data = data[a[i]];
          }

          // If array notation is used, we just want to strip it and use the property name
          // and assign the value. If it isn't used, then we get the result we want anyway
          data[a[a.length - 1].replace(__reArray, '')] = val;
        };

        return function (data, val) {
          return setData(data, val, mSource);
        };
      } else {
        /* Array or flat object mapping */
        return function (data, val) {
          data[mSource] = val;
        };
      }
    }


    /**
    * Return an array with the full table data
    *  @param {object} oSettings dataTables settings object
    *  @returns array {array} aData Master data array
    *  @memberof DataTable#oApi
    */
    function _fnGetDataMaster(oSettings) {
      var aData = [];
      var iLen = oSettings.aoData.length;
      for (var i = 0; i < iLen; i++) {
        aData.push(oSettings.aoData[i]._aData);
      }
      return aData;
    }


    /**
    * Nuke the table
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnClearTable(oSettings) {
      oSettings.aoData.splice(0, oSettings.aoData.length);
      oSettings.aiDisplayMaster.splice(0, oSettings.aiDisplayMaster.length);
      oSettings.aiDisplay.splice(0, oSettings.aiDisplay.length);
      _fnCalculateEnd(oSettings);
    }


    /**
    * Take an array of integers (index array) and remove a target integer (value - not 
    * the key!)
    *  @param {array} a Index array to target
    *  @param {int} iTarget value to find
    *  @memberof DataTable#oApi
    */
    function _fnDeleteIndex(a, iTarget) {
      var iTargetIndex = -1;

      for (var i = 0, iLen = a.length; i < iLen; i++) {
        if (a[i] == iTarget) {
          iTargetIndex = i;
        } else if (a[i] > iTarget) {
          a[i]--;
        }
      }

      if (iTargetIndex != -1) {
        a.splice(iTargetIndex, 1);
      }
    }


    /**
    * Call the developer defined fnRender function for a given cell (row/column) with
    * the required parameters and return the result.
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow aoData index for the row
    *  @param {int} iCol aoColumns index for the column
    *  @returns {*} Return of the developer's fnRender function
    *  @memberof DataTable#oApi
    */
    function _fnRender(oSettings, iRow, iCol) {
      var oCol = oSettings.aoColumns[iCol];

      return oCol.fnRender({
        "iDataRow": iRow,
        "iDataColumn": iCol,
        "oSettings": oSettings,
        "aData": oSettings.aoData[iRow]._aData,
        "mDataProp": oCol.mData
      }, _fnGetCellData(oSettings, iRow, iCol, 'display'));
    }

    /**
    * Create a new TR element (and it's TD children) for a row
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iRow Row to consider
    *  @memberof DataTable#oApi
    */
    function _fnCreateTr(oSettings, iRow) {
      var oData = oSettings.aoData[iRow];
      var nTd;

      if (oData.nTr === null) {
        oData.nTr = document.createElement('tr');

        /* Use a private property on the node to allow reserve mapping from the node
        * to the aoData array for fast look up
        */
        oData.nTr._DT_RowIndex = iRow;

        /* Special parameters can be given by the data source to be used on the row */
        if (oData._aData.DT_RowId) {
          oData.nTr.id = oData._aData.DT_RowId;
        }

        if (oData._aData.DT_RowClass) {
          $(oData.nTr).addClass(oData._aData.DT_RowClass);
        }

        /* Process each column */
        for (var i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
          var oCol = oSettings.aoColumns[i];
          nTd = document.createElement(oCol.sCellType);

          /* Render if needed - if bUseRendered is true then we already have the rendered
          * value in the data source - so can just use that
          */
          nTd.innerHTML = (typeof oCol.fnRender === 'function' && (!oCol.bUseRendered || oCol.mData === null)) ? _fnRender(oSettings, iRow, i) : _fnGetCellData(oSettings, iRow, i, 'display');

          /* Add user defined class */
          if (oCol.sClass !== null) {
            nTd.className = oCol.sClass;
          }

          if (oCol.bVisible) {
            oData.nTr.appendChild(nTd);
            oData._anHidden[i] = null;
          } else {
            oData._anHidden[i] = nTd;
          }

          if (oCol.fnCreatedCell) {
            oCol.fnCreatedCell.call(oSettings.oInstance, nTd, _fnGetCellData(oSettings, iRow, i, 'display'), oData._aData, iRow, i);
          }
        }

        _fnCallbackFire(oSettings, 'aoRowCreatedCallback', null, [oData.nTr, oData._aData, iRow]);
      }
    }


    /**
    * Create the HTML header for the table
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnBuildHead(oSettings) {
      var i, nTh, iLen, j, jLen;
      var iThs = oSettings.nTHead.getElementsByTagName('th').length;
      var iCorrector = 0;
      var jqChildren;

      /* If there is a header in place - then use it - otherwise it's going to get nuked... */
      if (iThs !== 0) {
        /* We've got a thead from the DOM, so remove hidden columns and apply width to vis cols */
        for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
          nTh = oSettings.aoColumns[i].nTh;
          nTh.setAttribute('role', 'columnheader');
          if (oSettings.aoColumns[i].bSortable) {
            nTh.setAttribute('tabindex', oSettings.iTabIndex);
            nTh.setAttribute('aria-controls', oSettings.sTableId);
          }

          if (oSettings.aoColumns[i].sClass !== null) {
            $(nTh).addClass(oSettings.aoColumns[i].sClass);
          }

          /* Set the title of the column if it is user defined (not what was auto detected) */
          if (oSettings.aoColumns[i].sTitle != nTh.innerHTML) {
            nTh.innerHTML = oSettings.aoColumns[i].sTitle;
          }
        }
      } else {
        /* We don't have a header in the DOM - so we are going to have to create one */
        var nTr = document.createElement("tr");

        for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
          nTh = oSettings.aoColumns[i].nTh;
          nTh.innerHTML = oSettings.aoColumns[i].sTitle;
          nTh.setAttribute('tabindex', '0');

          if (oSettings.aoColumns[i].sClass !== null) {
            $(nTh).addClass(oSettings.aoColumns[i].sClass);
          }

          nTr.appendChild(nTh);
        }
        $(oSettings.nTHead).html('')[0].appendChild(nTr);
        _fnDetectHeader(oSettings.aoHeader, oSettings.nTHead);
      }

      /* ARIA role for the rows */
      $(oSettings.nTHead).children('tr').attr('role', 'row');

      /* Add the extra markup needed by jQuery UI's themes */
      if (oSettings.bJUI) {
        for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
          nTh = oSettings.aoColumns[i].nTh;

          var nDiv = document.createElement('div');
          nDiv.className = oSettings.oClasses.sSortJUIWrapper;
          $(nTh).contents().appendTo(nDiv);

          var nSpan = document.createElement('span');
          nSpan.className = oSettings.oClasses.sSortIcon;
          nDiv.appendChild(nSpan);
          nTh.appendChild(nDiv);
        }
      }

      if (oSettings.oFeatures.bSort) {
        for (i = 0; i < oSettings.aoColumns.length; i++) {
          if (oSettings.aoColumns[i].bSortable !== false) {
            _fnSortAttachListener(oSettings, oSettings.aoColumns[i].nTh, i);
          } else {
            $(oSettings.aoColumns[i].nTh).addClass(oSettings.oClasses.sSortableNone);
          }
        }
      }

      /* Deal with the footer - add classes if required */
      if (oSettings.oClasses.sFooterTH !== "") {
        $(oSettings.nTFoot).children('tr').children('th').addClass(oSettings.oClasses.sFooterTH);
      }

      /* Cache the footer elements */
      if (oSettings.nTFoot !== null) {
        var anCells = _fnGetUniqueThs(oSettings, null, oSettings.aoFooter);
        for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
          if (anCells[i]) {
            oSettings.aoColumns[i].nTf = anCells[i];
            if (oSettings.aoColumns[i].sClass) {
              $(anCells[i]).addClass(oSettings.aoColumns[i].sClass);
            }
          }
        }
      }
    }


    /**
    * Draw the header (or footer) element based on the column visibility states. The
    * methodology here is to use the layout array from _fnDetectHeader, modified for
    * the instantaneous column visibility, to construct the new layout. The grid is
    * traversed over cell at a time in a rows x columns grid fashion, although each 
    * cell insert can cover multiple elements in the grid - which is tracks using the
    * aApplied array. Cell inserts in the grid will only occur where there isn't
    * already a cell in that position.
    *  @param {object} oSettings dataTables settings object
    *  @param array {objects} aoSource Layout array from _fnDetectHeader
    *  @param {boolean} [bIncludeHidden=false] If true then include the hidden columns in the calc, 
    *  @memberof DataTable#oApi
    */
    function _fnDrawHead(oSettings, aoSource, bIncludeHidden) {
      var i, iLen, j, jLen, k, kLen, n, nLocalTr;
      var aoLocal = [];
      var aApplied = [];
      var iColumns = oSettings.aoColumns.length;
      var iRowspan, iColspan;

      if (bIncludeHidden === undefined) {
        bIncludeHidden = false;
      }

      /* Make a copy of the master layout array, but without the visible columns in it */
      for (i = 0, iLen = aoSource.length; i < iLen; i++) {
        aoLocal[i] = aoSource[i].slice();
        aoLocal[i].nTr = aoSource[i].nTr;

        /* Remove any columns which are currently hidden */
        for (j = iColumns - 1; j >= 0; j--) {
          if (!oSettings.aoColumns[j].bVisible && !bIncludeHidden) {
            aoLocal[i].splice(j, 1);
          }
        }

        /* Prep the applied array - it needs an element for each row */
        aApplied.push([]);
      }

      for (i = 0, iLen = aoLocal.length; i < iLen; i++) {
        nLocalTr = aoLocal[i].nTr;

        /* All cells are going to be replaced, so empty out the row */
        if (nLocalTr) {
          while ((n = nLocalTr.firstChild)) {
            nLocalTr.removeChild(n);
          }
        }

        for (j = 0, jLen = aoLocal[i].length; j < jLen; j++) {
          iRowspan = 1;
          iColspan = 1;

          /* Check to see if there is already a cell (row/colspan) covering our target
          * insert point. If there is, then there is nothing to do.
          */
          if (aApplied[i][j] === undefined) {
            nLocalTr.appendChild(aoLocal[i][j].cell);
            aApplied[i][j] = 1;

            /* Expand the cell to cover as many rows as needed */
            while (aoLocal[i + iRowspan] !== undefined && aoLocal[i][j].cell == aoLocal[i + iRowspan][j].cell) {
              aApplied[i + iRowspan][j] = 1;
              iRowspan++;
            }

            /* Expand the cell to cover as many columns as needed */
            while (aoLocal[i][j + iColspan] !== undefined && aoLocal[i][j].cell == aoLocal[i][j + iColspan].cell) {
              /* Must update the applied array over the rows for the columns */
              for (k = 0; k < iRowspan; k++) {
                aApplied[i + k][j + iColspan] = 1;
              }
              iColspan++;
            }

            /* Do the actual expansion in the DOM */
            aoLocal[i][j].cell.rowSpan = iRowspan;
            aoLocal[i][j].cell.colSpan = iColspan;
          }
        }
      }
    }


    /**
    * Insert the required TR nodes into the table for display
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnDraw(oSettings) {
      /* Provide a pre-callback function which can be used to cancel the draw is false is returned */
      var aPreDraw = _fnCallbackFire(oSettings, 'aoPreDrawCallback', 'preDraw', [oSettings]);
      if ($.inArray(false, aPreDraw) !== -1) {
        _fnProcessingDisplay(oSettings, false);
        return;
      }

      var i, iLen, n;
      var anRows = [];
      var iRowCount = 0;
      var iStripes = oSettings.asStripeClasses.length;
      var iOpenRows = oSettings.aoOpenRows.length;

      oSettings.bDrawing = true;

      /* Check and see if we have an initial draw position from state saving */
      if (oSettings.iInitDisplayStart !== undefined && oSettings.iInitDisplayStart != -1) {
        if (oSettings.oFeatures.bServerSide) {
          oSettings._iDisplayStart = oSettings.iInitDisplayStart;
        } else {
          oSettings._iDisplayStart = (oSettings.iInitDisplayStart >= oSettings.fnRecordsDisplay()) ? 0 : oSettings.iInitDisplayStart;
        }
        oSettings.iInitDisplayStart = -1;
        _fnCalculateEnd(oSettings);
      }

      /* Server-side processing draw intercept */
      if (oSettings.bDeferLoading) {
        oSettings.bDeferLoading = false;
        oSettings.iDraw++;
      } else if (!oSettings.oFeatures.bServerSide) {
        oSettings.iDraw++;
      } else if (!oSettings.bDestroying && !_fnAjaxUpdate(oSettings)) {
        return;
      }

      if (oSettings.aiDisplay.length !== 0) {
        var iStart = oSettings._iDisplayStart;
        var iEnd = oSettings._iDisplayEnd;

        if (oSettings.oFeatures.bServerSide) {
          iStart = 0;
          iEnd = oSettings.aoData.length;
        }

        for (var j = iStart; j < iEnd; j++) {
          var aoData = oSettings.aoData[oSettings.aiDisplay[j]];
          if (aoData.nTr === null) {
            _fnCreateTr(oSettings, oSettings.aiDisplay[j]);
          }

          var nRow = aoData.nTr;

          /* Remove the old striping classes and then add the new one */
          if (iStripes !== 0) {
            var sStripe = oSettings.asStripeClasses[iRowCount % iStripes];
            if (aoData._sRowStripe != sStripe) {
              $(nRow).removeClass(aoData._sRowStripe).addClass(sStripe);
              aoData._sRowStripe = sStripe;
            }
          }

          /* Row callback functions - might want to manipulate the row */
          _fnCallbackFire(oSettings, 'aoRowCallback', null, [nRow, oSettings.aoData[oSettings.aiDisplay[j]]._aData, iRowCount, j]);

          anRows.push(nRow);
          iRowCount++;

          /* If there is an open row - and it is attached to this parent - attach it on redraw */
          if (iOpenRows !== 0) {
            for (var k = 0; k < iOpenRows; k++) {
              if (nRow == oSettings.aoOpenRows[k].nParent) {
                anRows.push(oSettings.aoOpenRows[k].nTr);
                break;
              }
            }
          }
        }
      } else {
        /* Table is empty - create a row with an empty message in it */
        anRows[0] = document.createElement('tr');

        if (oSettings.asStripeClasses[0]) {
          anRows[0].className = oSettings.asStripeClasses[0];
        }

        var oLang = oSettings.oLanguage;
        var sZero = oLang.sZeroRecords;
        if (oSettings.iDraw == 1 && oSettings.sAjaxSource !== null && !oSettings.oFeatures.bServerSide) {
          sZero = oLang.sLoadingRecords;
        } else if (oLang.sEmptyTable && oSettings.fnRecordsTotal() === 0) {
          sZero = oLang.sEmptyTable;
        }

        var nTd = document.createElement('td');
        nTd.setAttribute('valign', "top");
        nTd.colSpan = _fnVisbleColumns(oSettings);
        nTd.className = oSettings.oClasses.sRowEmpty;
        nTd.innerHTML = _fnInfoMacros(oSettings, sZero);

        anRows[iRowCount].appendChild(nTd);
      }

      /* Header and footer callbacks */
      _fnCallbackFire(oSettings, 'aoHeaderCallback', 'header', [$(oSettings.nTHead).children('tr')[0], _fnGetDataMaster(oSettings), oSettings._iDisplayStart, oSettings.fnDisplayEnd(), oSettings.aiDisplay]);

      _fnCallbackFire(oSettings, 'aoFooterCallback', 'footer', [$(oSettings.nTFoot).children('tr')[0], _fnGetDataMaster(oSettings), oSettings._iDisplayStart, oSettings.fnDisplayEnd(), oSettings.aiDisplay]);

      /* 
      * Need to remove any old row from the display - note we can't just empty the tbody using
      * $().html('') since this will unbind the jQuery event handlers (even although the node 
      * still exists!) - equally we can't use innerHTML, since IE throws an exception.
      */
      var 
        nAddFrag = document.createDocumentFragment(),
          nRemoveFrag = document.createDocumentFragment(),
          nBodyPar, nTrs;

      if (oSettings.nTBody) {
        nBodyPar = oSettings.nTBody.parentNode;
        nRemoveFrag.appendChild(oSettings.nTBody);

        /* When doing infinite scrolling, only remove child rows when sorting, filtering or start
        * up. When not infinite scroll, always do it.
        */
        if (!oSettings.oScroll.bInfinite || !oSettings._bInitComplete || oSettings.bSorted || oSettings.bFiltered) {
          while ((n = oSettings.nTBody.firstChild)) {
            oSettings.nTBody.removeChild(n);
          }
        }

        /* Put the draw table into the dom */
        for (i = 0, iLen = anRows.length; i < iLen; i++) {
          nAddFrag.appendChild(anRows[i]);
        }

        oSettings.nTBody.appendChild(nAddFrag);
        if (nBodyPar !== null) {
          nBodyPar.appendChild(oSettings.nTBody);
        }
      }

      /* Call all required callback functions for the end of a draw */
      _fnCallbackFire(oSettings, 'aoDrawCallback', 'draw', [oSettings]);

      /* Draw is complete, sorting and filtering must be as well */
      oSettings.bSorted = false;
      oSettings.bFiltered = false;
      oSettings.bDrawing = false;

      if (oSettings.oFeatures.bServerSide) {
        _fnProcessingDisplay(oSettings, false);
        if (!oSettings._bInitComplete) {
          _fnInitComplete(oSettings);
        }
      }
    }


    /**
    * Redraw the table - taking account of the various features which are enabled
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnReDraw(oSettings) {
      if (oSettings.oFeatures.bSort) {
        /* Sorting will refilter and draw for us */
        _fnSort(oSettings, oSettings.oPreviousSearch);
      } else if (oSettings.oFeatures.bFilter) {
        /* Filtering will redraw for us */
        _fnFilterComplete(oSettings, oSettings.oPreviousSearch);
      } else {
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      }
    }


    /**
    * Add the options to the page HTML for the table
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnAddOptionsHtml(oSettings) {
      /*
      * Create a temporary, empty, div which we can later on replace with what we have generated
      * we do it this way to rendering the 'options' html offline - speed :-)
      */
      var nHolding = $('<div></div>')[0];
      oSettings.nTable.parentNode.insertBefore(nHolding, oSettings.nTable);

      /* 
      * All DataTables are wrapped in a div
      */
      oSettings.nTableWrapper = $('<div id="' + oSettings.sTableId + '_wrapper" class="' + oSettings.oClasses.sWrapper + '" role="grid"></div>')[0];
      oSettings.nTableReinsertBefore = oSettings.nTable.nextSibling;

      /* Track where we want to insert the option */
      var nInsertNode = oSettings.nTableWrapper;

      /* Loop over the user set positioning and place the elements as needed */
      var aDom = oSettings.sDom.split('');
      var nTmp, iPushFeature, cOption, nNewNode, cNext, sAttr, j;
      for (var i = 0; i < aDom.length; i++) {
        iPushFeature = 0;
        cOption = aDom[i];

        if (cOption == '<') {
          /* New container div */
          nNewNode = $('<div></div>')[0];

          /* Check to see if we should append an id and/or a class name to the container */
          cNext = aDom[i + 1];
          if (cNext == "'" || cNext == '"') {
            sAttr = "";
            j = 2;
            while (aDom[i + j] != cNext) {
              sAttr += aDom[i + j];
              j++;
            }

            /* Replace jQuery UI constants */
            if (sAttr == "H") {
              sAttr = oSettings.oClasses.sJUIHeader;
            } else if (sAttr == "F") {
              sAttr = oSettings.oClasses.sJUIFooter;
            }

            /* The attribute can be in the format of "#id.class", "#id" or "class" This logic
            * breaks the string into parts and applies them as needed
            */
            if (sAttr.indexOf('.') != -1) {
              var aSplit = sAttr.split('.');
              nNewNode.id = aSplit[0].substr(1, aSplit[0].length - 1);
              nNewNode.className = aSplit[1];
            } else if (sAttr.charAt(0) == "#") {
              nNewNode.id = sAttr.substr(1, sAttr.length - 1);
            } else {
              nNewNode.className = sAttr;
            }

            i += j; /* Move along the position array */
          }

          nInsertNode.appendChild(nNewNode);
          nInsertNode = nNewNode;
        } else if (cOption == '>') {
          /* End container div */
          nInsertNode = nInsertNode.parentNode;
        } else if (cOption == 'l' && oSettings.oFeatures.bPaginate && oSettings.oFeatures.bLengthChange) {
          /* Length */
          nTmp = _fnFeatureHtmlLength(oSettings);
          iPushFeature = 1;
        } else if (cOption == 'f' && oSettings.oFeatures.bFilter) {
          /* Filter */
          nTmp = _fnFeatureHtmlFilter(oSettings);
          iPushFeature = 1;
        } else if (cOption == 'r' && oSettings.oFeatures.bProcessing) {
          /* pRocessing */
          nTmp = _fnFeatureHtmlProcessing(oSettings);
          iPushFeature = 1;
        } else if (cOption == 't') {
          /* Table */
          nTmp = _fnFeatureHtmlTable(oSettings);
          iPushFeature = 1;
        } else if (cOption == 'i' && oSettings.oFeatures.bInfo) {
          /* Info */
          nTmp = _fnFeatureHtmlInfo(oSettings);
          iPushFeature = 1;
        } else if (cOption == 'p' && oSettings.oFeatures.bPaginate) {
          /* Pagination */
          nTmp = _fnFeatureHtmlPaginate(oSettings);
          iPushFeature = 1;
        } else if (DataTable.ext.aoFeatures.length !== 0) {
          /* Plug-in features */
          var aoFeatures = DataTable.ext.aoFeatures;
          for (var k = 0, kLen = aoFeatures.length; k < kLen; k++) {
            if (cOption == aoFeatures[k].cFeature) {
              nTmp = aoFeatures[k].fnInit(oSettings);
              if (nTmp) {
                iPushFeature = 1;
              }
              break;
            }
          }
        }

        /* Add to the 2D features array */
        if (iPushFeature == 1 && nTmp !== null) {
          if (typeof oSettings.aanFeatures[cOption] !== 'object') {
            oSettings.aanFeatures[cOption] = [];
          }
          oSettings.aanFeatures[cOption].push(nTmp);
          nInsertNode.appendChild(nTmp);
        }
      }

      /* Built our DOM structure - replace the holding div with what we want */
      nHolding.parentNode.replaceChild(oSettings.nTableWrapper, nHolding);
    }


    /**
    * Use the DOM source to create up an array of header cells. The idea here is to
    * create a layout grid (array) of rows x columns, which contains a reference
    * to the cell that that point in the grid (regardless of col/rowspan), such that
    * any column / row could be removed and the new grid constructed
    *  @param array {object} aLayout Array to store the calculated layout in
    *  @param {node} nThead The header/footer element for the table
    *  @memberof DataTable#oApi
    */
    function _fnDetectHeader(aLayout, nThead) {
      var nTrs = $(nThead).children('tr');
      var nCell;
      var i, j, k, l, iLen, jLen, iColShifted;
      var fnShiftCol = function (a, i, j) {
        while (a[i][j]) {
          j++;
        }
        return j;
      };

      aLayout.splice(0, aLayout.length);

      /* We know how many rows there are in the layout - so prep it */
      for (i = 0, iLen = nTrs.length; i < iLen; i++) {
        aLayout.push([]);
      }

      /* Calculate a layout array */
      for (i = 0, iLen = nTrs.length; i < iLen; i++) {
        var iColumn = 0;

        /* For every cell in the row... */
        for (j = 0, jLen = nTrs[i].childNodes.length; j < jLen; j++) {
          nCell = nTrs[i].childNodes[j];

          if (nCell.nodeName.toUpperCase() == "TD" || nCell.nodeName.toUpperCase() == "TH") {
            /* Get the col and rowspan attributes from the DOM and sanitise them */
            var iColspan = nCell.getAttribute('colspan') * 1;
            var iRowspan = nCell.getAttribute('rowspan') * 1;
            iColspan = (!iColspan || iColspan === 0 || iColspan === 1) ? 1 : iColspan;
            iRowspan = (!iRowspan || iRowspan === 0 || iRowspan === 1) ? 1 : iRowspan;

            /* There might be colspan cells already in this row, so shift our target 
            * accordingly
            */
            iColShifted = fnShiftCol(aLayout, i, iColumn);

            /* If there is col / rowspan, copy the information into the layout grid */
            for (l = 0; l < iColspan; l++) {
              for (k = 0; k < iRowspan; k++) {
                aLayout[i + k][iColShifted + l] = {
                  "cell": nCell,
                  "unique": iColspan == 1 ? true : false
                };
                aLayout[i + k].nTr = nTrs[i];
              }
            }
          }
        }
      }
    }


    /**
    * Get an array of unique th elements, one for each column
    *  @param {object} oSettings dataTables settings object
    *  @param {node} nHeader automatically detect the layout from this node - optional
    *  @param {array} aLayout thead/tfoot layout from _fnDetectHeader - optional
    *  @returns array {node} aReturn list of unique th's
    *  @memberof DataTable#oApi
    */
    function _fnGetUniqueThs(oSettings, nHeader, aLayout) {
      var aReturn = [];
      if (!aLayout) {
        aLayout = oSettings.aoHeader;
        if (nHeader) {
          aLayout = [];
          _fnDetectHeader(aLayout, nHeader);
        }
      }

      for (var i = 0, iLen = aLayout.length; i < iLen; i++) {
        for (var j = 0, jLen = aLayout[i].length; j < jLen; j++) {
          if (aLayout[i][j].unique && (!aReturn[j] || !oSettings.bSortCellsTop)) {
            aReturn[j] = aLayout[i][j].cell;
          }
        }
      }

      return aReturn;
    }



    /**
    * Update the table using an Ajax call
    *  @param {object} oSettings dataTables settings object
    *  @returns {boolean} Block the table drawing or not
    *  @memberof DataTable#oApi
    */
    function _fnAjaxUpdate(oSettings) {
      if (oSettings.bAjaxDataGet) {
        oSettings.iDraw++;
        _fnProcessingDisplay(oSettings, true);
        var iColumns = oSettings.aoColumns.length;
        var aoData = _fnAjaxParameters(oSettings);
        _fnServerParams(oSettings, aoData);

        oSettings.fnServerData.call(oSettings.oInstance, oSettings.sAjaxSource, aoData, function (json) {
          _fnAjaxUpdateDraw(oSettings, json);
        }, oSettings);
        return false;
      } else {
        return true;
      }
    }


    /**
    * Build up the parameters in an object needed for a server-side processing request
    *  @param {object} oSettings dataTables settings object
    *  @returns {bool} block the table drawing or not
    *  @memberof DataTable#oApi
    */
    function _fnAjaxParameters(oSettings) {
      var iColumns = oSettings.aoColumns.length;
      var aoData = [],
          mDataProp, aaSort, aDataSort;
      var i, j;

      aoData.push({
        "name": "sEcho",
        "value": oSettings.iDraw
      });
      aoData.push({
        "name": "iColumns",
        "value": iColumns
      });
      aoData.push({
        "name": "sColumns",
        "value": _fnColumnOrdering(oSettings)
      });
      aoData.push({
        "name": "iDisplayStart",
        "value": oSettings._iDisplayStart
      });
      aoData.push({
        "name": "iDisplayLength",
        "value": oSettings.oFeatures.bPaginate !== false ? oSettings._iDisplayLength : -1
      });

      for (i = 0; i < iColumns; i++) {
        mDataProp = oSettings.aoColumns[i].mData;
        aoData.push({
          "name": "mDataProp_" + i,
          "value": typeof (mDataProp) === "function" ? 'function' : mDataProp
        });
      }

      /* Filtering */
      if (oSettings.oFeatures.bFilter !== false) {
        aoData.push({
          "name": "sSearch",
          "value": oSettings.oPreviousSearch.sSearch
        });
        aoData.push({
          "name": "bRegex",
          "value": oSettings.oPreviousSearch.bRegex
        });
        for (i = 0; i < iColumns; i++) {
          aoData.push({
            "name": "sSearch_" + i,
            "value": oSettings.aoPreSearchCols[i].sSearch
          });
          aoData.push({
            "name": "bRegex_" + i,
            "value": oSettings.aoPreSearchCols[i].bRegex
          });
          aoData.push({
            "name": "bSearchable_" + i,
            "value": oSettings.aoColumns[i].bSearchable
          });
        }
      }

      /* Sorting */
      if (oSettings.oFeatures.bSort !== false) {
        var iCounter = 0;

        aaSort = (oSettings.aaSortingFixed !== null) ? oSettings.aaSortingFixed.concat(oSettings.aaSorting) : oSettings.aaSorting.slice();

        for (i = 0; i < aaSort.length; i++) {
          aDataSort = oSettings.aoColumns[aaSort[i][0]].aDataSort;

          for (j = 0; j < aDataSort.length; j++) {
            aoData.push({
              "name": "iSortCol_" + iCounter,
              "value": aDataSort[j]
            });
            aoData.push({
              "name": "sSortDir_" + iCounter,
              "value": aaSort[i][1]
            });
            iCounter++;
          }
        }
        aoData.push({
          "name": "iSortingCols",
          "value": iCounter
        });

        for (i = 0; i < iColumns; i++) {
          aoData.push({
            "name": "bSortable_" + i,
            "value": oSettings.aoColumns[i].bSortable
          });
        }
      }

      return aoData;
    }


    /**
    * Add Ajax parameters from plug-ins
    *  @param {object} oSettings dataTables settings object
    *  @param array {objects} aoData name/value pairs to send to the server
    *  @memberof DataTable#oApi
    */
    function _fnServerParams(oSettings, aoData) {
      _fnCallbackFire(oSettings, 'aoServerParams', 'serverParams', [aoData]);
    }


    /**
    * Data the data from the server (nuking the old) and redraw the table
    *  @param {object} oSettings dataTables settings object
    *  @param {object} json json data return from the server.
    *  @param {string} json.sEcho Tracking flag for DataTables to match requests
    *  @param {int} json.iTotalRecords Number of records in the data set, not accounting for filtering
    *  @param {int} json.iTotalDisplayRecords Number of records in the data set, accounting for filtering
    *  @param {array} json.aaData The data to display on this page
    *  @param {string} [json.sColumns] Column ordering (sName, comma separated)
    *  @memberof DataTable#oApi
    */
    function _fnAjaxUpdateDraw(oSettings, json) {
      if (json.sEcho !== undefined) {
        /* Protect against old returns over-writing a new one. Possible when you get
        * very fast interaction, and later queries are completed much faster
        */
        if (json.sEcho * 1 < oSettings.iDraw) {
          return;
        } else {
          oSettings.iDraw = json.sEcho * 1;
        }
      }

      if (!oSettings.oScroll.bInfinite || (oSettings.oScroll.bInfinite && (oSettings.bSorted || oSettings.bFiltered))) {
        _fnClearTable(oSettings);
      }
      oSettings._iRecordsTotal = parseInt(json.iTotalRecords, 10);
      oSettings._iRecordsDisplay = parseInt(json.iTotalDisplayRecords, 10);

      /* Determine if reordering is required */
      var sOrdering = _fnColumnOrdering(oSettings);
      var bReOrder = (json.sColumns !== undefined && sOrdering !== "" && json.sColumns != sOrdering);
      var aiIndex;
      if (bReOrder) {
        aiIndex = _fnReOrderIndex(oSettings, json.sColumns);
      }

      var aData = _fnGetObjectDataFn(oSettings.sAjaxDataProp)(json);
      for (var i = 0, iLen = aData.length; i < iLen; i++) {
        if (bReOrder) {
          /* If we need to re-order, then create a new array with the correct order and add it */
          var aDataSorted = [];
          for (var j = 0, jLen = oSettings.aoColumns.length; j < jLen; j++) {
            aDataSorted.push(aData[i][aiIndex[j]]);
          }
          _fnAddData(oSettings, aDataSorted);
        } else {
          /* No re-order required, sever got it "right" - just straight add */
          _fnAddData(oSettings, aData[i]);
        }
      }
      oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();

      oSettings.bAjaxDataGet = false;
      _fnDraw(oSettings);
      oSettings.bAjaxDataGet = true;
      _fnProcessingDisplay(oSettings, false);
    }



    /**
    * Generate the node required for filtering text
    *  @returns {node} Filter control element
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlFilter(oSettings) {
      var oPreviousSearch = oSettings.oPreviousSearch;

      var sSearchStr = oSettings.oLanguage.sSearch;
      sSearchStr = (sSearchStr.indexOf('_INPUT_') !== -1) ? sSearchStr.replace('_INPUT_', '<input type="text" />') : sSearchStr === "" ? '<input type="text" />' : sSearchStr + ' <input type="text" />';

      var nFilter = document.createElement('div');
      nFilter.className = oSettings.oClasses.sFilter;
      nFilter.innerHTML = '<label>' + sSearchStr + '</label>';
      if (!oSettings.aanFeatures.f) {
        nFilter.id = oSettings.sTableId + '_filter';
      }

      var jqFilter = $('input[type="text"]', nFilter);

      // Store a reference to the input element, so other input elements could be
      // added to the filter wrapper if needed (submit button for example)
      nFilter._DT_Input = jqFilter[0];

      jqFilter.val(oPreviousSearch.sSearch.replace('"', '&quot;'));
      jqFilter.bind('keyup.DT', function (e) {
        /* Update all other filter input elements for the new display */
        var n = oSettings.aanFeatures.f;
        var val = this.value === "" ? "" : this.value; // mental IE8 fix :-(
        for (var i = 0, iLen = n.length; i < iLen; i++) {
          if (n[i] != $(this).parents('div.dataTables_filter')[0]) {
            $(n[i]._DT_Input).val(val);
          }
        }

        /* Now do the filter */
        if (val != oPreviousSearch.sSearch) {
          _fnFilterComplete(oSettings, {
            "sSearch": val,
            "bRegex": oPreviousSearch.bRegex,
            "bSmart": oPreviousSearch.bSmart,
            "bCaseInsensitive": oPreviousSearch.bCaseInsensitive
          });
        }
      });

      jqFilter.attr('aria-controls', oSettings.sTableId).bind('keypress.DT', function (e) {
        /* Prevent form submission */
        if (e.keyCode == 13) {
          return false;
        }
      });

      return nFilter;
    }


    /**
    * Filter the table using both the global filter and column based filtering
    *  @param {object} oSettings dataTables settings object
    *  @param {object} oSearch search information
    *  @param {int} [iForce] force a research of the master array (1) or not (undefined or 0)
    *  @memberof DataTable#oApi
    */
    function _fnFilterComplete(oSettings, oInput, iForce) {
      var oPrevSearch = oSettings.oPreviousSearch;
      var aoPrevSearch = oSettings.aoPreSearchCols;
      var fnSaveFilter = function (oFilter) {
        /* Save the filtering values */
        oPrevSearch.sSearch = oFilter.sSearch;
        oPrevSearch.bRegex = oFilter.bRegex;
        oPrevSearch.bSmart = oFilter.bSmart;
        oPrevSearch.bCaseInsensitive = oFilter.bCaseInsensitive;
      };

      /* In server-side processing all filtering is done by the server, so no point hanging around here */
      if (!oSettings.oFeatures.bServerSide) {
        /* Global filter */
        _fnFilter(oSettings, oInput.sSearch, iForce, oInput.bRegex, oInput.bSmart, oInput.bCaseInsensitive);
        fnSaveFilter(oInput);

        /* Now do the individual column filter */
        for (var i = 0; i < oSettings.aoPreSearchCols.length; i++) {
          _fnFilterColumn(oSettings, aoPrevSearch[i].sSearch, i, aoPrevSearch[i].bRegex, aoPrevSearch[i].bSmart, aoPrevSearch[i].bCaseInsensitive);
        }

        /* Custom filtering */
        _fnFilterCustom(oSettings);
      } else {
        fnSaveFilter(oInput);
      }

      /* Tell the draw function we have been filtering */
      oSettings.bFiltered = true;
      $(oSettings.oInstance).trigger('filter', oSettings);

      /* Redraw the table */
      oSettings._iDisplayStart = 0;
      _fnCalculateEnd(oSettings);
      _fnDraw(oSettings);

      /* Rebuild search array 'offline' */
      _fnBuildSearchArray(oSettings, 0);
    }


    /**
    * Apply custom filtering functions
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnFilterCustom(oSettings) {
      var afnFilters = DataTable.ext.afnFiltering;
      var aiFilterColumns = _fnGetColumns(oSettings, 'bSearchable');

      for (var i = 0, iLen = afnFilters.length; i < iLen; i++) {
        var iCorrector = 0;
        for (var j = 0, jLen = oSettings.aiDisplay.length; j < jLen; j++) {
          var iDisIndex = oSettings.aiDisplay[j - iCorrector];
          var bTest = afnFilters[i](
            oSettings, _fnGetRowData(oSettings, iDisIndex, 'filter', aiFilterColumns), iDisIndex);

          /* Check if we should use this row based on the filtering function */
          if (!bTest) {
            oSettings.aiDisplay.splice(j - iCorrector, 1);
            iCorrector++;
          }
        }
      }
    }


    /**
    * Filter the table on a per-column basis
    *  @param {object} oSettings dataTables settings object
    *  @param {string} sInput string to filter on
    *  @param {int} iColumn column to filter
    *  @param {bool} bRegex treat search string as a regular expression or not
    *  @param {bool} bSmart use smart filtering or not
    *  @param {bool} bCaseInsensitive Do case insenstive matching or not
    *  @memberof DataTable#oApi
    */
    function _fnFilterColumn(oSettings, sInput, iColumn, bRegex, bSmart, bCaseInsensitive) {
      if (sInput === "") {
        return;
      }

      var iIndexCorrector = 0;
      var rpSearch = _fnFilterCreateSearch(sInput, bRegex, bSmart, bCaseInsensitive);

      for (var i = oSettings.aiDisplay.length - 1; i >= 0; i--) {
        var sData = _fnDataToSearch(_fnGetCellData(oSettings, oSettings.aiDisplay[i], iColumn, 'filter'), oSettings.aoColumns[iColumn].sType);
        if (!rpSearch.test(sData)) {
          oSettings.aiDisplay.splice(i, 1);
          iIndexCorrector++;
        }
      }
    }


    /**
    * Filter the data table based on user input and draw the table
    *  @param {object} oSettings dataTables settings object
    *  @param {string} sInput string to filter on
    *  @param {int} iForce optional - force a research of the master array (1) or not (undefined or 0)
    *  @param {bool} bRegex treat as a regular expression or not
    *  @param {bool} bSmart perform smart filtering or not
    *  @param {bool} bCaseInsensitive Do case insenstive matching or not
    *  @memberof DataTable#oApi
    */
    function _fnFilter(oSettings, sInput, iForce, bRegex, bSmart, bCaseInsensitive) {
      var i;
      var rpSearch = _fnFilterCreateSearch(sInput, bRegex, bSmart, bCaseInsensitive);
      var oPrevSearch = oSettings.oPreviousSearch;

      /* Check if we are forcing or not - optional parameter */
      if (!iForce) {
        iForce = 0;
      }

      /* Need to take account of custom filtering functions - always filter */
      if (DataTable.ext.afnFiltering.length !== 0) {
        iForce = 1;
      }

      /*
      * If the input is blank - we want the full data set
      */
      if (sInput.length <= 0) {
        oSettings.aiDisplay.splice(0, oSettings.aiDisplay.length);
        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
      } else {
        /*
        * We are starting a new search or the new search string is smaller 
        * then the old one (i.e. delete). Search from the master array
        */
        if (oSettings.aiDisplay.length == oSettings.aiDisplayMaster.length || oPrevSearch.sSearch.length > sInput.length || iForce == 1 || sInput.indexOf(oPrevSearch.sSearch) !== 0) {
          /* Nuke the old display array - we are going to rebuild it */
          oSettings.aiDisplay.splice(0, oSettings.aiDisplay.length);

          /* Force a rebuild of the search array */
          _fnBuildSearchArray(oSettings, 1);

          /* Search through all records to populate the search array
          * The the oSettings.aiDisplayMaster and asDataSearch arrays have 1 to 1 
          * mapping
          */
          for (i = 0; i < oSettings.aiDisplayMaster.length; i++) {
            if (rpSearch.test(oSettings.asDataSearch[i])) {
              oSettings.aiDisplay.push(oSettings.aiDisplayMaster[i]);
            }
          }
        } else {
          /* Using old search array - refine it - do it this way for speed
          * Don't have to search the whole master array again
          */
          var iIndexCorrector = 0;

          /* Search the current results */
          for (i = 0; i < oSettings.asDataSearch.length; i++) {
            if (!rpSearch.test(oSettings.asDataSearch[i])) {
              oSettings.aiDisplay.splice(i - iIndexCorrector, 1);
              iIndexCorrector++;
            }
          }
        }
      }
    }


    /**
    * Create an array which can be quickly search through
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iMaster use the master data array - optional
    *  @memberof DataTable#oApi
    */
    function _fnBuildSearchArray(oSettings, iMaster) {
      if (!oSettings.oFeatures.bServerSide) {
        /* Clear out the old data */
        oSettings.asDataSearch = [];

        var aiFilterColumns = _fnGetColumns(oSettings, 'bSearchable');
        var aiIndex = (iMaster === 1) ? oSettings.aiDisplayMaster : oSettings.aiDisplay;

        for (var i = 0, iLen = aiIndex.length; i < iLen; i++) {
          oSettings.asDataSearch[i] = _fnBuildSearchRow(
            oSettings, _fnGetRowData(oSettings, aiIndex[i], 'filter', aiFilterColumns));
        }
      }
    }


    /**
    * Create a searchable string from a single data row
    *  @param {object} oSettings dataTables settings object
    *  @param {array} aData Row data array to use for the data to search
    *  @memberof DataTable#oApi
    */
    function _fnBuildSearchRow(oSettings, aData) {
      var sSearch = aData.join('  ');

      /* If it looks like there is an HTML entity in the string, attempt to decode it */
      if (sSearch.indexOf('&') !== -1) {
        sSearch = $('<div>').html(sSearch).text();
      }

      // Strip newline characters
      return sSearch.replace(/[\n\r]/g, " ");
    }

    /**
    * Build a regular expression object suitable for searching a table
    *  @param {string} sSearch string to search for
    *  @param {bool} bRegex treat as a regular expression or not
    *  @param {bool} bSmart perform smart filtering or not
    *  @param {bool} bCaseInsensitive Do case insensitive matching or not
    *  @returns {RegExp} constructed object
    *  @memberof DataTable#oApi
    */
    function _fnFilterCreateSearch(sSearch, bRegex, bSmart, bCaseInsensitive) {
      var asSearch, sRegExpString;

      if (bSmart) {
        /* Generate the regular expression to use. Something along the lines of:
        * ^(?=.*?\bone\b)(?=.*?\btwo\b)(?=.*?\bthree\b).*$
        */
        asSearch = bRegex ? sSearch.split(' ') : _fnEscapeRegex(sSearch).split(' ');
        sRegExpString = '^(?=.*?' + asSearch.join(')(?=.*?') + ').*$';
        return new RegExp(sRegExpString, bCaseInsensitive ? "i" : "");
      } else {
        sSearch = bRegex ? sSearch : _fnEscapeRegex(sSearch);
        return new RegExp(sSearch, bCaseInsensitive ? "i" : "");
      }
    }


    /**
    * Convert raw data into something that the user can search on
    *  @param {string} sData data to be modified
    *  @param {string} sType data type
    *  @returns {string} search string
    *  @memberof DataTable#oApi
    */
    function _fnDataToSearch(sData, sType) {
      if (typeof DataTable.ext.ofnSearch[sType] === "function") {
        return DataTable.ext.ofnSearch[sType](sData);
      } else if (sData === null) {
        return '';
      } else if (sType == "html") {
        return sData.replace(/[\r\n]/g, " ").replace(/<.*?>/g, "");
      } else if (typeof sData === "string") {
        return sData.replace(/[\r\n]/g, " ");
      }
      return sData;
    }


    /**
    * scape a string such that it can be used in a regular expression
    *  @param {string} sVal string to escape
    *  @returns {string} escaped string
    *  @memberof DataTable#oApi
    */
    function _fnEscapeRegex(sVal) {
      var acEscape = ['/', '.', '*', '+', '?', '|', '(', ')', '[', ']', '{', '}', '\\', '$', '^', '-'];
      var reReplace = new RegExp('(\\' + acEscape.join('|\\') + ')', 'g');
      return sVal.replace(reReplace, '\\$1');
    }



    /**
    * Generate the node required for the info display
    *  @param {object} oSettings dataTables settings object
    *  @returns {node} Information element
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlInfo(oSettings) {
      var nInfo = document.createElement('div');
      nInfo.className = oSettings.oClasses.sInfo;

      /* Actions that are to be taken once only for this feature */
      if (!oSettings.aanFeatures.i) {
        /* Add draw callback */
        oSettings.aoDrawCallback.push({
          "fn": _fnUpdateInfo,
          "sName": "information"
        });

        /* Add id */
        nInfo.id = oSettings.sTableId + '_info';
      }
      oSettings.nTable.setAttribute('aria-describedby', oSettings.sTableId + '_info');

      return nInfo;
    }


    /**
    * Update the information elements in the display
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnUpdateInfo(oSettings) {
      /* Show information about the table */
      if (!oSettings.oFeatures.bInfo || oSettings.aanFeatures.i.length === 0) {
        return;
      }

      var 
        oLang = oSettings.oLanguage,
          iStart = oSettings._iDisplayStart + 1,
          iEnd = oSettings.fnDisplayEnd(),
          iMax = oSettings.fnRecordsTotal(),
          iTotal = oSettings.fnRecordsDisplay(),
          sOut;

      if (iTotal === 0 && iTotal == iMax) {
        /* Empty record set */
        sOut = oLang.sInfoEmpty;
      } else if (iTotal === 0) {
        /* Empty record set after filtering */
        sOut = oLang.sInfoEmpty + ' ' + oLang.sInfoFiltered;
      } else if (iTotal == iMax) {
        /* Normal record set */
        sOut = oLang.sInfo;
      } else {
        /* Record set after filtering */
        sOut = oLang.sInfo + ' ' + oLang.sInfoFiltered;
      }

      // Convert the macros
      sOut += oLang.sInfoPostFix;
      sOut = _fnInfoMacros(oSettings, sOut);

      if (oLang.fnInfoCallback !== null) {
        sOut = oLang.fnInfoCallback.call(oSettings.oInstance, oSettings, iStart, iEnd, iMax, iTotal, sOut);
      }

      var n = oSettings.aanFeatures.i;
      for (var i = 0, iLen = n.length; i < iLen; i++) {
        $(n[i]).html(sOut);
      }
    }


    function _fnInfoMacros(oSettings, str) {
      var 
        iStart = oSettings._iDisplayStart + 1,
          sStart = oSettings.fnFormatNumber(iStart),
          iEnd = oSettings.fnDisplayEnd(),
          sEnd = oSettings.fnFormatNumber(iEnd),
          iTotal = oSettings.fnRecordsDisplay(),
          sTotal = oSettings.fnFormatNumber(iTotal),
          iMax = oSettings.fnRecordsTotal(),
          sMax = oSettings.fnFormatNumber(iMax);

      // When infinite scrolling, we are always starting at 1. _iDisplayStart is used only
      // internally
      if (oSettings.oScroll.bInfinite) {
        sStart = oSettings.fnFormatNumber(1);
      }

      return str.
        replace('_START_', sStart).
        replace('_END_', sEnd).
        replace('_TOTAL_', sTotal).
        replace('_MAX_', sMax);
    }



    /**
    * Draw the table for the first time, adding all required features
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnInitialise(oSettings) {
      var i, iLen, iAjaxStart = oSettings.iInitDisplayStart;

      /* Ensure that the table data is fully initialised */
      if (oSettings.bInitialised === false) {
        setTimeout(function () {
          _fnInitialise(oSettings);
        }, 200);
        return;
      }

      /* Show the display HTML options */
      _fnAddOptionsHtml(oSettings);

      /* Build and draw the header / footer for the table */
      _fnBuildHead(oSettings);
      _fnDrawHead(oSettings, oSettings.aoHeader);
      if (oSettings.nTFoot) {
        _fnDrawHead(oSettings, oSettings.aoFooter);
      }

      /* Okay to show that something is going on now */
      _fnProcessingDisplay(oSettings, true);

      /* Calculate sizes for columns */
      if (oSettings.oFeatures.bAutoWidth) {
        _fnCalculateColumnWidths(oSettings);
      }

      for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        if (oSettings.aoColumns[i].sWidth !== null) {
          oSettings.aoColumns[i].nTh.style.width = _fnStringToCss(oSettings.aoColumns[i].sWidth);
        }
      }

      /* If there is default sorting required - let's do it. The sort function will do the
      * drawing for us. Otherwise we draw the table regardless of the Ajax source - this allows
      * the table to look initialised for Ajax sourcing data (show 'loading' message possibly)
      */
      if (oSettings.oFeatures.bSort) {
        _fnSort(oSettings);
      } else if (oSettings.oFeatures.bFilter) {
        _fnFilterComplete(oSettings, oSettings.oPreviousSearch);
      } else {
        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      }

      /* if there is an ajax source load the data */
      if (oSettings.sAjaxSource !== null && !oSettings.oFeatures.bServerSide) {
        var aoData = [];
        _fnServerParams(oSettings, aoData);
        oSettings.fnServerData.call(oSettings.oInstance, oSettings.sAjaxSource, aoData, function (json) {
          var aData = (oSettings.sAjaxDataProp !== "") ? _fnGetObjectDataFn(oSettings.sAjaxDataProp)(json) : json;

          /* Got the data - add it to the table */
          for (i = 0; i < aData.length; i++) {
            _fnAddData(oSettings, aData[i]);
          }

          /* Reset the init display for cookie saving. We've already done a filter, and
          * therefore cleared it before. So we need to make it appear 'fresh'
          */
          oSettings.iInitDisplayStart = iAjaxStart;

          if (oSettings.oFeatures.bSort) {
            _fnSort(oSettings);
          } else {
            oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
            _fnCalculateEnd(oSettings);
            _fnDraw(oSettings);
          }

          _fnProcessingDisplay(oSettings, false);
          _fnInitComplete(oSettings, json);
        }, oSettings);
        return;
      }

      /* Server-side processing initialisation complete is done at the end of _fnDraw */
      if (!oSettings.oFeatures.bServerSide) {
        _fnProcessingDisplay(oSettings, false);
        _fnInitComplete(oSettings);
      }
    }


    /**
    * Draw the table for the first time, adding all required features
    *  @param {object} oSettings dataTables settings object
    *  @param {object} [json] JSON from the server that completed the table, if using Ajax source
    *    with client-side processing (optional)
    *  @memberof DataTable#oApi
    */
    function _fnInitComplete(oSettings, json) {
      oSettings._bInitComplete = true;
      _fnCallbackFire(oSettings, 'aoInitComplete', 'init', [oSettings, json]);
    }


    /**
    * Language compatibility - when certain options are given, and others aren't, we
    * need to duplicate the values over, in order to provide backwards compatibility
    * with older language files.
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnLanguageCompat(oLanguage) {
      var oDefaults = DataTable.defaults.oLanguage;

      /* Backwards compatibility - if there is no sEmptyTable given, then use the same as
      * sZeroRecords - assuming that is given.
      */
      if (!oLanguage.sEmptyTable && oLanguage.sZeroRecords && oDefaults.sEmptyTable === "No data available in table") {
        _fnMap(oLanguage, oLanguage, 'sZeroRecords', 'sEmptyTable');
      }

      /* Likewise with loading records */
      if (!oLanguage.sLoadingRecords && oLanguage.sZeroRecords && oDefaults.sLoadingRecords === "Loading...") {
        _fnMap(oLanguage, oLanguage, 'sZeroRecords', 'sLoadingRecords');
      }
    }



    /**
    * Generate the node required for user display length changing
    *  @param {object} oSettings dataTables settings object
    *  @returns {node} Display length feature node
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlLength(oSettings) {
      if (oSettings.oScroll.bInfinite) {
        return null;
      }

      /* This can be overruled by not using the _MENU_ var/macro in the language variable */
      var sName = 'name="' + oSettings.sTableId + '_length"';
      var sStdMenu = '<select size="1" ' + sName + '>';
      var i, iLen;
      var aLengthMenu = oSettings.aLengthMenu;

      if (aLengthMenu.length == 2 && typeof aLengthMenu[0] === 'object' && typeof aLengthMenu[1] === 'object') {
        for (i = 0, iLen = aLengthMenu[0].length; i < iLen; i++) {
          sStdMenu += '<option value="' + aLengthMenu[0][i] + '">' + aLengthMenu[1][i] + '</option>';
        }
      } else {
        for (i = 0, iLen = aLengthMenu.length; i < iLen; i++) {
          sStdMenu += '<option value="' + aLengthMenu[i] + '">' + aLengthMenu[i] + '</option>';
        }
      }
      sStdMenu += '</select>';

      var nLength = document.createElement('div');
      if (!oSettings.aanFeatures.l) {
        nLength.id = oSettings.sTableId + '_length';
      }
      nLength.className = oSettings.oClasses.sLength;
      nLength.innerHTML = '<label>' + oSettings.oLanguage.sLengthMenu.replace('_MENU_', sStdMenu) + '</label>';

      /*
      * Set the length to the current display length - thanks to Andrea Pavlovic for this fix,
      * and Stefan Skopnik for fixing the fix!
      */
      $('select option[value="' + oSettings._iDisplayLength + '"]', nLength).attr("selected", true);

      $('select', nLength).bind('change.DT', function (e) {
        var iVal = $(this).val();

        /* Update all other length options for the new display */
        var n = oSettings.aanFeatures.l;
        for (i = 0, iLen = n.length; i < iLen; i++) {
          if (n[i] != this.parentNode) {
            $('select', n[i]).val(iVal);
          }
        }

        /* Redraw the table */
        oSettings._iDisplayLength = parseInt(iVal, 10);
        _fnCalculateEnd(oSettings);

        /* If we have space to show extra rows (backing up from the end point - then do so */
        if (oSettings.fnDisplayEnd() == oSettings.fnRecordsDisplay()) {
          oSettings._iDisplayStart = oSettings.fnDisplayEnd() - oSettings._iDisplayLength;
          if (oSettings._iDisplayStart < 0) {
            oSettings._iDisplayStart = 0;
          }
        }

        if (oSettings._iDisplayLength == -1) {
          oSettings._iDisplayStart = 0;
        }

        _fnDraw(oSettings);
      });


      $('select', nLength).attr('aria-controls', oSettings.sTableId);

      return nLength;
    }


    /**
    * Recalculate the end point based on the start point
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnCalculateEnd(oSettings) {
      if (oSettings.oFeatures.bPaginate === false) {
        oSettings._iDisplayEnd = oSettings.aiDisplay.length;
      } else {
        /* Set the end point of the display - based on how many elements there are
        * still to display
        */
        if (oSettings._iDisplayStart + oSettings._iDisplayLength > oSettings.aiDisplay.length || oSettings._iDisplayLength == -1) {
          oSettings._iDisplayEnd = oSettings.aiDisplay.length;
        } else {
          oSettings._iDisplayEnd = oSettings._iDisplayStart + oSettings._iDisplayLength;
        }
      }
    }



    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * Note that most of the paging logic is done in 
    * DataTable.ext.oPagination
    */

    /**
    * Generate the node required for default pagination
    *  @param {object} oSettings dataTables settings object
    *  @returns {node} Pagination feature node
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlPaginate(oSettings) {
      if (oSettings.oScroll.bInfinite) {
        return null;
      }

      var nPaginate = document.createElement('div');
      nPaginate.className = oSettings.oClasses.sPaging + oSettings.sPaginationType;

      DataTable.ext.oPagination[oSettings.sPaginationType].fnInit(oSettings, nPaginate, function (oSettings) {
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      });

      /* Add a draw callback for the pagination on first instance, to update the paging display */
      if (!oSettings.aanFeatures.p) {
        oSettings.aoDrawCallback.push({
          "fn": function (oSettings) {
            DataTable.ext.oPagination[oSettings.sPaginationType].fnUpdate(oSettings, function (oSettings) {
              _fnCalculateEnd(oSettings);
              _fnDraw(oSettings);
            });
          },
          "sName": "pagination"
        });
      }
      return nPaginate;
    }


    /**
    * Alter the display settings to change the page
    *  @param {object} oSettings dataTables settings object
    *  @param {string|int} mAction Paging action to take: "first", "previous", "next" or "last"
    *    or page number to jump to (integer)
    *  @returns {bool} true page has changed, false - no change (no effect) eg 'first' on page 1
    *  @memberof DataTable#oApi
    */
    function _fnPageChange(oSettings, mAction) {
      var iOldStart = oSettings._iDisplayStart;

      if (typeof mAction === "number") {
        oSettings._iDisplayStart = mAction * oSettings._iDisplayLength;
        if (oSettings._iDisplayStart > oSettings.fnRecordsDisplay()) {
          oSettings._iDisplayStart = 0;
        }
      } else if (mAction == "first") {
        oSettings._iDisplayStart = 0;
      } else if (mAction == "previous") {
        oSettings._iDisplayStart = oSettings._iDisplayLength >= 0 ? oSettings._iDisplayStart - oSettings._iDisplayLength : 0;

        /* Correct for under-run */
        if (oSettings._iDisplayStart < 0) {
          oSettings._iDisplayStart = 0;
        }
      } else if (mAction == "next") {
        if (oSettings._iDisplayLength >= 0) {
          /* Make sure we are not over running the display array */
          if (oSettings._iDisplayStart + oSettings._iDisplayLength < oSettings.fnRecordsDisplay()) {
            oSettings._iDisplayStart += oSettings._iDisplayLength;
          }
        } else {
          oSettings._iDisplayStart = 0;
        }
      } else if (mAction == "last") {
        if (oSettings._iDisplayLength >= 0) {
          var iPages = parseInt((oSettings.fnRecordsDisplay() - 1) / oSettings._iDisplayLength, 10) + 1;
          oSettings._iDisplayStart = (iPages - 1) * oSettings._iDisplayLength;
        } else {
          oSettings._iDisplayStart = 0;
        }
      } else {
        _fnLog(oSettings, 0, "Unknown paging action: " + mAction);
      }
      $(oSettings.oInstance).trigger('page', oSettings);

      return iOldStart != oSettings._iDisplayStart;
    }



    /**
    * Generate the node required for the processing node
    *  @param {object} oSettings dataTables settings object
    *  @returns {node} Processing element
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlProcessing(oSettings) {
      var nProcessing = document.createElement('div');

      if (!oSettings.aanFeatures.r) {
        nProcessing.id = oSettings.sTableId + '_processing';
      }
      nProcessing.innerHTML = oSettings.oLanguage.sProcessing;
      nProcessing.className = oSettings.oClasses.sProcessing;
      oSettings.nTable.parentNode.insertBefore(nProcessing, oSettings.nTable);

      return nProcessing;
    }


    /**
    * Display or hide the processing indicator
    *  @param {object} oSettings dataTables settings object
    *  @param {bool} bShow Show the processing indicator (true) or not (false)
    *  @memberof DataTable#oApi
    */
    function _fnProcessingDisplay(oSettings, bShow) {
      if (oSettings.oFeatures.bProcessing) {
        var an = oSettings.aanFeatures.r;
        for (var i = 0, iLen = an.length; i < iLen; i++) {
          an[i].style.visibility = bShow ? "visible" : "hidden";
        }
      }

      $(oSettings.oInstance).trigger('processing', [oSettings, bShow]);
    }



    /**
    * Add any control elements for the table - specifically scrolling
    *  @param {object} oSettings dataTables settings object
    *  @returns {node} Node to add to the DOM
    *  @memberof DataTable#oApi
    */
    function _fnFeatureHtmlTable(oSettings) {
      /* Check if scrolling is enabled or not - if not then leave the DOM unaltered */
      if (oSettings.oScroll.sX === "" && oSettings.oScroll.sY === "") {
        return oSettings.nTable;
      }

      /*
      * The HTML structure that we want to generate in this function is:
      *  div - nScroller
      *    div - nScrollHead
      *      div - nScrollHeadInner
      *        table - nScrollHeadTable
      *          thead - nThead
      *    div - nScrollBody
      *      table - oSettings.nTable
      *        thead - nTheadSize
      *        tbody - nTbody
      *    div - nScrollFoot
      *      div - nScrollFootInner
      *        table - nScrollFootTable
      *          tfoot - nTfoot
      */
      var 
        nScroller = document.createElement('div'),
          nScrollHead = document.createElement('div'),
          nScrollHeadInner = document.createElement('div'),
          nScrollBody = document.createElement('div'),
          nScrollFoot = document.createElement('div'),
          nScrollFootInner = document.createElement('div'),
          nScrollHeadTable = oSettings.nTable.cloneNode(false),
          nScrollFootTable = oSettings.nTable.cloneNode(false),
          nThead = oSettings.nTable.getElementsByTagName('thead')[0],
          nTfoot = oSettings.nTable.getElementsByTagName('tfoot').length === 0 ? null : oSettings.nTable.getElementsByTagName('tfoot')[0],
          oClasses = oSettings.oClasses;

      nScrollHead.appendChild(nScrollHeadInner);
      nScrollFoot.appendChild(nScrollFootInner);
      nScrollBody.appendChild(oSettings.nTable);
      nScroller.appendChild(nScrollHead);
      nScroller.appendChild(nScrollBody);
      nScrollHeadInner.appendChild(nScrollHeadTable);
      nScrollHeadTable.appendChild(nThead);
      if (nTfoot !== null) {
        nScroller.appendChild(nScrollFoot);
        nScrollFootInner.appendChild(nScrollFootTable);
        nScrollFootTable.appendChild(nTfoot);
      }

      nScroller.className = oClasses.sScrollWrapper;
      nScrollHead.className = oClasses.sScrollHead;
      nScrollHeadInner.className = oClasses.sScrollHeadInner;
      nScrollBody.className = oClasses.sScrollBody;
      nScrollFoot.className = oClasses.sScrollFoot;
      nScrollFootInner.className = oClasses.sScrollFootInner;

      if (oSettings.oScroll.bAutoCss) {
        nScrollHead.style.overflow = "hidden";
        nScrollHead.style.position = "relative";
        nScrollFoot.style.overflow = "hidden";
        nScrollBody.style.overflow = "auto";
      }

      nScrollHead.style.border = "0";
      nScrollHead.style.width = "100%";
      nScrollFoot.style.border = "0";
      nScrollHeadInner.style.width = oSettings.oScroll.sXInner !== "" ? oSettings.oScroll.sXInner : "100%"; /* will be overwritten */

      /* Modify attributes to respect the clones */
      nScrollHeadTable.removeAttribute('id');
      nScrollHeadTable.style.marginLeft = "0";
      oSettings.nTable.style.marginLeft = "0";
      if (nTfoot !== null) {
        nScrollFootTable.removeAttribute('id');
        nScrollFootTable.style.marginLeft = "0";
      }

      /* Move caption elements from the body to the header, footer or leave where it is
      * depending on the configuration. Note that the DTD says there can be only one caption */
      var nCaption = $(oSettings.nTable).children('caption');
      if (nCaption.length > 0) {
        nCaption = nCaption[0];
        if (nCaption._captionSide === "top") {
          nScrollHeadTable.appendChild(nCaption);
        } else if (nCaption._captionSide === "bottom" && nTfoot) {
          nScrollFootTable.appendChild(nCaption);
        }
      }

      /*
      * Sizing
      */
      /* When x-scrolling add the width and a scroller to move the header with the body */
      if (oSettings.oScroll.sX !== "") {
        nScrollHead.style.width = _fnStringToCss(oSettings.oScroll.sX);
        nScrollBody.style.width = _fnStringToCss(oSettings.oScroll.sX);

        if (nTfoot !== null) {
          nScrollFoot.style.width = _fnStringToCss(oSettings.oScroll.sX);
        }

        /* When the body is scrolled, then we also want to scroll the headers */
        $(nScrollBody).scroll(function (e) {
          nScrollHead.scrollLeft = this.scrollLeft;

          if (nTfoot !== null) {
            nScrollFoot.scrollLeft = this.scrollLeft;
          }
        });
      }

      /* When yscrolling, add the height */
      if (oSettings.oScroll.sY !== "") {
        nScrollBody.style.height = _fnStringToCss(oSettings.oScroll.sY);
      }

      /* Redraw - align columns across the tables */
      oSettings.aoDrawCallback.push({
        "fn": _fnScrollDraw,
        "sName": "scrolling"
      });

      /* Infinite scrolling event handlers */
      if (oSettings.oScroll.bInfinite) {
        $(nScrollBody).scroll(function () {
          /* Use a blocker to stop scrolling from loading more data while other data is still loading */
          if (!oSettings.bDrawing && $(this).scrollTop() !== 0) {
            /* Check if we should load the next data set */
            if ($(this).scrollTop() + $(this).height() > $(oSettings.nTable).height() - oSettings.oScroll.iLoadGap) {
              /* Only do the redraw if we have to - we might be at the end of the data */
              if (oSettings.fnDisplayEnd() < oSettings.fnRecordsDisplay()) {
                _fnPageChange(oSettings, 'next');
                _fnCalculateEnd(oSettings);
                _fnDraw(oSettings);
              }
            }
          }
        });
      }

      oSettings.nScrollHead = nScrollHead;
      oSettings.nScrollFoot = nScrollFoot;

      return nScroller;
    }


    /**
    * Update the various tables for resizing. It's a bit of a pig this function, but
    * basically the idea to:
    *   1. Re-create the table inside the scrolling div
    *   2. Take live measurements from the DOM
    *   3. Apply the measurements
    *   4. Clean up
    *  @param {object} o dataTables settings object
    *  @returns {node} Node to add to the DOM
    *  @memberof DataTable#oApi
    */
    function _fnScrollDraw(o) {
      var 
        nScrollHeadInner = o.nScrollHead.getElementsByTagName('div')[0],
          nScrollHeadTable = nScrollHeadInner.getElementsByTagName('table')[0],
          nScrollBody = o.nTable.parentNode,
          i, iLen, j, jLen, anHeadToSize, anHeadSizers, anFootSizers, anFootToSize, oStyle, iVis, nTheadSize, nTfootSize, iWidth, aApplied = [],
          iSanityWidth, nScrollFootInner = (o.nTFoot !== null) ? o.nScrollFoot.getElementsByTagName('div')[0] : null,
          nScrollFootTable = (o.nTFoot !== null) ? nScrollFootInner.getElementsByTagName('table')[0] : null,
          ie67 = o.oBrowser.bScrollOversize;

      /*
      * 1. Re-create the table inside the scrolling div
      */

      /* Remove the old minimised thead and tfoot elements in the inner table */
      $(o.nTable).children('thead, tfoot').remove();

      /* Clone the current header and footer elements and then place it into the inner table */
      nTheadSize = $(o.nTHead).clone()[0];
      o.nTable.insertBefore(nTheadSize, o.nTable.childNodes[0]);

      if (o.nTFoot !== null) {
        nTfootSize = $(o.nTFoot).clone()[0];
        o.nTable.insertBefore(nTfootSize, o.nTable.childNodes[1]);
      }

      /*
      * 2. Take live measurements from the DOM - do not alter the DOM itself!
      */

      /* Remove old sizing and apply the calculated column widths
      * Get the unique column headers in the newly created (cloned) header. We want to apply the
      * calculated sizes to this header
      */
      if (o.oScroll.sX === "") {
        nScrollBody.style.width = '100%';
        nScrollHeadInner.parentNode.style.width = '100%';
      }

      var nThs = _fnGetUniqueThs(o, nTheadSize);
      for (i = 0, iLen = nThs.length; i < iLen; i++) {
        iVis = _fnVisibleToColumnIndex(o, i);
        nThs[i].style.width = o.aoColumns[iVis].sWidth;
      }

      if (o.nTFoot !== null) {
        _fnApplyToChildren(function (n) {
          n.style.width = "";
        }, nTfootSize.getElementsByTagName('tr'));
      }

      // If scroll collapse is enabled, when we put the headers back into the body for sizing, we
      // will end up forcing the scrollbar to appear, making our measurements wrong for when we
      // then hide it (end of this function), so add the header height to the body scroller.
      if (o.oScroll.bCollapse && o.oScroll.sY !== "") {
        nScrollBody.style.height = (nScrollBody.offsetHeight + o.nTHead.offsetHeight) + "px";
      }

      /* Size the table as a whole */
      iSanityWidth = $(o.nTable).outerWidth();
      if (o.oScroll.sX === "") {
        /* No x scrolling */
        o.nTable.style.width = "100%";

        /* I know this is rubbish - but IE7 will make the width of the table when 100% include
        * the scrollbar - which is shouldn't. When there is a scrollbar we need to take this
        * into account.
        */
        if (ie67 && ($('tbody', nScrollBody).height() > nScrollBody.offsetHeight || $(nScrollBody).css('overflow-y') == "scroll")) {
          o.nTable.style.width = _fnStringToCss($(o.nTable).outerWidth() - o.oScroll.iBarWidth);
        }
      } else {
        if (o.oScroll.sXInner !== "") {
          /* x scroll inner has been given - use it */
          o.nTable.style.width = _fnStringToCss(o.oScroll.sXInner);
        } else if (iSanityWidth == $(nScrollBody).width() && $(nScrollBody).height() < $(o.nTable).height()) {
          /* There is y-scrolling - try to take account of the y scroll bar */
          o.nTable.style.width = _fnStringToCss(iSanityWidth - o.oScroll.iBarWidth);
          if ($(o.nTable).outerWidth() > iSanityWidth - o.oScroll.iBarWidth) {
            /* Not possible to take account of it */
            o.nTable.style.width = _fnStringToCss(iSanityWidth);
          }
        } else {
          /* All else fails */
          o.nTable.style.width = _fnStringToCss(iSanityWidth);
        }
      }

      /* Recalculate the sanity width - now that we've applied the required width, before it was
      * a temporary variable. This is required because the column width calculation is done
      * before this table DOM is created.
      */
      iSanityWidth = $(o.nTable).outerWidth();

      /* We want the hidden header to have zero height, so remove padding and borders. Then
      * set the width based on the real headers
      */
      anHeadToSize = o.nTHead.getElementsByTagName('tr');
      anHeadSizers = nTheadSize.getElementsByTagName('tr');

      _fnApplyToChildren(function (nSizer, nToSize) {
        oStyle = nSizer.style;
        oStyle.paddingTop = "0";
        oStyle.paddingBottom = "0";
        oStyle.borderTopWidth = "0";
        oStyle.borderBottomWidth = "0";
        oStyle.height = 0;

        iWidth = $(nSizer).width();
        nToSize.style.width = _fnStringToCss(iWidth);
        aApplied.push(iWidth);
      }, anHeadSizers, anHeadToSize);
      $(anHeadSizers).height(0);

      if (o.nTFoot !== null) {
        /* Clone the current footer and then place it into the body table as a "hidden header" */
        anFootSizers = nTfootSize.getElementsByTagName('tr');
        anFootToSize = o.nTFoot.getElementsByTagName('tr');

        _fnApplyToChildren(function (nSizer, nToSize) {
          oStyle = nSizer.style;
          oStyle.paddingTop = "0";
          oStyle.paddingBottom = "0";
          oStyle.borderTopWidth = "0";
          oStyle.borderBottomWidth = "0";
          oStyle.height = 0;

          iWidth = $(nSizer).width();
          nToSize.style.width = _fnStringToCss(iWidth);
          aApplied.push(iWidth);
        }, anFootSizers, anFootToSize);
        $(anFootSizers).height(0);
      }

      /*
      * 3. Apply the measurements
      */

      /* "Hide" the header and footer that we used for the sizing. We want to also fix their width
      * to what they currently are
      */
      _fnApplyToChildren(function (nSizer) {
        nSizer.innerHTML = "";
        nSizer.style.width = _fnStringToCss(aApplied.shift());
      }, anHeadSizers);

      if (o.nTFoot !== null) {
        _fnApplyToChildren(function (nSizer) {
          nSizer.innerHTML = "";
          nSizer.style.width = _fnStringToCss(aApplied.shift());
        }, anFootSizers);
      }

      /* Sanity check that the table is of a sensible width. If not then we are going to get
      * misalignment - try to prevent this by not allowing the table to shrink below its min width
      */
      if ($(o.nTable).outerWidth() < iSanityWidth) {
        /* The min width depends upon if we have a vertical scrollbar visible or not */
        var iCorrection = ((nScrollBody.scrollHeight > nScrollBody.offsetHeight || $(nScrollBody).css('overflow-y') == "scroll")) ? iSanityWidth + o.oScroll.iBarWidth : iSanityWidth;

        /* IE6/7 are a law unto themselves... */
        if (ie67 && (nScrollBody.scrollHeight > nScrollBody.offsetHeight || $(nScrollBody).css('overflow-y') == "scroll")) {
          o.nTable.style.width = _fnStringToCss(iCorrection - o.oScroll.iBarWidth);
        }

        /* Apply the calculated minimum width to the table wrappers */
        nScrollBody.style.width = _fnStringToCss(iCorrection);
        nScrollHeadInner.parentNode.style.width = _fnStringToCss(iCorrection);

        if (o.nTFoot !== null) {
          nScrollFootInner.parentNode.style.width = _fnStringToCss(iCorrection);
        }

        /* And give the user a warning that we've stopped the table getting too small */
        if (o.oScroll.sX === "") {
          _fnLog(o, 1, "The table cannot fit into the current element which will cause column" + " misalignment. The table has been drawn at its minimum possible width.");
        } else if (o.oScroll.sXInner !== "") {
          _fnLog(o, 1, "The table cannot fit into the current element which will cause column" + " misalignment. Increase the sScrollXInner value or remove it to allow automatic" + " calculation");
        }
      } else {
        nScrollBody.style.width = _fnStringToCss('100%');
        nScrollHeadInner.parentNode.style.width = _fnStringToCss('100%');

        if (o.nTFoot !== null) {
          nScrollFootInner.parentNode.style.width = _fnStringToCss('100%');
        }
      }


      /*
      * 4. Clean up
      */
      if (o.oScroll.sY === "") {
        /* IE7< puts a vertical scrollbar in place (when it shouldn't be) due to subtracting
        * the scrollbar height from the visible display, rather than adding it on. We need to
        * set the height in order to sort this. Don't want to do it in any other browsers.
        */
        if (ie67) {
          nScrollBody.style.height = _fnStringToCss(o.nTable.offsetHeight + o.oScroll.iBarWidth);
        }
      }

      if (o.oScroll.sY !== "" && o.oScroll.bCollapse) {
        nScrollBody.style.height = _fnStringToCss(o.oScroll.sY);

        var iExtra = (o.oScroll.sX !== "" && o.nTable.offsetWidth > nScrollBody.offsetWidth) ? o.oScroll.iBarWidth : 0;
        if (o.nTable.offsetHeight < nScrollBody.offsetHeight) {
          nScrollBody.style.height = _fnStringToCss(o.nTable.offsetHeight + iExtra);
        }
      }

      /* Finally set the width's of the header and footer tables */
      var iOuterWidth = $(o.nTable).outerWidth();
      nScrollHeadTable.style.width = _fnStringToCss(iOuterWidth);
      nScrollHeadInner.style.width = _fnStringToCss(iOuterWidth);

      // Figure out if there are scrollbar present - if so then we need a the header and footer to
      // provide a bit more space to allow "overflow" scrolling (i.e. past the scrollbar)
      var bScrolling = $(o.nTable).height() > nScrollBody.clientHeight || $(nScrollBody).css('overflow-y') == "scroll";
      nScrollHeadInner.style.paddingRight = bScrolling ? o.oScroll.iBarWidth + "px" : "0px";

      if (o.nTFoot !== null) {
        nScrollFootTable.style.width = _fnStringToCss(iOuterWidth);
        nScrollFootInner.style.width = _fnStringToCss(iOuterWidth);
        nScrollFootInner.style.paddingRight = bScrolling ? o.oScroll.iBarWidth + "px" : "0px";
      }

      /* Adjust the position of the header in case we loose the y-scrollbar */
      $(nScrollBody).scroll();

      /* If sorting or filtering has occurred, jump the scrolling back to the top */
      if (o.bSorted || o.bFiltered) {
        nScrollBody.scrollTop = 0;
      }
    }


    /**
    * Apply a given function to the display child nodes of an element array (typically
    * TD children of TR rows
    *  @param {function} fn Method to apply to the objects
    *  @param array {nodes} an1 List of elements to look through for display children
    *  @param array {nodes} an2 Another list (identical structure to the first) - optional
    *  @memberof DataTable#oApi
    */
    function _fnApplyToChildren(fn, an1, an2) {
      for (var i = 0, iLen = an1.length; i < iLen; i++) {
        for (var j = 0, jLen = an1[i].childNodes.length; j < jLen; j++) {
          if (an1[i].childNodes[j].nodeType == 1) {
            if (an2) {
              fn(an1[i].childNodes[j], an2[i].childNodes[j]);
            } else {
              fn(an1[i].childNodes[j]);
            }
          }
        }
      }
    }



    /**
    * Convert a CSS unit width to pixels (e.g. 2em)
    *  @param {string} sWidth width to be converted
    *  @param {node} nParent parent to get the with for (required for relative widths) - optional
    *  @returns {int} iWidth width in pixels
    *  @memberof DataTable#oApi
    */
    function _fnConvertToWidth(sWidth, nParent) {
      if (!sWidth || sWidth === null || sWidth === '') {
        return 0;
      }

      if (!nParent) {
        nParent = document.getElementsByTagName('body')[0];
      }

      var iWidth;
      var nTmp = document.createElement("div");
      nTmp.style.width = _fnStringToCss(sWidth);

      nParent.appendChild(nTmp);
      iWidth = nTmp.offsetWidth;
      nParent.removeChild(nTmp);

      return (iWidth);
    }


    /**
    * Calculate the width of columns for the table
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnCalculateColumnWidths(oSettings) {
      var iTableWidth = oSettings.nTable.offsetWidth;
      var iUserInputs = 0;
      var iTmpWidth;
      var iVisibleColumns = 0;
      var iColums = oSettings.aoColumns.length;
      var i, iIndex, iCorrector, iWidth;
      var oHeaders = $('th', oSettings.nTHead);
      var widthAttr = oSettings.nTable.getAttribute('width');

      /* Convert any user input sizes into pixel sizes */
      for (i = 0; i < iColums; i++) {
        if (oSettings.aoColumns[i].bVisible) {
          iVisibleColumns++;

          if (oSettings.aoColumns[i].sWidth !== null) {
            iTmpWidth = _fnConvertToWidth(oSettings.aoColumns[i].sWidthOrig, oSettings.nTable.parentNode);
            if (iTmpWidth !== null) {
              oSettings.aoColumns[i].sWidth = _fnStringToCss(iTmpWidth);
            }

            iUserInputs++;
          }
        }
      }

      /* If the number of columns in the DOM equals the number that we have to process in 
      * DataTables, then we can use the offsets that are created by the web-browser. No custom 
      * sizes can be set in order for this to happen, nor scrolling used
      */
      if (iColums == oHeaders.length && iUserInputs === 0 && iVisibleColumns == iColums && oSettings.oScroll.sX === "" && oSettings.oScroll.sY === "") {
        for (i = 0; i < oSettings.aoColumns.length; i++) {
          iTmpWidth = $(oHeaders[i]).width();
          if (iTmpWidth !== null) {
            oSettings.aoColumns[i].sWidth = _fnStringToCss(iTmpWidth);
          }
        }
      } else {
        /* Otherwise we are going to have to do some calculations to get the width of each column.
        * Construct a 1 row table with the widest node in the data, and any user defined widths,
        * then insert it into the DOM and allow the browser to do all the hard work of
        * calculating table widths.
        */
        var 
          nCalcTmp = oSettings.nTable.cloneNode(false),
            nTheadClone = oSettings.nTHead.cloneNode(true),
            nBody = document.createElement('tbody'),
            nTr = document.createElement('tr'),
            nDivSizing;

        nCalcTmp.removeAttribute("id");
        nCalcTmp.appendChild(nTheadClone);
        if (oSettings.nTFoot !== null) {
          nCalcTmp.appendChild(oSettings.nTFoot.cloneNode(true));
          _fnApplyToChildren(function (n) {
            n.style.width = "";
          }, nCalcTmp.getElementsByTagName('tr'));
        }

        nCalcTmp.appendChild(nBody);
        nBody.appendChild(nTr);

        /* Remove any sizing that was previously applied by the styles */
        var jqColSizing = $('thead th', nCalcTmp);
        if (jqColSizing.length === 0) {
          jqColSizing = $('tbody tr:eq(0)>td', nCalcTmp);
        }

        /* Apply custom sizing to the cloned header */
        var nThs = _fnGetUniqueThs(oSettings, nTheadClone);
        iCorrector = 0;
        for (i = 0; i < iColums; i++) {
          var oColumn = oSettings.aoColumns[i];
          if (oColumn.bVisible && oColumn.sWidthOrig !== null && oColumn.sWidthOrig !== "") {
            nThs[i - iCorrector].style.width = _fnStringToCss(oColumn.sWidthOrig);
          } else if (oColumn.bVisible) {
            nThs[i - iCorrector].style.width = "";
          } else {
            iCorrector++;
          }
        }

        /* Find the biggest td for each column and put it into the table */
        for (i = 0; i < iColums; i++) {
          if (oSettings.aoColumns[i].bVisible) {
            var nTd = _fnGetWidestNode(oSettings, i);
            if (nTd !== null) {
              nTd = nTd.cloneNode(true);
              if (oSettings.aoColumns[i].sContentPadding !== "") {
                nTd.innerHTML += oSettings.aoColumns[i].sContentPadding;
              }
              nTr.appendChild(nTd);
            }
          }
        }

        /* Build the table and 'display' it */
        var nWrapper = oSettings.nTable.parentNode;
        nWrapper.appendChild(nCalcTmp);

        /* When scrolling (X or Y) we want to set the width of the table as appropriate. However,
        * when not scrolling leave the table width as it is. This results in slightly different,
        * but I think correct behaviour
        */
        if (oSettings.oScroll.sX !== "" && oSettings.oScroll.sXInner !== "") {
          nCalcTmp.style.width = _fnStringToCss(oSettings.oScroll.sXInner);
        } else if (oSettings.oScroll.sX !== "") {
          nCalcTmp.style.width = "";
          if ($(nCalcTmp).width() < nWrapper.offsetWidth) {
            nCalcTmp.style.width = _fnStringToCss(nWrapper.offsetWidth);
          }
        } else if (oSettings.oScroll.sY !== "") {
          nCalcTmp.style.width = _fnStringToCss(nWrapper.offsetWidth);
        } else if (widthAttr) {
          nCalcTmp.style.width = _fnStringToCss(widthAttr);
        }
        nCalcTmp.style.visibility = "hidden";

        /* Scrolling considerations */
        _fnScrollingWidthAdjust(oSettings, nCalcTmp);

        /* Read the width's calculated by the browser and store them for use by the caller. We
        * first of all try to use the elements in the body, but it is possible that there are
        * no elements there, under which circumstances we use the header elements
        */
        var oNodes = $("tbody tr:eq(0)", nCalcTmp).children();
        if (oNodes.length === 0) {
          oNodes = _fnGetUniqueThs(oSettings, $('thead', nCalcTmp)[0]);
        }

        /* Browsers need a bit of a hand when a width is assigned to any columns when 
        * x-scrolling as they tend to collapse the table to the min-width, even if
        * we sent the column widths. So we need to keep track of what the table width
        * should be by summing the user given values, and the automatic values
        */
        if (oSettings.oScroll.sX !== "") {
          var iTotal = 0;
          iCorrector = 0;
          for (i = 0; i < oSettings.aoColumns.length; i++) {
            if (oSettings.aoColumns[i].bVisible) {
              if (oSettings.aoColumns[i].sWidthOrig === null) {
                iTotal += $(oNodes[iCorrector]).outerWidth();
              } else {
                iTotal += parseInt(oSettings.aoColumns[i].sWidth.replace('px', ''), 10) + ($(oNodes[iCorrector]).outerWidth() - $(oNodes[iCorrector]).width());
              }
              iCorrector++;
            }
          }

          nCalcTmp.style.width = _fnStringToCss(iTotal);
          oSettings.nTable.style.width = _fnStringToCss(iTotal);
        }

        iCorrector = 0;
        for (i = 0; i < oSettings.aoColumns.length; i++) {
          if (oSettings.aoColumns[i].bVisible) {
            iWidth = $(oNodes[iCorrector]).width();
            if (iWidth !== null && iWidth > 0) {
              oSettings.aoColumns[i].sWidth = _fnStringToCss(iWidth);
            }
            iCorrector++;
          }
        }

        var cssWidth = $(nCalcTmp).css('width');
        oSettings.nTable.style.width = (cssWidth.indexOf('%') !== -1) ? cssWidth : _fnStringToCss($(nCalcTmp).outerWidth());
        nCalcTmp.parentNode.removeChild(nCalcTmp);
      }

      if (widthAttr) {
        oSettings.nTable.style.width = _fnStringToCss(widthAttr);
      }
    }


    /**
    * Adjust a table's width to take account of scrolling
    *  @param {object} oSettings dataTables settings object
    *  @param {node} n table node
    *  @memberof DataTable#oApi
    */
    function _fnScrollingWidthAdjust(oSettings, n) {
      if (oSettings.oScroll.sX === "" && oSettings.oScroll.sY !== "") {
        /* When y-scrolling only, we want to remove the width of the scroll bar so the table
        * + scroll bar will fit into the area avaialble.
        */
        var iOrigWidth = $(n).width();
        n.style.width = _fnStringToCss($(n).outerWidth() - oSettings.oScroll.iBarWidth);
      } else if (oSettings.oScroll.sX !== "") {
        /* When x-scrolling both ways, fix the table at it's current size, without adjusting */
        n.style.width = _fnStringToCss($(n).outerWidth());
      }
    }


    /**
    * Get the widest node
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iCol column of interest
    *  @returns {string} max string length for each column
    *  @memberof DataTable#oApi
    */
    function _fnGetWidestNode(oSettings, iCol) {
      var iMaxIndex = _fnGetMaxLenString(oSettings, iCol);
      if (iMaxIndex < 0) {
        return null;
      }

      if (oSettings.aoData[iMaxIndex].nTr === null) {
        var n = document.createElement('td');
        n.innerHTML = _fnGetCellData(oSettings, iMaxIndex, iCol, '');
        return n;
      }
      return _fnGetTdNodes(oSettings, iMaxIndex)[iCol];
    }


    /**
    * Get the maximum strlen for each data column
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iCol column of interest
    *  @returns {string} max string length for each column
    *  @memberof DataTable#oApi
    */
    function _fnGetMaxLenString(oSettings, iCol) {
      var iMax = -1;
      var iMaxIndex = -1;

      for (var i = 0; i < oSettings.aoData.length; i++) {
        var s = _fnGetCellData(oSettings, i, iCol, 'display') + "";
        s = s.replace(/<.*?>/g, "");
        if (s.length > iMax) {
          iMax = s.length;
          iMaxIndex = i;
        }
      }

      return iMaxIndex;
    }


    /**
    * Append a CSS unit (only if required) to a string
    *  @param {array} aArray1 first array
    *  @param {array} aArray2 second array
    *  @returns {int} 0 if match, 1 if length is different, 2 if no match
    *  @memberof DataTable#oApi
    */
    function _fnStringToCss(s) {
      if (s === null) {
        return "0px";
      }

      if (typeof s == 'number') {
        if (s < 0) {
          return "0px";
        }
        return s + "px";
      }

      /* Check if the last character is not 0-9 */
      var c = s.charCodeAt(s.length - 1);
      if (c < 0x30 || c > 0x39) {
        return s;
      }
      return s + "px";
    }


    /**
    * Get the width of a scroll bar in this browser being used
    *  @returns {int} width in pixels
    *  @memberof DataTable#oApi
    */
    function _fnScrollBarWidth() {
      var inner = document.createElement('p');
      var style = inner.style;
      style.width = "100%";
      style.height = "200px";
      style.padding = "0px";

      var outer = document.createElement('div');
      style = outer.style;
      style.position = "absolute";
      style.top = "0px";
      style.left = "0px";
      style.visibility = "hidden";
      style.width = "200px";
      style.height = "150px";
      style.padding = "0px";
      style.overflow = "hidden";
      outer.appendChild(inner);

      document.body.appendChild(outer);
      var w1 = inner.offsetWidth;
      outer.style.overflow = 'scroll';
      var w2 = inner.offsetWidth;
      if (w1 == w2) {
        w2 = outer.clientWidth;
      }

      document.body.removeChild(outer);
      return (w1 - w2);
    }



    /**
    * Change the order of the table
    *  @param {object} oSettings dataTables settings object
    *  @param {bool} bApplyClasses optional - should we apply classes or not
    *  @memberof DataTable#oApi
    */
    function _fnSort(oSettings, bApplyClasses) {
      var 
        i, iLen, j, jLen, k, kLen, sDataType, nTh, aaSort = [],
          aiOrig = [],
          oSort = DataTable.ext.oSort,
          aoData = oSettings.aoData,
          aoColumns = oSettings.aoColumns,
          oAria = oSettings.oLanguage.oAria;

      /* No sorting required if server-side or no sorting array */
      if (!oSettings.oFeatures.bServerSide && (oSettings.aaSorting.length !== 0 || oSettings.aaSortingFixed !== null)) {
        aaSort = (oSettings.aaSortingFixed !== null) ? oSettings.aaSortingFixed.concat(oSettings.aaSorting) : oSettings.aaSorting.slice();

        /* If there is a sorting data type, and a function belonging to it, then we need to
        * get the data from the developer's function and apply it for this column
        */
        for (i = 0; i < aaSort.length; i++) {
          var iColumn = aaSort[i][0];
          var iVisColumn = _fnColumnIndexToVisible(oSettings, iColumn);
          sDataType = oSettings.aoColumns[iColumn].sSortDataType;
          if (DataTable.ext.afnSortData[sDataType]) {
            var aData = DataTable.ext.afnSortData[sDataType].call(
              oSettings.oInstance, oSettings, iColumn, iVisColumn);
            if (aData.length === aoData.length) {
              for (j = 0, jLen = aoData.length; j < jLen; j++) {
                _fnSetCellData(oSettings, j, iColumn, aData[j]);
              }
            } else {
              _fnLog(oSettings, 0, "Returned data sort array (col " + iColumn + ") is the wrong length");
            }
          }
        }

        /* Create a value - key array of the current row positions such that we can use their
        * current position during the sort, if values match, in order to perform stable sorting
        */
        for (i = 0, iLen = oSettings.aiDisplayMaster.length; i < iLen; i++) {
          aiOrig[oSettings.aiDisplayMaster[i]] = i;
        }

        /* Build an internal data array which is specific to the sort, so we can get and prep
        * the data to be sorted only once, rather than needing to do it every time the sorting
        * function runs. This make the sorting function a very simple comparison
        */
        var iSortLen = aaSort.length;
        var fnSortFormat, aDataSort;
        for (i = 0, iLen = aoData.length; i < iLen; i++) {
          for (j = 0; j < iSortLen; j++) {
            aDataSort = aoColumns[aaSort[j][0]].aDataSort;

            for (k = 0, kLen = aDataSort.length; k < kLen; k++) {
              sDataType = aoColumns[aDataSort[k]].sType;
              fnSortFormat = oSort[(sDataType ? sDataType : 'string') + "-pre"];

              aoData[i]._aSortData[aDataSort[k]] = fnSortFormat ? fnSortFormat(_fnGetCellData(oSettings, i, aDataSort[k], 'sort')) : _fnGetCellData(oSettings, i, aDataSort[k], 'sort');
            }
          }
        }

        /* Do the sort - here we want multi-column sorting based on a given data source (column)
        * and sorting function (from oSort) in a certain direction. It's reasonably complex to
        * follow on it's own, but this is what we want (example two column sorting):
        *  fnLocalSorting = function(a,b){
        *  	var iTest;
        *  	iTest = oSort['string-asc']('data11', 'data12');
        *  	if (iTest !== 0)
        *  		return iTest;
        *    iTest = oSort['numeric-desc']('data21', 'data22');
        *    if (iTest !== 0)
        *  		return iTest;
        *  	return oSort['numeric-asc']( aiOrig[a], aiOrig[b] );
        *  }
        * Basically we have a test for each sorting column, if the data in that column is equal,
        * test the next column. If all columns match, then we use a numeric sort on the row 
        * positions in the original data array to provide a stable sort.
        */
        oSettings.aiDisplayMaster.sort(function (a, b) {
          var k, l, lLen, iTest, aDataSort, sDataType;
          for (k = 0; k < iSortLen; k++) {
            aDataSort = aoColumns[aaSort[k][0]].aDataSort;

            for (l = 0, lLen = aDataSort.length; l < lLen; l++) {
              sDataType = aoColumns[aDataSort[l]].sType;

              iTest = oSort[(sDataType ? sDataType : 'string') + "-" + aaSort[k][1]](
                aoData[a]._aSortData[aDataSort[l]], aoData[b]._aSortData[aDataSort[l]]);

              if (iTest !== 0) {
                return iTest;
              }
            }
          }

          return oSort['numeric-asc'](aiOrig[a], aiOrig[b]);
        });
      }

      /* Alter the sorting classes to take account of the changes */
      if ((bApplyClasses === undefined || bApplyClasses) && !oSettings.oFeatures.bDeferRender) {
        _fnSortingClasses(oSettings);
      }

      for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        var sTitle = aoColumns[i].sTitle.replace(/<.*?>/g, "");
        nTh = aoColumns[i].nTh;
        nTh.removeAttribute('aria-sort');
        nTh.removeAttribute('aria-label');

        /* In ARIA only the first sorting column can be marked as sorting - no multi-sort option */
        if (aoColumns[i].bSortable) {
          if (aaSort.length > 0 && aaSort[0][0] == i) {
            nTh.setAttribute('aria-sort', aaSort[0][1] == "asc" ? "ascending" : "descending");

            var nextSort = (aoColumns[i].asSorting[aaSort[0][2] + 1]) ? aoColumns[i].asSorting[aaSort[0][2] + 1] : aoColumns[i].asSorting[0];
            nTh.setAttribute('aria-label', sTitle + (nextSort == "asc" ? oAria.sSortAscending : oAria.sSortDescending));
          } else {
            nTh.setAttribute('aria-label', sTitle + (aoColumns[i].asSorting[0] == "asc" ? oAria.sSortAscending : oAria.sSortDescending));
          }
        } else {
          nTh.setAttribute('aria-label', sTitle);
        }
      }

      /* Tell the draw function that we have sorted the data */
      oSettings.bSorted = true;
      $(oSettings.oInstance).trigger('sort', oSettings);

      /* Copy the master data into the draw array and re-draw */
      if (oSettings.oFeatures.bFilter) {
        /* _fnFilter() will redraw the table for us */
        _fnFilterComplete(oSettings, oSettings.oPreviousSearch, 1);
      } else {
        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
        oSettings._iDisplayStart = 0; /* reset display back to page 0 */
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      }
    }


    /**
    * Attach a sort handler (click) to a node
    *  @param {object} oSettings dataTables settings object
    *  @param {node} nNode node to attach the handler to
    *  @param {int} iDataIndex column sorting index
    *  @param {function} [fnCallback] callback function
    *  @memberof DataTable#oApi
    */
    function _fnSortAttachListener(oSettings, nNode, iDataIndex, fnCallback) {
      _fnBindAction(nNode, {}, function (e) {
        /* If the column is not sortable - don't to anything */
        if (oSettings.aoColumns[iDataIndex].bSortable === false) {
          return;
        }

        /*
        * This is a little bit odd I admit... I declare a temporary function inside the scope of
        * _fnBuildHead and the click handler in order that the code presented here can be used 
        * twice - once for when bProcessing is enabled, and another time for when it is 
        * disabled, as we need to perform slightly different actions.
        *   Basically the issue here is that the Javascript engine in modern browsers don't 
        * appear to allow the rendering engine to update the display while it is still executing
        * it's thread (well - it does but only after long intervals). This means that the 
        * 'processing' display doesn't appear for a table sort. To break the js thread up a bit
        * I force an execution break by using setTimeout - but this breaks the expected 
        * thread continuation for the end-developer's point of view (their code would execute
        * too early), so we only do it when we absolutely have to.
        */
        var fnInnerSorting = function () {
          var iColumn, iNextSort;

          /* If the shift key is pressed then we are multiple column sorting */
          if (e.shiftKey) {
            /* Are we already doing some kind of sort on this column? */
            var bFound = false;
            for (var i = 0; i < oSettings.aaSorting.length; i++) {
              if (oSettings.aaSorting[i][0] == iDataIndex) {
                bFound = true;
                iColumn = oSettings.aaSorting[i][0];
                iNextSort = oSettings.aaSorting[i][2] + 1;

                if (!oSettings.aoColumns[iColumn].asSorting[iNextSort]) {
                  /* Reached the end of the sorting options, remove from multi-col sort */
                  oSettings.aaSorting.splice(i, 1);
                } else {
                  /* Move onto next sorting direction */
                  oSettings.aaSorting[i][1] = oSettings.aoColumns[iColumn].asSorting[iNextSort];
                  oSettings.aaSorting[i][2] = iNextSort;
                }
                break;
              }
            }

            /* No sort yet - add it in */
            if (bFound === false) {
              oSettings.aaSorting.push([iDataIndex, oSettings.aoColumns[iDataIndex].asSorting[0], 0]);
            }
          } else {
            /* If no shift key then single column sort */
            if (oSettings.aaSorting.length == 1 && oSettings.aaSorting[0][0] == iDataIndex) {
              iColumn = oSettings.aaSorting[0][0];
              iNextSort = oSettings.aaSorting[0][2] + 1;
              if (!oSettings.aoColumns[iColumn].asSorting[iNextSort]) {
                iNextSort = 0;
              }
              oSettings.aaSorting[0][1] = oSettings.aoColumns[iColumn].asSorting[iNextSort];
              oSettings.aaSorting[0][2] = iNextSort;
            } else {
              oSettings.aaSorting.splice(0, oSettings.aaSorting.length);
              oSettings.aaSorting.push([iDataIndex, oSettings.aoColumns[iDataIndex].asSorting[0], 0]);
            }
          }

          /* Run the sort */
          _fnSort(oSettings);
        }; /* /fnInnerSorting */

        if (!oSettings.oFeatures.bProcessing) {
          fnInnerSorting();
        } else {
          _fnProcessingDisplay(oSettings, true);
          setTimeout(function () {
            fnInnerSorting();
            if (!oSettings.oFeatures.bServerSide) {
              _fnProcessingDisplay(oSettings, false);
            }
          }, 0);
        }

        /* Call the user specified callback function - used for async user interaction */
        if (typeof fnCallback == 'function') {
          fnCallback(oSettings);
        }
      });
    }


    /**
    * Set the sorting classes on the header, Note: it is safe to call this function 
    * when bSort and bSortClasses are false
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnSortingClasses(oSettings) {
      var i, iLen, j, jLen, iFound;
      var aaSort, sClass;
      var iColumns = oSettings.aoColumns.length;
      var oClasses = oSettings.oClasses;

      for (i = 0; i < iColumns; i++) {
        if (oSettings.aoColumns[i].bSortable) {
          $(oSettings.aoColumns[i].nTh).removeClass(oClasses.sSortAsc + " " + oClasses.sSortDesc + " " + oSettings.aoColumns[i].sSortingClass);
        }
      }

      if (oSettings.aaSortingFixed !== null) {
        aaSort = oSettings.aaSortingFixed.concat(oSettings.aaSorting);
      } else {
        aaSort = oSettings.aaSorting.slice();
      }

      /* Apply the required classes to the header */
      for (i = 0; i < oSettings.aoColumns.length; i++) {
        if (oSettings.aoColumns[i].bSortable) {
          sClass = oSettings.aoColumns[i].sSortingClass;
          iFound = -1;
          for (j = 0; j < aaSort.length; j++) {
            if (aaSort[j][0] == i) {
              sClass = (aaSort[j][1] == "asc") ? oClasses.sSortAsc : oClasses.sSortDesc;
              iFound = j;
              break;
            }
          }
          $(oSettings.aoColumns[i].nTh).addClass(sClass);

          if (oSettings.bJUI) {
            /* jQuery UI uses extra markup */
            var jqSpan = $("span." + oClasses.sSortIcon, oSettings.aoColumns[i].nTh);
            jqSpan.removeClass(oClasses.sSortJUIAsc + " " + oClasses.sSortJUIDesc + " " + oClasses.sSortJUI + " " + oClasses.sSortJUIAscAllowed + " " + oClasses.sSortJUIDescAllowed);

            var sSpanClass;
            if (iFound == -1) {
              sSpanClass = oSettings.aoColumns[i].sSortingClassJUI;
            } else if (aaSort[iFound][1] == "asc") {
              sSpanClass = oClasses.sSortJUIAsc;
            } else {
              sSpanClass = oClasses.sSortJUIDesc;
            }

            jqSpan.addClass(sSpanClass);
          }
        } else {
          /* No sorting on this column, so add the base class. This will have been assigned by
          * _fnAddColumn
          */
          $(oSettings.aoColumns[i].nTh).addClass(oSettings.aoColumns[i].sSortingClass);
        }
      }

      /* 
      * Apply the required classes to the table body
      * Note that this is given as a feature switch since it can significantly slow down a sort
      * on large data sets (adding and removing of classes is always slow at the best of times..)
      * Further to this, note that this code is admittedly fairly ugly. It could be made a lot 
      * simpler using jQuery selectors and add/removeClass, but that is significantly slower
      * (on the order of 5 times slower) - hence the direct DOM manipulation here.
      * Note that for deferred drawing we do use jQuery - the reason being that taking the first
      * row found to see if the whole column needs processed can miss classes since the first
      * column might be new.
      */
      sClass = oClasses.sSortColumn;

      if (oSettings.oFeatures.bSort && oSettings.oFeatures.bSortClasses) {
        var nTds = _fnGetTdNodes(oSettings);

        /* Remove the old classes */
        if (oSettings.oFeatures.bDeferRender) {
          $(nTds).removeClass(sClass + '1 ' + sClass + '2 ' + sClass + '3');
        } else if (nTds.length >= iColumns) {
          for (i = 0; i < iColumns; i++) {
            if (nTds[i].className.indexOf(sClass + "1") != -1) {
              for (j = 0, jLen = (nTds.length / iColumns); j < jLen; j++) {
                nTds[(iColumns * j) + i].className = $.trim(nTds[(iColumns * j) + i].className.replace(sClass + "1", ""));
              }
            } else if (nTds[i].className.indexOf(sClass + "2") != -1) {
              for (j = 0, jLen = (nTds.length / iColumns); j < jLen; j++) {
                nTds[(iColumns * j) + i].className = $.trim(nTds[(iColumns * j) + i].className.replace(sClass + "2", ""));
              }
            } else if (nTds[i].className.indexOf(sClass + "3") != -1) {
              for (j = 0, jLen = (nTds.length / iColumns); j < jLen; j++) {
                nTds[(iColumns * j) + i].className = $.trim(nTds[(iColumns * j) + i].className.replace(" " + sClass + "3", ""));
              }
            }
          }
        }

        /* Add the new classes to the table */
        var iClass = 1,
            iTargetCol;
        for (i = 0; i < aaSort.length; i++) {
          iTargetCol = parseInt(aaSort[i][0], 10);
          for (j = 0, jLen = (nTds.length / iColumns); j < jLen; j++) {
            nTds[(iColumns * j) + iTargetCol].className += " " + sClass + iClass;
          }

          if (iClass < 3) {
            iClass++;
          }
        }
      }
    }



    /**
    * Save the state of a table in a cookie such that the page can be reloaded
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnSaveState(oSettings) {
      if (!oSettings.oFeatures.bStateSave || oSettings.bDestroying) {
        return;
      }

      /* Store the interesting variables */
      var i, iLen, bInfinite = oSettings.oScroll.bInfinite;
      var oState = {
        "iCreate": new Date().getTime(),
        "iStart": (bInfinite ? 0 : oSettings._iDisplayStart),
        "iEnd": (bInfinite ? oSettings._iDisplayLength : oSettings._iDisplayEnd),
        "iLength": oSettings._iDisplayLength,
        "aaSorting": $.extend(true, [], oSettings.aaSorting),
        "oSearch": $.extend(true, {}, oSettings.oPreviousSearch),
        "aoSearchCols": $.extend(true, [], oSettings.aoPreSearchCols),
        "abVisCols": []
      };

      for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        oState.abVisCols.push(oSettings.aoColumns[i].bVisible);
      }

      _fnCallbackFire(oSettings, "aoStateSaveParams", 'stateSaveParams', [oSettings, oState]);

      oSettings.fnStateSave.call(oSettings.oInstance, oSettings, oState);
    }


    /**
    * Attempt to load a saved table state from a cookie
    *  @param {object} oSettings dataTables settings object
    *  @param {object} oInit DataTables init object so we can override settings
    *  @memberof DataTable#oApi
    */
    function _fnLoadState(oSettings, oInit) {
      if (!oSettings.oFeatures.bStateSave) {
        return;
      }

      var oData = oSettings.fnStateLoad.call(oSettings.oInstance, oSettings);
      if (!oData) {
        return;
      }

      /* Allow custom and plug-in manipulation functions to alter the saved data set and
      * cancelling of loading by returning false
      */
      var abStateLoad = _fnCallbackFire(oSettings, 'aoStateLoadParams', 'stateLoadParams', [oSettings, oData]);
      if ($.inArray(false, abStateLoad) !== -1) {
        return;
      }

      /* Store the saved state so it might be accessed at any time */
      oSettings.oLoadedState = $.extend(true, {}, oData);

      /* Restore key features */
      oSettings._iDisplayStart = oData.iStart;
      oSettings.iInitDisplayStart = oData.iStart;
      oSettings._iDisplayEnd = oData.iEnd;
      oSettings._iDisplayLength = oData.iLength;
      oSettings.aaSorting = oData.aaSorting.slice();
      oSettings.saved_aaSorting = oData.aaSorting.slice();

      /* Search filtering  */
      $.extend(oSettings.oPreviousSearch, oData.oSearch);
      $.extend(true, oSettings.aoPreSearchCols, oData.aoSearchCols);

      /* Column visibility state
      * Pass back visibility settings to the init handler, but to do not here override
      * the init object that the user might have passed in
      */
      oInit.saved_aoColumns = [];
      for (var i = 0; i < oData.abVisCols.length; i++) {
        oInit.saved_aoColumns[i] = {};
        oInit.saved_aoColumns[i].bVisible = oData.abVisCols[i];
      }

      _fnCallbackFire(oSettings, 'aoStateLoaded', 'stateLoaded', [oSettings, oData]);
    }


    /**
    * Create a new cookie with a value to store the state of a table
    *  @param {string} sName name of the cookie to create
    *  @param {string} sValue the value the cookie should take
    *  @param {int} iSecs duration of the cookie
    *  @param {string} sBaseName sName is made up of the base + file name - this is the base
    *  @param {function} fnCallback User definable function to modify the cookie
    *  @memberof DataTable#oApi
    */
    function _fnCreateCookie(sName, sValue, iSecs, sBaseName, fnCallback) {
      var date = new Date();
      date.setTime(date.getTime() + (iSecs * 1000));

      /* 
      * Shocking but true - it would appear IE has major issues with having the path not having
      * a trailing slash on it. We need the cookie to be available based on the path, so we
      * have to append the file name to the cookie name. Appalling. Thanks to vex for adding the
      * patch to use at least some of the path
      */
      var aParts = window.location.pathname.split('/');
      var sNameFile = sName + '_' + aParts.pop().replace(/[\/:]/g, "").toLowerCase();
      var sFullCookie, oData;

      if (fnCallback !== null) {
        oData = (typeof $.parseJSON === 'function') ? $.parseJSON(sValue) : eval('(' + sValue + ')');
        sFullCookie = fnCallback(sNameFile, oData, date.toGMTString(), aParts.join('/') + "/");
      } else {
        sFullCookie = sNameFile + "=" + encodeURIComponent(sValue) + "; expires=" + date.toGMTString() + "; path=" + aParts.join('/') + "/";
      }

      /* Are we going to go over the cookie limit of 4KiB? If so, try to delete a cookies
      * belonging to DataTables. This is FAR from bullet proof
      */
      var sOldName = "",
          iOldTime = 9999999999999;
      var iLength = _fnReadCookie(sNameFile) !== null ? document.cookie.length : sFullCookie.length + document.cookie.length;

      if (iLength + 10 > 4096) /* Magic 10 for padding */
      {
        var aCookies = document.cookie.split(';');
        for (var i = 0, iLen = aCookies.length; i < iLen; i++) {
          if (aCookies[i].indexOf(sBaseName) != -1) {
            /* It's a DataTables cookie, so eval it and check the time stamp */
            var aSplitCookie = aCookies[i].split('=');
            try {
              oData = eval('(' + decodeURIComponent(aSplitCookie[1]) + ')');
            } catch (e) {
              continue;
            }

            if (oData.iCreate && oData.iCreate < iOldTime) {
              sOldName = aSplitCookie[0];
              iOldTime = oData.iCreate;
            }
          }
        }

        if (sOldName !== "") {
          document.cookie = sOldName + "=; expires=Thu, 01-Jan-1970 00:00:01 GMT; path=" + aParts.join('/') + "/";
        }
      }

      document.cookie = sFullCookie;
    }


    /**
    * Read an old cookie to get a cookie with an old table state
    *  @param {string} sName name of the cookie to read
    *  @returns {string} contents of the cookie - or null if no cookie with that name found
    *  @memberof DataTable#oApi
    */
    function _fnReadCookie(sName) {
      var 
        aParts = window.location.pathname.split('/'),
          sNameEQ = sName + '_' + aParts[aParts.length - 1].replace(/[\/:]/g, "").toLowerCase() + '=',
          sCookieContents = document.cookie.split(';');

      for (var i = 0; i < sCookieContents.length; i++) {
        var c = sCookieContents[i];

        while (c.charAt(0) == ' ') {
          c = c.substring(1, c.length);
        }

        if (c.indexOf(sNameEQ) === 0) {
          return decodeURIComponent(c.substring(sNameEQ.length, c.length));
        }
      }
      return null;
    }



    /**
    * Return the settings object for a particular table
    *  @param {node} nTable table we are using as a dataTable
    *  @returns {object} Settings object - or null if not found
    *  @memberof DataTable#oApi
    */
    function _fnSettingsFromNode(nTable) {
      for (var i = 0; i < DataTable.settings.length; i++) {
        if (DataTable.settings[i].nTable === nTable) {
          return DataTable.settings[i];
        }
      }

      return null;
    }


    /**
    * Return an array with the TR nodes for the table
    *  @param {object} oSettings dataTables settings object
    *  @returns {array} TR array
    *  @memberof DataTable#oApi
    */
    function _fnGetTrNodes(oSettings) {
      var aNodes = [];
      var aoData = oSettings.aoData;
      for (var i = 0, iLen = aoData.length; i < iLen; i++) {
        if (aoData[i].nTr !== null) {
          aNodes.push(aoData[i].nTr);
        }
      }
      return aNodes;
    }


    /**
    * Return an flat array with all TD nodes for the table, or row
    *  @param {object} oSettings dataTables settings object
    *  @param {int} [iIndividualRow] aoData index to get the nodes for - optional 
    *    if not given then the return array will contain all nodes for the table
    *  @returns {array} TD array
    *  @memberof DataTable#oApi
    */
    function _fnGetTdNodes(oSettings, iIndividualRow) {
      var anReturn = [];
      var iCorrector;
      var anTds;
      var iRow, iRows = oSettings.aoData.length,
          iColumn, iColumns, oData, sNodeName, iStart = 0,
          iEnd = iRows;

      /* Allow the collection to be limited to just one row */
      if (iIndividualRow !== undefined) {
        iStart = iIndividualRow;
        iEnd = iIndividualRow + 1;
      }

      for (iRow = iStart; iRow < iEnd; iRow++) {
        oData = oSettings.aoData[iRow];
        if (oData.nTr !== null) {
          /* get the TD child nodes - taking into account text etc nodes */
          anTds = [];
          for (iColumn = 0, iColumns = oData.nTr.childNodes.length; iColumn < iColumns; iColumn++) {
            sNodeName = oData.nTr.childNodes[iColumn].nodeName.toLowerCase();
            if (sNodeName == 'td' || sNodeName == 'th') {
              anTds.push(oData.nTr.childNodes[iColumn]);
            }
          }

          iCorrector = 0;
          for (iColumn = 0, iColumns = oSettings.aoColumns.length; iColumn < iColumns; iColumn++) {
            if (oSettings.aoColumns[iColumn].bVisible) {
              anReturn.push(anTds[iColumn - iCorrector]);
            } else {
              anReturn.push(oData._anHidden[iColumn]);
              iCorrector++;
            }
          }
        }
      }

      return anReturn;
    }


    /**
    * Log an error message
    *  @param {object} oSettings dataTables settings object
    *  @param {int} iLevel log error messages, or display them to the user
    *  @param {string} sMesg error message
    *  @memberof DataTable#oApi
    */
    function _fnLog(oSettings, iLevel, sMesg) {
      var sAlert = (oSettings === null) ? "DataTables warning: " + sMesg : "DataTables warning (table id = '" + oSettings.sTableId + "'): " + sMesg;

      if (iLevel === 0) {
        if (DataTable.ext.sErrMode == 'alert') {
          alert(sAlert);
        } else {
          throw new Error(sAlert);
        }
        return;
      } else if (window.console && console.log) {
        console.log(sAlert);
      }
    }


    /**
    * See if a property is defined on one object, if so assign it to the other object
    *  @param {object} oRet target object
    *  @param {object} oSrc source object
    *  @param {string} sName property
    *  @param {string} [sMappedName] name to map too - optional, sName used if not given
    *  @memberof DataTable#oApi
    */
    function _fnMap(oRet, oSrc, sName, sMappedName) {
      if (sMappedName === undefined) {
        sMappedName = sName;
      }
      if (oSrc[sName] !== undefined) {
        oRet[sMappedName] = oSrc[sName];
      }
    }


    /**
    * Extend objects - very similar to jQuery.extend, but deep copy objects, and shallow
    * copy arrays. The reason we need to do this, is that we don't want to deep copy array
    * init values (such as aaSorting) since the dev wouldn't be able to override them, but
    * we do want to deep copy arrays.
    *  @param {object} oOut Object to extend
    *  @param {object} oExtender Object from which the properties will be applied to oOut
    *  @returns {object} oOut Reference, just for convenience - oOut === the return.
    *  @memberof DataTable#oApi
    *  @todo This doesn't take account of arrays inside the deep copied objects.
    */
    function _fnExtend(oOut, oExtender) {
      var val;

      for (var prop in oExtender) {
        if (oExtender.hasOwnProperty(prop)) {
          val = oExtender[prop];

          if (typeof oInit[prop] === 'object' && val !== null && $.isArray(val) === false) {
            $.extend(true, oOut[prop], val);
          } else {
            oOut[prop] = val;
          }
        }
      }

      return oOut;
    }


    /**
    * Bind an event handers to allow a click or return key to activate the callback.
    * This is good for accessibility since a return on the keyboard will have the
    * same effect as a click, if the element has focus.
    *  @param {element} n Element to bind the action to
    *  @param {object} oData Data object to pass to the triggered function
    *  @param {function} fn Callback function for when the event is triggered
    *  @memberof DataTable#oApi
    */
    function _fnBindAction(n, oData, fn) {
      $(n).bind('click.DT', oData, function (e) {
        n.blur(); // Remove focus outline for mouse users
        fn(e);
      }).bind('keypress.DT', oData, function (e) {
        if (e.which === 13) {
          fn(e);
        }
      }).bind('selectstart.DT', function () {
        /* Take the brutal approach to cancelling text selection */
        return false;
      });
    }


    /**
    * Register a callback function. Easily allows a callback function to be added to
    * an array store of callback functions that can then all be called together.
    *  @param {object} oSettings dataTables settings object
    *  @param {string} sStore Name of the array storage for the callbacks in oSettings
    *  @param {function} fn Function to be called back
    *  @param {string} sName Identifying name for the callback (i.e. a label)
    *  @memberof DataTable#oApi
    */
    function _fnCallbackReg(oSettings, sStore, fn, sName) {
      if (fn) {
        oSettings[sStore].push({
          "fn": fn,
          "sName": sName
        });
      }
    }


    /**
    * Fire callback functions and trigger events. Note that the loop over the callback
    * array store is done backwards! Further note that you do not want to fire off triggers
    * in time sensitive applications (for example cell creation) as its slow.
    *  @param {object} oSettings dataTables settings object
    *  @param {string} sStore Name of the array storage for the callbacks in oSettings
    *  @param {string} sTrigger Name of the jQuery custom event to trigger. If null no trigger
    *    is fired
    *  @param {array} aArgs Array of arguments to pass to the callback function / trigger
    *  @memberof DataTable#oApi
    */
    function _fnCallbackFire(oSettings, sStore, sTrigger, aArgs) {
      var aoStore = oSettings[sStore];
      var aRet = [];

      for (var i = aoStore.length - 1; i >= 0; i--) {
        aRet.push(aoStore[i].fn.apply(oSettings.oInstance, aArgs));
      }

      if (sTrigger !== null) {
        $(oSettings.oInstance).trigger(sTrigger, aArgs);
      }

      return aRet;
    }


    /**
    * JSON stringify. If JSON.stringify it provided by the browser, json2.js or any other
    * library, then we use that as it is fast, safe and accurate. If the function isn't 
    * available then we need to built it ourselves - the inspiration for this function comes
    * from Craig Buckler ( http://www.sitepoint.com/javascript-json-serialization/ ). It is
    * not perfect and absolutely should not be used as a replacement to json2.js - but it does
    * do what we need, without requiring a dependency for DataTables.
    *  @param {object} o JSON object to be converted
    *  @returns {string} JSON string
    *  @memberof DataTable#oApi
    */
    var _fnJsonString = (window.JSON) ? JSON.stringify : function (o) {
      /* Not an object or array */
      var sType = typeof o;
      if (sType !== "object" || o === null) {
        // simple data type
        if (sType === "string") {
          o = '"' + o + '"';
        }
        return o + "";
      }

      /* If object or array, need to recurse over it */
      var 
          sProp, mValue, json = [],
            bArr = $.isArray(o);

      for (sProp in o) {
        mValue = o[sProp];
        sType = typeof mValue;

        if (sType === "string") {
          mValue = '"' + mValue + '"';
        } else if (sType === "object" && mValue !== null) {
          mValue = _fnJsonString(mValue);
        }

        json.push((bArr ? "" : '"' + sProp + '":') + mValue);
      }

      return (bArr ? "[" : "{") + json + (bArr ? "]" : "}");
    };


    /**
    * From some browsers (specifically IE6/7) we need special handling to work around browser
    * bugs - this function is used to detect when these workarounds are needed.
    *  @param {object} oSettings dataTables settings object
    *  @memberof DataTable#oApi
    */
    function _fnBrowserDetect(oSettings) {
      /* IE6/7 will oversize a width 100% element inside a scrolling element, to include the
      * width of the scrollbar, while other browsers ensure the inner element is contained
      * without forcing scrolling
      */
      var n = $('<div style="position:absolute; top:0; left:0; height:1px; width:1px; overflow:hidden">' + '<div style="position:absolute; top:1px; left:1px; width:100px; height:50px; overflow:scroll;">' + '<div id="DT_BrowserTest" style="width:100%; height:10px;"></div>' + '</div>' + '</div>')[0];

      document.body.appendChild(n);
      oSettings.oBrowser.bScrollOversize = $('#DT_BrowserTest', n)[0].offsetWidth === 100 ? true : false;
      document.body.removeChild(n);
    }




    /**
    * Perform a jQuery selector action on the table's TR elements (from the tbody) and
    * return the resulting jQuery object.
    *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
    *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
    *  @param {string} [oOpts.filter=none] Select TR elements that meet the current filter
    *    criterion ("applied") or all TR elements (i.e. no filter).
    *  @param {string} [oOpts.order=current] Order of the TR elements in the processed array.
    *    Can be either 'current', whereby the current sorting of the table is used, or
    *    'original' whereby the original order the data was read into the table is used.
    *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
    *    ("current") or not ("all"). If 'current' is given, then order is assumed to be 
    *    'current' and filter is 'applied', regardless of what they might be given as.
    *  @returns {object} jQuery object, filtered by the given selector.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *
    *      // Highlight every second row
    *      oTable.$('tr:odd').css('backgroundColor', 'blue');
    *    } );
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *
    *      // Filter to rows with 'Webkit' in them, add a background colour and then
    *      // remove the filter, thus highlighting the 'Webkit' rows only.
    *      oTable.fnFilter('Webkit');
    *      oTable.$('tr', {"filter": "applied"}).css('backgroundColor', 'blue');
    *      oTable.fnFilter('');
    *    } );
    */
    this.$ = function (sSelector, oOpts) {
      var i, iLen, a = [],
          tr;
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var aoData = oSettings.aoData;
      var aiDisplay = oSettings.aiDisplay;
      var aiDisplayMaster = oSettings.aiDisplayMaster;

      if (!oOpts) {
        oOpts = {};
      }

      oOpts = $.extend({}, {
        "filter": "none",
        // applied
        "order": "current",
        // "original"
        "page": "all" // current
      }, oOpts);

      // Current page implies that order=current and fitler=applied, since it is fairly
      // senseless otherwise
      if (oOpts.page == 'current') {
        for (i = oSettings._iDisplayStart, iLen = oSettings.fnDisplayEnd(); i < iLen; i++) {
          tr = aoData[aiDisplay[i]].nTr;
          if (tr) {
            a.push(tr);
          }
        }
      } else if (oOpts.order == "current" && oOpts.filter == "none") {
        for (i = 0, iLen = aiDisplayMaster.length; i < iLen; i++) {
          tr = aoData[aiDisplayMaster[i]].nTr;
          if (tr) {
            a.push(tr);
          }
        }
      } else if (oOpts.order == "current" && oOpts.filter == "applied") {
        for (i = 0, iLen = aiDisplay.length; i < iLen; i++) {
          tr = aoData[aiDisplay[i]].nTr;
          if (tr) {
            a.push(tr);
          }
        }
      } else if (oOpts.order == "original" && oOpts.filter == "none") {
        for (i = 0, iLen = aoData.length; i < iLen; i++) {
          tr = aoData[i].nTr;
          if (tr) {
            a.push(tr);
          }
        }
      } else if (oOpts.order == "original" && oOpts.filter == "applied") {
        for (i = 0, iLen = aoData.length; i < iLen; i++) {
          tr = aoData[i].nTr;
          if ($.inArray(i, aiDisplay) !== -1 && tr) {
            a.push(tr);
          }
        }
      } else {
        _fnLog(oSettings, 1, "Unknown selection options");
      }

      /* We need to filter on the TR elements and also 'find' in their descendants
      * to make the selector act like it would in a full table - so we need
      * to build both results and then combine them together
      */
      var jqA = $(a);
      var jqTRs = jqA.filter(sSelector);
      var jqDescendants = jqA.find(sSelector);

      return $([].concat($.makeArray(jqTRs), $.makeArray(jqDescendants)));
    };


    /**
    * Almost identical to $ in operation, but in this case returns the data for the matched
    * rows - as such, the jQuery selector used should match TR row nodes or TD/TH cell nodes
    * rather than any descendants, so the data can be obtained for the row/cell. If matching
    * rows are found, the data returned is the original data array/object that was used to  
    * create the row (or a generated array if from a DOM source).
    *
    * This method is often useful in-combination with $ where both functions are given the
    * same parameters and the array indexes will match identically.
    *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
    *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
    *  @param {string} [oOpts.filter=none] Select elements that meet the current filter
    *    criterion ("applied") or all elements (i.e. no filter).
    *  @param {string} [oOpts.order=current] Order of the data in the processed array.
    *    Can be either 'current', whereby the current sorting of the table is used, or
    *    'original' whereby the original order the data was read into the table is used.
    *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
    *    ("current") or not ("all"). If 'current' is given, then order is assumed to be 
    *    'current' and filter is 'applied', regardless of what they might be given as.
    *  @returns {array} Data for the matched elements. If any elements, as a result of the
    *    selector, were not TR, TD or TH elements in the DataTable, they will have a null 
    *    entry in the array.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *
    *      // Get the data from the first row in the table
    *      var data = oTable._('tr:first');
    *
    *      // Do something useful with the data
    *      alert( "First cell is: "+data[0] );
    *    } );
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *
    *      // Filter to 'Webkit' and get all data for 
    *      oTable.fnFilter('Webkit');
    *      var data = oTable._('tr', {"filter": "applied"});
    *      
    *      // Do something with the data
    *      alert( data.length+" rows matched the filter" );
    *    } );
    */
    this._ = function (sSelector, oOpts) {
      var aOut = [];
      var i, iLen, iIndex;
      var aTrs = this.$(sSelector, oOpts);

      for (i = 0, iLen = aTrs.length; i < iLen; i++) {
        aOut.push(this.fnGetData(aTrs[i]));
      }

      return aOut;
    };


    /**
    * Add a single new row or multiple rows of data to the table. Please note
    * that this is suitable for client-side processing only - if you are using 
    * server-side processing (i.e. "bServerSide": true), then to add data, you
    * must add it to the data source, i.e. the server-side, through an Ajax call.
    *  @param {array|object} mData The data to be added to the table. This can be:
    *    <ul>
    *      <li>1D array of data - add a single row with the data provided</li>
    *      <li>2D array of arrays - add multiple rows in a single call</li>
    *      <li>object - data object when using <i>mData</i></li>
    *      <li>array of objects - multiple data objects when using <i>mData</i></li>
    *    </ul>
    *  @param {bool} [bRedraw=true] redraw the table or not
    *  @returns {array} An array of integers, representing the list of indexes in 
    *    <i>aoData</i> ({@link DataTable.models.oSettings}) that have been added to 
    *    the table.
    *  @dtopt API
    *
    *  @example
    *    // Global var for counter
    *    var giCount = 2;
    *    
    *    $(document).ready(function() {
    *      $('#example').dataTable();
    *    } );
    *    
    *    function fnClickAddRow() {
    *      $('#example').dataTable().fnAddData( [
    *        giCount+".1",
    *        giCount+".2",
    *        giCount+".3",
    *        giCount+".4" ]
    *      );
    *        
    *      giCount++;
    *    }
    */
    this.fnAddData = function (mData, bRedraw) {
      if (mData.length === 0) {
        return [];
      }

      var aiReturn = [];
      var iTest;

      /* Find settings from table node */
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      /* Check if we want to add multiple rows or not */
      if (typeof mData[0] === "object" && mData[0] !== null) {
        for (var i = 0; i < mData.length; i++) {
          iTest = _fnAddData(oSettings, mData[i]);
          if (iTest == -1) {
            return aiReturn;
          }
          aiReturn.push(iTest);
        }
      } else {
        iTest = _fnAddData(oSettings, mData);
        if (iTest == -1) {
          return aiReturn;
        }
        aiReturn.push(iTest);
      }

      oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();

      if (bRedraw === undefined || bRedraw) {
        _fnReDraw(oSettings);
      }
      return aiReturn;
    };


    /**
    * This function will make DataTables recalculate the column sizes, based on the data 
    * contained in the table and the sizes applied to the columns (in the DOM, CSS or 
    * through the sWidth parameter). This can be useful when the width of the table's 
    * parent element changes (for example a window resize).
    *  @param {boolean} [bRedraw=true] Redraw the table or not, you will typically want to
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable( {
    *        "sScrollY": "200px",
    *        "bPaginate": false
    *      } );
    *      
    *      $(window).bind('resize', function () {
    *        oTable.fnAdjustColumnSizing();
    *      } );
    *    } );
    */
    this.fnAdjustColumnSizing = function (bRedraw) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      _fnAdjustColumnSizing(oSettings);

      if (bRedraw === undefined || bRedraw) {
        this.fnDraw(false);
      } else if (oSettings.oScroll.sX !== "" || oSettings.oScroll.sY !== "") {
        /* If not redrawing, but scrolling, we want to apply the new column sizes anyway */
        this.oApi._fnScrollDraw(oSettings);
      }
    };


    /**
    * Quickly and simply clear a table
    *  @param {bool} [bRedraw=true] redraw the table or not
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Immediately 'nuke' the current rows (perhaps waiting for an Ajax callback...)
    *      oTable.fnClearTable();
    *    } );
    */
    this.fnClearTable = function (bRedraw) {
      /* Find settings from table node */
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      _fnClearTable(oSettings);

      if (bRedraw === undefined || bRedraw) {
        _fnDraw(oSettings);
      }
    };


    /**
    * The exact opposite of 'opening' a row, this function will close any rows which 
    * are currently 'open'.
    *  @param {node} nTr the table row to 'close'
    *  @returns {int} 0 on success, or 1 if failed (can't find the row)
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable;
    *      
    *      // 'open' an information row when a row is clicked on
    *      $('#example tbody tr').click( function () {
    *        if ( oTable.fnIsOpen(this) ) {
    *          oTable.fnClose( this );
    *        } else {
    *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
    *        }
    *      } );
    *      
    *      oTable = $('#example').dataTable();
    *    } );
    */
    this.fnClose = function (nTr) {
      /* Find settings from table node */
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      for (var i = 0; i < oSettings.aoOpenRows.length; i++) {
        if (oSettings.aoOpenRows[i].nParent == nTr) {
          var nTrParent = oSettings.aoOpenRows[i].nTr.parentNode;
          if (nTrParent) {
            /* Remove it if it is currently on display */
            nTrParent.removeChild(oSettings.aoOpenRows[i].nTr);
          }
          oSettings.aoOpenRows.splice(i, 1);
          return 0;
        }
      }
      return 1;
    };


    /**
    * Remove a row for the table
    *  @param {mixed} mTarget The index of the row from aoData to be deleted, or
    *    the TR element you want to delete
    *  @param {function|null} [fnCallBack] Callback function
    *  @param {bool} [bRedraw=true] Redraw the table or not
    *  @returns {array} The row that was deleted
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Immediately remove the first row
    *      oTable.fnDeleteRow( 0 );
    *    } );
    */
    this.fnDeleteRow = function (mTarget, fnCallBack, bRedraw) {
      /* Find settings from table node */
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var i, iLen, iAODataIndex;

      iAODataIndex = (typeof mTarget === 'object') ? _fnNodeToDataIndex(oSettings, mTarget) : mTarget;

      /* Return the data array from this row */
      var oData = oSettings.aoData.splice(iAODataIndex, 1);

      /* Update the _DT_RowIndex parameter */
      for (i = 0, iLen = oSettings.aoData.length; i < iLen; i++) {
        if (oSettings.aoData[i].nTr !== null) {
          oSettings.aoData[i].nTr._DT_RowIndex = i;
        }
      }

      /* Remove the target row from the search array */
      var iDisplayIndex = $.inArray(iAODataIndex, oSettings.aiDisplay);
      oSettings.asDataSearch.splice(iDisplayIndex, 1);

      /* Delete from the display arrays */
      _fnDeleteIndex(oSettings.aiDisplayMaster, iAODataIndex);
      _fnDeleteIndex(oSettings.aiDisplay, iAODataIndex);

      /* If there is a user callback function - call it */
      if (typeof fnCallBack === "function") {
        fnCallBack.call(this, oSettings, oData);
      }

      /* Check for an 'overflow' they case for displaying the table */
      if (oSettings._iDisplayStart >= oSettings.fnRecordsDisplay()) {
        oSettings._iDisplayStart -= oSettings._iDisplayLength;
        if (oSettings._iDisplayStart < 0) {
          oSettings._iDisplayStart = 0;
        }
      }

      if (bRedraw === undefined || bRedraw) {
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      }

      return oData;
    };


    /**
    * Restore the table to it's original state in the DOM by removing all of DataTables 
    * enhancements, alterations to the DOM structure of the table and event listeners.
    *  @param {boolean} [bRemove=false] Completely remove the table from the DOM
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      // This example is fairly pointless in reality, but shows how fnDestroy can be used
    *      var oTable = $('#example').dataTable();
    *      oTable.fnDestroy();
    *    } );
    */
    this.fnDestroy = function (bRemove) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var nOrig = oSettings.nTableWrapper.parentNode;
      var nBody = oSettings.nTBody;
      var i, iLen;

      bRemove = (bRemove === undefined) ? false : true;

      /* Flag to note that the table is currently being destroyed - no action should be taken */
      oSettings.bDestroying = true;

      /* Fire off the destroy callbacks for plug-ins etc */
      _fnCallbackFire(oSettings, "aoDestroyCallback", "destroy", [oSettings]);

      /* Restore hidden columns */
      for (i = 0, iLen = oSettings.aoColumns.length; i < iLen; i++) {
        if (oSettings.aoColumns[i].bVisible === false) {
          this.fnSetColumnVis(i, true);
        }
      }

      /* Blitz all DT events */
      $(oSettings.nTableWrapper).find('*').andSelf().unbind('.DT');

      /* If there is an 'empty' indicator row, remove it */
      $('tbody>tr>td.' + oSettings.oClasses.sRowEmpty, oSettings.nTable).parent().remove();

      /* When scrolling we had to break the table up - restore it */
      if (oSettings.nTable != oSettings.nTHead.parentNode) {
        $(oSettings.nTable).children('thead').remove();
        oSettings.nTable.appendChild(oSettings.nTHead);
      }

      if (oSettings.nTFoot && oSettings.nTable != oSettings.nTFoot.parentNode) {
        $(oSettings.nTable).children('tfoot').remove();
        oSettings.nTable.appendChild(oSettings.nTFoot);
      }

      /* Remove the DataTables generated nodes, events and classes */
      oSettings.nTable.parentNode.removeChild(oSettings.nTable);
      $(oSettings.nTableWrapper).remove();

      oSettings.aaSorting = [];
      oSettings.aaSortingFixed = [];
      _fnSortingClasses(oSettings);

      $(_fnGetTrNodes(oSettings)).removeClass(oSettings.asStripeClasses.join(' '));

      $('th, td', oSettings.nTHead).removeClass([
        oSettings.oClasses.sSortable, oSettings.oClasses.sSortableAsc, oSettings.oClasses.sSortableDesc, oSettings.oClasses.sSortableNone].join(' '));
      if (oSettings.bJUI) {
        $('th span.' + oSettings.oClasses.sSortIcon + ', td span.' + oSettings.oClasses.sSortIcon, oSettings.nTHead).remove();

        $('th, td', oSettings.nTHead).each(function () {
          var jqWrapper = $('div.' + oSettings.oClasses.sSortJUIWrapper, this);
          var kids = jqWrapper.contents();
          $(this).append(kids);
          jqWrapper.remove();
        });
      }

      /* Add the TR elements back into the table in their original order */
      if (!bRemove && oSettings.nTableReinsertBefore) {
        nOrig.insertBefore(oSettings.nTable, oSettings.nTableReinsertBefore);
      } else if (!bRemove) {
        nOrig.appendChild(oSettings.nTable);
      }

      for (i = 0, iLen = oSettings.aoData.length; i < iLen; i++) {
        if (oSettings.aoData[i].nTr !== null) {
          nBody.appendChild(oSettings.aoData[i].nTr);
        }
      }

      /* Restore the width of the original table */
      if (oSettings.oFeatures.bAutoWidth === true) {
        oSettings.nTable.style.width = _fnStringToCss(oSettings.sDestroyWidth);
      }

      /* If the were originally odd/even type classes - then we add them back here. Note
      * this is not fool proof (for example if not all rows as odd/even classes - but 
      * it's a good effort without getting carried away
      */
      $(nBody).children('tr:even').addClass(oSettings.asDestroyStripes[0]);
      $(nBody).children('tr:odd').addClass(oSettings.asDestroyStripes[1]);

      /* Remove the settings object from the settings array */
      for (i = 0, iLen = DataTable.settings.length; i < iLen; i++) {
        if (DataTable.settings[i] == oSettings) {
          DataTable.settings.splice(i, 1);
        }
      }

      /* End it all */
      oSettings = null;
    };


    /**
    * Redraw the table
    *  @param {bool} [bComplete=true] Re-filter and resort (if enabled) the table before the draw.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Re-draw the table - you wouldn't want to do it here, but it's an example :-)
    *      oTable.fnDraw();
    *    } );
    */
    this.fnDraw = function (bComplete) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      if (bComplete === false) {
        _fnCalculateEnd(oSettings);
        _fnDraw(oSettings);
      } else {
        _fnReDraw(oSettings);
      }
    };


    /**
    * Filter the input based on data
    *  @param {string} sInput String to filter the table on
    *  @param {int|null} [iColumn] Column to limit filtering to
    *  @param {bool} [bRegex=false] Treat as regular expression or not
    *  @param {bool} [bSmart=true] Perform smart filtering or not
    *  @param {bool} [bShowGlobal=true] Show the input global filter in it's input box(es)
    *  @param {bool} [bCaseInsensitive=true] Do case-insensitive matching (true) or not (false)
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Sometime later - filter...
    *      oTable.fnFilter( 'test string' );
    *    } );
    */
    this.fnFilter = function (sInput, iColumn, bRegex, bSmart, bShowGlobal, bCaseInsensitive) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      if (!oSettings.oFeatures.bFilter) {
        return;
      }

      if (bRegex === undefined || bRegex === null) {
        bRegex = false;
      }

      if (bSmart === undefined || bSmart === null) {
        bSmart = true;
      }

      if (bShowGlobal === undefined || bShowGlobal === null) {
        bShowGlobal = true;
      }

      if (bCaseInsensitive === undefined || bCaseInsensitive === null) {
        bCaseInsensitive = true;
      }

      if (iColumn === undefined || iColumn === null) {
        /* Global filter */
        _fnFilterComplete(oSettings, {
          "sSearch": sInput + "",
          "bRegex": bRegex,
          "bSmart": bSmart,
          "bCaseInsensitive": bCaseInsensitive
        }, 1);

        if (bShowGlobal && oSettings.aanFeatures.f) {
          var n = oSettings.aanFeatures.f;
          for (var i = 0, iLen = n.length; i < iLen; i++) {
            $(n[i]._DT_Input).val(sInput);
          }
        }
      } else {
        /* Single column filter */
        $.extend(oSettings.aoPreSearchCols[iColumn], {
          "sSearch": sInput + "",
          "bRegex": bRegex,
          "bSmart": bSmart,
          "bCaseInsensitive": bCaseInsensitive
        });
        _fnFilterComplete(oSettings, oSettings.oPreviousSearch, 1);
      }
    };


    /**
    * Get the data for the whole table, an individual row or an individual cell based on the 
    * provided parameters.
    *  @param {int|node} [mRow] A TR row node, TD/TH cell node or an integer. If given as
    *    a TR node then the data source for the whole row will be returned. If given as a
    *    TD/TH cell node then iCol will be automatically calculated and the data for the
    *    cell returned. If given as an integer, then this is treated as the aoData internal
    *    data index for the row (see fnGetPosition) and the data for that row used.
    *  @param {int} [iCol] Optional column index that you want the data of.
    *  @returns {array|object|string} If mRow is undefined, then the data for all rows is
    *    returned. If mRow is defined, just data for that row, and is iCol is
    *    defined, only data for the designated cell is returned.
    *  @dtopt API
    *
    *  @example
    *    // Row data
    *    $(document).ready(function() {
    *      oTable = $('#example').dataTable();
    *
    *      oTable.$('tr').click( function () {
    *        var data = oTable.fnGetData( this );
    *        // ... do something with the array / object of data for the row
    *      } );
    *    } );
    *
    *  @example
    *    // Individual cell data
    *    $(document).ready(function() {
    *      oTable = $('#example').dataTable();
    *
    *      oTable.$('td').click( function () {
    *        var sData = oTable.fnGetData( this );
    *        alert( 'The cell clicked on had the value of '+sData );
    *      } );
    *    } );
    */
    this.fnGetData = function (mRow, iCol) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      if (mRow !== undefined) {
        var iRow = mRow;
        if (typeof mRow === 'object') {
          var sNode = mRow.nodeName.toLowerCase();
          if (sNode === "tr") {
            iRow = _fnNodeToDataIndex(oSettings, mRow);
          } else if (sNode === "td") {
            iRow = _fnNodeToDataIndex(oSettings, mRow.parentNode);
            iCol = _fnNodeToColumnIndex(oSettings, iRow, mRow);
          }
        }

        if (iCol !== undefined) {
          return _fnGetCellData(oSettings, iRow, iCol, '');
        }
        return (oSettings.aoData[iRow] !== undefined) ? oSettings.aoData[iRow]._aData : null;
      }
      return _fnGetDataMaster(oSettings);
    };


    /**
    * Get an array of the TR nodes that are used in the table's body. Note that you will 
    * typically want to use the '$' API method in preference to this as it is more 
    * flexible.
    *  @param {int} [iRow] Optional row index for the TR element you want
    *  @returns {array|node} If iRow is undefined, returns an array of all TR elements
    *    in the table's body, or iRow is defined, just the TR element requested.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Get the nodes from the table
    *      var nNodes = oTable.fnGetNodes( );
    *    } );
    */
    this.fnGetNodes = function (iRow) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      if (iRow !== undefined) {
        return (oSettings.aoData[iRow] !== undefined) ? oSettings.aoData[iRow].nTr : null;
      }
      return _fnGetTrNodes(oSettings);
    };


    /**
    * Get the array indexes of a particular cell from it's DOM element
    * and column index including hidden columns
    *  @param {node} nNode this can either be a TR, TD or TH in the table's body
    *  @returns {int} If nNode is given as a TR, then a single index is returned, or
    *    if given as a cell, an array of [row index, column index (visible)] is given.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      $('#example tbody td').click( function () {
    *        // Get the position of the current data from the node
    *        var aPos = oTable.fnGetPosition( this );
    *        
    *        // Get the data array for this row
    *        var aData = oTable.fnGetData( aPos[0] );
    *        
    *        // Update the data array and return the value
    *        aData[ aPos[1] ] = 'clicked';
    *        this.innerHTML = 'clicked';
    *      } );
    *      
    *      // Init DataTables
    *      oTable = $('#example').dataTable();
    *    } );
    */
    this.fnGetPosition = function (nNode) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var sNodeName = nNode.nodeName.toUpperCase();

      if (sNodeName == "TR") {
        return _fnNodeToDataIndex(oSettings, nNode);
      } else if (sNodeName == "TD" || sNodeName == "TH") {
        var iDataIndex = _fnNodeToDataIndex(oSettings, nNode.parentNode);
        var iColumnIndex = _fnNodeToColumnIndex(oSettings, iDataIndex, nNode);
        return [iDataIndex, _fnColumnIndexToVisible(oSettings, iColumnIndex), iColumnIndex];
      }
      return null;
    };


    /**
    * Check to see if a row is 'open' or not.
    *  @param {node} nTr the table row to check
    *  @returns {boolean} true if the row is currently open, false otherwise
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable;
    *      
    *      // 'open' an information row when a row is clicked on
    *      $('#example tbody tr').click( function () {
    *        if ( oTable.fnIsOpen(this) ) {
    *          oTable.fnClose( this );
    *        } else {
    *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
    *        }
    *      } );
    *      
    *      oTable = $('#example').dataTable();
    *    } );
    */
    this.fnIsOpen = function (nTr) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var aoOpenRows = oSettings.aoOpenRows;

      for (var i = 0; i < oSettings.aoOpenRows.length; i++) {
        if (oSettings.aoOpenRows[i].nParent == nTr) {
          return true;
        }
      }
      return false;
    };


    /**
    * This function will place a new row directly after a row which is currently 
    * on display on the page, with the HTML contents that is passed into the 
    * function. This can be used, for example, to ask for confirmation that a 
    * particular record should be deleted.
    *  @param {node} nTr The table row to 'open'
    *  @param {string|node|jQuery} mHtml The HTML to put into the row
    *  @param {string} sClass Class to give the new TD cell
    *  @returns {node} The row opened. Note that if the table row passed in as the
    *    first parameter, is not found in the table, this method will silently
    *    return.
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable;
    *      
    *      // 'open' an information row when a row is clicked on
    *      $('#example tbody tr').click( function () {
    *        if ( oTable.fnIsOpen(this) ) {
    *          oTable.fnClose( this );
    *        } else {
    *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
    *        }
    *      } );
    *      
    *      oTable = $('#example').dataTable();
    *    } );
    */
    this.fnOpen = function (nTr, mHtml, sClass) {
      /* Find settings from table node */
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);

      /* Check that the row given is in the table */
      var nTableRows = _fnGetTrNodes(oSettings);
      if ($.inArray(nTr, nTableRows) === -1) {
        return;
      }

      /* the old open one if there is one */
      this.fnClose(nTr);

      var nNewRow = document.createElement("tr");
      var nNewCell = document.createElement("td");
      nNewRow.appendChild(nNewCell);
      nNewCell.className = sClass;
      nNewCell.colSpan = _fnVisbleColumns(oSettings);

      if (typeof mHtml === "string") {
        nNewCell.innerHTML = mHtml;
      } else {
        $(nNewCell).html(mHtml);
      }

      /* If the nTr isn't on the page at the moment - then we don't insert at the moment */
      var nTrs = $('tr', oSettings.nTBody);
      if ($.inArray(nTr, nTrs) != -1) {
        $(nNewRow).insertAfter(nTr);
      }

      oSettings.aoOpenRows.push({
        "nTr": nNewRow,
        "nParent": nTr
      });

      return nNewRow;
    };


    /**
    * Change the pagination - provides the internal logic for pagination in a simple API 
    * function. With this function you can have a DataTables table go to the next, 
    * previous, first or last pages.
    *  @param {string|int} mAction Paging action to take: "first", "previous", "next" or "last"
    *    or page number to jump to (integer), note that page 0 is the first page.
    *  @param {bool} [bRedraw=true] Redraw the table or not
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      oTable.fnPageChange( 'next' );
    *    } );
    */
    this.fnPageChange = function (mAction, bRedraw) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      _fnPageChange(oSettings, mAction);
      _fnCalculateEnd(oSettings);

      if (bRedraw === undefined || bRedraw) {
        _fnDraw(oSettings);
      }
    };


    /**
    * Show a particular column
    *  @param {int} iCol The column whose display should be changed
    *  @param {bool} bShow Show (true) or hide (false) the column
    *  @param {bool} [bRedraw=true] Redraw the table or not
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Hide the second column after initialisation
    *      oTable.fnSetColumnVis( 1, false );
    *    } );
    */
    this.fnSetColumnVis = function (iCol, bShow, bRedraw) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var i, iLen;
      var aoColumns = oSettings.aoColumns;
      var aoData = oSettings.aoData;
      var nTd, bAppend, iBefore;

      /* No point in doing anything if we are requesting what is already true */
      if (aoColumns[iCol].bVisible == bShow) {
        return;
      }

      /* Show the column */
      if (bShow) {
        var iInsert = 0;
        for (i = 0; i < iCol; i++) {
          if (aoColumns[i].bVisible) {
            iInsert++;
          }
        }

        /* Need to decide if we should use appendChild or insertBefore */
        bAppend = (iInsert >= _fnVisbleColumns(oSettings));

        /* Which coloumn should we be inserting before? */
        if (!bAppend) {
          for (i = iCol; i < aoColumns.length; i++) {
            if (aoColumns[i].bVisible) {
              iBefore = i;
              break;
            }
          }
        }

        for (i = 0, iLen = aoData.length; i < iLen; i++) {
          if (aoData[i].nTr !== null) {
            if (bAppend) {
              aoData[i].nTr.appendChild(
                aoData[i]._anHidden[iCol]);
            } else {
              aoData[i].nTr.insertBefore(
                aoData[i]._anHidden[iCol], _fnGetTdNodes(oSettings, i)[iBefore]);
            }
          }
        }
      } else {
        /* Remove a column from display */
        for (i = 0, iLen = aoData.length; i < iLen; i++) {
          if (aoData[i].nTr !== null) {
            nTd = _fnGetTdNodes(oSettings, i)[iCol];
            aoData[i]._anHidden[iCol] = nTd;
            nTd.parentNode.removeChild(nTd);
          }
        }
      }

      /* Clear to set the visible flag */
      aoColumns[iCol].bVisible = bShow;

      /* Redraw the header and footer based on the new column visibility */
      _fnDrawHead(oSettings, oSettings.aoHeader);
      if (oSettings.nTFoot) {
        _fnDrawHead(oSettings, oSettings.aoFooter);
      }

      /* If there are any 'open' rows, then we need to alter the colspan for this col change */
      for (i = 0, iLen = oSettings.aoOpenRows.length; i < iLen; i++) {
        oSettings.aoOpenRows[i].nTr.colSpan = _fnVisbleColumns(oSettings);
      }

      /* Do a redraw incase anything depending on the table columns needs it 
      * (built-in: scrolling) 
      */
      if (bRedraw === undefined || bRedraw) {
        _fnAdjustColumnSizing(oSettings);
        _fnDraw(oSettings);
      }

      _fnSaveState(oSettings);
    };


    /**
    * Get the settings for a particular table for external manipulation
    *  @returns {object} DataTables settings object. See 
    *    {@link DataTable.models.oSettings}
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      var oSettings = oTable.fnSettings();
    *      
    *      // Show an example parameter from the settings
    *      alert( oSettings._iDisplayStart );
    *    } );
    */
    this.fnSettings = function () {
      return _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
    };


    /**
    * Sort the table by a particular column
    *  @param {int} iCol the data index to sort on. Note that this will not match the 
    *    'display index' if you have hidden data entries
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Sort immediately with columns 0 and 1
    *      oTable.fnSort( [ [0,'asc'], [1,'asc'] ] );
    *    } );
    */
    this.fnSort = function (aaSort) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      oSettings.aaSorting = aaSort;
      _fnSort(oSettings);
    };


    /**
    * Attach a sort listener to an element for a given column
    *  @param {node} nNode the element to attach the sort listener to
    *  @param {int} iColumn the column that a click on this node will sort on
    *  @param {function} [fnCallback] callback function when sort is run
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      
    *      // Sort on column 1, when 'sorter' is clicked on
    *      oTable.fnSortListener( document.getElementById('sorter'), 1 );
    *    } );
    */
    this.fnSortListener = function (nNode, iColumn, fnCallback) {
      _fnSortAttachListener(_fnSettingsFromNode(this[DataTable.ext.iApiIndex]), nNode, iColumn, fnCallback);
    };


    /**
    * Update a table cell or row - this method will accept either a single value to
    * update the cell with, an array of values with one element for each column or
    * an object in the same format as the original data source. The function is
    * self-referencing in order to make the multi column updates easier.
    *  @param {object|array|string} mData Data to update the cell/row with
    *  @param {node|int} mRow TR element you want to update or the aoData index
    *  @param {int} [iColumn] The column to update (not used of mData is an array or object)
    *  @param {bool} [bRedraw=true] Redraw the table or not
    *  @param {bool} [bAction=true] Perform pre-draw actions or not
    *  @returns {int} 0 on success, 1 on error
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      oTable.fnUpdate( 'Example update', 0, 0 ); // Single cell
    *      oTable.fnUpdate( ['a', 'b', 'c', 'd', 'e'], 1, 0 ); // Row
    *    } );
    */
    this.fnUpdate = function (mData, mRow, iColumn, bRedraw, bAction) {
      var oSettings = _fnSettingsFromNode(this[DataTable.ext.iApiIndex]);
      var i, iLen, sDisplay;
      var iRow = (typeof mRow === 'object') ? _fnNodeToDataIndex(oSettings, mRow) : mRow;

      if ($.isArray(mData) && iColumn === undefined) {
        /* Array update - update the whole row */
        oSettings.aoData[iRow]._aData = mData.slice();

        /* Flag to the function that we are recursing */
        for (i = 0; i < oSettings.aoColumns.length; i++) {
          this.fnUpdate(_fnGetCellData(oSettings, iRow, i), iRow, i, false, false);
        }
      } else if ($.isPlainObject(mData) && iColumn === undefined) {
        /* Object update - update the whole row - assume the developer gets the object right */
        oSettings.aoData[iRow]._aData = $.extend(true, {}, mData);

        for (i = 0; i < oSettings.aoColumns.length; i++) {
          this.fnUpdate(_fnGetCellData(oSettings, iRow, i), iRow, i, false, false);
        }
      } else {
        /* Individual cell update */
        _fnSetCellData(oSettings, iRow, iColumn, mData);
        sDisplay = _fnGetCellData(oSettings, iRow, iColumn, 'display');

        var oCol = oSettings.aoColumns[iColumn];
        if (oCol.fnRender !== null) {
          sDisplay = _fnRender(oSettings, iRow, iColumn);
          if (oCol.bUseRendered) {
            _fnSetCellData(oSettings, iRow, iColumn, sDisplay);
          }
        }

        if (oSettings.aoData[iRow].nTr !== null) {
          /* Do the actual HTML update */
          _fnGetTdNodes(oSettings, iRow)[iColumn].innerHTML = sDisplay;
        }
      }

      /* Modify the search index for this row (strictly this is likely not needed, since fnReDraw
      * will rebuild the search array - however, the redraw might be disabled by the user)
      */
      var iDisplayIndex = $.inArray(iRow, oSettings.aiDisplay);
      oSettings.asDataSearch[iDisplayIndex] = _fnBuildSearchRow(
        oSettings, _fnGetRowData(oSettings, iRow, 'filter', _fnGetColumns(oSettings, 'bSearchable')));

      /* Perform pre-draw actions */
      if (bAction === undefined || bAction) {
        _fnAdjustColumnSizing(oSettings);
      }

      /* Redraw the table */
      if (bRedraw === undefined || bRedraw) {
        _fnReDraw(oSettings);
      }
      return 0;
    };


    /**
    * Provide a common method for plug-ins to check the version of DataTables being used, in order
    * to ensure compatibility.
    *  @param {string} sVersion Version string to check for, in the format "X.Y.Z". Note that the
    *    formats "X" and "X.Y" are also acceptable.
    *  @returns {boolean} true if this version of DataTables is greater or equal to the required
    *    version, or false if this version of DataTales is not suitable
    *  @method
    *  @dtopt API
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      alert( oTable.fnVersionCheck( '1.9.0' ) );
    *    } );
    */
    this.fnVersionCheck = DataTable.ext.fnVersionCheck;


    /*
    * This is really a good bit rubbish this method of exposing the internal methods
    * publicly... - To be fixed in 2.0 using methods on the prototype
    */


    /**
    * Create a wrapper function for exporting an internal functions to an external API.
    *  @param {string} sFunc API function name
    *  @returns {function} wrapped function
    *  @memberof DataTable#oApi
    */
    function _fnExternApiFunc(sFunc) {
      return function () {
        var aArgs = [_fnSettingsFromNode(this[DataTable.ext.iApiIndex])].concat(
          Array.prototype.slice.call(arguments));
        return DataTable.ext.oApi[sFunc].apply(this, aArgs);
      };
    }


    /**
    * Reference to internal functions for use by plug-in developers. Note that these
    * methods are references to internal functions and are considered to be private.
    * If you use these methods, be aware that they are liable to change between versions
    * (check the upgrade notes).
    *  @namespace
    */
    this.oApi = {
      "_fnExternApiFunc": _fnExternApiFunc,
      "_fnInitialise": _fnInitialise,
      "_fnInitComplete": _fnInitComplete,
      "_fnLanguageCompat": _fnLanguageCompat,
      "_fnAddColumn": _fnAddColumn,
      "_fnColumnOptions": _fnColumnOptions,
      "_fnAddData": _fnAddData,
      "_fnCreateTr": _fnCreateTr,
      "_fnGatherData": _fnGatherData,
      "_fnBuildHead": _fnBuildHead,
      "_fnDrawHead": _fnDrawHead,
      "_fnDraw": _fnDraw,
      "_fnReDraw": _fnReDraw,
      "_fnAjaxUpdate": _fnAjaxUpdate,
      "_fnAjaxParameters": _fnAjaxParameters,
      "_fnAjaxUpdateDraw": _fnAjaxUpdateDraw,
      "_fnServerParams": _fnServerParams,
      "_fnAddOptionsHtml": _fnAddOptionsHtml,
      "_fnFeatureHtmlTable": _fnFeatureHtmlTable,
      "_fnScrollDraw": _fnScrollDraw,
      "_fnAdjustColumnSizing": _fnAdjustColumnSizing,
      "_fnFeatureHtmlFilter": _fnFeatureHtmlFilter,
      "_fnFilterComplete": _fnFilterComplete,
      "_fnFilterCustom": _fnFilterCustom,
      "_fnFilterColumn": _fnFilterColumn,
      "_fnFilter": _fnFilter,
      "_fnBuildSearchArray": _fnBuildSearchArray,
      "_fnBuildSearchRow": _fnBuildSearchRow,
      "_fnFilterCreateSearch": _fnFilterCreateSearch,
      "_fnDataToSearch": _fnDataToSearch,
      "_fnSort": _fnSort,
      "_fnSortAttachListener": _fnSortAttachListener,
      "_fnSortingClasses": _fnSortingClasses,
      "_fnFeatureHtmlPaginate": _fnFeatureHtmlPaginate,
      "_fnPageChange": _fnPageChange,
      "_fnFeatureHtmlInfo": _fnFeatureHtmlInfo,
      "_fnUpdateInfo": _fnUpdateInfo,
      "_fnFeatureHtmlLength": _fnFeatureHtmlLength,
      "_fnFeatureHtmlProcessing": _fnFeatureHtmlProcessing,
      "_fnProcessingDisplay": _fnProcessingDisplay,
      "_fnVisibleToColumnIndex": _fnVisibleToColumnIndex,
      "_fnColumnIndexToVisible": _fnColumnIndexToVisible,
      "_fnNodeToDataIndex": _fnNodeToDataIndex,
      "_fnVisbleColumns": _fnVisbleColumns,
      "_fnCalculateEnd": _fnCalculateEnd,
      "_fnConvertToWidth": _fnConvertToWidth,
      "_fnCalculateColumnWidths": _fnCalculateColumnWidths,
      "_fnScrollingWidthAdjust": _fnScrollingWidthAdjust,
      "_fnGetWidestNode": _fnGetWidestNode,
      "_fnGetMaxLenString": _fnGetMaxLenString,
      "_fnStringToCss": _fnStringToCss,
      "_fnDetectType": _fnDetectType,
      "_fnSettingsFromNode": _fnSettingsFromNode,
      "_fnGetDataMaster": _fnGetDataMaster,
      "_fnGetTrNodes": _fnGetTrNodes,
      "_fnGetTdNodes": _fnGetTdNodes,
      "_fnEscapeRegex": _fnEscapeRegex,
      "_fnDeleteIndex": _fnDeleteIndex,
      "_fnReOrderIndex": _fnReOrderIndex,
      "_fnColumnOrdering": _fnColumnOrdering,
      "_fnLog": _fnLog,
      "_fnClearTable": _fnClearTable,
      "_fnSaveState": _fnSaveState,
      "_fnLoadState": _fnLoadState,
      "_fnCreateCookie": _fnCreateCookie,
      "_fnReadCookie": _fnReadCookie,
      "_fnDetectHeader": _fnDetectHeader,
      "_fnGetUniqueThs": _fnGetUniqueThs,
      "_fnScrollBarWidth": _fnScrollBarWidth,
      "_fnApplyToChildren": _fnApplyToChildren,
      "_fnMap": _fnMap,
      "_fnGetRowData": _fnGetRowData,
      "_fnGetCellData": _fnGetCellData,
      "_fnSetCellData": _fnSetCellData,
      "_fnGetObjectDataFn": _fnGetObjectDataFn,
      "_fnSetObjectDataFn": _fnSetObjectDataFn,
      "_fnApplyColumnDefs": _fnApplyColumnDefs,
      "_fnBindAction": _fnBindAction,
      "_fnExtend": _fnExtend,
      "_fnCallbackReg": _fnCallbackReg,
      "_fnCallbackFire": _fnCallbackFire,
      "_fnJsonString": _fnJsonString,
      "_fnRender": _fnRender,
      "_fnNodeToColumnIndex": _fnNodeToColumnIndex,
      "_fnInfoMacros": _fnInfoMacros,
      "_fnBrowserDetect": _fnBrowserDetect,
      "_fnGetColumns": _fnGetColumns
    };

    $.extend(DataTable.ext.oApi, this.oApi);

    for (var sFunc in DataTable.ext.oApi) {
      if (sFunc) {
        this[sFunc] = _fnExternApiFunc(sFunc);
      }
    }


    var _that = this;
    return this.each(function () {

      var i = 0,
          iLen, j, jLen, k, kLen;
      var sId = this.getAttribute('id');
      var bInitHandedOff = false;
      var bUsePassedData = false;


      /* Sanity check */
      if (this.nodeName.toLowerCase() != 'table') {
        _fnLog(null, 0, "Attempted to initialise DataTables on a node which is not a " + "table: " + this.nodeName);
        return;
      }

      /* Check to see if we are re-initialising a table */
      for (i = 0, iLen = DataTable.settings.length; i < iLen; i++) {
        /* Base check on table node */
        if (DataTable.settings[i].nTable == this) {
          if (oInit === undefined || oInit.bRetrieve) {
            return DataTable.settings[i].oInstance;
          } else if (oInit.bDestroy) {
            DataTable.settings[i].oInstance.fnDestroy();
            break;
          } else {
            _fnLog(DataTable.settings[i], 0, "Cannot reinitialise DataTable.\n\n" + "To retrieve the DataTables object for this table, pass no arguments or see " + "the docs for bRetrieve and bDestroy");
            return;
          }
        }

        /* If the element we are initialising has the same ID as a table which was previously
        * initialised, but the table nodes don't match (from before) then we destroy the old
        * instance by simply deleting it. This is under the assumption that the table has been
        * destroyed by other methods. Anyone using non-id selectors will need to do this manually
        */
        if (DataTable.settings[i].sTableId == this.id) {
          DataTable.settings.splice(i, 1);
          break;
        }
      }

      /* Ensure the table has an ID - required for accessibility */
      if (sId === null || sId === "") {
        sId = "DataTables_Table_" + (DataTable.ext._oExternConfig.iNextUnique++);
        this.id = sId;
      }

      /* Create the settings object for this table and set some of the default parameters */
      var oSettings = $.extend(true, {}, DataTable.models.oSettings, {
        "nTable": this,
        "oApi": _that.oApi,
        "oInit": oInit,
        "sDestroyWidth": $(this).width(),
        "sInstance": sId,
        "sTableId": sId
      });
      DataTable.settings.push(oSettings);

      // Need to add the instance after the instance after the settings object has been added
      // to the settings array, so we can self reference the table instance if more than one
      oSettings.oInstance = (_that.length === 1) ? _that : $(this).dataTable();

      /* Setting up the initialisation object */
      if (!oInit) {
        oInit = {};
      }

      // Backwards compatibility, before we apply all the defaults
      if (oInit.oLanguage) {
        _fnLanguageCompat(oInit.oLanguage);
      }

      oInit = _fnExtend($.extend(true, {}, DataTable.defaults), oInit);

      // Map the initialisation options onto the settings object
      _fnMap(oSettings.oFeatures, oInit, "bPaginate");
      _fnMap(oSettings.oFeatures, oInit, "bLengthChange");
      _fnMap(oSettings.oFeatures, oInit, "bFilter");
      _fnMap(oSettings.oFeatures, oInit, "bSort");
      _fnMap(oSettings.oFeatures, oInit, "bInfo");
      _fnMap(oSettings.oFeatures, oInit, "bProcessing");
      _fnMap(oSettings.oFeatures, oInit, "bAutoWidth");
      _fnMap(oSettings.oFeatures, oInit, "bSortClasses");
      _fnMap(oSettings.oFeatures, oInit, "bServerSide");
      _fnMap(oSettings.oFeatures, oInit, "bDeferRender");
      _fnMap(oSettings.oScroll, oInit, "sScrollX", "sX");
      _fnMap(oSettings.oScroll, oInit, "sScrollXInner", "sXInner");
      _fnMap(oSettings.oScroll, oInit, "sScrollY", "sY");
      _fnMap(oSettings.oScroll, oInit, "bScrollCollapse", "bCollapse");
      _fnMap(oSettings.oScroll, oInit, "bScrollInfinite", "bInfinite");
      _fnMap(oSettings.oScroll, oInit, "iScrollLoadGap", "iLoadGap");
      _fnMap(oSettings.oScroll, oInit, "bScrollAutoCss", "bAutoCss");
      _fnMap(oSettings, oInit, "asStripeClasses");
      _fnMap(oSettings, oInit, "asStripClasses", "asStripeClasses"); // legacy
      _fnMap(oSettings, oInit, "fnServerData");
      _fnMap(oSettings, oInit, "fnFormatNumber");
      _fnMap(oSettings, oInit, "sServerMethod");
      _fnMap(oSettings, oInit, "aaSorting");
      _fnMap(oSettings, oInit, "aaSortingFixed");
      _fnMap(oSettings, oInit, "aLengthMenu");
      _fnMap(oSettings, oInit, "sPaginationType");
      _fnMap(oSettings, oInit, "sAjaxSource");
      _fnMap(oSettings, oInit, "sAjaxDataProp");
      _fnMap(oSettings, oInit, "iCookieDuration");
      _fnMap(oSettings, oInit, "sCookiePrefix");
      _fnMap(oSettings, oInit, "sDom");
      _fnMap(oSettings, oInit, "bSortCellsTop");
      _fnMap(oSettings, oInit, "iTabIndex");
      _fnMap(oSettings, oInit, "oSearch", "oPreviousSearch");
      _fnMap(oSettings, oInit, "aoSearchCols", "aoPreSearchCols");
      _fnMap(oSettings, oInit, "iDisplayLength", "_iDisplayLength");
      _fnMap(oSettings, oInit, "bJQueryUI", "bJUI");
      _fnMap(oSettings, oInit, "fnCookieCallback");
      _fnMap(oSettings, oInit, "fnStateLoad");
      _fnMap(oSettings, oInit, "fnStateSave");
      _fnMap(oSettings.oLanguage, oInit, "fnInfoCallback");

      /* Callback functions which are array driven */
      _fnCallbackReg(oSettings, 'aoDrawCallback', oInit.fnDrawCallback, 'user');
      _fnCallbackReg(oSettings, 'aoServerParams', oInit.fnServerParams, 'user');
      _fnCallbackReg(oSettings, 'aoStateSaveParams', oInit.fnStateSaveParams, 'user');
      _fnCallbackReg(oSettings, 'aoStateLoadParams', oInit.fnStateLoadParams, 'user');
      _fnCallbackReg(oSettings, 'aoStateLoaded', oInit.fnStateLoaded, 'user');
      _fnCallbackReg(oSettings, 'aoRowCallback', oInit.fnRowCallback, 'user');
      _fnCallbackReg(oSettings, 'aoRowCreatedCallback', oInit.fnCreatedRow, 'user');
      _fnCallbackReg(oSettings, 'aoHeaderCallback', oInit.fnHeaderCallback, 'user');
      _fnCallbackReg(oSettings, 'aoFooterCallback', oInit.fnFooterCallback, 'user');
      _fnCallbackReg(oSettings, 'aoInitComplete', oInit.fnInitComplete, 'user');
      _fnCallbackReg(oSettings, 'aoPreDrawCallback', oInit.fnPreDrawCallback, 'user');

      if (oSettings.oFeatures.bServerSide && oSettings.oFeatures.bSort && oSettings.oFeatures.bSortClasses) {
        /* Enable sort classes for server-side processing. Safe to do it here, since server-side
        * processing must be enabled by the developer
        */
        _fnCallbackReg(oSettings, 'aoDrawCallback', _fnSortingClasses, 'server_side_sort_classes');
      } else if (oSettings.oFeatures.bDeferRender) {
        _fnCallbackReg(oSettings, 'aoDrawCallback', _fnSortingClasses, 'defer_sort_classes');
      }

      if (oInit.bJQueryUI) {
        /* Use the JUI classes object for display. You could clone the oStdClasses object if 
        * you want to have multiple tables with multiple independent classes 
        */
        $.extend(oSettings.oClasses, DataTable.ext.oJUIClasses);

        if (oInit.sDom === DataTable.defaults.sDom && DataTable.defaults.sDom === "lfrtip") {
          /* Set the DOM to use a layout suitable for jQuery UI's theming */
          oSettings.sDom = '<"H"lfr>t<"F"ip>';
        }
      } else {
        $.extend(oSettings.oClasses, DataTable.ext.oStdClasses);
      }
      $(this).addClass(oSettings.oClasses.sTable);

      /* Calculate the scroll bar width and cache it for use later on */
      if (oSettings.oScroll.sX !== "" || oSettings.oScroll.sY !== "") {
        oSettings.oScroll.iBarWidth = _fnScrollBarWidth();
      }

      if (oSettings.iInitDisplayStart === undefined) {
        /* Display start point, taking into account the save saving */
        oSettings.iInitDisplayStart = oInit.iDisplayStart;
        oSettings._iDisplayStart = oInit.iDisplayStart;
      }

      /* Must be done after everything which can be overridden by a cookie! */
      if (oInit.bStateSave) {
        oSettings.oFeatures.bStateSave = true;
        _fnLoadState(oSettings, oInit);
        _fnCallbackReg(oSettings, 'aoDrawCallback', _fnSaveState, 'state_save');
      }

      if (oInit.iDeferLoading !== null) {
        oSettings.bDeferLoading = true;
        var tmp = $.isArray(oInit.iDeferLoading);
        oSettings._iRecordsDisplay = tmp ? oInit.iDeferLoading[0] : oInit.iDeferLoading;
        oSettings._iRecordsTotal = tmp ? oInit.iDeferLoading[1] : oInit.iDeferLoading;
      }

      if (oInit.aaData !== null) {
        bUsePassedData = true;
      }

      /* Language definitions */
      if (oInit.oLanguage.sUrl !== "") {
        /* Get the language definitions from a file - because this Ajax call makes the language
        * get async to the remainder of this function we use bInitHandedOff to indicate that 
        * _fnInitialise will be fired by the returned Ajax handler, rather than the constructor
        */
        oSettings.oLanguage.sUrl = oInit.oLanguage.sUrl;
        $.getJSON(oSettings.oLanguage.sUrl, null, function (json) {
          _fnLanguageCompat(json);
          $.extend(true, oSettings.oLanguage, oInit.oLanguage, json);
          _fnInitialise(oSettings);
        });
        bInitHandedOff = true;
      } else {
        $.extend(true, oSettings.oLanguage, oInit.oLanguage);
      }


      /*
      * Stripes
      */
      if (oInit.asStripeClasses === null) {
        oSettings.asStripeClasses = [
          oSettings.oClasses.sStripeOdd, oSettings.oClasses.sStripeEven];
      }

      /* Remove row stripe classes if they are already on the table row */
      var bStripeRemove = false;
      var anRows = $(this).children('tbody').children('tr');
      for (i = 0, iLen = oSettings.asStripeClasses.length; i < iLen; i++) {
        if (anRows.filter(":lt(2)").hasClass(oSettings.asStripeClasses[i])) {
          bStripeRemove = true;
          break;
        }
      }

      if (bStripeRemove) {
        /* Store the classes which we are about to remove so they can be re-added on destroy */
        oSettings.asDestroyStripes = ['', ''];
        if ($(anRows[0]).hasClass(oSettings.oClasses.sStripeOdd)) {
          oSettings.asDestroyStripes[0] += oSettings.oClasses.sStripeOdd + " ";
        }
        if ($(anRows[0]).hasClass(oSettings.oClasses.sStripeEven)) {
          oSettings.asDestroyStripes[0] += oSettings.oClasses.sStripeEven;
        }
        if ($(anRows[1]).hasClass(oSettings.oClasses.sStripeOdd)) {
          oSettings.asDestroyStripes[1] += oSettings.oClasses.sStripeOdd + " ";
        }
        if ($(anRows[1]).hasClass(oSettings.oClasses.sStripeEven)) {
          oSettings.asDestroyStripes[1] += oSettings.oClasses.sStripeEven;
        }

        anRows.removeClass(oSettings.asStripeClasses.join(' '));
      }


      /*
      * Columns
      * See if we should load columns automatically or use defined ones
      */
      var anThs = [];
      var aoColumnsInit;
      var nThead = this.getElementsByTagName('thead');
      if (nThead.length !== 0) {
        _fnDetectHeader(oSettings.aoHeader, nThead[0]);
        anThs = _fnGetUniqueThs(oSettings);
      }

      /* If not given a column array, generate one with nulls */
      if (oInit.aoColumns === null) {
        aoColumnsInit = [];
        for (i = 0, iLen = anThs.length; i < iLen; i++) {
          aoColumnsInit.push(null);
        }
      } else {
        aoColumnsInit = oInit.aoColumns;
      }

      /* Add the columns */
      for (i = 0, iLen = aoColumnsInit.length; i < iLen; i++) {
        /* Short cut - use the loop to check if we have column visibility state to restore */
        if (oInit.saved_aoColumns !== undefined && oInit.saved_aoColumns.length == iLen) {
          if (aoColumnsInit[i] === null) {
            aoColumnsInit[i] = {};
          }
          aoColumnsInit[i].bVisible = oInit.saved_aoColumns[i].bVisible;
        }

        _fnAddColumn(oSettings, anThs ? anThs[i] : null);
      }

      /* Apply the column definitions */
      _fnApplyColumnDefs(oSettings, oInit.aoColumnDefs, aoColumnsInit, function (iCol, oDef) {
        _fnColumnOptions(oSettings, iCol, oDef);
      });


      /*
      * Sorting
      * Check the aaSorting array
      */
      for (i = 0, iLen = oSettings.aaSorting.length; i < iLen; i++) {
        if (oSettings.aaSorting[i][0] >= oSettings.aoColumns.length) {
          oSettings.aaSorting[i][0] = 0;
        }
        var oColumn = oSettings.aoColumns[oSettings.aaSorting[i][0]];

        /* Add a default sorting index */
        if (oSettings.aaSorting[i][2] === undefined) {
          oSettings.aaSorting[i][2] = 0;
        }

        /* If aaSorting is not defined, then we use the first indicator in asSorting */
        if (oInit.aaSorting === undefined && oSettings.saved_aaSorting === undefined) {
          oSettings.aaSorting[i][1] = oColumn.asSorting[0];
        }

        /* Set the current sorting index based on aoColumns.asSorting */
        for (j = 0, jLen = oColumn.asSorting.length; j < jLen; j++) {
          if (oSettings.aaSorting[i][1] == oColumn.asSorting[j]) {
            oSettings.aaSorting[i][2] = j;
            break;
          }
        }
      }

      /* Do a first pass on the sorting classes (allows any size changes to be taken into
      * account, and also will apply sorting disabled classes if disabled
      */
      _fnSortingClasses(oSettings);


      /*
      * Final init
      * Cache the header, body and footer as required, creating them if needed
      */

      /* Browser support detection */
      _fnBrowserDetect(oSettings);

      // Work around for Webkit bug 83867 - store the caption-side before removing from doc
      var captions = $(this).children('caption').each(function () {
        this._captionSide = $(this).css('caption-side');
      });

      var thead = $(this).children('thead');
      if (thead.length === 0) {
        thead = [document.createElement('thead')];
        this.appendChild(thead[0]);
      }
      oSettings.nTHead = thead[0];

      var tbody = $(this).children('tbody');
      if (tbody.length === 0) {
        tbody = [document.createElement('tbody')];
        this.appendChild(tbody[0]);
      }
      oSettings.nTBody = tbody[0];
      oSettings.nTBody.setAttribute("role", "alert");
      oSettings.nTBody.setAttribute("aria-live", "polite");
      oSettings.nTBody.setAttribute("aria-relevant", "all");

      var tfoot = $(this).children('tfoot');
      if (tfoot.length === 0 && captions.length > 0 && (oSettings.oScroll.sX !== "" || oSettings.oScroll.sY !== "")) {
        // If we are a scrolling table, and no footer has been given, then we need to create
        // a tfoot element for the caption element to be appended to
        tfoot = [document.createElement('tfoot')];
        this.appendChild(tfoot[0]);
      }

      if (tfoot.length > 0) {
        oSettings.nTFoot = tfoot[0];
        _fnDetectHeader(oSettings.aoFooter, oSettings.nTFoot);
      }

      /* Check if there is data passing into the constructor */
      if (bUsePassedData) {
        for (i = 0; i < oInit.aaData.length; i++) {
          _fnAddData(oSettings, oInit.aaData[i]);
        }
      } else {
        /* Grab the data from the page */
        _fnGatherData(oSettings);
      }

      /* Copy the data index array */
      oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();

      /* Initialisation complete - table can be drawn */
      oSettings.bInitialised = true;

      /* Check if we need to initialise the table (it might not have been handed off to the
      * language processor)
      */
      if (bInitHandedOff === false) {
        _fnInitialise(oSettings);
      }
    });
  };



  /**
  * Provide a common method for plug-ins to check the version of DataTables being used, in order
  * to ensure compatibility.
  *  @param {string} sVersion Version string to check for, in the format "X.Y.Z". Note that the
  *    formats "X" and "X.Y" are also acceptable.
  *  @returns {boolean} true if this version of DataTables is greater or equal to the required
  *    version, or false if this version of DataTales is not suitable
  *  @static
  *  @dtopt API-Static
  *
  *  @example
  *    alert( $.fn.dataTable.fnVersionCheck( '1.9.0' ) );
  */
  DataTable.fnVersionCheck = function (sVersion) {
    /* This is cheap, but effective */
    var fnZPad = function (Zpad, count) {
      while (Zpad.length < count) {
        Zpad += '0';
      }
      return Zpad;
    };
    var aThis = DataTable.ext.sVersion.split('.');
    var aThat = sVersion.split('.');
    var sThis = '',
      sThat = '';

    for (var i = 0, iLen = aThat.length; i < iLen; i++) {
      sThis += fnZPad(aThis[i], 3);
      sThat += fnZPad(aThat[i], 3);
    }

    return parseInt(sThis, 10) >= parseInt(sThat, 10);
  };


  /**
  * Check if a TABLE node is a DataTable table already or not.
  *  @param {node} nTable The TABLE node to check if it is a DataTable or not (note that other
  *    node types can be passed in, but will always return false).
  *  @returns {boolean} true the table given is a DataTable, or false otherwise
  *  @static
  *  @dtopt API-Static
  *
  *  @example
  *    var ex = document.getElementById('example');
  *    if ( ! $.fn.DataTable.fnIsDataTable( ex ) ) {
  *      $(ex).dataTable();
  *    }
  */
  DataTable.fnIsDataTable = function (nTable) {
    var o = DataTable.settings;

    for (var i = 0; i < o.length; i++) {
      if (o[i].nTable === nTable || o[i].nScrollHead === nTable || o[i].nScrollFoot === nTable) {
        return true;
      }
    }

    return false;
  };


  /**
  * Get all DataTable tables that have been initialised - optionally you can select to
  * get only currently visible tables.
  *  @param {boolean} [bVisible=false] Flag to indicate if you want all (default) or 
  *    visible tables only.
  *  @returns {array} Array of TABLE nodes (not DataTable instances) which are DataTables
  *  @static
  *  @dtopt API-Static
  *
  *  @example
  *    var table = $.fn.dataTable.fnTables(true);
  *    if ( table.length > 0 ) {
  *      $(table).dataTable().fnAdjustColumnSizing();
  *    }
  */
  DataTable.fnTables = function (bVisible) {
    var out = [];

    jQuery.each(DataTable.settings, function (i, o) {
      if (!bVisible || (bVisible === true && $(o.nTable).is(':visible'))) {
        out.push(o.nTable);
      }
    });

    return out;
  };


  /**
  * Version string for plug-ins to check compatibility. Allowed format is
  * a.b.c.d.e where: a:int, b:int, c:int, d:string(dev|beta), e:int. d and
  * e are optional
  *  @member
  *  @type string
  *  @default Version number
  */
  DataTable.version = "1.9.3";

  /**
  * Private data store, containing all of the settings objects that are created for the
  * tables on a given page.
  * 
  * Note that the <i>DataTable.settings</i> object is aliased to <i>jQuery.fn.dataTableExt</i> 
  * through which it may be accessed and manipulated, or <i>jQuery.fn.dataTable.settings</i>.
  *  @member
  *  @type array
  *  @default []
  *  @private
  */
  DataTable.settings = [];

  /**
  * Object models container, for the various models that DataTables has available
  * to it. These models define the objects that are used to hold the active state 
  * and configuration of the table.
  *  @namespace
  */
  DataTable.models = {};


  /**
  * DataTables extension options and plug-ins. This namespace acts as a collection "area"
  * for plug-ins that can be used to extend the default DataTables behaviour - indeed many
  * of the build in methods use this method to provide their own capabilities (sorting methods
  * for example).
  * 
  * Note that this namespace is aliased to jQuery.fn.dataTableExt so it can be readily accessed
  * and modified by plug-ins.
  *  @namespace
  */
  DataTable.models.ext = {
    /**
    * Plug-in filtering functions - this method of filtering is complimentary to the default
    * type based filtering, and a lot more comprehensive as it allows you complete control
    * over the filtering logic. Each element in this array is a function (parameters
    * described below) that is called for every row in the table, and your logic decides if
    * it should be included in the filtered data set or not.
    *   <ul>
    *     <li>
    *       Function input parameters:
    *       <ul>
    *         <li>{object} DataTables settings object: see {@link DataTable.models.oSettings}.</li>
    *         <li>{array|object} Data for the row to be processed (same as the original format
    *           that was passed in as the data source, or an array from a DOM data source</li>
    *         <li>{int} Row index in aoData ({@link DataTable.models.oSettings.aoData}), which can
    *           be useful to retrieve the TR element if you need DOM interaction.</li>
    *       </ul>
    *     </li>
    *     <li>
    *       Function return:
    *       <ul>
    *         <li>{boolean} Include the row in the filtered result set (true) or not (false)</li>
    *       </ul>
    *     </il>
    *   </ul>
    *  @type array
    *  @default []
    *
    *  @example
    *    // The following example shows custom filtering being applied to the fourth column (i.e.
    *    // the aData[3] index) based on two input values from the end-user, matching the data in 
    *    // a certain range.
    *    $.fn.dataTableExt.afnFiltering.push(
    *      function( oSettings, aData, iDataIndex ) {
    *        var iMin = document.getElementById('min').value * 1;
    *        var iMax = document.getElementById('max').value * 1;
    *        var iVersion = aData[3] == "-" ? 0 : aData[3]*1;
    *        if ( iMin == "" && iMax == "" ) {
    *          return true;
    *        }
    *        else if ( iMin == "" && iVersion < iMax ) {
    *          return true;
    *        }
    *        else if ( iMin < iVersion && "" == iMax ) {
    *          return true;
    *        }
    *        else if ( iMin < iVersion && iVersion < iMax ) {
    *          return true;
    *        }
    *        return false;
    *      }
    *    );
    */
    "afnFiltering": [],


    /**
    * Plug-in sorting functions - this method of sorting is complimentary to the default type
    * based sorting that DataTables does automatically, allowing much greater control over the
    * the data that is being used to sort a column. This is useful if you want to do sorting
    * based on live data (for example the contents of an 'input' element) rather than just the
    * static string that DataTables knows of. The way these plug-ins work is that you create
    * an array of the values you wish to be sorted for the column in question and then return
    * that array. Which pre-sorting function is run here depends on the sSortDataType parameter
    * that is used for the column (if any). This is the corollary of <i>ofnSearch</i> for sort 
    * data.
    *   <ul>
    *     <li>
    *       Function input parameters:
    *       <ul>
    *         <li>{object} DataTables settings object: see {@link DataTable.models.oSettings}.</li>
    *         <li>{int} Target column index</li>
    *       </ul>
    *     </li>
    *     <li>
    *       Function return:
    *       <ul>
    *         <li>{array} Data for the column to be sorted upon</li>
    *       </ul>
    *     </il>
    *   </ul>
    *  
    * Note that as of v1.9, it is typically preferable to use <i>mData</i> to prepare data for
    * the different uses that DataTables can put the data to. Specifically <i>mData</i> when
    * used as a function will give you a 'type' (sorting, filtering etc) that you can use to 
    * prepare the data as required for the different types. As such, this method is deprecated.
    *  @type array
    *  @default []
    *  @deprecated
    *
    *  @example
    *    // Updating the cached sorting information with user entered values in HTML input elements
    *    jQuery.fn.dataTableExt.afnSortData['dom-text'] = function ( oSettings, iColumn )
    *    {
    *      var aData = [];
    *      $( 'td:eq('+iColumn+') input', oSettings.oApi._fnGetTrNodes(oSettings) ).each( function () {
    *        aData.push( this.value );
    *      } );
    *      return aData;
    *    }
    */
    "afnSortData": [],


    /**
    * Feature plug-ins - This is an array of objects which describe the feature plug-ins that are
    * available to DataTables. These feature plug-ins are accessible through the sDom initialisation
    * option. As such, each feature plug-in must describe a function that is used to initialise
    * itself (fnInit), a character so the feature can be enabled by sDom (cFeature) and the name
    * of the feature (sFeature). Thus the objects attached to this method must provide:
    *   <ul>
    *     <li>{function} fnInit Initialisation of the plug-in
    *       <ul>
    *         <li>
    *           Function input parameters:
    *           <ul>
    *             <li>{object} DataTables settings object: see {@link DataTable.models.oSettings}.</li>
    *           </ul>
    *         </li>
    *         <li>
    *           Function return:
    *           <ul>
    *             <li>{node|null} The element which contains your feature. Note that the return
    *                may also be void if your plug-in does not require to inject any DOM elements 
    *                into DataTables control (sDom) - for example this might be useful when 
    *                developing a plug-in which allows table control via keyboard entry.</li>
    *           </ul>
    *         </il>
    *       </ul>
    *     </li>
    *     <li>{character} cFeature Character that will be matched in sDom - case sensitive</li>
    *     <li>{string} sFeature Feature name</li>
    *   </ul>
    *  @type array
    *  @default []
    * 
    *  @example
    *    // How TableTools initialises itself.
    *    $.fn.dataTableExt.aoFeatures.push( {
    *      "fnInit": function( oSettings ) {
    *        return new TableTools( { "oDTSettings": oSettings } );
    *      },
    *      "cFeature": "T",
    *      "sFeature": "TableTools"
    *    } );
    */
    "aoFeatures": [],


    /**
    * Type detection plug-in functions - DataTables utilises types to define how sorting and
    * filtering behave, and types can be either  be defined by the developer (sType for the
    * column) or they can be automatically detected by the methods in this array. The functions
    * defined in the array are quite simple, taking a single parameter (the data to analyse) 
    * and returning the type if it is a known type, or null otherwise.
    *   <ul>
    *     <li>
    *       Function input parameters:
    *       <ul>
    *         <li>{*} Data from the column cell to be analysed</li>
    *       </ul>
    *     </li>
    *     <li>
    *       Function return:
    *       <ul>
    *         <li>{string|null} Data type detected, or null if unknown (and thus pass it
    *           on to the other type detection functions.</li>
    *       </ul>
    *     </il>
    *   </ul>
    *  @type array
    *  @default []
    *  
    *  @example
    *    // Currency type detection plug-in:
    *    jQuery.fn.dataTableExt.aTypes.push(
    *      function ( sData ) {
    *        var sValidChars = "0123456789.-";
    *        var Char;
    *        
    *        // Check the numeric part
    *        for ( i=1 ; i<sData.length ; i++ ) {
    *          Char = sData.charAt(i); 
    *          if (sValidChars.indexOf(Char) == -1) {
    *            return null;
    *          }
    *        }
    *        
    *        // Check prefixed by currency
    *        if ( sData.charAt(0) == '$' || sData.charAt(0) == '&pound;' ) {
    *          return 'currency';
    *        }
    *        return null;
    *      }
    *    );
    */
    "aTypes": [],


    /**
    * Provide a common method for plug-ins to check the version of DataTables being used, 
    * in order to ensure compatibility.
    *  @type function
    *  @param {string} sVersion Version string to check for, in the format "X.Y.Z". Note 
    *    that the formats "X" and "X.Y" are also acceptable.
    *  @returns {boolean} true if this version of DataTables is greater or equal to the 
    *    required version, or false if this version of DataTales is not suitable
    *
    *  @example
    *    $(document).ready(function() {
    *      var oTable = $('#example').dataTable();
    *      alert( oTable.fnVersionCheck( '1.9.0' ) );
    *    } );
    */
    "fnVersionCheck": DataTable.fnVersionCheck,


    /**
    * Index for what 'this' index API functions should use
    *  @type int
    *  @default 0
    */
    "iApiIndex": 0,


    /**
    * Pre-processing of filtering data plug-ins - When you assign the sType for a column
    * (or have it automatically detected for you by DataTables or a type detection plug-in), 
    * you will typically be using this for custom sorting, but it can also be used to provide 
    * custom filtering by allowing you to pre-processing the data and returning the data in
    * the format that should be filtered upon. This is done by adding functions this object 
    * with a parameter name which matches the sType for that target column. This is the
    * corollary of <i>afnSortData</i> for filtering data.
    *   <ul>
    *     <li>
    *       Function input parameters:
    *       <ul>
    *         <li>{*} Data from the column cell to be prepared for filtering</li>
    *       </ul>
    *     </li>
    *     <li>
    *       Function return:
    *       <ul>
    *         <li>{string|null} Formatted string that will be used for the filtering.</li>
    *       </ul>
    *     </il>
    *   </ul>
    * 
    * Note that as of v1.9, it is typically preferable to use <i>mData</i> to prepare data for
    * the different uses that DataTables can put the data to. Specifically <i>mData</i> when
    * used as a function will give you a 'type' (sorting, filtering etc) that you can use to 
    * prepare the data as required for the different types. As such, this method is deprecated.
    *  @type object
    *  @default {}
    *  @deprecated
    *
    *  @example
    *    $.fn.dataTableExt.ofnSearch['title-numeric'] = function ( sData ) {
    *      return sData.replace(/\n/g," ").replace( /<.*?>/g, "" );
    *    }
    */
    "ofnSearch": {},


    /**
    * Container for all private functions in DataTables so they can be exposed externally
    *  @type object
    *  @default {}
    */
    "oApi": {},


    /**
    * Storage for the various classes that DataTables uses
    *  @type object
    *  @default {}
    */
    "oStdClasses": {},


    /**
    * Storage for the various classes that DataTables uses - jQuery UI suitable
    *  @type object
    *  @default {}
    */
    "oJUIClasses": {},


    /**
    * Pagination plug-in methods - The style and controls of the pagination can significantly 
    * impact on how the end user interacts with the data in your table, and DataTables allows 
    * the addition of pagination controls by extending this object, which can then be enabled
    * through the <i>sPaginationType</i> initialisation parameter. Each pagination type that
    * is added is an object (the property name of which is what <i>sPaginationType</i> refers
    * to) that has two properties, both methods that are used by DataTables to update the
    * control's state.
    *   <ul>
    *     <li>
    *       fnInit -  Initialisation of the paging controls. Called only during initialisation 
    *         of the table. It is expected that this function will add the required DOM elements 
    *         to the page for the paging controls to work. The element pointer 
    *         'oSettings.aanFeatures.p' array is provided by DataTables to contain the paging 
    *         controls (note that this is a 2D array to allow for multiple instances of each 
    *         DataTables DOM element). It is suggested that you add the controls to this element 
    *         as children
    *       <ul>
    *         <li>
    *           Function input parameters:
    *           <ul>
    *             <li>{object} DataTables settings object: see {@link DataTable.models.oSettings}.</li>
    *             <li>{node} Container into which the pagination controls must be inserted</li>
    *             <li>{function} Draw callback function - whenever the controls cause a page
    *               change, this method must be called to redraw the table.</li>
    *           </ul>
    *         </li>
    *         <li>
    *           Function return:
    *           <ul>
    *             <li>No return required</li>
    *           </ul>
    *         </il>
    *       </ul>
    *     </il>
    *     <li>
    *       fnInit -  This function is called whenever the paging status of the table changes and is
    *         typically used to update classes and/or text of the paging controls to reflex the new 
    *         status.
    *       <ul>
    *         <li>
    *           Function input parameters:
    *           <ul>
    *             <li>{object} DataTables settings object: see {@link DataTable.models.oSettings}.</li>
    *             <li>{function} Draw callback function - in case you need to redraw the table again
    *               or attach new event listeners</li>
    *           </ul>
    *         </li>
    *         <li>
    *           Function return:
    *           <ul>
    *             <li>No return required</li>
    *           </ul>
    *         </il>
    *       </ul>
    *     </il>
    *   </ul>
    *  @type object
    *  @default {}
    *
    *  @example
    *    $.fn.dataTableExt.oPagination.four_button = {
    *      "fnInit": function ( oSettings, nPaging, fnCallbackDraw ) {
    *        nFirst = document.createElement( 'span' );
    *        nPrevious = document.createElement( 'span' );
    *        nNext = document.createElement( 'span' );
    *        nLast = document.createElement( 'span' );
    *        
    *        nFirst.appendChild( document.createTextNode( oSettings.oLanguage.oPaginate.sFirst ) );
    *        nPrevious.appendChild( document.createTextNode( oSettings.oLanguage.oPaginate.sPrevious ) );
    *        nNext.appendChild( document.createTextNode( oSettings.oLanguage.oPaginate.sNext ) );
    *        nLast.appendChild( document.createTextNode( oSettings.oLanguage.oPaginate.sLast ) );
    *        
    *        nFirst.className = "paginate_button first";
    *        nPrevious.className = "paginate_button previous";
    *        nNext.className="paginate_button next";
    *        nLast.className = "paginate_button last";
    *        
    *        nPaging.appendChild( nFirst );
    *        nPaging.appendChild( nPrevious );
    *        nPaging.appendChild( nNext );
    *        nPaging.appendChild( nLast );
    *        
    *        $(nFirst).click( function () {
    *          oSettings.oApi._fnPageChange( oSettings, "first" );
    *          fnCallbackDraw( oSettings );
    *        } );
    *        
    *        $(nPrevious).click( function() {
    *          oSettings.oApi._fnPageChange( oSettings, "previous" );
    *          fnCallbackDraw( oSettings );
    *        } );
    *        
    *        $(nNext).click( function() {
    *          oSettings.oApi._fnPageChange( oSettings, "next" );
    *          fnCallbackDraw( oSettings );
    *        } );
    *        
    *        $(nLast).click( function() {
    *          oSettings.oApi._fnPageChange( oSettings, "last" );
    *          fnCallbackDraw( oSettings );
    *        } );
    *        
    *        $(nFirst).bind( 'selectstart', function () { return false; } );
    *        $(nPrevious).bind( 'selectstart', function () { return false; } );
    *        $(nNext).bind( 'selectstart', function () { return false; } );
    *        $(nLast).bind( 'selectstart', function () { return false; } );
    *      },
    *      
    *      "fnUpdate": function ( oSettings, fnCallbackDraw ) {
    *        if ( !oSettings.aanFeatures.p ) {
    *          return;
    *        }
    *        
    *        // Loop over each instance of the pager
    *        var an = oSettings.aanFeatures.p;
    *        for ( var i=0, iLen=an.length ; i<iLen ; i++ ) {
    *          var buttons = an[i].getElementsByTagName('span');
    *          if ( oSettings._iDisplayStart === 0 ) {
    *            buttons[0].className = "paginate_disabled_previous";
    *            buttons[1].className = "paginate_disabled_previous";
    *          }
    *          else {
    *            buttons[0].className = "paginate_enabled_previous";
    *            buttons[1].className = "paginate_enabled_previous";
    *          }
    *          
    *          if ( oSettings.fnDisplayEnd() == oSettings.fnRecordsDisplay() ) {
    *            buttons[2].className = "paginate_disabled_next";
    *            buttons[3].className = "paginate_disabled_next";
    *          }
    *          else {
    *            buttons[2].className = "paginate_enabled_next";
    *            buttons[3].className = "paginate_enabled_next";
    *          }
    *        }
    *      }
    *    };
    */
    "oPagination": {},


    /**
    * Sorting plug-in methods - Sorting in DataTables is based on the detected type of the
    * data column (you can add your own type detection functions, or override automatic 
    * detection using sType). With this specific type given to the column, DataTables will 
    * apply the required sort from the functions in the object. Each sort type must provide
    * two mandatory methods, one each for ascending and descending sorting, and can optionally
    * provide a pre-formatting method that will help speed up sorting by allowing DataTables
    * to pre-format the sort data only once (rather than every time the actual sort functions
    * are run). The two sorting functions are typical Javascript sort methods:
    *   <ul>
    *     <li>
    *       Function input parameters:
    *       <ul>
    *         <li>{*} Data to compare to the second parameter</li>
    *         <li>{*} Data to compare to the first parameter</li>
    *       </ul>
    *     </li>
    *     <li>
    *       Function return:
    *       <ul>
    *         <li>{int} Sorting match: <0 if first parameter should be sorted lower than
    *           the second parameter, ===0 if the two parameters are equal and >0 if
    *           the first parameter should be sorted height than the second parameter.</li>
    *       </ul>
    *     </il>
    *   </ul>
    *  @type object
    *  @default {}
    *
    *  @example
    *    // Case-sensitive string sorting, with no pre-formatting method
    *    $.extend( $.fn.dataTableExt.oSort, {
    *      "string-case-asc": function(x,y) {
    *        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    *      },
    *      "string-case-desc": function(x,y) {
    *        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    *      }
    *    } );
    *
    *  @example
    *    // Case-insensitive string sorting, with pre-formatting
    *    $.extend( $.fn.dataTableExt.oSort, {
    *      "string-pre": function(x) {
    *        return x.toLowerCase();
    *      },
    *      "string-asc": function(x,y) {
    *        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    *      },
    *      "string-desc": function(x,y) {
    *        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    *      }
    *    } );
    */
    "oSort": {},


    /**
    * Version string for plug-ins to check compatibility. Allowed format is
    * a.b.c.d.e where: a:int, b:int, c:int, d:string(dev|beta), e:int. d and
    * e are optional
    *  @type string
    *  @default Version number
    */
    "sVersion": DataTable.version,


    /**
    * How should DataTables report an error. Can take the value 'alert' or 'throw'
    *  @type string
    *  @default alert
    */
    "sErrMode": "alert",


    /**
    * Store information for DataTables to access globally about other instances
    *  @namespace
    *  @private
    */
    "_oExternConfig": {
      /* int:iNextUnique - next unique number for an instance */"iNextUnique": 0
    }
  };




  /**
  * Template object for the way in which DataTables holds information about
  * search information for the global filter and individual column filters.
  *  @namespace
  */
  DataTable.models.oSearch = {
    /**
    * Flag to indicate if the filtering should be case insensitive or not
    *  @type boolean
    *  @default true
    */
    "bCaseInsensitive": true,

    /**
    * Applied search term
    *  @type string
    *  @default <i>Empty string</i>
    */
    "sSearch": "",

    /**
    * Flag to indicate if the search term should be interpreted as a
    * regular expression (true) or not (false) and therefore and special
    * regex characters escaped.
    *  @type boolean
    *  @default false
    */
    "bRegex": false,

    /**
    * Flag to indicate if DataTables is to use its smart filtering or not.
    *  @type boolean
    *  @default true
    */
    "bSmart": true
  };




  /**
  * Template object for the way in which DataTables holds information about
  * each individual row. This is the object format used for the settings 
  * aoData array.
  *  @namespace
  */
  DataTable.models.oRow = {
    /**
    * TR element for the row
    *  @type node
    *  @default null
    */
    "nTr": null,

    /**
    * Data object from the original data source for the row. This is either
    * an array if using the traditional form of DataTables, or an object if
    * using mData options. The exact type will depend on the passed in
    * data from the data source, or will be an array if using DOM a data 
    * source.
    *  @type array|object
    *  @default []
    */
    "_aData": [],

    /**
    * Sorting data cache - this array is ostensibly the same length as the
    * number of columns (although each index is generated only as it is 
    * needed), and holds the data that is used for sorting each column in the
    * row. We do this cache generation at the start of the sort in order that
    * the formatting of the sort data need be done only once for each cell
    * per sort. This array should not be read from or written to by anything
    * other than the master sorting methods.
    *  @type array
    *  @default []
    *  @private
    */
    "_aSortData": [],

    /**
    * Array of TD elements that are cached for hidden rows, so they can be
    * reinserted into the table if a column is made visible again (or to act
    * as a store if a column is made hidden). Only hidden columns have a 
    * reference in the array. For non-hidden columns the value is either
    * undefined or null.
    *  @type array nodes
    *  @default []
    *  @private
    */
    "_anHidden": [],

    /**
    * Cache of the class name that DataTables has applied to the row, so we
    * can quickly look at this variable rather than needing to do a DOM check
    * on className for the nTr property.
    *  @type string
    *  @default <i>Empty string</i>
    *  @private
    */
    "_sRowStripe": ""
  };



  /**
  * Template object for the column information object in DataTables. This object
  * is held in the settings aoColumns array and contains all the information that
  * DataTables needs about each individual column.
  * 
  * Note that this object is related to {@link DataTable.defaults.columns} 
  * but this one is the internal data store for DataTables's cache of columns.
  * It should NOT be manipulated outside of DataTables. Any configuration should
  * be done through the initialisation options.
  *  @namespace
  */
  DataTable.models.oColumn = {
    /**
    * A list of the columns that sorting should occur on when this column
    * is sorted. That this property is an array allows multi-column sorting
    * to be defined for a column (for example first name / last name columns
    * would benefit from this). The values are integers pointing to the
    * columns to be sorted on (typically it will be a single integer pointing
    * at itself, but that doesn't need to be the case).
    *  @type array
    */
    "aDataSort": null,

    /**
    * Define the sorting directions that are applied to the column, in sequence
    * as the column is repeatedly sorted upon - i.e. the first value is used
    * as the sorting direction when the column if first sorted (clicked on).
    * Sort it again (click again) and it will move on to the next index.
    * Repeat until loop.
    *  @type array
    */
    "asSorting": null,

    /**
    * Flag to indicate if the column is searchable, and thus should be included
    * in the filtering or not.
    *  @type boolean
    */
    "bSearchable": null,

    /**
    * Flag to indicate if the column is sortable or not.
    *  @type boolean
    */
    "bSortable": null,

    /**
    * When using fnRender, you have two options for what to do with the data,
    * and this property serves as the switch. Firstly, you can have the sorting
    * and filtering use the rendered value (true - default), or you can have
    * the sorting and filtering us the original value (false).
    * 
    * *NOTE* It is it is advisable now to use mData as a function and make 
    * use of the 'type' that it gives, allowing (potentially) different data to
    * be used for sorting, filtering, display and type detection.
    *  @type boolean
    *  @deprecated
    */
    "bUseRendered": null,

    /**
    * Flag to indicate if the column is currently visible in the table or not
    *  @type boolean
    */
    "bVisible": null,

    /**
    * Flag to indicate to the type detection method if the automatic type
    * detection should be used, or if a column type (sType) has been specified
    *  @type boolean
    *  @default true
    *  @private
    */
    "_bAutoType": true,

    /**
    * Developer definable function that is called whenever a cell is created (Ajax source,
    * etc) or processed for input (DOM source). This can be used as a compliment to fnRender
    * allowing you to modify the DOM element (add background colour for example) when the
    * element is available (since it is not when fnRender is called).
    *  @type function
    *  @param {element} nTd The TD node that has been created
    *  @param {*} sData The Data for the cell
    *  @param {array|object} oData The data for the whole row
    *  @param {int} iRow The row index for the aoData data store
    *  @default null
    */
    "fnCreatedCell": null,

    /**
    * Function to get data from a cell in a column. You should <b>never</b>
    * access data directly through _aData internally in DataTables - always use
    * the method attached to this property. It allows mData to function as
    * required. This function is automatically assigned by the column 
    * initialisation method
    *  @type function
    *  @param {array|object} oData The data array/object for the array 
    *    (i.e. aoData[]._aData)
    *  @param {string} sSpecific The specific data type you want to get - 
    *    'display', 'type' 'filter' 'sort'
    *  @returns {*} The data for the cell from the given row's data
    *  @default null
    */
    "fnGetData": null,

    /**
    * Custom display function that will be called for the display of each cell 
    * in this column.
    *  @type function
    *  @param {object} o Object with the following parameters:
    *  @param {int}    o.iDataRow The row in aoData
    *  @param {int}    o.iDataColumn The column in question
    *  @param {array}  o.aData The data for the row in question
    *  @param {object} o.oSettings The settings object for this DataTables instance
    *  @returns {string} The string you which to use in the display
    *  @default null
    */
    "fnRender": null,

    /**
    * Function to set data for a cell in the column. You should <b>never</b> 
    * set the data directly to _aData internally in DataTables - always use
    * this method. It allows mData to function as required. This function
    * is automatically assigned by the column initialisation method
    *  @type function
    *  @param {array|object} oData The data array/object for the array 
    *    (i.e. aoData[]._aData)
    *  @param {*} sValue Value to set
    *  @default null
    */
    "fnSetData": null,

    /**
    * Property to read the value for the cells in the column from the data 
    * source array / object. If null, then the default content is used, if a
    * function is given then the return from the function is used.
    *  @type function|int|string|null
    *  @default null
    */
    "mData": null,

    /**
    * Partner property to mData which is used (only when defined) to get
    * the data - i.e. it is basically the same as mData, but without the
    * 'set' option, and also the data fed to it is the result from mData.
    * This is the rendering method to match the data method of mData.
    *  @type function|int|string|null
    *  @default null
    */
    "mRender": null,

    /**
    * Unique header TH/TD element for this column - this is what the sorting
    * listener is attached to (if sorting is enabled.)
    *  @type node
    *  @default null
    */
    "nTh": null,

    /**
    * Unique footer TH/TD element for this column (if there is one). Not used 
    * in DataTables as such, but can be used for plug-ins to reference the 
    * footer for each column.
    *  @type node
    *  @default null
    */
    "nTf": null,

    /**
    * The class to apply to all TD elements in the table's TBODY for the column
    *  @type string
    *  @default null
    */
    "sClass": null,

    /**
    * When DataTables calculates the column widths to assign to each column,
    * it finds the longest string in each column and then constructs a
    * temporary table and reads the widths from that. The problem with this
    * is that "mmm" is much wider then "iiii", but the latter is a longer 
    * string - thus the calculation can go wrong (doing it properly and putting
    * it into an DOM object and measuring that is horribly(!) slow). Thus as
    * a "work around" we provide this option. It will append its value to the
    * text that is found to be the longest string for the column - i.e. padding.
    *  @type string
    */
    "sContentPadding": null,

    /**
    * Allows a default value to be given for a column's data, and will be used
    * whenever a null data source is encountered (this can be because mData
    * is set to null, or because the data source itself is null).
    *  @type string
    *  @default null
    */
    "sDefaultContent": null,

    /**
    * Name for the column, allowing reference to the column by name as well as
    * by index (needs a lookup to work by name).
    *  @type string
    */
    "sName": null,

    /**
    * Custom sorting data type - defines which of the available plug-ins in
    * afnSortData the custom sorting will use - if any is defined.
    *  @type string
    *  @default std
    */
    "sSortDataType": 'std',

    /**
    * Class to be applied to the header element when sorting on this column
    *  @type string
    *  @default null
    */
    "sSortingClass": null,

    /**
    * Class to be applied to the header element when sorting on this column -
    * when jQuery UI theming is used.
    *  @type string
    *  @default null
    */
    "sSortingClassJUI": null,

    /**
    * Title of the column - what is seen in the TH element (nTh).
    *  @type string
    */
    "sTitle": null,

    /**
    * Column sorting and filtering type
    *  @type string
    *  @default null
    */
    "sType": null,

    /**
    * Width of the column
    *  @type string
    *  @default null
    */
    "sWidth": null,

    /**
    * Width of the column when it was first "encountered"
    *  @type string
    *  @default null
    */
    "sWidthOrig": null
  };



  /**
  * Initialisation options that can be given to DataTables at initialisation 
  * time.
  *  @namespace
  */
  DataTable.defaults = {
    /**
    * An array of data to use for the table, passed in at initialisation which 
    * will be used in preference to any data which is already in the DOM. This is
    * particularly useful for constructing tables purely in Javascript, for
    * example with a custom Ajax call.
    *  @type array
    *  @default null
    *  @dtopt Option
    * 
    *  @example
    *    // Using a 2D array data source
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "aaData": [
    *          ['Trident', 'Internet Explorer 4.0', 'Win 95+', 4, 'X'],
    *          ['Trident', 'Internet Explorer 5.0', 'Win 95+', 5, 'C'],
    *        ],
    *        "aoColumns": [
    *          { "sTitle": "Engine" },
    *          { "sTitle": "Browser" },
    *          { "sTitle": "Platform" },
    *          { "sTitle": "Version" },
    *          { "sTitle": "Grade" }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using an array of objects as a data source (mData)
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "aaData": [
    *          {
    *            "engine":   "Trident",
    *            "browser":  "Internet Explorer 4.0",
    *            "platform": "Win 95+",
    *            "version":  4,
    *            "grade":    "X"
    *          },
    *          {
    *            "engine":   "Trident",
    *            "browser":  "Internet Explorer 5.0",
    *            "platform": "Win 95+",
    *            "version":  5,
    *            "grade":    "C"
    *          }
    *        ],
    *        "aoColumns": [
    *          { "sTitle": "Engine",   "mData": "engine" },
    *          { "sTitle": "Browser",  "mData": "browser" },
    *          { "sTitle": "Platform", "mData": "platform" },
    *          { "sTitle": "Version",  "mData": "version" },
    *          { "sTitle": "Grade",    "mData": "grade" }
    *        ]
    *      } );
    *    } );
    */
    "aaData": null,


    /**
    * If sorting is enabled, then DataTables will perform a first pass sort on 
    * initialisation. You can define which column(s) the sort is performed upon, 
    * and the sorting direction, with this variable. The aaSorting array should 
    * contain an array for each column to be sorted initially containing the 
    * column's index and a direction string ('asc' or 'desc').
    *  @type array
    *  @default [[0,'asc']]
    *  @dtopt Option
    * 
    *  @example
    *    // Sort by 3rd column first, and then 4th column
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aaSorting": [[2,'asc'], [3,'desc']]
    *      } );
    *    } );
    *    
    *    // No initial sorting
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aaSorting": []
    *      } );
    *    } );
    */
    "aaSorting": [
      [0, 'asc']
    ],


    /**
    * This parameter is basically identical to the aaSorting parameter, but 
    * cannot be overridden by user interaction with the table. What this means 
    * is that you could have a column (visible or hidden) which the sorting will 
    * always be forced on first - any sorting after that (from the user) will 
    * then be performed as required. This can be useful for grouping rows 
    * together.
    *  @type array
    *  @default null
    *  @dtopt Option
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aaSortingFixed": [[0,'asc']]
    *      } );
    *    } )
    */
    "aaSortingFixed": null,


    /**
    * This parameter allows you to readily specify the entries in the length drop
    * down menu that DataTables shows when pagination is enabled. It can be 
    * either a 1D array of options which will be used for both the displayed 
    * option and the value, or a 2D array which will use the array in the first 
    * position as the value, and the array in the second position as the 
    * displayed options (useful for language strings such as 'All').
    *  @type array
    *  @default [ 10, 25, 50, 100 ]
    *  @dtopt Option
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aLengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    *      } );
    *    } );
    *  
    *  @example
    *    // Setting the default display length as well as length menu
    *    // This is likely to be wanted if you remove the '10' option which
    *    // is the iDisplayLength default.
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "iDisplayLength": 25,
    *        "aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]]
    *      } );
    *    } );
    */
    "aLengthMenu": [10, 25, 50, 100],


    /**
    * The aoColumns option in the initialisation parameter allows you to define
    * details about the way individual columns behave. For a full list of
    * column options that can be set, please see 
    * {@link DataTable.defaults.columns}. Note that if you use aoColumns to
    * define your columns, you must have an entry in the array for every single
    * column that you have in your table (these can be null if you don't which
    * to specify any options).
    *  @member
    */
    "aoColumns": null,

    /**
    * Very similar to aoColumns, aoColumnDefs allows you to target a specific 
    * column, multiple columns, or all columns, using the aTargets property of 
    * each object in the array. This allows great flexibility when creating 
    * tables, as the aoColumnDefs arrays can be of any length, targeting the 
    * columns you specifically want. aoColumnDefs may use any of the column 
    * options available: {@link DataTable.defaults.columns}, but it _must_
    * have aTargets defined in each object in the array. Values in the aTargets
    * array may be:
    *   <ul>
    *     <li>a string - class name will be matched on the TH for the column</li>
    *     <li>0 or a positive integer - column index counting from the left</li>
    *     <li>a negative integer - column index counting from the right</li>
    *     <li>the string "_all" - all columns (i.e. assign a default)</li>
    *   </ul>
    *  @member
    */
    "aoColumnDefs": null,


    /**
    * Basically the same as oSearch, this parameter defines the individual column
    * filtering state at initialisation time. The array must be of the same size 
    * as the number of columns, and each element be an object with the parameters
    * "sSearch" and "bEscapeRegex" (the latter is optional). 'null' is also
    * accepted and the default will be used.
    *  @type array
    *  @default []
    *  @dtopt Option
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoSearchCols": [
    *          null,
    *          { "sSearch": "My filter" },
    *          null,
    *          { "sSearch": "^[0-9]", "bEscapeRegex": false }
    *        ]
    *      } );
    *    } )
    */
    "aoSearchCols": [],


    /**
    * An array of CSS classes that should be applied to displayed rows. This 
    * array may be of any length, and DataTables will apply each class 
    * sequentially, looping when required.
    *  @type array
    *  @default null <i>Will take the values determined by the oClasses.sStripe*
    *    options</i>
    *  @dtopt Option
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "asStripeClasses": [ 'strip1', 'strip2', 'strip3' ]
    *      } );
    *    } )
    */
    "asStripeClasses": null,


    /**
    * Enable or disable automatic column width calculation. This can be disabled
    * as an optimisation (it takes some time to calculate the widths) if the
    * tables widths are passed in using aoColumns.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bAutoWidth": false
    *      } );
    *    } );
    */
    "bAutoWidth": true,


    /**
    * Deferred rendering can provide DataTables with a huge speed boost when you
    * are using an Ajax or JS data source for the table. This option, when set to
    * true, will cause DataTables to defer the creation of the table elements for
    * each row until they are needed for a draw - saving a significant amount of
    * time.
    *  @type boolean
    *  @default false
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "sAjaxSource": "sources/arrays.txt",
    *        "bDeferRender": true
    *      } );
    *    } );
    */
    "bDeferRender": false,


    /**
    * Replace a DataTable which matches the given selector and replace it with 
    * one which has the properties of the new initialisation object passed. If no
    * table matches the selector, then the new DataTable will be constructed as
    * per normal.
    *  @type boolean
    *  @default false
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sScrollY": "200px",
    *        "bPaginate": false
    *      } );
    *      
    *      // Some time later....
    *      $('#example').dataTable( {
    *        "bFilter": false,
    *        "bDestroy": true
    *      } );
    *    } );
    */
    "bDestroy": false,


    /**
    * Enable or disable filtering of data. Filtering in DataTables is "smart" in
    * that it allows the end user to input multiple words (space separated) and
    * will match a row containing those words, even if not in the order that was
    * specified (this allow matching across multiple columns). Note that if you
    * wish to use filtering in DataTables this must remain 'true' - to remove the
    * default filtering input box and retain filtering abilities, please use
    * {@link DataTable.defaults.sDom}.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bFilter": false
    *      } );
    *    } );
    */
    "bFilter": true,


    /**
    * Enable or disable the table information display. This shows information 
    * about the data that is currently visible on the page, including information
    * about filtered data if that action is being performed.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bInfo": false
    *      } );
    *    } );
    */
    "bInfo": true,


    /**
    * Enable jQuery UI ThemeRoller support (required as ThemeRoller requires some
    * slightly different and additional mark-up from what DataTables has
    * traditionally used).
    *  @type boolean
    *  @default false
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bJQueryUI": true
    *      } );
    *    } );
    */
    "bJQueryUI": false,


    /**
    * Allows the end user to select the size of a formatted page from a select
    * menu (sizes are 10, 25, 50 and 100). Requires pagination (bPaginate).
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bLengthChange": false
    *      } );
    *    } );
    */
    "bLengthChange": true,


    /**
    * Enable or disable pagination.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bPaginate": false
    *      } );
    *    } );
    */
    "bPaginate": true,


    /**
    * Enable or disable the display of a 'processing' indicator when the table is
    * being processed (e.g. a sort). This is particularly useful for tables with
    * large amounts of data where it can take a noticeable amount of time to sort
    * the entries.
    *  @type boolean
    *  @default false
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bProcessing": true
    *      } );
    *    } );
    */
    "bProcessing": false,


    /**
    * Retrieve the DataTables object for the given selector. Note that if the
    * table has already been initialised, this parameter will cause DataTables
    * to simply return the object that has already been set up - it will not take
    * account of any changes you might have made to the initialisation object
    * passed to DataTables (setting this parameter to true is an acknowledgement
    * that you understand this). bDestroy can be used to reinitialise a table if
    * you need.
    *  @type boolean
    *  @default false
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      initTable();
    *      tableActions();
    *    } );
    *    
    *    function initTable ()
    *    {
    *      return $('#example').dataTable( {
    *        "sScrollY": "200px",
    *        "bPaginate": false,
    *        "bRetrieve": true
    *      } );
    *    }
    *    
    *    function tableActions ()
    *    {
    *      var oTable = initTable();
    *      // perform API operations with oTable 
    *    }
    */
    "bRetrieve": false,


    /**
    * Indicate if DataTables should be allowed to set the padding / margin
    * etc for the scrolling header elements or not. Typically you will want
    * this.
    *  @type boolean
    *  @default true
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bScrollAutoCss": false,
    *        "sScrollY": "200px"
    *      } );
    *    } );
    */
    "bScrollAutoCss": true,


    /**
    * When vertical (y) scrolling is enabled, DataTables will force the height of
    * the table's viewport to the given height at all times (useful for layout).
    * However, this can look odd when filtering data down to a small data set,
    * and the footer is left "floating" further down. This parameter (when
    * enabled) will cause DataTables to collapse the table's viewport down when
    * the result set will fit within the given Y height.
    *  @type boolean
    *  @default false
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sScrollY": "200",
    *        "bScrollCollapse": true
    *      } );
    *    } );
    */
    "bScrollCollapse": false,


    /**
    * Enable infinite scrolling for DataTables (to be used in combination with
    * sScrollY). Infinite scrolling means that DataTables will continually load
    * data as a user scrolls through a table, which is very useful for large
    * dataset. This cannot be used with pagination, which is automatically
    * disabled. Note - the Scroller extra for DataTables is recommended in
    * in preference to this option.
    *  @type boolean
    *  @default false
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bScrollInfinite": true,
    *        "bScrollCollapse": true,
    *        "sScrollY": "200px"
    *      } );
    *    } );
    */
    "bScrollInfinite": false,


    /**
    * Configure DataTables to use server-side processing. Note that the
    * sAjaxSource parameter must also be given in order to give DataTables a
    * source to obtain the required data for each draw.
    *  @type boolean
    *  @default false
    *  @dtopt Features
    *  @dtopt Server-side
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bServerSide": true,
    *        "sAjaxSource": "xhr.php"
    *      } );
    *    } );
    */
    "bServerSide": false,


    /**
    * Enable or disable sorting of columns. Sorting of individual columns can be
    * disabled by the "bSortable" option for each column.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bSort": false
    *      } );
    *    } );
    */
    "bSort": true,


    /**
    * Allows control over whether DataTables should use the top (true) unique
    * cell that is found for a single column, or the bottom (false - default).
    * This is useful when using complex headers.
    *  @type boolean
    *  @default false
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bSortCellsTop": true
    *      } );
    *    } );
    */
    "bSortCellsTop": false,


    /**
    * Enable or disable the addition of the classes 'sorting_1', 'sorting_2' and
    * 'sorting_3' to the columns which are currently being sorted on. This is
    * presented as a feature switch as it can increase processing time (while
    * classes are removed and added) so for large data sets you might want to
    * turn this off.
    *  @type boolean
    *  @default true
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bSortClasses": false
    *      } );
    *    } );
    */
    "bSortClasses": true,


    /**
    * Enable or disable state saving. When enabled a cookie will be used to save
    * table display information such as pagination information, display length,
    * filtering and sorting. As such when the end user reloads the page the
    * display display will match what thy had previously set up.
    *  @type boolean
    *  @default false
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "bStateSave": true
    *      } );
    *    } );
    */
    "bStateSave": false,


    /**
    * Customise the cookie and / or the parameters being stored when using
    * DataTables with state saving enabled. This function is called whenever
    * the cookie is modified, and it expects a fully formed cookie string to be
    * returned. Note that the data object passed in is a Javascript object which
    * must be converted to a string (JSON.stringify for example).
    *  @type function
    *  @param {string} sName Name of the cookie defined by DataTables
    *  @param {object} oData Data to be stored in the cookie
    *  @param {string} sExpires Cookie expires string
    *  @param {string} sPath Path of the cookie to set
    *  @returns {string} Cookie formatted string (which should be encoded by
    *    using encodeURIComponent())
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function () {
    *      $('#example').dataTable( {
    *        "fnCookieCallback": function (sName, oData, sExpires, sPath) {
    *          // Customise oData or sName or whatever else here
    *          return sName + "="+JSON.stringify(oData)+"; expires=" + sExpires +"; path=" + sPath;
    *        }
    *      } );
    *    } );
    */
    "fnCookieCallback": null,


    /**
    * This function is called when a TR element is created (and all TD child
    * elements have been inserted), or registered if using a DOM source, allowing
    * manipulation of the TR element (adding classes etc).
    *  @type function
    *  @param {node} nRow "TR" element for the current row
    *  @param {array} aData Raw data array for this row
    *  @param {int} iDataIndex The index of this row in aoData
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnCreatedRow": function( nRow, aData, iDataIndex ) {
    *          // Bold the grade for all 'A' grade browsers
    *          if ( aData[4] == "A" )
    *          {
    *            $('td:eq(4)', nRow).html( '<b>A</b>' );
    *          }
    *        }
    *      } );
    *    } );
    */
    "fnCreatedRow": null,


    /**
    * This function is called on every 'draw' event, and allows you to
    * dynamically modify any aspect you want about the created DOM.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnDrawCallback": function( oSettings ) {
    *          alert( 'DataTables has redrawn the table' );
    *        }
    *      } );
    *    } );
    */
    "fnDrawCallback": null,


    /**
    * Identical to fnHeaderCallback() but for the table footer this function
    * allows you to modify the table footer on every 'draw' even.
    *  @type function
    *  @param {node} nFoot "TR" element for the footer
    *  @param {array} aData Full table data (as derived from the original HTML)
    *  @param {int} iStart Index for the current display starting point in the 
    *    display array
    *  @param {int} iEnd Index for the current display ending point in the 
    *    display array
    *  @param {array int} aiDisplay Index array to translate the visual position
    *    to the full data array
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnFooterCallback": function( nFoot, aData, iStart, iEnd, aiDisplay ) {
    *          nFoot.getElementsByTagName('th')[0].innerHTML = "Starting index is "+iStart;
    *        }
    *      } );
    *    } )
    */
    "fnFooterCallback": null,


    /**
    * When rendering large numbers in the information element for the table
    * (i.e. "Showing 1 to 10 of 57 entries") DataTables will render large numbers
    * to have a comma separator for the 'thousands' units (e.g. 1 million is
    * rendered as "1,000,000") to help readability for the end user. This
    * function will override the default method DataTables uses.
    *  @type function
    *  @member
    *  @param {int} iIn number to be formatted
    *  @returns {string} formatted string for DataTables to show the number
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnFormatNumber": function ( iIn ) {
    *          if ( iIn &lt; 1000 ) {
    *            return iIn;
    *          } else {
    *            var 
    *              s=(iIn+""), 
    *              a=s.split(""), out="", 
    *              iLen=s.length;
    *            
    *            for ( var i=0 ; i&lt;iLen ; i++ ) {
    *              if ( i%3 === 0 &amp;&amp; i !== 0 ) {
    *                out = "'"+out;
    *              }
    *              out = a[iLen-i-1]+out;
    *            }
    *          }
    *          return out;
    *        };
    *      } );
    *    } );
    */
    "fnFormatNumber": function (iIn) {
      if (iIn < 1000) {
        // A small optimisation for what is likely to be the majority of use cases
        return iIn;
      }

      var s = (iIn + ""),
        a = s.split(""),
        out = "",
        iLen = s.length;

      for (var i = 0; i < iLen; i++) {
        if (i % 3 === 0 && i !== 0) {
          out = this.oLanguage.sInfoThousands + out;
        }
        out = a[iLen - i - 1] + out;
      }
      return out;
    },


    /**
    * This function is called on every 'draw' event, and allows you to
    * dynamically modify the header row. This can be used to calculate and
    * display useful information about the table.
    *  @type function
    *  @param {node} nHead "TR" element for the header
    *  @param {array} aData Full table data (as derived from the original HTML)
    *  @param {int} iStart Index for the current display starting point in the
    *    display array
    *  @param {int} iEnd Index for the current display ending point in the
    *    display array
    *  @param {array int} aiDisplay Index array to translate the visual position
    *    to the full data array
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnHeaderCallback": function( nHead, aData, iStart, iEnd, aiDisplay ) {
    *          nHead.getElementsByTagName('th')[0].innerHTML = "Displaying "+(iEnd-iStart)+" records";
    *        }
    *      } );
    *    } )
    */
    "fnHeaderCallback": null,


    /**
    * The information element can be used to convey information about the current
    * state of the table. Although the internationalisation options presented by
    * DataTables are quite capable of dealing with most customisations, there may
    * be times where you wish to customise the string further. This callback
    * allows you to do exactly that.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @param {int} iStart Starting position in data for the draw
    *  @param {int} iEnd End position in data for the draw
    *  @param {int} iMax Total number of rows in the table (regardless of
    *    filtering)
    *  @param {int} iTotal Total number of rows in the data set, after filtering
    *  @param {string} sPre The string that DataTables has formatted using it's
    *    own rules
    *  @returns {string} The string to be displayed in the information element.
    *  @dtopt Callbacks
    * 
    *  @example
    *    $('#example').dataTable( {
    *      "fnInfoCallback": function( oSettings, iStart, iEnd, iMax, iTotal, sPre ) {
    *        return iStart +" to "+ iEnd;
    *      }
    *    } );
    */
    "fnInfoCallback": null,


    /**
    * Called when the table has been initialised. Normally DataTables will
    * initialise sequentially and there will be no need for this function,
    * however, this does not hold true when using external language information
    * since that is obtained using an async XHR call.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @param {object} json The JSON object request from the server - only
    *    present if client-side Ajax sourced data is used
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnInitComplete": function(oSettings, json) {
    *          alert( 'DataTables has finished its initialisation.' );
    *        }
    *      } );
    *    } )
    */
    "fnInitComplete": null,


    /**
    * Called at the very start of each table draw and can be used to cancel the
    * draw by returning false, any other return (including undefined) results in
    * the full draw occurring).
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @returns {boolean} False will cancel the draw, anything else (including no
    *    return) will allow it to complete.
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnPreDrawCallback": function( oSettings ) {
    *          if ( $('#test').val() == 1 ) {
    *            return false;
    *          }
    *        }
    *      } );
    *    } );
    */
    "fnPreDrawCallback": null,


    /**
    * This function allows you to 'post process' each row after it have been
    * generated for each table draw, but before it is rendered on screen. This
    * function might be used for setting the row class name etc.
    *  @type function
    *  @param {node} nRow "TR" element for the current row
    *  @param {array} aData Raw data array for this row
    *  @param {int} iDisplayIndex The display index for the current table draw
    *  @param {int} iDisplayIndexFull The index of the data in the full list of
    *    rows (after filtering)
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "fnRowCallback": function( nRow, aData, iDisplayIndex, iDisplayIndexFull ) {
    *          // Bold the grade for all 'A' grade browsers
    *          if ( aData[4] == "A" )
    *          {
    *            $('td:eq(4)', nRow).html( '<b>A</b>' );
    *          }
    *        }
    *      } );
    *    } );
    */
    "fnRowCallback": null,


    /**
    * This parameter allows you to override the default function which obtains
    * the data from the server ($.getJSON) so something more suitable for your
    * application. For example you could use POST data, or pull information from
    * a Gears or AIR database.
    *  @type function
    *  @member
    *  @param {string} sSource HTTP source to obtain the data from (sAjaxSource)
    *  @param {array} aoData A key/value pair object containing the data to send
    *    to the server
    *  @param {function} fnCallback to be called on completion of the data get
    *    process that will draw the data on the page.
    *  @param {object} oSettings DataTables settings object
    *  @dtopt Callbacks
    *  @dtopt Server-side
    * 
    *  @example
    *    // POST data to server
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bProcessing": true,
    *        "bServerSide": true,
    *        "sAjaxSource": "xhr.php",
    *        "fnServerData": function ( sSource, aoData, fnCallback, oSettings ) {
    *          oSettings.jqXHR = $.ajax( {
    *            "dataType": 'json', 
    *            "type": "POST", 
    *            "url": sSource, 
    *            "data": aoData, 
    *            "success": fnCallback
    *          } );
    *        }
    *      } );
    *    } );
    */
    "fnServerData": function (sUrl, aoData, fnCallback, oSettings) {
      oSettings.jqXHR = $.ajax({
        "url": sUrl,
        "data": aoData,
        "success": function (json) {
          if (json.sError) {
            oSettings.oApi._fnLog(oSettings, 0, json.sError);
          }

          $(oSettings.oInstance).trigger('xhr', [oSettings, json]);
          fnCallback(json);
        },
        "dataType": "json",
        "cache": false,
        "type": oSettings.sServerMethod,
        "error": function (xhr, error, thrown) {
          if (error == "parsererror") {
            oSettings.oApi._fnLog(oSettings, 0, "DataTables warning: JSON data from " + "server could not be parsed. This is caused by a JSON formatting error.");
          }
        }
      });
    },


    /**
    * It is often useful to send extra data to the server when making an Ajax
    * request - for example custom filtering information, and this callback
    * function makes it trivial to send extra information to the server. The
    * passed in parameter is the data set that has been constructed by
    * DataTables, and you can add to this or modify it as you require.
    *  @type function
    *  @param {array} aoData Data array (array of objects which are name/value
    *    pairs) that has been constructed by DataTables and will be sent to the
    *    server. In the case of Ajax sourced data with server-side processing
    *    this will be an empty array, for server-side processing there will be a
    *    significant number of parameters!
    *  @returns {undefined} Ensure that you modify the aoData array passed in,
    *    as this is passed by reference.
    *  @dtopt Callbacks
    *  @dtopt Server-side
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bProcessing": true,
    *        "bServerSide": true,
    *        "sAjaxSource": "scripts/server_processing.php",
    *        "fnServerParams": function ( aoData ) {
    *          aoData.push( { "name": "more_data", "value": "my_value" } );
    *        }
    *      } );
    *    } );
    */
    "fnServerParams": null,


    /**
    * Load the table state. With this function you can define from where, and how, the
    * state of a table is loaded. By default DataTables will load from its state saving
    * cookie, but you might wish to use local storage (HTML5) or a server-side database.
    *  @type function
    *  @member
    *  @param {object} oSettings DataTables settings object
    *  @return {object} The DataTables state object to be loaded
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateLoad": function (oSettings) {
    *          var o;
    *          
    *          // Send an Ajax request to the server to get the data. Note that
    *          // this is a synchronous request.
    *          $.ajax( {
    *            "url": "/state_load",
    *            "async": false,
    *            "dataType": "json",
    *            "success": function (json) {
    *              o = json;
    *            }
    *          } );
    *          
    *          return o;
    *        }
    *      } );
    *    } );
    */
    "fnStateLoad": function (oSettings) {
      var sData = this.oApi._fnReadCookie(oSettings.sCookiePrefix + oSettings.sInstance);
      var oData;

      try {
        oData = (typeof $.parseJSON === 'function') ? $.parseJSON(sData) : eval('(' + sData + ')');
      } catch (e) {
        oData = null;
      }

      return oData;
    },


    /**
    * Callback which allows modification of the saved state prior to loading that state.
    * This callback is called when the table is loading state from the stored data, but
    * prior to the settings object being modified by the saved state. Note that for 
    * plug-in authors, you should use the 'stateLoadParams' event to load parameters for 
    * a plug-in.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @param {object} oData The state object that is to be loaded
    *  @dtopt Callbacks
    * 
    *  @example
    *    // Remove a saved filter, so filtering is never loaded
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateLoadParams": function (oSettings, oData) {
    *          oData.oSearch.sSearch = "";
    *        }
    *      } );
    *    } );
    * 
    *  @example
    *    // Disallow state loading by returning false
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateLoadParams": function (oSettings, oData) {
    *          return false;
    *        }
    *      } );
    *    } );
    */
    "fnStateLoadParams": null,


    /**
    * Callback that is called when the state has been loaded from the state saving method
    * and the DataTables settings object has been modified as a result of the loaded state.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @param {object} oData The state object that was loaded
    *  @dtopt Callbacks
    * 
    *  @example
    *    // Show an alert with the filtering value that was saved
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateLoaded": function (oSettings, oData) {
    *          alert( 'Saved filter was: '+oData.oSearch.sSearch );
    *        }
    *      } );
    *    } );
    */
    "fnStateLoaded": null,


    /**
    * Save the table state. This function allows you to define where and how the state
    * information for the table is stored - by default it will use a cookie, but you
    * might want to use local storage (HTML5) or a server-side database.
    *  @type function
    *  @member
    *  @param {object} oSettings DataTables settings object
    *  @param {object} oData The state object to be saved
    *  @dtopt Callbacks
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateSave": function (oSettings, oData) {
    *          // Send an Ajax request to the server with the state object
    *          $.ajax( {
    *            "url": "/state_save",
    *            "data": oData,
    *            "dataType": "json",
    *            "method": "POST"
    *            "success": function () {}
    *          } );
    *        }
    *      } );
    *    } );
    */
    "fnStateSave": function (oSettings, oData) {
      this.oApi._fnCreateCookie(
      oSettings.sCookiePrefix + oSettings.sInstance, this.oApi._fnJsonString(oData), oSettings.iCookieDuration, oSettings.sCookiePrefix, oSettings.fnCookieCallback);
    },


    /**
    * Callback which allows modification of the state to be saved. Called when the table 
    * has changed state a new state save is required. This method allows modification of
    * the state saving object prior to actually doing the save, including addition or 
    * other state properties or modification. Note that for plug-in authors, you should 
    * use the 'stateSaveParams' event to save parameters for a plug-in.
    *  @type function
    *  @param {object} oSettings DataTables settings object
    *  @param {object} oData The state object to be saved
    *  @dtopt Callbacks
    * 
    *  @example
    *    // Remove a saved filter, so filtering is never saved
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bStateSave": true,
    *        "fnStateSaveParams": function (oSettings, oData) {
    *          oData.oSearch.sSearch = "";
    *        }
    *      } );
    *    } );
    */
    "fnStateSaveParams": null,


    /**
    * Duration of the cookie which is used for storing session information. This
    * value is given in seconds.
    *  @type int
    *  @default 7200 <i>(2 hours)</i>
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "iCookieDuration": 60*60*24; // 1 day
    *      } );
    *    } )
    */
    "iCookieDuration": 7200,


    /**
    * When enabled DataTables will not make a request to the server for the first
    * page draw - rather it will use the data already on the page (no sorting etc
    * will be applied to it), thus saving on an XHR at load time. iDeferLoading
    * is used to indicate that deferred loading is required, but it is also used
    * to tell DataTables how many records there are in the full table (allowing
    * the information element and pagination to be displayed correctly). In the case
    * where a filtering is applied to the table on initial load, this can be
    * indicated by giving the parameter as an array, where the first element is
    * the number of records available after filtering and the second element is the
    * number of records without filtering (allowing the table information element
    * to be shown correctly).
    *  @type int | array
    *  @default null
    *  @dtopt Options
    * 
    *  @example
    *    // 57 records available in the table, no filtering applied
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bServerSide": true,
    *        "sAjaxSource": "scripts/server_processing.php",
    *        "iDeferLoading": 57
    *      } );
    *    } );
    * 
    *  @example
    *    // 57 records after filtering, 100 without filtering (an initial filter applied)
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bServerSide": true,
    *        "sAjaxSource": "scripts/server_processing.php",
    *        "iDeferLoading": [ 57, 100 ],
    *        "oSearch": {
    *          "sSearch": "my_filter"
    *        }
    *      } );
    *    } );
    */
    "iDeferLoading": null,


    /**
    * Number of rows to display on a single page when using pagination. If
    * feature enabled (bLengthChange) then the end user will be able to override
    * this to a custom setting using a pop-up menu.
    *  @type int
    *  @default 10
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "iDisplayLength": 50
    *      } );
    *    } )
    */
    "iDisplayLength": 10,


    /**
    * Define the starting point for data display when using DataTables with
    * pagination. Note that this parameter is the number of records, rather than
    * the page number, so if you have 10 records per page and want to start on
    * the third page, it should be "20".
    *  @type int
    *  @default 0
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "iDisplayStart": 20
    *      } );
    *    } )
    */
    "iDisplayStart": 0,


    /**
    * The scroll gap is the amount of scrolling that is left to go before
    * DataTables will load the next 'page' of data automatically. You typically
    * want a gap which is big enough that the scrolling will be smooth for the
    * user, while not so large that it will load more data than need.
    *  @type int
    *  @default 100
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bScrollInfinite": true,
    *        "bScrollCollapse": true,
    *        "sScrollY": "200px",
    *        "iScrollLoadGap": 50
    *      } );
    *    } );
    */
    "iScrollLoadGap": 100,


    /**
    * By default DataTables allows keyboard navigation of the table (sorting, paging,
    * and filtering) by adding a tabindex attribute to the required elements. This
    * allows you to tab through the controls and press the enter key to activate them.
    * The tabindex is default 0, meaning that the tab follows the flow of the document.
    * You can overrule this using this parameter if you wish. Use a value of -1 to
    * disable built-in keyboard navigation.
    *  @type int
    *  @default 0
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "iTabIndex": 1
    *      } );
    *    } );
    */
    "iTabIndex": 0,


    /**
    * All strings that DataTables uses in the user interface that it creates
    * are defined in this object, allowing you to modified them individually or
    * completely replace them all as required.
    *  @namespace
    */
    "oLanguage": {
      /**
      * Strings that are used for WAI-ARIA labels and controls only (these are not
      * actually visible on the page, but will be read by screenreaders, and thus
      * must be internationalised as well).
      *  @namespace
      */
      "oAria": {
        /**
        * ARIA label that is added to the table headers when the column may be
        * sorted ascending by activing the column (click or return when focused).
        * Note that the column header is prefixed to this string.
        *  @type string
        *  @default : activate to sort column ascending
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oAria": {
        *            "sSortAscending": " - click/return to sort ascending"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sSortAscending": ": activate to sort column ascending",

        /**
        * ARIA label that is added to the table headers when the column may be
        * sorted descending by activing the column (click or return when focused).
        * Note that the column header is prefixed to this string.
        *  @type string
        *  @default : activate to sort column ascending
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oAria": {
        *            "sSortDescending": " - click/return to sort descending"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sSortDescending": ": activate to sort column descending"
      },

      /**
      * Pagination string used by DataTables for the two built-in pagination
      * control types ("two_button" and "full_numbers")
      *  @namespace
      */
      "oPaginate": {
        /**
        * Text to use when using the 'full_numbers' type of pagination for the
        * button to take the user to the first page.
        *  @type string
        *  @default First
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oPaginate": {
        *            "sFirst": "First page"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sFirst": "First",


        /**
        * Text to use when using the 'full_numbers' type of pagination for the
        * button to take the user to the last page.
        *  @type string
        *  @default Last
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oPaginate": {
        *            "sLast": "Last page"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sLast": "Last",


        /**
        * Text to use when using the 'full_numbers' type of pagination for the
        * button to take the user to the next page.
        *  @type string
        *  @default Next
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oPaginate": {
        *            "sNext": "Next page"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sNext": "Next",


        /**
        * Text to use when using the 'full_numbers' type of pagination for the
        * button to take the user to the previous page.
        *  @type string
        *  @default Previous
        *  @dtopt Language
        * 
        *  @example
        *    $(document).ready( function() {
        *      $('#example').dataTable( {
        *        "oLanguage": {
        *          "oPaginate": {
        *            "sPrevious": "Previous page"
        *          }
        *        }
        *      } );
        *    } );
        */
        "sPrevious": "Previous"
      },

      /**
      * This string is shown in preference to sZeroRecords when the table is
      * empty of data (regardless of filtering). Note that this is an optional
      * parameter - if it is not given, the value of sZeroRecords will be used
      * instead (either the default or given value).
      *  @type string
      *  @default No data available in table
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sEmptyTable": "No data available in table"
      *        }
      *      } );
      *    } );
      */
      "sEmptyTable": "No data available in table",


      /**
      * This string gives information to the end user about the information that 
      * is current on display on the page. The _START_, _END_ and _TOTAL_ 
      * variables are all dynamically replaced as the table display updates, and 
      * can be freely moved or removed as the language requirements change.
      *  @type string
      *  @default Showing _START_ to _END_ of _TOTAL_ entries
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sInfo": "Got a total of _TOTAL_ entries to show (_START_ to _END_)"
      *        }
      *      } );
      *    } );
      */
      "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",


      /**
      * Display information string for when the table is empty. Typically the 
      * format of this string should match sInfo.
      *  @type string
      *  @default Showing 0 to 0 of 0 entries
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sInfoEmpty": "No entries to show"
      *        }
      *      } );
      *    } );
      */
      "sInfoEmpty": "Showing 0 to 0 of 0 entries",


      /**
      * When a user filters the information in a table, this string is appended 
      * to the information (sInfo) to give an idea of how strong the filtering 
      * is. The variable _MAX_ is dynamically updated.
      *  @type string
      *  @default (filtered from _MAX_ total entries)
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sInfoFiltered": " - filtering from _MAX_ records"
      *        }
      *      } );
      *    } );
      */
      "sInfoFiltered": "(filtered from _MAX_ total entries)",


      /**
      * If can be useful to append extra information to the info string at times,
      * and this variable does exactly that. This information will be appended to
      * the sInfo (sInfoEmpty and sInfoFiltered in whatever combination they are
      * being used) at all times.
      *  @type string
      *  @default <i>Empty string</i>
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sInfoPostFix": "All records shown are derived from real information."
      *        }
      *      } );
      *    } );
      */
      "sInfoPostFix": "",


      /**
      * DataTables has a build in number formatter (fnFormatNumber) which is used
      * to format large numbers that are used in the table information. By
      * default a comma is used, but this can be trivially changed to any
      * character you wish with this parameter.
      *  @type string
      *  @default ,
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sInfoThousands": "'"
      *        }
      *      } );
      *    } );
      */
      "sInfoThousands": ",",


      /**
      * Detail the action that will be taken when the drop down menu for the
      * pagination length option is changed. The '_MENU_' variable is replaced
      * with a default select list of 10, 25, 50 and 100, and can be replaced
      * with a custom select box if required.
      *  @type string
      *  @default Show _MENU_ entries
      *  @dtopt Language
      * 
      *  @example
      *    // Language change only
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sLengthMenu": "Display _MENU_ records"
      *        }
      *      } );
      *    } );
      *    
      *  @example
      *    // Language and options change
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sLengthMenu": 'Display <select>'+
      *            '<option value="10">10</option>'+
      *            '<option value="20">20</option>'+
      *            '<option value="30">30</option>'+
      *            '<option value="40">40</option>'+
      *            '<option value="50">50</option>'+
      *            '<option value="-1">All</option>'+
      *            '</select> records'
      *        }
      *      } );
      *    } );
      */
      "sLengthMenu": "Show _MENU_ entries",


      /**
      * When using Ajax sourced data and during the first draw when DataTables is
      * gathering the data, this message is shown in an empty row in the table to
      * indicate to the end user the the data is being loaded. Note that this
      * parameter is not used when loading data by server-side processing, just
      * Ajax sourced data with client-side processing.
      *  @type string
      *  @default Loading...
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sLoadingRecords": "Please wait - loading..."
      *        }
      *      } );
      *    } );
      */
      "sLoadingRecords": "Loading...",


      /**
      * Text which is displayed when the table is processing a user action
      * (usually a sort command or similar).
      *  @type string
      *  @default Processing...
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sProcessing": "DataTables is currently busy"
      *        }
      *      } );
      *    } );
      */
      "sProcessing": "Processing...",


      /**
      * Details the actions that will be taken when the user types into the
      * filtering input text box. The variable "_INPUT_", if used in the string,
      * is replaced with the HTML text box for the filtering input allowing
      * control over where it appears in the string. If "_INPUT_" is not given
      * then the input box is appended to the string automatically.
      *  @type string
      *  @default Search:
      *  @dtopt Language
      * 
      *  @example
      *    // Input text box will be appended at the end automatically
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sSearch": "Filter records:"
      *        }
      *      } );
      *    } );
      *    
      *  @example
      *    // Specify where the filter should appear
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sSearch": "Apply filter _INPUT_ to table"
      *        }
      *      } );
      *    } );
      */
      "sSearch": "Search:",


      /**
      * All of the language information can be stored in a file on the
      * server-side, which DataTables will look up if this parameter is passed.
      * It must store the URL of the language file, which is in a JSON format,
      * and the object has the same properties as the oLanguage object in the
      * initialiser object (i.e. the above parameters). Please refer to one of
      * the example language files to see how this works in action.
      *  @type string
      *  @default <i>Empty string - i.e. disabled</i>
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sUrl": "http://www.sprymedia.co.uk/dataTables/lang.txt"
      *        }
      *      } );
      *    } );
      */
      "sUrl": "",


      /**
      * Text shown inside the table records when the is no information to be
      * displayed after filtering. sEmptyTable is shown when there is simply no
      * information in the table at all (regardless of filtering).
      *  @type string
      *  @default No matching records found
      *  @dtopt Language
      * 
      *  @example
      *    $(document).ready( function() {
      *      $('#example').dataTable( {
      *        "oLanguage": {
      *          "sZeroRecords": "No records to display"
      *        }
      *      } );
      *    } );
      */
      "sZeroRecords": "No matching records found"
    },


    /**
    * This parameter allows you to have define the global filtering state at
    * initialisation time. As an object the "sSearch" parameter must be
    * defined, but all other parameters are optional. When "bRegex" is true,
    * the search string will be treated as a regular expression, when false
    * (default) it will be treated as a straight string. When "bSmart"
    * DataTables will use it's smart filtering methods (to word match at
    * any point in the data), when false this will not be done.
    *  @namespace
    *  @extends DataTable.models.oSearch
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "oSearch": {"sSearch": "Initial search"}
    *      } );
    *    } )
    */
    "oSearch": $.extend({}, DataTable.models.oSearch),


    /**
    * By default DataTables will look for the property 'aaData' when obtaining
    * data from an Ajax source or for server-side processing - this parameter
    * allows that property to be changed. You can use Javascript dotted object
    * notation to get a data source for multiple levels of nesting.
    *  @type string
    *  @default aaData
    *  @dtopt Options
    *  @dtopt Server-side
    * 
    *  @example
    *    // Get data from { "data": [...] }
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "sAjaxSource": "sources/data.txt",
    *        "sAjaxDataProp": "data"
    *      } );
    *    } );
    *    
    *  @example
    *    // Get data from { "data": { "inner": [...] } }
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "sAjaxSource": "sources/data.txt",
    *        "sAjaxDataProp": "data.inner"
    *      } );
    *    } );
    */
    "sAjaxDataProp": "aaData",


    /**
    * You can instruct DataTables to load data from an external source using this
    * parameter (use aData if you want to pass data in you already have). Simply
    * provide a url a JSON object can be obtained from. This object must include
    * the parameter 'aaData' which is the data source for the table.
    *  @type string
    *  @default null
    *  @dtopt Options
    *  @dtopt Server-side
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sAjaxSource": "http://www.sprymedia.co.uk/dataTables/json.php"
    *      } );
    *    } )
    */
    "sAjaxSource": null,


    /**
    * This parameter can be used to override the default prefix that DataTables
    * assigns to a cookie when state saving is enabled.
    *  @type string
    *  @default SpryMedia_DataTables_
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sCookiePrefix": "my_datatable_",
    *      } );
    *    } );
    */
    "sCookiePrefix": "SpryMedia_DataTables_",


    /**
    * This initialisation variable allows you to specify exactly where in the
    * DOM you want DataTables to inject the various controls it adds to the page
    * (for example you might want the pagination controls at the top of the
    * table). DIV elements (with or without a custom class) can also be added to
    * aid styling. The follow syntax is used:
    *   <ul>
    *     <li>The following options are allowed:	
    *       <ul>
    *         <li>'l' - Length changing</li
    *         <li>'f' - Filtering input</li>
    *         <li>'t' - The table!</li>
    *         <li>'i' - Information</li>
    *         <li>'p' - Pagination</li>
    *         <li>'r' - pRocessing</li>
    *       </ul>
    *     </li>
    *     <li>The following constants are allowed:
    *       <ul>
    *         <li>'H' - jQueryUI theme "header" classes ('fg-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix')</li>
    *         <li>'F' - jQueryUI theme "footer" classes ('fg-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix')</li>
    *       </ul>
    *     </li>
    *     <li>The following syntax is expected:
    *       <ul>
    *         <li>'&lt;' and '&gt;' - div elements</li>
    *         <li>'&lt;"class" and '&gt;' - div with a class</li>
    *         <li>'&lt;"#id" and '&gt;' - div with an ID</li>
    *       </ul>
    *     </li>
    *     <li>Examples:
    *       <ul>
    *         <li>'&lt;"wrapper"flipt&gt;'</li>
    *         <li>'&lt;lf&lt;t&gt;ip&gt;'</li>
    *       </ul>
    *     </li>
    *   </ul>
    *  @type string
    *  @default lfrtip <i>(when bJQueryUI is false)</i> <b>or</b> 
    *    <"H"lfr>t<"F"ip> <i>(when bJQueryUI is true)</i>
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sDom": '&lt;"top"i&gt;rt&lt;"bottom"flp&gt;&lt;"clear"&gt;'
    *      } );
    *    } );
    */
    "sDom": "lfrtip",


    /**
    * DataTables features two different built-in pagination interaction methods
    * ('two_button' or 'full_numbers') which present different page controls to
    * the end user. Further methods can be added using the API (see below).
    *  @type string
    *  @default two_button
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sPaginationType": "full_numbers"
    *      } );
    *    } )
    */
    "sPaginationType": "two_button",


    /**
    * Enable horizontal scrolling. When a table is too wide to fit into a certain
    * layout, or you have a large number of columns in the table, you can enable
    * x-scrolling to show the table in a viewport, which can be scrolled. This
    * property can be any CSS unit, or a number (in which case it will be treated
    * as a pixel measurement).
    *  @type string
    *  @default <i>blank string - i.e. disabled</i>
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sScrollX": "100%",
    *        "bScrollCollapse": true
    *      } );
    *    } );
    */
    "sScrollX": "",


    /**
    * This property can be used to force a DataTable to use more width than it
    * might otherwise do when x-scrolling is enabled. For example if you have a
    * table which requires to be well spaced, this parameter is useful for
    * "over-sizing" the table, and thus forcing scrolling. This property can by
    * any CSS unit, or a number (in which case it will be treated as a pixel
    * measurement).
    *  @type string
    *  @default <i>blank string - i.e. disabled</i>
    *  @dtopt Options
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sScrollX": "100%",
    *        "sScrollXInner": "110%"
    *      } );
    *    } );
    */
    "sScrollXInner": "",


    /**
    * Enable vertical scrolling. Vertical scrolling will constrain the DataTable
    * to the given height, and enable scrolling for any data which overflows the
    * current viewport. This can be used as an alternative to paging to display
    * a lot of data in a small area (although paging and scrolling can both be
    * enabled at the same time). This property can be any CSS unit, or a number
    * (in which case it will be treated as a pixel measurement).
    *  @type string
    *  @default <i>blank string - i.e. disabled</i>
    *  @dtopt Features
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "sScrollY": "200px",
    *        "bPaginate": false
    *      } );
    *    } );
    */
    "sScrollY": "",


    /**
    * Set the HTTP method that is used to make the Ajax call for server-side
    * processing or Ajax sourced data.
    *  @type string
    *  @default GET
    *  @dtopt Options
    *  @dtopt Server-side
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "bServerSide": true,
    *        "sAjaxSource": "scripts/post.php",
    *        "sServerMethod": "POST"
    *      } );
    *    } );
    */
    "sServerMethod": "GET"
  };



  /**
  * Column options that can be given to DataTables at initialisation time.
  *  @namespace
  */
  DataTable.defaults.columns = {
    /**
    * Allows a column's sorting to take multiple columns into account when 
    * doing a sort. For example first name / last name columns make sense to 
    * do a multi-column sort over the two columns.
    *  @type array
    *  @default null <i>Takes the value of the column index automatically</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [
    *          { "aDataSort": [ 0, 1 ], "aTargets": [ 0 ] },
    *          { "aDataSort": [ 1, 0 ], "aTargets": [ 1 ] },
    *          { "aDataSort": [ 2, 3, 4 ], "aTargets": [ 2 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [
    *          { "aDataSort": [ 0, 1 ] },
    *          { "aDataSort": [ 1, 0 ] },
    *          { "aDataSort": [ 2, 3, 4 ] },
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "aDataSort": null,


    /**
    * You can control the default sorting direction, and even alter the behaviour
    * of the sort handler (i.e. only allow ascending sorting etc) using this
    * parameter.
    *  @type array
    *  @default [ 'asc', 'desc' ]
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [
    *          { "asSorting": [ "asc" ], "aTargets": [ 1 ] },
    *          { "asSorting": [ "desc", "asc", "asc" ], "aTargets": [ 2 ] },
    *          { "asSorting": [ "desc" ], "aTargets": [ 3 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [
    *          null,
    *          { "asSorting": [ "asc" ] },
    *          { "asSorting": [ "desc", "asc", "asc" ] },
    *          { "asSorting": [ "desc" ] },
    *          null
    *        ]
    *      } );
    *    } );
    */
    "asSorting": ['asc', 'desc'],


    /**
    * Enable or disable filtering on the data in this column.
    *  @type boolean
    *  @default true
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "bSearchable": false, "aTargets": [ 0 ] }
    *        ] } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "bSearchable": false },
    *          null,
    *          null,
    *          null,
    *          null
    *        ] } );
    *    } );
    */
    "bSearchable": true,


    /**
    * Enable or disable sorting on this column.
    *  @type boolean
    *  @default true
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "bSortable": false, "aTargets": [ 0 ] }
    *        ] } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "bSortable": false },
    *          null,
    *          null,
    *          null,
    *          null
    *        ] } );
    *    } );
    */
    "bSortable": true,


    /**
    * When using fnRender() for a column, you may wish to use the original data
    * (before rendering) for sorting and filtering (the default is to used the
    * rendered data that the user can see). This may be useful for dates etc.
    * 
    * *NOTE* This property is now deprecated, and it is suggested that you use
    * mData and / or mRender to render data for the DataTable.
    *  @type boolean
    *  @default true
    *  @dtopt Columns
    *  @deprecated
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          {
    *            "fnRender": function ( oObj ) {
    *              return oObj.aData[0] +' '+ oObj.aData[3];
    *            },
    *            "bUseRendered": false,
    *            "aTargets": [ 0 ]
    *          }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          {
    *            "fnRender": function ( oObj ) {
    *              return oObj.aData[0] +' '+ oObj.aData[3];
    *            },
    *            "bUseRendered": false
    *          },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "bUseRendered": true,


    /**
    * Enable or disable the display of this column.
    *  @type boolean
    *  @default true
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "bVisible": false, "aTargets": [ 0 ] }
    *        ] } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "bVisible": false },
    *          null,
    *          null,
    *          null,
    *          null
    *        ] } );
    *    } );
    */
    "bVisible": true,


    /**
    * Developer definable function that is called whenever a cell is created (Ajax source,
    * etc) or processed for input (DOM source). This can be used as a compliment to fnRender
    * allowing you to modify the DOM element (add background colour for example) when the
    * element is available (since it is not when fnRender is called).
    *  @type function
    *  @param {element} nTd The TD node that has been created
    *  @param {*} sData The Data for the cell
    *  @param {array|object} oData The data for the whole row
    *  @param {int} iRow The row index for the aoData data store
    *  @param {int} iCol The column index for aoColumns
    *  @dtopt Columns
    * 
    *  @example
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ {
    *          "aTargets": [3],
    *          "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
    *            if ( sData == "1.7" ) {
    *              $(nTd).css('color', 'blue')
    *            }
    *          }
    *        } ]
    *      });
    *    } );
    */
    "fnCreatedCell": null,


    /**
    * Custom display function that will be called for the display of each cell in
    * this column.
    *  @type function
    *  @param {object} o Object with the following parameters:
    *  @param {int}    o.iDataRow The row in aoData
    *  @param {int}    o.iDataColumn The column in question
    *  @param {array}  o.aData The data for the row in question
    *  @param {object} o.oSettings The settings object for this DataTables instance
    *  @param {object} o.mDataProp The data property used for this column
    *  @param {*}      val The current cell value
    *  @returns {string} The string you which to use in the display
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          {
    *            "fnRender": function ( o, val ) {
    *              return o.aData[0] +' '+ o.aData[3];
    *            },
    *            "aTargets": [ 0 ]
    *          }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "fnRender": function ( o, val ) {
    *            return o.aData[0] +' '+ o.aData[3];
    *          } },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "fnRender": null,


    /**
    * The column index (starting from 0!) that you wish a sort to be performed
    * upon when this column is selected for sorting. This can be used for sorting
    * on hidden columns for example.
    *  @type int
    *  @default -1 <i>Use automatically calculated column index</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "iDataSort": 1, "aTargets": [ 0 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "iDataSort": 1 },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "iDataSort": -1,


    /**
    * This parameter has been replaced by mData in DataTables to ensure naming
    * consistency. mDataProp can still be used, as there is backwards compatibility
    * in DataTables for this option, but it is strongly recommended that you use
    * mData in preference to mDataProp.
    *  @name DataTable.defaults.columns.mDataProp
    */


    /**
    * This property can be used to read data from any JSON data source property,
    * including deeply nested objects / properties. mData can be given in a
    * number of different ways which effect its behaviour:
    *   <ul>
    *     <li>integer - treated as an array index for the data source. This is the
    *       default that DataTables uses (incrementally increased for each column).</li>
    *     <li>string - read an object property from the data source. Note that you can
    *       use Javascript dotted notation to read deep properties / arrays from the
    *       data source.</li>
    *     <li>null - the sDefaultContent option will be used for the cell (null
    *       by default, so you will need to specify the default content you want -
    *       typically an empty string). This can be useful on generated columns such 
    *       as edit / delete action columns.</li>
    *     <li>function - the function given will be executed whenever DataTables 
    *       needs to set or get the data for a cell in the column. The function 
    *       takes three parameters:
    *       <ul>
    *         <li>{array|object} The data source for the row</li>
    *         <li>{string} The type call data requested - this will be 'set' when
    *           setting data or 'filter', 'display', 'type', 'sort' or undefined when 
    *           gathering data. Note that when <i>undefined</i> is given for the type
    *           DataTables expects to get the raw data for the object back</li>
    *         <li>{*} Data to set when the second parameter is 'set'.</li>
    *       </ul>
    *       The return value from the function is not required when 'set' is the type
    *       of call, but otherwise the return is what will be used for the data
    *       requested.</li>
    *    </ul>
    *
    * Note that prior to DataTables 1.9.2 mData was called mDataProp. The name change
    * reflects the flexibility of this property and is consistent with the naming of
    * mRender. If 'mDataProp' is given, then it will still be used by DataTables, as
    * it automatically maps the old name to the new if required.
    *  @type string|int|function|null
    *  @default null <i>Use automatically calculated column index</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Read table data from objects
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "sAjaxSource": "sources/deep.txt",
    *        "aoColumns": [
    *          { "mData": "engine" },
    *          { "mData": "browser" },
    *          { "mData": "platform.inner" },
    *          { "mData": "platform.details.0" },
    *          { "mData": "platform.details.1" }
    *        ]
    *      } );
    *    } );
    * 
    *  @example
    *    // Using mData as a function to provide different information for
    *    // sorting, filtering and display. In this case, currency (price)
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "aoColumnDefs": [ {
    *          "aTargets": [ 0 ],
    *          "mData": function ( source, type, val ) {
    *            if (type === 'set') {
    *              source.price = val;
    *              // Store the computed dislay and filter values for efficiency
    *              source.price_display = val=="" ? "" : "$"+numberFormat(val);
    *              source.price_filter  = val=="" ? "" : "$"+numberFormat(val)+" "+val;
    *              return;
    *            }
    *            else if (type === 'display') {
    *              return source.price_display;
    *            }
    *            else if (type === 'filter') {
    *              return source.price_filter;
    *            }
    *            // 'sort', 'type' and undefined all just use the integer
    *            return source.price;
    *          }
    *        } ]
    *      } );
    *    } );
    */
    "mData": null,


    /**
    * This property is the rendering partner to mData and it is suggested that
    * when you want to manipulate data for display (including filtering, sorting etc)
    * but not altering the underlying data for the table, use this property. mData
    * can actually do everything this property can and more, but this parameter is
    * easier to use since there is no 'set' option. Like mData is can be given
    * in a number of different ways to effect its behaviour, with the addition of 
    * supporting array syntax for easy outputting of arrays (including arrays of
    * objects):
    *   <ul>
    *     <li>integer - treated as an array index for the data source. This is the
    *       default that DataTables uses (incrementally increased for each column).</li>
    *     <li>string - read an object property from the data source. Note that you can
    *       use Javascript dotted notation to read deep properties / arrays from the
    *       data source and also array brackets to indicate that the data reader should
    *       loop over the data source array. When characters are given between the array
    *       brackets, these characters are used to join the data source array together.
    *       For example: "accounts[, ].name" would result in a comma separated list with
    *       the 'name' value from the 'accounts' array of objects.</li>
    *     <li>function - the function given will be executed whenever DataTables 
    *       needs to set or get the data for a cell in the column. The function 
    *       takes three parameters:
    *       <ul>
    *         <li>{array|object} The data source for the row (based on mData)</li>
    *         <li>{string} The type call data requested - this will be 'filter', 'display', 
    *           'type' or 'sort'.</li>
    *         <li>{array|object} The full data source for the row (not based on mData)</li>
    *       </ul>
    *       The return value from the function is what will be used for the data
    *       requested.</li>
    *    </ul>
    *  @type string|int|function|null
    *  @default null <i>Use mData</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Create a comma separated list from an array of objects
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "sAjaxSource": "sources/deep.txt",
    *        "aoColumns": [
    *          { "mData": "engine" },
    *          { "mData": "browser" },
    *          {
    *            "mData": "platform",
    *            "mRender": "[, ].name"
    *          }
    *        ]
    *      } );
    *    } );
    * 
    *  @example
    *    // Use as a function to create a link from the data source
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "aoColumnDefs": [
    *        {
    *          "aTargets": [ 0 ],
    *          "mData": "download_link",
    *          "mRender": function ( data, type, full ) {
    *            return '<a href="'+data+'">Download</a>';
    *          }
    *        ]
    *      } );
    *    } );
    */
    "mRender": null,


    /**
    * Change the cell type created for the column - either TD cells or TH cells. This
    * can be useful as TH cells have semantic meaning in the table body, allowing them
    * to act as a header for a row (you may wish to add scope='row' to the TH elements).
    *  @type string
    *  @default td
    *  @dtopt Columns
    * 
    *  @example
    *    // Make the first column use TH cells
    *    $(document).ready( function() {
    *      var oTable = $('#example').dataTable( {
    *        "aoColumnDefs": [ {
    *          "aTargets": [ 0 ],
    *          "sCellType": "th"
    *        } ]
    *      } );
    *    } );
    */
    "sCellType": "td",


    /**
    * Class to give to each cell in this column.
    *  @type string
    *  @default <i>Empty string</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "sClass": "my_class", "aTargets": [ 0 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "sClass": "my_class" },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "sClass": "",

    /**
    * When DataTables calculates the column widths to assign to each column,
    * it finds the longest string in each column and then constructs a
    * temporary table and reads the widths from that. The problem with this
    * is that "mmm" is much wider then "iiii", but the latter is a longer 
    * string - thus the calculation can go wrong (doing it properly and putting
    * it into an DOM object and measuring that is horribly(!) slow). Thus as
    * a "work around" we provide this option. It will append its value to the
    * text that is found to be the longest string for the column - i.e. padding.
    * Generally you shouldn't need this, and it is not documented on the 
    * general DataTables.net documentation
    *  @type string
    *  @default <i>Empty string<i>
    *  @dtopt Columns
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          null,
    *          null,
    *          null,
    *          {
    *            "sContentPadding": "mmm"
    *          }
    *        ]
    *      } );
    *    } );
    */
    "sContentPadding": "",


    /**
    * Allows a default value to be given for a column's data, and will be used
    * whenever a null data source is encountered (this can be because mData
    * is set to null, or because the data source itself is null).
    *  @type string
    *  @default null
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          {
    *            "mData": null,
    *            "sDefaultContent": "Edit",
    *            "aTargets": [ -1 ]
    *          }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          null,
    *          null,
    *          null,
    *          {
    *            "mData": null,
    *            "sDefaultContent": "Edit"
    *          }
    *        ]
    *      } );
    *    } );
    */
    "sDefaultContent": null,


    /**
    * This parameter is only used in DataTables' server-side processing. It can
    * be exceptionally useful to know what columns are being displayed on the
    * client side, and to map these to database fields. When defined, the names
    * also allow DataTables to reorder information from the server if it comes
    * back in an unexpected order (i.e. if you switch your columns around on the
    * client-side, your server-side code does not also need updating).
    *  @type string
    *  @default <i>Empty string</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "sName": "engine", "aTargets": [ 0 ] },
    *          { "sName": "browser", "aTargets": [ 1 ] },
    *          { "sName": "platform", "aTargets": [ 2 ] },
    *          { "sName": "version", "aTargets": [ 3 ] },
    *          { "sName": "grade", "aTargets": [ 4 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "sName": "engine" },
    *          { "sName": "browser" },
    *          { "sName": "platform" },
    *          { "sName": "version" },
    *          { "sName": "grade" }
    *        ]
    *      } );
    *    } );
    */
    "sName": "",


    /**
    * Defines a data source type for the sorting which can be used to read
    * real-time information from the table (updating the internally cached
    * version) prior to sorting. This allows sorting to occur on user editable
    * elements such as form inputs.
    *  @type string
    *  @default std
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [
    *          { "sSortDataType": "dom-text", "aTargets": [ 2, 3 ] },
    *          { "sType": "numeric", "aTargets": [ 3 ] },
    *          { "sSortDataType": "dom-select", "aTargets": [ 4 ] },
    *          { "sSortDataType": "dom-checkbox", "aTargets": [ 5 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [
    *          null,
    *          null,
    *          { "sSortDataType": "dom-text" },
    *          { "sSortDataType": "dom-text", "sType": "numeric" },
    *          { "sSortDataType": "dom-select" },
    *          { "sSortDataType": "dom-checkbox" }
    *        ]
    *      } );
    *    } );
    */
    "sSortDataType": "std",


    /**
    * The title of this column.
    *  @type string
    *  @default null <i>Derived from the 'TH' value for this column in the 
    *    original HTML table.</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "sTitle": "My column title", "aTargets": [ 0 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "sTitle": "My column title" },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "sTitle": null,


    /**
    * The type allows you to specify how the data for this column will be sorted.
    * Four types (string, numeric, date and html (which will strip HTML tags
    * before sorting)) are currently available. Note that only date formats
    * understood by Javascript's Date() object will be accepted as type date. For
    * example: "Mar 26, 2008 5:03 PM". May take the values: 'string', 'numeric',
    * 'date' or 'html' (by default). Further types can be adding through
    * plug-ins.
    *  @type string
    *  @default null <i>Auto-detected from raw data</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "sType": "html", "aTargets": [ 0 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "sType": "html" },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "sType": null,


    /**
    * Defining the width of the column, this parameter may take any CSS value
    * (3em, 20px etc). DataTables apples 'smart' widths to columns which have not
    * been given a specific width through this interface ensuring that the table
    * remains readable.
    *  @type string
    *  @default null <i>Automatic</i>
    *  @dtopt Columns
    * 
    *  @example
    *    // Using aoColumnDefs
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumnDefs": [ 
    *          { "sWidth": "20%", "aTargets": [ 0 ] }
    *        ]
    *      } );
    *    } );
    *    
    *  @example
    *    // Using aoColumns
    *    $(document).ready( function() {
    *      $('#example').dataTable( {
    *        "aoColumns": [ 
    *          { "sWidth": "20%" },
    *          null,
    *          null,
    *          null,
    *          null
    *        ]
    *      } );
    *    } );
    */
    "sWidth": null
  };



  /**
  * DataTables settings object - this holds all the information needed for a
  * given table, including configuration, data and current application of the
  * table options. DataTables does not have a single instance for each DataTable
  * with the settings attached to that instance, but rather instances of the
  * DataTable "class" are created on-the-fly as needed (typically by a 
  * $().dataTable() call) and the settings object is then applied to that
  * instance.
  * 
  * Note that this object is related to {@link DataTable.defaults} but this 
  * one is the internal data store for DataTables's cache of columns. It should
  * NOT be manipulated outside of DataTables. Any configuration should be done
  * through the initialisation options.
  *  @namespace
  *  @todo Really should attach the settings object to individual instances so we
  *    don't need to create new instances on each $().dataTable() call (if the
  *    table already exists). It would also save passing oSettings around and
  *    into every single function. However, this is a very significant 
  *    architecture change for DataTables and will almost certainly break
  *    backwards compatibility with older installations. This is something that
  *    will be done in 2.0.
  */
  DataTable.models.oSettings = {
    /**
    * Primary features of DataTables and their enablement state.
    *  @namespace
    */
    "oFeatures": {

      /**
      * Flag to say if DataTables should automatically try to calculate the
      * optimum table and columns widths (true) or not (false).
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bAutoWidth": null,

      /**
      * Delay the creation of TR and TD elements until they are actually
      * needed by a driven page draw. This can give a significant speed
      * increase for Ajax source and Javascript source data, but makes no
      * difference at all fro DOM and server-side processing tables.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bDeferRender": null,

      /**
      * Enable filtering on the table or not. Note that if this is disabled
      * then there is no filtering at all on the table, including fnFilter.
      * To just remove the filtering input use sDom and remove the 'f' option.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bFilter": null,

      /**
      * Table information element (the 'Showing x of y records' div) enable
      * flag.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bInfo": null,

      /**
      * Present a user control allowing the end user to change the page size
      * when pagination is enabled.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bLengthChange": null,

      /**
      * Pagination enabled or not. Note that if this is disabled then length
      * changing must also be disabled.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bPaginate": null,

      /**
      * Processing indicator enable flag whenever DataTables is enacting a
      * user request - typically an Ajax request for server-side processing.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bProcessing": null,

      /**
      * Server-side processing enabled flag - when enabled DataTables will
      * get all data from the server for every draw - there is no filtering,
      * sorting or paging done on the client-side.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bServerSide": null,

      /**
      * Sorting enablement flag.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bSort": null,

      /**
      * Apply a class to the columns which are being sorted to provide a
      * visual highlight or not. This can slow things down when enabled since
      * there is a lot of DOM interaction.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bSortClasses": null,

      /**
      * State saving enablement flag.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bStateSave": null
    },


    /**
    * Scrolling settings for a table.
    *  @namespace
    */
    "oScroll": {
      /**
      * Indicate if DataTables should be allowed to set the padding / margin
      * etc for the scrolling header elements or not. Typically you will want
      * this.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bAutoCss": null,

      /**
      * When the table is shorter in height than sScrollY, collapse the
      * table container down to the height of the table (when true).
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bCollapse": null,

      /**
      * Infinite scrolling enablement flag. Now deprecated in favour of
      * using the Scroller plug-in.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type boolean
      */
      "bInfinite": null,

      /**
      * Width of the scrollbar for the web-browser's platform. Calculated
      * during table initialisation.
      *  @type int
      *  @default 0
      */
      "iBarWidth": 0,

      /**
      * Space (in pixels) between the bottom of the scrolling container and 
      * the bottom of the scrolling viewport before the next page is loaded
      * when using infinite scrolling.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type int
      */
      "iLoadGap": null,

      /**
      * Viewport width for horizontal scrolling. Horizontal scrolling is 
      * disabled if an empty string.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type string
      */
      "sX": null,

      /**
      * Width to expand the table to when using x-scrolling. Typically you
      * should not need to use this.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type string
      *  @deprecated
      */
      "sXInner": null,

      /**
      * Viewport height for vertical scrolling. Vertical scrolling is disabled
      * if an empty string.
      * Note that this parameter will be set by the initialisation routine. To
      * set a default use {@link DataTable.defaults}.
      *  @type string
      */
      "sY": null
    },

    /**
    * Language information for the table.
    *  @namespace
    *  @extends DataTable.defaults.oLanguage
    */
    "oLanguage": {
      /**
      * Information callback function. See 
      * {@link DataTable.defaults.fnInfoCallback}
      *  @type function
      *  @default null
      */
      "fnInfoCallback": null
    },

    /**
    * Browser support parameters
    *  @namespace
    */
    "oBrowser": {
      /**
      * Indicate if the browser incorrectly calculates width:100% inside a
      * scrolling element (IE6/7)
      *  @type boolean
      *  @default false
      */
      "bScrollOversize": false
    },

    /**
    * Array referencing the nodes which are used for the features. The 
    * parameters of this object match what is allowed by sDom - i.e.
    *   <ul>
    *     <li>'l' - Length changing</li>
    *     <li>'f' - Filtering input</li>
    *     <li>'t' - The table!</li>
    *     <li>'i' - Information</li>
    *     <li>'p' - Pagination</li>
    *     <li>'r' - pRocessing</li>
    *   </ul>
    *  @type array
    *  @default []
    */
    "aanFeatures": [],

    /**
    * Store data information - see {@link DataTable.models.oRow} for detailed
    * information.
    *  @type array
    *  @default []
    */
    "aoData": [],

    /**
    * Array of indexes which are in the current display (after filtering etc)
    *  @type array
    *  @default []
    */
    "aiDisplay": [],

    /**
    * Array of indexes for display - no filtering
    *  @type array
    *  @default []
    */
    "aiDisplayMaster": [],

    /**
    * Store information about each column that is in use
    *  @type array
    *  @default []
    */
    "aoColumns": [],

    /**
    * Store information about the table's header
    *  @type array
    *  @default []
    */
    "aoHeader": [],

    /**
    * Store information about the table's footer
    *  @type array
    *  @default []
    */
    "aoFooter": [],

    /**
    * Search data array for regular expression searching
    *  @type array
    *  @default []
    */
    "asDataSearch": [],

    /**
    * Store the applied global search information in case we want to force a 
    * research or compare the old search to a new one.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @namespace
    *  @extends DataTable.models.oSearch
    */
    "oPreviousSearch": {},

    /**
    * Store the applied search for each column - see 
    * {@link DataTable.models.oSearch} for the format that is used for the
    * filtering information for each column.
    *  @type array
    *  @default []
    */
    "aoPreSearchCols": [],

    /**
    * Sorting that is applied to the table. Note that the inner arrays are
    * used in the following manner:
    * <ul>
    *   <li>Index 0 - column number</li>
    *   <li>Index 1 - current sorting direction</li>
    *   <li>Index 2 - index of asSorting for this column</li>
    * </ul>
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type array
    *  @todo These inner arrays should really be objects
    */
    "aaSorting": null,

    /**
    * Sorting that is always applied to the table (i.e. prefixed in front of
    * aaSorting).
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type array|null
    *  @default null
    */
    "aaSortingFixed": null,

    /**
    * Classes to use for the striping of a table.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type array
    *  @default []
    */
    "asStripeClasses": null,

    /**
    * If restoring a table - we should restore its striping classes as well
    *  @type array
    *  @default []
    */
    "asDestroyStripes": [],

    /**
    * If restoring a table - we should restore its width 
    *  @type int
    *  @default 0
    */
    "sDestroyWidth": 0,

    /**
    * Callback functions array for every time a row is inserted (i.e. on a draw).
    *  @type array
    *  @default []
    */
    "aoRowCallback": [],

    /**
    * Callback functions for the header on each draw.
    *  @type array
    *  @default []
    */
    "aoHeaderCallback": [],

    /**
    * Callback function for the footer on each draw.
    *  @type array
    *  @default []
    */
    "aoFooterCallback": [],

    /**
    * Array of callback functions for draw callback functions
    *  @type array
    *  @default []
    */
    "aoDrawCallback": [],

    /**
    * Array of callback functions for row created function
    *  @type array
    *  @default []
    */
    "aoRowCreatedCallback": [],

    /**
    * Callback functions for just before the table is redrawn. A return of 
    * false will be used to cancel the draw.
    *  @type array
    *  @default []
    */
    "aoPreDrawCallback": [],

    /**
    * Callback functions for when the table has been initialised.
    *  @type array
    *  @default []
    */
    "aoInitComplete": [],


    /**
    * Callbacks for modifying the settings to be stored for state saving, prior to
    * saving state.
    *  @type array
    *  @default []
    */
    "aoStateSaveParams": [],

    /**
    * Callbacks for modifying the settings that have been stored for state saving
    * prior to using the stored values to restore the state.
    *  @type array
    *  @default []
    */
    "aoStateLoadParams": [],

    /**
    * Callbacks for operating on the settings object once the saved state has been
    * loaded
    *  @type array
    *  @default []
    */
    "aoStateLoaded": [],

    /**
    * Cache the table ID for quick access
    *  @type string
    *  @default <i>Empty string</i>
    */
    "sTableId": "",

    /**
    * The TABLE node for the main table
    *  @type node
    *  @default null
    */
    "nTable": null,

    /**
    * Permanent ref to the thead element
    *  @type node
    *  @default null
    */
    "nTHead": null,

    /**
    * Permanent ref to the tfoot element - if it exists
    *  @type node
    *  @default null
    */
    "nTFoot": null,

    /**
    * Permanent ref to the tbody element
    *  @type node
    *  @default null
    */
    "nTBody": null,

    /**
    * Cache the wrapper node (contains all DataTables controlled elements)
    *  @type node
    *  @default null
    */
    "nTableWrapper": null,

    /**
    * Indicate if when using server-side processing the loading of data 
    * should be deferred until the second draw.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type boolean
    *  @default false
    */
    "bDeferLoading": false,

    /**
    * Indicate if all required information has been read in
    *  @type boolean
    *  @default false
    */
    "bInitialised": false,

    /**
    * Information about open rows. Each object in the array has the parameters
    * 'nTr' and 'nParent'
    *  @type array
    *  @default []
    */
    "aoOpenRows": [],

    /**
    * Dictate the positioning of DataTables' control elements - see
    * {@link DataTable.model.oInit.sDom}.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string
    *  @default null
    */
    "sDom": null,

    /**
    * Which type of pagination should be used.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string 
    *  @default two_button
    */
    "sPaginationType": "two_button",

    /**
    * The cookie duration (for bStateSave) in seconds.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type int
    *  @default 0
    */
    "iCookieDuration": 0,

    /**
    * The cookie name prefix.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string
    *  @default <i>Empty string</i>
    */
    "sCookiePrefix": "",

    /**
    * Callback function for cookie creation.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type function
    *  @default null
    */
    "fnCookieCallback": null,

    /**
    * Array of callback functions for state saving. Each array element is an 
    * object with the following parameters:
    *   <ul>
    *     <li>function:fn - function to call. Takes two parameters, oSettings
    *       and the JSON string to save that has been thus far created. Returns
    *       a JSON string to be inserted into a json object 
    *       (i.e. '"param": [ 0, 1, 2]')</li>
    *     <li>string:sName - name of callback</li>
    *   </ul>
    *  @type array
    *  @default []
    */
    "aoStateSave": [],

    /**
    * Array of callback functions for state loading. Each array element is an 
    * object with the following parameters:
    *   <ul>
    *     <li>function:fn - function to call. Takes two parameters, oSettings 
    *       and the object stored. May return false to cancel state loading</li>
    *     <li>string:sName - name of callback</li>
    *   </ul>
    *  @type array
    *  @default []
    */
    "aoStateLoad": [],

    /**
    * State that was loaded from the cookie. Useful for back reference
    *  @type object
    *  @default null
    */
    "oLoadedState": null,

    /**
    * Source url for AJAX data for the table.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string
    *  @default null
    */
    "sAjaxSource": null,

    /**
    * Property from a given object from which to read the table data from. This
    * can be an empty string (when not server-side processing), in which case 
    * it is  assumed an an array is given directly.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string
    */
    "sAjaxDataProp": null,

    /**
    * Note if draw should be blocked while getting data
    *  @type boolean
    *  @default true
    */
    "bAjaxDataGet": true,

    /**
    * The last jQuery XHR object that was used for server-side data gathering. 
    * This can be used for working with the XHR information in one of the 
    * callbacks
    *  @type object
    *  @default null
    */
    "jqXHR": null,

    /**
    * Function to get the server-side data.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type function
    */
    "fnServerData": null,

    /**
    * Functions which are called prior to sending an Ajax request so extra 
    * parameters can easily be sent to the server
    *  @type array
    *  @default []
    */
    "aoServerParams": [],

    /**
    * Send the XHR HTTP method - GET or POST (could be PUT or DELETE if 
    * required).
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type string
    */
    "sServerMethod": null,

    /**
    * Format numbers for display.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type function
    */
    "fnFormatNumber": null,

    /**
    * List of options that can be used for the user selectable length menu.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type array
    *  @default []
    */
    "aLengthMenu": null,

    /**
    * Counter for the draws that the table does. Also used as a tracker for
    * server-side processing
    *  @type int
    *  @default 0
    */
    "iDraw": 0,

    /**
    * Indicate if a redraw is being done - useful for Ajax
    *  @type boolean
    *  @default false
    */
    "bDrawing": false,

    /**
    * Draw index (iDraw) of the last error when parsing the returned data
    *  @type int
    *  @default -1
    */
    "iDrawError": -1,

    /**
    * Paging display length
    *  @type int
    *  @default 10
    */
    "_iDisplayLength": 10,

    /**
    * Paging start point - aiDisplay index
    *  @type int
    *  @default 0
    */
    "_iDisplayStart": 0,

    /**
    * Paging end point - aiDisplay index. Use fnDisplayEnd rather than
    * this property to get the end point
    *  @type int
    *  @default 10
    *  @private
    */
    "_iDisplayEnd": 10,

    /**
    * Server-side processing - number of records in the result set
    * (i.e. before filtering), Use fnRecordsTotal rather than
    * this property to get the value of the number of records, regardless of
    * the server-side processing setting.
    *  @type int
    *  @default 0
    *  @private
    */
    "_iRecordsTotal": 0,

    /**
    * Server-side processing - number of records in the current display set
    * (i.e. after filtering). Use fnRecordsDisplay rather than
    * this property to get the value of the number of records, regardless of
    * the server-side processing setting.
    *  @type boolean
    *  @default 0
    *  @private
    */
    "_iRecordsDisplay": 0,

    /**
    * Flag to indicate if jQuery UI marking and classes should be used.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type boolean
    */
    "bJUI": null,

    /**
    * The classes to use for the table
    *  @type object
    *  @default {}
    */
    "oClasses": {},

    /**
    * Flag attached to the settings object so you can check in the draw 
    * callback if filtering has been done in the draw. Deprecated in favour of
    * events.
    *  @type boolean
    *  @default false
    *  @deprecated
    */
    "bFiltered": false,

    /**
    * Flag attached to the settings object so you can check in the draw 
    * callback if sorting has been done in the draw. Deprecated in favour of
    * events.
    *  @type boolean
    *  @default false
    *  @deprecated
    */
    "bSorted": false,

    /**
    * Indicate that if multiple rows are in the header and there is more than 
    * one unique cell per column, if the top one (true) or bottom one (false) 
    * should be used for sorting / title by DataTables.
    * Note that this parameter will be set by the initialisation routine. To
    * set a default use {@link DataTable.defaults}.
    *  @type boolean
    */
    "bSortCellsTop": null,

    /**
    * Initialisation object that is used for the table
    *  @type object
    *  @default null
    */
    "oInit": null,

    /**
    * Destroy callback functions - for plug-ins to attach themselves to the
    * destroy so they can clean up markup and events.
    *  @type array
    *  @default []
    */
    "aoDestroyCallback": [],


    /**
    * Get the number of records in the current record set, before filtering
    *  @type function
    */
    "fnRecordsTotal": function () {
      if (this.oFeatures.bServerSide) {
        return parseInt(this._iRecordsTotal, 10);
      } else {
        return this.aiDisplayMaster.length;
      }
    },

    /**
    * Get the number of records in the current record set, after filtering
    *  @type function
    */
    "fnRecordsDisplay": function () {
      if (this.oFeatures.bServerSide) {
        return parseInt(this._iRecordsDisplay, 10);
      } else {
        return this.aiDisplay.length;
      }
    },

    /**
    * Set the display end point - aiDisplay index
    *  @type function
    *  @todo Should do away with _iDisplayEnd and calculate it on-the-fly here
    */
    "fnDisplayEnd": function () {
      if (this.oFeatures.bServerSide) {
        if (this.oFeatures.bPaginate === false || this._iDisplayLength == -1) {
          return this._iDisplayStart + this.aiDisplay.length;
        } else {
          return Math.min(this._iDisplayStart + this._iDisplayLength, this._iRecordsDisplay);
        }
      } else {
        return this._iDisplayEnd;
      }
    },

    /**
    * The DataTables object for this table
    *  @type object
    *  @default null
    */
    "oInstance": null,

    /**
    * Unique identifier for each instance of the DataTables object. If there
    * is an ID on the table node, then it takes that value, otherwise an
    * incrementing internal counter is used.
    *  @type string
    *  @default null
    */
    "sInstance": null,

    /**
    * tabindex attribute value that is added to DataTables control elements, allowing
    * keyboard navigation of the table and its controls.
    */
    "iTabIndex": 0,

    /**
    * DIV container for the footer scrolling table if scrolling
    */
    "nScrollHead": null,

    /**
    * DIV container for the footer scrolling table if scrolling
    */
    "nScrollFoot": null
  };

  /**
  * Extension object for DataTables that is used to provide all extension options.
  * 
  * Note that the <i>DataTable.ext</i> object is available through
  * <i>jQuery.fn.dataTable.ext</i> where it may be accessed and manipulated. It is
  * also aliased to <i>jQuery.fn.dataTableExt</i> for historic reasons.
  *  @namespace
  *  @extends DataTable.models.ext
  */
  DataTable.ext = $.extend(true, {}, DataTable.models.ext);

  $.extend(DataTable.ext.oStdClasses, {
    "sTable": "dataTable",

    /* Two buttons buttons */"sPagePrevEnabled": "paginate_enabled_previous",
    "sPagePrevDisabled": "paginate_disabled_previous",
    "sPageNextEnabled": "paginate_enabled_next",
    "sPageNextDisabled": "paginate_disabled_next",
    "sPageJUINext": "",
    "sPageJUIPrev": "",

    /* Full numbers paging buttons */"sPageButton": "paginate_button",
    "sPageButtonActive": "paginate_active",
    "sPageButtonStaticDisabled": "paginate_button paginate_button_disabled",
    "sPageFirst": "first",
    "sPagePrevious": "previous",
    "sPageNext": "next",
    "sPageLast": "last",

    /* Striping classes */"sStripeOdd": "odd",
    "sStripeEven": "even",

    /* Empty row */"sRowEmpty": "dataTables_empty",

    /* Features */"sWrapper": "dataTables_wrapper",
    "sFilter": "dataTables_filter",
    "sInfo": "dataTables_info",
    "sPaging": "dataTables_paginate paging_",
    /* Note that the type is postfixed */
    "sLength": "dataTables_length",
    "sProcessing": "dataTables_processing",

    /* Sorting */"sSortAsc": "sorting_asc",
    "sSortDesc": "sorting_desc",
    "sSortable": "sorting",
    /* Sortable in both directions */
    "sSortableAsc": "sorting_asc_disabled",
    "sSortableDesc": "sorting_desc_disabled",
    "sSortableNone": "sorting_disabled",
    "sSortColumn": "sorting_",
    /* Note that an int is postfixed for the sorting order */
    "sSortJUIAsc": "",
    "sSortJUIDesc": "",
    "sSortJUI": "",
    "sSortJUIAscAllowed": "",
    "sSortJUIDescAllowed": "",
    "sSortJUIWrapper": "",
    "sSortIcon": "",

    /* Scrolling */"sScrollWrapper": "dataTables_scroll",
    "sScrollHead": "dataTables_scrollHead",
    "sScrollHeadInner": "dataTables_scrollHeadInner",
    "sScrollBody": "dataTables_scrollBody",
    "sScrollFoot": "dataTables_scrollFoot",
    "sScrollFootInner": "dataTables_scrollFootInner",

    /* Misc */"sFooterTH": "",
    "sJUIHeader": "",
    "sJUIFooter": ""
  });


  $.extend(DataTable.ext.oJUIClasses, DataTable.ext.oStdClasses, {
    /* Two buttons buttons */"sPagePrevEnabled": "fg-button ui-button ui-state-default ui-corner-left",
    "sPagePrevDisabled": "fg-button ui-button ui-state-default ui-corner-left ui-state-disabled",
    "sPageNextEnabled": "fg-button ui-button ui-state-default ui-corner-right",
    "sPageNextDisabled": "fg-button ui-button ui-state-default ui-corner-right ui-state-disabled",
    "sPageJUINext": "ui-icon ui-icon-circle-arrow-e",
    "sPageJUIPrev": "ui-icon ui-icon-circle-arrow-w",

    /* Full numbers paging buttons */"sPageButton": "fg-button ui-button ui-state-default",
    "sPageButtonActive": "fg-button ui-button ui-state-default ui-state-disabled",
    "sPageButtonStaticDisabled": "fg-button ui-button ui-state-default ui-state-disabled",
    "sPageFirst": "first ui-corner-tl ui-corner-bl",
    "sPageLast": "last ui-corner-tr ui-corner-br",

    /* Features */"sPaging": "dataTables_paginate fg-buttonset ui-buttonset fg-buttonset-multi " + "ui-buttonset-multi paging_",
    /* Note that the type is postfixed */

    /* Sorting */"sSortAsc": "ui-state-default",
    "sSortDesc": "ui-state-default",
    "sSortable": "ui-state-default",
    "sSortableAsc": "ui-state-default",
    "sSortableDesc": "ui-state-default",
    "sSortableNone": "ui-state-default",
    "sSortJUIAsc": "css_right ui-icon ui-icon-triangle-1-n",
    "sSortJUIDesc": "css_right ui-icon ui-icon-triangle-1-s",
    "sSortJUI": "css_right ui-icon ui-icon-carat-2-n-s",
    "sSortJUIAscAllowed": "css_right ui-icon ui-icon-carat-1-n",
    "sSortJUIDescAllowed": "css_right ui-icon ui-icon-carat-1-s",
    "sSortJUIWrapper": "DataTables_sort_wrapper",
    "sSortIcon": "DataTables_sort_icon",

    /* Scrolling */"sScrollHead": "dataTables_scrollHead ui-state-default",
    "sScrollFoot": "dataTables_scrollFoot ui-state-default",

    /* Misc */"sFooterTH": "ui-state-default",
    "sJUIHeader": "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix",
    "sJUIFooter": "fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
  });


  /*
  * Variable: oPagination
  * Purpose:  
  * Scope:    jQuery.fn.dataTableExt
  */
  $.extend(DataTable.ext.oPagination, {
    /*
    * Variable: two_button
    * Purpose:  Standard two button (forward/back) pagination
    * Scope:    jQuery.fn.dataTableExt.oPagination
    */
    "two_button": {
      /*
      * Function: oPagination.two_button.fnInit
      * Purpose:  Initialise dom elements required for pagination with forward/back buttons only
      * Returns:  -
      * Inputs:   object:oSettings - dataTables settings object
      *           node:nPaging - the DIV which contains this pagination control
      *           function:fnCallbackDraw - draw function which must be called on update
      */
      "fnInit": function (oSettings, nPaging, fnCallbackDraw) {
        var oLang = oSettings.oLanguage.oPaginate;
        var oClasses = oSettings.oClasses;
        var fnClickHandler = function (e) {
          if (oSettings.oApi._fnPageChange(oSettings, e.data.action)) {
            fnCallbackDraw(oSettings);
          }
        };

        var sAppend = (!oSettings.bJUI) ? '<a class="' + oSettings.oClasses.sPagePrevDisabled + '" tabindex="' + oSettings.iTabIndex + '" role="button">' + oLang.sPrevious + '</a>' + '<a class="' + oSettings.oClasses.sPageNextDisabled + '" tabindex="' + oSettings.iTabIndex + '" role="button">' + oLang.sNext + '</a>' : '<a class="' + oSettings.oClasses.sPagePrevDisabled + '" tabindex="' + oSettings.iTabIndex + '" role="button"><span class="' + oSettings.oClasses.sPageJUIPrev + '"></span></a>' + '<a class="' + oSettings.oClasses.sPageNextDisabled + '" tabindex="' + oSettings.iTabIndex + '" role="button"><span class="' + oSettings.oClasses.sPageJUINext + '"></span></a>';
        $(nPaging).append(sAppend);

        var els = $('a', nPaging);
        var nPrevious = els[0],
          nNext = els[1];

        oSettings.oApi._fnBindAction(nPrevious, {
          action: "previous"
        }, fnClickHandler);
        oSettings.oApi._fnBindAction(nNext, {
          action: "next"
        }, fnClickHandler);

        /* ID the first elements only */
        if (!oSettings.aanFeatures.p) {
          nPaging.id = oSettings.sTableId + '_paginate';
          nPrevious.id = oSettings.sTableId + '_previous';
          nNext.id = oSettings.sTableId + '_next';

          nPrevious.setAttribute('aria-controls', oSettings.sTableId);
          nNext.setAttribute('aria-controls', oSettings.sTableId);
        }
      },

      /*
      * Function: oPagination.two_button.fnUpdate
      * Purpose:  Update the two button pagination at the end of the draw
      * Returns:  -
      * Inputs:   object:oSettings - dataTables settings object
      *           function:fnCallbackDraw - draw function to call on page change
      */
      "fnUpdate": function (oSettings, fnCallbackDraw) {
        if (!oSettings.aanFeatures.p) {
          return;
        }

        var oClasses = oSettings.oClasses;
        var an = oSettings.aanFeatures.p;

        /* Loop over each instance of the pager */
        for (var i = 0, iLen = an.length; i < iLen; i++) {
          if (an[i].childNodes.length !== 0) {
            an[i].childNodes[0].className = (oSettings._iDisplayStart === 0) ? oClasses.sPagePrevDisabled : oClasses.sPagePrevEnabled;

            an[i].childNodes[1].className = (oSettings.fnDisplayEnd() == oSettings.fnRecordsDisplay()) ? oClasses.sPageNextDisabled : oClasses.sPageNextEnabled;
          }
        }
      }
    },


    /*
    * Variable: iFullNumbersShowPages
    * Purpose:  Change the number of pages which can be seen
    * Scope:    jQuery.fn.dataTableExt.oPagination
    */
    "iFullNumbersShowPages": 5,

    /*
    * Variable: full_numbers
    * Purpose:  Full numbers pagination
    * Scope:    jQuery.fn.dataTableExt.oPagination
    */
    "full_numbers": {
      /*
      * Function: oPagination.full_numbers.fnInit
      * Purpose:  Initialise dom elements required for pagination with a list of the pages
      * Returns:  -
      * Inputs:   object:oSettings - dataTables settings object
      *           node:nPaging - the DIV which contains this pagination control
      *           function:fnCallbackDraw - draw function which must be called on update
      */
      "fnInit": function (oSettings, nPaging, fnCallbackDraw) {
        var oLang = oSettings.oLanguage.oPaginate;
        var oClasses = oSettings.oClasses;
        var fnClickHandler = function (e) {
          if (oSettings.oApi._fnPageChange(oSettings, e.data.action)) {
            fnCallbackDraw(oSettings);
          }
        };

        $(nPaging).append('<a  tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButton + " " + oClasses.sPageFirst + '">' + oLang.sFirst + '</a>' + '<a  tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButton + " " + oClasses.sPagePrevious + '">' + oLang.sPrevious + '</a>' + '<span></span>' + '<a tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButton + " " + oClasses.sPageNext + '">' + oLang.sNext + '</a>' + '<a tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButton + " " + oClasses.sPageLast + '">' + oLang.sLast + '</a>');
        var els = $('a', nPaging);
        var nFirst = els[0],
          nPrev = els[1],
          nNext = els[2],
          nLast = els[3];

        oSettings.oApi._fnBindAction(nFirst, {
          action: "first"
        }, fnClickHandler);
        oSettings.oApi._fnBindAction(nPrev, {
          action: "previous"
        }, fnClickHandler);
        oSettings.oApi._fnBindAction(nNext, {
          action: "next"
        }, fnClickHandler);
        oSettings.oApi._fnBindAction(nLast, {
          action: "last"
        }, fnClickHandler);

        /* ID the first elements only */
        if (!oSettings.aanFeatures.p) {
          nPaging.id = oSettings.sTableId + '_paginate';
          nFirst.id = oSettings.sTableId + '_first';
          nPrev.id = oSettings.sTableId + '_previous';
          nNext.id = oSettings.sTableId + '_next';
          nLast.id = oSettings.sTableId + '_last';
        }
      },

      /*
      * Function: oPagination.full_numbers.fnUpdate
      * Purpose:  Update the list of page buttons shows
      * Returns:  -
      * Inputs:   object:oSettings - dataTables settings object
      *           function:fnCallbackDraw - draw function to call on page change
      */
      "fnUpdate": function (oSettings, fnCallbackDraw) {
        if (!oSettings.aanFeatures.p) {
          return;
        }

        var iPageCount = DataTable.ext.oPagination.iFullNumbersShowPages;
        var iPageCountHalf = Math.floor(iPageCount / 2);
        var iPages = Math.ceil((oSettings.fnRecordsDisplay()) / oSettings._iDisplayLength);
        var iCurrentPage = Math.ceil(oSettings._iDisplayStart / oSettings._iDisplayLength) + 1;
        var sList = "";
        var iStartButton, iEndButton, i, iLen;
        var oClasses = oSettings.oClasses;
        var anButtons, anStatic, nPaginateList;
        var an = oSettings.aanFeatures.p;
        var fnBind = function (j) {
          oSettings.oApi._fnBindAction(this, {
            "page": j + iStartButton - 1
          }, function (e) {
            /* Use the information in the element to jump to the required page */
            oSettings.oApi._fnPageChange(oSettings, e.data.page);
            fnCallbackDraw(oSettings);
            e.preventDefault();
          });
        };

        /* Pages calculation */
        if (oSettings._iDisplayLength === -1) {
          iStartButton = 1;
          iEndButton = 1;
          iCurrentPage = 1;
        } else if (iPages < iPageCount) {
          iStartButton = 1;
          iEndButton = iPages;
        } else if (iCurrentPage <= iPageCountHalf) {
          iStartButton = 1;
          iEndButton = iPageCount;
        } else if (iCurrentPage >= (iPages - iPageCountHalf)) {
          iStartButton = iPages - iPageCount + 1;
          iEndButton = iPages;
        } else {
          iStartButton = iCurrentPage - Math.ceil(iPageCount / 2) + 1;
          iEndButton = iStartButton + iPageCount - 1;
        }


        /* Build the dynamic list */
        for (i = iStartButton; i <= iEndButton; i++) {
          sList += (iCurrentPage !== i) ? '<a tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButton + '">' + oSettings.fnFormatNumber(i) + '</a>' : '<a tabindex="' + oSettings.iTabIndex + '" class="' + oClasses.sPageButtonActive + '">' + oSettings.fnFormatNumber(i) + '</a>';
        }

        /* Loop over each instance of the pager */
        for (i = 0, iLen = an.length; i < iLen; i++) {
          if (an[i].childNodes.length === 0) {
            continue;
          }

          /* Build up the dynamic list first - html and listeners */
          $('span:eq(0)', an[i]).html(sList).children('a').each(fnBind);

          /* Update the permanent button's classes */
          anButtons = an[i].getElementsByTagName('a');
          anStatic = [
          anButtons[0], anButtons[1], anButtons[anButtons.length - 2], anButtons[anButtons.length - 1]];

          $(anStatic).removeClass(oClasses.sPageButton + " " + oClasses.sPageButtonActive + " " + oClasses.sPageButtonStaticDisabled);
          $([anStatic[0], anStatic[1]]).addClass(
          (iCurrentPage == 1) ? oClasses.sPageButtonStaticDisabled : oClasses.sPageButton);
          $([anStatic[2], anStatic[3]]).addClass(
          (iPages === 0 || iCurrentPage === iPages || oSettings._iDisplayLength === -1) ? oClasses.sPageButtonStaticDisabled : oClasses.sPageButton);
        }
      }
    }
  });

  $.extend(DataTable.ext.oSort, {
    /*
    * text sorting
    */
    "string-pre": function (a) {
      if (typeof a != 'string') {
        a = (a !== null && a.toString) ? a.toString() : '';
      }
      return a.toLowerCase();
    },

    "string-asc": function (x, y) {
      return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    },

    "string-desc": function (x, y) {
      return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    },


    /*
    * html sorting (ignore html tags)
    */
    "html-pre": function (a) {
      return a.replace(/<.*?>/g, "").toLowerCase();
    },

    "html-asc": function (x, y) {
      return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    },

    "html-desc": function (x, y) {
      return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    },


    /*
    * date sorting
    */
    "date-pre": function (a) {
      var x = Date.parse(a);

      if (isNaN(x) || x === "") {
        x = Date.parse("01/01/1970 00:00:00");
      }
      return x;
    },

    "date-asc": function (x, y) {
      return x - y;
    },

    "date-desc": function (x, y) {
      return y - x;
    },


    /*
    * numerical sorting
    */
    "numeric-pre": function (a) {
      return (a == "-" || a === "") ? 0 : a * 1;
    },

    "numeric-asc": function (x, y) {
      return x - y;
    },

    "numeric-desc": function (x, y) {
      return y - x;
    }
  });


  $.extend(DataTable.ext.aTypes, [
  /*
  * Function: -
  * Purpose:  Check to see if a string is numeric
  * Returns:  string:'numeric' or null
  * Inputs:   mixed:sText - string to check
  */
  function (sData) {
    /* Allow zero length strings as a number */
    if (typeof sData === 'number') {
      return 'numeric';
    } else if (typeof sData !== 'string') {
      return null;
    }

    var sValidFirstChars = "0123456789-";
    var sValidChars = "0123456789.";
    var Char;
    var bDecimal = false;

    /* Check for a valid first char (no period and allow negatives) */
    Char = sData.charAt(0);
    if (sValidFirstChars.indexOf(Char) == -1) {
      return null;
    }

    /* Check all the other characters are valid */
    for (var i = 1; i < sData.length; i++) {
      Char = sData.charAt(i);
      if (sValidChars.indexOf(Char) == -1) {
        return null;
      }

      /* Only allowed one decimal place... */
      if (Char == ".") {
        if (bDecimal) {
          return null;
        }
        bDecimal = true;
      }
    }

    return 'numeric';
  },

  /*
  * Function: -
  * Purpose:  Check to see if a string is actually a formatted date
  * Returns:  string:'date' or null
  * Inputs:   string:sText - string to check
  */
  function (sData) {
    var iParse = Date.parse(sData);
    if ((iParse !== null && !isNaN(iParse)) || (typeof sData === 'string' && sData.length === 0)) {
      return 'date';
    }
    return null;
  },

  /*
  * Function: -
  * Purpose:  Check to see if a string should be treated as an HTML string
  * Returns:  string:'html' or null
  * Inputs:   string:sText - string to check
  */
  function (sData) {
    if (typeof sData === 'string' && sData.indexOf('<') != -1 && sData.indexOf('>') != -1) {
      return 'html';
    }
    return null;
  } ]);


  // jQuery aliases
  $.fn.DataTable = DataTable;
  $.fn.dataTable = DataTable;
  $.fn.dataTableSettings = DataTable.settings;
  $.fn.dataTableExt = DataTable.ext;


  // Information about events fired by DataTables - for documentation.
  /**
  * Draw event, fired whenever the table is redrawn on the page, at the same point as
  * fnDrawCallback. This may be useful for binding events or performing calculations when
  * the table is altered at all.
  *  @name DataTable#draw
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  */

  /**
  * Filter event, fired when the filtering applied to the table (using the build in global
  * global filter, or column filters) is altered.
  *  @name DataTable#filter
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  */

  /**
  * Page change event, fired when the paging of the table is altered.
  *  @name DataTable#page
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  */

  /**
  * Sort event, fired when the sorting applied to the table is altered.
  *  @name DataTable#sort
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  */

  /**
  * DataTables initialisation complete event, fired when the table is fully drawn,
  * including Ajax data loaded, if Ajax data is required.
  *  @name DataTable#init
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} oSettings DataTables settings object
  *  @param {object} json The JSON object request from the server - only
  *    present if client-side Ajax sourced data is used</li></ol>
  */

  /**
  * State save event, fired when the table has changed state a new state save is required.
  * This method allows modification of the state saving object prior to actually doing the
  * save, including addition or other state properties (for plug-ins) or modification
  * of a DataTables core property.
  *  @name DataTable#stateSaveParams
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} oSettings DataTables settings object
  *  @param {object} json The state information to be saved
  */

  /**
  * State load event, fired when the table is loading state from the stored data, but
  * prior to the settings object being modified by the saved state - allowing modification
  * of the saved state is required or loading of state for a plug-in.
  *  @name DataTable#stateLoadParams
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} oSettings DataTables settings object
  *  @param {object} json The saved state information
  */

  /**
  * State loaded event, fired when state has been loaded from stored data and the settings
  * object has been modified by the loaded data.
  *  @name DataTable#stateLoaded
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} oSettings DataTables settings object
  *  @param {object} json The saved state information
  */

  /**
  * Processing event, fired when DataTables is doing some kind of processing (be it,
  * sort, filter or anything else). Can be used to indicate to the end user that
  * there is something happening, or that something has finished.
  *  @name DataTable#processing
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} oSettings DataTables settings object
  *  @param {boolean} bShow Flag for if DataTables is doing processing or not
  */

  /**
  * Ajax (XHR) event, fired whenever an Ajax request is completed from a request to 
  * made to the server for new data (note that this trigger is called in fnServerData,
  * if you override fnServerData and which to use this event, you need to trigger it in
  * you success function).
  *  @name DataTable#xhr
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  *  @param {object} json JSON returned from the server
  */

  /**
  * Destroy event, fired when the DataTable is destroyed by calling fnDestroy or passing
  * the bDestroy:true parameter in the initialisation object. This can be used to remove
  * bound events, added DOM nodes, etc.
  *  @name DataTable#destroy
  *  @event
  *  @param {event} e jQuery event object
  *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
  */
} (jQuery, window, document, undefined));

/*
### jQuery XML to JSON Plugin v1.1 - 2008-07-01 ###
* http://www.fyneworks.com/ - diego@fyneworks.com
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
###
Website: http://www.fyneworks.com/jquery/xml-to-json/
*/
/*
# INSPIRED BY: http://www.terracoder.com/
AND: http://www.thomasfrank.se/xml_to_json.html
AND: http://www.kawa.net/works/js/xml/objtree-e.html
*/
/*
This simple script converts XML (document of code) into a JSON object. It is the combination of 2
'xml to json' great parsers (see below) which allows for both 'simple' and 'extended' parsing modes.
*/
// Avoid collisions
; if (window.jQuery) (function ($) {

  // Add function to jQuery namespace
  $.extend({

    // converts xml documents and xml text to json object
    xml2json: function (xml, extended) {
      if (!xml) return {}; // quick fail

      //### PARSER LIBRARY
      // Core function
      function parseXML(node, simple) {
        if (!node) return null;
        var txt = '', obj = null, att = null;
        var nt = node.nodeType, nn = jsVar(node.localName || node.nodeName);
        var nv = node.text || node.nodeValue || '';
        /*DBG*/ //if(window.console) console.log(['x2j',nn,nt,nv.length+' bytes']);
        if (node.childNodes) {
          if (node.childNodes.length > 0) {
            /*DBG*/ //if(window.console) console.log(['x2j',nn,'CHILDREN',node.childNodes]);
            $.each(node.childNodes, function (n, cn) {
              var cnt = cn.nodeType, cnn = jsVar(cn.localName || cn.nodeName);
              var cnv = cn.text || cn.nodeValue || '';
              /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>a',cnn,cnt,cnv]);
              if (cnt == 8) {
                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>b',cnn,'COMMENT (ignore)']);
                return; // ignore comment node
              }
              else if (cnt == 3 || cnt == 4 || !cnn) {
                // ignore white-space in between tags
                if (cnv.match(/^\s+$/)) {
                  /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>c',cnn,'WHITE-SPACE (ignore)']);
                  return;
                };
                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>d',cnn,'TEXT']);
                txt += cnv.replace(/^\s+/, '').replace(/\s+$/, '');
                // make sure we ditch trailing spaces from markup
              }
              else {
                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>e',cnn,'OBJECT']);
                obj = obj || {};
                if (obj[cnn]) {
                  /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>f',cnn,'ARRAY']);

                  // http://forum.jquery.com/topic/jquery-jquery-xml2json-problems-when-siblings-of-the-same-tagname-only-have-a-textnode-as-a-child
                  if (!obj[cnn].length) obj[cnn] = myArr(obj[cnn]);
                  obj[cnn] = myArr(obj[cnn]);

                  obj[cnn][obj[cnn].length] = parseXML(cn, true/* simple */);
                  obj[cnn].length = obj[cnn].length;
                }
                else {
                  /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>g',cnn,'dig deeper...']);
                  obj[cnn] = parseXML(cn);
                };
              };
            });
          }; //node.childNodes.length>0
        }; //node.childNodes
        if (node.attributes) {
          if (node.attributes.length > 0) {
            /*DBG*/ //if(window.console) console.log(['x2j',nn,'ATTRIBUTES',node.attributes])
            att = {}; obj = obj || {};
            $.each(node.attributes, function (a, at) {
              var atn = jsVar(at.name), atv = at.value;
              att[atn] = atv;
              if (obj[atn]) {
                /*DBG*/ //if(window.console) console.log(['x2j',nn,'attr>',atn,'ARRAY']);

                // http://forum.jquery.com/topic/jquery-jquery-xml2json-problems-when-siblings-of-the-same-tagname-only-have-a-textnode-as-a-child
                //if(!obj[atn].length) obj[atn] = myArr(obj[atn]);//[ obj[ atn ] ];
                obj[cnn] = myArr(obj[cnn]);

                obj[atn][obj[atn].length] = atv;
                obj[atn].length = obj[atn].length;
              }
              else {
                /*DBG*/ //if(window.console) console.log(['x2j',nn,'attr>',atn,'TEXT']);
                obj[atn] = atv;
              };
            });
            //obj['attributes'] = att;
          }; //node.attributes.length>0
        }; //node.attributes
        if (obj) {
          obj = $.extend((txt != '' ? new String(txt) : {}), /* {text:txt},*/obj || {}/*, att || {}*/);
          txt = (obj.text) ? (typeof (obj.text) == 'object' ? obj.text : [obj.text || '']).concat([txt]) : txt;
          if (txt) obj.text = txt;
          txt = '';
        };
        var out = obj || txt;
        //console.log([extended, simple, out]);
        if (extended) {
          if (txt) out = {}; //new String(out);
          txt = out.text || txt || '';
          if (txt) out.text = txt;
          if (!simple) out = myArr(out);
        };
        return out;
      }; // parseXML
      // Core Function End
      // Utility functions
      var jsVar = function (s) { return String(s || '').replace(/-/g, "_"); };

      // NEW isNum function: 01/09/2010
      // Thanks to Emile Grau, GigaTecnologies S.L., www.gigatransfer.com, www.mygigamail.com
      function isNum(s) {
        // based on utility function isNum from xml2json plugin (http://www.fyneworks.com/ - diego@fyneworks.com)
        // few bugs corrected from original function :
        // - syntax error : regexp.test(string) instead of string.test(reg)
        // - regexp modified to accept  comma as decimal mark (latin syntax : 25,24 )
        // - regexp modified to reject if no number before decimal mark  : ".7" is not accepted
        // - string is "trimmed", allowing to accept space at the beginning and end of string
        var regexp = /^((-)?([0-9]+)(([\.\,]{0,1})([0-9]+))?$)/
        return (typeof s == "number") || regexp.test(String((s && typeof s == "string") ? jQuery.trim(s) : ''));
      };
      // OLD isNum function: (for reference only)
      //var isNum = function(s){ return (typeof s == "number") || String((s && typeof s == "string") ? s : '').test(/^((-)?([0-9]*)((\.{0,1})([0-9]+))?$)/); };

      var myArr = function (o) {

        // http://forum.jquery.com/topic/jquery-jquery-xml2json-problems-when-siblings-of-the-same-tagname-only-have-a-textnode-as-a-child
        //if(!o.length) o = [ o ]; o.length=o.length;
        if (!$.isArray(o)) o = [o]; o.length = o.length;

        // here is where you can attach additional functionality, such as searching and sorting...
        return o;
      };
      // Utility functions End
      //### PARSER LIBRARY END

      // Convert plain text to xml
      if (typeof xml == 'string') xml = $.text2xml(xml);

      // Quick fail if not xml (or if this is a node)
      if (!xml.nodeType) return;
      if (xml.nodeType == 3 || xml.nodeType == 4) return xml.nodeValue;

      // Find xml root node
      var root = (xml.nodeType == 9) ? xml.documentElement : xml;

      // Convert xml to json
      var out = parseXML(root, true /* simple */);

      // Clean-up memory
      xml = null; root = null;

      // Send output
      return out;
    },

    // Convert text to XML DOM
    text2xml: function (str) {
      // NOTE: I'd like to use jQuery for this, but jQuery makes all tags uppercase
      //return $(xml)[0];
      var out;
      try {
        var xml = ($.browser.msie) ? new ActiveXObject("Microsoft.XMLDOM") : new DOMParser();
        xml.async = false;
      } catch (e) { throw new Error("XML Parser could not be instantiated") };
      try {
        if ($.browser.msie) out = (xml.loadXML(str)) ? xml : false;
        else out = xml.parseFromString(str, "text/xml");
      } catch (e) { throw new Error("Error parsing XML string") };
      return out;
    }

  }); // extend $

})(jQuery);

/* Use this for plugins */
/**
*
*  MD5 (Message-Digest Algorithm)
*  http://www.webtoolkit.info/
*
**/

var MD5 = function (string) {

  function RotateLeft(lValue, iShiftBits) {
    return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits));
  }

  function AddUnsigned(lX, lY) {
    var lX4, lY4, lX8, lY8, lResult;
    lX8 = (lX & 0x80000000);
    lY8 = (lY & 0x80000000);
    lX4 = (lX & 0x40000000);
    lY4 = (lY & 0x40000000);
    lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);
    if (lX4 & lY4) {
      return (lResult ^ 0x80000000 ^ lX8 ^ lY8);
    }
    if (lX4 | lY4) {
      if (lResult & 0x40000000) {
        return (lResult ^ 0xC0000000 ^ lX8 ^ lY8);
      } else {
        return (lResult ^ 0x40000000 ^ lX8 ^ lY8);
      }
    } else {
      return (lResult ^ lX8 ^ lY8);
    }
  }

  function F(x, y, z) { return (x & y) | ((~x) & z); }
  function G(x, y, z) { return (x & z) | (y & (~z)); }
  function H(x, y, z) { return (x ^ y ^ z); }
  function I(x, y, z) { return (y ^ (x | (~z))); }

  function FF(a, b, c, d, x, s, ac) {
    a = AddUnsigned(a, AddUnsigned(AddUnsigned(F(b, c, d), x), ac));
    return AddUnsigned(RotateLeft(a, s), b);
  };

  function GG(a, b, c, d, x, s, ac) {
    a = AddUnsigned(a, AddUnsigned(AddUnsigned(G(b, c, d), x), ac));
    return AddUnsigned(RotateLeft(a, s), b);
  };

  function HH(a, b, c, d, x, s, ac) {
    a = AddUnsigned(a, AddUnsigned(AddUnsigned(H(b, c, d), x), ac));
    return AddUnsigned(RotateLeft(a, s), b);
  };

  function II(a, b, c, d, x, s, ac) {
    a = AddUnsigned(a, AddUnsigned(AddUnsigned(I(b, c, d), x), ac));
    return AddUnsigned(RotateLeft(a, s), b);
  };

  function ConvertToWordArray(string) {
    var lWordCount;
    var lMessageLength = string.length;
    var lNumberOfWords_temp1 = lMessageLength + 8;
    var lNumberOfWords_temp2 = (lNumberOfWords_temp1 - (lNumberOfWords_temp1 % 64)) / 64;
    var lNumberOfWords = (lNumberOfWords_temp2 + 1) * 16;
    var lWordArray = Array(lNumberOfWords - 1);
    var lBytePosition = 0;
    var lByteCount = 0;
    while (lByteCount < lMessageLength) {
      lWordCount = (lByteCount - (lByteCount % 4)) / 4;
      lBytePosition = (lByteCount % 4) * 8;
      lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition));
      lByteCount++;
    }
    lWordCount = (lByteCount - (lByteCount % 4)) / 4;
    lBytePosition = (lByteCount % 4) * 8;
    lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition);
    lWordArray[lNumberOfWords - 2] = lMessageLength << 3;
    lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29;
    return lWordArray;
  };

  function WordToHex(lValue) {
    var WordToHexValue = "", WordToHexValue_temp = "", lByte, lCount;
    for (lCount = 0; lCount <= 3; lCount++) {
      lByte = (lValue >>> (lCount * 8)) & 255;
      WordToHexValue_temp = "0" + lByte.toString(16);
      WordToHexValue = WordToHexValue + WordToHexValue_temp.substr(WordToHexValue_temp.length - 2, 2);
    }
    return WordToHexValue;
  };

  function Utf8Encode(string) {
    string = string.replace(/\r\n/g, "\n");
    var utftext = "";

    for (var n = 0; n < string.length; n++) {

      var c = string.charCodeAt(n);

      if (c < 128) {
        utftext += String.fromCharCode(c);
      }
      else if ((c > 127) && (c < 2048)) {
        utftext += String.fromCharCode((c >> 6) | 192);
        utftext += String.fromCharCode((c & 63) | 128);
      }
      else {
        utftext += String.fromCharCode((c >> 12) | 224);
        utftext += String.fromCharCode(((c >> 6) & 63) | 128);
        utftext += String.fromCharCode((c & 63) | 128);
      }

    }

    return utftext;
  };

  var x = Array();
  var k, AA, BB, CC, DD, a, b, c, d;
  var S11 = 7, S12 = 12, S13 = 17, S14 = 22;
  var S21 = 5, S22 = 9, S23 = 14, S24 = 20;
  var S31 = 4, S32 = 11, S33 = 16, S34 = 23;
  var S41 = 6, S42 = 10, S43 = 15, S44 = 21;

  string = Utf8Encode(string);

  x = ConvertToWordArray(string);

  a = 0x67452301; b = 0xEFCDAB89; c = 0x98BADCFE; d = 0x10325476;

  for (k = 0; k < x.length; k += 16) {
    AA = a; BB = b; CC = c; DD = d;
    a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
    d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
    c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
    b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
    a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
    d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
    c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
    b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
    a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
    d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
    c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
    b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
    a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
    d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
    c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
    b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
    a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
    d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
    c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
    b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
    a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
    d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
    c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
    b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
    a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
    d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
    c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
    b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
    a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
    d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
    c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
    b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
    a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
    d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
    c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
    b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
    a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
    d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
    c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
    b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
    a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
    d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
    c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
    b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
    a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
    d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
    c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
    b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
    a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
    d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
    c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
    b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
    a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
    d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
    c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
    b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
    a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
    d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
    c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
    b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
    a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
    d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
    c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
    b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
    a = AddUnsigned(a, AA);
    b = AddUnsigned(b, BB);
    c = AddUnsigned(c, CC);
    d = AddUnsigned(d, DD);
  }

  var temp = WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d);

  return temp.toLowerCase();
};

// Underscore.js 1.2.3
// (c) 2009-2011 Jeremy Ashkenas, DocumentCloud Inc.
// Underscore is freely distributable under the MIT license.
// Portions of Underscore are inspired or borrowed from Prototype,
// Oliver Steele's Functional, and John Resig's Micro-Templating.
// For all details and documentation:
// http://documentcloud.github.com/underscore
(function () {
  function r(a, c, d) {
    if (a === c) return a !== 0 || 1 / a == 1 / c;
    if (a == null || c == null) return a === c;
    if (a._chain) a = a._wrapped;
    if (c._chain) c = c._wrapped;
    if (a.isEqual && b.isFunction(a.isEqual)) return a.isEqual(c);
    if (c.isEqual && b.isFunction(c.isEqual)) return c.isEqual(a);
    var e = l.call(a);
    if (e != l.call(c)) return false;
    switch (e) {
      case "[object String]":
        return a == String(c);
      case "[object Number]":
        return a != +a ? c != +c : a == 0 ? 1 / a == 1 / c : a == +c;
      case "[object Date]":
      case "[object Boolean]":
        return +a == +c;
      case "[object RegExp]":
        return a.source == c.source && a.global == c.global && a.multiline == c.multiline && a.ignoreCase == c.ignoreCase
    }
    if (typeof a != "object" || typeof c != "object") return false;
    for (var f = d.length; f--; ) if (d[f] == a) return true;
    d.push(a);
    var f = 0,
      g = true;
    if (e == "[object Array]") {
      if (f = a.length, g = f == c.length) for (; f--; ) if (!(g = f in a == f in c && r(a[f], c[f], d))) break
    } else {
      if ("constructor" in a != "constructor" in c || a.constructor != c.constructor) return false;
      for (var h in a) if (m.call(a, h) && (f++, !(g = m.call(c, h) && r(a[h], c[h], d)))) break;
      if (g) {
        for (h in c) if (m.call(c, h) && !f--) break;
        g = !f
      }
    }
    d.pop();
    return g
  }
  var s = this,
    F = s._,
    o = {},
    k = Array.prototype,
    p = Object.prototype,
    i = k.slice,
    G = k.concat,
    H = k.unshift,
    l = p.toString,
    m = p.hasOwnProperty,
    v = k.forEach,
    w = k.map,
    x = k.reduce,
    y = k.reduceRight,
    z = k.filter,
    A = k.every,
    B = k.some,
    q = k.indexOf,
    C = k.lastIndexOf,
    p = Array.isArray,
    I = Object.keys,
    t = Function.prototype.bind,
    b = function (a) {
      return new n(a)
    };
  if (typeof exports !== "undefined") {
    if (typeof module !== "undefined" && module.exports) exports = module.exports = b;
    exports._ = b
  } else typeof define === "function" && define.amd ? define("underscore", function () {
    return b
  }) : s._ = b;
  b.VERSION = "1.2.3";
  var j = b.each = b.forEach = function (a, c, b) {
    if (a != null) if (v && a.forEach === v) a.forEach(c, b);
    else if (a.length === +a.length) for (var e = 0, f = a.length; e < f; e++) {
      if (e in a && c.call(b, a[e], e, a) === o) break
    } else
      for (e in a) if (m.call(a, e) && c.call(b, a[e], e, a) === o) break
  };
  b.map = function (a, c, b) {
    var e = [];
    if (a == null) return e;
    if (w && a.map === w) return a.map(c, b);
    j(a, function (a, g, h) {
      e[e.length] = c.call(b, a, g, h)
    });
    return e
  };
  b.reduce = b.foldl = b.inject = function (a, c, d, e) {
    var f = arguments.length > 2;
    a == null && (a = []);
    if (x && a.reduce === x) return e && (c = b.bind(c, e)), f ? a.reduce(c, d) : a.reduce(c);
    j(a, function (a, b, i) {
      f ? d = c.call(e, d, a, b, i) : (d = a, f = true)
    });
    if (!f) throw new TypeError("Reduce of empty array with no initial value");
    return d
  };
  b.reduceRight = b.foldr = function (a, c, d, e) {
    var f = arguments.length > 2;
    a == null && (a = []);
    if (y && a.reduceRight === y) return e && (c = b.bind(c, e)), f ? a.reduceRight(c, d) : a.reduceRight(c);
    var g = b.toArray(a).reverse();
    e && !f && (c = b.bind(c, e));
    return f ? b.reduce(g, c, d, e) : b.reduce(g, c)
  };
  b.find = b.detect = function (a, c, b) {
    var e;
    D(a, function (a, g, h) {
      if (c.call(b, a, g, h)) return e = a, true
    });
    return e
  };
  b.filter = b.select = function (a, c, b) {
    var e = [];
    if (a == null) return e;
    if (z && a.filter === z) return a.filter(c, b);
    j(a, function (a, g, h) {
      c.call(b, a, g, h) && (e[e.length] = a)
    });
    return e
  };
  b.reject = function (a, c, b) {
    var e = [];
    if (a == null) return e;
    j(a, function (a, g, h) {
      c.call(b, a, g, h) || (e[e.length] = a)
    });
    return e
  };
  b.every = b.all = function (a, c, b) {
    var e = true;
    if (a == null) return e;
    if (A && a.every === A) return a.every(c, b);
    j(a, function (a, g, h) {
      if (!(e = e && c.call(b, a, g, h))) return o
    });
    return e
  };
  var D = b.some = b.any = function (a, c, d) {
    c || (c = b.identity);
    var e = false;
    if (a == null) return e;
    if (B && a.some === B) return a.some(c, d);
    j(a, function (a, b, h) {
      if (e || (e = c.call(d, a, b, h))) return o
    });
    return !!e
  };
  b.include = b.contains = function (a, c) {
    var b = false;
    if (a == null) return b;
    return q && a.indexOf === q ? a.indexOf(c) != -1 : b = D(a, function (a) {
      return a === c
    })
  };
  b.invoke = function (a, c) {
    var d = i.call(arguments, 2);
    return b.map(a, function (a) {
      return (c.call ? c || a : a[c]).apply(a, d)
    })
  };
  b.pluck = function (a, c) {
    return b.map(a, function (a) {
      return a[c]
    })
  };
  b.max = function (a, c, d) {
    if (!c && b.isArray(a)) return Math.max.apply(Math, a);
    if (!c && b.isEmpty(a)) return -Infinity;
    var e = {
      computed: -Infinity
    };
    j(a, function (a, b, h) {
      b = c ? c.call(d, a, b, h) : a;
      b >= e.computed && (e = {
        value: a,
        computed: b
      })
    });
    return e.value
  };
  b.min = function (a, c, d) {
    if (!c && b.isArray(a)) return Math.min.apply(Math, a);
    if (!c && b.isEmpty(a)) return Infinity;
    var e = {
      computed: Infinity
    };
    j(a, function (a, b, h) {
      b = c ? c.call(d, a, b, h) : a;
      b < e.computed && (e = {
        value: a,
        computed: b
      })
    });
    return e.value
  };
  b.shuffle = function (a) {
    var c = [],
      b;
    j(a, function (a, f) {
      f == 0 ? c[0] = a : (b = Math.floor(Math.random() * (f + 1)), c[f] = c[b], c[b] = a)
    });
    return c
  };
  b.sortBy = function (a, c, d) {
    return b.pluck(b.map(a, function (a, b, g) {
      return {
        value: a,
        criteria: c.call(d, a, b, g)
      }
    }).sort(function (a, c) {
      var b = a.criteria,
        d = c.criteria;
      return b < d ? -1 : b > d ? 1 : 0
    }), "value")
  };
  b.groupBy = function (a, c) {
    var d = {},
      e = b.isFunction(c) ? c : function (a) {
        return a[c]
      };
    j(a, function (a, b) {
      var c = e(a, b);
      (d[c] || (d[c] = [])).push(a)
    });
    return d
  };
  b.sortedIndex =

function (a, c, d) {
  d || (d = b.identity);
  for (var e = 0, f = a.length; e < f; ) {
    var g = e + f >> 1;
    d(a[g]) < d(c) ? e = g + 1 : f = g
  }
  return e
};
  b.toArray = function (a) {
    return !a ? [] : a.toArray ? a.toArray() : b.isArray(a) ? i.call(a) : b.isArguments(a) ? i.call(a) : b.values(a)
  };
  b.size = function (a) {
    return b.toArray(a).length
  };
  b.first = b.head = function (a, b, d) {
    return b != null && !d ? i.call(a, 0, b) : a[0]
  };
  b.initial = function (a, b, d) {
    return i.call(a, 0, a.length - (b == null || d ? 1 : b))
  };
  b.last = function (a, b, d) {
    return b != null && !d ? i.call(a, Math.max(a.length - b, 0)) : a[a.length - 1]
  };
  b.rest = b.tail = function (a, b, d) {
    return i.call(a, b == null || d ? 1 : b)
  };
  b.compact = function (a) {
    return b.filter(a, function (a) {
      return !!a
    })
  };
  b.flatten = function (a, c) {
    return b.reduce(a, function (a, e) {
      if (b.isArray(e)) return a.concat(c ? e : b.flatten(e));
      a[a.length] = e;
      return a
    }, [])
  };
  b.without = function (a) {
    return b.difference(a, i.call(arguments, 1))
  };
  b.uniq = b.unique = function (a, c, d) {
    var d = d ? b.map(a, d) : a,
      e = [];
    b.reduce(d, function (d, g, h) {
      if (0 == h || (c === true ? b.last(d) != g : !b.include(d, g))) d[d.length] = g, e[e.length] = a[h];
      return d
    }, []);
    return e
  };
  b.union = function () {
    return b.uniq(b.flatten(arguments, true))
  };
  b.intersection = b.intersect = function (a) {
    var c = i.call(arguments, 1);
    return b.filter(b.uniq(a), function (a) {
      return b.every(c, function (c) {
        return b.indexOf(c, a) >= 0
      })
    })
  };
  b.difference = function (a) {
    var c = b.flatten(i.call(arguments, 1));
    return b.filter(a, function (a) {
      return !b.include(c, a)
    })
  };
  b.zip = function () {
    for (var a = i.call(arguments), c = b.max(b.pluck(a, "length")), d = Array(c), e = 0; e < c; e++) d[e] = b.pluck(a, "" + e);
    return d
  };
  b.indexOf = function (a, c, d) {
    if (a == null) return -1;
    var e;
    if (d) return d = b.sortedIndex(a, c), a[d] === c ? d : -1;
    if (q && a.indexOf === q) return a.indexOf(c);
    for (d = 0, e = a.length; d < e; d++) if (d in a && a[d] === c) return d;
    return -1
  };
  b.lastIndexOf = function (a, b) {
    if (a == null) return -1;
    if (C && a.lastIndexOf === C) return a.lastIndexOf(b);
    for (var d = a.length; d--; ) if (d in a && a[d] === b) return d;
    return -1
  };
  b.range = function (a, b, d) {
    arguments.length <= 1 && (b = a || 0, a = 0);
    for (var d = arguments[2] || 1, e = Math.max(Math.ceil((b - a) / d), 0), f = 0, g = Array(e); f < e; ) g[f++] = a, a += d;
    return g
  };
  var E = function () { };
  b.bind = function (a, c) {
    var d, e;
    if (a.bind === t && t) return t.apply(a, i.call(arguments, 1));
    if (!b.isFunction(a)) throw new TypeError;
    e = i.call(arguments, 2);
    return d = function () {
      if (!(this instanceof d)) return a.apply(c, e.concat(i.call(arguments)));
      E.prototype = a.prototype;
      var b = new E,
        g = a.apply(b, e.concat(i.call(arguments)));
      return Object(g) === g ? g : b
    }
  };
  b.bindAll = function (a) {
    var c = i.call(arguments, 1);
    c.length == 0 && (c = b.functions(a));
    j(c, function (c) {
      a[c] = b.bind(a[c], a)
    });
    return a
  };
  b.memoize = function (a, c) {
    var d = {};
    c || (c = b.identity);
    return function () {
      var b = c.apply(this, arguments);
      return m.call(d, b) ? d[b] : d[b] = a.apply(this, arguments)
    }
  };
  b.delay = function (a, b) {
    var d = i.call(arguments, 2);
    return setTimeout(function () {
      return a.apply(a, d)
    }, b)
  };
  b.defer = function (a) {
    return b.delay.apply(b, [a, 1].concat(i.call(arguments, 1)))
  };
  b.throttle = function (a, c) {
    var d, e, f, g, h, i = b.debounce(function () {
      h = g = false
    }, c);
    return function () {
      d = this;
      e = arguments;
      var b;
      f || (f = setTimeout(function () {
        f = null;
        h && a.apply(d, e);
        i()
      }, c));
      g ? h = true : a.apply(d, e);
      i();
      g = true
    }
  };
  b.debounce = function (a, b) {
    var d;
    return function () {
      var e = this,
        f = arguments;
      clearTimeout(d);
      d = setTimeout(function () {
        d = null;
        a.apply(e, f)
      }, b)
    }
  };
  b.once = function (a) {
    var b = false,
      d;
    return function () {
      if (b) return d;
      b = true;
      return d = a.apply(this, arguments)
    }
  };
  b.wrap = function (a, b) {
    return function () {
      var d = G.apply([a], arguments);
      return b.apply(this, d)
    }
  };
  b.compose = function () {
    var a = arguments;
    return function () {
      for (var b = arguments, d = a.length - 1; d >= 0; d--) b = [a[d].apply(this, b)];
      return b[0]
    }
  };
  b.after =

function (a, b) {
  return a <= 0 ? b() : function () {
    if (--a < 1) return b.apply(this, arguments)
  }
};
  b.keys = I ||
function (a) {
  if (a !== Object(a)) throw new TypeError("Invalid object");
  var b = [],
      d;
  for (d in a) m.call(a, d) && (b[b.length] = d);
  return b
};
  b.values = function (a) {
    return b.map(a, b.identity)
  };
  b.functions = b.methods = function (a) {
    var c = [],
      d;
    for (d in a) b.isFunction(a[d]) && c.push(d);
    return c.sort()
  };
  b.extend = function (a) {
    j(i.call(arguments, 1), function (b) {
      for (var d in b) b[d] !== void 0 && (a[d] = b[d])
    });
    return a
  };
  b.defaults = function (a) {
    j(i.call(arguments, 1), function (b) {
      for (var d in b) a[d] == null && (a[d] = b[d])
    });
    return a
  };
  b.clone = function (a) {
    return !b.isObject(a) ? a : b.isArray(a) ? a.slice() : b.extend({}, a)
  };
  b.tap = function (a, b) {
    b(a);
    return a
  };
  b.isEqual = function (a, b) {
    return r(a, b, [])
  };
  b.isEmpty = function (a) {
    if (b.isArray(a) || b.isString(a)) return a.length === 0;
    for (var c in a) if (m.call(a, c)) return false;
    return true
  };
  b.isElement = function (a) {
    return !!(a && a.nodeType == 1)
  };
  b.isArray = p ||
function (a) {
  return l.call(a) == "[object Array]"
};
  b.isObject = function (a) {
    return a === Object(a)
  };
  b.isArguments = function (a) {
    return l.call(a) == "[object Arguments]"
  };
  if (!b.isArguments(arguments)) b.isArguments = function (a) {
    return !(!a || !m.call(a, "callee"))
  };
  b.isFunction = function (a) {
    return l.call(a) == "[object Function]"
  };
  b.isString = function (a) {
    return l.call(a) == "[object String]"
  };
  b.isNumber = function (a) {
    return l.call(a) == "[object Number]"
  };
  b.isNaN = function (a) {
    return a !== a
  };
  b.isBoolean = function (a) {
    return a === true || a === false || l.call(a) == "[object Boolean]"
  };
  b.isDate = function (a) {
    return l.call(a) == "[object Date]"
  };
  b.isRegExp = function (a) {
    return l.call(a) == "[object RegExp]"
  };
  b.isNull = function (a) {
    return a === null
  };
  b.isUndefined = function (a) {
    return a === void 0
  };
  b.noConflict = function () {
    s._ = F;
    return this
  };
  b.identity = function (a) {
    return a
  };
  b.times = function (a, b, d) {
    for (var e = 0; e < a; e++) b.call(d, e)
  };
  b.escape = function (a) {
    return ("" + a).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27;").replace(/\//g, "&#x2F;")
  };
  b.mixin = function (a) {
    j(b.functions(a), function (c) {
      J(c, b[c] = a[c])
    })
  };
  var K = 0;
  b.uniqueId = function (a) {
    var b = K++;
    return a ? a + b : b
  };
  b.templateSettings = {
    evaluate: /<%([\s\S]+?)%>/g,
    interpolate: /<%=([\s\S]+?)%>/g,
    escape: /<%-([\s\S]+?)%>/g
  };
  b.template = function (a, c) {
    var d = b.templateSettings,
      d = "var __p=[],print=function(){__p.push.apply(__p,arguments);};with(obj||{}){__p.push('" + a.replace(/\\/g, "\\\\").replace(/'/g, "\\'").replace(d.escape, function (a, b) {
        return "',_.escape(" + b.replace(/\\'/g, "'") + "),'"
      }).replace(d.interpolate, function (a, b) {
        return "'," + b.replace(/\\'/g, "'") + ",'"
      }).replace(d.evaluate || null, function (a, b) {
        return "');" + b.replace(/\\'/g, "'").replace(/[\r\n\t]/g, " ") + ";__p.push('"
      }).replace(/\r/g, "\\r").replace(/\n/g, "\\n").replace(/\t/g, "\\t") + "');}return __p.join('');",
      e = new Function("obj", "_", d);
    return c ? e(c, b) : function (a) {
      return e.call(this, a, b)
    }
  };
  var n = function (a) {
    this._wrapped = a
  };
  b.prototype = n.prototype;
  var u = function (a, c) {
    return c ? b(a).chain() : a
  },
    J = function (a, c) {
      n.prototype[a] = function () {
        var a = i.call(arguments);
        H.call(a, this._wrapped);
        return u(c.apply(b, a), this._chain)
      }
    };
  b.mixin(b);
  j("pop,push,reverse,shift,sort,splice,unshift".split(","), function (a) {
    var b = k[a];
    n.prototype[a] = function () {
      b.apply(this._wrapped, arguments);
      return u(this._wrapped, this._chain)
    }
  });
  j(["concat", "join", "slice"], function (a) {
    var b = k[a];
    n.prototype[a] = function () {
      return u(b.apply(this._wrapped, arguments), this._chain)
    }
  });
  n.prototype.chain = function () {
    this._chain = true;
    return this
  };
  n.prototype.value = function () {
    return this._wrapped
  }
}).call(this);

/**
* SyntaxHighlighter
* http://alexgorbatchev.com/SyntaxHighlighter
*
* SyntaxHighlighter is donationware. If you are using it, please donate.
* http://alexgorbatchev.com/SyntaxHighlighter/donate.html
*
* @version
* 3.0.83 (July 02 2010)
* 
* @copyright
* Copyright (C) 2004-2010 Alex Gorbatchev.
*
* @license
* Dual licensed under the MIT and GPL licenses.
*/
eval(function (p, a, c, k, e, d) { e = function (c) { return (c < a ? '' : e(parseInt(c / a))) + ((c = c % a) > 35 ? String.fromCharCode(c + 29) : c.toString(36)) }; if (!''.replace(/^/, String)) { while (c--) { d[e(c)] = k[c] || e(c) } k = [function (e) { return d[e] } ]; e = function () { return '\\w+' }; c = 1 }; while (c--) { if (k[c]) { p = p.replace(new RegExp('\\b' + e(c) + '\\b', 'g'), k[c]) } } return p } ('K M;I(M)1S 2U("2a\'t 4k M 4K 2g 3l 4G 4H");(6(){6 r(f,e){I(!M.1R(f))1S 3m("3s 15 4R");K a=f.1w;f=M(f.1m,t(f)+(e||""));I(a)f.1w={1m:a.1m,19:a.19?a.19.1a(0):N};H f}6 t(f){H(f.1J?"g":"")+(f.4s?"i":"")+(f.4p?"m":"")+(f.4v?"x":"")+(f.3n?"y":"")}6 B(f,e,a,b){K c=u.L,d,h,g;v=R;5K{O(;c--;){g=u[c];I(a&g.3r&&(!g.2p||g.2p.W(b))){g.2q.12=e;I((h=g.2q.X(f))&&h.P===e){d={3k:g.2b.W(b,h,a),1C:h};1N}}}}5v(i){1S i}5q{v=11}H d}6 p(f,e,a){I(3b.Z.1i)H f.1i(e,a);O(a=a||0;a<f.L;a++)I(f[a]===e)H a;H-1}M=6(f,e){K a=[],b=M.1B,c=0,d,h;I(M.1R(f)){I(e!==1d)1S 3m("2a\'t 5r 5I 5F 5B 5C 15 5E 5p");H r(f)}I(v)1S 2U("2a\'t W 3l M 59 5m 5g 5x 5i");e=e||"";O(d={2N:11,19:[],2K:6(g){H e.1i(g)>-1},3d:6(g){e+=g}};c<f.L;)I(h=B(f,c,b,d)){a.U(h.3k);c+=h.1C[0].L||1}Y I(h=n.X.W(z[b],f.1a(c))){a.U(h[0]);c+=h[0].L}Y{h=f.3a(c);I(h==="[")b=M.2I;Y I(h==="]")b=M.1B;a.U(h);c++}a=15(a.1K(""),n.Q.W(e,w,""));a.1w={1m:f,19:d.2N?d.19:N};H a};M.3v="1.5.0";M.2I=1;M.1B=2;K C=/\\$(?:(\\d\\d?|[$&`\'])|{([$\\w]+)})/g,w=/[^5h]+|([\\s\\S])(?=[\\s\\S]*\\1)/g,A=/^(?:[?*+]|{\\d+(?:,\\d*)?})\\??/,v=11,u=[],n={X:15.Z.X,1A:15.Z.1A,1C:1r.Z.1C,Q:1r.Z.Q,1e:1r.Z.1e},x=n.X.W(/()??/,"")[1]===1d,D=6(){K f=/^/g;n.1A.W(f,"");H!f.12}(),y=6(){K f=/x/g;n.Q.W("x",f,"");H!f.12}(),E=15.Z.3n!==1d,z={};z[M.2I]=/^(?:\\\\(?:[0-3][0-7]{0,2}|[4-7][0-7]?|x[\\29-26-f]{2}|u[\\29-26-f]{4}|c[A-3o-z]|[\\s\\S]))/;z[M.1B]=/^(?:\\\\(?:0(?:[0-3][0-7]{0,2}|[4-7][0-7]?)?|[1-9]\\d*|x[\\29-26-f]{2}|u[\\29-26-f]{4}|c[A-3o-z]|[\\s\\S])|\\(\\?[:=!]|[?*+]\\?|{\\d+(?:,\\d*)?}\\??)/;M.1h=6(f,e,a,b){u.U({2q:r(f,"g"+(E?"y":"")),2b:e,3r:a||M.1B,2p:b||N})};M.2n=6(f,e){K a=f+"/"+(e||"");H M.2n[a]||(M.2n[a]=M(f,e))};M.3c=6(f){H r(f,"g")};M.5l=6(f){H f.Q(/[-[\\]{}()*+?.,\\\\^$|#\\s]/g,"\\\\$&")};M.5e=6(f,e,a,b){e=r(e,"g"+(b&&E?"y":""));e.12=a=a||0;f=e.X(f);H b?f&&f.P===a?f:N:f};M.3q=6(){M.1h=6(){1S 2U("2a\'t 55 1h 54 3q")}};M.1R=6(f){H 53.Z.1q.W(f)==="[2m 15]"};M.3p=6(f,e,a,b){O(K c=r(e,"g"),d=-1,h;h=c.X(f);){a.W(b,h,++d,f,c);c.12===h.P&&c.12++}I(e.1J)e.12=0};M.57=6(f,e){H 6 a(b,c){K d=e[c].1I?e[c]:{1I:e[c]},h=r(d.1I,"g"),g=[],i;O(i=0;i<b.L;i++)M.3p(b[i],h,6(k){g.U(d.3j?k[d.3j]||"":k[0])});H c===e.L-1||!g.L?g:a(g,c+1)}([f],0)};15.Z.1p=6(f,e){H J.X(e[0])};15.Z.W=6(f,e){H J.X(e)};15.Z.X=6(f){K e=n.X.1p(J,14),a;I(e){I(!x&&e.L>1&&p(e,"")>-1){a=15(J.1m,n.Q.W(t(J),"g",""));n.Q.W(f.1a(e.P),a,6(){O(K c=1;c<14.L-2;c++)I(14[c]===1d)e[c]=1d})}I(J.1w&&J.1w.19)O(K b=1;b<e.L;b++)I(a=J.1w.19[b-1])e[a]=e[b];!D&&J.1J&&!e[0].L&&J.12>e.P&&J.12--}H e};I(!D)15.Z.1A=6(f){(f=n.X.W(J,f))&&J.1J&&!f[0].L&&J.12>f.P&&J.12--;H!!f};1r.Z.1C=6(f){M.1R(f)||(f=15(f));I(f.1J){K e=n.1C.1p(J,14);f.12=0;H e}H f.X(J)};1r.Z.Q=6(f,e){K a=M.1R(f),b,c;I(a&&1j e.58()==="3f"&&e.1i("${")===-1&&y)H n.Q.1p(J,14);I(a){I(f.1w)b=f.1w.19}Y f+="";I(1j e==="6")c=n.Q.W(J,f,6(){I(b){14[0]=1f 1r(14[0]);O(K d=0;d<b.L;d++)I(b[d])14[0][b[d]]=14[d+1]}I(a&&f.1J)f.12=14[14.L-2]+14[0].L;H e.1p(N,14)});Y{c=J+"";c=n.Q.W(c,f,6(){K d=14;H n.Q.W(e,C,6(h,g,i){I(g)5b(g){24"$":H"$";24"&":H d[0];24"`":H d[d.L-1].1a(0,d[d.L-2]);24"\'":H d[d.L-1].1a(d[d.L-2]+d[0].L);5a:i="";g=+g;I(!g)H h;O(;g>d.L-3;){i=1r.Z.1a.W(g,-1)+i;g=1Q.3i(g/10)}H(g?d[g]||"":"$")+i}Y{g=+i;I(g<=d.L-3)H d[g];g=b?p(b,i):-1;H g>-1?d[g+1]:h}})})}I(a&&f.1J)f.12=0;H c};1r.Z.1e=6(f,e){I(!M.1R(f))H n.1e.1p(J,14);K a=J+"",b=[],c=0,d,h;I(e===1d||+e<0)e=5D;Y{e=1Q.3i(+e);I(!e)H[]}O(f=M.3c(f);d=f.X(a);){I(f.12>c){b.U(a.1a(c,d.P));d.L>1&&d.P<a.L&&3b.Z.U.1p(b,d.1a(1));h=d[0].L;c=f.12;I(b.L>=e)1N}f.12===d.P&&f.12++}I(c===a.L){I(!n.1A.W(f,"")||h)b.U("")}Y b.U(a.1a(c));H b.L>e?b.1a(0,e):b};M.1h(/\\(\\?#[^)]*\\)/,6(f){H n.1A.W(A,f.2S.1a(f.P+f[0].L))?"":"(?:)"});M.1h(/\\((?!\\?)/,6(){J.19.U(N);H"("});M.1h(/\\(\\?<([$\\w]+)>/,6(f){J.19.U(f[1]);J.2N=R;H"("});M.1h(/\\\\k<([\\w$]+)>/,6(f){K e=p(J.19,f[1]);H e>-1?"\\\\"+(e+1)+(3R(f.2S.3a(f.P+f[0].L))?"":"(?:)"):f[0]});M.1h(/\\[\\^?]/,6(f){H f[0]==="[]"?"\\\\b\\\\B":"[\\\\s\\\\S]"});M.1h(/^\\(\\?([5A]+)\\)/,6(f){J.3d(f[1]);H""});M.1h(/(?:\\s+|#.*)+/,6(f){H n.1A.W(A,f.2S.1a(f.P+f[0].L))?"":"(?:)"},M.1B,6(){H J.2K("x")});M.1h(/\\./,6(){H"[\\\\s\\\\S]"},M.1B,6(){H J.2K("s")})})();1j 2e!="1d"&&(2e.M=M);K 1v=6(){6 r(a,b){a.1l.1i(b)!=-1||(a.1l+=" "+b)}6 t(a){H a.1i("3e")==0?a:"3e"+a}6 B(a){H e.1Y.2A[t(a)]}6 p(a,b,c){I(a==N)H N;K d=c!=R?a.3G:[a.2G],h={"#":"1c",".":"1l"}[b.1o(0,1)]||"3h",g,i;g=h!="3h"?b.1o(1):b.5u();I((a[h]||"").1i(g)!=-1)H a;O(a=0;d&&a<d.L&&i==N;a++)i=p(d[a],b,c);H i}6 C(a,b){K c={},d;O(d 2g a)c[d]=a[d];O(d 2g b)c[d]=b[d];H c}6 w(a,b,c,d){6 h(g){g=g||1P.5y;I(!g.1F){g.1F=g.52;g.3N=6(){J.5w=11}}c.W(d||1P,g)}a.3g?a.3g("4U"+b,h):a.4y(b,h,11)}6 A(a,b){K c=e.1Y.2j,d=N;I(c==N){c={};O(K h 2g e.1U){K g=e.1U[h];d=g.4x;I(d!=N){g.1V=h.4w();O(g=0;g<d.L;g++)c[d[g]]=h}}e.1Y.2j=c}d=e.1U[c[a]];d==N&&b!=11&&1P.1X(e.13.1x.1X+(e.13.1x.3E+a));H d}6 v(a,b){O(K c=a.1e("\\n"),d=0;d<c.L;d++)c[d]=b(c[d],d);H c.1K("\\n")}6 u(a,b){I(a==N||a.L==0||a=="\\n")H a;a=a.Q(/</g,"&1y;");a=a.Q(/ {2,}/g,6(c){O(K d="",h=0;h<c.L-1;h++)d+=e.13.1W;H d+" "});I(b!=N)a=v(a,6(c){I(c.L==0)H"";K d="";c=c.Q(/^(&2s;| )+/,6(h){d=h;H""});I(c.L==0)H d;H d+\'<17 1g="\'+b+\'">\'+c+"</17>"});H a}6 n(a,b){a.1e("\\n");O(K c="",d=0;d<50;d++)c+="                    ";H a=v(a,6(h){I(h.1i("\\t")==-1)H h;O(K g=0;(g=h.1i("\\t"))!=-1;)h=h.1o(0,g)+c.1o(0,b-g%b)+h.1o(g+1,h.L);H h})}6 x(a){H a.Q(/^\\s+|\\s+$/g,"")}6 D(a,b){I(a.P<b.P)H-1;Y I(a.P>b.P)H 1;Y I(a.L<b.L)H-1;Y I(a.L>b.L)H 1;H 0}6 y(a,b){6 c(k){H k[0]}O(K d=N,h=[],g=b.2D?b.2D:c;(d=b.1I.X(a))!=N;){K i=g(d,b);I(1j i=="3f")i=[1f e.2L(i,d.P,b.23)];h=h.1O(i)}H h}6 E(a){K b=/(.*)((&1G;|&1y;).*)/;H a.Q(e.3A.3M,6(c){K d="",h=N;I(h=b.X(c)){c=h[1];d=h[2]}H\'<a 2h="\'+c+\'">\'+c+"</a>"+d})}6 z(){O(K a=1E.36("1k"),b=[],c=0;c<a.L;c++)a[c].3s=="20"&&b.U(a[c]);H b}6 f(a){a=a.1F;K b=p(a,".20",R);a=p(a,".3O",R);K c=1E.4i("3t");I(!(!a||!b||p(a,"3t"))){B(b.1c);r(b,"1m");O(K d=a.3G,h=[],g=0;g<d.L;g++)h.U(d[g].4z||d[g].4A);h=h.1K("\\r");c.39(1E.4D(h));a.39(c);c.2C();c.4C();w(c,"4u",6(){c.2G.4E(c);b.1l=b.1l.Q("1m","")})}}I(1j 3F!="1d"&&1j M=="1d")M=3F("M").M;K e={2v:{"1g-27":"","2i-1s":1,"2z-1s-2t":11,1M:N,1t:N,"42-45":R,"43-22":4,1u:R,16:R,"3V-17":R,2l:11,"41-40":R,2k:11,"1z-1k":11},13:{1W:"&2s;",2M:R,46:11,44:11,34:"4n",1x:{21:"4o 1m",2P:"?",1X:"1v\\n\\n",3E:"4r\'t 4t 1D O: ",4g:"4m 4B\'t 51 O 1z-1k 4F: ",37:\'<!4T 1z 4S "-//4V//3H 4W 1.0 4Z//4Y" "1Z://2y.3L.3K/4X/3I/3H/3I-4P.4J"><1z 4I="1Z://2y.3L.3K/4L/5L"><3J><4N 1Z-4M="5G-5M" 6K="2O/1z; 6J=6I-8" /><1t>6L 1v</1t></3J><3B 1L="25-6M:6Q,6P,6O,6N-6F;6y-2f:#6x;2f:#6w;25-22:6v;2O-3D:3C;"><T 1L="2O-3D:3C;3w-32:1.6z;"><T 1L="25-22:6A-6E;">1v</T><T 1L="25-22:.6C;3w-6B:6R;"><T>3v 3.0.76 (72 73 3x)</T><T><a 2h="1Z://3u.2w/1v" 1F="38" 1L="2f:#3y">1Z://3u.2w/1v</a></T><T>70 17 6U 71.</T><T>6T 6X-3x 6Y 6D.</T></T><T>6t 61 60 J 1k, 5Z <a 2h="6u://2y.62.2w/63-66/65?64=5X-5W&5P=5O" 1L="2f:#3y">5R</a> 5V <2R/>5U 5T 5S!</T></T></3B></1z>\'}},1Y:{2j:N,2A:{}},1U:{},3A:{6n:/\\/\\*[\\s\\S]*?\\*\\//2c,6m:/\\/\\/.*$/2c,6l:/#.*$/2c,6k:/"([^\\\\"\\n]|\\\\.)*"/g,6o:/\'([^\\\\\'\\n]|\\\\.)*\'/g,6p:1f M(\'"([^\\\\\\\\"]|\\\\\\\\.)*"\',"3z"),6s:1f M("\'([^\\\\\\\\\']|\\\\\\\\.)*\'","3z"),6q:/(&1y;|<)!--[\\s\\S]*?--(&1G;|>)/2c,3M:/\\w+:\\/\\/[\\w-.\\/?%&=:@;]*/g,6a:{18:/(&1y;|<)\\?=?/g,1b:/\\?(&1G;|>)/g},69:{18:/(&1y;|<)%=?/g,1b:/%(&1G;|>)/g},6d:{18:/(&1y;|<)\\s*1k.*?(&1G;|>)/2T,1b:/(&1y;|<)\\/\\s*1k\\s*(&1G;|>)/2T}},16:{1H:6(a){6 b(i,k){H e.16.2o(i,k,e.13.1x[k])}O(K c=\'<T 1g="16">\',d=e.16.2x,h=d.2X,g=0;g<h.L;g++)c+=(d[h[g]].1H||b)(a,h[g]);c+="</T>";H c},2o:6(a,b,c){H\'<2W><a 2h="#" 1g="6e 6h\'+b+" "+b+\'">\'+c+"</a></2W>"},2b:6(a){K b=a.1F,c=b.1l||"";b=B(p(b,".20",R).1c);K d=6(h){H(h=15(h+"6f(\\\\w+)").X(c))?h[1]:N}("6g");b&&d&&e.16.2x[d].2B(b);a.3N()},2x:{2X:["21","2P"],21:{1H:6(a){I(a.V("2l")!=R)H"";K b=a.V("1t");H e.16.2o(a,"21",b?b:e.13.1x.21)},2B:6(a){a=1E.6j(t(a.1c));a.1l=a.1l.Q("47","")}},2P:{2B:6(){K a="68=0";a+=", 18="+(31.30-33)/2+", 32="+(31.2Z-2Y)/2+", 30=33, 2Z=2Y";a=a.Q(/^,/,"");a=1P.6Z("","38",a);a.2C();K b=a.1E;b.6W(e.13.1x.37);b.6V();a.2C()}}}},35:6(a,b){K c;I(b)c=[b];Y{c=1E.36(e.13.34);O(K d=[],h=0;h<c.L;h++)d.U(c[h]);c=d}c=c;d=[];I(e.13.2M)c=c.1O(z());I(c.L===0)H d;O(h=0;h<c.L;h++){O(K g=c[h],i=a,k=c[h].1l,j=3W 0,l={},m=1f M("^\\\\[(?<2V>(.*?))\\\\]$"),s=1f M("(?<27>[\\\\w-]+)\\\\s*:\\\\s*(?<1T>[\\\\w-%#]+|\\\\[.*?\\\\]|\\".*?\\"|\'.*?\')\\\\s*;?","g");(j=s.X(k))!=N;){K o=j.1T.Q(/^[\'"]|[\'"]$/g,"");I(o!=N&&m.1A(o)){o=m.X(o);o=o.2V.L>0?o.2V.1e(/\\s*,\\s*/):[]}l[j.27]=o}g={1F:g,1n:C(i,l)};g.1n.1D!=N&&d.U(g)}H d},1M:6(a,b){K c=J.35(a,b),d=N,h=e.13;I(c.L!==0)O(K g=0;g<c.L;g++){b=c[g];K i=b.1F,k=b.1n,j=k.1D,l;I(j!=N){I(k["1z-1k"]=="R"||e.2v["1z-1k"]==R){d=1f e.4l(j);j="4O"}Y I(d=A(j))d=1f d;Y 6H;l=i.3X;I(h.2M){l=l;K m=x(l),s=11;I(m.1i("<![6G[")==0){m=m.4h(9);s=R}K o=m.L;I(m.1i("]]\\>")==o-3){m=m.4h(0,o-3);s=R}l=s?m:l}I((i.1t||"")!="")k.1t=i.1t;k.1D=j;d.2Q(k);b=d.2F(l);I((i.1c||"")!="")b.1c=i.1c;i.2G.74(b,i)}}},2E:6(a){w(1P,"4k",6(){e.1M(a)})}};e.2E=e.2E;e.1M=e.1M;e.2L=6(a,b,c){J.1T=a;J.P=b;J.L=a.L;J.23=c;J.1V=N};e.2L.Z.1q=6(){H J.1T};e.4l=6(a){6 b(j,l){O(K m=0;m<j.L;m++)j[m].P+=l}K c=A(a),d,h=1f e.1U.5Y,g=J,i="2F 1H 2Q".1e(" ");I(c!=N){d=1f c;O(K k=0;k<i.L;k++)(6(){K j=i[k];g[j]=6(){H h[j].1p(h,14)}})();d.28==N?1P.1X(e.13.1x.1X+(e.13.1x.4g+a)):h.2J.U({1I:d.28.17,2D:6(j){O(K l=j.17,m=[],s=d.2J,o=j.P+j.18.L,F=d.28,q,G=0;G<s.L;G++){q=y(l,s[G]);b(q,o);m=m.1O(q)}I(F.18!=N&&j.18!=N){q=y(j.18,F.18);b(q,j.P);m=m.1O(q)}I(F.1b!=N&&j.1b!=N){q=y(j.1b,F.1b);b(q,j.P+j[0].5Q(j.1b));m=m.1O(q)}O(j=0;j<m.L;j++)m[j].1V=c.1V;H m}})}};e.4j=6(){};e.4j.Z={V:6(a,b){K c=J.1n[a];c=c==N?b:c;K d={"R":R,"11":11}[c];H d==N?c:d},3Y:6(a){H 1E.4i(a)},4c:6(a,b){K c=[];I(a!=N)O(K d=0;d<a.L;d++)I(1j a[d]=="2m")c=c.1O(y(b,a[d]));H J.4e(c.6b(D))},4e:6(a){O(K b=0;b<a.L;b++)I(a[b]!==N)O(K c=a[b],d=c.P+c.L,h=b+1;h<a.L&&a[b]!==N;h++){K g=a[h];I(g!==N)I(g.P>d)1N;Y I(g.P==c.P&&g.L>c.L)a[b]=N;Y I(g.P>=c.P&&g.P<d)a[h]=N}H a},4d:6(a){K b=[],c=2u(J.V("2i-1s"));v(a,6(d,h){b.U(h+c)});H b},3U:6(a){K b=J.V("1M",[]);I(1j b!="2m"&&b.U==N)b=[b];a:{a=a.1q();K c=3W 0;O(c=c=1Q.6c(c||0,0);c<b.L;c++)I(b[c]==a){b=c;1N a}b=-1}H b!=-1},2r:6(a,b,c){a=["1s","6i"+b,"P"+a,"6r"+(b%2==0?1:2).1q()];J.3U(b)&&a.U("67");b==0&&a.U("1N");H\'<T 1g="\'+a.1K(" ")+\'">\'+c+"</T>"},3Q:6(a,b){K c="",d=a.1e("\\n").L,h=2u(J.V("2i-1s")),g=J.V("2z-1s-2t");I(g==R)g=(h+d-1).1q().L;Y I(3R(g)==R)g=0;O(K i=0;i<d;i++){K k=b?b[i]:h+i,j;I(k==0)j=e.13.1W;Y{j=g;O(K l=k.1q();l.L<j;)l="0"+l;j=l}a=j;c+=J.2r(i,k,a)}H c},49:6(a,b){a=x(a);K c=a.1e("\\n");J.V("2z-1s-2t");K d=2u(J.V("2i-1s"));a="";O(K h=J.V("1D"),g=0;g<c.L;g++){K i=c[g],k=/^(&2s;|\\s)+/.X(i),j=N,l=b?b[g]:d+g;I(k!=N){j=k[0].1q();i=i.1o(j.L);j=j.Q(" ",e.13.1W)}i=x(i);I(i.L==0)i=e.13.1W;a+=J.2r(g,l,(j!=N?\'<17 1g="\'+h+\' 5N">\'+j+"</17>":"")+i)}H a},4f:6(a){H a?"<4a>"+a+"</4a>":""},4b:6(a,b){6 c(l){H(l=l?l.1V||g:g)?l+" ":""}O(K d=0,h="",g=J.V("1D",""),i=0;i<b.L;i++){K k=b[i],j;I(!(k===N||k.L===0)){j=c(k);h+=u(a.1o(d,k.P-d),j+"48")+u(k.1T,j+k.23);d=k.P+k.L+(k.75||0)}}h+=u(a.1o(d),c()+"48");H h},1H:6(a){K b="",c=["20"],d;I(J.V("2k")==R)J.1n.16=J.1n.1u=11;1l="20";J.V("2l")==R&&c.U("47");I((1u=J.V("1u"))==11)c.U("6S");c.U(J.V("1g-27"));c.U(J.V("1D"));a=a.Q(/^[ ]*[\\n]+|[\\n]*[ ]*$/g,"").Q(/\\r/g," ");b=J.V("43-22");I(J.V("42-45")==R)a=n(a,b);Y{O(K h="",g=0;g<b;g++)h+=" ";a=a.Q(/\\t/g,h)}a=a;a:{b=a=a;h=/<2R\\s*\\/?>|&1y;2R\\s*\\/?&1G;/2T;I(e.13.46==R)b=b.Q(h,"\\n");I(e.13.44==R)b=b.Q(h,"");b=b.1e("\\n");h=/^\\s*/;g=4Q;O(K i=0;i<b.L&&g>0;i++){K k=b[i];I(x(k).L!=0){k=h.X(k);I(k==N){a=a;1N a}g=1Q.4q(k[0].L,g)}}I(g>0)O(i=0;i<b.L;i++)b[i]=b[i].1o(g);a=b.1K("\\n")}I(1u)d=J.4d(a);b=J.4c(J.2J,a);b=J.4b(a,b);b=J.49(b,d);I(J.V("41-40"))b=E(b);1j 2H!="1d"&&2H.3S&&2H.3S.1C(/5s/)&&c.U("5t");H b=\'<T 1c="\'+t(J.1c)+\'" 1g="\'+c.1K(" ")+\'">\'+(J.V("16")?e.16.1H(J):"")+\'<3Z 5z="0" 5H="0" 5J="0">\'+J.4f(J.V("1t"))+"<3T><3P>"+(1u?\'<2d 1g="1u">\'+J.3Q(a)+"</2d>":"")+\'<2d 1g="17"><T 1g="3O">\'+b+"</T></2d></3P></3T></3Z></T>"},2F:6(a){I(a===N)a="";J.17=a;K b=J.3Y("T");b.3X=J.1H(a);J.V("16")&&w(p(b,".16"),"5c",e.16.2b);J.V("3V-17")&&w(p(b,".17"),"56",f);H b},2Q:6(a){J.1c=""+1Q.5d(1Q.5n()*5k).1q();e.1Y.2A[t(J.1c)]=J;J.1n=C(e.2v,a||{});I(J.V("2k")==R)J.1n.16=J.1n.1u=11},5j:6(a){a=a.Q(/^\\s+|\\s+$/g,"").Q(/\\s+/g,"|");H"\\\\b(?:"+a+")\\\\b"},5f:6(a){J.28={18:{1I:a.18,23:"1k"},1b:{1I:a.1b,23:"1k"},17:1f M("(?<18>"+a.18.1m+")(?<17>.*?)(?<1b>"+a.1b.1m+")","5o")}}};H e}();1j 2e!="1d"&&(2e.1v=1v);', 62, 441, '||||||function|||||||||||||||||||||||||||||||||||||return|if|this|var|length|XRegExp|null|for|index|replace|true||div|push|getParam|call|exec|else|prototype||false|lastIndex|config|arguments|RegExp|toolbar|code|left|captureNames|slice|right|id|undefined|split|new|class|addToken|indexOf|typeof|script|className|source|params|substr|apply|toString|String|line|title|gutter|SyntaxHighlighter|_xregexp|strings|lt|html|test|OUTSIDE_CLASS|match|brush|document|target|gt|getHtml|regex|global|join|style|highlight|break|concat|window|Math|isRegExp|throw|value|brushes|brushName|space|alert|vars|http|syntaxhighlighter|expandSource|size|css|case|font|Fa|name|htmlScript|dA|can|handler|gm|td|exports|color|in|href|first|discoveredBrushes|light|collapse|object|cache|getButtonHtml|trigger|pattern|getLineHtml|nbsp|numbers|parseInt|defaults|com|items|www|pad|highlighters|execute|focus|func|all|getDiv|parentNode|navigator|INSIDE_CLASS|regexList|hasFlag|Match|useScriptTags|hasNamedCapture|text|help|init|br|input|gi|Error|values|span|list|250|height|width|screen|top|500|tagName|findElements|getElementsByTagName|aboutDialog|_blank|appendChild|charAt|Array|copyAsGlobal|setFlag|highlighter_|string|attachEvent|nodeName|floor|backref|output|the|TypeError|sticky|Za|iterate|freezeTokens|scope|type|textarea|alexgorbatchev|version|margin|2010|005896|gs|regexLib|body|center|align|noBrush|require|childNodes|DTD|xhtml1|head|org|w3|url|preventDefault|container|tr|getLineNumbersHtml|isNaN|userAgent|tbody|isLineHighlighted|quick|void|innerHTML|create|table|links|auto|smart|tab|stripBrs|tabs|bloggerMode|collapsed|plain|getCodeLinesHtml|caption|getMatchesHtml|findMatches|figureOutLineNumbers|removeNestedMatches|getTitleHtml|brushNotHtmlScript|substring|createElement|Highlighter|load|HtmlScript|Brush|pre|expand|multiline|min|Can|ignoreCase|find|blur|extended|toLowerCase|aliases|addEventListener|innerText|textContent|wasn|select|createTextNode|removeChild|option|same|frame|xmlns|dtd|twice|1999|equiv|meta|htmlscript|transitional|1E3|expected|PUBLIC|DOCTYPE|on|W3C|XHTML|TR|EN|Transitional||configured|srcElement|Object|after|run|dblclick|matchChain|valueOf|constructor|default|switch|click|round|execAt|forHtmlScript|token|gimy|functions|getKeywords|1E6|escape|within|random|sgi|another|finally|supply|MSIE|ie|toUpperCase|catch|returnValue|definition|event|border|imsx|constructing|one|Infinity|from|when|Content|cellpadding|flags|cellspacing|try|xhtml|Type|spaces|2930402|hosted_button_id|lastIndexOf|donate|active|development|keep|to|xclick|_s|Xml|please|like|you|paypal|cgi|cmd|webscr|bin|highlighted|scrollbars|aspScriptTags|phpScriptTags|sort|max|scriptScriptTags|toolbar_item|_|command|command_|number|getElementById|doubleQuotedString|singleLinePerlComments|singleLineCComments|multiLineCComments|singleQuotedString|multiLineDoubleQuotedString|xmlComments|alt|multiLineSingleQuotedString|If|https|1em|000|fff|background|5em|xx|bottom|75em|Gorbatchev|large|serif|CDATA|continue|utf|charset|content|About|family|sans|Helvetica|Arial|Geneva|3em|nogutter|Copyright|syntax|close|write|2004|Alex|open|JavaScript|highlighter|July|02|replaceChild|offset|83'.split('|'), 0, {}))


/**
* SyntaxHighlighter
* http://alexgorbatchev.com/SyntaxHighlighter
*
* SyntaxHighlighter is donationware. If you are using it, please donate.
* http://alexgorbatchev.com/SyntaxHighlighter/donate.html
*
* @version
* 3.0.83 (July 02 2010)
* 
* @copyright
* Copyright (C) 2004-2010 Alex Gorbatchev.
*
* @license
* Dual licensed under the MIT and GPL licenses.
*/
; (function () {
// CommonJS
typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

function Brush() {
  function process(match, regexInfo) {
    var constructor = SyntaxHighlighter.Match,
			code = match[0],
			tag = new XRegExp('(&lt;|<)[\\s\\/\\?]*(?<name>[:\\w-\\.]+)', 'xg').exec(code),
			result = []
			;

    if (match.attributes != null) {
      var attributes,
				regex = new XRegExp('(?<name> [\\w:\\-\\.]+)' +
									'\\s*=\\s*' +
									'(?<value> ".*?"|\'.*?\'|\\w+)',
									'xg');

      while ((attributes = regex.exec(code)) != null) {
        result.push(new constructor(attributes.name, match.index + attributes.index, 'color1'));
        result.push(new constructor(attributes.value, match.index + attributes.index + attributes[0].indexOf(attributes.value), 'string'));
      }
    }

    if (tag != null)
      result.push(
				new constructor(tag.name, match.index + tag[0].indexOf(tag.name), 'keyword')
			);

    return result;
  }

  this.regexList = [
		{ regex: new XRegExp('(\\&lt;|<)\\!\\[[\\w\\s]*?\\[(.|\\s)*?\\]\\](\\&gt;|>)', 'gm'), css: 'color2' }, // <![ ... [ ... ]]>
		{regex: SyntaxHighlighter.regexLib.xmlComments, css: 'comments' }, // <!-- ... -->
		{regex: new XRegExp('(&lt;|<)[\\s\\/\\?]*(\\w+)(?<attributes>.*?)[\\s\\/\\?]*(&gt;|>)', 'sg'), func: process }
	];
};

Brush.prototype = new SyntaxHighlighter.Highlighter();
Brush.aliases = ['xml', 'xhtml', 'xslt', 'html'];

SyntaxHighlighter.brushes.Xml = Brush;

// CommonJS
typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();


/**
* SyntaxHighlighter
* http://alexgorbatchev.com/SyntaxHighlighter
*
* SyntaxHighlighter is donationware. If you are using it, please donate.
* http://alexgorbatchev.com/SyntaxHighlighter/donate.html
*
* @version
* 3.0.83 (July 02 2010)
* 
* @copyright
* Copyright (C) 2004-2010 Alex Gorbatchev.
*
* @license
* Dual licensed under the MIT and GPL licenses.
*/
; (function () {
  // CommonJS
  typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

  function Brush() {
    var keywords = 'abstract as base bool break byte case catch char checked class const ' +
					'continue decimal default delegate do double else enum event explicit ' +
					'extern false finally fixed float for foreach get goto if implicit in int ' +
					'interface internal is lock long namespace new null object operator out ' +
					'override params private protected public readonly ref return sbyte sealed set ' +
					'short sizeof stackalloc static string struct switch this throw true try ' +
					'typeof uint ulong unchecked unsafe ushort using virtual void while';

    function fixComments(match, regexInfo) {
      var css = (match[0].indexOf("///") == 0)
			? 'color1'
			: 'comments'
			;

      return [new SyntaxHighlighter.Match(match[0], match.index, css)];
    }

    this.regexList = [
		{ regex: SyntaxHighlighter.regexLib.singleLineCComments, func: fixComments }, 	// one line comments
		{regex: SyntaxHighlighter.regexLib.multiLineCComments, css: 'comments' }, 		// multiline comments
		{regex: /@"(?:[^"]|"")*"/g, css: 'string' }, 		// @-quoted strings
		{regex: SyntaxHighlighter.regexLib.doubleQuotedString, css: 'string' }, 		// strings
		{regex: SyntaxHighlighter.regexLib.singleQuotedString, css: 'string' }, 		// strings
		{regex: /^\s*#.*/gm, css: 'preprocessor' }, 	// preprocessor tags like #region and #endregion
		{regex: new RegExp(this.getKeywords(keywords), 'gm'), css: 'keyword' }, 		// c# keyword
		{regex: /\bpartial(?=\s+(?:class|interface|struct)\b)/g, css: 'keyword' }, 		// contextual keyword: 'partial'
		{regex: /\byield(?=\s+(?:return|break)\b)/g, css: 'keyword'}			// contextual keyword: 'yield'
		];

    this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
  };

  Brush.prototype = new SyntaxHighlighter.Highlighter();
  Brush.aliases = ['c#', 'c-sharp', 'csharp'];

  SyntaxHighlighter.brushes.CSharp = Brush;

  // CommonJS
  typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
/**
* SyntaxHighlighter
* http://alexgorbatchev.com/SyntaxHighlighter
*
* SyntaxHighlighter is donationware. If you are using it, please donate.
* http://alexgorbatchev.com/SyntaxHighlighter/donate.html
*
* @version
* 3.0.83 (July 02 2010)
* 
* @copyright
* Copyright (C) 2004-2010 Alex Gorbatchev.
*
* @license
* Dual licensed under the MIT and GPL licenses.
*/
; (function () {
  // CommonJS
  typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

  function Brush() {
    var keywords = 'break case catch continue ' +
					'default delete do else false  ' +
					'for function if in instanceof ' +
					'new null return super switch ' +
					'this throw true try typeof var while with'
					;

    var r = SyntaxHighlighter.regexLib;

    this.regexList = [
		{ regex: r.multiLineDoubleQuotedString, css: 'string' }, 		// double quoted strings
		{regex: r.multiLineSingleQuotedString, css: 'string' }, 		// single quoted strings
		{regex: r.singleLineCComments, css: 'comments' }, 		// one line comments
		{regex: r.multiLineCComments, css: 'comments' }, 		// multiline comments
		{regex: /\s*#.*/gm, css: 'preprocessor' }, 	// preprocessor tags like #region and #endregion
		{regex: new RegExp(this.getKeywords(keywords), 'gm'), css: 'keyword'}			// keywords
		];

    this.forHtmlScript(r.scriptScriptTags);
  };

  Brush.prototype = new SyntaxHighlighter.Highlighter();
  Brush.aliases = ['js', 'jscript', 'javascript'];

  SyntaxHighlighter.brushes.JScript = Brush;

  // CommonJS
  typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
