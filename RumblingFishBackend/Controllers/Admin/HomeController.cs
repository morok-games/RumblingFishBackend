using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Admin.Base;

namespace RumblingFishBackend.Controllers.Admin
{
    [Route("admin/[controller]")]
    public class HomeController : AuthorizedAdminController
    {
        [HttpGet]
        [Route("")]
        [Route("/admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
