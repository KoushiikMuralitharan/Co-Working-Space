using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Infrastructure.Authentication
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(tokenBytes);
        }

        public string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
