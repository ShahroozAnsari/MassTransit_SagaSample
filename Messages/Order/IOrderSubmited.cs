using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Order
{
    public interface IOrderSubmitted : Imessage
    {
        Guid CorrelationId { get; set; }
        Order Order { get; set; }
    }
}
