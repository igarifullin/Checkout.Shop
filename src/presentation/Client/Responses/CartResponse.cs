namespace Shop.Client.Responses
{
    public class CartResponse
    {
        public string Id { get; set; }

        public CartItem[] Items { get; set; }

        public decimal Total { get; set; }
    }

    public class CartItem
    {
        public int CatalogItemId { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }

        public decimal UnitPrice { get; set; }
    }
}