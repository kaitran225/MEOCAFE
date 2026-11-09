using System.Data.Common;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IDbConnectionFactory _factory;

    public AnalyticsRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT COALESCE(SUM(total_price), 0) FROM orders WHERE strftime('%Y', created_at) = strftime('%Y', 'now', 'localtime')");
        var o = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToDecimal(o ?? 0);
    }

    public async Task<decimal> GetAverageRevenuePerTransactionAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT COALESCE(AVG(total_price), 0) FROM orders WHERE strftime('%Y', created_at) = strftime('%Y', 'now', 'localtime')");
        var o = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToDecimal(o ?? 0);
    }

    public async Task<KeyValuePair<string, float>> GetTopSellingAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT m.name, COALESCE(SUM(od.quantity * od.price), 0) AS total FROM order_details od JOIN menu_items m ON od.menu_item_id = m.id GROUP BY m.name ORDER BY total DESC LIMIT 1");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new KeyValuePair<string, float>(reader.GetString(0), (float)reader.GetDecimal(1));
        return new KeyValuePair<string, float>("", 0f);
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleByWeekAsync(DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT COALESCE(SUM(o.total_price), 0) FROM orders o WHERE date(o.created_at) >= date(@Start) AND date(o.created_at) <= date(@End) GROUP BY date(o.created_at) ORDER BY date(o.created_at)");
        var start = dateTime.Date.AddDays(-(int)dateTime.DayOfWeek + 1);
        var end = start.AddDays(6);
        cmd.Parameters.AddWithValue("@Start", start.ToString("O"));
        cmd.Parameters.AddWithValue("@End", end.ToString("O"));
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(0));
        return list;
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleInMonthAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT strftime('%m', created_at), COALESCE(SUM(total_price), 0) FROM orders WHERE strftime('%Y', created_at) = strftime('%Y', 'now', 'localtime') GROUP BY strftime('%m', created_at) ORDER BY 1");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(1));
        return list;
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleByYearAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT strftime('%Y', created_at), COALESCE(SUM(total_price), 0) FROM orders GROUP BY strftime('%Y', created_at) ORDER BY 1");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(1));
        return list;
    }

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByWeekAsync(CancellationToken cancellationToken = default)
        => await GetTopSellingByPeriodAsync("week", cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByMonthAsync(CancellationToken cancellationToken = default)
        => await GetTopSellingByPeriodAsync("month", cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByYearAsync(CancellationToken cancellationToken = default)
        => await GetTopSellingByPeriodAsync("year", cancellationToken).ConfigureAwait(false);

    private async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByPeriodAsync(string period, CancellationToken cancellationToken)
    {
        var list = new List<KeyValuePair<string, float>>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var whereClause = period switch
        {
            "week" => "date(o.created_at) >= date('now', '-7 days')",
            "month" => "strftime('%Y-%m', o.created_at) = strftime('%Y-%m', 'now', 'localtime')",
            _ => "strftime('%Y', o.created_at) = strftime('%Y', 'now', 'localtime')"
        };
        await using var cmd = conn.CreateTextCommand(
            $"SELECT m.name, COALESCE(SUM(od.quantity * od.price), 0) AS total FROM order_details od JOIN menu_items m ON od.menu_item_id = m.id JOIN orders o ON od.order_id = o.id WHERE {whereClause} GROUP BY m.name ORDER BY total DESC LIMIT 10");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new KeyValuePair<string, float>(reader.GetString(0), (float)reader.GetDecimal(1)));
        return list;
    }

    public async Task<IReadOnlyList<float>> GetSalesByHourAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var values = new float[24];
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT CAST(strftime('%H', created_at) AS INTEGER) AS h, COALESCE(SUM(total_price), 0) FROM orders WHERE date(created_at) = date(@Date) GROUP BY h");
        cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var hour = reader.GetInt32(0);
            if (hour is >= 0 and < 24)
                values[hour] = (float)reader.GetDecimal(1);
        }
        return values;
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT COALESCE(SUM(total_price), 0), COUNT(*), COALESCE(AVG(total_price), 0) FROM orders WHERE created_at >= @From AND created_at <= @To");
        // orders.created_at is written with datetime('now') => "yyyy-MM-dd HH:mm:ss".
        // Binding as round-trip ("O") strings puts a 'T' where the column has a space,
        // so lexicographic comparison silently excluded the whole range.
        cmd.Parameters.AddWithValue("@From", from.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@To", to.ToString("yyyy-MM-dd HH:mm:ss"));
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new SalesSummaryDto
            {
                TotalRevenue = reader.GetDecimal(0),
                OrderCount = reader.GetInt32(1),
                AverageTicket = reader.GetDecimal(2)
            };
        return new SalesSummaryDto();
    }
}
