using Npgsql;
using POS.Core.Contracts;

namespace POS.Data.Repositories;

public sealed class ShiftRepository : IShiftRepository
{
    private readonly IDbConnectionFactory _factory;

    public ShiftRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task SetRegisterShiftAsync(string username, DateTime day, int shift, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM shifts WHERE _day = @Day AND shift = @Shift", conn);
        checkCmd.Parameters.AddWithValue("@Day", day.Date);
        checkCmd.Parameters.AddWithValue("@Shift", shift);
        var count = (long)(await checkCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0L);
        var uname = username + ",";
        if (count == 0)
        {
            await using var insertCmd = new NpgsqlCommand("INSERT INTO shifts (_day, shift, employees_username) VALUES (@Day, @Shift, @Username)", conn);
            insertCmd.Parameters.AddWithValue("@Day", day.Date);
            insertCmd.Parameters.AddWithValue("@Shift", shift);
            insertCmd.Parameters.AddWithValue("@Username", uname);
            await insertCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await using var updateCmd = new NpgsqlCommand("UPDATE shifts SET employees_username = employees_username || @Username WHERE _day = @Day AND shift = @Shift AND employees_username NOT LIKE @Pattern", conn);
            updateCmd.Parameters.AddWithValue("@Day", day.Date);
            updateCmd.Parameters.AddWithValue("@Shift", shift);
            updateCmd.Parameters.AddWithValue("@Username", uname);
            updateCmd.Parameters.AddWithValue("@Pattern", "%" + username + "%");
            await updateCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyList<int>> GetRegisterShiftAsync(string username, DateTime day, CancellationToken cancellationToken = default)
    {
        var list = new List<int>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT shift FROM shifts WHERE _day >= @StartDay AND _day <= @StartDay + INTERVAL '6 days' AND employees_username LIKE @Pattern", conn);
        cmd.Parameters.AddWithValue("@StartDay", day.Date);
        cmd.Parameters.AddWithValue("@Pattern", "%" + username + "%");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(reader.GetInt32(0));
        return list;
    }

    public async Task DeleteRegisterShiftAsync(DateTime day, int shift, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE shifts SET employees_username = '' WHERE _day = @Day AND shift = @Shift", conn);
        cmd.Parameters.AddWithValue("@Day", day.Date);
        cmd.Parameters.AddWithValue("@Shift", shift);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
