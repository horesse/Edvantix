import { PostStatus } from "@workspace/types/blog";

const STATUS_CONFIG = {
  [PostStatus.Draft]: {
    label: "Draft",
    color: "bg-muted-foreground",
    text: "text-muted-foreground",
  },
  [PostStatus.Scheduled]: {
    label: "Scheduled",
    color: "bg-amber-500",
    text: "text-amber-600 dark:text-amber-400",
  },
  [PostStatus.Published]: {
    label: "Published",
    color: "bg-green-500",
    text: "text-green-600 dark:text-green-400",
  },
  [PostStatus.Archived]: {
    label: "Archived",
    color: "bg-destructive",
    text: "text-destructive",
  },
} as const;

type StatusBarProps = Readonly<{
  status: PostStatus;
  count: number | null;
  total: number;
}>;

export function StatusBar({ status, count, total }: StatusBarProps) {
  const cfg = STATUS_CONFIG[status];
  const pct = total > 0 && count !== null ? Math.round((count / total) * 100) : 0;

  return (
    <div className="flex items-center gap-3">
      <span className={`w-20 shrink-0 text-xs font-medium ${cfg.text}`}>
        {cfg.label}
      </span>
      <div className="bg-muted h-2 flex-1 overflow-hidden rounded-full">
        <div
          className={`h-full rounded-full ${cfg.color} transition-all duration-700 ease-out`}
          style={{ width: `${pct}%` }}
        />
      </div>
      <span className="text-muted-foreground w-8 text-right font-mono text-xs">
        {count ?? "—"}
      </span>
    </div>
  );
}
