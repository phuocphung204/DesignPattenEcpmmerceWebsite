using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Extensions;

public static class ResultExtensions
{
  /// <summary>
  /// Đảm bảo một điều kiện nhất định phải thỏa mãn, nếu không sẽ trả về lỗi.
  /// </summary>
  /// <typeparam name="T">Kiểu dữ liệu của Result</typeparam>
  /// <param name="result">Đối tượng Result hiện tại</param>
  /// <param name="predicate">Hàm điều kiện (trả về true nếu hợp lệ)</param>
  /// <param name="error">Lỗi trả về nếu điều kiện không thỏa mãn</param>
  /// <returns>Result thành công hoặc Result thất bại với lỗi mới</returns>
  public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
  {
    if (!result.IsSuccess) return result;
    return predicate(result.Value!) ? result : Result<T>.Failure(error);
  }


  public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
  {
    if (result.IsFailure) return Result<TOut>.Failure(result.Error);
    return Result<TOut>.Success(mapper(result.Value));
  }

  /// <summary>
  /// Hàm Bind (còn gọi là FlatMap hoặc SelectMany) 
  /// cho phép bạn nối tiếp các phép toán trả về Result mà không cần phải lồng nhiều lớp Result.
  /// </summary>
  /// <typeparam name="TIn"></typeparam>
  /// <typeparam name="TOut"></typeparam>
  /// <param name="result"></param>
  /// <param name="binder"></param>
  /// <returns>
  /// Nếu kết quả ban đầu là Failure, nó sẽ trả về lỗi ngay lập tức mà không thực hiện hàm binder.
  /// Nếu kết quả là Success, nó sẽ lấy giá trị bên trong và truyền vào hàm binder để thực hiện bước tiếp theo.
  /// </returns>
  public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> binder)
  {
    if (result.IsFailure) return Result<TOut>.Failure(result.Error);
    return binder(result.Value);
  }

  public async static Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> binder)
  {
    var resultValue = await result;
    if (resultValue.IsFailure) return Result<TOut>.Failure(resultValue.Error);
    return binder(resultValue.Value);
  }

  public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, Task<Result<TOut>>> binder)
  {
    if (result.IsFailure) return Result<TOut>.Failure(result.Error);
    return await binder(result.Value);
  }

  public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
    this Task<Result<TIn>> result,
    Func<TIn, Task<Result<TOut>>> binder)
  {
    var resultValue = await result;
    if (resultValue.IsFailure) return Result<TOut>.Failure(resultValue.Error);
    return await binder(resultValue.Value);
  }

  /// <summary>
  /// Hàm Tap (còn gọi là Do) cho phép bạn thực hiện một hành động phụ 
  /// (side effect) trên giá trị của Result mà không làm thay đổi kết quả của nó.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="result"></param>
  /// <param name="action"></param>
  /// <returns>
  /// Nếu Result là Failure, nó sẽ trả về lỗi mà không thực hiện action.
  /// Nếu Result là Success, nó sẽ thực hiện action với giá trị bên trong nhưng vẫn trả
  /// về Result ban đầu (không thay đổi giá trị hoặc lỗi). 
  /// </returns>
  public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
  {
    if (result.IsFailure) return result;
    action(result.Value);
    return result;
  }

  /// <summary cref="Tap{T}"/>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="result"></param>
  /// <param name="action"></param>
  /// <
  /// <returns></returns>
  public static async Task<Result<T>> TapAsync<T>(
    this Result<T> result,
    Func<T, Task> action)
  {
    if (result.IsFailure) return result;
    await action(result.Value);
    return result;
  }

  // Bind: hàm tiếp theo trả về Result, lỗi thì dừng luôn.
  // Map: transform giá trị thành kiểu khác, giữ nguyên lỗi.
  // Tap: chạy side effect (log/save/metric), không đổi value.
}