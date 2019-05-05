using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Repositories;
using Shop.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogItemRepository _catalogItemRepository;

        public CatalogController(ICatalogItemRepository catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }

        [HttpGet("items")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemViewModel>), (int)HttpStatusCode.OK)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _catalogItemRepository.GetCatalogItemsAsync();

            return Ok(items.Select(CatalogItemViewModel.FromEntity));
        }

        [HttpGet("items/{id}")]
        [ProducesResponseType(typeof(CatalogItemViewModel), (int)HttpStatusCode.OK)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetItem(int catalogItemId)
        {
            var item = await _catalogItemRepository.GetCatalogItemAsync(catalogItemId);

            return Ok(CatalogItemViewModel.FromEntity(item));
        }
    }
}