using FluentValidation;

namespace Application.Validators
{
  public static class ValidatorExtension
  {
    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
      var options = ruleBuilder
                        .NotEmpty()
                        .MinimumLength(6).WithMessage("Password must contain at least 6 characters")
                        .Matches("[A-Z]").WithMessage("Password must contain 1 uppercase")
                        .Matches("[a-z]").WithMessage("Password must contain 1 lowercase")
                        .Matches("[0-9]").WithMessage("Password must contain 1 number")
                        .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain non alphanumeric");

      return options;
    }
  }
}