function ShowSigninPage(json) {
  if (!json.length) return false;
  if ("object" != typeof json) return false;

  var filled = function (data) {
    var ret = [];
    $(data).each(function (index, item) {

      if (item.Name.match(/windows/gi)) {
        item.CssClass = "windows-live";
      } else if (item.Name.match(/facebook/gi)) {
        item.CssClass = "facebook";
      } else if (item.Name.match(/google/gi)) {
        item.CssClass = "google";
      } else if (item.Name.match(/yahoo/gi)) {
        item.CssClass = "yahoo";
      }

      ret.push(item);
    });

    return ret;
  } (json);

  var data = { Providers: json };
  mojo.Model.set("abc.menu.IdentityProviders", data);
  mojo.Model.set("abc.User.SignIn", data);
  
  return true;
};
(function ($) {
  var realm = window.location.protocol + "//" + window.location.host;
  var urlToScript = "https://" + ABC.FederationTLD + ":443/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm=" + realm + "&reply_to=&context=&request_id=&version=1.0&callback=ShowSigninPage";
  $.getScript(urlToScript);
})(jQuery);
