using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Queries;

public sealed class GetAllUsersHandler(IUserRepository userRepository) : IQueryHandler<GetAllUsers, IReadOnlyList<User>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task<IReadOnlyList<User>> HandleAsync(GetAllUsers query, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }
}