  using Livraria.Models;

namespace Livraria.Repositories
    {
        public interface IUsuarioRepository
        {
            Task<Login?> ObterPorEmailAsync(string email);
        }
    }

