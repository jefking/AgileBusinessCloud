describe("abc.helper.AggregateGraph", function () {


  describe("Constructor Specs", function () {
    it("should seed itself in the Window context as abc.AggregateGraph", function () {
      expect(window.abc.AggregateGraph).toBeDefined();
    });

    it("should not throw an error when _ library does not exist", function () {
      var tmp = window._;
      delete window._;
      expect(function () {
        new abc.AggregateGraph("Method", {}, {});
      }).toThrow("_ library missing.");
      window._ = tmp;
    });
    it("should not throw an error when jQuery plot library does not exist", function () {
      var tmp = jQuery.plot;
      delete jQuery.plot;
      expect(function () {
        new abc.AggregateGraph("Method", {}, {});
      }).toThrow("plot library missing.");
      jQuery.plot = tmp;
    });
    it("should throw an error when passing no property as first argument", function () {
      expect(function () {
        new abc.AggregateGraph(null, {}, {});
      }).toThrow("property parameter is required.");
    });

    it("should throw an error when passing a non-string value as first argument", function () {
      expect(function () {
        new abc.AggregateGraph(true, {}, {});
      }).toThrow("property parameter must be a string.");
    });
    it("should throw an error when passing no view object as second argument", function () {
      expect(function () {
        new abc.AggregateGraph("ServerName", null, {});
      }).toThrow("view parameter is required.");
    });
    it("should throw an error when passing no data object as third argument", function () {
      expect(function () {
        new abc.AggregateGraph("ServerName", {}, null);
      }).toThrow("data parameter is required.");
    });
    it("should throw an error when passing a configuration object that isn't an object", function () {
      expect(function () {
        new abc.AggregateGraph("ServerName", {}, {}, true);
      }).toThrow("configuration object must be an object.");
    });

    it("should set the property value of the instance", function () {
      expect(new abc.AggregateGraph("Method", {}, {}).property).toEqual("Method");
    });

    it("should set the view property of the instance", function () {
      expect(new abc.AggregateGraph("Method", {}, {}).view).toBeDefined();
    });

    it("should set the data property of the instance", function () {
      expect(new abc.AggregateGraph("Method", {}, []).data).toBeDefined();
    });

    it("should return itself upon instantiation", function () {
      var g = new abc.AggregateGraph("Method", {}, []);
      expect(g.property).toEqual(g.property);
    });
  });

  describe("Render Specs", function () {

    it("should throw an error if there is no data", function () {

    });
  });
});