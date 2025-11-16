using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Users;

public class UserAppAssignment
{
    public UserAppAssignmentId Id { get; private set; }
    public UserId UserId { get; private set; }
    public AppId AppId { get; private set; }
    public RoleId RoleId { get; private set; }
    public bool IsAppActive { get; private set; }

    internal UserAppAssignment(UserAppAssignmentId id, UserId userId, AppId appId, RoleId roleId, bool isAppActive)
    {
        if (!id.IsValid) throw new DomainException("Assignment Id invalid");
        if (userId.IsEmpty) throw new DomainException("User Id empty");
        if (!appId.IsValid) throw new DomainException("Application Id invalid");
        if (!roleId.IsValid) throw new DomainException("Role Id invalid");
        Id = id;
        UserId = userId;
        AppId = appId;
        RoleId = roleId;
        IsAppActive = isAppActive;
    }

    public void Activate() => IsAppActive = true;
    public void Deactivate() => IsAppActive = false;
}
