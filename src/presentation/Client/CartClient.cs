using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shop.Client.Requests;
using Shop.Client.Responses;

namespace Shop.Client
{
    public class CartClient : ICartClient
    {
        private readonly HttpClient _client;
        private readonly Uri _apiUri;

        public CartClient(HttpClient client,
            Uri apiUri)
        {
            _client = client;
            _apiUri = apiUri;
        }

        public async Task<CartResponse> AddProductAsync(string id, int catalogItemId, int quantity)
        {
            var cartResponse = await GetCartAsync(id);
            var items = cartResponse.Items
                .Select(x => new CartItemRequest { CatalogItemId = x.CatalogItemId, Quantity = x.Quantity })
                .ToList();
            
            items.Add(new CartItemRequest { CatalogItemId = catalogItemId, Quantity = quantity });

            return await UpdateCartAsync(id, items);
        }

        public async Task ClearCartAsync(string id)
        {
            var response = await _client.DeleteAsync(new Uri(_apiUri, $"/api/cart/{id}"));
            response.EnsureSuccessStatusCode();
        }

        public Task<CartResponse> CreateCartAsync(string id)
        {
            return GetCartAsync(id);
        }

        public async Task<CartResponse> GetCartAsync(string id)
        {
            var response = await _client.GetAsync(new Uri(_apiUri, $"/api/cart/{id}"));
            response.EnsureSuccessStatusCode();

            return await DeserializeResponse<CartResponse>(response);
        }

        public Task<CartResponse> RemoveProductAsync(string id, int catalogItemId)
        {
            return SetProductsQuantityAsync(id, catalogItemId, 0);
        }

        public async Task<CartResponse> SetProductsQuantityAsync(string id, int catalogItemId, int quantity)
        {
            var cartResponse = await GetCartAsync(id);
            var items = cartResponse.Items
                .Select(x => new CartItemRequest { CatalogItemId = x.CatalogItemId, Quantity = x.Quantity })
                .ToList();

            var item = items.FirstOrDefault(x => x.CatalogItemId == catalogItemId);
            if (item == null)
            {
                return cartResponse;
            }
            item.Quantity = quantity;
            if (item.Quantity <= 0)
            {
                items.Remove(item);
            }

            return await UpdateCartAsync(id, items);
        }

        private async Task<CartResponse> UpdateCartAsync(string id, IEnumerable<CartItemRequest> items)
        {
            var request = new CartUpdateRequest
            {
                Id = id,
                Items = items.ToArray()
            };

            var response = await _client.PostAsync(new Uri(_apiUri, $"/api/cart/{id}"), GetJsonHttpContent(request));
            response.EnsureSuccessStatusCode();

            return await DeserializeResponse<CartResponse>(response);
        }

        private static HttpContent GetJsonHttpContent(object obj) => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response) where T : class, new()
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}