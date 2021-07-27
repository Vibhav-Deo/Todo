using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Backend.TodoList.CommandHandler;
using Todo.Contracts.Api;
using Todo.Contracts.Commands.TodoList;
using Todo.Contracts.StringResources;
using Todo.Database.Models;
using Todo.Web.RequestResponse.TodoList;

namespace Todo.Web.Controllers
{
    [ApiController]
    public class TodoListController : BaseApiController
    {
        private readonly ILogger<TodoListController> _logger;
        private readonly IMapper _mapper;
        private readonly IBus _bus;
        public TodoListController(ILogger<TodoListController> logger, IBus bus, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _bus = bus;
        }

        /// <summary>
        /// This method is used for user registeration upon success this method returns typeof(UserResponse).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.TodoList.CREATE_TODO_LIST)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(typeof(TodoList))]
        //[Authorize]
        public async Task<IActionResult> CreateTodoList([FromBody] CreateTodoListRequest request, [FromRoute] Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return await MakeServiceCall(async () =>
            {
                var createTodoListCommand = new CreateTodoListCommand
                {
                    UserId = userId
                };

                _mapper.Map(request, createTodoListCommand);

                await _bus.Publish(createTodoListCommand);

                return Success("Success");

            }, StringResources.GeneralError);
        }
    }
}
