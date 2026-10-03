using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Authentication.Login;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<Guid> CreateUserAsync(string firstName, string lastName, string email, string passwordHash, string? phoneNo);
        Task MarkEmailAsVerifiedAsync(Guid userId);
        Task<LoginUser?> GetUserByEmailAsync(string email);
    }
}
