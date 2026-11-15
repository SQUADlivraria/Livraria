using MySqlConnector;
using Livraria.Models;

namespace Livraria.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConfiguration _configuration;

        public UsuarioRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Login?> ObterPorEmailAsync(string email)
        {
            string conexao = _configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "String de conexão não encontrada."
                );

            await using var connection = new MySqlConnection(conexao);

            await connection.OpenAsync();

            string sql = @"SELECT id, nome, email, senhahash
                           FROM login
                           WHERE email = @Email";

            await using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Email", email);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Login
            {
                Id = reader.GetInt32("id"),
                Nome = reader.GetString("nome"),
                Email = reader.GetString("email"),
                SenhaHash = reader.GetString("senhahash")
            };
        }
    }
}