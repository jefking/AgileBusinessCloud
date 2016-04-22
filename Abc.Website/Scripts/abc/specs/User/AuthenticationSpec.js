describe("User Authentication Spec", function () {

  var callback = window.ShowSigninPage;
  
  it("should be defined", function () {
    expect(callback).toBeDefined();
  });

  describe("Error Cases", function () {
    it("should return false if payload from ACS is empty", function () {
      expect(callback([])).toBeFalsy();
    });
    it("should return false if the payload is not a hash (Iteratable)", function () {
      expect(callback(true)).toBeFalsy();
    });
  });
});