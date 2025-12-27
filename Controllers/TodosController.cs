
using FirstWebApi.DTOs;
using FirstWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FirstWebApi.Models;

namespace FirstWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TodosController(ITodoService todoService, ILogger<TodosController> logger) : ControllerBase
{
  private readonly ITodoService _todoService = todoService;
  private readonly ILogger<TodosController> _logger = logger;

  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoDto>>), StatusCodes.Status200OK)]
  public async Task<ActionResult<ApiResponse<IEnumerable<TodoDto>>>> GetAllTodos(
    [FromQuery] PaginationParams paginationParams,
    [FromQuery] TodoFilterParams? filterParams = null)
  {
    var result = await _todoService.GetAllTodosAsync(paginationParams, filterParams);
    return Ok(result);
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> GetTodoById(int id)
  {
    var todo = await _todoService.GetTodoByIdAsync(id);
    return Ok(ApiResponse<TodoDto>.SuccessResponse(todo, "Todo retrieved successfully"));
  }

  [HttpPost]
  [ProducesResponseType(typeof(TodoDto), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> CreateTodo([FromBody] CreateTodoDto createTodoDto)
  {
    var todo = await _todoService.CreateTodoAsync(createTodoDto);

    return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id },
        ApiResponse<TodoDto>.SuccessResponse(todo, "Todo created successfully", 201));
  }

  [HttpPut("{id}")]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> UpdateTodo(int id, [FromBody] UpdateTodoDto updateTodoDto)
  {
    var todo = await _todoService.UpdateTodoAsync(id, updateTodoDto);
    return Ok(ApiResponse<TodoDto>.SuccessResponse(todo, "Todo updated successfully"));
  }

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteTodo(int id)
  {
    await _todoService.DeleteTodoAsync(id);
    return NoContent();
  }
}
