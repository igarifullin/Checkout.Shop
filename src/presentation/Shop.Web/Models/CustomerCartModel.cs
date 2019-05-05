using Shop.Domain.Dto;
using System.Linq;

namespace Shop.Web.Models
{
    public class CustomerCartModel
    {
        public string Id { get; set; }

        public CustomerCartItemModel[] Items { get; set; }

        public CustomerCartDto ToDto() => new CustomerCartDto(Id)
        {
            Items = Items.Select(x => x.ToDto()).ToArray()
        };
    }
}