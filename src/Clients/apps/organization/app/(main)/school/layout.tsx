import type React from "react";

/**
 * School section layout wrapper.
 *
 * School pages (e.g. courses split-panel) manage their own internal scrolling,
 * so the ContentArea in the parent layout must not add overflow-y-auto or
 * extra padding. This layout slot is intentionally thin — the parent
 * (main)/layout.tsx already wraps everything in ContentArea; pages here
 * receive the full available height and override padding via ContentArea's
 * className prop when needed.
 */
export default function SchoolLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <>{children}</>;
}
