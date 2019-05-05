using System.Collections.Generic;

namespace Shop.Client.Responses
{
    public class CatalogItemsResponse
    {
        public IEnumerable<CatalogItem> Items { get; set; }
    }

    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUrl { get; set; }
    }
}