using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using QuickCode.Demo.UserManagerModule.Persistence.Contexts;

namespace QuickCode.Demo.UserManagerModule.Api.Extension;

public class CustomClaimsPrincipalFactory(
    UserManager<ApiUser> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApiUser>(userManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApiUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("PermissionGroupId", user.PermissionGroupId.ToString()!));
        return identity;
    }
}