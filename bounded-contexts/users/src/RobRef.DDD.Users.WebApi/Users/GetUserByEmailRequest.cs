using System.ComponentModel.DataAnnotations;
using DomainUsers = RobRef.DDD.Users.Domain.Users;

namespace RobRef.DDD.Users.WebApi.Users;

public sealed class GetUserByEmailRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(DomainUsers.Email.MaxLength, ErrorMessage = "Email cannot exceed {1} characters.")]
    public required string Email { get; init; }
}