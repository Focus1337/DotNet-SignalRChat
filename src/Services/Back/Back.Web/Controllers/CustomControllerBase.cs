using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Back.Web.Controllers;

public class CustomControllerBase : ControllerBase
{
    internal BadRequestObjectResult BadRequestDueToToken() =>
        BadRequest(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.InvalidGrant,
            ErrorDescription = "The refresh token is no longer valid."
        });
}