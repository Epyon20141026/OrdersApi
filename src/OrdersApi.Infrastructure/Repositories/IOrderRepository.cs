using OrdersApi.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace OrdersApi.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);

    }
}