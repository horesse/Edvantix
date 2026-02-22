"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { Building, ChevronLeft, ChevronRight, Settings } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Island } from "@workspace/ui/components/island";
import { Separator } from "@workspace/ui/components/separator";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@workspace/ui/components/tooltip";
import { cn } from "@workspace/ui/lib/utils";

import { OrganizationSelector } from "./organization-selector";
import { useSidebarContext } from "./sidebar-context";

export function AppSidebarIsland() {
  const pathname = usePathname();
  const { isCollapsed, toggle } = useSidebarContext();

  return (
    <Island
      variant="default"
      padding="sm"
      className={cn(
        "sticky top-0 hidden h-full flex-col transition-all duration-200 lg:flex",
        isCollapsed ? "w-16" : "w-64",
      )}
    >
      <div className="flex items-center justify-between">
        {!isCollapsed && (
          <Link href="/" className="flex items-center gap-2">
            <div className="bg-primary text-primary-foreground flex size-8 items-center justify-center rounded-lg">
              <Building className="size-4" />
            </div>
            <span className="text-lg font-bold">Edvantix</span>
          </Link>
        )}
        <Button
          variant="ghost"
          size="icon"
          onClick={toggle}
          className={cn("size-8", isCollapsed && "mx-auto")}
        >
          {isCollapsed ? (
            <ChevronRight className="size-4" />
          ) : (
            <ChevronLeft className="size-4" />
          )}
        </Button>
      </div>

      <Separator className="my-3" />

      <div className="flex-1">
        <nav className="flex flex-col gap-1">
          {!isCollapsed ? (
            <Link
              href="/organization"
              className={cn(
                "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                pathname.startsWith("/organization")
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
              )}
            >
              <Building className="size-4 shrink-0" />
              <span>Организация</span>
            </Link>
          ) : (
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Link
                    href="/organization"
                    className={cn(
                      "flex items-center justify-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                      pathname.startsWith("/organization")
                        ? "bg-primary text-primary-foreground"
                        : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                    )}
                  >
                    <Building className="size-4" />
                  </Link>
                </TooltipTrigger>
                <TooltipContent side="right">
                  <p>Организация</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          )}
        </nav>
      </div>

      <Separator className="my-3" />

      <div className="flex flex-col gap-1">
        {!isCollapsed && <OrganizationSelector />}
        {isCollapsed && (
          <Link
            href="/settings"
            className={cn(
              "flex items-center justify-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
              pathname.startsWith("/settings")
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
            )}
          >
            <Settings className="size-4" />
          </Link>
        )}
      </div>
    </Island>
  );
}
