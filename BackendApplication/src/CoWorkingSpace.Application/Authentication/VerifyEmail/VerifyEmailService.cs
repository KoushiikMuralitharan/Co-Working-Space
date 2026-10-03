using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Authentication.VerifyEmail
{
    public class VerifyEmailService
    {
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly IEmailVerificationTokenRepository _verificationTokenRepository;
        private readonly IUserRepository _userRepository;

        public VerifyEmailService(
            IVerificationTokenService verificationTokenService,
            IEmailVerificationTokenRepository verificationTokenRepository,
            IUserRepository userRepository)
        {
            _verificationTokenService = verificationTokenService;
            _verificationTokenRepository = verificationTokenRepository;
            _userRepository = userRepository;
        }


        public async Task VerifyEmailAsync(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException(
                    "Verification token is required.");
            }

            var tokenHash = _verificationTokenService.HashToken(token);

            var userId = await _verificationTokenRepository.GetUserIdByValidTokenAsync(tokenHash);

            if(userId is null)
            {
                throw new BadRequestException("The verification link is invalid or has expired.");
            }

            await _userRepository.MarkEmailAsVerifiedAsync(
           userId.Value);

            await _verificationTokenRepository
                .MarkVerificationTokenAsUsedAsync(tokenHash);
        }
    }
}
