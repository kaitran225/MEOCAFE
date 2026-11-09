using System.Data.Common;
using POS.Data;
using POS.Core.Contracts;

namespace POS.Data.Repositories;

public sealed class ShiftRepository : IShiftRepository
{
    private readonly IDbConnectionFactory _factory;

    public ShiftRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task SetRegisterShiftAsync(string username, DateTime day, int shift, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var checkCmd = conn.CreateTextCommand("SELECT COUNT(*) FROM shifts WHERE _day = @Day AND shift = @Shift");
        checkCmd.Parameters.AddWithValue("@Day", day.Date.ToString("yyyy-MM-dd"));
        checkCmd.Parameters.AddWithValue("@Shift", shift);
        var count = (long)(await checkCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0L);
        var uname = username + ",";
        if (count == 0)
        {
            await using var insertCmd = conn.CreateTextCommand("INSERT INTO shifts (_day, shift, employees_username) VALUES (@Day, @Shift, @Username)");
            insertCmd.Parameters.AddWithValue("@Day", day.Date.ToString("yyyy-MM-dd"));
            insertCmd.Parameters.AddWithValue("@Shift", shift);
            insertCmd.Parameters.AddWithValue("@Username", uname);
            await insertCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await using var updateCmd = conn.CreateTextCommand("UPDATE shifts SET employees_username = employees_username || @Username WHERE _day = @Day AND shift = @Shift AND employees_username NOT LIKE @Pattern");
            updateCmd.Parameters.AddWithValue("@Day", day.Date.ToString("yyyy-MM-dd"));
            updateCmd.Parameters.AddWithValue("@Shift", shift);
            updateCmd.Parameters.AddWithValue("@Username", uname);
            updateCmd.Parameters.AddWithValue("@Pattern", "%" + username + "%");
            await updateCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyList<int>> GetRegisterShiftAsync(string username, DateTime day, CancellationToken cancellationToken = default)
    {
        var list = new List<int>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT shift FROM shifts WHERE _day >= @StartDay AND _day <= date(@StartDay, '+6 days') AND employees_username LIKE @Pattern");
        cmd.Parameters.AddWithValue("@StartDay", day.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@Pattern", "%" + username + "%");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(reader.GetInt32(0));
        return list;
    }

    public async Task DeleteRegisterShiftAsync(DateTime day, int shift, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("UPDATE shifts SET employees_username = '' WHERE _day = @Day AND shift = @Shift");
        cmd.Parameters.AddWithValue("@Day", day.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@Shift", shift);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
