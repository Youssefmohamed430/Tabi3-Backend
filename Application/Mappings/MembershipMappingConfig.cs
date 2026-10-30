#nullable enable

namespace Application.Mappings;

public sealed class MembershipMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateMemberRequestDto, TenantMembership>()
            .Map(target => target.PermissionsJson, source => NormalizePermissions(source.Permissions))
            .Ignore(target => target.Id)
            .Ignore(target => target.TenantId)
            .Ignore(target => target.UserId)
            .Ignore(target => target.Role)
            .Ignore(target => target.Status)
            .Ignore(target => target.InvitedByUserId)
            .Ignore(target => target.JoinedAt)
            .Ignore(target => target.CreatedAt)
            .Ignore(target => target.IsDeleted);

        config.NewConfig<InviteMemberRequestDto, TenantMembership>()
            .Map(target => target.PermissionsJson, source => NormalizePermissions(source.Permissions))
            .Ignore(target => target.Id)
            .Ignore(target => target.TenantId)
            .Ignore(target => target.UserId)
            .Ignore(target => target.RoleId)
            .Ignore(target => target.Role)
            .Ignore(target => target.Status)
            .Ignore(target => target.InvitedByUserId)
            .Ignore(target => target.JoinedAt)
            .Ignore(target => target.CreatedAt)
            .Ignore(target => target.IsDeleted);
    }

    private static string? NormalizePermissions(IReadOnlyCollection<string>? permissions)
    {
        if (permissions is null || permissions.Count == 0)
            return null;
        var normalized = permissions.Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return normalized.Length == 0 ? null : System.Text.Json.JsonSerializer.Serialize(normalized);
    }
}
