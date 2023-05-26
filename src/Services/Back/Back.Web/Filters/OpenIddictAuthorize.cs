using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Back.Web.Filters;

public class OpenIddictAuthorize : AuthorizeAttribute
{
    public OpenIddictAuthorize() =>
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}