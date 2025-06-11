using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Web.DTOs
{
    public class CreateOrderRequestDto
    {
        [Required(ErrorMessage = "OrderId is required.")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [StringLength(255, ErrorMessage = "CustomerName cannot exceed 255 characters.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Items are required.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        [Required(ErrorMessage = "CreatedAt timestamp is required.")]
        public DateTime CreatedAt { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemDto
    {
        [Required(ErrorMessage = "ProductId is required for each item.")]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}