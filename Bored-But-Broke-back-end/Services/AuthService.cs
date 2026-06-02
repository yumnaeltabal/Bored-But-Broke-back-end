using Bored_But_Broke_back_end.Exceptions;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
using Bored_But_Broke_back_end.Models.Requests;
using Bored_But_Broke_back_end.Models.Responses;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bored_But_Broke_back_end.Services
{
    public interface IAuthService
    {
        Task RegisterUserAsync(RegisterUserRequest request);
        Task LoginUserAsync(LoginUserRequest request);
        Task LogoutUserAsync();
        Task<UserInfoResponse?> GetCurrentUserAsync(HttpContext context);
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public async Task LoginUserAsync(LoginUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null) throw new LoginUnsuccessfulException();

            var result = await _signInManager.PasswordSignInAsync(
                user, request.Password, 
                isPersistent: true, 
                lockoutOnFailure: true);

            if (result.IsLockedOut) throw new UserLockedOutException();

            if (!result.Succeeded) throw new LoginUnsuccessfulException();
        }
        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<UserInfoResponse?> GetCurrentUserAsync(HttpContext context)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (user is null) return null;

            return new UserInfoResponse
            {
                Email = user.Email,
                FirstName = user.FirstName
            };
        }
    }
}
