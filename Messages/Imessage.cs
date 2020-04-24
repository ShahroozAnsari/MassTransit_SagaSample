using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
    public interface Imessage
    {
        Guid CorrelationId { get; set; }
        Order.Order Order { get; set; }
    }
}
