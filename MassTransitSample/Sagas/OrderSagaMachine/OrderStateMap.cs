using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransitSample.Sagas.OrderSagaMachine
{
    public class OrderStateMap :
        SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            base.Configure(entity, model);
 
            // If using Optimistic concurrency, otherwise remove this property
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }

}
