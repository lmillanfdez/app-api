using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using App_api.Contracts;
using App_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace App_api.Respositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApiDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public TodoRepository(ApiDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }
        
        public IEnumerable<Todo> GetAll()
        {
            return _dbContext.Todos;
        }

        public async Task<bool> ExistsTodo(int id)
        {
            return await _dbContext.Todos.AnyAsync(item => item.Id == id);
        }

        public async Task<Todo> GetTodo(int id)
        {
            var inMemoryTodo = _memoryCache.Get<Todo>(id);

            if (inMemoryTodo != null)
                return inMemoryTodo;
            
            var retrievedTodo = await _dbContext.Todos.SingleOrDefaultAsync(item => item.Id == id);

            var memoryCacheEntryOption = new MemoryCacheEntryOptions()
                                                .SetSlidingExpiration(TimeSpan.FromSeconds(10));
            _memoryCache.Set(id, retrievedTodo, memoryCacheEntryOption);

            return retrievedTodo;
        }

        public async Task<Todo> AddTodo(Todo todoToAdd)
        {
            _dbContext.Todos.Add(todoToAdd);
            await _dbContext.SaveChangesAsync();

            return todoToAdd;
        }

        public async Task UpdateTodo(Todo todoToAdd)
        {
            _dbContext.Entry(todoToAdd).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Todo> DeleteTodo(int id)
        {
            var todoToDelete = await _dbContext.Todos.SingleOrDefaultAsync(item => item.Id == id);

            if (todoToDelete == null)
                return null;
            
            _dbContext.Todos.Remove(todoToDelete);
            await _dbContext.SaveChangesAsync();

            return todoToDelete;
        }
    }
}