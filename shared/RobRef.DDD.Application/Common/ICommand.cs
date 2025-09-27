namespace RobRef.DDD.Application.Common;

/// <summary>
/// Marker interface for commands that do not return a result.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker interface for commands that return a result of type TResult.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommand<out TResult>
{
}