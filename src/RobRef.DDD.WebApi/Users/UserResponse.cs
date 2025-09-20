using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.WebApi.Users;

public sealed record UserResponse(
    string Id,
    string Email,
    string? Title,
    string FirstName,
    string LastName)
{
    public static UserResponse FromDomain(User user)
    {
        return new UserResponse(
            user.Id.ToString(),
            user.Email.Value,
            user.Title?.Value,
            user.FirstName.Value,
            user.LastName.Value
        );
    }
}