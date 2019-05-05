using Shop.Domain.CartAggregate;
using System.Linq;

namespace Shop.Web.ViewModels
{
    public class CustomerCartViewModel
    {
        public string Id { get; set; }

        public CustomerCartItemViewModel[] Items { get; set; }

        public decimal Total => Items?.Sum(x => x.UnitPrice * x.Quantity) ?? 0m;

        public static CustomerCartViewModel FromEntity(Cart entity) => new CustomerCartViewModel
        {
            Id = entity.Id,
            Items = entity.Items.Select(CustomerCartItemViewModel.FromEntity).ToArray()
        };
    }
}