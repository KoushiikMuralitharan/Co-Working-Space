using CoWorkingSpace.Application.Authentication.Login;
using CoWorkingSpace.Application.Authentication.Register;
using CoWorkingSpace.Application.Authentication.VerifyEmail;
using Microsoft.AspNetCore.Mvc;

namespace CoWorkingSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly RegisterService _registerService;
        private readonly VerifyEmailService _verifyEmailService;
        private readonly LoginService _loginService;

        public AuthController(RegisterService registerService, VerifyEmailService verifyEmailService, LoginService loginService)
        {
            _registerService = registerService;
            _verifyEmailService = verifyEmailService;
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _registerService.RegisterAsync(request);

            return Ok(result);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _verifyEmailService.VerifyEmailAsync(token);

            return Ok(new
            {
                message = "Email verified successfully." 
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _loginService.LoginAsync(request);

            return Ok(result);
        }

    }
}
