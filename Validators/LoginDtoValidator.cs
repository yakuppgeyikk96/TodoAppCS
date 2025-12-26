using FluentValidation;
using FirstWebApi.DTOs;

namespace FirstWebApi.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
  public LoginDtoValidator()
  {
    RuleFor(x => x.Username)
        .NotEmpty().WithMessage("Username is required")
        .MinimumLength(1).WithMessage("Username cannot be empty");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required")
        .MinimumLength(1).WithMessage("Password cannot be empty");
  }
}
