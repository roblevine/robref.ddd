namespace RobRef.DDD.Users.WebApi.Users;

public sealed record GetAllUsersResponse(IReadOnlyList<UserResponse> Users);