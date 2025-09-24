using RobRef.DDD.Users.Application.Common;
using RobRef.DDD.Users.Domain.Users;

namespace RobRef.DDD.Users.Application.Users.Commands;

public sealed record RegisterUser(
    string Email,
    string? Title,
    string FirstName,
    string LastName
) : ICommand<UserId>;