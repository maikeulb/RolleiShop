using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using RolleiShop.Data.Context;
using RolleiShop.Entities;
using RolleiShop.Services.Interfaces;

namespace RolleiShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrderAsync (int cartId)
        {
            var cart = await _context.Carts.FindAsync (cartId);
            EnsureArg.IsNotNull (cart, nameof (cart));

            var items = new List<OrderItem> ();
            foreach (var item in cart.Items)
            {
                var catalogItem = await _context.CatalogItems.FindAsync (item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered (catalogItem.Id, catalogItem.Name, catalogItem.ImageUrl);
                var orderItem = new OrderItem (itemOrdered, item.UnitPrice, item.Quantity);
                items.Add (orderItem);
            }
            var order = Order.Create (cart.BuyerId, items);

            _context.Orders.Add (order);
            await _context.SaveChangesAsync ();
        }
    }
}