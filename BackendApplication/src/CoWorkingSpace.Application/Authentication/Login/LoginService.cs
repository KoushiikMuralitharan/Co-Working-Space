using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Authentication.Login
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResult> LoginAsync(
        LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new BadRequestException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadRequestException(
                    "Password is required.");
            }

            var email = request.Email
                .Trim()
                .ToLowerInvariant();

            var user =
                await _userRepository.GetUserByEmailAsync(
                    email);

            if (user is null)
            {
                throw new BadRequestException(
                    "Invalid email or password.");
            }

            if (user.Status != "ACTIVE")
            {
                throw new BadRequestException(
                    "Your account is not active.");
            }

            if (user.EmailVerifiedAt is null)
            {
                throw new BadRequestException(
                    "Please verify your email before logging in.");
            }

            var passwordValid =
                _passwordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                throw new BadRequestException(
                    "Invalid email or password.");
            }

            var accessToken =
                _jwtTokenService.GenerateAccessToken(
                    user.UserId,
                    user.Email,
                    user.UserRole);

            return new LoginResult
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole,
                AccessToken = accessToken
            };
        }
    }
}
