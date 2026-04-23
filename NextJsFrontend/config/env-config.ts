import { z } from "zod";

const isClient = typeof window !== "undefined";

const envSchema = z.object({
  NEXT_PUBLIC_BACKEND_API_URL: z.string().url(),
  NODE_ENV: z.enum(["development", "production", "test"]),
});

const evnClientSchema = envSchema.omit({ NODE_ENV: true });

const configProject = isClient ?
  evnClientSchema.safeParse({
    NEXT_PUBLIC_BACKEND_API_URL: process.env.NEXT_PUBLIC_BACKEND_API_URL
  })
  : envSchema.safeParse(process.env);

// console.log(">>> Config parse result:", configProject);

if (!configProject.success) {
  console.error("Invalid environment variables:", configProject.error.message);
  console.error("Invalid environment variables:", configProject.error.issues);
  throw new Error("Invalid environment variables");
}

const envConfig = configProject.data;

export default envConfig;