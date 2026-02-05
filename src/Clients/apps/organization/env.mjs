import { createEnv } from "@t3-oss/env-nextjs";
import { z } from "zod";

export const env = createEnv({
  server: {
    AUTH_SECRET: z.string().min(1),
    KEYCLOAK_URL: z.url(),
    KEYCLOAK_REALM: z.string(),
    KEYCLOAK_CLIENT_ID: z.string(),
    KEYCLOAK_CLIENT_SECRET: z.string().optional().default(""),
  },

  client: {
    NEXT_PUBLIC_APP_URL: z.url().optional(),
    NEXT_PUBLIC_GATEWAY_HTTPS: z.url().optional(),
    NEXT_PUBLIC_GATEWAY_HTTP: z.url().optional(),
  },

  runtimeEnv: {
    AUTH_SECRET: process.env.AUTH_SECRET,
    KEYCLOAK_URL: process.env.KEYCLOAK_URL,
    KEYCLOAK_REALM: process.env.KEYCLOAK_REALM,
    KEYCLOAK_CLIENT_ID: process.env.KEYCLOAK_CLIENT_ID,
    KEYCLOAK_CLIENT_SECRET: process.env.KEYCLOAK_CLIENT_SECRET,
    NEXT_PUBLIC_APP_URL: process.env.NEXT_PUBLIC_APP_URL,
    NEXT_PUBLIC_GATEWAY_HTTPS: process.env.NEXT_PUBLIC_GATEWAY_HTTPS,
    NEXT_PUBLIC_GATEWAY_HTTP: process.env.NEXT_PUBLIC_GATEWAY_HTTP,
  },

  skipValidation: !!process.env.SKIP_ENV_VALIDATION,
  emptyStringAsUndefined: true,
});
