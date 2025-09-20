using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Queries;

public record GetAllUsers : IQuery<IReadOnlyList<User>>;