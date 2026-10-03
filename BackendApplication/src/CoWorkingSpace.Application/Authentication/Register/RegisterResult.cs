using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Authentication.Register
{
    public class RegisterResult
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
