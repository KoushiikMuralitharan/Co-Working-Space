using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Authentication.Login
{
    public class LoginUser
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string UserRole { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset? EmailVerifiedAt { get; set; }
    }
}
