namespace OrdersApi.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } 
        public Guid OrderId { get; set; } 
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Order? Order { get; set; } 
    }
}