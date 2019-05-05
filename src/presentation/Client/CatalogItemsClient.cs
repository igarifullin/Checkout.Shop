using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shop.Client.Responses;

namespace Shop.Client
{
    public class CatalogItemsClient : ICatalogItemsClient
    {
        private readonly HttpClient _client;
        private readonly Uri _apiUri;

        public CatalogItemsClient(HttpClient client,
            Uri apiUri)
        {
            _client = client;
            _apiUri = apiUri;
        }

        public async Task<CatalogItemsResponse> GetCatalogItemsAsync()
        {
            var response = await _client.GetAsync(new Uri(_apiUri, $"/api/catalog/items"));
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CatalogItemsResponse>(responseContent);
        }
    }
}