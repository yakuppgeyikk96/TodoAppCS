

public class ApiResponse<T>
{
  public bool Success { get; set; }
  public int Status { get; set; }
  public T? Data { get; set; }
  public string? Message { get; set; } = string.Empty;

  public static ApiResponse<T> SuccessResponse(T? data, string message = "Operation successful", int status = 200)
  {
    return new ApiResponse<T>
    {
      Success = true,
      Status = status,
      Data = data,
      Message = message
    };
  }

  public static ApiResponse<T> ErrorResponse(string message, int status = 400)
  {
    return new ApiResponse<T>
    {
      Success = false,
      Status = status,
      Data = default,
      Message = message
    };
  }
}
