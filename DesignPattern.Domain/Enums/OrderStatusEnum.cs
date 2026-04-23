
namespace DesignPattern.Domain.Enums;

public enum OrderStatusEnum
{
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4,
  Returned = 5,
  Completed = 6
}

// Pending # Khách hàng đặt hàng -> Thực hiện thanh toán cho đơn hàng phương thức online hoặc chờ thanh toán đối với phương thức offline (ví dụ: COD)

// Pending --(Hủy đơn)--> Cancelled # Nhân viên/Khách hàng hủy đơn hàng

// Pending --(Xác nhận đơn)--> Processing # Nhân viên xác nhận đơn hàng (có chọn kho thủ công?)
// -> Hệ thống tự động kiểm tra tồn kho (thêm số lượng sản phẩm vào "chờ giao")

// Processing --(Hủy đơn)--> Cancelled # Nhân viên/Khách hàng hủy đơn hàng

// Processing --(Chốt đơn)--> Shipped # Nhân viên chốt đơn hàng

// Shipped --(Giao hàng thành công)--> Delivered # Hệ thống trừ tồn kho của sản phẩm

// Delivered --(Trả hàng)--> Returned # Khách hàng trả hàng

// Returned --(Xử lý hoàn tiền)--> Completed # Hệ thống xử lý hoàn tiền cho khách hàng

// Delivered --(Hoàn thành)--> Completed # Khách hàng đánh giá và hoàn tất đơn hàng