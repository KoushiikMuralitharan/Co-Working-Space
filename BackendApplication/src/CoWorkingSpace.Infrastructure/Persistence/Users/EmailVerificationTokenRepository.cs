using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Interfaces;
using Npgsql;

namespace CoWorkingSpace.Infrastructure.Persistence.Users
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly string _connectionString;

        public EmailVerificationTokenRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateVerificationTokenAsync(Guid userId, string tokenHash, DateTimeOffset expiresAt)
        {
            const string query = "INSERT INTO email_verification_tokens ( user_id, token_hash, expires_at ) VALUES ( @user_id, @token_hash, @expires_at );";

            await using var connection =  new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("user_id", userId);

            command.Parameters.AddWithValue("token_hash", tokenHash);

            command.Parameters.AddWithValue("expires_at", expiresAt.UtcDateTime);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Guid?> GetUserIdByValidTokenAsync(
            string tokenHash)
        {
            const string query = "SELECT user_id FROM email_verification_tokens WHERE token_hash = @token_hash AND used_at IS NULL AND expires_at > CURRENT_TIMESTAMP;";

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("token_hash", tokenHash);

            var result = await command.ExecuteScalarAsync();

            if (result is null)
            {
                return null;
            }

            return (Guid)result;
        }

        public async Task MarkVerificationTokenAsUsedAsync(
            string tokenHash)
        {
            const string query = "UPDATE email_verification_tokens SET used_at = CURRENT_TIMESTAMP  WHERE token_hash = @token_hash AND used_at IS NULL;";

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("token_hash", tokenHash);

            await command.ExecuteNonQueryAsync();
        }
    }
}
