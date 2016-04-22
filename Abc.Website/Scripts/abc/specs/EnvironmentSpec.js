describe("Agile Business Cloud Environment", function () {
  it("should have a CDN path", function () {
    expect(window.ContentDeliveryNetworkScriptsURL).toBeDefined();
  });
  it("should have a Federation Variable set", function () {
    expect(window.ABC.FederationTLD).toBeDefined();
  });
});