using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Warehouse.Items
{
   public interface IAllocatedInventory: Imessage
    {
        Guid CorrelationId { get; set; }
        Order.Order Order { get; set; }
    }
}
