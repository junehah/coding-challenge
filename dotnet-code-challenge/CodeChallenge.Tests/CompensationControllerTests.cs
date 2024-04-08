using CodeChallenge.Models;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensationById_Returns_OK()
        {
            //Arrange
            var employeeId = "99c9999e-6e34-4630-93fd-9153afb99999";
            double salary = 15000.00;

            var employee = new Employee()
            {
                EmployeeId = employeeId,
                Department = "CompensationDepartment",
                FirstName = "Money",
                LastName = "Compensation",
                Position = "Compensator",
            };

            var comp = new Compensation()
            {
                Id = employeeId,
                Employee = employee,
                Salary = salary,
                EffectiveDate = DateTime.Now
            };

            // Execute
            var requestContent = new JsonSerialization().ToJson(comp);
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.Salary, salary);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var id = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var salary = 150000.00;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{id}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.Salary, salary);
        }
    }
}
