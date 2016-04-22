/// <reference path="~/Scripts/lib/jquery.js" />
/// <reference path="~/Scripts/lib/knockout.js" />

define(["knockout"], function (ko) {

  var context = $(".codestorm-hero-view")[0];
  var props = $(context).data();

  return function ProfileScoreBoard() {
    if (!context) return false;
    var self = this;

    this.githubHandle = ko.observable(props.csGithubhandle);
    this.twitterHandle = ko.observable(props.csTwitterhandle);
    this.abcHandle = ko.observable(props.csAbchandle);
    this.twitterStatusCount = ko.observable(0);
    this.twitterFollowerCount = ko.observable(0);
    this.name = ko.observable('');
    this.description = ko.observable('');

    $.ajax({
      url: "//api.twitter.com/1/users/show.json?callback=parseTweets&screen_name=" + props.csTwitterhandle,
      dataType: "jsonp"
    });
    window.parseTweets = function (data) {
      var githubUsername = props.csGithubhandle;
      var twitterUsername = props.csTwitterhandle;
      if (!twitterUsername) { return false; };
      self.twitterStatusCount(data.statuses_count);
      self.twitterFollowerCount(data.followers_count);
      self.name(data.name);
      var description = data.description.replace(/(\r\n|\n|\r)/gm, "");
      self.description(description);
    };
  };

});