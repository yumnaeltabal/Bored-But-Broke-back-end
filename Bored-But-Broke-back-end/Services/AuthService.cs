using Bored_But_Broke_back_end.Exceptions;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
using Bored_But_Broke_back_end.Models.Requests;
using Microsoft.AspNetCore.Identity;

namespace Bored_But_Broke_back_end.Services
{
    public interface IAuthService
    {
        Task RegisterUserAsync(RegisterUserRequest request);
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task RegisterUserAsync(RegisterUserRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null) throw new EmailAlreadyInUseException(request.Email);

            var firstName = string.Empty;
            var lastName = string.Empty;
            if (request.FullName.Contains(' '))
            {
                var name = request.FullName.Split(' ', 2);
                firstName = name[0];
                lastName = name[1];
            }

            var user = new ApplicationUser
            { 
                UserName = request.Email,
                FullName = request.FullName,
                FirstName = firstName,
                LastName = lastName,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new RegistrationFailedException(
                    string.Join(" | ", result.Errors.Select(x => x.Description))
                );
            }
        }
    }
}
