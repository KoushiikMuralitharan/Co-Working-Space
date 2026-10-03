using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string from, string body);
    }
}
