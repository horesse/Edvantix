"use client";

import * as React from "react";

import { usePathname } from "next/navigation";

import { Menu } from "lucide-react";

import { OrganizationRole } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@workspace/ui/components/sheet";

import { useOrganization } from "@/components/organization/provider";
import { OrganizationSelector } from "@/components/organization/selector";

import { getNavSections } from "./nav-config";
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

function isNavItemActive(
  pathname: string,
  item: { url: string; exact?: boolean },
): boolean {
  if (item.exact) return pathname === item.url;
  return pathname.startsWith(item.url) && item.url !== "/";
}

/**
 * Mobile sidebar rendered inside a Sheet (drawer).
 * Shown on screens smaller than lg via the hamburger trigger in the Header.
 */
export function MobileSidebar() {
  const pathname = usePathname();
  const { userRole } = useOrganization();
  const [open, setOpen] = React.useState(false);

  const close = () => setOpen(false);

  const sections = React.useMemo(
    () => getNavSections(userRole ?? OrganizationRole.Student),
    [userRole],
  );

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="size-5" />
          <span className="sr-only">Открыть меню</span>
        </Button>
      </SheetTrigger>

      <SheetContent side="left" className="bg-sidebar flex w-64 flex-col p-0">
        {/* Logo */}
        <SheetHeader className="border-sidebar-border border-b px-5 py-4">
          <SheetTitle className="sr-only">Меню навигации</SheetTitle>
          <SidebarLogo />
        </SheetHeader>

        {/* Organisation selector */}
        <div className="border-sidebar-border border-b px-3 py-3">
          <OrganizationSelector />
        </div>

        {/* Scrollable navigation — min-h-0 + flex-1 for bounded height */}
        <ScrollArea className="min-h-0 flex-1">
          <nav className="px-3 py-3">
            {sections.map((section, idx) => (
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
                      isActive={isNavItemActive(pathname, item)}
                      onClick={close}
                    />
                  ))}
                </div>
              </div>
            ))}
          </nav>
        </ScrollArea>

        {/* User profile */}
        <div className="border-sidebar-border border-t px-2 py-2">
          <SidebarUser />
        </div>
      </SheetContent>
    </Sheet>
  );
}
