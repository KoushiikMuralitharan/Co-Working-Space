using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Bookings.CreateBooking;
using CoWorkingSpace.Application.Bookings.GetBooking;
using CoWorkingSpace.Application.Bookings.GetMyBookings;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;
using Npgsql;

namespace CoWorkingSpace.Infrastructure.Persistence.Bookings
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Guid> CreateBookingAsync(CreateBookingRequest request)
        {
            const string query = "SELECT create_booking(@customer_id, @resource_id, @start_at, @end_at, @price);";

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("customer_id", request.CustomerId);
            command.Parameters.AddWithValue("resource_id", request.ResourceId);
            command.Parameters.AddWithValue("start_at", request.StartAt.ToUniversalTime());
            command.Parameters.AddWithValue("end_at", request.EndAt.ToUniversalTime());
            command.Parameters.AddWithValue("price", request.Price);

            try
            {
                var result = await command.ExecuteScalarAsync();
                return (Guid)result!;
            }
            catch(PostgresException ex)
             when (ex.SqlState == "P0001")
            {
                throw new ConflictException(ex.MessageText);
            }
            catch (PostgresException ex)
                when (ex.SqlState == "P0002")
            {
                throw new NotFoundException(ex.MessageText);
            }
            catch (PostgresException ex)
                when (ex.SqlState == "P0003" ||
                       ex.SqlState == "P0004")
            {
                throw new BadRequestException(ex.MessageText);
            }


        }
        public async Task<GetBookingResult?> GetBookingByIdAsync(Guid bookingId)
        {
            const string query = " SELECT b.booking_id, b.customer_id, b.start_at, b.end_at, b.status, b.total_amount, b.currency, brs.bookable_resource_id, brs.price FROM bookings b LEFT JOIN booking_resources brs ON brs.booking_id = b.booking_id WHERE b.booking_id = @booking_id ORDER BY brs.created_at;";

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("booking_id", bookingId);

            await using var reader = await command.ExecuteReaderAsync();

            GetBookingResult? result = null;

            while (await reader.ReadAsync())
            {
                if(result == null)
                {
                    result = new GetBookingResult
                    {
                        BookingId = reader.GetGuid(reader.GetOrdinal("booking_id")),
                        CustomerId = reader.GetGuid(reader.GetOrdinal("customer_id")),
                        StartAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("start_at")),
                        EndAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("end_at")),
                        Status = reader.GetString(reader.GetOrdinal("status")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount")),
                        Currency = reader.GetString(reader.GetOrdinal("currency"))
                    };
                }

                var resourceIdOrdinal = reader.GetOrdinal("bookable_resource_id");

                if(!reader.IsDBNull(resourceIdOrdinal))
                {
                    result.Resources.Add(new GetBookingResourceResult
                    {
                        BookableResourceId = reader.GetGuid(resourceIdOrdinal),
                        Price = reader.GetDecimal(reader.GetOrdinal("price"))
                    });
                }
            }

            return result;
        }
        public async Task<List<GetMyBookingsResult>> GetMyBookingsAsync(Guid customerId)
        {
            const string query = """
            SELECT
                booking_id,
                customer_id,
                start_at,
                end_at,
                status,
                total_amount,
                currency
            FROM bookings
            WHERE customer_id = @customer_id
            ORDER BY start_at DESC;
            """;

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("customer_id", customerId);

            await using var reader = await command.ExecuteReaderAsync();

            var bookings = new List<GetMyBookingsResult>();

            while (await reader.ReadAsync())
            {
                bookings.Add(new GetMyBookingsResult
                {
                    BookingId = reader.GetGuid(reader.GetOrdinal("booking_id")),

                    CustomerId = reader.GetGuid(reader.GetOrdinal("customer_id")),

                    StartAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("start_at")),

                    EndAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("end_at")),

                    Status = reader.GetString(reader.GetOrdinal("status")),

                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount")),

                    Currency = reader.GetString(reader.GetOrdinal("currency"))
                });
            }

            return bookings;
        }
        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            const string query = "SELECT cancel_booking(@booking_id);";

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("booking_id", bookingId);

            try
            {
                var result =
                    await command.ExecuteScalarAsync();

                return (bool)result!;
            }
            catch (PostgresException ex)
                when (ex.SqlState == "P0005" || ex.SqlState == "P0006")
            {
                throw new ConflictException(ex.MessageText);
            }
        }
    }

        
}
