using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string email, string userRole);
    }
}
