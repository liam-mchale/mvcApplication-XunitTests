using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mvcApplication.Data;
using mvcApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace mvcApplication.Test
{

    // For context cleanup, add the IDisposable interface to your test class, and put the cleanup code in the Dispose() method.
    public class AuthorizedUserHTTPClientFixture : IDisposable
    {
        public HttpClient Client;

        public SqlConnection Db { get; private set; }

        public WebApplicationFactory<Program> Factory { get; private set; }

        public AuthorizedUserHTTPClientFixture()
        {
            Factory = new WebApplicationFactory<Program>();

            Db = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=xxxxxxxx.Test;Integrated Security=True;Connect Timeout=15;Encrypt=False;Packet Size=4096");

            Client = Factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", options => { });

                    services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(Db)
                            );

                }).UseEnvironment("Development");

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme: "TestScheme");
        }


        public void Dispose()
        {
        }
    }

    // Handling Authentication with an Identity Provider through testing is a complicated procedure

    // Create a 'Test Scheme' to mimic an authenticated user. 

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                            ILoggerFactory logger,
                            UrlEncoder encoder,
                            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, "Test Project - Test User"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("uid", "E21B64F8-8888-41EB-9999-3CDC9AA457F3"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
