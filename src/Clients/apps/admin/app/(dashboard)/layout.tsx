import type React from "react";

import { AdminGuard } from "@/components/auth/admin-guard";
import { AuthGuard } from "@/components/auth/auth-guard";
import { AppSidebar } from "@/components/sidebar/app-sidebar";

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthGuard>
      <AdminGuard>
        <div className="bg-background text-foreground flex h-screen overflow-hidden">
          <AppSidebar />
          <main className="flex min-w-0 flex-1 flex-col overflow-y-auto">
            <div className="px-6 pt-6 pb-6 lg:px-8 lg:pt-8">{children}</div>
          </main>
        </div>
      </AdminGuard>
    </AuthGuard>
  );
}
