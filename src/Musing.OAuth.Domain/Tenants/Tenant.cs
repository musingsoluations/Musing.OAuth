using System;
using System.Collections.Generic;
using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Tenants;

public class Tenant
{
    private readonly List<TenantSubscription> _subscriptions = new();
    private readonly List<Group> _groups = new();

    public TenantId Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<TenantSubscription> Subscriptions => _subscriptions;
    public IReadOnlyCollection<Group> Groups => _groups;

    public Tenant(TenantId id, string name, bool isActive = true)
    {
        if (id.IsEmpty) throw new DomainException("Tenant Id cannot be empty");
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new DomainException("Tenant name required");
        Id = id;
        IsActive = isActive;
    }

    public TenantSubscription AddSubscription(SubscriptionId subscriptionId, AppId appId, bool isActive = true)
    {
        if (!subscriptionId.IsValid) throw new DomainException("Subscription Id invalid");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        if (_subscriptions.Any(s => s.AppId == appId))
            throw new DomainException("Subscription for application already exists");
        var sub = new TenantSubscription(subscriptionId, Id, appId, isActive);
        _subscriptions.Add(sub);
        return sub;
    }

    public Group AddGroup(GroupId groupId, string groupName)
    {
        if (!groupId.IsValid) throw new DomainException("Group Id invalid");
        if (_groups.Any(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException("Group name already exists");
        var group = new Group(groupId, Id, groupName);
        _groups.Add(group);
        return group;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
