using Xunit;
using Moq; 
using Microsoft.Extensions.Logging;
using OrdersApi.Application.Services;
using OrdersApi.Application.DTOs;
using OrdersApi.Infrastructure.Repositories;
using OrdersApi.Domain.Entities;

namespace OrdersApi.Tests.Application.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_mockOrderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidRequest_ReturnsCreatedOrderId()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();
            var request = new CreateOrderRequestDto
            {
                OrderId = orderId,
                CustomerName = "Test Customer",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = productId1, Quantity = 2 },
                    new OrderItemDto { ProductId = productId2, Quantity = 1 }
                },
                CreatedAt = DateTime.UtcNow,
                Quantity = 3
            };

            
            _mockOrderRepository.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

         
            var response = await _orderService.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(orderId, response.OrderId);

            // Verify that AddAsync was called exactly once with an Order object
            _mockOrderRepository.Verify(r => r.AddAsync(It.Is<Order>(o =>
                o.Id == orderId &&
                o.CustomerName == "Test Customer" &&
                o.Items.Count == 2 &&
                o.Items.Any(item => item.ProductId == productId1 && item.Quantity == 2) &&
                o.Items.Any(item => item.ProductId == productId2 && item.Quantity == 1)
            )), Times.Once);

            // Verify that LogInformation was called (optional, for logging verification)
            _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Order {orderId} created successfully")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_NoItems_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Items = new List<OrderItemDto>(), // No items
                CreatedAt = DateTime.UtcNow,
                Quantity = 0
            };

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _orderService.CreateOrderAsync(request));
            Assert.Equal("Order must contain at least one item.", exception.Message);

            // Verify that AddAsync was NOT called
            _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);

            // Verify that LogWarning was called
            _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Attempted to create an order with no items for customer Test Customer")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_RepositoryThrowsException_ThrowsApplicationException()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 } },
                CreatedAt = DateTime.UtcNow,
                Quantity = 1
            };

            // Simulate repository throwing an exception
            _mockOrderRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                                .ThrowsAsync(new InvalidOperationException("Simulated database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _orderService.CreateOrderAsync(request));
            Assert.Contains("Failed to create order due to an internal error.", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);

            // Verify that LogError was called
            _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred while creating order for customer Test Customer.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ZeroQuantity_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 0 } }, // Zero quantity
                CreatedAt = DateTime.UtcNow,
                Quantity = 0
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _orderService.CreateOrderAsync(request));
            // Check part of the string as ProductId changes dynamically
            Assert.Contains("Quantity for product item", exception.Message);
            Assert.Contains("must be greater than zero.", exception.Message);


            // Verify that AddAsync was NOT called
            _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);

            // Verify that LogWarning was called
            _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid quantity")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
