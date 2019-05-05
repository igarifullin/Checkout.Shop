using Shop.Domain.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Web.Models
{
    public class CustomerCartItemModel : IValidatableObject
    {
        public int Quantity { get; set; }

        public int CatalogItemId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Quantity < 0)
            {
                yield return new ValidationResult($"Quantity must be more or equal than 0", new[] { nameof(Quantity) });
            }

            if (CatalogItemId < 0)
            {
                yield return new ValidationResult($"Catalog item id must be more or equal than 0", new[] { nameof(CatalogItemId) });
            }
        }

        public CustomerCartItemDto ToDto()
        {
            return new CustomerCartItemDto(CatalogItemId, Quantity);
        }
    }
}