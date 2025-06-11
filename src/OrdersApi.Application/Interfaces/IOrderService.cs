using OrdersApi.Application.DTOs;

namespace OrdersApi.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
    }
}