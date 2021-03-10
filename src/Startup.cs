using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using BoxService.Consumers;
using BoxService.Daemons;
using BoxService.Services;

namespace BoxService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddTransient<IBoxPrinterService, BoxPrinterService>();

            services.AddMassTransit(c =>
            {
                c.AddConsumer<BoxCreatedEventHandler>();

                c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    IRabbitMqHost host = cfg.Host(new Uri(Configuration["RabbitMqHostName"]),
                        hostConfigurator =>
                        {
                            hostConfigurator.Username(Configuration["RabbitMqUsername"]);
                            hostConfigurator.Password(Configuration["RabbitMqPassword"]);
                        });
                    
                    services.AddSingleton<IPublishEndpoint>(p => p.GetRequiredService<IBusControl>());
                    services.AddSingleton<ISendEndpointProvider>(p => p.GetRequiredService<IBusControl>());
                    services.AddSingleton<IBus>(p => p.GetRequiredService<IBusControl>());

                    cfg.ReceiveEndpoint("box-created",
                        ep =>
                        {
                            ep.BindMessageExchanges = false;
                            ep.ExchangeType = ExchangeType.Direct;
                            ep.Bind("BoxApi:BoxCreated", x =>
                            { });
                            ep.PrefetchCount = Convert.ToUInt16(Configuration["PrefetchCount"]);
                            ep.UseConcurrencyLimit(Convert.ToUInt16(Configuration["UseConcurrencyLimit"]));
                            ep.ConfigureConsumer<BoxCreatedEventHandler>(provider);

                            ep.UseMessageRetry(r =>
                                r.Incremental(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)));
                        }
                    );
                }));

                services.AddHostedService<RabbitMqBusService>();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/healthcheck");
        }
    }
}