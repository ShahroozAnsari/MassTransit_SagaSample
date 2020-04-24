using System;
using System.Collections.Generic;
using System.Text;
using Messages.Order;

namespace Messages.Warehouse.Items
{
   public interface IAllocateInventory : Imessage
    {
        Guid CorrelationId { get; set; } 
        Order.Order Order { get; set; }
    }
}
