using System;
using Common;
using EasyNetQ;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitApp.Infrastructure;
using WebApp.Infrastructure;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApp", Version = "v1"}); });

            var bus = RabbitHutch.CreateBus(Configuration["Rabbit:ConnectionString"]);
            services.AddSingleton(bus);

            services.AddTransient<StartCalculationConsumerHandler>();

            services.AddSingleton<FibonachiRequestRabbitMqProducer>();
            services.AddSingleton<FibonachiRequestWebApiConsumer>();

            services.AddSingleton<IExternalSystemProducer<FibonachiRequest>>(x =>
            {
                return x.GetService<FibonachiRequestRabbitMqProducer>();
            });
            services.AddSingleton<IExternalSystemProducer<FibonachiResponse>>(x =>
            {
                return x.GetService<FibonachiRequestRabbitMqProducer>();
            });
            services.AddSingleton<IExternalSystemConsumer<FibonachiRequest>>(x =>
            {
                var consumer = x.GetService<FibonachiRequestWebApiConsumer>();
                var handler = x.GetService<StartCalculationConsumerHandler>();

                consumer.RegisterHandler(handler);

                return consumer;
            });

            services.AddSingleton<IExternalSystemConsumer<FibonachiResponse>>(x =>
            {
                return x.GetService<FibonachiRequestWebApiConsumer>();
            });

            services.AddTransient(typeof(IPipelineBehavior<FibonachiRequest, FibonachiResponse>),
                typeof(FibonachiResponseProducePipelineBehavior));

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}