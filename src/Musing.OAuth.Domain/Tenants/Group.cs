using System;
using System.Collections.Generic;
using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Tenants;

public class Group
{
    private readonly List<GroupAppAssignment> _appAssignments = new();

    public GroupId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }

    public IReadOnlyCollection<GroupAppAssignment> AppAssignments => _appAssignments;

    internal Group(GroupId id, TenantId tenantId, string name)
    {
        if (!id.IsValid) throw new DomainException("Group Id invalid");
        if (tenantId.IsEmpty) throw new DomainException("Tenant Id empty");
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new DomainException("Group name required");
        Id = id;
        TenantId = tenantId;
    }

    public GroupAppAssignment AssignApp(GroupAppAssignmentId assignmentId, AppId appId, RoleId roleId)
    {
        if (!assignmentId.IsValid) throw new DomainException("Assignment Id invalid");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        if (!roleId.IsValid) throw new DomainException("Role Id invalid");
        if (_appAssignments.Any(a => a.AppId == appId && a.RoleId == roleId))
            throw new DomainException("App & Role already assigned to group");
        var assignment = new GroupAppAssignment(assignmentId, Id, appId, roleId);
        _appAssignments.Add(assignment);
        return assignment;
    }
}
