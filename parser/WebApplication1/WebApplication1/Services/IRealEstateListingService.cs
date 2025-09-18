using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IRealEstateListingService
{
    Task<IReadOnlyList<RealEstateListing>> GetListingsAsync(CancellationToken cancellationToken);
}
