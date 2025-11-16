namespace Musing.OAuth.Domain.Identifiers;

public readonly record struct UserAppAssignmentId(long Value)
{
    public override string ToString() => Value.ToString();
    public bool IsValid => Value > 0;
}
