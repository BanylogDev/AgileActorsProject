using AAP.Application.DTOs;
using AAP.Application.Interfaces;
using AAP.Application.UseCases;
using AAP.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class LoginUserUseCaseTests
{
    [Fact]
    public async Task Execute_ReturnsNull_WhenEmailAndPasswordMissing()
    {
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtService>();
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<LoginUserUseCase>>();

        hasher.Setup(x => x.HashPassword(It.IsAny<string>()))
              .Returns("hashed");

        jwt.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IConfiguration>()))
           .Returns("testToken");

        jwt.Setup(x => x.GenerateRefreshToken())
           .Returns("testRefresh");

        var useCase = new LoginUserUseCase(hasher.Object, jwt.Object, config.Object, logger.Object);

        var result = await useCase.Execute(new LoginRequest
        {
            Email = "",
            Password = ""
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Execute_ReturnsNull_WhenWrongEmailAndPAssword()
    {
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtService>();
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<LoginUserUseCase>>();

        hasher.Setup(x => x.HashPassword(It.IsAny<string>()))
              .Returns("hashed");

        jwt.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IConfiguration>()))
           .Returns("testToken");

        jwt.Setup(x => x.GenerateRefreshToken())
           .Returns("testRefresh");

        var useCase = new LoginUserUseCase(hasher.Object, jwt.Object, config.Object, logger.Object);

        var result = await useCase.Execute(new LoginRequest
        {
            Email = "wrong",
            Password = "wrong"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Execute_ReturnsUser_WhenCorrectEmailAndPassword()
    {
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtService>();
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<LoginUserUseCase>>();

        hasher.Setup(x => x.HashPassword(It.IsAny<string>()))
              .Returns("hashed");

        jwt.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IConfiguration>()))
           .Returns("testToken");

        jwt.Setup(x => x.GenerateRefreshToken())
           .Returns("testRefresh");

        var useCase = new LoginUserUseCase(hasher.Object, jwt.Object, config.Object, logger.Object);

        var result = await useCase.Execute(new LoginRequest
        {
            Email = "aap@gmail.com",
            Password = "1234"
        });

        Assert.NotNull(result);
        Assert.Equal("aap@gmail.com", result.Email);
        Assert.Equal("testToken", result.AccessToken);
        Assert.Equal("testRefresh", result.RefreshToken);
    }
}
