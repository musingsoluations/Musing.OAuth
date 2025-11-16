using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Tenants;

public class GroupAppAssignment
{
    public GroupAppAssignmentId Id { get; private set; }
    public GroupId GroupId { get; private set; }
    public AppId AppId { get; private set; }
    public RoleId RoleId { get; private set; }

    internal GroupAppAssignment(GroupAppAssignmentId id, GroupId groupId, AppId appId, RoleId roleId)
    {
        if (!id.IsValid) throw new DomainException("Assignment Id invalid");
        if (!groupId.IsValid) throw new DomainException("Group Id invalid");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        if (!roleId.IsValid) throw new DomainException("Role Id invalid");
        Id = id;
        GroupId = groupId;
        AppId = appId;
        RoleId = roleId;
    }
}
