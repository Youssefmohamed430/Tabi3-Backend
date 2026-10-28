using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class MemberService : IMemberService
{
    public Task<Result<IReadOnlyList<TenantMembership>>> GetMembersAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TenantMembership>> InviteAsync(InviteMemberRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetMembershipActiveAsync(Guid membershipId, bool isActive, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TenantMembership>> UpdateMembershipAsync(UpdateMemberRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
