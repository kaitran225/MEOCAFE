using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IAnalyticsRepository
{
    Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetAverageRevenuePerTransactionAsync(CancellationToken cancellationToken = default);
    Task<KeyValuePair<string, float>> GetTopSellingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<float>> GetTotalSaleByWeekAsync(DateTime dateTime, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<float>> GetTotalSaleInMonthAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<float>> GetTotalSaleByYearAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByWeekAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByMonthAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeyValuePair<string, float>>> GetTopSellingByYearAsync(CancellationToken cancellationToken = default);
    Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>Revenue per hour (24 entries, index = hour 0-23) for the given day.</summary>
    Task<IReadOnlyList<float>> GetSalesByHourAsync(DateTime date, CancellationToken cancellationToken = default);
}
