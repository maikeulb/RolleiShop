using RolleiShop.Models.Entities.Order;
using RolleiShop.Models.Entities;
using System.Threading.Tasks;

namespace RolleiShop.Models.Interfaces
{
    public interface IOrderRepository : IRepository<Order>, IAsyncRepository<Order>
    {
        Order GetByIdWithItems(int id);
        Task<Order> GetByIdWithItemsAsync(int id);
    }
}