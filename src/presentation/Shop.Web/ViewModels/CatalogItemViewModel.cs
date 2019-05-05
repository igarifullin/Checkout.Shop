using Shop.Domain;

namespace Shop.Web.ViewModels
{
    public class CatalogItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUrl { get; set; }

        public static CatalogItemViewModel FromEntity(CatalogItem entity) => new CatalogItemViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            PictureUrl = entity.PictureUrl
        };
    }
}