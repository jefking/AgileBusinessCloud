
/*
* JavaScript Pretty Date
* Copyright (c) 2008 John Resig (jquery.com)
* Licensed under the MIT license.
* Modified for UTC
*/
function prettyDate(date, fix) {
  // duckpunch for C# date times to human relative dates
  if (fix) {
    date = eval('new ' + date.replace(/\//g, ''));
  }

  //modification to prettyDate function
  //- assume: param 'date' is now utc from backend
  //- wrap better checking around nulls

  if ('undefined' == typeof date || !date) return "";

  var now = new Date();
  var utc = new Date(now.getUTCFullYear(),
                      now.getUTCMonth(),
                      now.getUTCDate(),
                      now.getUTCHours(),
                      now.getUTCMinutes(),
                      now.getUTCSeconds());

  //dateOffset = 0;
  dateOffset = date.getTimezoneOffset() * 60000;
  utcOffset = 0;
  //utcOffset = utc.getTimezoneOffset() * 60000;
  diff = (((utc.getTime() + utcOffset) - (date.getTime() + dateOffset)) / 1000),
  day_diff = Math.floor(diff / 86400);

  if (isNaN(day_diff) || day_diff < 0 || day_diff >= 365) {
    return "recently";
  }

  return day_diff == 0 && (
			diff < 60 && "just now" ||
			diff < 120 && "1 minute ago" ||
			diff < 3600 && Math.floor(diff / 60) + " minutes ago" ||
			diff < 7200 && "1 hour ago" ||
			diff < 86400 && Math.floor(diff / 3600) + " hours ago") ||
		day_diff == 1 && "yesterday" ||
		day_diff < 7 && day_diff + " days ago" ||
        day_diff < 14 && "1 week ago" ||
		day_diff < 365 && Math.ceil(day_diff / 7) + " weeks ago";
}
