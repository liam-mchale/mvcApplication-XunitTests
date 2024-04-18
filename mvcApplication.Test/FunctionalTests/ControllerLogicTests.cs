using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace mvcApplication.Test.FunctionalTests
{
    // Uses the AuthorizedUserHTTPClient Fixture
    public class ControllerLogicTests : IClassFixture<AuthorizedUserHTTPClientFixture>
    {
        private AuthorizedUserHTTPClientFixture AuthorizedUserHTTPClientFixture { get; set; }

        public ControllerLogicTests(AuthorizedUserHTTPClientFixture authorizedUserHTTPClientFixture)
        {
            AuthorizedUserHTTPClientFixture = authorizedUserHTTPClientFixture;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Home/Error")]
        [InlineData("/Home/Admin")]
        public async Task CheckHomeViewsExist(string url)
        {
            // Arrange
            var client = AuthorizedUserHTTPClientFixture.Client;

            // Act
            HttpResponseMessage httpResponseMessage = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }
    }




    public class AuthorizationAccessTests
    {
        private HttpClient UnAuthorizedUserHTTPClientFixture { get; set; }

        public AuthorizationAccessTests()
        {
            var Factory = new WebApplicationFactory<Program>();

            UnAuthorizedUserHTTPClientFixture = Factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => { }).UseEnvironment("Development");

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        }

        [Fact]
        public async Task UnAuthorizedUser_HomePageAccess()
        {
            // Arrange
            string homePageUrl = "/Home/Index";

            // Act
            var noAuthorizationRequiredView = await UnAuthorizedUserHTTPClientFixture.GetAsync(homePageUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, noAuthorizationRequiredView.StatusCode);
        }

        [Fact]
        public async Task UnAuthorizedUser_AuthorizedPageAccess()
        {
            // Arrange
            string homePageUrl = "/Home/Privacy";

            // Act
            var authorizationRequiredView = await UnAuthorizedUserHTTPClientFixture.GetAsync(homePageUrl);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, authorizationRequiredView.StatusCode);
        }
    }
}
