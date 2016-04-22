describe("abc.ServiceLocator", function () {
  var locator = ServiceLocator
    , services = locator.getServices();

  describe("Getting user information - Service Spec", function () {
    var service = services['getUsers'];
    it("should have a name", function () {
      expect(service.name).toEqual("getUsers");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/User/Users");
    });
    it("should have be a GET", function () {
      expect(service.options.method).toEqual("get");
    });
  });

  describe("Saving user role - Service Spec", function () {
    var service = services['addUserRole'];
    it("should have a name", function () {
      expect(service.name).toEqual("addUserRole");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/ManagementAPI/SaveUserRole");
    });
    it("should have be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Adding a user to a particular application - Service Spec", function () {
    var service = services['addUserToApplication'];
    it("should have a name", function () {
      expect(service.name).toEqual("addUserToApplication");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Application/AddUser");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Adding an application to the system - Service Spec", function () {
    var service = services['addApplication'];
    it("should have a name", function () {
      expect(service.name).toEqual("addApplication");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Apps/Details");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Setting a user preference - Service Spec", function () {
    var service = services['addUserPreference'];
    it("should have a name", function () {
      expect(service.name).toEqual("addUserPreference");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/User/Save");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Retrieving a list of applications - Service Spec", function () {
    var service = services['getUserApplications'];
    it("should have a name", function () {
      expect(service.name).toEqual("getUserApplications");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Application/GetApplications");
    });
    it("should be a GET", function () {
      expect(service.options.method).toEqual("get");
    });
  });

  describe("Setting application configurations - Service Spec", function () {
    var service = services['addApplicationConfiguration'];
    it("should have a name", function () {
      expect(service.name).toEqual("addApplicationConfiguration");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Configuration/Save");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Retrieve application configurations - Service Spec", function () {
    var service = services['getApplicationConfiguration'];
    it("should have a name", function () {
      expect(service.name).toEqual("getApplicationConfiguration");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Configuration/Get");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("post");
    });
  });

  describe("Retrieve application performance - Service Spec", function () {
    var service = services['getApplicationPerformance'];
    it("should have a name", function () {
      expect(service.name).toEqual("getApplicationPerformance");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Log/Performance");
    });
    it("should be a GET", function () {
      expect(service.options.method).toEqual("get");
    });
  });

  describe("Retrieve application messsages - Service Spec", function () {
    var service = services['getApplicationMessages'];
    it("should have a name", function () {
      expect(service.name).toEqual("getApplicationMessages");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Log/Message");
    });
    it("should be a GET", function () {
      expect(service.options.method).toEqual("get");
    });
  });

  describe("Retrieve application errors - Service Spec", function () {
    var service = services['getApplicationErrors'];
    it("should have a name", function () {
      expect(service.name).toEqual("getApplicationErrors");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Log/Error");
    });
    it("should be a GET", function () {
      expect(service.options.method).toEqual("get");
    });
  });
  describe("Retrieving application data usage - Service Spec", function () {
    var service = services['getApplicationUsage'];
    it("should have a name", function () {
      expect(service.name).toEqual("getApplicationUsage");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/Analytics/ApplicationDataUsage");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("get");
    });
  });

  describe("Retrieving company tweets - Service Spec", function () {
    var service = services['getTweets'];
    it("should have a name", function () {
      expect(service.name).toEqual("getTweets");
    });
    it("should have a url", function () {
      expect(service.uri).toEqual("/External/CompanyTweets");
    });
    it("should be a POST", function () {
      expect(service.options.method).toEqual("get");
    });
  });

});