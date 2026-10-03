using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IEmailVerificationTokenRepository
    {
        Task CreateVerificationTokenAsync(Guid userId, string tokenHash, DateTimeOffset expiresAt);

        Task<Guid?> GetUserIdByValidTokenAsync(string tokenHash); 

        Task MarkVerificationTokenAsUsedAsync(string tokenHash);
    }
}
