import type { DirectoryUsageDto } from "@workspace/types/organization";
import { cn } from "@workspace/ui/lib/utils";

interface UsageCellProps {
  usage?: readonly DirectoryUsageDto[];
  dim?: boolean;
}

/** Ячейка «Где используется» в таблице справочника. */
export function UsageCell({ usage, dim }: Readonly<UsageCellProps>) {
  if (!usage || usage.length === 0) {
    return <span className="text-sm text-slate-400">не используется</span>;
  }

  const total = usage.reduce((sum, u) => sum + u.count, 0);
  if (total === 0) {
    return <span className="text-sm text-slate-400">не используется</span>;
  }

  return (
    <div
      className={cn(
        "flex items-center gap-3 tabular-nums",
        dim ? "text-slate-400" : "text-slate-500",
      )}
    >
      {usage.map((u) => (
        <span key={u.label} className="inline-flex items-baseline gap-1">
          <strong
            className={cn(
              "font-semibold",
              dim ? "text-slate-400" : "text-slate-900",
            )}
          >
            {u.count}
          </strong>
          <span className="text-xs text-slate-400">{u.label}</span>
        </span>
      ))}
    </div>
  );
}
