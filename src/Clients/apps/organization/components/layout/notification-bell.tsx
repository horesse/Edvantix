"use client";

import * as React from "react";

import Link from "next/link";

import { Bell } from "lucide-react";

import useMarkAllAsRead from "@workspace/api-hooks/notifications/useMarkAllAsRead";
import useMarkAsRead from "@workspace/api-hooks/notifications/useMarkAsRead";
import useNotifications from "@workspace/api-hooks/notifications/useNotifications";
import useUnreadCount from "@workspace/api-hooks/notifications/useUnreadCount";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { NotificationItem } from "./notification-item";

export function NotificationBell() {
  const [open, setOpen] = React.useState(false);

  const { data: countData } = useUnreadCount();
  const unreadCount = countData?.count ?? 0;

  const { data: notificationsData, isLoading } = useNotifications(
    { pageSize: 10 },
    { enabled: open },
  );

  const { mutate: markAsRead } = useMarkAsRead();
  const { mutate: markAllAsRead, isPending: isMarkingAll } = useMarkAllAsRead();

  const notifications = notificationsData?.items ?? [];

  const ariaLabel =
    unreadCount > 0
      ? `Уведомления: ${unreadCount} непрочитанных`
      : "Уведомления";

  const renderContent = () => {
    if (isLoading) {
      return (
        <div className="space-y-0">
          {["skeleton-1", "skeleton-2", "skeleton-3"].map((id) => (
            <div key={id} className="flex items-start gap-3 px-4 py-2.5">
              <Skeleton className="mt-0.5 size-4 shrink-0 rounded" />
              <div className="flex-1 space-y-1.5">
                <Skeleton className="h-3.5 w-3/4" />
                <Skeleton className="h-3 w-1/2" />
              </div>
            </div>
          ))}
        </div>
      );
    }

    if (notifications.length === 0) {
      return (
        <div className="flex flex-col items-center justify-center py-8 text-center">
          <Bell className="text-muted-foreground/40 mb-2 size-8" />
          <p className="text-muted-foreground text-sm">Нет новых уведомлений</p>
        </div>
      );
    }

    return (
      <div>
        {notifications.map((n) => (
          <NotificationItem
            key={n.id}
            notification={n}
            onMarkAsRead={markAsRead}
            compact
          />
        ))}
      </div>
    );
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="relative size-8 rounded-full"
          aria-label={ariaLabel}
        >
          <Bell className="size-4" />
          {unreadCount > 0 && (
            <Badge
              variant="destructive"
              className="absolute -top-1 -right-1 flex size-4 items-center justify-center rounded-full p-0 text-[9px]"
            >
              {unreadCount > 99 ? "99+" : unreadCount}
            </Badge>
          )}
        </Button>
      </PopoverTrigger>

      <PopoverContent align="end" sideOffset={8} className="w-80 p-0">
        <div className="flex items-center justify-between px-4 py-3">
          <span className="text-sm font-medium">Уведомления</span>
          {unreadCount > 0 && (
            <Button
              variant="ghost"
              size="sm"
              className="text-muted-foreground hover:text-foreground h-auto px-2 py-0.5 text-xs"
              disabled={isMarkingAll}
              onClick={() => markAllAsRead()}
            >
              Отметить все
            </Button>
          )}
        </div>

        <Separator />

        <ScrollArea className="max-h-80">{renderContent()}</ScrollArea>

        {notifications.length > 0 && (
          <>
            <Separator />
            <div className="px-4 py-2">
              <Link
                href="/notifications"
                onClick={() => setOpen(false)}
                className="text-muted-foreground hover:text-foreground block text-center text-xs transition-colors"
              >
                Все уведомления
              </Link>
            </div>
          </>
        )}
      </PopoverContent>
    </Popover>
  );
}
