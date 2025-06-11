using OrdersApi.Application.DTOs;
using OrdersApi.Application.Interfaces;
using OrdersApi.Domain.Entities;
using OrdersApi.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace OrdersApi.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                {
                    _logger.LogWarning("Attempted to create an order with no items for customer {CustomerName}", request.CustomerName);
                    throw new ArgumentException("Order must contain at least one item."); 
                }

                if (request.Quantity < 0)
                {
                    _logger.LogWarning("Attempted to create an order with a negative total quantity for customer {CustomerName}", request.CustomerName);
                    throw new ArgumentException("Total order quantity cannot be negative.");
                }

                var order = new Order
                {
                    Id = request.OrderId,
                    CustomerName = request.CustomerName,
                    CreatedAt = request.CreatedAt,
                    Items = request.Items.Select(itemDto => new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity
                    }).ToList()
                };

                foreach (var item in order.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        _logger.LogWarning("Invalid quantity ({Quantity}) for product item {ProductId}, customer {CustomerName}", item.Quantity, item.ProductId, request.CustomerName);
                        throw new ArgumentException($"Quantity for product item {item.ProductId} must be greater than zero.");
                    }
                }

                await _orderRepository.AddAsync(order);

                _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerName}", order.Id, order.CustomerName);

                return new CreateOrderResponseDto { OrderId = order.Id };
            }
            catch (ArgumentException ex) 
            {
                _logger.LogError(ex, "Validation error occurred while creating order: {Message}", ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating order for customer {CustomerName}.", request.CustomerName);
                throw new ApplicationException("Failed to create order due to an internal error.", ex);
            }
        }
    }
}
