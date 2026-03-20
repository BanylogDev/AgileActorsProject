using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace AAP.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IConfiguration config);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration config);
    }
}
