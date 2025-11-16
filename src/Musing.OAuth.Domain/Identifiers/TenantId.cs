using System;

namespace Musing.OAuth.Domain.Identifiers;

public readonly record struct TenantId(Guid Value)
{
    public static TenantId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
    public bool IsEmpty => Value == Guid.Empty;
}
