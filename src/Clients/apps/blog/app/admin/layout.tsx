import type React from "react";

import Link from "next/link";

import { BlogHeader } from "@/components/blog-header";
import { AdminGuard } from "@/components/admin/admin-guard";
import { AdminNav } from "@/components/admin/admin-nav";

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
          <main className="flex-1 p-6 md:p-8 overflow-auto">{children}</main>
        </div>
      </AdminGuard>
    </div>
  );
}
