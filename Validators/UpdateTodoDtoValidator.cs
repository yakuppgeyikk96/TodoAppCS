using FluentValidation;
using FirstWebApi.DTOs;

namespace FirstWebApi.Validators;

public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
{
  public UpdateTodoDtoValidator()
  {
    RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required")
        .MinimumLength(1).WithMessage("Title cannot be empty")
        .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
        .Must(title => !string.IsNullOrWhiteSpace(title))
        .WithMessage("Title cannot be only whitespace");

    RuleFor(x => x.Description)
        .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
        .When(x => !string.IsNullOrEmpty(x.Description));
  }
}
