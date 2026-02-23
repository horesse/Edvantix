"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import {
  Bell,
  BookOpen,
  Building,
  CalendarDays,
  ChevronDown,
  MessageSquare,
  Settings,
  UserPlus,
} from "lucide-react";

import { OrganizationRole } from "@workspace/types/company";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@workspace/ui/components/collapsible";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import { cn } from "@workspace/ui/lib/utils";

import { OrganizationSelector } from "./organization-selector";
import { useOrganization } from "./organization-provider";
import { getOrgNavItems } from "./school-nav-items";

// ── Global navigation (workspace-level, all roles) ────────────────────────
const globalNavItems = [
  { id: "schedule", title: "Мое расписание", url: "/schedule", icon: CalendarDays },
  { id: "my-courses", title: "Мои курсы", url: "/my-courses", icon: BookOpen },
  { id: "messages", title: "Сообщения", url: "/messages", icon: MessageSquare },
  { id: "notifications", title: "Уведомления", url: "/notifications", icon: Bell },
];

// ── Footer actions ────────────────────────────────────────────────────────
const footerItems = [
  { id: "invite", title: "Пригласить", url: "/invitations/create", icon: UserPlus },
  { id: "settings", title: "Настройки аккаунта", url: "/settings", icon: Settings },
];

// ── Primitives ────────────────────────────────────────────────────────────

function NavItem({
  href,
  icon: Icon,
  label,
  isActive,
  indent = false,
}: {
  href: string;
  icon: React.ElementType;
  label: string;
  isActive: boolean;
  indent?: boolean;
}) {
  return (
    <Link
      href={href}
      className={cn(
        "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors",
        indent && "pl-7",
        isActive
          ? "bg-accent text-accent-foreground font-medium"
          : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
      )}
    >
      <Icon className="size-4 shrink-0" />
      <span>{label}</span>
    </Link>
  );
}

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <p className="text-muted-foreground/60 px-2 pb-1 pt-3 text-[11px] font-semibold uppercase tracking-widest">
      {children}
    </p>
  );
}

// ── Main component ────────────────────────────────────────────────────────

/**
 * Primary sidebar — flat, Linear-style, no shadows or rounded cards.
 * Desktop only (lg:flex). Mobile navigation is in MobileSidebar (Sheet).
 *
 * ScrollArea fix: `min-h-0` is required alongside `flex-1` so that the flex
 * item can actually shrink and the Radix viewport gets a bounded height.
 */
export function AppSidebar() {
  const pathname = usePathname();
  const { userRole, currentOrg } = useOrganization();

  const [isOrgOpen, setIsOrgOpen] = React.useState(() =>
    pathname === "/" ||
    pathname.startsWith("/school") ||
    pathname.startsWith("/organization"),
  );

  const orgNavItems = React.useMemo(
    () => getOrgNavItems(userRole ?? OrganizationRole.Student),
    [userRole],
  );

  const isOrgSectionActive =
    pathname === "/" ||
    pathname.startsWith("/school") ||
    pathname.startsWith("/organization");

  return (
    <aside className="bg-sidebar text-sidebar-foreground hidden w-64 shrink-0 flex-col border-r border-border lg:flex">
      {/* ── Org switcher ─────────────────────────────────────────── */}
      <div className="border-b border-border px-3 py-2">
        <OrganizationSelector />
      </div>

      {/* ── Nav (scroll fix: flex-1 + min-h-0) ──────────────────── */}
      <ScrollArea className="min-h-0 flex-1">
        <div className="px-2 pb-3">

          {/* Global workspace nav */}
          <SectionLabel>Общее</SectionLabel>
          <div className="space-y-0.5">
            {globalNavItems.map((item) => (
              <NavItem
                key={item.id}
                href={item.url}
                icon={item.icon}
                label={item.title}
                isActive={pathname.startsWith(item.url)}
              />
            ))}
          </div>

          <div className="py-2">
            <Separator />
          </div>

          {/* Current organisation (= school) — one merged section */}
          <Collapsible open={isOrgOpen} onOpenChange={setIsOrgOpen}>
            <CollapsibleTrigger asChild>
              <button
                type="button"
                className={cn(
                  "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                  isOrgSectionActive && !isOrgOpen
                    ? "bg-accent text-accent-foreground"
                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                )}
              >
                <Building className="size-4 shrink-0" />
                <span className="flex-1 truncate text-left">
                  {currentOrg?.shortName ?? "Организация"}
                </span>
                <ChevronDown
                  className={cn(
                    "size-3.5 shrink-0 transition-transform duration-150",
                    isOrgOpen && "rotate-180",
                  )}
                />
              </button>
            </CollapsibleTrigger>
            <CollapsibleContent className="mt-0.5 space-y-0.5">
              {orgNavItems.map((item) => {
                const isActive = item.exact
                  ? pathname === item.url
                  : pathname.startsWith(item.url) && item.url !== "/";

                return (
                  <NavItem
                    key={item.id}
                    href={item.url}
                    icon={item.icon}
                    label={item.title}
                    isActive={isActive}
                    indent
                  />
                );
              })}
            </CollapsibleContent>
          </Collapsible>

        </div>
      </ScrollArea>

      {/* ── Footer ────────────────────────────────────────────────── */}
      <div className="border-t border-border px-2 py-2">
        <div className="space-y-0.5">
          {footerItems.map((item) => (
            <NavItem
              key={item.id}
              href={item.url}
              icon={item.icon}
              label={item.title}
              isActive={pathname.startsWith(item.url)}
            />
          ))}
        </div>
      </div>
    </aside>
  );
}
