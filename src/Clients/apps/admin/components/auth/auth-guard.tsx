"use client";

import type React from "react";

import { AuthGuard as SharedAuthGuard } from "@workspace/auth/auth-guard";

import { LoadingScreen } from "./loading-screen";

/**
 * Thin app wrapper around the shared AuthGuard.
 * Passes the admin-specific LoadingScreen as the loading UI.
 */
export function AuthGuard({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <SharedAuthGuard loadingScreen={<LoadingScreen stage="auth" />}>
      {children}
    </SharedAuthGuard>
  );
}
