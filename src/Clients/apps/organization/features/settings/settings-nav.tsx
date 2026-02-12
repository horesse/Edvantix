"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import type { LucideIcon } from "lucide-react";
import { Briefcase, CreditCard, Mail, User } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

type SettingsNavItem = {
  title: string;
  href: string;
  icon: LucideIcon;
  disabled?: boolean;
};

const settingsNavItems: SettingsNavItem[] = [
  {
    title: "Профиль",
    href: "/settings/profile",
    icon: User,
  },
  {
    title: "Контакты",
    href: "/settings/contacts",
    icon: Mail,
  },
  {
    title: "Карьера",
    href: "/settings/career",
    icon: Briefcase,
  },
  {
    title: "Оплата",
    href: "/settings/billing",
    icon: CreditCard,
    disabled: true,
  },
];

export function SettingsNav() {
  const pathname = usePathname();

  return (
    <nav className="flex gap-1 overflow-x-auto sm:flex-col">
      {settingsNavItems.map((item) => {
        const isActive = pathname === item.href;
        const Icon = item.icon;

        if (item.disabled) {
          return (
            <span
              key={item.href}
              className="text-muted-foreground/50 flex shrink-0 cursor-not-allowed items-center gap-2 rounded-md px-3 py-2 text-sm"
            >
              <Icon className="size-4" />
              <span>{item.title}</span>
            </span>
          );
        }

        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "flex shrink-0 items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors",
              isActive
                ? "bg-primary/10 text-primary"
                : "text-muted-foreground hover:bg-muted hover:text-foreground",
            )}
          >
            <Icon className="size-4" />
            <span>{item.title}</span>
          </Link>
        );
      })}
    </nav>
  );
}
