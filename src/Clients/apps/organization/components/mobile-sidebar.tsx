"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { Building, ChevronDown, Menu } from "lucide-react";

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

import { organizationNavItems } from "./organization-nav-items";
import { OrganizationSelector } from "./organization-selector";

export function MobileSidebar() {
  const pathname = usePathname();
  const [open, setOpen] = React.useState(false);
  const [isOrgOpen, setIsOrgOpen] = React.useState(true);

  const isOrgActive = pathname.startsWith("/organization");

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="size-5" />
          <span className="sr-only">Открыть меню</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="w-64 p-0">
        <SheetHeader className="p-4">
          <SheetTitle className="flex items-center gap-2">
            <div className="bg-primary text-primary-foreground flex size-8 items-center justify-center rounded-lg">
              <Building className="size-4" />
            </div>
            <span className="text-lg font-bold">Edvantix</span>
          </SheetTitle>
        </SheetHeader>
        <Separator />
        <ScrollArea className="h-[calc(100vh-8rem)] px-4 py-4">
          <nav className="flex flex-col gap-1">
            <Collapsible open={isOrgOpen} onOpenChange={setIsOrgOpen}>
              <CollapsibleTrigger asChild>
                <button
                  type="button"
                  className={cn(
                    "flex w-full items-center justify-between rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                    isOrgActive
                      ? "bg-primary text-primary-foreground"
                      : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                  )}
                >
                  <div className="flex items-center gap-2">
                    <Building className="size-4" />
                    <span>Организация</span>
                  </div>
                  <ChevronDown
                    className={cn(
                      "size-4 transition-transform",
                      isOrgOpen && "rotate-180",
                    )}
                  />
                </button>
              </CollapsibleTrigger>
              <CollapsibleContent className="mt-1 space-y-1 pl-6">
                {organizationNavItems.map((item) => {
                  const isActive = item.exact
                    ? pathname === item.url
                    : pathname.startsWith(item.url) &&
                      item.url !== "/organization";

                  return (
                    <Link
                      key={item.id}
                      href={item.url}
                      onClick={() => setOpen(false)}
                      className={cn(
                        "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors",
                        isActive
                          ? "bg-primary text-primary-foreground font-medium"
                          : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                      )}
                    >
                      <item.icon className="size-4" />
                      <span>{item.title}</span>
                    </Link>
                  );
                })}
              </CollapsibleContent>
            </Collapsible>
          </nav>
        </ScrollArea>
        <div className="absolute right-0 bottom-0 left-0 border-t p-4">
          <OrganizationSelector />
        </div>
      </SheetContent>
    </Sheet>
  );
}
