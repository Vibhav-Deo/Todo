using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Todo.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Todo.Web.Infrastructure;
using Todo.Database.Cosmos;
using Todo.Web.TokenManager;
using Todo.Backend.User.EventConsumer;
using Todo.Backend.User.CommandHandler;
using Todo.Backend.Todo.EventConsumers;
using Todo.Backend.TodoList.CommandHandler;
using Todo.Backend.User.Services;
using Todo.Backend.User.Repository;
using Todo.Backend.User.Repositories.Write;
using Todo.Backend.TodoList.Services;
using Todo.Backend.TodoList.Repositories.Read;
using Todo.Backend.TodoList.Repositories.Write;

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

            var messageBusConfig = Configuration.GetSection("MessageBus");
            var busType = messageBusConfig["Type"];
            if(busType.ToLower().Equals("inmemory"))
            {
                services.AddMassTransit(massTransitConfig =>
                {
                    massTransitConfig.AddConsumer<UserEventConsumer>();
                    massTransitConfig.AddConsumer<UserCommandHandler>();
                    massTransitConfig.AddConsumer<TodoListEventConsumer>();
                    massTransitConfig.AddConsumer<TodoListCommandHandler>();

                    massTransitConfig.UsingInMemory((context, cfg) =>
                    {

                        cfg.ReceiveEndpoint(nameof(UserEventConsumer), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<UserEventConsumer>(context);
                        });

                        cfg.ReceiveEndpoint(nameof(UserCommandHandler), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<UserCommandHandler>(context);
                        });

                        cfg.ReceiveEndpoint(nameof(TodoListEventConsumer), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<TodoListEventConsumer>(context);
                        });

                        cfg.ReceiveEndpoint(nameof(TodoListCommandHandler), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<TodoListCommandHandler>(context);
                        });
                    });
                });
            }
            else
            {
                var busConfig = messageBusConfig.GetSection("BusConfig");

                services.AddMassTransit(massTransitConfig =>
                {
                    massTransitConfig.AddConsumer<UserEventConsumer>();
                    massTransitConfig.AddConsumer<UserCommandHandler>();
                    massTransitConfig.AddConsumer<TodoListEventConsumer>();
                    massTransitConfig.AddConsumer<TodoListCommandHandler>();

                    massTransitConfig.UsingRabbitMq((context, rabbitMqConfig) =>
                    {
                        rabbitMqConfig.Host(new Uri(busConfig["Url"]), h =>
                        {
                            h.Username(busConfig["Username"]);
                            h.Password(busConfig["Password"]);
                        });

                        rabbitMqConfig.ReceiveEndpoint(nameof(UserEventConsumer), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<UserEventConsumer>(context);
                        });

                        rabbitMqConfig.ReceiveEndpoint(nameof(UserCommandHandler), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<UserCommandHandler>(context);
                        });

                        rabbitMqConfig.ReceiveEndpoint(nameof(TodoListEventConsumer), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<TodoListEventConsumer>(context);
                        });

                        rabbitMqConfig.ReceiveEndpoint(nameof(TodoListCommandHandler), cfg =>
                        {
                            cfg.PrefetchCount = 10;
                            cfg.ConfigureConsumer<TodoListCommandHandler>(context);
                        });
                    });
                });
            }

            //Adding DB context for interacting with DB
            services.AddDbContext<TodoListContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton(provider =>
            {
                var cosmosCredentials = Configuration.GetSection("CosmosCredentials");
                return new CosmosDbContext(cosmosCredentials["Key"], cosmosCredentials["Url"]);
            });
            services.AddMassTransitHostedService();
            
            services.AddScoped<IUserReadService,UserReadService>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();

            services.AddScoped<ITodoListReadService, TodoListReadService>();
            services.AddScoped<ITodoListReadRepository, TodoListReadRepository>();
            services.AddScoped<ITodoListWriteRepository, TodoListWriteRepository>();

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
            var jwtTokenConfig = Configuration.GetSection("TokenConfig").Get<TokenConfig>();
            services.AddSingleton<ITokenManager, TokenManager.TokenManager>();
            services.AddSingleton(jwtTokenConfig);
            //Add authentication scheme for JWT.
            services.AddAuthentication(authConfig =>
            {
                authConfig.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authConfig.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtConfig =>
            {
                jwtConfig.RequireHttpsMetadata = true;
                jwtConfig.SaveToken = true;
                jwtConfig.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
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
