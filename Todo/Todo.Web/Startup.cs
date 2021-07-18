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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Todo.Web.Infrastructure;

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

            var busType = Configuration.GetSection("MessageBus")["Type"];
            if(busType.ToLower().Equals("inmemory"))
            {
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

                    massTransitConfig.UsingInMemory((context, cfg) =>
                    {
                        cfg.TransportConcurrencyLimit = 100;

                        cfg.ConfigureEndpoints(context);

                        cfg.ReceiveEndpoint("TodoList.MessageQueue", cfg =>
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
            }
            else
            {
                var busConfig = Configuration.GetSection("MessageBus.BusConfig");
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
                        rabbitMqConfig.Host(new Uri(busConfig["Url"]), h =>
                        {
                            h.Username(busConfig["Username"]);
                            h.Password(busConfig["Password"]);
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
            }

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
                //Creates a swagger doc for APi's
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo", Version = "v1" });
                //Ading security definition and speficcation for swagger
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Please enter JWT here without Bearer word",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };
                c.AddSecurityRequirement(securityRequirement);
            });

            //Get Token config from the model. TODO: Decide wheather TokenConfig should be in infrastructure or should be moved to models.
            var jwtTokenConfig = Configuration.GetSection("TokenConfig");
            services.AddSingleton(jwtTokenConfig);
            //Add authentication scheme for JWT.
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig["Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig["Secret"])),
                    ValidAudience = jwtTokenConfig["Audience"],
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            //Adding global JSON serializer settings.
            services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

            //Adding and configuring Automapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            //TODO: Decide if the mapper should be singleton.
            services.AddSingleton(mapper);
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
