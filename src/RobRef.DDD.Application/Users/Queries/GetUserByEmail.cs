using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Queries;

public record GetUserByEmail(string Email) : IQuery<User?>;