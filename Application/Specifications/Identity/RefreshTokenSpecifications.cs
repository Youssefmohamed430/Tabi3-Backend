namespace Application.Specifications.Identity;

public sealed class RefreshTokenByHashSpecification : Specification<RefreshToken>
{
    public RefreshTokenByHashSpecification(string tokenHash) => Query.Where(item => item.TokenHash == tokenHash);
}
