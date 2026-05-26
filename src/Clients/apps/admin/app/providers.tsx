"use client";

import * as React from "react";

import { QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider as NextThemesProvider } from "next-themes";

import { EdvantixAuthProvider } from "@workspace/auth/provider";
import { Toaster } from "@workspace/ui/components/sonner";

import { env } from "@/env.mjs";
import { getQueryClient } from "@/lib/query-client";

const ADMIN_SCOPES = [
  "openid",
  "profile",
  "email",
  "persona_read",
  "persona_write",
  "notification_read",
  "notification_write",
];

export function Providers({ children }: { children: React.ReactNode }) {
  const queryClient = getQueryClient();
  const authority = `${env.NEXT_PUBLIC_KEYCLOAK_URL ?? ""}/realms/${env.NEXT_PUBLIC_KEYCLOAK_REALM ?? ""}`;

  return (
    <EdvantixAuthProvider
      authority={authority}
      clientId={env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID ?? ""}
      scopes={ADMIN_SCOPES}
      appUrl={env.NEXT_PUBLIC_APP_URL ?? "http://localhost:3002"}
    >
      <QueryClientProvider client={queryClient}>
        <NextThemesProvider
          attribute="class"
          defaultTheme="light"
          enableSystem={false}
          disableTransitionOnChange
          enableColorScheme
        >
          {children}
          <Toaster richColors closeButton position="top-right" />
        </NextThemesProvider>
      </QueryClientProvider>
    </EdvantixAuthProvider>
  );
}
