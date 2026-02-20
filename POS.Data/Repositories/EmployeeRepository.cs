using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _factory;

    public EmployeeRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Employee>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, fullname, username, password, phone_number, gender, address, dob, role FROM employee ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(Map(reader));
        return list;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, fullname, username, password, phone_number, gender, address, dob, role FROM employee WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return Map(reader);
        return null;
    }

    public async Task<Employee?> GetByUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, fullname, username, password, phone_number, gender, address, dob, role FROM employee WHERE username = @U AND password = @P", conn);
        cmd.Parameters.AddWithValue("@U", username);
        cmd.Parameters.AddWithValue("@P", password);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return Map(reader);
        return null;
    }

    public async Task<bool> ExistsUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT 1 FROM employee WHERE username = @U AND password = @P LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@U", username);
        cmd.Parameters.AddWithValue("@P", password);
        var o = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return o != null;
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO employee (fullname, username, password, phone_number, gender, address, dob, role) VALUES (@Fullname, @Username, @Password, @PhoneNumber, @Gender, @Address, @Dob, @Role) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Fullname", employee.Fullname ?? "");
        cmd.Parameters.AddWithValue("@Username", employee.Username ?? "");
        cmd.Parameters.AddWithValue("@Password", employee.Password ?? "");
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)employee.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Gender", (object?)employee.Gender ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Address", (object?)employee.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Dob", (object?)employee.Dob ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Role", employee.Role ?? "");
        employee.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "UPDATE employee SET fullname = @Fullname, username = @Username, password = @Password, phone_number = @PhoneNumber, gender = @Gender, address = @Address, dob = @Dob, role = @Role WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Fullname", employee.Fullname ?? "");
        cmd.Parameters.AddWithValue("@Username", employee.Username ?? "");
        cmd.Parameters.AddWithValue("@Password", employee.Password ?? "");
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)employee.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Gender", (object?)employee.Gender ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Address", (object?)employee.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Dob", (object?)employee.Dob ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Role", employee.Role ?? "");
        cmd.Parameters.AddWithValue("@Id", employee.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM employee WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Employee Map(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Fullname = r.GetString(1),
        Username = r.GetString(2),
        Password = r.GetString(3),
        PhoneNumber = r.IsDBNull(4) ? null : r.GetString(4),
        Gender = r.IsDBNull(5) ? null : r.GetString(5),
        Address = r.IsDBNull(6) ? null : r.GetString(6),
        Dob = r.IsDBNull(7) ? null : r.GetString(7),
        Role = r.GetString(8)
    };
}
