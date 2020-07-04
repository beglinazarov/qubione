
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Models;
using e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalog;
        private readonly IBasketService _basket;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalog = catalogService;
            _basket = basketService;
        }

        // Method for GRPC test
        [HttpPost]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddBasketItemAsync([FromBody] AddBasketItemRequest data)
        {
            if (data == null || data.Quantity == 0)
            {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            var item = await _catalog.GetCatalogItemAsync(data.CatalogItemId);

            //item.PictureUri = 

            // Step 2: Get current basket status
            var currentBasket = (await _basket.GetById(data.BasketId)) ?? new BasketData(data.BasketId);
            // Step 3: Search if exist product into basket
            var product = currentBasket.Items.SingleOrDefault(i => i.ProductId == item.Id);
            if (product != null)
            {
                // Step 4: Update quantity for product
                product.Quantity += data.Quantity;
            }
            else
            {
                // Step 4: Merge current status with new product
                currentBasket.Items.Add(new BasketDataItem()
                {
                    UnitPrice = item.Price,
                    PictureUrl = item.PictureUri,
                    ProductId = item.Id,
                    ProductName = item.Name,
                    Quantity = data.Quantity,
                    Id = Guid.NewGuid().ToString()
                });
            }

            // Step 5: Update basket
            await _basket.UpdateAsync(currentBasket);

            return Ok();
        }
    }
}