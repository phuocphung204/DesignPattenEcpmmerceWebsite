using System.Linq.Expressions;

namespace DesignPattern.Domain.Extensions;

public static class ExpressionExtensions
{
  /// <summary>
  /// Kiểm tra xem một thuộc tính có được gọi đến trong Expression hay không.
  /// </summary>
  /// <remarks>
  /// selector = new { x.Name, x.Age, x.A.B } <br/>
  /// selector.IsPropertySelected("Name"); // true <br/>
  /// selector.IsPropertySelected("Age"); // false <br/>
  /// selector.IsPropertySelected("A.B"); // true <br/>
  /// selector.IsPropertySelected("A"); // false <br/>
  /// selector.IsPropertySelected("C"); // false <br/>
  /// </remarks>
  public static bool IsPropertySelected<TEntity, TResult>(
      this Expression<Func<TEntity, TResult>> selector,
      string propertyName)
  {
    if (selector == null) throw new ArgumentNullException(nameof(selector));

    // Khởi tạo Visitor và cho nó "đi dạo" trong cây biểu thức
    var visitor = new PropertyFinderVisitor(propertyName, selector.Parameters[0]);
    visitor.Visit(selector);

    return visitor.IsFound;
  }

  // Class nội bộ làm nhiệm vụ duyệt cây
  private class PropertyFinderVisitor : ExpressionVisitor
  {
    private readonly string _targetProperty;
    private readonly ParameterExpression _rootParameter;
    public bool IsFound { get; private set; }

    public PropertyFinderVisitor(string targetProperty, ParameterExpression rootParameter)
    {
      _targetProperty = targetProperty;
      _rootParameter = rootParameter; // Lưu lại tham số 'x' gốc
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      // Tối ưu: Nếu đã tìm thấy rồi thì ngừng tìm kiếm để tiết kiệm CPU
      if (IsFound) return node;

      // Kiểm tra tên thuộc tính có khớp không (Bỏ qua hoa thường)
      if (node.Member.Name.Equals(_targetProperty, StringComparison.OrdinalIgnoreCase))
      {
        // KIỂM TRA BẢO MẬT: Đảm bảo thuộc tính này thuộc về biến 'x' (TEntity)
        // Chứ không phải thuộc tính của một biến bên ngoài đưa vào (Closure)
        if (IsSourcedFromParameter(node))
        {
          IsFound = true;
          return node;
        }
      }

      // Tiếp tục duyệt sâu xuống cấp cha (Ví dụ: từ x.A.B -> lùi về kiểm tra x.A)
      return base.VisitMember(node);
    }

    // Hàm đệ quy ngược để kiểm tra xem rễ của biểu thức có phải là 'x' không
    private bool IsSourcedFromParameter(MemberExpression node)
    {
      Expression current = node;

      // Lột dần các lớp Member (x.A.B -> x.A -> x)
      while (current is MemberExpression memberExp)
      {
        current = memberExp.Expression!;
      }

      // Xóa bỏ Convert nếu có
      if (current is UnaryExpression unaryExp)
      {
        current = unaryExp.Operand;
      }

      // Nếu cái gốc cuối cùng chính là tham số truyền vào Lambda (biến 'x') -> Hợp lệ
      return current == _rootParameter;
    }
  }
}