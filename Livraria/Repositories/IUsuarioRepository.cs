  using Livraria.Models;

namespace Livraria.Repositories
    {
        public interface IUsuarioRepository
        {
            Task<Login?> ObterPorEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
            Task CadastrarAsync(Login usuario);
        }
    }

