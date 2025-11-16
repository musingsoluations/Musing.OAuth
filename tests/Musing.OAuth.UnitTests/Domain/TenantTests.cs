using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;
using Musing.OAuth.Domain.Tenants;
using Xunit;

namespace Musing.OAuth.UnitTests.Domain;

public class TenantTests
{
    [Fact]
    public void AddGroup_AddsGroup()
    {
        var tenant = new Tenant(TenantId.New(), "Tenant A");
        var group = tenant.AddGroup(new GroupId(1), "Admins");
        Assert.Single(tenant.Groups);
        Assert.Equal(group, tenant.Groups.First());
    }

    [Fact]
    public void AddGroup_DuplicateName_Throws()
    {
        var tenant = new Tenant(TenantId.New(), "Tenant A");
        tenant.AddGroup(new GroupId(1), "Admins");
        Assert.Throws<DomainException>(() => tenant.AddGroup(new GroupId(2), "Admins"));
    }

    [Fact]
    public void AddSubscription_AddsSubscription()
    {
        var tenant = new Tenant(TenantId.New(), "Tenant A");
        var sub = tenant.AddSubscription(new SubscriptionId(10), new AppId(5));
        Assert.Single(tenant.Subscriptions);
        Assert.Equal(sub, tenant.Subscriptions.First());
    }

    [Fact]
    public void AddSubscription_DuplicateApplication_Throws()
    {
        var tenant = new Tenant(TenantId.New(), "Tenant A");
        tenant.AddSubscription(new SubscriptionId(10), new AppId(5));
        Assert.Throws<DomainException>(() => tenant.AddSubscription(new SubscriptionId(11), new AppId(5)));
    }
}
