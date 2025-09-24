namespace RobRef.DDD.Users.Application.Common;

public interface ICommand
{
}

public interface ICommand<out TResult>
{
}