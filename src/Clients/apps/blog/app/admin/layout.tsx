import type React from "react";

import { AdminGuard } from "@/components/admin/admin-guard";
import { AdminNav } from "@/components/admin/admin-nav";
import { BlogHeader } from "@/components/blog-header";

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex min-h-screen flex-col">
      <BlogHeader />
      <AdminGuard>
        <div className="flex flex-1">
          <AdminNav />
          <main className="flex-1 overflow-auto p-6 md:p-8">{children}</main>
        </div>
      </AdminGuard>
    </div>
  );
}
