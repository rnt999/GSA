using GSA.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            Program.InitHost(_server.Host);
        }

        [DataTestMethod]
        [DataRow("/api/monthly-capital?strategies=Strategy1,Strategy2")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync(url);
            var responseStrong = await response.Content.ReadAsStringAsync();


            //        // Assert
            //        response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
