/// <reference path="~/Scripts/lib/jquery.js" />
/// <reference path="~/Scripts/lib/knockout.js" />

require([
  "knockout",
  "App/PluginInjector",
  "Analytics/GlobalMetrics",
  "Membership/ProfileScoreBoard",
  "Membership/ProfileConversations",
  "Membership/ProfileWelcome"
], function (ko
  , Injector
  , Metrics
  , ProfileScoreBoard
  , ProfileWelcome) {

  ko.applyBindings(new ProfileScoreBoard(), $(".codestorm-hero-view")[0]);
  ko.applyBindings(new ProfileConversations(), $(".conversations-component")[0]);
});