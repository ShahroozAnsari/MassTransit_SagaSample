using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MassTransitSample.Sagas.OrderSagaMachine
{
    public class OrderStateDbContext : SagaDbContext
    {
        public OrderStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OrderStateMap(); }
        }
    }
}
