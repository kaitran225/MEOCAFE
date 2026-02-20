using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IDbConnectionFactory _factory;

    public AnalyticsRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(SUM(total_price), 0) FROM orders WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE)", conn);
        var o = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToDecimal(o ?? 0);
    }

    public async Task<decimal> GetAverageRevenuePerTransactionAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(AVG(total_price), 0) FROM orders WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE)", conn);
        var o = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToDecimal(o ?? 0);
    }

    public async Task<KeyValuePair<string, float>> GetTopSellingAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT m.name, COALESCE(SUM(od.quantity * od.price), 0) AS total FROM order_details od JOIN menu_items m ON od.menu_item_id = m.id GROUP BY m.name ORDER BY total DESC LIMIT 1", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new KeyValuePair<string, float>(reader.GetString(0), (float)reader.GetDecimal(1));
        return new KeyValuePair<string, float>("", 0f);
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleByWeekAsync(DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(SUM(o.total_price), 0) FROM orders o WHERE DATE(o.created_at) >= @Start AND DATE(o.created_at) <= @End GROUP BY DATE(o.created_at) ORDER BY DATE(o.created_at)", conn);
        var start = dateTime.Date.AddDays(-(int)dateTime.DayOfWeek + 1);
        var end = start.AddDays(6);
        cmd.Parameters.AddWithValue("@Start", start);
        cmd.Parameters.AddWithValue("@End", end);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(0));
        return list;
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleInMonthAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT EXTRACT(MONTH FROM created_at), COALESCE(SUM(total_price), 0) FROM orders WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE) GROUP BY EXTRACT(MONTH FROM created_at) ORDER BY 1", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(1));
        return list;
    }

    public async Task<IReadOnlyList<float>> GetTotalSaleByYearAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<float>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT EXTRACT(YEAR FROM created_at), COALESCE(SUM(total_price), 0) FROM orders GROUP BY EXTRACT(YEAR FROM created_at) ORDER BY 1", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add((float)reader.GetDecimal(1));
        return list;
    }

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByWeekAsync(CancellationToken cancellationToken = default)
    {
        return await GetTopSellingByPeriodAsync("week", cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByMonthAsync(CancellationToken cancellationToken = default)
    {
        return await GetTopSellingByPeriodAsync("month", cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByYearAsync(CancellationToken cancellationToken = default)
    {
        return await GetTopSellingByPeriodAsync("year", cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByPeriodAsync(string period, CancellationToken cancellationToken)
    {
        var list = new List<KeyValuePair<string, float>>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var whereClause = period switch
        {
            "week" => "o.created_at >= CURRENT_DATE - INTERVAL '7 days'",
            "month" => "EXTRACT(MONTH FROM o.created_at) = EXTRACT(MONTH FROM CURRENT_DATE) AND EXTRACT(YEAR FROM o.created_at) = EXTRACT(YEAR FROM CURRENT_DATE)",
            _ => "EXTRACT(YEAR FROM o.created_at) = EXTRACT(YEAR FROM CURRENT_DATE)"
        };
        await using var cmd = new NpgsqlCommand(
            $"SELECT m.name, COALESCE(SUM(od.quantity * od.price), 0) AS total FROM order_details od JOIN menu_items m ON od.menu_item_id = m.id JOIN orders o ON od.order_id = o.id WHERE {whereClause} GROUP BY m.name ORDER BY total DESC LIMIT 10", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new KeyValuePair<string, float>(reader.GetString(0), (float)reader.GetDecimal(1)));
        return list;
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT COALESCE(SUM(total_price), 0), COUNT(*), COALESCE(AVG(total_price), 0) FROM orders WHERE created_at >= @From AND created_at <= @To", conn);
        cmd.Parameters.AddWithValue("@From", from);
        cmd.Parameters.AddWithValue("@To", to);
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
