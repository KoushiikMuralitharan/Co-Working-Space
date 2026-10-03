using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Authentication.Register
{
    public class RegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly IEmailVerificationTokenRepository _verificationTokenRepository;
        private readonly IEmailService _emailService;

        public RegisterService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IVerificationTokenService verificationTokenService,
            IEmailVerificationTokenRepository verificationTokenRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _verificationTokenService = verificationTokenService;
            _verificationTokenRepository = verificationTokenRepository;
            _emailService = emailService;
        }
        public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                throw new BadRequestException("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                throw new BadRequestException("Last name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new BadRequestException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadRequestException("Password is required.");
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var emailExists =
            await _userRepository.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new ConflictException("An account with this email already exists.");
            }

            var passwordHash =
                _passwordHasher.HashPassword(request.Password);

            var userId =
                await _userRepository.CreateUserAsync(
                    request.FirstName.Trim(),
                    request.LastName.Trim(),
                    email,
                    passwordHash,
                    request.PhoneNo?.Trim());

            var verificationToken = _verificationTokenService.GenerateToken();

            var tokenHash =  _verificationTokenService.HashToken(verificationToken);

            var expiresAt = DateTimeOffset.UtcNow.AddHours(24);

            await _verificationTokenRepository.CreateVerificationTokenAsync(
                userId,
                tokenHash,
                expiresAt);

            //var verificationLink = $"http://192.168.0.104:5263/api/Auth/verify-email?token={verificationToken}";
            var verificationLink = $"http://192.168.0.104:5263/api/Auth/verify-email?token={Uri.EscapeDataString(verificationToken)}";

            await _emailService.SendEmailAsync(
                    email,
                    "Verify your CoWorkingSpace account",
                    $"""
                    <h2>Welcome to CoWorkingSpace!</h2>

                    <p>Please verify your email address by clicking the link below:</p>

                    <p>
                        <a href="{verificationLink}">
                            Verify Email
                        </a>
                    </p>

                    <p>This link will expire in 24 hours.</p>
                    """);

            return new RegisterResult
            {
                UserId = userId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                UserRole = "CUSTOMER"
            };
        }
    }
}
