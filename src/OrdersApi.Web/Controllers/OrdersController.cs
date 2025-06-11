using OrdersApi.Application.Interfaces;
using OrdersApi.Web.DTOs; // Ensure correct DTO namespace
using Microsoft.AspNetCore.Mvc;

namespace OrdersApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateOrder request: {Errors}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var applicationRequest = new OrdersApi.Application.DTOs.CreateOrderRequestDto
                {
                    OrderId = request.OrderId,
                    CustomerName = request.CustomerName,
                    Items = request.Items.Select(item => new OrdersApi.Application.DTOs.OrderItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList(),
                    CreatedAt = request.CreatedAt,
                    Quantity = request.Quantity
                };

                var response = await _orderService.CreateOrderAsync(applicationRequest);

                _logger.LogInformation("Order creation request received and processed for OrderId: {OrderId}", response.OrderId);

                return CreatedAtAction(nameof(CreateOrder), new { id = response.OrderId }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Client-side or application layer validation failed, order creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error during order creation: {Message}", ex.Message);
                return StatusCode(500, new { message = "An internal application error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in CreateOrder endpoint.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}