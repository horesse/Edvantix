"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { Hash, LayoutDashboard, Tag, FileText, Sparkles } from "lucide-react";

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
    <nav className="hidden md:flex w-56 shrink-0 flex-col border-r border-border bg-card/40 py-6 px-3 gap-1">
      {/* Brand label */}
      <div className="flex items-center gap-2 px-3 mb-5">
        <Sparkles className="h-3.5 w-3.5 text-primary" />
        <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">
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
              className={`absolute -left-3 top-1/2 -translate-y-1/2 h-5 w-0.5 rounded-r-full bg-primary transition-all duration-200 ${active ? "opacity-100" : "opacity-0"}`}
            />
            <Icon className="h-4 w-4 shrink-0" />
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}
