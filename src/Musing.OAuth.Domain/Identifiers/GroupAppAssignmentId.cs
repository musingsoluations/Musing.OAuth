namespace Musing.OAuth.Domain.Identifiers;

public readonly record struct GroupAppAssignmentId(long Value)
{
    public override string ToString() => Value.ToString();
    public bool IsValid => Value > 0;
}
