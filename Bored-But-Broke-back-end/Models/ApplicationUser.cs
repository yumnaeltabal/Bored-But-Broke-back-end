using Microsoft.AspNetCore.Identity;

namespace Bored_But_Broke_back_end.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        //public DbSet<Place> BookmarkedPlaces { get; set; }
    }
}
