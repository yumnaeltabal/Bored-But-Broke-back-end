using System.ComponentModel.DataAnnotations;

namespace Bored_But_Broke_back_end.Models.Requests
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Email must not be empty")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password must not be empty")]
        public required string Password { get; set; }
    }
}
