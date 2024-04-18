using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace mvcApplication.Test.FunctionalTests
{
    public class ApiTests : IClassFixture<AuthorizedUserHTTPClientFixture>
    {
        AuthorizedUserHTTPClientFixture AuthorizedUserHTTPClientFixture { get; set; }

        ITestOutputHelper output { get; set; }

        public ApiTests(AuthorizedUserHTTPClientFixture authorizedUserHTTPClientFixture, ITestOutputHelper testOutputHelper) {

            AuthorizedUserHTTPClientFixture = authorizedUserHTTPClientFixture;
            this.output = testOutputHelper;
        }


        [Fact]
        public async Task ExternalApiCallTest()
        {
            // Arrange
            string _address = "http://api.worldbank.org/countries?format=json";
            var client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(_address).Result;
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            //output.WriteLine(result);

            foreach (JArray countryData in JArray.Parse(result).Where(x => x.Type == JTokenType.Array))
            {
                foreach(JObject country in countryData)
                {
                    output.WriteLine(country.ToString());
                    string? countryDataContainsIsoCode = (string?)country["iso2Code"];
                    Assert.NotNull(countryDataContainsIsoCode);
                }
            }
        }
    }
}
