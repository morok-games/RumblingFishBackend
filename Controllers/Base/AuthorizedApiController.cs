using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RumblingFishBackend.Controllers.Base
{
    [Authorize]
    public abstract class AuthorizedApiController : ControllerBase
    {
        protected string FirebaseUid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
