using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SampleExam.Common;
using SampleExam.Infrastructure;


namespace SampleExam.Features.User
{
    public class Create
    {
        public class UserData
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }

            public string Middlename { get; set; }

            public int GenderId { get; set; }

            public DateTime Dob { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class Request : IRequest<UserDTOEnvelope>
        {
            public UserData User { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                var errorCodePrefix = nameof(Create);
                RuleFor(x => x.Firstname).UserFirstname<UserData, string>(errorCodePrefix);
                RuleFor(x => x.Middlename).UserMiddlename<UserData, string>(errorCodePrefix);
                RuleFor(x => x.Lastname).UserLastname<UserData, string>(errorCodePrefix);
                RuleFor(x => x.GenderId).UserGenderId<UserData, string>(errorCodePrefix);
                RuleFor(x => x.Dob).UserDob<UserData, string>(errorCodePrefix);
                RuleFor(x => x.Email).UserEmail<UserData, string>(errorCodePrefix);
                RuleFor(x => x.Password).UserPassword<UserData, string>(errorCodePrefix);
                RuleFor(x => x.ConfirmPassword).UserConfirmPassword<UserData, string>(x => x.Password,
                errorCodePrefix);
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }


        public class Handler : IRequestHandler<Request, UserDTOEnvelope>
        {
            private IMapper mapper;
            private SampleExamContext context;
            private IPasswordHasher<Domain.User> hasher;

            public Handler(IMapper mapper, IPasswordHasher<Domain.User> hasher, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
                this.hasher = hasher;
            }
            public async Task<UserDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = mapper.Map<UserData, Domain.User>(request.User);
                user.Password = hasher.HashPassword(user, user.Password);
                var utcNow = DateTime.UtcNow;
                user.CreatedAt = utcNow;
                user.UpdatedAt = utcNow;

                await this.context.Users.AddAsync(user, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                var userDto = mapper.Map<Domain.User, UserDTO>(user);

                return new UserDTOEnvelope(userDto);
            }
        }
    }
}