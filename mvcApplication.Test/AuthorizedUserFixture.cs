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
    // A class fixture is used when we wish to setup a configuration to be used across multiple test classes
    // When the AuthorizedUserHTTPClientFixture is applied to a test class, the constructor & disposal processes

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



            // Example of Seeding Data upon initialization of of the Fixture

            /*
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer(Db)
                        .Options
                        );

                // Seeding ...

                AppUser? Testing_User = dbContext.AppUsers.Where(x => x.FirstName == "Controllers Test Project - Test User").SingleOrDefault();

                if (Testing_User == null)
                {
                    Testing_User = new AppUser()
                    {
                        FirstName = "Controllers Test Project - Test User",
                        LastName = "TestUser",
                        AppUserId = new Guid("E21B64F8-8888-41EB-9999-3CDC9AA457F3"),
                        Role = UserRole.Staff
                    };

                    dbContext.AppUsers.Add(Testing_User);
                    dbContext.SaveChanges();
                } else
                {
                    var designObjects = dbContext.StructuralDesignObjectModel.Where(x => x.AppUserId == Testing_User.AppUserId)
                        .AsSplitQuery()
                        .ToList();
                    dbContext.StructuralDesignObjectModel.RemoveRange(designObjects);
                    dbContext.SaveChanges();
                }
                _AppUser = Testing_User;

                if (dbContext.Projects.SingleOrDefault(x => x.Name == TestProjectReference.GetTestProject().Name) == null)
                {
                    dbContext.Projects.Add(TestProjectReference.GetTestProject());
                    dbContext.SaveChanges();
                }
            }
            */
        }


        public void Dispose()
        {
            // Remove Seed Data if required
            // Dispose of any other unwanted artifacts of the test process
        }
    }

    // Handling Authentication with an Identity Provider through testing is a complicated procedure

    // Create a 'Test Scheme' to mimic an authenticated user.
    //
    // You can mimic various site Groups or Roles by adjusting the claims to a test handler. 


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
                //new Claim(ClaimTypes.Role, "Admin"),
                new Claim("uid", "E21B64F8-8888-41EB-9999-3CDC9AA457F3"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }


    // Collection Fixtures
    // When you want to create a single test context and share it among tests in several test classes, and have it cleaned up after all the tests in the test classes have finished.

    // IE If you wish to use a test database with values which persist across several test classes
    // 







}
