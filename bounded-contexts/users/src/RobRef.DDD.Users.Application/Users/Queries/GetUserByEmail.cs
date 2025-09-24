using RobRef.DDD.Users.Application.Common;
using RobRef.DDD.Users.Domain.Users;

namespace RobRef.DDD.Users.Application.Users.Queries;

public record GetUserByEmail(string Email) : IQuery<User?>;