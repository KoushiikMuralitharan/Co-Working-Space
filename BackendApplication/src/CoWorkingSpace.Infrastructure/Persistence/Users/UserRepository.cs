using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Authentication.Login;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Infrastructure.Persistence.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            const string query = "SELECT EXISTS (SELECT 1 FROM users WHERE email = @email);";

            await using var connection = new Npgsql.NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new Npgsql.NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("email", email);

            var result = await command.ExecuteScalarAsync();

            return (bool)result!;
        }

        public async Task<Guid> CreateUserAsync(string firstName, string lastName, string email, string passwordHash, string? phoneNo)
        {
            const string query = "INSERT INTO users (first_name, last_name, email, password_hash, phone_no, user_role, status) VALUES (@first_name, @last_name, @email, @password_hash, @phone_no, 'CUSTOMER', 'ACTIVE' ) RETURNING userid;";

            await using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new Npgsql.NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("first_name", firstName);
            command.Parameters.AddWithValue("last_name", lastName);
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("password_hash", passwordHash);

            command.Parameters.AddWithValue("phone_no", (object?)phoneNo ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();

            return (Guid)result!;
        }

        public async Task MarkEmailAsVerifiedAsync(Guid userId)
        {
            const string query = "UPDATE users SET email_verified_at = CURRENT_TIMESTAMP WHERE userid = @user_id AND email_verified_at is NULL";

            await using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new Npgsql.NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("user_id", userId);

            await command.ExecuteNonQueryAsync();

        }

        public async Task<LoginUser?> GetUserByEmailAsync(string email)
        {
            const string query = "SELECT userid, first_name, last_name, email, password_hash, user_role, status, email_verified_at FROM users WHERE email = @email;";

            await using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new Npgsql.NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("email", email);

            await using var reader = await command.ExecuteReaderAsync();

            if(!await reader.ReadAsync())
            {
                return null;
            }

            return new LoginUser
            {
                UserId = reader.GetGuid(reader.GetOrdinal("userid")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                UserRole = reader.GetString(reader.GetOrdinal("user_role")),
                Status = reader.GetString(reader.GetOrdinal("status")),
                EmailVerifiedAt = reader.IsDBNull(reader.GetOrdinal("email_verified_at")) ? null : reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("email_verified_at"))

            };
        }
    }
}
