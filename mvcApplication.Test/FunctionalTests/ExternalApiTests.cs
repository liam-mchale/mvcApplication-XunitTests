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
    public class ExternalApiTests 
    {
        ITestOutputHelper output { get; set; }

        public ExternalApiTests(ITestOutputHelper testOutputHelper) {

            this.output = testOutputHelper;
        }


        [Fact]
        public async Task ExternalApiCallTest()
        {
            // Arrange
            string _address = "http://api.worldbank.org/countries?format=json";

            // The HTTP Client being used to gather data should be configured to match your real site.
            // IE Configure the Request Headers

            var client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(_address).Result;
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

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
