
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CatalogApi;
using e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator;
using e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Services;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CatalogApi.Catalog;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly UrlsConfig _urls;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }
        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await GrpcCallerService.CallService(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemById(id), async channel =>
            {
                var client = new CatalogClient(channel);
                var request = new CatalogItemRequest { Id = id };
                var response = await client.GetItemByIdAsync(request);
                return MapToCatalogItemResponse(response);
            });
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            return await GrpcCallerService.CallService(_urls.GrpcCatalog, async channel =>
            {
                var client = new CatalogClient(channel);
                var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
                var response = await client.GetItemsByIdsAsync(request);
                return response.Data.Select(this.MapToCatalogItemResponse);
            });
        }

        private CatalogItem MapToCatalogItemResponse(CatalogItemResponse catalogItemResponse)
        {
            return new CatalogItem
            {
                Id = catalogItemResponse.Id,
                Name = catalogItemResponse.Name,
                PictureUri = catalogItemResponse.PictureUri,
                Price = (decimal)catalogItemResponse.Price
            };
        }
    }
}