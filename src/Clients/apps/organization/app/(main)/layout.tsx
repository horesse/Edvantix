import type React from "react";

import {
  SidebarInset,
  SidebarProvider,
} from "@workspace/ui/components/sidebar";

import { AppSidebar } from "@/components/app-sidebar";
import { AuthGuard } from "@/components/auth-guard";
import { Header } from "@/components/header";
import { OrganizationProvider } from "@/components/organization-provider";
import { ProfileGuard } from "@/components/profile-guard";

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthGuard>
      <ProfileGuard>
        <OrganizationProvider>
          <SidebarProvider>
            <AppSidebar variant="inset" />
            <SidebarInset>
              <Header />
              <main
                className="flex flex-1 flex-col gap-4 p-4"
                id="main-content"
              >
                {children}
              </main>
            </SidebarInset>
          </SidebarProvider>
        </OrganizationProvider>
      </ProfileGuard>
    </AuthGuard>
  );
}
