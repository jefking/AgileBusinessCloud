/*
****  On DOM Load 
*/
$(document).ready(function () {


  /***** Feature - Form validation ******************************************/
  if (typeof ($.validity) == 'object') {
    $.validity.setup({ outputMode: "label" });

    $("form").validity(function () {
      $('#val-date').require().match('date');
      $('#val-number').require().match('integer');
      $('#val-time').require().match('time24');
      $('#val-url').require().match('url');
      $('#val-email').require().match('email');
    });

    $("input").focus(function () {
      $("label.error[for='" + this.id + "']").fadeOut(function () {
        $(this).remove();
      })
    });
  }


  /***** Feature - accordion *************************************************/
  if ($('.widget-accordion').size()) {

    $('.widget-accordion li>h3').click(function () {
      var parent = $(this).closest('li');
      var widget = $(this).closest('.widget-accordion');
      var expanded = true;

      if ($('div:hidden', parent).size()) {
        expanded = false;
      }

      if (expanded == false) {

        $('li.current>div', widget).slideUp(150, function () {
          $('li.current', widget).removeClass('current');

          $('div:hidden', parent).slideDown(350, function () {
            $(parent).addClass('current');
          });
        });

      } else {
        $('div:hidden', parent).slideUp();
      }

    });
  }


  /***** Feature - Notice bars **********************************************/
  if ($('.bar-notice').size()) {
    $('.bar-notice>span').click(function () {
      $(this).closest('div').fadeOut('fast');
    });
  }


  /***** Feature - Dialogs **************************************************/
  if ($('.modalDialog').size()) {
    $('.modalDialog').each(function () {
      var id = '#' + $(this).attr('rel');
      $(this).colorbox({ width: "50%", inline: true, href: id });
    });
  }


  /***** Feature - Lightbox *************************************************/
  if ($("a[rel='photoGallery']").size()) {
    $("a[rel='photoGallery']").colorbox();
  }


  /***** Panel - Tables *****************************************************/
  if ($('.table-sortable').size()) {
    var str_ver = typeof (jQuery.browser.msie) + jQuery.browser.version;

    if (str_ver != 'boolean7.0') {
      $('.table-sortable').dataTable();
    }
  }

  /***** Panel - Gallery ****************************************************/
  if ($('.gallery-view-thumbnail').size()) {
    rev_loadGallery();
  }


//  /***** Panel - WYSIWYG ****************************************************/
//  if ($('#niceEditArea').size()) {
//    new nicEditor({ fullPanel: true, iconsPath: '_layout/scripts/nicedit/nicEditorIcons.gif' }).panelInstance('niceEditArea');
//  }


  /***** JGrowl *************************************************************/
  if ($('.jgrowl-notify').size()) {
    $('.jgrowl-notify').click(function () {
      var text = $(this).attr('rel');
      $.jGrowl(text);

      return false;
    });
  }

  /***** Panel - Calendar ****************************************************/
  if ($('#calendar-normal').size()) {
    rev_loadCalendar();
  }


  /***** Feature- Top menu **************************************************/
  $('#toolbar>ul>li').mouseenter(function () {
    menuItem = $('ul', this).css('visibility', 'visible');

    var parent = $(this).closest('li');
    $(parent).addClass('current');
  });

  $('#toolbar>ul>li').mouseleave(function () {
    $(this).removeClass('current');
  });

  /***** Feature- Tabs Support **********************************************/
  if ($('.widget-tabs-horizontal').size()) {
    rev_loadTabsWidget();
  }
});





function rev_loadTabsWidget() {
  $('.widget-tabs-box li').click(function () {
    var widget = $(this).closest('.widget-tabs-horizontal');
    var tab_content_id = $('a', this).attr('rel');

    $('.widget-tabs-box li', widget).removeClass('current');
    $(this).addClass('current');

    if ($('#' + tab_content_id, widget).size()) {
      $('.tabs-content.current', widget).removeClass('current');
      $('#' + tab_content_id + '.tabs-content', widget).addClass('current');
    }

    return false;
  });

  $('.widget-tabs-box li').click(function () {
    var widget = $(this).closest('.widget-tabs-vertical');
    var tab_content_id = $('a', this).attr('rel');

    $('.widget-tabs-box li', widget).removeClass('current');
    $(this).addClass('current');

    if ($('#' + tab_content_id, widget).size()) {
      $('.tabs-content.current', widget).removeClass('current');
      $('#' + tab_content_id + '.tabs-content', widget).addClass('current');
    }

    return false;
  });
}

function rev_loadCalendar() {
  var date = new Date();
  var d = date.getDate();
  var m = date.getMonth();
  var y = date.getFullYear();

  $('#calendar-normal').fullCalendar({
    editable: true,
    events: [
				{
				  title: 'All Day Event',
				  start: new Date(y, m, 1)
				},
				{
				  title: 'Long Event',
				  start: new Date(y, m, d - 5),
				  end: new Date(y, m, d - 2)
				},
				{
				  id: 999,
				  title: 'Repeating Event',
				  start: new Date(y, m, d - 3, 16, 0),
				  allDay: false
				},
				{
				  id: 999,
				  title: 'Repeating Event',
				  start: new Date(y, m, d + 4, 16, 0),
				  allDay: false
				},
				{
				  title: 'Meeting',
				  start: new Date(y, m, d, 10, 30),
				  allDay: false
				},
				{
				  title: 'Lunch',
				  start: new Date(y, m, d, 12, 0),
				  end: new Date(y, m, d, 14, 0),
				  allDay: false
				},
				{
				  title: 'Birthday Party',
				  start: new Date(y, m, d + 1, 19, 0),
				  end: new Date(y, m, d + 1, 22, 30),
				  allDay: false
				},
				{
				  title: 'Click for Google',
				  start: new Date(y, m, 28),
				  end: new Date(y, m, 29),
				  url: 'http://google.com/'
				}
			]
  });
}

function rev_loadGallery() {
  $(".gallery-view-thumbnail").sortable({
    placeholder: "thumb-state-highlight"
  });

  $(".gallery-view-thumbnail").disableSelection();

  $('.gallery-view-thumbnail .remove').click(function () {
    var thumb = $(this).closest('li');
    $(thumb).fadeOut('slow');
  });


  $('.gallery-view-thumbnail li').hover(function () {
    $('.edit, .remove', this).show(); //fadeIn('fast');
  }, function () {
    $('.edit, .remove', this).hide(); //.fadeOut('slow');
  });
}
	

	