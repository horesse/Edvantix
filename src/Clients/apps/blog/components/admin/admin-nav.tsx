"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { Hash, LayoutDashboard, Tag, FileText } from "lucide-react";

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
    <nav className="hidden md:flex w-56 shrink-0 flex-col border-r border-border bg-card/50 py-6 px-3 gap-1">
      <p className="px-3 mb-3 text-xs font-semibold text-muted-foreground uppercase tracking-wider">
        Admin
      </p>
      {navItems.map((item) => {
        const Icon = item.icon;
        const active = isActive(item.href, item.exact);
        return (
          <Link
            key={item.href}
            href={item.href}
            className={`flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors ${
              active
                ? "bg-primary/10 text-primary"
                : "text-muted-foreground hover:bg-accent hover:text-foreground"
            }`}
          >
            <Icon className="h-4 w-4 shrink-0" />
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}
