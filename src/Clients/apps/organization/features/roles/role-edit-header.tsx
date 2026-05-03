"use client";

import { Lock, Trash2 } from "lucide-react";

import type { RoleDetailDto } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";

interface RoleEditHeaderProps {
  role: RoleDetailDto;
  name: string;
  totalGranted: number;
  totalPerms: number;
  pct: number;
  colors: { bg: string; fg: string };
  canManage: boolean;
  onBack: () => void;
  onDeleteOpen: () => void;
}

export function RoleEditHeader({
  role,
  name,
  totalGranted,
  totalPerms,
  pct,
  colors,
  canManage,
  onBack,
  onDeleteOpen,
}: Readonly<RoleEditHeaderProps>) {
  return (
    <div className="space-y-3">
      <Button
        variant="ghost"
        size="sm"
        className="text-muted-foreground -ml-2 h-7 gap-1.5 px-2 text-xs"
        onClick={onBack}
      >
        ← Роли и права
      </Button>
      <div className="flex items-center gap-4">
        <div
          className="flex size-12 shrink-0 items-center justify-center rounded-xl text-lg font-bold"
          style={{ background: colors.bg, color: colors.fg }}
        >
          {(name || role.name).charAt(0)}
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex items-center gap-2">
            <h1 className="text-xl font-bold tracking-tight">{role.name}</h1>
            {role.isSystem && (
              <span className="inline-flex items-center gap-1 rounded-full bg-slate-100 px-2 py-0.5 text-[11px] font-medium text-slate-500">
                <Lock className="size-2.5" />
                системная
              </span>
            )}
          </div>
          <p className="text-muted-foreground text-sm">
            {role.membersCount} участников · {totalGranted} из {totalPerms} прав
            ({pct}%)
          </p>
        </div>
        {!role.isSystem && canManage && (
          <Button
            variant="outline"
            size="sm"
            className="text-destructive hover:text-destructive border-red-200 hover:bg-red-50"
            onClick={onDeleteOpen}
          >
            <Trash2 className="size-4" />
            Удалить роль
          </Button>
        )}
      </div>
    </div>
  );
}

export function RoleEditHeaderSkeleton() {
  return (
    <div className="flex items-center gap-4">
      <Skeleton className="size-12 rounded-xl" />
      <div className="space-y-2">
        <Skeleton className="h-5 w-40" />
        <Skeleton className="h-4 w-56" />
      </div>
    </div>
  );
}
