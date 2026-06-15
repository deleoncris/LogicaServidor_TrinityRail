using System.Security.Claims;
using LogicaServidor.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    public AccountController()
    {
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    private string Usuario = "Administrador", Contrasena = "Contraseña";
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (ModelState.IsValid)
        {
            if (vm.Usuario == Usuario && vm.Contrasena == Contrasena)
            {
                List<Claim> claims = [];
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal, new AuthenticationProperties
                {
                    IsPersistent = true
                });
                return RedirectToAction("Index", "Relation");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(vm);
            }
        }
        else
        {
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}