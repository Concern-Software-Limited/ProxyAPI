namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ResponseModel<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    
    public static ResponseModel<T> SuccessResponse(T data, string message = "Success")
    {
        return new ResponseModel<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
    
    public static ResponseModel<T> ErrorResponse(string message)
    {
        return new ResponseModel<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
