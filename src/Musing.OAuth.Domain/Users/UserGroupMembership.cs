using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Users;

public class UserGroupMembership
{
    public UserId UserId { get; private set; }
    public GroupId GroupId { get; private set; }

    internal UserGroupMembership(UserId userId, GroupId groupId)
    {
        UserId = userId;
        GroupId = groupId;
    }
}
