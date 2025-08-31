using Xunit;

namespace UserDomain.Tests.Smoke;

public class ProjectIntegrityTests
{
    [Fact]
    public void DomainAssemblyMarker_Type_Should_Exist()
    {
        var t = typeof(UserDomain.DomainAssemblyMarker);
        Assert.NotNull(t);
    }
}
