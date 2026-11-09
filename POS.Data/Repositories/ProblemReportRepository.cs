using System.Data.Common;
using POS.Data;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class ProblemReportRepository : IProblemReportRepository
{
    private readonly IDbConnectionFactory _factory;

    public ProblemReportRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ProblemReport>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<ProblemReport>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT id, title, description, category, priority, status, reported_by, reported_at, resolution, resolved_at FROM problem_reports ORDER BY reported_at DESC LIMIT 500");
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
            list.Add(new ProblemReport
            {
                Id = reader.GetInt64(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Category = reader.GetString(3),
                Priority = reader.GetString(4),
                Status = reader.GetString(5),
                ReportedBy = reader.GetString(6),
                ReportedAt = reader.ReadDateTime(7),
                Resolution = reader.IsDBNull(8) ? null : reader.GetString(8),
                ResolvedAt = reader.IsDBNull(9) ? null : reader.ReadDateTime(9)
            });
        return list;
    }

    public async Task<long> InsertAsync(ProblemReport report, CancellationToken ct)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "INSERT INTO problem_reports (title, description, category, priority, status, reported_by, reported_at, resolution, resolved_at) VALUES (@Title, @Description, @Category, @Priority, @Status, @ReportedBy, @ReportedAt, @Resolution, @ResolvedAt) RETURNING id");
        cmd.Parameters.AddWithValue("@Title", report.Title);
        cmd.Parameters.AddWithValue("@Description", report.Description);
        cmd.Parameters.AddWithValue("@Category", report.Category);
        cmd.Parameters.AddWithValue("@Priority", report.Priority);
        cmd.Parameters.AddWithValue("@Status", report.Status);
        cmd.Parameters.AddWithValue("@ReportedBy", report.ReportedBy);
        cmd.Parameters.AddWithValue("@ReportedAt", report.ReportedAt);
        cmd.Parameters.AddWithValue("@Resolution", (object?)report.Resolution ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ResolvedAt", (object?)report.ResolvedAt ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return Convert.ToInt64(result);
    }

    public async Task UpdateStatusAsync(long id, string status, string? resolution, CancellationToken ct)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "UPDATE problem_reports SET status = @Status, resolution = @Resolution, resolved_at = @ResolvedAt WHERE id = @Id");
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@Resolution", (object?)resolution ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ResolvedAt", status == "Resolved" ? DateTime.UtcNow : DBNull.Value);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }
}
