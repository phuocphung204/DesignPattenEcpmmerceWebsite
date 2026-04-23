using System.Security;
using System.Security.Principal;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Common;

public class Result
{
  protected Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
      throw new ArgumentException("Invalid error", nameof(error));

    IsSuccess = isSuccess;
    Error = error;
  }

  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Error Error { get; }

  public static Result Success() => new(true, Error.None);
  public static Result Failure(Error error) => new(false, error);

  public static implicit operator Result(bool isSuccess) => isSuccess ? Success() : Failure(Error.None);
  public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{

  private readonly TValue? _value;

  /// <remarks>
  /// <b>1.Ý nghĩa của internal</b>
  /// <para>Khi bạn đánh dấu một class hoặc một thành phần (biến, hàm) là internal, nó chỉ có thể được nhìn thấy và sử dụng bởi các mã nguồn nằm trong cùng một Assembly (thường là cùng một project, file .dll hoặc .exe sau khi biên dịch).</para>
  /// <list type="bullet">
  /// <item>Bên trong cùng project: Bạn gọi thoải mái.</item>
  /// <item>Bên ngoài project (Project khác tham chiếu đến): Sẽ không thấy và không dùng được, dù đã using namespace.</item>
  /// </list>
  /// </remarks>
  /// <param name="value">Giá trị của kết quả</param>
  /// <param name="isSuccess"></param>
  /// <param name="error"></param>
  protected internal Result(TValue? value, bool isSuccess, Error error)
      : base(isSuccess, error) => _value = value;


  /// <summary>
  /// Lấy giá trị của Result nếu thành công, nếu thất bại sẽ ném ra InvalidOperationException
  /// </summary>
  public TValue Value => IsSuccess
      ? _value!
      : throw new InvalidOperationException("Cannot access the value of a failed Result.");

  /// <summary>
  /// Tạo một Result thành công với giá trị
  /// </summary>
  /// <param name="value"></param>
  public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

  /// <remarks>
  /// Từ khóa new (Hiding): Trong C#, từ khóa new ở đây dùng để che giấu (shadowing) phương thức của lớp cha. Nó báo với trình biên dịch rằng: "Tôi biết lớp cha đã có hàm Failure rồi, nhưng ở lớp con này, tôi muốn định nghĩa một hàm Failure mới hoàn toàn với kiểu trả về khác."
  /// </remarks>
  /// <param name="error">Lỗi của Result thất bại</param>
  /// <returns>Trả về một Result thất bại với lỗi được chỉ định</returns>
  public static new Result<TValue> Failure(Error error) => new(default, false, error);

  public static Result<TValue> From(TValue value) => Success(value);

  // Implicit conversions for cleaner syntax
  public static implicit operator Result<TValue>(TValue value) => Success(value);
  public static implicit operator Result<TValue>(Error error) => Failure(error);
}
