using e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Models;
using System.Threading.Tasks;

namespace e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Services
{
    public interface IBasketService
    {
        Task<BasketData> GetById(string id);

        Task UpdateAsync(BasketData currentBasket);
    }
}
