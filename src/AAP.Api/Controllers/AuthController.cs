using AAP.Application.DTOs;
using AAP.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AAP.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUserUseCase _loginUserUseCase;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, ILoginUserUseCase loginUserUseCase)
        {
            _logger = logger;
            _loginUserUseCase = loginUserUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginReq)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _loginUserUseCase.Execute(loginReq);
            if (user == null) return Unauthorized("Invalid username or password");
                
            return Ok(new { message = "Login successful", user.Id, user.Name, user.Email, user.AccessToken, user.RefreshToken });
        }
    }
}
