using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Messages.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Order.Models;

namespace Order.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderContext _context;
        private readonly IPublishEndpoint _publishEndpoint;


        public OrderController(Models.OrderContext context, IPublishEndpoint _publishEndpoint)
        {
            _context = context;
            this._publishEndpoint = _publishEndpoint;
        }
        [HttpGet]
        public List<Models.Order> Get()
        {
            return _context.Orders
                .Include(a => a.OrderItems)
                .ToList();
        }
        [HttpPost]
        public async void Post(Models.Order Order)
        {
            var or = new Messages.Order.Order()
            {
                Date = Order.Date,
                OrderItems =
                    Order.OrderItems.Select(a => new Messages.Order.OrderItem()
                    {
                        ItemId = a.ItemId,
                        Quantity = a.Quantity
                    }).ToList()
            };
            await _publishEndpoint.Publish<IOrderSubmit>(new
            {
                CorrelationId = Guid.NewGuid(),
                Order = or
            });
        }
    }
}