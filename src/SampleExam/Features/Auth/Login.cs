using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SampleExam.Infrastructure;
using System.Net;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Security;
using System;
using Microsoft.EntityFrameworkCore;

namespace SampleExam.Features.Auth
{
    public class Login
    {
        public class UserData
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class Request : IRequest<LoginUserDTOEnvelope>
        {
            public UserData User { get; set; }
        }
        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Email)
                .NotNull()
                .WithErrorCode("LoginUserEmailNotNull")
                .NotEmpty()
                .WithErrorCode("LoginUserEmailNotEmpty")
                .EmailAddress()
                .WithErrorCode("LoginUserEmailEmailAddress");

                RuleFor(x => x.Password)
                .NotNull()
                .WithErrorCode("LoginUserPasswordNotNull")
                .NotEmpty()
                .WithErrorCode("LoginUserPasswordNotEmpty");

            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Request, LoginUserDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private IApiJwtTokenGenerator tokenGenrator;
            private IApiTokenRefreshTokenGenrator refreshTokenGenerator;
            private IPasswordHasher<Domain.User> hasher;

            public Handler(
                IMapper mapper,
                IPasswordHasher<Domain.User> hasher,
                IApiJwtTokenGenerator tokenGenrator,
                IApiTokenRefreshTokenGenrator refreshTokenGenerator,
                SampleExamContext context)
            {
                this.mapper = mapper;
                this.hasher = hasher;
                this.context = context;
                this.tokenGenrator = tokenGenrator;
                this.refreshTokenGenerator = refreshTokenGenerator;
            }

            //TODO add middleware to check if user is deleted for authorized action.
            public async Task<LoginUserDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var userData = request.User;
                var user = context.Users.Include(e => e.RefreshTokens).Where(u => u.Email == userData.Email).FirstOrDefault();
                var failException = new RestException(HttpStatusCode.Unauthorized,
                    "Invalid email / password.",
                    "Email or password is invalid");

                if (user == null)
                {
                    throw failException;
                }

                var result = hasher.VerifyHashedPassword(user, user.Password, userData.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    throw failException;
                }

                var userLoginDto = mapper.Map<Domain.User, LoginUserDTO>(user);
                userLoginDto.Token = await this.tokenGenrator.GenerateToken(userLoginDto.Id, userLoginDto.Email);
                var refreshToken = this.refreshTokenGenerator.GenerateToken();
                user.RefreshTokens.Add(new Domain.RefreshToken() { Token = refreshToken, CreatedAt = DateTime.UtcNow });
                await context.SaveChangesAsync(cancellationToken);
                userLoginDto.RefresToken = refreshToken;
                return new LoginUserDTOEnvelope(userLoginDto);
            }
        }

    }
}