namespace RobRef.DDD.WebApi.Users;

public sealed record GetAllUsersResponse(IReadOnlyList<UserResponse> Users);