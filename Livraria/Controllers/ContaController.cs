using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Livraria.Models;
using Livraria.Repositories;

namespace Livraria.Controllers
{
    public class ContaController : Controller
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IPasswordHasher<Login> _passwordHasher;

        public ContaController(
            ILoginRepository loginRepository,
            IPasswordHasher<Login> passwordHasher)
        {
            _loginRepository = loginRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _loginRepository.ObterPorEmailAsync(model.Email);

            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail ou senha incorretos.");
                return View(model);
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.SenhaHash,
                model.Senha
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "E-mail ou senha incorretos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var identidade = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identidade);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }
    }
}