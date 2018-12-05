using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GSA.IntegrationTests
{
    [TestClass]
    public class MonthlyCapitalsApiTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MonthlyCapitalsApiTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        [DataTestMethod]
        [DataRow("/api/monthly-capital")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _server.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
