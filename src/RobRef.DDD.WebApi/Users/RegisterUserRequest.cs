using System.ComponentModel.DataAnnotations;
using DomainUsers = RobRef.DDD.Domain.Users;

namespace RobRef.DDD.WebApi.Users;

public sealed class RegisterUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(DomainUsers.Email.MaxLength, ErrorMessage = "Email cannot exceed {1} characters.")]
    public required string Email { get; init; }

    [StringLength(DomainUsers.Title.MaxLength, MinimumLength = DomainUsers.Title.MinLength, ErrorMessage = "Title must be between {2} and {1} characters.")]
    public string? Title { get; init; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(DomainUsers.FirstName.MaxLength, MinimumLength = DomainUsers.FirstName.MinLength, ErrorMessage = "First name must be between {2} and {1} characters.")]
    public required string FirstName { get; init; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(DomainUsers.LastName.MaxLength, MinimumLength = DomainUsers.LastName.MinLength, ErrorMessage = "Last name must be between {2} and {1} characters.")]
    public required string LastName { get; init; }
}
