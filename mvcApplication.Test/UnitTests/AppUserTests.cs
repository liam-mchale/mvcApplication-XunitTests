using mvcApplication.Models;

namespace mvcApplication.Test.UnitTests
{
    // 
    public class AppUserTests
    {
        [Fact]
        public void FullNameTest()
        {
            // Arrange
            
            AppUser appUser = new()
            {
                FirstName = "John",
                LastName = "Doe",
            };

            // Act

            string fullName = appUser.FullName();

            // Assert

            Assert.Equal("John Doe", fullName);
        }
    }
}