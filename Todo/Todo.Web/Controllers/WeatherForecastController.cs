using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Backend.TodoList.CommandHandlers;
using Todo.Backend.TodoList.CommandHandlers.Commands;
using Todo.Contracts.Events;

namespace Todo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBus _bus;
        private readonly ISendEndpointProvider sendEndpointProvider;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBus bus, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _bus = bus;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();
            //await _bus.Send(new TodoListCommand());
            await _bus.Publish(new TodoListCreatedEvent());
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
