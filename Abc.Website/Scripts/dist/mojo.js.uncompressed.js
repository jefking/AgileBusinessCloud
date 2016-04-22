/*!
 * Mojo JavaScript Microframework
 * 
 * Copyright 2012 Agile Business Cloud Solutions Ltd.

 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/*!
 * mustache.js - Logic-less {{mustache}} templates with JavaScript
 * http://github.com/janl/mustache.js
 */

/*global define: false*/

var Mustache;

(function(exports) {
  if (typeof module !== "undefined") {
    module.exports = exports; // CommonJS
  } else if (typeof define === "function") {
    define(exports); // AMD
  } else {
    Mustache = exports; // <script>
  }
}((function() {
  var exports = {};

  exports.name = "mustache.js";
  exports.version = "0.5.2";
  exports.tags = ["{{", "}}"];

  exports.parse = parse;
  exports.clearCache = clearCache;
  exports.compile = compile;
  exports.compilePartial = compilePartial;
  exports.render = render;

  exports.Scanner = Scanner;
  exports.Context = Context;
  exports.Renderer = Renderer;

  // This is here for backwards compatibility with 0.4.x.
  exports.to_html = function(template, view, partials, send) {
    var result = render(template, view, partials);

    if (typeof send === "function") {
      send(result);
    } else {
      return result;
    }
  };

  var whiteRe = /\s*/;
  var spaceRe = /\s+/;
  var nonSpaceRe = /\S/;
  var eqRe = /\s*=/;
  var curlyRe = /\s*\}/;
  var tagRe = /#|\^|\/|>|\{|&|=|!/;

  // Workaround for https://issues.apache.org/jira/browse/COUCHDB-577
  // See https://github.com/janl/mustache.js/issues/189
  function testRe(re, string) {
    return RegExp.prototype.test.call(re, string);
  }

  function isWhitespace(string) {
    return !testRe(nonSpaceRe, string);
  }

  var isArray = Array.isArray ||
  function(obj) {
    return Object.prototype.toString.call(obj) === "[object Array]";
  };

  // OSWASP Guidelines: escape all non alphanumeric characters in ASCII space.
  var jsCharsRe = /[\x00-\x2F\x3A-\x40\x5B-\x60\x7B-\xFF\u2028\u2029]/gm;

  function quote(text) {
    var escaped = text.replace(jsCharsRe, function(c) {
      return "\\u" + ('0000' + c.charCodeAt(0).toString(16)).slice(-4);
    });

    return '"' + escaped + '"';
  }

  function escapeRe(string) {
    return string.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&");
  }

  var entityMap = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': '&quot;',
    "'": '&#39;',
    "/": '&#x2F;'
  };

  function escapeHtml(string) {
    return String(string).replace(/[&<>"'\/]/g, function(s) {
      return entityMap[s];
    });
  }

  // Export these utility functions.
  exports.isWhitespace = isWhitespace;
  exports.isArray = isArray;
  exports.quote = quote;
  exports.escapeRe = escapeRe;
  exports.escapeHtml = escapeHtml;

  function Scanner(string) {
    this.string = string;
    this.tail = string;
    this.pos = 0;
  }

  /**
   * Returns `true` if the tail is empty (end of string).
   */
  Scanner.prototype.eos = function() {
    return this.tail === "";
  };

  /**
   * Tries to match the given regular expression at the current position.
   * Returns the matched text if it can match, the empty string otherwise.
   */
  Scanner.prototype.scan = function(re) {
    var match = this.tail.match(re);

    if (match && match.index === 0) {
      this.tail = this.tail.substring(match[0].length);
      this.pos += match[0].length;
      return match[0];
    }

    return "";
  };

  /**
   * Skips all text until the given regular expression can be matched. Returns
   * the skipped string, which is the entire tail if no match can be made.
   */
  Scanner.prototype.scanUntil = function(re) {
    var match, pos = this.tail.search(re);

    switch (pos) {
    case -1:
      match = this.tail;
      this.pos += this.tail.length;
      this.tail = "";
      break;
    case 0:
      match = "";
      break;
    default:
      match = this.tail.substring(0, pos);
      this.tail = this.tail.substring(pos);
      this.pos += pos;
    }

    return match;
  };

  function Context(view, parent) {
    this.view = view;
    this.parent = parent;
    this.clearCache();
  }

  Context.make = function(view) {
    return (view instanceof Context) ? view : new Context(view);
  };

  Context.prototype.clearCache = function() {
    this._cache = {};
  };

  Context.prototype.push = function(view) {
    return new Context(view, this);
  };

  Context.prototype.lookup = function(name) {
    var value = this._cache[name];

    if (!value) {
      if (name === ".") {
        value = this.view;
      } else {
        var context = this;

        while (context) {
          if (name.indexOf(".") > 0) {
            var names = name.split("."),
              i = 0;

            value = context.view;

            while (value && i < names.length) {
              value = value[names[i++]];
            }
          } else {
            value = context.view[name];
          }

          if (value != null) {
            break;
          }

          context = context.parent;
        }
      }

      this._cache[name] = value;
    }

    if (typeof value === "function") {
      value = value.call(this.view);
    }

    return value;
  };

  function Renderer() {
    this.clearCache();
  }

  Renderer.prototype.clearCache = function() {
    this._cache = {};
    this._partialCache = {};
  };

  Renderer.prototype.compile = function(tokens, tags) {
    if (typeof tokens === "string") {
      tokens = parse(tokens, tags);
    }

    var fn = compileTokens(tokens),
      self = this;

    return function(view) {
      return fn(Context.make(view), self);
    };
  };

  Renderer.prototype.compilePartial = function(name, tokens, tags) {
    this._partialCache[name] = this.compile(tokens, tags);
    return this._partialCache[name];
  };

  Renderer.prototype.render = function(template, view) {
    var fn = this._cache[template];

    if (!fn) {
      fn = this.compile(template);
      this._cache[template] = fn;
    }

    return fn(view);
  };

  Renderer.prototype._section = function(name, context, callback) {
    var value = context.lookup(name);

    switch (typeof value) {
    case "object":
      if (isArray(value)) {
        var buffer = "";

        for (var i = 0, len = value.length; i < len; ++i) {
          buffer += callback(context.push(value[i]), this);
        }

        return buffer;
      }

      return value ? callback(context.push(value), this) : "";
    case "function":
      // TODO: The text should be passed to the callback plain, not rendered.
      var sectionText = callback(context, this),
        self = this;

      var scopedRender = function(template) {
          return self.render(template, context);
        };

      return value.call(context.view, sectionText, scopedRender) || "";
    default:
      if (value) {
        return callback(context, this);
      }
    }

    return "";
  };

  Renderer.prototype._inverted = function(name, context, callback) {
    var value = context.lookup(name);

    // From the spec: inverted sections may render text once based on the
    // inverse value of the key. That is, they will be rendered if the key
    // doesn't exist, is false, or is an empty list.
    if (value == null || value === false || (isArray(value) && value.length === 0)) {
      return callback(context, this);
    }

    return "";
  };

  Renderer.prototype._partial = function(name, context) {
    var fn = this._partialCache[name];

    if (fn) {
      return fn(context);
    }

    return "";
  };

  Renderer.prototype._name = function(name, context, escape) {
    var value = context.lookup(name);

    if (typeof value === "function") {
      value = value.call(context.view);
    }

    var string = (value == null) ? "" : String(value);

    if (escape) {
      return escapeHtml(string);
    }

    return string;
  };

  /**
   * Low-level function that compiles the given `tokens` into a
   * function that accepts two arguments: a Context and a
   * Renderer. Returns the body of the function as a string if
   * `returnBody` is true.
   */
  function compileTokens(tokens, returnBody) {
    var body = ['""'];
    var token, method, escape;

    for (var i = 0, len = tokens.length; i < len; ++i) {
      token = tokens[i];

      switch (token.type) {
      case "#":
      case "^":
        method = (token.type === "#") ? "_section" : "_inverted";
        body.push("r." + method + "(" + quote(token.value) + ", c, function (c, r) {\n" + "  " + compileTokens(token.tokens, true) + "\n" + "})");
        break;
      case "{":
      case "&":
      case "name":
        escape = token.type === "name" ? "true" : "false";
        body.push("r._name(" + quote(token.value) + ", c, " + escape + ")");
        break;
      case ">":
        body.push("r._partial(" + quote(token.value) + ", c)");
        break;
      case "text":
        body.push(quote(token.value));
        break;
      }
    }

    // Convert to a string body.
    body = "return " + body.join(" + ") + ";";

    // Good for debugging.
    // console.log(body);
    if (returnBody) {
      return body;
    }

    // For great evil!
    return new Function("c, r", body);
  }

  function escapeTags(tags) {
    if (tags.length === 2) {
      return [
      new RegExp(escapeRe(tags[0]) + "\\s*"), new RegExp("\\s*" + escapeRe(tags[1]))];
    }

    throw new Error("Invalid tags: " + tags.join(" "));
  }

  /**
   * Forms the given linear array of `tokens` into a nested tree structure
   * where tokens that represent a section have a "tokens" array property
   * that contains all tokens that are in that section.
   */
  function nestTokens(tokens) {
    var tree = [];
    var collector = tree;
    var sections = [];
    var token, section;

    for (var i = 0; i < tokens.length; ++i) {
      token = tokens[i];

      switch (token.type) {
      case "#":
      case "^":
        token.tokens = [];
        sections.push(token);
        collector.push(token);
        collector = token.tokens;
        break;
      case "/":
        if (sections.length === 0) {
          throw new Error("Unopened section: " + token.value);
        }

        section = sections.pop();

        if (section.value !== token.value) {
          throw new Error("Unclosed section: " + section.value);
        }

        if (sections.length > 0) {
          collector = sections[sections.length - 1].tokens;
        } else {
          collector = tree;
        }
        break;
      default:
        collector.push(token);
      }
    }

    // Make sure there were no open sections when we're done.
    section = sections.pop();

    if (section) {
      throw new Error("Unclosed section: " + section.value);
    }

    return tree;
  }

  /**
   * Combines the values of consecutive text tokens in the given `tokens` array
   * to a single token.
   */
  function squashTokens(tokens) {
    var lastToken;

    for (var i = 0; i < tokens.length; ++i) {
      var token = tokens[i];

      if (lastToken && lastToken.type === "text" && token.type === "text") {
        lastToken.value += token.value;
        tokens.splice(i--, 1); // Remove this token from the array.
      } else {
        lastToken = token;
      }
    }
  }

  /**
   * Breaks up the given `template` string into a tree of token objects. If
   * `tags` is given here it must be an array with two string values: the
   * opening and closing tags used in the template (e.g. ["<%", "%>"]). Of
   * course, the default is to use mustaches (i.e. Mustache.tags).
   */
  function parse(template, tags) {
    tags = tags || exports.tags;

    var tagRes = escapeTags(tags);
    var scanner = new Scanner(template);

    var tokens = [],
      // Buffer to hold the tokens
      spaces = [],
      // Indices of whitespace tokens on the current line
      hasTag = false,
      // Is there a {{tag}} on the current line?
      nonSpace = false; // Is there a non-space char on the current line?

    // Strips all whitespace tokens array for the current line
    // if there was a {{#tag}} on it and otherwise only space.
    var stripSpace = function() {
        if (hasTag && !nonSpace) {
          while (spaces.length) {
            tokens.splice(spaces.pop(), 1);
          }
        } else {
          spaces = [];
        }

        hasTag = false;
        nonSpace = false;
      };

    var type, value, chr;

    while (!scanner.eos()) {
      value = scanner.scanUntil(tagRes[0]);

      if (value) {
        for (var i = 0, len = value.length; i < len; ++i) {
          chr = value.charAt(i);

          if (isWhitespace(chr)) {
            spaces.push(tokens.length);
          } else {
            nonSpace = true;
          }

          tokens.push({
            type: "text",
            value: chr
          });

          if (chr === "\n") {
            stripSpace(); // Check for whitespace on the current line.
          }
        }
      }

      // Match the opening tag.
      if (!scanner.scan(tagRes[0])) {
        break;
      }

      hasTag = true;
      type = scanner.scan(tagRe) || "name";

      // Skip any whitespace between tag and value.
      scanner.scan(whiteRe);

      // Extract the tag value.
      if (type === "=") {
        value = scanner.scanUntil(eqRe);
        scanner.scan(eqRe);
        scanner.scanUntil(tagRes[1]);
      } else if (type === "{") {
        var closeRe = new RegExp("\\s*" + escapeRe("}" + tags[1]));
        value = scanner.scanUntil(closeRe);
        scanner.scan(curlyRe);
        scanner.scanUntil(tagRes[1]);
      } else {
        value = scanner.scanUntil(tagRes[1]);
      }

      // Match the closing tag.
      if (!scanner.scan(tagRes[1])) {
        throw new Error("Unclosed tag at " + scanner.pos);
      }

      tokens.push({
        type: type,
        value: value
      });

      if (type === "name" || type === "{" || type === "&") {
        nonSpace = true;
      }

      // Set the tags for the next time around.
      if (type === "=") {
        tags = value.split(spaceRe);
        tagRes = escapeTags(tags);
      }
    }

    squashTokens(tokens);

    return nestTokens(tokens);
  }

  // The high-level clearCache, compile, compilePartial, and render functions
  // use this default renderer.
  var _renderer = new Renderer();

  /**
   * Clears all cached templates and partials.
   */
  function clearCache() {
    _renderer.clearCache();
  }

  /**
   * High-level API for compiling the given `tokens` down to a reusable
   * function. If `tokens` is a string it will be parsed using the given `tags`
   * before it is compiled.
   */
  function compile(tokens, tags) {
    return _renderer.compile(tokens, tags);
  }

  /**
   * High-level API for compiling the `tokens` for the partial with the given
   * `name` down to a reusable function. If `tokens` is a string it will be
   * parsed using the given `tags` before it is compiled.
   */
  function compilePartial(name, tokens, tags) {
    return _renderer.compilePartial(name, tokens, tags);
  }

  /**
   * High-level API for rendering the `template` using the given `view`. The
   * optional `partials` object may be given here for convenience, but note that
   * it will cause all partials to be re-compiled, thus hurting performance. Of
   * course, this only matters if you're going to render the same template more
   * than once. If so, it is best to call `compilePartial` before calling this
   * function and to leave the `partials` argument blank.
   */
  function render(template, view, partials) {
    if (partials) {
      for (var name in partials) {
        compilePartial(name, partials[name]);
      }
    }

    return _renderer.render(template, view);
  }

  return exports;
}())));(function(win, doc) {
  
  "use strict";
  
  var $ = jQuery;

  var mojo = function() {};
  mojo.controllers = {};
  mojo.applications = {};
  mojo.options = {};
  mojo._loaded = [];
  
  mojo.resolve = function resolve(name) {
    if (!mojo._namespace._provided[name]) {
      return name.replace(/\./gi, '/');
    }
    
    return false;
  };
  /*
   * @private
   */
  mojo._namespace = function namespace(namespace) {
    var list = ('' + namespace).split(/\./)
      , listLength = list.length
      , obj = []
      , context = window || {};

    if (!mojo._namespace._provided) mojo._namespace._provided = {};
    
    if (mojo._namespace._provided[namespace] == namespace) throw new Error (namespace + " has already been defined.");


    for (var i = 0; i < listLength; i += 1) {
      var name = list[i];

      if (!context[ name ]) {
        obj[i] = name;
        context[name] = function() {};
        mojo._namespace._provided[obj.join('.')] = context[ name ];
      }
      context = context[ name ];
    }
    return context;
  };

  mojo.template = function template(template, data, partials) {
    if ('undefined' == typeof Mustache) return false;
    if ('undefined' == typeof template || !template) throw new Error("'template' is required");
    if ('undefined' == typeof data || !data) throw new Error("'data' is required");
    return Mustache.to_html(template, data, partials);
  };
  /* 
   * Returns an array of DOM nodes
   */
  mojo.query = function query() {
    if (!arguments.length) { return false; }
    return $.apply(this, arguments);
  };
  /* 
   * Returns the first element in a node list
   */
  mojo.queryFirst = function queryFirst() {
    if (!arguments.length) { return false; }
    var result = mojo.query.apply(this, arguments);
    if (!result.length) { return false; }
    return result[0];
  };
  
  /* 
   * Returns a GUID
   *
   */
  mojo.guid = function guid() {
    var s = [], itoh = '0123456789ABCDEF';

    // Make array of random hex digits. The UUID only has 32 digits in it, but we
    // allocate an extra items to make room for the '-'s we'll be inserting.
    for (var i = 0; i <36; i++) s[i] = Math.floor(Math.random()*0x10);

    // Conform to RFC-4122, section 4.4
    s[14] = 4;  // Set 4 high bits of time_high field to version
    s[19] = (s[19] & 0x3) | 0x8;  // Specify 2 high bits of clock sequence

    // Convert to hex chars
    for (var i = 0; i <36; i++) s[i] = itoh[s[i]];

    // Insert '-'s
    s[8] = s[13] = s[18] = s[23] = '-';

    return s.join('');
  };

  /* 
   * Fetch an array of dependencies, then fire a callback when done
   * @param dependencies {Array}
   * @param callback {Function}
   */
  mojo.require = function require(dependencies, callback) {
    if ('undefined' == typeof dependencies || !dependencies) throw new Error("'dependencies' is required");
    if ('undefined' == typeof callback || !callback) throw new Error("'callback' is required");
    
    if (!$.isArray(dependencies)) dependencies = [ dependencies ];
    
    var last = dependencies.length
      , path
      , callbackIndex = 0; 
      
    var allocated = mojo.controllers;
    for ( var i = 0; i < last; i++ ) {
      var dep = dependencies[i];
      path = mojo.options.baseSrc + mojo.resolve(dep) + ".js";
      mojo._loaded.push(dep);
      $.getScript(path, function() {
        //these are all loaded asynchronously
        callbackIndex++;  //callback counter so we can invoke a resolution event
                          //at the end of loading all dependencies
      });
    }

    var interval = setInterval(function() {
      if(callback && callbackIndex == last) { 
        clearInterval(interval);
        callback.call(this);
      }
    }, 25);
    
  };
  /* 
   * Synchronously load a module
   */
  mojo.requireSync = function requireSync(name) {
    var path = mojo.options.baseSrc + mojo.resolve(name) + ".js";
    $.ajaxSetup({async: false});
    $.getScript(path);
    $.ajaxSetup({async: true});
  };
  /* 
   * Get Controller Reference
   */
  mojo.getController = function getController(controllerName) {
    if ('undefined' == typeof mojo.controllers[controllerName]) return false;
    if ('string' != typeof controllerName) return false;
    return mojo.controllers[controllerName];
  };

  /* 
   * Retrieves a plugin
   * @param path {String} The location of the module on the file system
   * @param callback {Function} A callback to be executed when the plugin has completed loading
   * @deprecated 
   */
  mojo.fetch = function fetch(path, callback) {
    $.getScript(path, function() {
      if (callback) callback.apply(this, arguments);
    });
  };
  /* 
   * Defines a controller into mojo.controller namespace
   * @param id {String} The unique location of a Controller
   * @param factory {Function | Object} A body of a Controller
   */
  mojo.define = function(id, factory) { 
    if ('undefined' == typeof id || !id) throw new Error("'id' is required");
    if ('undefined' == typeof factory || !factory) throw new Error(id + " missing factory implementation");
    if ('function' == typeof factory) {
      factory = factory.call(this, jQuery);
    }
    
    if ('string' != typeof id) return false;
  
    if('string' == typeof id) {
      mojo._namespace( id );
      mojo._loaded[ id ] = factory;
      mojo.controllers[ id ] = factory;
    }    
  };

  /* 
   * Creates an mojo application instance. One web site can contain multiple mojo applications.
   * @param options {Object} A set of default options for particular mojo application
   */
  mojo.create = function create(options) {
    if (arguments.length > 1) throw new Error("Incorrect arguments");
    if ('undefined' == typeof options) {
      options = {};
      if (!options.baseSrc) options.baseSrc = 'js/';
    }
    $.extend(this.options, options);
    return new mojo.Application();
  };
  
  if (window) window.mojo = mojo;
   
})(window, document);
/* 
* Service
* @author Jaime Bueza
* A place where helpers can help developers
*/
mojo.define('mojo.helper', function($) {
  
  var helpers = {};
  
  /* 
   * isUrl
   * Determines whether the input is a valid URL
   * @param value {String} 
   */
  helpers.isUrl = function isUrl(value) {
    if (!value) return false;
    return /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(value);
  };
  
  /* 
   * isEmail
   * Determines whether the input is a valid RFC 2822 email address
   * @param value {String} 
   */
  helpers.isEmail = function isEmail(value) {
    if (!value) return false;
    return /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$/.test(value);
  }
  
  mojo.helper = helpers;
});/* 
 * @author Jaime Bueza
 */
mojo.define('mojo.Messaging', function() {
"use strict";

var $ = jQuery
  , messageStore = $({})
  , Messaging = function() {};

  /*
   * Subscribes to a particular messaging topic
   *
   * @param topic {String} - Represents a topic for messages to get pushed to
   * @param callback {Function} - A callback to execute when the topic receives a message
   */
  Messaging.subscribe = function() {
    messageStore.bind.apply( messageStore, arguments );
  };
  
  /* 
   * Unsubscribes to a particular topic
   * 
   * @param topic {String} - A Messaging topic to unsubscribe from
   */
  Messaging.unsubscribe = function() {
    messageStore.unbind.apply( messageStore, arguments );
  };

 /* 
  * Publishes a message to a particular topic
  * @param topic {String} - A Messaging Topic to push a message to
  * @param params {Object} - A JSON object that gets passed into the subscription topic
  * Example: mojo.Messaging.publish("MyAwesomeTopic", parameters)
  */
  Messaging.publish = function() {
    messageStore.trigger.apply( messageStore, arguments );
  };

  mojo.Messaging = Messaging;

});mojo.define("mojo.Model", function() {

"use strict";

var $ = jQuery, Model = function() {};
  
Model.set = function(key, value) {
  //find in the DOM, if it's an element, pass it into the templating engine
  //if it's not an HTML element, then we can just store it in DOM
  var models = mojo.query('*[modelSource="' + key + '"]')
    , contentOfModel;
  
  //makes an assumption that there is only one model 
  mojo._namespace(key);
  
  if (models.length) {
    $(models).each(function(index, model) {

      if (!model.mojoTemplate) {
        model.mojoTemplate = $(model).html().replace('%7B%7B', '{{').replace('%7D%7D', '}}');
      } 
      $(model).html("");
      var content = mojo.template(model.mojoTemplate, value);
      $(models).html(content);
      
      contentOfModel = $(models).html();
    });

    return contentOfModel;
    
  } else {
  	
  	window[key] = value;
  	return window[key];
  }
};

Model.get = function(key) {
  if (!key) { return false; }
  if ('string' != typeof key) { return false; }
  if ('undefined' == typeof mojo.ModelRegistry[key]) { return false; }
  if (arguments.lenght > 1) { return false; }
  return mojo.ModelRegistry[key];
};

Model.remove = function(key) {
  if (!key) { return false; }
  if ('string' != typeof(key)) { return false; }
  if ('undefined' == typeof mojo.ModelRegistry[key]) { return false; }
  delete mojo.ModelRegistry[key];
  return true;
};
  
  mojo.Model = Model;
  mojo.ModelRegistry = {};
});/* 
 * Request 
 *
 * Class representation of a Controller Request instance. 
 * Encapsulates request-specific parameters, and context-specific 
 * information.
 *
 * @author Jaime Bueza
 */
mojo.define('mojo.Request', function Request($) {

"use strict"; 
/* 
 * @param paramsObj {Object} - Hash object that gets passed into the request
 * @param callerObj {DOM} - When invoked by a user interaction, request objects will have access to the DOM element that was clicked (caller)
 * @param eventObj {DOM} - Browser event object, typically you can use this to preventDefault() inside a Mojo Controller's method
 * @param controllerObj {Object} - Reference to the controller that the interaction occurred in
 */
function Request(paramsObj, callerObj, eventObj, controllerObj) {
  if ('undefined' == typeof paramsObj || !paramsObj) throw new Error("'paramsObj' is required");
  if ('undefined' == typeof callerObj || !callerObj) throw new Error("'callerObj' is required");
  if ('undefined' == typeof eventObj || !eventObj) throw new Error("'eventObj' is required");
  if ('undefined' == typeof controllerObj || !controllerObj) throw new Error("'controllerObj' is required");
  
  this.paramsObj = paramsObj;
  this.callerObj = callerObj;
  this.eventObj = eventObj;
  this.controllerObj = controllerObj;  
};

/* 
 * Return the request's controller
 */
Request.prototype.getController = function() {
  return this.controllerObj;
};

/* 
 * Returns the context DOM element
 */
Request.prototype.getContextElement = function() {
  return this.getController().getContextElement();
};

/* 
 * Returns the object that invoked the request
 */
Request.prototype.getCaller = function() {
  return this.callerObj;
};

/* 
 * Returns an event object that was generated from the user interaction
 */
Request.prototype.getEvent = function() {
  return this.eventObj;
};

  mojo.Request = Request;
  
});/* 
 * Controller Class
 *
 * An abstract class used in implementing Mojo Controllers. A Controller is an 
 * object that encapsulates all event handling, dispatching and intercepting in 
 * a Mojo application.
 * 
 * @class       Controller
 * @author      Jaime Bueza
 * @constructor
 *
 */
mojo.define('mojo.Controller', function() {  

"use strict";

var $ = jQuery;
var noop = function() {};

function Controller() {
  this.contextElement = null;
  this.controllerClass = null;
  this.events;
};

Controller.prototype.onInit = function () {};
Controller.prototype.onParamChange = function () {};
Controller.prototype.onComplete = function () {};
Controller.prototype.onBind = function () {};
Controller.prototype.onIntercept = function () {};

Controller.prototype.params = {};

Controller.prototype.initialize = function(context, controllerName, params) {
  var self = this;

  self.contextElement = context;
  self.controllerClass = controllerName;
  
  if ('undefined' != typeof params || !params) self.params = params;

  $(self.events).each(function(index, observer) {
    var root = $(document)
      , scope = observer[0]
      , selector = observer[1]
      , eventName = observer[2]
      , commandName = observer[3];

    if (scope == "context") root = $(context);
    
    $(root).delegate(selector, eventName, function(evt) {
      self.onBind();
      var requestObj = new mojo.Request($(this).data() || {}, this, evt, self);

      if (typeof self.before != 'undefined' && typeof self.before[commandName] != 'undefined') {
        self.before[commandName].call(self, requestObj);
        self.onIntercept('Before');
      }
      
      if (!self.methods[commandName] || 'undefined' == typeof self.methods[commandName]) {
        throw new Error("Command does not exist within Controller");
      }
      try {
        self.methods[commandName].call(mojo.controllers[controllerName], requestObj);
      } catch(err) {
        throw err;
      }
      
      if (typeof self.after != 'undefined' && typeof self.after[commandName] != 'undefined') {
        self.after[commandName].call(self, requestObj);
        self.onIntercept('After');
      }
    });
  });
  self.onInit();
};

/* 
 * @member  Controller
 * @return  {DOM} Context Element
 */
Controller.prototype.getContextElement = function() {
  if (!this.contextElement) return null;
  return this.contextElement;
};

/* 
 * Provides the capability to set params on controllers: (key, value) or get (key)
 * @member Controller
 */
 
Controller.prototype.param = function(key, value) {
  if ('undefined' == typeof this.params) this.params = {};
  if (arguments.length > 1) {
    this.params[key] = value;
    this.onParamChange();
    return this;
  } else {
    return this.params[key];
  }
};
  
  mojo.Controller = Controller;
  
});
/*
 * Application Class
 *
 * Class representation of your application where you're provided
 * with the capability to inject plugins, as well as, handle
 * dependencies. Also maps all controllers to DOM elements.
 * 
 * @author Jaime Bueza
 */
mojo.define('mojo.Application', function() {

"use strict";

var $ = jQuery;

function Application() {
  if (!this.options) this.options = {};
  
  var self = this, localOptions = self.options;
    localOptions['locale'] = 'en_CA';
    localOptions['plugins'] = [];
    localOptions['pluginSrc'] = 'js/lib/plugins/';
    localOptions['pluginsAsync'] = true;
    localOptions['environment'] = 'dev';
    localOptions['logging'] = false;
    localOptions['selector'] = jQuery || (function() { throw new Error('Unable to find jQuery'); }) ();
    self.siteMap = [];
};
/* 
 * Triggered when application is fully bootstrapped
 */
Application.prototype.onComplete = function() {};

/* 
 * Provides the capability to set/get properties of the application, such as,
 * logging, plugins, mode (dev/prod)
 * 
 * @param key { String }
 * @param value { Object }
 *
 * Additionally, you can get a property from the application by specifying only the key
 * app.configure('logging') 
 *
 * @returns application instance { Object }
 */
Application.prototype.configure = function configure(key, value) {
  
  if ( !arguments.length ) throw new Error("passing no parameters into configure() is invalid");
  if ( arguments.length > 2 ) throw new Error("passing too many parameters into configure() is invalid");
  
  if (arguments.length > 1) {
    this.options[key] = value;
    if (this.options.environment == 'dev' && ('undefined' != typeof this.options.logging && this.options.logging)) try { console.info("Configure: ", key, " -> ", value); } catch(err) {}
    return this;
  } else {
    return this.options[key];
  }
};
/* 
 * Reads the css selector from a map and executes the callback, which is actually 
 * just a function that returns an array of controllers with parameters
 * 
 * @param selector { String | HTML Element } 
 * @param callback { Function }
 * 
 */
Application.prototype.map = function map(selector, callback) {
  
  if ( 'undefined' == typeof selector || !selector ) throw new Error("'selector' is a required parameter");
  if ( 'undefined' == typeof callback || !callback ) throw new Error("'callback' is a required parameter");
  if ( 'string' != typeof selector ) throw new Error("'selector' needs to be a String");
  if ( $.isArray(callback) && callback.length === 0 ) throw new Error("'callback' is an array and is required to have controllers") 
  
  if ( $.isArray(callback) ) {
    $(callback).each(function(index, controller) {
      if (!controller.hasOwnProperty('controller')) throw new Error("'callback' must contain only Mojo Controller objects");
    });
    
  }
  
  var self = this;
  var elements = $(selector);
  elements.each(function(index, item) {
    self.siteMap.push({ context: item, init: callback });
  });
  
  if ('function' == typeof callback) callback.call(this, self);
  return this;
};

Application.prototype.setupController = function setupController(context, controller, params) {
  if ( 'undefined' == typeof context || !context ) throw new Error("'context' is a required parameter");
  if ( 'undefined' == typeof controller || !controller ) throw new Error("'controller' is a required parameter");
  
  var sizzleContext = $(context);

  var controllerObj = mojo.controllers[controller];
  var abstractController = new mojo.Controller()
    , controllerObj = $.extend(controllerObj, controllerObj.methods)
    , controllerObj = $.extend(controllerObj, abstractController);
  mojo.controllers[controller] = controllerObj;
  
  if ( typeof controllerObj == 'undefined') throw new Error("Undefined Controller @ ", controller);
  controllerObj.initialize(context, controller, params);
  if('undefined' == typeof context.mojoControllers) context.mojoControllers = [];
  context.mojoControllers.push({controller: controllerObj});
  if (typeof controllerObj.after != 'undefined' && controllerObj.after['Start'] != 'undefined') { 
    controllerObj.after['Start'].call(controllerObj, null);
  }

  if ('undefined' != typeof controllerObj.methods['Initialize']) {
    controllerObj.methods['Initialize'].call(controllerObj);
  }
  
};

Application.prototype.disconnectController = function disconnectController(node, controller) {
  if ( 'undefined' == typeof node || !node ) throw new Error("'node' is a required parameter");
  if ( 'undefined' == typeof controller || !controller ) throw new Error("'controller' is a required parameter");
  $(node).unbind().undelegate();
  if ('undefined' != typeof $(node)[0].mojoControllers) delete $(node)[0].mojoControllers;
  return this;
};
Application.prototype.disconnectControllers = function disconnectControllers(callback) {
  var self = this;
  $(this.siteMap).each(function(index, silo) {
    $(silo.context).unbind().undelegate();
    if ('undefined' != typeof $(silo.context)[0].mojoControllers) delete $(silo.context)[0].mojoControllers;
  });
  if ('undefined' != typeof callback && 'function' == typeof callback) callback.apply(this);
};

Application.prototype.connectControllers = function connectControllers() {
  var self = this
    , controllers2load = [];
    
  $(self.siteMap).each(function(index, mapping) {
    var silos;
    if ('function' == typeof mapping.init ) { 
      silos = mapping.init.call(this);
    } else {
      silos = mapping.init;
    }
    
    $(silos).each(function(i, silo) {
      if (!mojo.controllers.hasOwnProperty(silo.controller)) { 
        controllers2load.push(silo.controller);
      } else {
        mojo._loaded[silo.controller] = silo.controller;
      }
    });
  });
  
  mojo.require($.unique(controllers2load), function() {
    $(self.siteMap).each(function(index, mapping) {
    
      if (self.options.environment == 'dev' && self.options.logging) {
        try { console.log("Mapping [" + index + "]: ", mapping.context); } catch (err) {}
      } 
      var silos = ('function' == typeof mapping.init ) ? mapping.init.call(this) : mapping.init;

      $(silos).each(function(i, silo) {
        self.setupController(mapping.context, silo.controller, silo.params);
      });
      
      mojo.Messaging.publish("/app/start");
    
    });      
  });
};

Application.prototype.getPlugins = function(callback) {
   var self = this, path = self.options.pluginSrc;
   
   if (!self.options.pluginsAsync) $.ajaxSetup({async: false});
   $(self.options.plugins).each(function(index, plugin) {
     mojo.fetch(path + plugin + ".js");
   });
   if (!self.options.pluginsAsync) $.ajaxSetup({async: true});
   if ('undefined' != typeof callback && 'function' == typeof callback) callback.call(self);
};

/* 
 * Starts the application instance by fetching all plugins, fetching all controllers,
 * mapping the controllers to dom nodes, as well as, emits onComplete
 */
Application.prototype.start = function start() {
  var self = this;
  $(document).ready(function() {
    self.disconnectControllers(function() {
      if (self.options.plugins.length) { 
        self.getPlugins(function() {
          mojo.Messaging.publish("/app/plugins/loaded");
          self.connectControllers();
        });
      } else {
        self.connectControllers();
      }
      self.onComplete();
    });
  });
  
};

Application.prototype.remap = function remap() {
  var self = this;
  self.disconnectControllers(function() {
    self.connectControllers();
    self.onComplete();
  });
};

  mojo.Application = Application;
});
/* 
* Service
* @author Jaime Bueza
* Represents a service call
*/
mojo.define('mojo.Service', function Service($) {
  
  function Service(name, uri, options) {
    if (typeof options == 'undefined') options = {};

    var defaults = {
      method: options.method || function () {
        var type = "get";
        if (name.match(/^get/i)) {
          type = "get";
        } else if (name.match(/^add|del|update/i)) {
          type = "post";
        }
        return type;

      } (),
      jsonp: false,
      wrapped: false,
      template: false,
      contentType: "application/json; charset=utf-8"
    };
    this.name = name;
    this.uri = uri;
    this.options = $.extend({}, defaults, options);
  };

  Service.prototype.invoke = function (params, callback, scope) {

    var self = this;

    var options = self.getOptions() || {},
                  method = options.method,
                  uri = self.getURI(),
                  responseType = options.responseType || 'JSON';

    if ('undefined' == typeof callback || !callback) throw new Error("'callback' is a required parameter");

    if (options.template) {
      uri = self.parse(uri, params);
      if (method == 'get') params = null;
    }

    $.ajaxSetup({
      dataTypeString: responseType,
      dataType: options.jsonp ? 'jsonp' : responseType,
      type: method,
      async: options.async || true,
      cache: options.cache || false,
      contentType: options.contentType || "application/json; charset=utf-8"
    });
    
    var data;
    if (method == 'post' && options.contentType.match(/application\/json/gi)) {
      data = JSON.stringify(params);
    } else {
      data = params;
    }

    $.ajax({
      url: uri,
      data: data,
      headers: {
        "RequestId": mojo.guid()
      }
    }).success(function (data) {
      if (responseType == 'JSON' && this.contentType.match(/javascript/g)) {
        data = $.parseJSON(data);
      }
      
      if (options.wrapped) data = self.unwrap(data);

      if ('undefined' != typeof callback) {
        var args = [ null, data, arguments[1], arguments[2] ];

        mojo.Messaging.publish("mojo.Service." + self.getName(), { response: data, service: self });
        
        if (typeof callback == 'function') {
          callback.apply(scope, args);
        } else {
          //string
          scope[callback].apply(scope, args);
        }
      }
    }).error(function () {
      if ('undefined' != typeof callback) {
        callback.apply(scope || this, arguments);
      }
    });
  };

  /* 
   *  Returns the current service's name
   */
  Service.prototype.getName = function () {
    return this.name;
  };

 /* 
  * Returns the current service's URI
  */
  Service.prototype.getURI = function () {
    return this.uri;
  };
  
  /*
   * Returns the optional parameters of the current service
   */ 
  Service.prototype.getOptions = function () {
    return this.options;
  };

  /* 
   * Unwraps the ____Result from a WebServiceResult 
   * and returns the object that the ___Result wraps.
   * @param data A web service result from typical WCF service
   */
  Service.prototype.unwrap = function (data) {
    var self = this;
    var unwrapped = {};
    for (var prop in data) {
      if (typeof prop === 'string' && prop.substr(prop.length - 6, prop.length) == 'Result') {
        data = self.convert(data[prop]);
        break;
      }
    }
    return data;
  };
  /*
   *  Works in conjunction with the unwrap method
   * @param o {Object} 
   */
  Service.prototype.convert = function (o) {
    var newResult = {};
    for (var rootProp in o) {
      newResult[rootProp] = o[rootProp];
    }
    return newResult;
  };

  /*
  * Sets or Gets an option from a particular Service
  */
  Service.prototype.option = function () {
    if (!arguments.length) return false;
    if (arguments.length > 2) return false;
    if ('string' != typeof arguments[0]) return false;

    if (arguments.length == 2) {
      this.options[arguments[0]] = arguments[1];
      return this;
    } else if (arguments.length == 1) {
      return this.options[arguments[0]];
    }
  };

  /*
  * Returns an HTML fragment from {} templating
  * @param context {String} Accepts any length string with mustaches ({myKey})
  * @param params {Object} A JSON object that is ran against the content string that has mustaches
  */
  Service.prototype.parse = function (content, params) {
    if (arguments.length != 2) return false;
    if ('string' != typeof content) return false;
    if ('object' != typeof params) return false;
    $.each(params, function (key, value) {
      content = content.split("{" + key + "}").join(value);
    });
    return content;
  };

  mojo.Service = Service;
});/*
 * Provides a singleton that we can access to fetch services for invocation
   -> http://java.sun.com/blueprints/corej2eepatterns/Patterns/ServiceLocator.html
 * @class         ServiceLocator
 * @author        Jaime Bueza
 */
mojo.define('mojo.ServiceLocator', function ServiceLocator($) {

  "use strict"; 

  var ServiceLocator = {
    services: {},
    /* 
     * Adds a particular service to the Service Locator
     * @param service {Service Object} An instance of a mojo Service class
     */
    addService: function(service) {
      if (!service) return false;
      this.services[service.name] = service;
      return this;
    },
    /* 
     * Gets a particular service to the Service Locator
     * @param service {Service Object} An instance of a mojo Service class
     */
    getService: function(name) {
      return this.services[name];
    },
    /* 
     * Removes a particular Service from the Service Locator
     * @param name {String} Specific service to be removed
     */
    removeService: function(name) {
      delete this.services[name];
    },
    /* 
     * Destroys all service references in the Service Locator
     */
    removeServices: function() { 
      this.services = {}; 
      return true;
    },
    /*
     * Returns all services in the Service Locator
     */
    getServices: function() {
      if (typeof this.services === undefined) return false;
      return this.services;
    }
  };

  mojo.ServiceLocator = ServiceLocator;
});