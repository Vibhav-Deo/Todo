using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models = Todo.Database.Models;

namespace Todo.Backend.TodoList.Repositories.Read
{
    public interface ITodoListReadRepository
    {
        Task<Models.TodoList> GetTodoListByIdAsync(Guid todoListId);
    }
    public class TodoListReadRepository : ITodoListReadRepository
    {
        private readonly IConfiguration _configuration;

        public TodoListReadRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Models.TodoList> GetTodoListByIdAsync(Guid todoListId)
        {
            var sql = "SELECT * FROM TodoLists WHERE Id = @TodoListId";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var todoList = await connection.QueryFirstOrDefaultAsync<Models.TodoList>(sql, new { TodoListId = todoListId });

                return todoList;
            }
        }
    }
}
