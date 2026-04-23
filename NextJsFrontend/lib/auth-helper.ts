
import Cookie from "js-cookie"

/**
 * Lấy token của người dùng từ cookies
 * @returns current user token
 */
export function getUserJwtToken(): string | null {
  return Cookie.get("jwtToken") || null;
}

/**
 * Tạo header Authorization nếu token tồn tại
 * @returns object chứa header Authorization hoặc object rỗng nếu không có token
 */
export function safeAuthorizationHeader(): { Authorization: string } | object {
  const token = getUserJwtToken()
  return token ? { Authorization: `Bearer ${token}` } : {};
}
