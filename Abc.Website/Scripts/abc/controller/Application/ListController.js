/* 
* @class: Application List Controller
* @namespace: Application
* @author: Agile Business Cloud
* 
* Responsible applying behaviours on top of the Application List View
*/
mojo.define("abc.controller.Application.ListController", function ($) {

  var Controller = {
    events: [
      ['context', 'a.btn-copy-to-clipboard', 'click', 'Copy']
    ],
    methods: {
      Initialize: function () {
        var context = $(this.getContextElement());
        $(context).find("a.btn-copy-to-clipboard").zclip({
          path: '/Content/flash/ZeroClipboard.swf',
          copy: function (event) {
            return $(event.target).data().id;
          },
          afterCopy: function (event) {
            alert("Copied " + $(event.target).data().id + " to your clipboard");
          }
        });
      },
      Copy: function (requestObj) {
        requestObj.getEvent().preventDefault();
      }
    }
  };

  return Controller;
});
