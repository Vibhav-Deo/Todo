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
using Todo.Backend.TodoList.Repositories.Dtos;
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
        private readonly IRequestClient<CreateTodoListCommand> _createTodoListCommandRequestClient;
        private readonly IRequestClient<CreateTodoListItemsCommand> _createTodoListItemsCommandRequestClient;
        public TodoListController(ILogger<TodoListController> logger, IRequestClient<CreateTodoListCommand> createTodoListCommandRequestClient, IRequestClient<CreateTodoListItemsCommand> createTodoListItemsCommandRequestClient, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _createTodoListCommandRequestClient = createTodoListCommandRequestClient;
            _createTodoListItemsCommandRequestClient = createTodoListItemsCommandRequestClient;
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
        [Produces(typeof(TodoList))]
        [Authorize]
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

                var todoList = await _createTodoListCommandRequestClient.GetResponse<TodoList>(createTodoListCommand);

                return Created(todoList);

            }, StringResources.GeneralError);
        }

        [HttpPost]
        [Route(ApiRoutes.TodoList.CREATE_TODO_LIST_ITEMS)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(TodoListItem))]
        [Authorize]
        public async Task<IActionResult> CreateTodoListItem([FromBody] CreateTodoListItemsRequest request, [FromRoute] Guid todoListId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return await MakeServiceCall(async () =>
            {
                var createTodoListItemsCommand = new CreateTodoListItemsCommand
                {
                    TodoListId = todoListId
                };

                _mapper.Map(request, createTodoListItemsCommand);

                var createdListItems = await _createTodoListItemsCommandRequestClient.GetResponse<CreatedTodoListItemsDto>(createTodoListItemsCommand);

                return Created(createdListItems.Message.TodoListItems);

            }, StringResources.GeneralError);
        }
    }
}
