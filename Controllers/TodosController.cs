using FirstWebApi.DTOs;
using FirstWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FirstWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TodosController(ITodoService todoService, ILogger<TodosController> logger) : ControllerBase
{
  private readonly ITodoService _todoService = todoService;
  private readonly ILogger<TodosController> _logger = logger;

  /// <summary>
  /// Gets all todos
  /// </summary>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<ApiResponse<IEnumerable<TodoDto>>>> GetAllTodos()
  {
    var todos = await _todoService.GetAllTodosAsync();
    return Ok(ApiResponse<IEnumerable<TodoDto>>.SuccessResponse(todos));
  }

  /// <summary>
  /// Gets a todo by id
  /// </summary>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> GetTodoById(int id)
  {
    var todo = await _todoService.GetTodoByIdAsync(id);

    if (todo == null)
    {
      return NotFound(ApiResponse<TodoDto>.ErrorResponse($"Todo with id {id} not found", 404));
    }

    return Ok(ApiResponse<TodoDto>.SuccessResponse(todo));
  }

  /// <summary>
  /// Creates a new todo
  /// </summary>
  [HttpPost]
  [ProducesResponseType(typeof(TodoDto), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> CreateTodo([FromBody] CreateTodoDto createTodoDto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ApiResponse<TodoDto>.ErrorResponse("Validation failed"));
    }

    var todo = await _todoService.CreateTodoAsync(createTodoDto);

    return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id },
        ApiResponse<TodoDto>.SuccessResponse(todo, "Todo created successfully", 201));
  }

  /// <summary>
  /// Updates an existing todo
  /// </summary>
  [HttpPut("{id}")]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<TodoDto>>> UpdateTodo(int id, [FromBody] UpdateTodoDto updateTodoDto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ApiResponse<TodoDto>.ErrorResponse("Validation failed", 400));
    }

    var todo = await _todoService.UpdateTodoAsync(id, updateTodoDto);

    if (todo == null)
    {
      return NotFound(ApiResponse<TodoDto>.ErrorResponse($"Todo with id {id} not found", 404));
    }

    return Ok(ApiResponse<TodoDto>.SuccessResponse(todo, "Todo updated successfully"));
  }

  /// <summary>
  /// Deletes a todo
  /// </summary>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<object>>> DeleteTodo(int id)
  {
    var deleted = await _todoService.DeleteTodoAsync(id);
    if (!deleted)
    {
      return NotFound(ApiResponse<object>.ErrorResponse($"Todo with id {id} not found", 404));
    }

    return Ok(ApiResponse<object>.SuccessResponse(null, "Todo deleted successfully"));
  }
}
