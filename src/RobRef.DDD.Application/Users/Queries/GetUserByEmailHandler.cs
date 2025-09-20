using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Queries;

public sealed class GetUserByEmailHandler(IUserRepository userRepository) : IQueryHandler<GetUserByEmail, User?>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task<User?> HandleAsync(GetUserByEmail query, CancellationToken cancellationToken = default)
    {
        var email = new Email(query.Email);
        return await _userRepository.FindByEmailAsync(email, cancellationToken);
    }
}