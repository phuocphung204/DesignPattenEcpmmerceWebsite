public class ApiResponse
{
  #pragma warning disable CS8618
  public bool Success { get; set; }
  public string Message { get; set; }
  public object Data { get; set; }
  public List<string>? Errors { get; set; }
  public string ErrorCode { get; set; }


  // Các phương thức tĩnh giúp tạo Response nhanh
  public static ApiResponse SuccessResult(object data, string msg = "Success")
      => new ApiResponse { Success = true, Data = data, Message = msg };

  public static ApiResponse FailureResult(string msg, string errorCode, List<string>? errors = null)
      => new ApiResponse { Success = false, Message = msg, ErrorCode = errorCode, Errors = errors };
  #pragma warning restore CS8618
}
