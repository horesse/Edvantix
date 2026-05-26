"use client";

import * as React from "react";

import { QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider as NextThemesProvider } from "next-themes";

import { EdvantixAuthProvider } from "@workspace/auth/provider";
import { Toaster } from "@workspace/ui/components/sonner";

import { env } from "@/env.mjs";
import { getQueryClient } from "@/lib/query-client";

const ORG_SCOPES = [
  "openid",
  "profile",
  "email",
  "persona_read",
  "persona_write",
  "organisational_read",
  "organisational_write",
];

export function Providers({ children }: { children: React.ReactNode }) {
  const queryClient = getQueryClient();
  const authority = `${env.NEXT_PUBLIC_KEYCLOAK_URL ?? ""}/realms/${env.NEXT_PUBLIC_KEYCLOAK_REALM ?? ""}`;

  return (
    <EdvantixAuthProvider
      authority={authority}
      clientId={env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID ?? ""}
      scopes={ORG_SCOPES}
      appUrl={env.NEXT_PUBLIC_APP_URL ?? "http://localhost:3001"}
    >
      <QueryClientProvider client={queryClient}>
        <NextThemesProvider
          attribute="class"
          defaultTheme="light"
          enableSystem={false}
          disableTransitionOnChange
          enableColorScheme
        >
          <div className="pb-16 md:pb-0">{children}</div>
          <Toaster richColors closeButton position="top-right" />
          {/*<Analytics />*/}
        </NextThemesProvider>
      </QueryClientProvider>
    </EdvantixAuthProvider>
  );
}
