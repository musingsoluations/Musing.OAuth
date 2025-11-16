using System;

namespace Musing.OAuth.Domain.Identifiers;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
    public bool IsEmpty => Value == Guid.Empty;
}
