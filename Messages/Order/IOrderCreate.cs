using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Order
{
    public interface IOrderCreate : Imessage
    {
        Guid CorrelationId { get; set; }
        Order Order { get; set; }
    }

    public interface IOrderCreated : Imessage
    {
        Guid CorrelationId { get; set; }
        Order Order { get; set; }

    }
}
