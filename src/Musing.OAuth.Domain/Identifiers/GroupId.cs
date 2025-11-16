namespace Musing.OAuth.Domain.Identifiers;

public readonly record struct GroupId(int Value)
{
    public override string ToString() => Value.ToString();
    public bool IsValid => Value > 0;
}
