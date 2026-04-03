"use client";

import { Users } from "lucide-react";

import useAdminProfiles from "@workspace/api-hooks/admin/useAdminProfiles";
import { Skeleton } from "@workspace/ui/components/skeleton";

export function DashboardPage() {
  // Fetch with pageSize=1 — we only need totalCount
  const { data, isLoading } = useAdminProfiles({ pageIndex: 1, pageSize: 1 });

  const total = data?.totalCount ?? 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-foreground text-2xl font-bold tracking-tight">Дашборд</h1>
        <p className="text-muted-foreground mt-1 text-sm">Обзор системы</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <div className="bg-card border-border flex items-center gap-4 rounded-xl border p-5 shadow-sm">
          <div className="flex size-12 shrink-0 items-center justify-center rounded-xl bg-blue-50">
            <Users className="size-6 text-blue-600" />
          </div>
          <div>
            <p className="text-muted-foreground text-sm">Всего пользователей</p>
            {isLoading ? (
              <Skeleton className="mt-1 h-7 w-16" />
            ) : (
              <p className="text-foreground text-3xl font-bold tabular-nums">{total}</p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
