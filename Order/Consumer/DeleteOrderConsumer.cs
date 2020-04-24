using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Messages.Order;
using Order.Models;

namespace Order.Consumer
{
    public class DeleteOrderConsumer:IConsumer<IOrderDelete>
    {
        private readonly OrderContext _dbContext;

        public DeleteOrderConsumer(Models.OrderContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task Consume(ConsumeContext<IOrderDelete> context)
        {
          var t=  _dbContext.Orders.Single(a => a.Id == context.Message.Order.Id);
          _dbContext.Remove(t);
          return _dbContext.SaveChangesAsync();
        }
    }
}
