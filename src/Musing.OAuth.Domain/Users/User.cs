using System;
using System.Collections.Generic;
using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Users;

public class User
{
    private readonly List<UserGroupMembership> _groupMemberships = new();
    private readonly List<UserAppAssignment> _appAssignments = new();

    public UserId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsGlobalActive { get; private set; }

    public IReadOnlyCollection<UserGroupMembership> GroupMemberships => _groupMemberships;
    public IReadOnlyCollection<UserAppAssignment> AppAssignments => _appAssignments;

    public User(UserId id, TenantId tenantId, string email, string passwordHash, bool isGlobalActive = true)
    {
        if (id.IsEmpty) throw new DomainException("User Id empty");
        if (tenantId.IsEmpty) throw new DomainException("Tenant Id empty");
        Email = ValidateEmail(email);
        PasswordHash = !string.IsNullOrWhiteSpace(passwordHash) ? passwordHash : throw new DomainException("Password hash required");
        Id = id;
        TenantId = tenantId;
        IsGlobalActive = isGlobalActive;
    }

    public UserGroupMembership JoinGroup(GroupId groupId)
    {
        if (!groupId.IsValid) throw new DomainException("Group Id invalid");
        if (_groupMemberships.Any(m => m.GroupId == groupId)) throw new DomainException("Already in group");
        var membership = new UserGroupMembership(Id, groupId);
        _groupMemberships.Add(membership);
        return membership;
    }

    public void LeaveGroup(GroupId groupId)
    {
        var membership = _groupMemberships.FirstOrDefault(m => m.GroupId == groupId);
        if (membership == null) throw new DomainException("Membership not found");
        _groupMemberships.Remove(membership);
    }

    public UserAppAssignment AssignDirectAccess(UserAppAssignmentId assignmentId, AppId appId, RoleId roleId, bool isAppActive = true)
    {
        if (!assignmentId.IsValid) throw new DomainException("Assignment Id invalid");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        if (!roleId.IsValid) throw new DomainException("Role Id invalid");
        if (_appAssignments.Any(a => a.AppId == appId && a.RoleId == roleId))
            throw new DomainException("Direct access already exists");
        var assignment = new UserAppAssignment(assignmentId, Id, appId, roleId, isAppActive);
        _appAssignments.Add(assignment);
        return assignment;
    }

    public void DeactivateGlobal() => IsGlobalActive = false;
    public void ActivateGlobal() => IsGlobalActive = true;

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email required");
        if (!email.Contains('@')) throw new DomainException("Invalid email format");
        return email;
    }
}
