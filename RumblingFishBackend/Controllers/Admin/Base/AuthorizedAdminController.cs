using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RumblingFishBackend.Controllers.Admin.Base
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public abstract class AuthorizedAdminController : Controller
    {
    }
}
