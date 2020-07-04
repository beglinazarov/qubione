using System.Collections.Generic;
using System.Threading.Tasks;

namespace e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Services
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
    }

}