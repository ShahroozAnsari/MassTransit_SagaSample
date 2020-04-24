using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Messages.Warehouse.Items;
using Warehouse.Models;

namespace Warehouse.Consumers
{
    public class AllocateInventoryConsumer : IConsumer<IAllocateInventory>
    {
        private readonly WarehouseContext _context;

        public AllocateInventoryConsumer(WarehouseContext context)
        {
            _context = context;
        }
        public Task Consume(ConsumeContext<IAllocateInventory> context)
        {
            foreach (var order in context.Message.Order.OrderItems)
            {
                var item = _context.Items.Single(a => a.Id == order.ItemId);

                if (item.Count < order.Quantity)
                    throw new Exception("مقدار کافی نیست");
                item.Count-= order.Quantity;
            }

            _context.SaveChanges();
            return context.Publish<IAllocatedInventory>(new
            {
                context.Message.CorrelationId,
                context.Message.Order 
            });
        }
    }
}
