using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RumblingFishBackend.Controllers.Admin
{
    [Route("admin/[controller]")]
    public class HomeController : Controller
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }
    }
}
