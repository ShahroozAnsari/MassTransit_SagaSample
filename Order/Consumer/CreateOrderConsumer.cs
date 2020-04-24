using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Messages.Order;
using Microsoft.EntityFrameworkCore;
using Order.Models;

namespace Order.Consumer
{
    public class CreateOrderConsumer : IConsumer<IOrderCreate>
    {
        private readonly OrderContext _dbContext;

        public CreateOrderConsumer(Models.OrderContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task Consume(ConsumeContext<IOrderCreate> context)
        {
            var order = new Models.Order()
            {
                Date = context.Message.Order.Date,
                Id = context.Message.Order.Id,
            };


            _dbContext.Add(order);
            _dbContext.SaveChanges();
            context.Message.Order.Id = order.Id;
            return context.Publish<IOrderCreated>(context.Message);
        }
    }
}
