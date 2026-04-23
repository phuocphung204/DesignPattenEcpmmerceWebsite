import type { LoginFormData } from "@/app/(main)/(auth)/_components/LoginForm";
import type { RegisterFormData } from "@/app/(main)/(auth)/_components/RegisterForm";
import http from "../http";

export enum UserRole {
  Customer = "Customer",
  Admin = "Admin"
}

type LoginResponse = {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  avatarLink: string;
  role: UserRole;
  loyaltyPoints: number;
};

// type RegisterPayload = {
//   fullName: string;
//   email: string;
//   phoneNumber: string;
//   password: string;
//   address: {
//     receiverName: string;
//     phoneNumber: string;
//     province: string;
//     country: string;
//     district: string;
//     ward: string;
//     street: string;
//     provinceCode: string;
//     districtCode: string;
//     wardCode: string;
//   };
// };

// type RegisterResponse = {
//   token?: string;
// };

const authRequest = {
  login: ({ email, password }: LoginFormData) =>
    http.post<LoginResponse>(
      "/auth/login",
      { email, password }
    ),

  register: ({ fullName, email, phoneNumber, password, address, }: RegisterFormData) =>
    http.post<unknown>(
      "/users/registry",
      {
        fullName,
        email,
        phoneNumber,
        password,
        address,
      }
    ),

  validateToken: (token: string) =>
    http.head(
      "/auth/validate-token",
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    )
};

export default authRequest;