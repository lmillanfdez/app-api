using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using App_api.Models;
using System.Threading.Tasks;
using App_api.Contracts;

namespace App_api.Controllers
{
    [Produces("application/json")]
    [Route("todos")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        
        public TodosController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        
        [HttpGet]
        [ResponseCache(Duration = 30)]
        public IActionResult GetAll()
        {
            var result = new ObjectResult(_todoRepository.GetAll()) {StatusCode = (int) HttpStatusCode.OK};
            
            Request.HttpContext.Response.Headers.Add("X-Total-Todos", 
                                                                _todoRepository.GetAll().Count().ToString());

            return result;
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<IActionResult> GetTodo([FromRoute] int id)
        {
            if (!await _todoRepository.ExistsTodo(id))
                return NotFound();
            
            var resultingTodo = await _todoRepository.GetTodo(id);
            return Ok(resultingTodo);
        }

        [HttpPost]
        public async Task<IActionResult> AddTodo([FromBody] Todo todoToAdd)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _todoRepository.AddTodo(todoToAdd);
            
            return CreatedAtAction("GetTodo", new { id = todoToAdd.Id }, todoToAdd);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo([FromRoute] int id, [FromBody] Todo todoToUpdate)
        {
            await _todoRepository.UpdateTodo(todoToUpdate);
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo([FromRoute] int id)
        {
            var deletedTodo = await _todoRepository.DeleteTodo(id);

            if (deletedTodo == null)
                return BadRequest();
            
            return Ok(deletedTodo);
        }
    }
}