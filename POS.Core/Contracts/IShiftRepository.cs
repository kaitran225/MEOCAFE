namespace POS.Core.Contracts;

public interface IShiftRepository
{
    Task SetRegisterShiftAsync(string username, DateTime day, int shift, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<int>> GetRegisterShiftAsync(string username, DateTime day, CancellationToken cancellationToken = default);
    Task DeleteRegisterShiftAsync(DateTime day, int shift, CancellationToken cancellationToken = default);
}
