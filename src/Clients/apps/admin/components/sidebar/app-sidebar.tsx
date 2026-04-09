"use client";

import * as React from "react";

import { usePathname } from "next/navigation";

import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";

import { navSections } from "./nav-config";
import { NavItem } from "./nav-item";
import { SidebarLogo } from "./sidebar-logo";
import { SidebarUser } from "./sidebar-user";

function SectionLabel({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <p className="text-sidebar-foreground/50 px-3 pt-1 pb-2 text-[10px] font-semibold tracking-wider uppercase">
      {children}
    </p>
  );
}

function isActive(
  pathname: string,
  item: { url: string; exact?: boolean },
): boolean {
  if (item.exact) return pathname === item.url;
  return pathname.startsWith(item.url);
}

export function AppSidebar() {
  const pathname = usePathname();

  return (
    <aside className="bg-sidebar border-sidebar-border hidden w-60 shrink-0 flex-col border-r lg:flex">
      {/* Logo */}
      <div className="border-sidebar-border border-b px-5 py-4">
        <SidebarLogo />
      </div>

      {/* Navigation */}
      <ScrollArea className="min-h-0 flex-1">
        <nav className="px-3 py-3">
          {navSections.map((section, idx) => (
            <div key={section.id} className={idx > 0 ? "mt-1" : undefined}>
              {idx > 0 && <Separator className="mb-3" />}
              <SectionLabel>{section.label}</SectionLabel>
              <div className="space-y-0.5">
                {section.items.map((item) => (
                  <NavItem
                    key={item.id}
                    href={item.url}
                    icon={item.icon}
                    label={item.title}
                    isActive={isActive(pathname, item)}
                  />
                ))}
              </div>
            </div>
          ))}
        </nav>
      </ScrollArea>

      {/* User */}
      <div className="border-sidebar-border border-t px-3 py-3">
        <SidebarUser />
      </div>
    </aside>
  );
}
