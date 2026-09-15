using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Services;
using System.Security.Claims;
using RumblingFishBackend.Models.DTO.Login;

namespace RumblingFishBackend.Controllers.Admin
{
    [Route("admin/[controller]")]
    public class LoginController : Controller
    {
        private readonly AdminAccountService _adminAccountService;

        public LoginController(AdminAccountService adminAccountService)
        {
            _adminAccountService = adminAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync("AdminCookie");

            if (result.Succeeded)
            {
                return RedirectToAction("index", "home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, string? returnUrl)
        {
            var account = await _adminAccountService.ValidateCredentialsAsync(model.Username, model.Password);

            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }

            var claims = new[] { new Claim(ClaimTypes.Name, account.Username) };
            var identity = new ClaimsIdentity(claims, "AdminCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AdminCookie", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("index", "home");
        }

        [HttpPost("/admin/logout")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "AdminCookie")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("index");
        }
    }
}
