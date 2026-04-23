"use server";

import type { LoginFormData } from "@/app/(main)/(auth)/_components/LoginForm";
import authRequest from "@/services/backend-requests/auth-request";
import { cookies } from "next/headers";

export async function loginAction({ email, password }: LoginFormData) {
  const result = await authRequest.login({ email, password });
  console.log(">>> Login result:", result);

  if (result.payload?.success) { 

    const jwtToken = result.payload.data?.token;
    const cookieStore = await cookies();
    cookieStore.set({
      name: 'jwtToken',
      value: jwtToken || '',
      path: '/',
    });
  } 
  
  return result;
}