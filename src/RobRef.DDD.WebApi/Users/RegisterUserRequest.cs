using System.ComponentModel.DataAnnotations;
using DomainUsers = RobRef.DDD.Domain.Users;

namespace RobRef.DDD.WebApi.Users;

public sealed record RegisterUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(DomainUsers.Email.MaxLength, ErrorMessage = "Email cannot exceed {1} characters.")]
    public string Email { get; init; } = string.Empty;

    [StringLength(DomainUsers.Title.MaxLength, MinimumLength = DomainUsers.Title.MinLength, ErrorMessage = "Title must be between {2} and {1} characters.")]
    public string? Title { get; init; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(DomainUsers.FirstName.MaxLength, MinimumLength = DomainUsers.FirstName.MinLength, ErrorMessage = "First name must be between {2} and {1} characters.")]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(DomainUsers.LastName.MaxLength, MinimumLength = DomainUsers.LastName.MinLength, ErrorMessage = "Last name must be between {2} and {1} characters.")]
    public string LastName { get; init; } = string.Empty;

    public IDictionary<string, string[]>? Validate()
    {
        var validationContext = new ValidationContext(this);
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateObject(this, validationContext, validationResults, validateAllProperties: true))
        {
            return null;
        }

        return validationResults
            .SelectMany(result => result.MemberNames.Select(member => new { Member = member, Error = result.ErrorMessage ?? "Invalid value." }))
            .GroupBy(item => item.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(item => item.Error).Distinct().ToArray());
    }

    public string? NormalizeTitle()
    {
        return string.IsNullOrWhiteSpace(Title) ? null : Title.Trim();
    }
}
