using KayuSushi.Models;
using Microsoft.Data.SqlClient;

namespace KayuSushi.Data
{
    public class CategorieAdoRepository
    {
        private readonly string _connectionString;

        public CategorieAdoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Categorie> GetAll()
        {
            var categories = new List<Categorie>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT IdCategorie, NomCategorie, Description FROM Categories", connection);

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Categorie
                {
                    IdCategorie = reader.GetInt32(0),
                    NomCategorie = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }

            return categories;
        }

        public Categorie? GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT IdCategorie, NomCategorie, Description FROM Categories WHERE IdCategorie = @IdCategorie",
                connection);

            command.Parameters.AddWithValue("@IdCategorie", id);

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Categorie
                {
                    IdCategorie = reader.GetInt32(0),
                    NomCategorie = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }

            return null;
        }

        public void Create(Categorie categorie)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "INSERT INTO Categories (NomCategorie, Description) VALUES (@NomCategorie, @Description)",
                connection);

            command.Parameters.AddWithValue("@NomCategorie", categorie.NomCategorie);
            command.Parameters.AddWithValue("@Description", (object?)categorie.Description ?? DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(Categorie categorie)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE Categories 
                  SET NomCategorie = @NomCategorie, Description = @Description 
                  WHERE IdCategorie = @IdCategorie",
                connection);

            command.Parameters.AddWithValue("@IdCategorie", categorie.IdCategorie);
            command.Parameters.AddWithValue("@NomCategorie", categorie.NomCategorie);
            command.Parameters.AddWithValue("@Description", (object?)categorie.Description ?? DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "DELETE FROM Categories WHERE IdCategorie = @IdCategorie",
                connection);

            command.Parameters.AddWithValue("@IdCategorie", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
