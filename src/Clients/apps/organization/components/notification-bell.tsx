"use client";

import * as React from "react";

import Link from "next/link";

import { Bell } from "lucide-react";

import useMarkAllAsRead from "@workspace/api-hooks/notifications/useMarkAllAsRead";
import useNotifications from "@workspace/api-hooks/notifications/useNotifications";
import useUnreadCount from "@workspace/api-hooks/notifications/useUnreadCount";
import useMarkAsRead from "@workspace/api-hooks/notifications/useMarkAsRead";
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

/**
 * Кнопка уведомлений в шапке приложения.
 * Открывает всплывающую панель с последними уведомлениями в стиле Linear.
 * Бейдж обновляется каждые 30 с автоматически.
 */
export function NotificationBell() {
  const [open, setOpen] = React.useState(false);

  const { data: countData } = useUnreadCount();
  const unreadCount = countData?.count ?? 0;

  // Загружаем 10 последних уведомлений при открытии попапа
  const {
    data: notificationsData,
    isLoading,
  } = useNotifications({ pageSize: 10 }, { enabled: open });

  const { mutate: markAsRead } = useMarkAsRead();
  const { mutate: markAllAsRead, isPending: isMarkingAll } = useMarkAllAsRead();

  const notifications = notificationsData?.items ?? [];

  const handleMarkAllAsRead = () => {
    markAllAsRead();
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="relative size-8 rounded-full"
          aria-label={`Уведомления${unreadCount > 0 ? `: ${unreadCount} непрочитанных` : ""}`}
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

      <PopoverContent
        align="end"
        sideOffset={8}
        className="w-80 p-0"
      >
        {/* Шапка панели */}
        <div className="flex items-center justify-between px-4 py-3">
          <span className="text-sm font-medium">Уведомления</span>
          {unreadCount > 0 && (
            <Button
              variant="ghost"
              size="sm"
              className="h-auto py-0.5 px-2 text-xs text-muted-foreground hover:text-foreground"
              disabled={isMarkingAll}
              onClick={handleMarkAllAsRead}
            >
              Отметить все
            </Button>
          )}
        </div>

        <Separator />

        {/* Список уведомлений */}
        <ScrollArea className="max-h-80">
          {isLoading ? (
            <NotificationSkeleton />
          ) : notifications.length === 0 ? (
            <EmptyNotifications />
          ) : (
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
          )}
        </ScrollArea>

        {/* Ссылка на полную страницу */}
        {notifications.length > 0 && (
          <>
            <Separator />
            <div className="px-4 py-2">
              <Link
                href="/notifications"
                onClick={() => setOpen(false)}
                className="block text-center text-xs text-muted-foreground hover:text-foreground transition-colors"
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

function NotificationSkeleton() {
  return (
    <div className="space-y-0">
      {Array.from({ length: 3 }).map((_, i) => (
        <div key={i} className="flex items-start gap-3 px-4 py-2.5">
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

function EmptyNotifications() {
  return (
    <div className="flex flex-col items-center justify-center py-8 text-center">
      <Bell className="mb-2 size-8 text-muted-foreground/40" />
      <p className="text-sm text-muted-foreground">Нет новых уведомлений</p>
    </div>
  );
}
