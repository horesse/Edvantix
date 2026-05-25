"use client";

import useOrganizationMembersKpi from "@workspace/api-hooks/organization/useOrganizationMembersKpi";
import { OrganizationStatus } from "@workspace/types/organization";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { KPI_ITEMS } from "./members-constants";

interface KpiCardProps {
  label: string;
  value: number | undefined;
  delta: string;
  icon: React.ElementType;
  iconBg: string;
  iconColor: string;
  isLoading: boolean;
}

function KpiCard({
  label,
  value,
  delta,
  icon: Icon,
  iconBg,
  iconColor,
  isLoading,
}: Readonly<KpiCardProps>) {
  return (
    <div className="bg-card border-border flex items-center gap-3.5 rounded-2xl border p-4 shadow-sm sm:p-5">
      <div
        className={`flex size-10 shrink-0 items-center justify-center rounded-xl ${iconBg}`}
      >
        <Icon className={`size-5 ${iconColor}`} />
      </div>
      <div className="min-w-0 flex-1">
        <p className="text-muted-foreground truncate text-xs font-medium">
          {label}
        </p>
        <div className="mt-1 flex min-w-0 items-baseline gap-2">
          {isLoading ? (
            <Skeleton className="h-7 w-10" />
          ) : (
            <span className="text-foreground text-2xl leading-none font-bold tracking-tight tabular-nums">
              {value ?? 0}
            </span>
          )}
          {!isLoading && (
            <span className="text-muted-foreground min-w-0 truncate text-[11px] font-medium">
              {delta}
            </span>
          )}
        </div>
      </div>
    </div>
  );
}

/** Четыре KPI-карточки: всего, активные, архив, удалены. */
export function MembersKpiCards({ orgId }: Readonly<{ orgId: string }>) {
  const { data, isLoading } = useOrganizationMembersKpi(orgId);

  const countByStatus: Record<string, number | undefined> = {
    total: data?.total,
    [OrganizationStatus.Active]: data?.active,
    [OrganizationStatus.Archived]: data?.archived,
    [OrganizationStatus.Deleted]: data?.deleted,
  };

  return (
    <div className="grid grid-cols-2 gap-3 sm:gap-4 lg:grid-cols-4">
      {KPI_ITEMS.map((kpi) => (
        <KpiCard
          key={kpi.label}
          label={kpi.label}
          value={countByStatus[kpi.status ?? "total"]}
          delta={kpi.delta}
          icon={kpi.icon}
          iconBg={kpi.iconBg}
          iconColor={kpi.iconColor}
          isLoading={isLoading}
        />
      ))}
    </div>
  );
}
