describe("Application Menu Spec", function () {
  var controller = mojo.controllers["abc.controller.Application.ApplicationMenuController"];

  describe("Context", function () {
    it("should be mapped to a DOM element", function () {
      expect(controller.contextElement.className).toBe("applications-menu-list component");
    });
    it("should have the correct namespace", function () {
      expect(controller.controllerClass).toBe("abc.controller.Application.ApplicationMenuController");
    });
  });

  describe("Methods", function () {

    describe("Logout", function () {
      it("should be defined", function () {
        expect(controller.methods.Logout).toBeDefined();
      });
    });

    describe("Render", function () {
      it("should be defined", function () {
        expect(controller.methods.Render).toBeDefined();
      });
    });

    describe("Refresh", function () {
      it("should be defined", function () {
        expect(controller.methods.Refresh).toBeDefined();
      });
    });

    describe("SetDefaultApplication", function () {
      it("should be defined", function () {
        expect(controller.methods.SetDefaultApplication).toBeDefined();
      });
    });

  });
  describe("Events", function () {
    it("should have 3 binding in its Event Map", function () {
      expect(controller.events.length).toBe(3);
    });

    describe("SetDefaultApplication", function () {
      it("should have context", function () {
        expect(controller.events[0][0]).toBe("context");
      });
      it("should have a binding to an element", function () {
        expect(controller.events[0][1]).toBe("li a");
      });
      it("should have a bind to click", function () {
        expect(controller.events[0][2]).toBe("click");
      });

      it("should invoke a command", function () {
        expect(controller.events[0][3]).toBe("SetDefaultApplication");
      });
    });

    describe("Refresh", function () {
      it("should have context", function () {
        expect(controller.events[1][0]).toBe("context");
      });
      it("should have a binding to an element", function () {
        expect(controller.events[1][1]).toBe(".btn-refresh span");
      });
      it("should have a bind to click", function () {
        expect(controller.events[1][2]).toBe("click");
      });

      it("should invoke a command", function () {
        expect(controller.events[1][3]).toBe("Refresh");
      });
    });

    describe("Logout", function () {
      it("should have context", function () {
        expect(controller.events[2][0]).toBe("dom");
      });
      it("should have a binding to an element", function () {
        expect(controller.events[2][1]).toBe("li.btn-logout");
      });
      it("should have a bind to click", function () {
        expect(controller.events[2][2]).toBe("click");
      });

      it("should invoke a command", function () {
        expect(controller.events[2][3]).toBe("Logout");
      });
    });

  });

});