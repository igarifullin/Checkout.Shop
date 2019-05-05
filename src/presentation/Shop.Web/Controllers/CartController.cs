using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Services;
using Shop.Web.Models;
using Shop.Web.ViewModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartManager _cartManager;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ILogger<CartController> logger,
            ICartManager cartManager,
            IEventBus eventBus)
        {
            _logger = logger;
            _cartManager = cartManager;
            _eventBus = eventBus;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerCartViewModel), (int)HttpStatusCode.OK)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CustomerCartViewModel>> GetBasketByIdAsync(string id)
        {
            var basket = await _cartManager.GetCartAsync(id);

            return Ok(CustomerCartViewModel.FromEntity(basket));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerCartViewModel), (int)HttpStatusCode.OK)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CustomerCartViewModel>> UpdateBasketAsync([FromBody]CustomerCartModel request)
        {
            var cart = await _cartManager.UpdateCartAsync(request.ToDto());

            return Ok(CustomerCartViewModel.FromEntity(cart));
        }

        [Route("{id}/checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> CheckoutAsync(string id, [FromBody]CartCheckoutModel cartCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            cartCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
                guid : cartCheckout.RequestId;

            var cart = await _cartManager.GetCartAsync(id);

            if (cart == null || cart.Items.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                var eventMessage = new object();

                _logger.LogDebug("----- Publishing integration event");

                _eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration UserCheckoutCart event");

                throw;
            }

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> DeleteBasketByIdAsync(string id)
        {
            await _cartManager.DeleteCartAsync(id);
            return NoContent();
        }
    }
}