namespace mvcApplication.Models
{
    public class AppUser
    {
        public Guid AppUserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName()
        {
            return FirstName + " " + LastName;  
        }
    }
}
