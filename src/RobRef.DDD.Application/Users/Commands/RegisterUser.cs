using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Commands;

public sealed record RegisterUser(
    string Email,
    string? Title,
    string FirstName,
    string LastName
) : ICommand<UserId>;