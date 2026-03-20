using AAP.Application.DTOs;
using AAP.Application.Interfaces;
using AAP.Application.UseCases.Interfaces;
using AAP.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AAP.Application.UseCases
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly ILogger<LoginUserUseCase> _logger;

        public LoginUserUseCase(
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IConfiguration config,
            ILogger<LoginUserUseCase> logger)
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
        }

        public async Task<User?> Execute(LoginRequest loginReq)
        {
            _logger.LogInformation("Login started with email, {Email}", loginReq.Email);

            if (string.IsNullOrEmpty(loginReq.Email) || string.IsNullOrEmpty(loginReq.Password))
            {
                _logger.LogWarning("Login failed, missing email or password");
                return null;
            }

            if (loginReq.Email != "aap@gmail.com" || loginReq.Password != "1234")
            {
                _logger.LogWarning("Login failed, invalid {Email} or {Password}", loginReq.Email, loginReq.Password);
                return null;
            }

            var testUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "AAP User",
                Email = loginReq.Email,
                PasswordHash = _passwordHasher.HashPassword(loginReq.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            var token = _jwtService.GenerateAccessToken(testUser, _config);
            var refreshToken = _jwtService.GenerateRefreshToken();

            testUser.RefreshToken = refreshToken;
            testUser.AccessToken = token;
            testUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            _logger.LogInformation("User {Email} logged in successffully", loginReq.Email);

            return testUser;
        }
    }
}
