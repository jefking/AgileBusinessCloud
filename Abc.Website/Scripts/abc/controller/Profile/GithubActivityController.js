/* 
* @class: Github Activity View Controller
* @namespace: Profile
* @author: Agile Business Cloud
* 
* Responsible fetching and rendering Github Activity over JSONP.
*/
mojo.define("abc.controller.Profile.GithubActivityController", function ($) {

  var Controller = {
    events: [],
    methods: {
      Initialize: function () {
        mojo.Messaging.publish("UI.LoadingScreen", { show: true, target: this.getContextElement() });
        var user = $("input[name='GitHubHandle']").val();
        if (!user) {
          return this.View("abc.profile.GithubActivities", { GithubActivities: [] });
        }
        ServiceLocator.getService("getGithubActivity").invoke({ username: user, callback: "Render" }, this.Render, this);
      },
      View: function (key, value) {
        mojo.Model.set(key, value);
      },
      Render: function (err, data) {
        var self = this;

        var data = { GithubActivities: function (data) {
          var tmp = [];
          for (var i = 0; i < data.length && i < 10; i++) {
            var activity = data[i];
            activity.TargetPicture = activity.actor_attributes.gravatar_id;
            activity.DateRelative = prettyDate(new Date(activity.created_at));
            switch (activity.type) {
              case "FollowEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just followed <a href='" + activity.url + "' target='_blank'>" + activity.payload.target.login + "</a>.";
                activity.TargetPicture = activity.payload.target.gravatar_id;
                break;
              case "PushEvent":
                var commits = activity.payload.shas;
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just pushed to <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a> &rarr; <a href='" + activity.url + "' target='_blank'>View Diff</a>";
                break;
              case "IssuesEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just created an issue in <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a> &rarr; <a href='" + activity.url + "' target='_blank'>View Issue</a>";
                break;
              case "IssueCommentEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just commented on <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a> &rarr; <a href='" + activity.url + "' target='_blank'>View Comment</a>";
                break;
              case "WatchEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just watched <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a>";
                break;
              case "ForkEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just forked <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a>";
                break;
              case "PullRequestEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just initiated a pull request on <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a> &rarr; <a href='" + activity.url + "' target='_blank'>View Pull Request</a>";
                break;
              case "CommitCommentEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just commented on <a href='" + activity.repository.url + "' target='_blank'>" + activity.repository.owner + "." + activity.repository.name + "</a> &rarr; <a href='" + activity.url + "' target='_blank'>View Issue</a>";
                break;
              case "GistEvent":
                activity.Message = "<a href='http://github.com/" + activity.actor + "' target='_blank'>" + activity.actor + "</a> just <a href='" + activity.url + "' target='_blank'>created a gist</a>";
                break;
              default:
                activity.Message = "Not found.";
            }

            activity.Commits = [];

            tmp.push(activity);
          }

          return tmp;
        } (data)
        };
        self.View("abc.profile.GithubActivities", data);

        mojo.Messaging.publish("UI.LoadingScreen", { show: false });
      }
    }
  };

  return Controller;
});
