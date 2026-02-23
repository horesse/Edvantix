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
  Menu,
  MessageSquare,
} from "lucide-react";

import { OrganizationRole } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@workspace/ui/components/collapsible";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@workspace/ui/components/sheet";
import { cn } from "@workspace/ui/lib/utils";

import { OrganizationSelector } from "./organization-selector";
import { useOrganization } from "./organization-provider";
import { getOrgNavItems } from "./school-nav-items";

const globalNavItems = [
  { id: "schedule", title: "Мое расписание", url: "/schedule", icon: CalendarDays },
  { id: "my-courses", title: "Мои курсы", url: "/my-courses", icon: BookOpen },
  { id: "messages", title: "Сообщения", url: "/messages", icon: MessageSquare },
  { id: "notifications", title: "Уведомления", url: "/notifications", icon: Bell },
];

function MobileNavLink({
  href,
  icon: Icon,
  label,
  isActive,
  indent,
  onClick,
}: {
  href: string;
  icon: React.ElementType;
  label: string;
  isActive: boolean;
  indent?: boolean;
  onClick: () => void;
}) {
  return (
    <Link
      href={href}
      onClick={onClick}
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

export function MobileSidebar() {
  const pathname = usePathname();
  const { userRole, currentOrg } = useOrganization();
  const [open, setOpen] = React.useState(false);

  const [isOrgOpen, setIsOrgOpen] = React.useState(
    () =>
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

  const close = () => setOpen(false);

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="size-5" />
          <span className="sr-only">Открыть меню</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="flex w-64 flex-col p-0">
        <SheetHeader className="border-b border-border px-3 py-2">
          <SheetTitle className="sr-only">Меню навигации</SheetTitle>
          <OrganizationSelector />
        </SheetHeader>

        {/* ScrollArea fix: min-h-0 + flex-1 to get bounded height */}
        <ScrollArea className="min-h-0 flex-1">
          <nav className="px-2 pb-3">
            {/* Global */}
            <p className="text-muted-foreground/60 px-2 pb-1 pt-3 text-[11px] font-semibold uppercase tracking-widest">
              Общее
            </p>
            <div className="space-y-0.5">
              {globalNavItems.map((item) => (
                <MobileNavLink
                  key={item.id}
                  href={item.url}
                  icon={item.icon}
                  label={item.title}
                  isActive={pathname.startsWith(item.url)}
                  onClick={close}
                />
              ))}
            </div>

            <div className="py-2">
              <Separator />
            </div>

            {/* Organisation = school (merged) */}
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
                    <MobileNavLink
                      key={item.id}
                      href={item.url}
                      icon={item.icon}
                      label={item.title}
                      isActive={isActive}
                      indent
                      onClick={close}
                    />
                  );
                })}
              </CollapsibleContent>
            </Collapsible>
          </nav>
        </ScrollArea>

        <div className="border-t border-border px-3 py-2">
          <OrganizationSelector />
        </div>
      </SheetContent>
    </Sheet>
  );
}
