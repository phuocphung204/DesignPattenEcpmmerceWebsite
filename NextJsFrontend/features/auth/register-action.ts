"use server";
import type { RegisterFormData } from "@/app/(main)/(auth)/_components/RegisterForm";
import authRequest from "@/services/backend-requests/auth-request";

export async function registerAction(payload: RegisterFormData) {
  const result = await authRequest.register(payload);
  throw new Error("Not implemented yet");
  return result;
}