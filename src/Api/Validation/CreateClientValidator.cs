using Api.Dtos;
using FluentValidation;

namespace Api.Validation;

public class CreateClientValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
            .Matches(@"^(\d{3}-\d{3}-\d{4})?$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone must be in format XXX-XXX-XXXX.");

        RuleFor(x => x.Company)
            .MaximumLength(255).WithMessage("Company must not exceed 255 characters.");

        RuleFor(x => x.AddressLine1)
            .MaximumLength(255).WithMessage("Address line 1 must not exceed 255 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(255).WithMessage("Address line 2 must not exceed 255 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");
    }
}
