namespace Shop.Domain.Dto
{
    public class CustomerCartDto
    {
        public string Id { get; set; }

        public CustomerCartItemDto[] Items { get; set; }

        public CustomerCartDto(string id)
        {
            this.Id = id;
        }
    }
}