using Bored_But_Broke_back_end.Models.Requests;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bored_But_Broke_back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest request)
        {
            await _authService.RegisterUserAsync(request);

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserRequest request)
        {
            await _authService.LoginUserAsync(request);

            return Ok("Login successful");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutUserAsync()
        {
            await _authService.LogoutUserAsync();

            return Ok("Logout successful");
        }

    }
}