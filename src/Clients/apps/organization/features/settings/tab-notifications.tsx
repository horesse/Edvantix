"use client";

import { Bell } from "lucide-react";

export function TabNotifications() {
  return (
    <div className="flex flex-col items-center justify-center gap-4 py-16 text-center">
      <div className="flex size-12 items-center justify-center rounded-xl bg-muted">
        <Bell className="size-5 text-muted-foreground" />
      </div>
      <div className="space-y-1">
        <p className="text-sm font-medium">Уведомления</p>
        <p className="max-w-sm text-xs text-muted-foreground">
          Настройки уведомлений появятся здесь. Вы сможете управлять email-уведомлениями,
          push-уведомлениями и уведомлениями в приложении.
        </p>
      </div>
      <span className="inline-flex items-center rounded-md bg-muted px-2 py-0.5 text-[10px] font-medium text-muted-foreground">
        TODO
      </span>
    </div>
  );
}
