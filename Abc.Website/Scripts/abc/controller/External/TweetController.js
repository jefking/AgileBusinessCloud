/* 
 * Class: External.TweetController
 *
 * @description Provides a modular approach to rendering tweets from our own web service
 */
mojo.define("abc.controller.External.TweetController", function () {
  var Controller = {
    methods: {
      Initialize: function () {
        var context = this.getContextElement();
        ServiceLocator.getService("getTweets").invoke(null, function (err, response) {
          if (response.length) {
            $(context).show();
            mojo.Model.set("abc.External.Tweets", { Tweets: response });
            $(context).cycle({ delay: 1000 });
          }
        });
      }
    }
  };

  return Controller;
});