"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { FileText, Hash, LayoutDashboard, Sparkles, Tag } from "lucide-react";

const navItems = [
  {
    href: "/admin",
    label: "Dashboard",
    icon: LayoutDashboard,
    exact: true,
  },
  {
    href: "/admin/posts",
    label: "Posts",
    icon: FileText,
  },
  {
    href: "/admin/categories",
    label: "Categories",
    icon: Hash,
  },
  {
    href: "/admin/tags",
    label: "Tags",
    icon: Tag,
  },
];

export function AdminNav() {
  const pathname = usePathname();

  const isActive = (href: string, exact?: boolean) =>
    exact ? pathname === href : pathname.startsWith(href);

  return (
    <nav className="border-border bg-card/40 hidden w-56 shrink-0 flex-col gap-1 border-r px-3 py-6 md:flex">
      {/* Brand label */}
      <div className="mb-5 flex items-center gap-2 px-3">
        <Sparkles className="text-primary h-3.5 w-3.5" />
        <p className="text-muted-foreground text-xs font-semibold tracking-wider uppercase">
          Admin
        </p>
      </div>

      {navItems.map((item) => {
        const Icon = item.icon;
        const active = isActive(item.href, item.exact);
        return (
          <Link
            key={item.href}
            href={item.href}
            className={`relative flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-150 ${
              active
                ? "bg-primary/10 text-primary shadow-sm"
                : "text-muted-foreground hover:bg-accent hover:text-foreground"
            }`}
          >
            {/* Left accent bar for active item */}
            <span
              className={`bg-primary absolute top-1/2 -left-3 h-5 w-0.5 -translate-y-1/2 rounded-r-full transition-all duration-200 ${active ? "opacity-100" : "opacity-0"}`}
            />
            <Icon className="h-4 w-4 shrink-0" />
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}
