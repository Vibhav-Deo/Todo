using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Reflection;
using Todo.Contracts.Commands.TodoList;
using Todo.Contracts.Commands.User;
using Todo.Database.Models;
using Todo.Web.Services;
using Microsoft.EntityFrameworkCore;

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
            Assembly.Load("Todo.Contracts");
            var backendAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name.Equals("Todo.Backend"));
            var eventConsumers = backendAssembly.GetExportedTypes().Where(type => type.Name.Contains("EventConsumer") && type.IsClass).ToArray();
            var commandHandlers = backendAssembly.GetExportedTypes().Where(type => type.Name.Contains("CommandHandler") && type.IsClass).ToArray();

            var messageBusSettings = Configuration.GetSection("MessageBus");
            services.AddMassTransit(massTransitConfig =>
            {
                foreach (var consumer in eventConsumers)
                {
                    massTransitConfig.AddConsumer(consumer);
                }

                foreach (var handler in commandHandlers)
                {
                    massTransitConfig.AddConsumer(handler);
                }

                massTransitConfig.UsingRabbitMq((context, rabbitMqConfig) =>
                {
                    rabbitMqConfig.ConfigureEndpoints(context);
                    rabbitMqConfig.PurgeOnStartup = true;
                    rabbitMqConfig.Host(new Uri(messageBusSettings["Url"]), h =>
                    {
                        h.Username(messageBusSettings["Username"]);
                        h.Password(messageBusSettings["Password"]);
                    });
                    rabbitMqConfig.ReceiveEndpoint("TodoList.MessageQueue", cfg =>
                    {
                        foreach (var consumer in eventConsumers)
                        {
                            cfg.ConfigureConsumer(context, consumer);
                        }

                        foreach (var handler in commandHandlers)
                        {
                            cfg.ConfigureConsumer(context, handler);
                        }
                        EndpointConvention.Map<CreateTodoListCommand>(cfg.InputAddress);
                        EndpointConvention.Map<UpdateTodoListCommand>(cfg.InputAddress);
                        EndpointConvention.Map<DeleteTodoListCommand>(cfg.InputAddress);
                        EndpointConvention.Map<CreateUserCommand>(cfg.InputAddress);
                        EndpointConvention.Map<UpdateUserCommand>(cfg.InputAddress);
                        EndpointConvention.Map<DeleteUserCommand>(cfg.InputAddress);
                    });
                });
            });

            //Adding DB context for interacting with DB
            services.AddDbContext<TodoListContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IHostedService, BusService>();
            services.AddMassTransitHostedService();
            var classes = backendAssembly.GetExportedTypes().Where(type => type.Name.Contains("ReadService") && type.IsClass);
            foreach (var type in classes)
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
