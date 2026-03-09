"use client";

import { Bell } from "lucide-react";

export function TabNotifications() {
  return (
    <div className="flex flex-col items-center justify-center gap-4 py-16 text-center">
      <div className="bg-muted flex size-12 items-center justify-center rounded-xl">
        <Bell className="text-muted-foreground size-5" />
      </div>
      <div className="space-y-1">
        <p className="text-sm font-medium">Уведомления</p>
        <p className="text-muted-foreground max-w-sm text-xs">
          Настройки уведомлений появятся здесь. Вы сможете управлять
          email-уведомлениями, push-уведомлениями и уведомлениями в приложении.
        </p>
      </div>
      <span className="bg-muted text-muted-foreground inline-flex items-center rounded-md px-2 py-0.5 text-[10px] font-medium">
        TODO
      </span>
    </div>
  );
}
