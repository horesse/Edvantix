import type React from "react";

import { AuthGuard } from "@/components/auth/auth-guard";
import { ProfileGuard } from "@/components/auth/profile-guard";
import { OrganizationProvider } from "@/components/organization/provider";
import { AppSidebar } from "@/components/sidebar/app-sidebar";

/**
 * Main application layout — flat, Linear-style structure.
 *
 * Structure:
 *   div.flex.h-screen
 *     AppSidebar  ← fixed-width left nav (lg+), logo + org selector + nav + user
 *     main        ← scrollable page area; each page provides its own header
 *
 * Auth flow:
 *   AuthGuard redirects to Keycloak if unauthenticated.
 *   ProfileGuard redirects to /profile/register if profile is missing (404).
 */
export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthGuard>
      <ProfileGuard>
        <OrganizationProvider>
          <div className="bg-background text-foreground flex h-screen overflow-hidden">
            <AppSidebar />
            {/* main is the scroll container — intentionally no padding.
                Padding lives in the inner div so sticky page headers can use
                top-0 relative to main's outer edge (true viewport top).
                Sticky headers break out horizontally via -mx-4 lg:-mx-6. */}
            <main className="flex min-w-0 flex-1 flex-col overflow-y-auto">
              <div className="px-4 pt-4 pb-4 lg:px-6 lg:pt-6 lg:pb-6">
                {children}
              </div>
            </main>
          </div>
        </OrganizationProvider>
      </ProfileGuard>
    </AuthGuard>
  );
}
