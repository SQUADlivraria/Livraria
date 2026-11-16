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
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<Login> _passwordHasher;

        public ContaController(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher<Login> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
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

            var usuario = await _usuarioRepository.ObterPorEmailAsync(model.Email);

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
        
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(CadastroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExiste = await _usuarioRepository.EmailExisteAsync(model.Email);

            if (emailExiste)
            {
                ModelState.AddModelError(
                    "Email",
                    "Este e-mail já está cadastrado."
                );

                return View(model);
            }

            var usuario = new Login
            {
                Nome = model.Nome,
                Email = model.Email
            };

            usuario.SenhaHash = _passwordHasher.HashPassword(
                usuario,
                model.Senha
            );

            await _usuarioRepository.CadastrarAsync(usuario);

            TempData["Mensagem"] =
                "Cadastro realizado com sucesso! Faça seu login.";

            return RedirectToAction("Login");
        }
    }
}
  
