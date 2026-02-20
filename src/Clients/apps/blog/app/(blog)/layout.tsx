import type React from "react";

import { BlogFooter } from "@/components/blog-footer";
import { BlogHeader } from "@/components/blog-header";

export default function BlogLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex min-h-screen flex-col">
      <BlogHeader />
      <main className="flex-1">{children}</main>
      <BlogFooter />
    </div>
  );
}
