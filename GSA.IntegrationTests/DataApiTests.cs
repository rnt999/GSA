using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            _client = _server.CreateClient();
        }

        [DataTestMethod]
        [DataRow("/api/monthly-capital")]
        public async Task Should_GetSuccessResponseForMonthlyCapitalWhenDontHaveParameters(string url)
        {
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }

        [DataTestMethod]
        [DataRow("/api/cumulative-pnl/?startDate=2010-01-01&region=EU")]
        public async Task Should_GetSuccessResponseForCumulativePnlWhenDontHaveParameters(string url)
        {
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }

        [DataTestMethod]
        [DataRow("/api/compound-daily-returns/Strategy15/")]
        public async Task Should_GetSuccessResponseForCompoundDailyReturns(string url)
        {
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }


        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
