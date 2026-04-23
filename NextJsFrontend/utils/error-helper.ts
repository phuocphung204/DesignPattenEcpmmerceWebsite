const getErrorMessage = (errorCode: string | null): string => {
  const errorMessages: { [key: string]: string } = {
    "Login.InvalidCredentials": "Email hoặc mật khẩu không đúng. Vui lòng thử lại.",
    "Login.EmailUnregistered": "Email chưa được đăng ký. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới.",
    "User.EmailAlreadyExists": "Email đã được sử dụng. Vui lòng chọn email khác.",
    "DiscountCode.MinimumOrderAmountNotReached": "Mã giảm giá không áp dụng được vì tổng tiền chưa đạt mức tối thiểu yêu cầu.",
    "DiscountCode.NotFound": "Mã giảm giá không tồn tại. Vui lòng kiểm tra lại.",
  };

  if (!errorCode) {
    return "Đã xảy ra lỗi không xác định.";
  }

  return errorMessages[errorCode] || "Máy chủ đang bận. Vui lòng thử lại sau.";
};

export default getErrorMessage;