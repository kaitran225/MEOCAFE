using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _factory;

    public CustomerRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Customer>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, phone_number, email, address, point FROM customer ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapCustomerFull(reader));
        return list;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, phone_number, email, address, point FROM customer WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return MapCustomerFull(reader);
        return null;
    }

    public async Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, phone_number, email, address, point FROM customer WHERE phone_number = @P", conn);
        cmd.Parameters.AddWithValue("@P", phoneNumber);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return MapCustomerFull(reader);
        return null;
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string? name, string? phone, CancellationToken cancellationToken = default)
    {
        var list = new List<Customer>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var sql = "SELECT id, name, phone_number, email, address, point FROM customer WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(name)) sql += " AND name ILIKE @Name";
        if (!string.IsNullOrWhiteSpace(phone)) sql += " AND phone_number ILIKE @Phone";
        sql += " ORDER BY id";
        await using var cmd = new NpgsqlCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(name)) cmd.Parameters.AddWithValue("@Name", "%" + name.Trim() + "%");
        if (!string.IsNullOrWhiteSpace(phone)) cmd.Parameters.AddWithValue("@Phone", "%" + phone.Trim() + "%");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapCustomerFull(reader));
        return list;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO customer (name, phone_number, email, address, point) VALUES (@Name, @PhoneNumber, @Email, @Address, @Point) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", (object?)customer.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? "");
        cmd.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Point", customer.Point);
        customer.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE customer SET name = @Name, email = @Email, address = @Address, point = @Point WHERE phone_number = @PhoneNumber", conn);
        cmd.Parameters.AddWithValue("@Name", (object?)customer.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Point", customer.Point);
        cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? "");
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM customer WHERE phone_number = @P", conn);
        cmd.Parameters.AddWithValue("@P", phoneNumber);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddPointsAsync(string phoneNumber, decimal points, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE customer SET point = point + @Points WHERE phone_number = @P", conn);
        cmd.Parameters.AddWithValue("@Points", (int)points);
        cmd.Parameters.AddWithValue("@P", phoneNumber);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Customer MapCustomerFull(NpgsqlDataReader reader)
    {
        var c = new Customer
        {
            Id = reader.GetInt32(0),
            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
            PhoneNumber = reader.GetString(2),
            Point = reader.GetInt32(reader.FieldCount > 5 ? 5 : 3)
        };
        if (reader.FieldCount >= 6)
        {
            c.Email = reader.IsDBNull(3) ? null : reader.GetString(3);
            c.Address = reader.IsDBNull(4) ? null : reader.GetString(4);
        }
        return c;
    }
}
