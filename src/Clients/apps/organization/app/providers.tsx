"use client";

import * as React from "react";
import { useEffect } from "react";

import { QueryClientProvider } from "@tanstack/react-query";
import { Analytics } from "@vercel/analytics/next";
import { ThemeProvider as NextThemesProvider } from "next-themes";

import { env } from "@/env.mjs";
import { initMocks } from "@/lib/msw";
import { getQueryClient } from "@/lib/query-client";

export function Providers({ children }: { children: React.ReactNode }) {
  const queryClient = getQueryClient();

  const gatewayUrl =
    env.NEXT_PUBLIC_GATEWAY_HTTPS || env.NEXT_PUBLIC_GATEWAY_HTTP;

  useEffect(() => {
    if (!gatewayUrl && process.env.NODE_ENV === "development") {
      initMocks();
    }
  }, [gatewayUrl]);

  return (
    <QueryClientProvider client={queryClient}>
      <NextThemesProvider
        attribute="class"
        defaultTheme="system"
        enableSystem
        disableTransitionOnChange
        enableColorScheme
      >
        <div className="pb-16 md:pb-0">{children}</div>
        {/*<Analytics />*/}
      </NextThemesProvider>
    </QueryClientProvider>
  );
}
