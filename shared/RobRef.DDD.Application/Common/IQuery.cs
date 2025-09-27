namespace RobRef.DDD.Application.Common;

/// <summary>
/// Marker interface for queries that return a result of type TResult.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQuery<out TResult>
{
}