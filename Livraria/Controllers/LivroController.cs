
using Livraria.Models;
using Livraria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Livraria.Controllers
{
    public class LivroController : Controller
    {
        private readonly ILivroRepository _livroRepository;

        public LivroController(ILivroRepository livroRepository)
        {
            _livroRepository = livroRepository;
        }

        public async Task<IActionResult> Index()
        {
            var livros = await _livroRepository.ListarTodos();

            return View(livros);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Livro livro)
        {
            if (!ModelState.IsValid)
                return View(livro);

            await _livroRepository.Cadastrar(livro);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var livro = await _livroRepository.BuscarPorId(id);

            if (livro == null)
                return NotFound();

            return View(livro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Livro livro)
        {
            if (!ModelState.IsValid)
                return View(livro);

            await _livroRepository.Atualizar(livro);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            await _livroRepository.Excluir(id);

            return RedirectToAction("Index");
        }
    }
}
