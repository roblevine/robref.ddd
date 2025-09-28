using System.Reflection;
using NetArchTest.Rules;
using RobRef.DDD.Users.Application.Users.Commands;
using RobRef.DDD.Users.Domain.Users;
using RobRef.DDD.Users.Infrastructure.Persistence;
using RobRef.DDD.Users.WebApi.Users;
using Xunit;

namespace RobRef.DDD.Architecture.Tests;

public static class AssemblyCatalog
{
    public static Assembly UsersDomain => typeof(User).Assembly;
    public static Assembly UsersApplication => typeof(RegisterUser).Assembly;
    public static Assembly UsersInfrastructure => typeof(ApplicationDbContext).Assembly;
    public static Assembly UsersWebApi => typeof(RegisterUserRequest).Assembly;
}

public class UsersLayeringRules
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Outer_Layers()
    {
        var result = Types
            .InAssembly(AssemblyCatalog.UsersDomain)
            .Should()
            .NotHaveDependencyOnAny(
                "RobRef.DDD.Users.Application",
                "RobRef.DDD.Users.Infrastructure",
                "RobRef.DDD.Users.WebApi",
                "RobRef.DDD.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetResultMessage());
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_WebApi()
    {
        var result = Types
            .InAssembly(AssemblyCatalog.UsersApplication)
            .Should()
            .NotHaveDependencyOnAny(
                "RobRef.DDD.Users.Infrastructure",
                "RobRef.DDD.Users.WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetResultMessage());
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_WebApi()
    {
        var result = Types
            .InAssembly(AssemblyCatalog.UsersInfrastructure)
            .Should()
            .NotHaveDependencyOn("RobRef.DDD.Users.WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetResultMessage());
    }
}

public class UsersIsolationRules
{
    [Fact]
    public void Users_Context_Should_Not_Depend_On_Other_Bounded_Contexts()
    {
        var assemblies = new[]
        {
            AssemblyCatalog.UsersDomain,
            AssemblyCatalog.UsersApplication,
            AssemblyCatalog.UsersInfrastructure,
            AssemblyCatalog.UsersWebApi
        };

        var result = Types
            .InAssemblies(assemblies)
            .Should()
            .NotHaveDependencyOn("RobRef.DDD.Products")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetResultMessage());
    }

    [Fact]
    public void Application_Should_Use_Shared_Cqrs_Contracts()
    {
        var result = Types
            .InAssembly(AssemblyCatalog.UsersApplication)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn("RobRef.DDD.Application.Common")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetResultMessage());
    }
}

internal static class TestResultExtensions
{
    public static string GetResultMessage(this TestResult result)
        => result.FailingTypeNames != null
            ? string.Join("\n", result.FailingTypeNames)
            : "No failing types found";
}
