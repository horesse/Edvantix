"use client";

import * as React from "react";

import { QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider as NextThemesProvider } from "next-themes";

import { Toaster } from "@workspace/ui/components/sonner";

import { getQueryClient } from "@/lib/query-client";

export function Providers({ children }: { children: React.ReactNode }) {
  const queryClient = getQueryClient();

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
        <Toaster richColors closeButton position="top-right" />
        {/*<Analytics />*/}
      </NextThemesProvider>
    </QueryClientProvider>
  );
}
