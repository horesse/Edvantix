"use client";

import type React from "react";

import { AppSidebar } from "@/components/app-sidebar";
import { AuthGuard } from "@/components/auth-guard";
import { Header } from "@/components/header";
import { OrganizationProvider } from "@/components/organization-provider";
import { ProfileGuard } from "@/components/profile-guard";

/**
 * Main application layout — flat Linear-style structure.
 *
 * Structure:
 *   div.flex.h-screen          ← full-height container
 *     AppSidebar               ← fixed-width left nav (desktop only)
 *     div.flex-col             ← right column: topbar + page content
 *       Header                 ← slim border-b topbar (h-12)
 *       main                   ← scrollable page area
 *
 * Mobile navigation is provided by MobileSidebar (Sheet) inside Header.
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
            <div className="flex min-w-0 flex-1 flex-col overflow-hidden">
              <Header />
              <main className="flex-1 overflow-y-auto p-4 lg:p-6">
                {children}
              </main>
            </div>
          </div>
        </OrganizationProvider>
      </ProfileGuard>
    </AuthGuard>
  );
}
