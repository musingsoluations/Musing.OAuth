using System;
using System.Collections.Generic;
using System.Linq;
using Musing.OAuth.Domain.Abstractions;
using Musing.OAuth.Domain.Identifiers;

namespace Musing.OAuth.Domain.Applications;

public class Application
{
    private readonly List<Role> _roles = new();

    public AppId Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles;

    public Application(AppId id, string name, string code)
    {
        if (!id.IsValid) throw new DomainException("Application Id invalid");
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new DomainException("Application name required");
        Code = !string.IsNullOrWhiteSpace(code) ? code : throw new DomainException("Application code required");
        Id = id;
    }

    public Role DefineRole(RoleId roleId, string roleName)
    {
        if (!roleId.IsValid) throw new DomainException("Role Id invalid");
        if (_roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException("Role name already exists");
        var role = new Role(roleId, Id, roleName);
        _roles.Add(role);
        return role;
    }
}
