using AAP.Application.DTOs;
using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Application.UseCases.Interfaces
{
    public interface ILoginUserUseCase
    {
        Task<User?> Execute(LoginRequest loginReq);
    }
}
