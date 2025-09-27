using RobRef.DDD.Application.Common;
using RobRef.DDD.Users.Domain.Users;

namespace RobRef.DDD.Users.Application.Users.Queries;

public record GetAllUsers : IQuery<IReadOnlyList<User>>;