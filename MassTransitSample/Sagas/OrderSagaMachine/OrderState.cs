using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit.Saga;
using Messages.Order;

namespace MassTransitSample.Sagas.OrderSagaMachine
{
    public class OrderState : SagaStateMachineInstance, ISaga
    {
        public Guid CorrelationId { get; set; }

        public DateTime? SubmitDate { get; set; }
        public DateTime? Updated { get; set; }
        public string CurrentState { get; set; }

        public  Order  Order{ get; set; }

        public string FaultReason { get; set; }


        public byte[] RowVersion { get; set; }
    }
}
