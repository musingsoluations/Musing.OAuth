using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Applications;

public class Role
{
    public RoleId Id { get; private set; }
    public AppId ApplicationId { get; private set; }
    public string Name { get; private set; }

    internal Role(RoleId id, AppId applicationId, string name)
    {
        if (!id.IsValid) throw new DomainException("Role Id invalid");
        if (!applicationId.IsValid) throw new DomainException("Application Id invalid");
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new DomainException("Role name required");
        Id = id;
        ApplicationId = applicationId;
    }
}
