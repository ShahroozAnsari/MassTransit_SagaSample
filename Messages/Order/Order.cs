using System;
using System.Collections.Generic;

namespace Messages.Order
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
