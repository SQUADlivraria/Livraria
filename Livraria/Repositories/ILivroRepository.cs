
using Livraria.Models;

namespace Livraria.Repositories
{
    public interface ILivroRepository
    {
        Task<bool> Cadastrar(Livro livro);

        Task<Livro?> BuscarPorId(int id);

        Task<bool> Atualizar(Livro livro);

        Task<bool> Excluir(int id);
    }
}
