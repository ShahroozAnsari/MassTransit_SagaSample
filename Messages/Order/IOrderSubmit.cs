using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Order
{
   public interface IOrderSubmit : Imessage
    {
        Guid CorrelationId { get; set; }
        Order Order { get; set; }
    }
}
