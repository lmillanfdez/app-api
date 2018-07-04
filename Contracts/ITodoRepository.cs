using System.Collections.Generic;
using System.Threading.Tasks;
using App_api.Models;

namespace App_api.Contracts
{
    public interface ITodoRepository
    {
        IEnumerable<Todo> GetAll();
        Task<bool> ExistsTodo(int id);
        Task<Todo> GetTodo(int id);
        Task<Todo> AddTodo(Todo todoToAdd);
        Task UpdateTodo(Todo todoToAdd);
        Task<Todo> DeleteTodo(int id);
    }
}