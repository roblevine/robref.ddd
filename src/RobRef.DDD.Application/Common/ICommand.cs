namespace RobRef.DDD.Application.Common;

public interface ICommand
{
}

public interface ICommand<out TResult>
{
}