"use client";

import * as React from "react";

import { Bell } from "lucide-react";

import useMarkAllAsRead from "@workspace/api-hooks/notifications/useMarkAllAsRead";
import useMarkAsRead from "@workspace/api-hooks/notifications/useMarkAsRead";
import useNotifications from "@workspace/api-hooks/notifications/useNotifications";
import { Button } from "@workspace/ui/components/button";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { NotificationItem } from "@/components/notification-item";

const TABS = [
  { label: "Все", value: undefined },
  { label: "Непрочитанные", value: false },
  { label: "Прочитанные", value: true },
] as const;

type TabValue = (typeof TABS)[number]["value"];

/**
 * Полная страница уведомлений — Linear-стиль.
 * Плоский список без карточек, фильтрация по вкладкам.
 */
export function NotificationsFeature() {
  const [activeTab, setActiveTab] = React.useState<TabValue>(undefined);

  const {
    data,
    isLoading,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteNotifications(activeTab);

  const { mutate: markAsRead } = useMarkAsRead();
  const { mutate: markAllAsRead, isPending: isMarkingAll } = useMarkAllAsRead();

  const notifications = data?.pages.flatMap((p) => p.items) ?? [];
  const totalCount = data?.pages[0]?.totalCount ?? 0;
  const unreadInList = notifications.filter((n) => !n.isRead).length;

  return (
    <div className="mx-auto max-w-2xl">
      {/* Заголовок страницы */}
      <div className="flex items-center justify-between pb-4">
        <div>
          <h1 className="text-lg font-semibold">Уведомления</h1>
          {totalCount > 0 && (
            <p className="text-sm text-muted-foreground">{totalCount} всего</p>
          )}
        </div>
        {unreadInList > 0 && (
          <Button
            variant="ghost"
            size="sm"
            className="text-muted-foreground"
            disabled={isMarkingAll}
            onClick={() => markAllAsRead()}
          >
            Отметить все как прочитанные
          </Button>
        )}
      </div>

      {/* Вкладки-фильтры */}
      <div className="mb-1 flex gap-0.5">
        {TABS.map((tab) => (
          <button
            key={String(tab.value)}
            type="button"
            onClick={() => setActiveTab(tab.value)}
            className={cn(
              "rounded-md px-3 py-1.5 text-sm transition-colors",
              activeTab === tab.value
                ? "bg-accent text-accent-foreground font-medium"
                : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <Separator className="mb-0" />

      {/* Список уведомлений */}
      {isLoading ? (
        <NotificationsListSkeleton />
      ) : notifications.length === 0 ? (
        <EmptyState tab={activeTab} />
      ) : (
        <div className="divide-y divide-border">
          {notifications.map((n) => (
            <NotificationItem
              key={n.id}
              notification={n}
              onMarkAsRead={markAsRead}
            />
          ))}
        </div>
      )}

      {/* Подгрузка */}
      {hasNextPage && (
        <div className="pt-4 text-center">
          <Button
            variant="ghost"
            size="sm"
            className="text-muted-foreground"
            disabled={isFetchingNextPage}
            onClick={() => fetchNextPage()}
          >
            {isFetchingNextPage ? "Загрузка…" : "Показать ещё"}
          </Button>
        </div>
      )}
    </div>
  );
}

// ── Infinite query ───────────────────────────────────────────────────────────

import { useInfiniteQuery } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";
import { notificationKeys } from "@workspace/api-hooks/keys";

const PAGE_SIZE = 25;

function useInfiniteNotifications(isRead: boolean | undefined) {
  return useInfiniteQuery({
    queryKey: notificationKeys.list({ isRead }),
    queryFn: ({ pageParam }) =>
      notificationApiClient.getNotifications({
        pageIndex: pageParam,
        pageSize: PAGE_SIZE,
        isRead,
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) => {
      const loaded = allPages.reduce((acc, p) => acc + p.items.length, 0);

      return loaded < lastPage.totalCount ? allPages.length + 1 : undefined;
    },
  });
}

// ── Вспомогательные компоненты ───────────────────────────────────────────────

function NotificationsListSkeleton() {
  return (
    <div className="divide-y divide-border">
      {Array.from({ length: 5 }).map((_, i) => (
        <div key={i} className="flex items-start gap-3 px-4 py-3">
          <Skeleton className="mt-0.5 size-4 shrink-0 rounded" />
          <div className="flex-1 space-y-1.5">
            <Skeleton className="h-4 w-2/3" />
            <Skeleton className="h-3 w-1/2" />
          </div>
          <Skeleton className="h-3 w-12" />
        </div>
      ))}
    </div>
  );
}

function EmptyState({ tab }: { tab: TabValue }) {
  const message =
    tab === false
      ? "Непрочитанных уведомлений нет"
      : tab === true
        ? "Прочитанных уведомлений нет"
        : "Уведомлений пока нет";

  return (
    <div className="flex flex-col items-center justify-center py-16 text-center">
      <Bell className="mb-3 size-10 text-muted-foreground/30" />
      <p className="text-sm text-muted-foreground">{message}</p>
    </div>
  );
}
