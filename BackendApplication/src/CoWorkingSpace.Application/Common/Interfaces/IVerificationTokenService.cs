using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IVerificationTokenService
    {
        string GenerateToken();
        string HashToken(string token);
    }
}
