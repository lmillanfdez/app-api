using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Produces("application/json")]
[Route("todos")]
public class TodosController : ControllerBase
{
    private readonly IBaseRepository<Todo> _todoRepository;
    
    public TodosController(IBaseRepository<Todo> todoRepository)
    {
        _todoRepository = todoRepository;
    }
    
    [HttpGet]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetAll()
    {
        var _entities = await _todoRepository.GetAll(false);
        var _result = new ObjectResult(_entities) { StatusCode = (int)HttpStatusCode.OK };
        
        Request.HttpContext.Response.Headers.Add("X-Total-Todos", _entities.Count().ToString());

        return _result;
    }

    [HttpGet("{id}", Name = "GetTodo")]
    public async Task<IActionResult> GetTodo([FromRoute] int id)
    {
        if (!await _todoRepository.Exists(id, false))
            return NotFound();
        
        var resultingTodo = await _todoRepository.Get(id, false);
        return Ok(resultingTodo);
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] AddTodoDTO todoToAdd)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Todo _todo = new Todo { Description = todoToAdd.Description, CreatedOn = DateTime.UtcNow };
        
        await _todoRepository.Add(_todo);
        
        return CreatedAtAction("GetTodo", new { id = _todo.Id }, _todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo([FromBody] Todo todoToUpdate)
    {
        await _todoRepository.Update(todoToUpdate);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo([FromRoute] int id)
    {
        var deletedTodo = await _todoRepository.Delete(id);

        if (deletedTodo == null)
            return BadRequest();
        
        return Ok(deletedTodo);
    }
}