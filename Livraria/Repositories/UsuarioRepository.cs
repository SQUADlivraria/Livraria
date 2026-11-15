using Livraria.Models;
using MySqlConnector;

namespace Livraria.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Conexão com o banco não encontrada.");
        }

        // Cadastra um novo usuário no banco de dados
        public async Task CadastrarAsync(Login usuario)
        {
            try
            {
                using var conexao = new MySqlConnection(_connectionString);
                await conexao.OpenAsync();

                string query = @"INSERT INTO login (nome, email, senhahash)
                                 VALUES (@Nome, @Email, @SenhaHash)";

                using var comando = new MySqlCommand(query, conexao);

                comando.Parameters.AddWithValue("@Nome", usuario.Nome);
                comando.Parameters.AddWithValue("@Email", usuario.Email);
                comando.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);

                await comando.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar usuário: " + ex.Message);
            }
        }

        // Busca um usuário pelo email para realizar o login
        public async Task<Login?> ObterPorEmailAsync(string email)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            string query = @"SELECT id, nome, email, senhahash
                             FROM login
                             WHERE email = @Email";

            using var comando = new MySqlCommand(query, conexao);
            comando.Parameters.AddWithValue("@Email", email);

            using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Login
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nome = reader.GetString("nome"),
                    Email = reader.GetString("email"),
                    SenhaHash = reader.GetString("senhahash")
                };
            }

            return null;
        }

        // Verifica se o e-mail já está cadastrado
        public async Task<bool> EmailExisteAsync(string email)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            string query = "SELECT COUNT(*) FROM login WHERE email = @Email";

            using var comando = new MySqlCommand(query, conexao);
            comando.Parameters.AddWithValue("@Email", email);

            int quantidade = Convert.ToInt32(await comando.ExecuteScalarAsync());

            return quantidade > 0;
        }
    }
}