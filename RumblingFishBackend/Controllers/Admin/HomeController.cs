using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Admin.Base;
using RumblingFishBackend.Models.DTO.Admin;
using RumblingFishBackend.Services;

namespace RumblingFishBackend.Controllers.Admin
{
    [Route("admin/[controller]")]
    public class HomeController : AuthorizedAdminController
    {
        private const int DefaultPageSize = 10;
        private static readonly int[] AllowedPageSizes = { 10, 20, 50, 100 };
        private readonly AdminPlayerService _service;

        public HomeController(AdminPlayerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        [Route("/admin")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = DefaultPageSize,
        string? countryCode = null, string? nickname = null, DateOnly? dateFrom = null, DateOnly? dateTo = null)
        {
            if (!AllowedPageSizes.Contains(pageSize))
            {
                pageSize = DefaultPageSize;
            }

            var filter = new PlayerListFilter(countryCode?.Trim().ToUpperInvariant(), nickname?.Trim(), dateFrom, dateTo);

            var model = await _service.GetPlayersAsync(page, pageSize, filter);
            return View(model);
        }
    }
}
