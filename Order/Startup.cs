using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Definition;
using Messages.Order;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Order.Consumer;
using Order.Models;

namespace Order
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            OrderContext orderContext=new OrderContext();
            orderContext.Database.EnsureCreated();
            services.AddControllers();
            services.AddDbContext<OrderContext>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateOrderConsumer>();
                x.AddConsumer<DeleteOrderConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {

                    cfg.Host("rabbitmq://localhost");

                    cfg.ConfigureEndpoints(provider);

                }));
       
            });
            //services.AddMassTransitHostedService();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
            services.AddHostedService<MassTransitConsoleHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
