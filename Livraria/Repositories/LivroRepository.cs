
using MySqlConnector;
using Livraria.Models;

namespace Livraria.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly string _connectionString;

        public LivroRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<bool> Cadastrar(Livro livro)
        {
            try
            {
                using var conexao = new MySqlConnection(_connectionString);
                await conexao.OpenAsync();

                var query = "INSERT INTO livro (isbn, nome, descricao, valor) VALUES (@Isbn, @Nome, @Descricao, @Valor)";
                using var comando = new MySqlCommand(query, conexao);

                comando.Parameters.AddWithValue("@Isbn", livro.Isbn);
                comando.Parameters.AddWithValue("@Nome", livro.Nome);
                comando.Parameters.AddWithValue("@Descricao", livro.Descricao);
                comando.Parameters.AddWithValue("@Valor", livro.Valor);

                var linhasAfetadas = await comando.ExecuteNonQueryAsync();
                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar livro no MySQL: {ex.Message}");
            }
        }

        public async Task<List<Livro>> ListarTodos()
        {
            var livros = new List<Livro>();

            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            var query = "SELECT Id, Isbn, Nome, Descricao, Valor FROM livro";
            using var comando = new MySqlCommand(query, conexao);

            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                livros.Add(new Livro
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Isbn = reader["Isbn"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Isbn"])!,
                    Nome = reader["Nome"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Nome"])!,
                    Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Descricao"])!,
                    Valor = reader["Valor"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Valor"])
                });
            }

            return livros;
        }

        public async Task<Livro?> BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            var query = "SELECT Id, Isbn, Nome, Descricao, Valor FROM livro WHERE Id = @Id";
            using var comando = new MySqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@Id", id);

            using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Livro
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Isbn = reader["Isbn"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Isbn"])!,
                    Nome = reader["Nome"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Nome"])!,
                    Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Descricao"])!,
                    Valor = reader["Valor"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Valor"])
                };
            }

            return null;
        }

        public async Task<bool> Atualizar(Livro livro)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            var query = @"UPDATE livro
                          SET Isbn = @Isbn,
                              Nome = @Nome,
                              Descricao = @Descricao,
                              Valor = @Valor
                          WHERE Id = @Id";

            using var comando = new MySqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@Id", livro.Id);
            comando.Parameters.AddWithValue("@Isbn", livro.Isbn);
            comando.Parameters.AddWithValue("@Nome", livro.Nome);
            comando.Parameters.AddWithValue("@Descricao", livro.Descricao);
            comando.Parameters.AddWithValue("@Valor", livro.Valor);

            var linhasAfetadas = await comando.ExecuteNonQueryAsync();
            return linhasAfetadas > 0;
        }

        public async Task<bool> Excluir(int id)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            var query = "DELETE FROM livro WHERE Id = @Id";
            using var comando = new MySqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@Id", id);

            var linhasAfetadas = await comando.ExecuteNonQueryAsync();
            return linhasAfetadas > 0;
        }
    }
}
