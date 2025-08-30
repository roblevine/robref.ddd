using System;

namespace AcmeStore.Domain.Tests.Users.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void Create_RequiresFirstAndLast_AllowsOptionalTitle()
    {
        var name = AcmeStore.Domain.Users.ValueObjects.PersonName.Create("Dr", "Ada", "Lovelace");
        Assert.Equal("Dr", name.Title);
        Assert.Equal("Ada", name.FirstName);
        Assert.Equal("Lovelace", name.LastName);
    }

    [Fact]
    public void Create_Throws_IfFirstOrLastMissing()
    {
        Assert.Throws<ArgumentException>(() => AcmeStore.Domain.Users.ValueObjects.PersonName.Create(null, "", "Smith"));
        Assert.Throws<ArgumentException>(() => AcmeStore.Domain.Users.ValueObjects.PersonName.Create(null, "Jane", " "));
    }

    [Fact]
    public void Equality_BasedOnAllComponents_CaseSensitiveForNames()
    {
        var a = AcmeStore.Domain.Users.ValueObjects.PersonName.Create("Mr", "John", "Smith");
        var b = AcmeStore.Domain.Users.ValueObjects.PersonName.Create("Mr", "John", "Smith");

        Assert.Equal(a, b);
    }
}


