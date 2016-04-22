/// <reference path="~/Scripts/lib/jquery.js" />
/// <reference path="~/Scripts/lib/knockout.js" />

// Provides injection for most plugins in our plguin directory
define([
], function () {
  return function PluginInjector() {
    console.log("Plugins Loaded: " + arguments.length);
  };

});