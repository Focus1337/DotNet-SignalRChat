using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Back.Web;

public class OpenIddictAuthorize : AuthorizeAttribute
{
    public OpenIddictAuthorize() =>
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}