using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransitSample.Sagas.OrderSagaMachine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
   
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic; 
                        
                            r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                            {
                                builder.UseSqlServer("Server=.;Database=SagaStates;Trusted_Connection=True;", m =>
                                {
                                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                    m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                                    
                                });
                            });
                    });

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {

                    cfg.Host("rabbitmq://localhost");
               
                    cfg.ConfigureEndpoints(provider);

                }));
            });
            //services.AddMassTransitHostedService();
            services.AddHostedService<MassTransitConsoleHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
