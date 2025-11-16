using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;
using Musing.OAuth.Domain.Users;
using Xunit;

namespace Musing.OAuth.UnitTests.Domain;

public class UserTests
{
    private static User NewUser() => new User(UserId.New(), TenantId.New(), "user@example.com", "hash");

    [Fact]
    public void JoinGroup_AddsMembership()
    {
        var user = NewUser();
        var membership = user.JoinGroup(new GroupId(1));
        Assert.Single(user.GroupMemberships);
        Assert.Equal(membership, user.GroupMemberships.First());
    }

    [Fact]
    public void JoinGroup_Duplicate_Throws()
    {
        var user = NewUser();
        user.JoinGroup(new GroupId(1));
        Assert.Throws<DomainException>(() => user.JoinGroup(new GroupId(1)));
    }

    [Fact]
    public void AssignDirectAccess_AddsAssignment()
    {
        var user = NewUser();
        var assignment = user.AssignDirectAccess(new UserAppAssignmentId(100), new AppId(5), new RoleId(9));
        Assert.Single(user.AppAssignments);
        Assert.Equal(assignment, user.AppAssignments.First());
    }

    [Fact]
    public void AssignDirectAccess_Duplicate_Throws()
    {
        var user = NewUser();
        user.AssignDirectAccess(new UserAppAssignmentId(100), new AppId(5), new RoleId(9));
        Assert.Throws<DomainException>(() => user.AssignDirectAccess(new UserAppAssignmentId(101), new AppId(5), new RoleId(9)));
    }
}
