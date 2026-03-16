"use client";

import Link from "next/link";

import type { LucideIcon } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

interface NavItemProps {
  href: string;
  icon: LucideIcon;
  label: string;
  isActive: boolean;
  /** Optional numeric or string badge shown on the right. */
  badge?: number | string;
  /** Indent for sub-items. */
  indent?: boolean;
  onClick?: () => void;
}

/**
 * A single sidebar navigation link.
 * Active state uses primary color (maps to brand indigo via CSS variables).
 */
export function NavItem({
  href,
  icon: Icon,
  label,
  isActive,
  badge,
  indent = false,
  onClick,
}: Readonly<NavItemProps>) {
  return (
    <Link
      href={href}
      onClick={onClick}
      className={cn(
        "flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm transition-colors",
        indent && "pl-7",
        isActive
          ? "bg-sidebar-accent text-sidebar-accent-foreground font-medium"
          : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground",
      )}
    >
      <Icon className="size-4 shrink-0" />
      <span className="flex-1 truncate">{label}</span>
      {badge !== undefined && (
        <span
          className={cn(
            "ml-auto rounded-full px-1.5 py-0.5 text-[11px] font-medium",
            isActive
              ? "bg-sidebar-accent-foreground/15 text-sidebar-accent-foreground"
              : "bg-sidebar-accent text-sidebar-foreground/70",
          )}
        >
          {badge}
        </span>
      )}
    </Link>
  );
}
