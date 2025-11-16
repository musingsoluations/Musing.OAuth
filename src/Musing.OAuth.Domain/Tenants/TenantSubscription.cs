using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Tenants;

public class TenantSubscription
{
    public SubscriptionId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public AppId AppId { get; private set; }
    public bool IsActive { get; private set; }

    internal TenantSubscription(SubscriptionId id, TenantId tenantId, AppId appId, bool isActive)
    {
        if (!id.IsValid) throw new DomainException("Subscription Id invalid");
        if (tenantId.IsEmpty) throw new DomainException("Tenant Id empty");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        Id = id;
        TenantId = tenantId;
        AppId = appId;
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
