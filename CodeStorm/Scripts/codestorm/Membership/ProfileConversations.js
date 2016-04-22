// <reference path="~/Scripts/lib/jquery.js" />
/// <reference path="~/Scripts/lib/knockout.js" />

define(["knockout"], function (ko) {

  var context = $(".conversations-component")[0];
  var props = $(context).data();

  ProfileConversations = function ProfileConversations() {
    if (!context) return false;
    var self = this;

    this.mentions = ko.observableArray([]);
    this.links = ko.observableArray([]);

    $.ajax({
      url: "//content.codestorm.in/social/" + props.csAbchandle + "?callback=?",
      dataType: "jsonp"
    });

    window.CodeStormRenderProfile = function (data) {

      self.GetMentions(data.TwitterMentions, function (mentions) {

        for (var i = 0; i < mentions.length; i++) {
          self.mentions.push(mentions[i]);
        }

      });

    };
  };

  ProfileConversations.prototype.GetMentions = function (mentions, callback) {
    var arr = [];

    var self = this;

    $(mentions).each(function (index, mention) {
      $.getJSON("http://api.twitter.com/1/users/show.json?callback=?&screen_name=" + mention.TwitterHandle, function (response) {

        mention.TwitterPicture = "https://api.twitter.com/1/users/profile_image?screen_name=" + mention.TwitterHandle;
        mention.TwitterDescription = response.description;
        mention.TwitterName = response.name;
        if (mention.AbcHandle) {
          mention.TwitterProfileUrl = "/DevOp/" + mention.AbcHandle;
        } else {
          mention.TwitterProfileUrl = "http://www.twitter.com/" + response.screen_name;
        }

        arr.push(mention);
        if (mentions.length - 1 == index) {
          callback.call(self, arr);
        }
      });
    });
  };

  return ProfileConversations;
});
