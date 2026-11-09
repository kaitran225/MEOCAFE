using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IWasteRepository
{
    Task<IEnumerable<IngredientWaste>> GetAllAsync(CancellationToken ct);
    Task<long> InsertAsync(IngredientWaste entry, CancellationToken ct);
}
