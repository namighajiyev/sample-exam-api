using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
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
                RuleFor(x => x.Firstname)
                .NotNull()
                .WithErrorCode("CreateUserFirstnameNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserFirstnameNotEmpty");

                RuleFor(x => x.Lastname)
                .NotNull()
                .WithErrorCode("CreateUserLastnameNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserLastnameNotEmpty");

                RuleFor(x => x.GenderId)
                .NotNull()
                .WithErrorCode("CreateUserGenderIdNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserGenderIdNotEmpty")
                .Must(id => id == SeedData.Genders.Male.Id || id == SeedData.Genders.Female.Id)
                .WithMessage($"User GenderId must be either {SeedData.Genders.Male.Id} ({SeedData.Genders.Male.Name}) or {SeedData.Genders.Female.Id} ({SeedData.Genders.Female.Name})")
                .WithErrorCode("CreateUserGenderIdMustBeMaleOrFemale");

                RuleFor(x => x.Dob)
                .NotNull()
                .WithErrorCode("CreateUserDobNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserDobNotEmpty")
                .LessThan(DateTime.UtcNow.Date)
                .WithMessage($"User DOB must be less than {DateTime.UtcNow.Date}")
                .WithErrorCode("CreateUserDobLessThanUtcNowDate");

                RuleFor(x => x.Email)
                .NotNull()
                .WithErrorCode("CreateUserEmailNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserEmailNotEmpty")
                .EmailAddress()
                .WithErrorCode("CreateUserEmailEmailAddress");

                RuleFor(x => x.Password)
                .NotNull()
                .WithErrorCode("CreateUserPasswordNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserPasswordNotEmpty")
                .MinimumLength(Consts.PASSWORD_MIN_LENGTH)
                .WithErrorCode("CreateUserPasswordMinimumLength")
                .Matches(Consts.ASCII_PRINTABLE_CHARS_REGEX)
                .WithErrorCode("CreateUserPasswordMatchesAsciiPrintableChars")
                .WithMessage("Password must contain only printable ASCII characters")
                .NotMatches(Consts.NOT_BEGINING_OR_ENDING_WITH_ASCII_SPACE_REGEX)
                .WithErrorCode("CreateUserPasswordMatchesNotBeginsOrEndsWithAsciiSpace")
                .WithMessage("Your password cannot start or end with a blank space")
                .StrongPassword()
                .WithErrorCode("CreateUserPasswordStrongPassword");

                RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .WithErrorCode("CreateUserConfirmPasswordNotNull")
                .NotEmpty()
                .WithErrorCode("CreateUserConfirmPasswordNotEmpty")
                .Equal(x => x.Password)
                .WithErrorCode("CreateUserConfirmPasswordEqualPassword")
                .WithMessage("Passwords do not match");


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
            public Handler(IMapper mapper, SampleExamContext context)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<UserDTOEnvelope> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = mapper.Map<UserData, Domain.User>(request.User);

                await this.context.Users.AddAsync(user, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                var userDto = mapper.Map<Domain.User, UserDTO>(user);

                return new UserDTOEnvelope(userDto);
            }
        }
    }
}