using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Applications;
using Musing.OAuth.Domain.Identifiers;
using Xunit;

namespace Musing.OAuth.UnitTests.Domain;

public class ApplicationTests
{
    [Fact]
    public void DefineRole_AddsRole()
    {
        var app = new Application(new AppId(1), "App One", "APP1");
        var role = app.DefineRole(new RoleId(10), "Admin");
        Assert.Single(app.Roles);
        Assert.Equal(role, app.Roles.First());
    }

    [Fact]
    public void DefineRole_DuplicateName_Throws()
    {
        var app = new Application(new AppId(1), "App One", "APP1");
        app.DefineRole(new RoleId(10), "Admin");
        Assert.Throws<DomainException>(() => app.DefineRole(new RoleId(11), "Admin"));
    }
}
