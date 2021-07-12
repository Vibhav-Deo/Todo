using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Todo.Backend.Todo.EventConsumers;
using Todo.Backend.TodoList.CommandHandlers;
using Todo.Backend.TodoList.CommandHandlers.Commands;
using Todo.Backend.TodoList.Services;
using Todo.Contracts.Events;
using Todo.Web.Services;

namespace Todo.Web
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
            Assembly.Load("Todo.Backend");
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name.Equals("Todo.Backend"));
            //TODO: Findout how to register event consumers and command handlers dynamically
            /*            var eventConsumers = assembly.GetExportedTypes().Where(type => type.Name.Contains("EventConsumer") && type.IsClass).ToArray();
                        var commandHandlers = assembly.GetExportedTypes().Where(type => type.Name.Contains("CommandHandler") && type.IsClass).ToArray();
            foreach (var consumer in eventConsumers)
            {
                massTransitConfig.AddConsumer(consumer);
            }

            foreach (var handler in commandHandlers)
            {
                massTransitConfig.AddConsumer(handler);
            }*/

            var messageBusSettings = Configuration.GetSection("MessageBus");
            services.AddMassTransit(massTransitConfig =>
            {
                massTransitConfig.AddConsumer<TodoListCommandHandler>();
                massTransitConfig.AddConsumer<TodoListEventConsumer>();
                massTransitConfig.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(messageBusSettings["Url"]), h =>
                    {
                        h.Username(messageBusSettings["Username"]);
                        h.Password(messageBusSettings["Password"]);
                    });

            
                    cfg.ReceiveEndpoint("TodoList.MessageQueue", rabbitMqConfig =>
                    {
                        rabbitMqConfig.Bind("TodoList.MessageQueue.Bind", x =>
                        {
                            x.ExchangeType = ExchangeType.Topic;

                            // route all exchange messages to our queue
                            x.RoutingKey = "#";
                        });

                        rabbitMqConfig.ConfigureConsumer<TodoListCommandHandler>(provider);
                        rabbitMqConfig.ConfigureConsumer<TodoListEventConsumer>(provider);
                        EndpointConvention.Map<TodoListCommand>(rabbitMqConfig.InputAddress);
                    });
                }));
            });
            services.AddSingleton<IHostedService, BusService>();
            services.AddMassTransitHostedService();
            var classes = assembly.GetExportedTypes().Where(type => type.Name.Contains("ReadService") && type.IsClass);
            foreach(var type in classes)
            {
                services.AddScoped(type.GetInterface($"I{type.Name}"), type);
            }
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo.Web", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
