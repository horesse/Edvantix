"use client";

import * as React from "react";

import {
  Bell,
  CheckCircle2,
  Info,
  OctagonX,
  Star,
  Triangle,
  UserPlus,
} from "lucide-react";

import { NotificationType } from "@workspace/types/notifications";
import type { Notification } from "@workspace/types/notifications";
import { cn } from "@workspace/ui/lib/utils";

// ── Иконки и цвета по типу уведомления ──────────────────────────────────────

const typeConfig: Record<
  NotificationType,
  { icon: React.ElementType; className: string }
> = {
  [NotificationType.Info]: {
    icon: Info,
    className: "text-blue-500",
  },
  [NotificationType.Success]: {
    icon: CheckCircle2,
    className: "text-emerald-500",
  },
  [NotificationType.Warning]: {
    icon: Triangle,
    className: "text-amber-500",
  },
  [NotificationType.Error]: {
    icon: OctagonX,
    className: "text-red-500",
  },
  [NotificationType.Invitation]: {
    icon: UserPlus,
    className: "text-violet-500",
  },
  [NotificationType.Achievement]: {
    icon: Star,
    className: "text-yellow-500",
  },
  [NotificationType.System]: {
    icon: Bell,
    className: "text-muted-foreground",
  },
};

// ── Относительное время ──────────────────────────────────────────────────────

function formatRelativeTime(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diff / 60_000);

  if (minutes < 1) return "только что";
  if (minutes < 60) return `${minutes} мин назад`;

  const hours = Math.floor(minutes / 60);

  if (hours < 24) return `${hours} ч назад`;

  const days = Math.floor(hours / 24);

  if (days === 1) return "вчера";
  if (days < 7) return `${days} д назад`;

  return new Date(iso).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "short",
  });
}

// ── Компонент ────────────────────────────────────────────────────────────────

type NotificationItemProps = {
  notification: Notification;
  onMarkAsRead?: (id: string) => void;
  compact?: boolean;
};

/**
 * Одно уведомление в стиле Linear — минималистичная строка с иконкой,
 * заголовком, временем и индикатором непрочитанного.
 */
export function NotificationItem({
  notification,
  onMarkAsRead,
  compact = false,
}: NotificationItemProps) {
  const config = typeConfig[notification.type] ?? typeConfig[NotificationType.Info];
  const Icon = config.icon;

  const handleClick = () => {
    if (!notification.isRead) {
      onMarkAsRead?.(notification.id);
    }
  };

  return (
    <button
      type="button"
      onClick={handleClick}
      className={cn(
        "group flex w-full items-start gap-3 px-4 text-left transition-colors",
        compact ? "py-2.5" : "py-3",
        notification.isRead
          ? "hover:bg-accent/50"
          : "hover:bg-accent",
      )}
    >
      {/* Иконка типа */}
      <span
        className={cn(
          "mt-0.5 shrink-0",
          config.className,
        )}
      >
        <Icon className="size-4" />
      </span>

      {/* Текст */}
      <span className="min-w-0 flex-1">
        <span
          className={cn(
            "block truncate text-sm",
            notification.isRead
              ? "text-muted-foreground font-normal"
              : "text-foreground font-medium",
          )}
        >
          {notification.title}
        </span>
        {!compact && (
          <span className="mt-0.5 block truncate text-xs text-muted-foreground">
            {notification.message}
          </span>
        )}
      </span>

      {/* Правая часть: время + точка непрочитанного */}
      <span className="ml-auto flex shrink-0 flex-col items-end gap-1">
        <span className="text-[11px] tabular-nums text-muted-foreground/70">
          {formatRelativeTime(notification.createdAt)}
        </span>
        {!notification.isRead && (
          <span className="size-1.5 rounded-full bg-primary" />
        )}
      </span>
    </button>
  );
}
