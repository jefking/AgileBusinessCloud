//SERVICES
var ServiceLocator = mojo.ServiceLocator;

ServiceLocator
  .addService(new mojo.Service("getUsers", "/User/Users"))
  .addService(new mojo.Service("addUserRole", "/ManagementAPI/SaveUserRole"))
  .addService(new mojo.Service("addUserToApplication", "/Application/AddUser"))
  .addService(new mojo.Service("addApplication", "/Apps/Details"))
  .addService(new mojo.Service("addUserPreference", "/User/Save"))
  .addService(new mojo.Service("getUserApplications", "/Application/GetApplications"))

  .addService(new mojo.Service("addApplicationConfiguration", "/Configuration/Save"))
  .addService(new mojo.Service("getApplicationConfiguration", "/Configuration/Get", { method: "post" }))

  //.addService(new mojo.Service("getApplicationPerformance", "/Log/Performance"))
  .addService(new mojo.Service("getApplicationPerformance", "http://content.agilebusinesscloud.com/api/{PublicKey}/Occurrences/1-days", { jsonp: true, template: true }))
  .addService(new mojo.Service("getApplicationMessages", "/Log/Message"))
  .addService(new mojo.Service("getApplicationErrors", "/Log/Error"))
  .addService(new mojo.Service("getTopApplicationErrors", "/Log/ErrorsCompressed"))
  .addService(new mojo.Service("getCompressedErrors", "http://content.agilebusinesscloud.com/api/{PublicKey}/ErrorsCompressed/1-days", { jsonp: true, template: true }))
  .addService(new mojo.Service("getServerStatistics", "/Log/ServerStatistics", { method: "post" }))

  .addService(new mojo.Service("getGithubActivity", "https://github.com/{username}.json?callback=?", { jsonp: true, template: true }))

  .addService(new mojo.Service("getCurrentThroughput", "/Analytics/CurrentThroughput"))
  .addService(new mojo.Service("getApplicationUsage", "/Analytics/ApplicationDataUsage"))
  .addService(new mojo.Service("getTweets", "/External/CompanyTweets"));

var app = mojo.create({ baseSrc: "/Scripts/" });

app
  .configure("appName", "abc")
  .configure("logging", false);

if (window.location.href.match(/local|test/gi)) {
  app.configure("logging", true);
  $.ajaxSetup({ timeout: 45e3 });
}

app
 .map("html", [{ controller: "abc.controller.UI.LoadingScreenController" }, { controller: "abc.controller.UI.RealTimeGraphLoadingScreenController" }])
 .map("ul.applications-menu-list", [{ controller: "abc.controller.Application.ApplicationMenuController" }])
 .map(".component-user-plate", [{ controller: "abc.controller.User.UserPlateController" }])
 .map("#add-application-form", [{ controller: "abc.controller.Application.ApplicationDetailsController" }])
 .map("#add-user-to-application", [{ controller: "abc.controller.Application.AddUserToApplicationController" }])
 .map(".component-tweets", [{ controller: "abc.controller.External.TweetController" }])
 .map(".component-application-roles", [{ controller: "abc.controller.Application.ApplicationRolesController" }])
 .map(".component-application-list", [{ controller: "abc.controller.Application.ListController" }])

// User Profiles
 .map(".account-my-profile .section-apps .applications-list", [{ controller: "abc.controller.Profile.ApplicationsController" }])
 .map(".section-github-activities", [{ controller: "abc.controller.Profile.GithubActivityController" }])

// Graphs
 .map(".component-widget-graph:visible", [{ controller: "abc.controller.Application.HealthGraphController" }])
 .map(".component-performance-usage.method-execution", [{ controller: "abc.controller.Application.Performance.MethodExecutionController" }])
 .map(".component-errors-throughput", [{ controller: "abc.controller.Application.Error.ErrorsThroughputController" }])
 .map(".component-server-statistics", [{ controller: "abc.controller.Application.Server.ServerStatsController" }])
 .map(".component-performance-class-list", [{ controller: "abc.controller.Application.Performance.ClassListController" }])

//Experimental
 .map(".component-application-settings", [{ controller: "abc.controller.Application.ConfigurationController" }])
 .map(".component-user-gallery", [{ controller: "abc.controller.User.UserGalleryController" }])
 .map(".component-top-errors", [{ controller: "abc.controller.Application.Error.TopErrorsController" }])

 .start();
