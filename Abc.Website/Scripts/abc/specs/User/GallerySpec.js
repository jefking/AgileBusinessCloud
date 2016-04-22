describe("User Gallery Spec", function () {

  var controller = mojo.controllers["abc.controller.User.UserGalleryController"];

  describe("Context", function () {
    it("should have context", function () {
      expect(controller.contextElement.className).toBe("component component-user-gallery");
    });
    it("should have the correct namespace", function () {
      expect(controller.controllerClass).toBe("abc.controller.User.UserGalleryController");
    });
  });
  describe("Events", function () {
    it("should have no bindings", function () {
      expect(controller.events.length).toBe(0);
    });
  });
  describe("Methods", function () {
    describe("Initialize", function () {
      it("should be defined", function () {
        expect(controller.methods.Initialize).toBeDefined();
      });
    });
    describe("Gravatar", function () {
      it("should be defined", function () {
        expect(controller.methods.Gravatar).toBeDefined();
      });
    });
  });
});