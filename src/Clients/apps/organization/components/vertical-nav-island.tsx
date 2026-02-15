"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { Island } from "@workspace/ui/components/island";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@workspace/ui/components/tooltip";
import { cn } from "@workspace/ui/lib/utils";

import { organizationNavItems } from "./organization-nav-items";

/**
 * VerticalNavIsland - расширение AppSidebarIsland для раздела "Организация"
 *
 * Концептуально является вертикальным продолжением основного сайдбара,
 * отображающим подразделы текущего активного раздела (Организация).
 *
 * Связь с другими компонентами навигации:
 * - Desktop: Расширяет AppSidebarIsland (показывает детализацию раздела "Организация")
 * - Mobile: Контент соответствует выпадающему списку "Организация" в MobileSidebar
 * - Данные: Использует общий источник organizationNavItems
 */
export function VerticalNavIsland() {
  const pathname = usePathname();

  return (
    <Island
      variant="default"
      padding="none"
      rounded="lg"
      className="sticky top-0 hidden h-full flex-col items-center justify-center p-2 lg:flex"
    >
      <div
        className="absolute left-1/2 top-4 -translate-x-1/2 text-xs font-medium text-muted-foreground/50"
        style={{ writingMode: "vertical-rl", textOrientation: "mixed" }}
      >
        Организация
      </div>

      <nav className="flex flex-col items-center justify-center gap-1.5">
        {organizationNavItems.map((item) => {
          const isActive = item.exact
            ? pathname === item.url
            : pathname.startsWith(item.url) && item.url !== "/organization";

          return (
            <TooltipProvider key={item.id}>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Link
                    href={item.url}
                    className={cn(
                      "flex size-8 items-center justify-center rounded-md transition-colors",
                      isActive
                        ? "bg-primary text-primary-foreground"
                        : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                    )}
                  >
                    <item.icon className="size-4" />
                    <span className="sr-only">{item.title}</span>
                  </Link>
                </TooltipTrigger>
                <TooltipContent side="right">
                  <div className="flex items-center gap-2">
                    <item.icon className="size-3.5" />
                    <span>{item.title}</span>
                  </div>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          );
        })}
      </nav>
    </Island>
  );
}
