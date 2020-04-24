using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Binders;
using MassTransit;
using Messages;
using Messages.Order;
using Messages.Warehouse.Items;

namespace MassTransitSample.Sagas.OrderSagaMachine
{
    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            this.State(() => this.Submit);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetOrderSubmitHandler());
            this.During(OrderCreate, this.SetOrderCreatedHandler());
            this.During(AllocateInventory, this.SetAllocatedHandler(), SetAllocateFaultHandler());
            SetCompletedWhenFinalized();

        }
        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.OrderSubmitEvent, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => Guid.NewGuid()));
            this.Event(() => this.OrderCreatedEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.AllocatedInventoryEvent, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.FaultAllocateInventoryEvent, x => x.CorrelateById(c => c.Message.Message.CorrelationId));
        }
        private EventActivityBinder<OrderState, IOrderSubmit> SetOrderSubmitHandler() =>
            When(OrderSubmitEvent)
                .Then(c =>
                {
                    c.Instance.CorrelationId = c.Data.CorrelationId;
                    c.Instance.Order = c.Data.Order;
                    c.Instance.SubmitDate = DateTime.Now;
                    c.Instance.Updated = DateTime.Now;
                })
                .ThenAsync(c =>
                    this.SendCommand<IOrderCreate>("rabbitmq://localhost/Messages.Order:IOrderCreate", c))
                .TransitionTo(OrderCreate);
        private EventActivityBinder<OrderState, IOrderCreated> SetOrderCreatedHandler() =>
            When(OrderCreatedEvent)
                .Then(c =>
                {
                    c.Instance.CorrelationId = c.Data.CorrelationId;
                    c.Instance.Order = c.Data.Order;
                    c.Instance.Updated = DateTime.Now;
                })
                .ThenAsync(c =>
                    this.SendCommand<IAllocateInventory>("rabbitmq://localhost/Messages.Warehouse.Items:IAllocateInventory", c))
                .TransitionTo(AllocateInventory);
        private EventActivityBinder<OrderState, IAllocatedInventory> SetAllocatedHandler() =>
            When(AllocatedInventoryEvent)
                .Then(c =>
                {
                    c.Instance.CorrelationId = c.Data.CorrelationId;
                    c.Instance.Order = c.Data.Order;
                    c.Instance.Updated = DateTime.Now;
                })
                .TransitionTo(Completed)
                .Finalize();

        private EventActivityBinder<OrderState, Fault<IAllocateInventory>> SetAllocateFaultHandler() =>
            When(FaultAllocateInventoryEvent)
                .Then(c =>
                {
                    c.Instance.FaultReason = c.Data.Exceptions[0].Message;
                })
                .ThenAsync(c => this.SendFaultCommand<IOrderDelete>("rabbitmq://localhost//Messages.Order:IOrderDelete", c))
                .TransitionTo(Faulted);
        public State Submit { get; private set; }
        public State OrderCreate { get; private set; }
        public State AllocateInventory { get; private set; }
        public State Faulted { get; private set; }
        public State Completed { get; private set; }

        public Event<IOrderSubmit> OrderSubmitEvent { get;  set; }
        public Event<IOrderCreated> OrderCreatedEvent { get;  set; }
        public Event<IAllocatedInventory> AllocatedInventoryEvent { get;  set; }
        public Event<Fault<IAllocateInventory>> FaultAllocateInventoryEvent { get; set; }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<OrderState, Imessage> context)
            where TCommand : class
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(endpointKey));
            await sendEndpoint.Send<TCommand>(new
            {
                context.Data.CorrelationId, 
                context.Data.Order
            });
        }
        private async Task SendFaultCommand<TCommand>(string endpointKey, BehaviorContext<OrderState, Fault<Imessage>> context)
            where TCommand : class
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(endpointKey));
            await sendEndpoint.Send<TCommand>(new
            {
                context.Data.Message.CorrelationId,
                context.Data.Message.Order
            });
        }
    }
}
