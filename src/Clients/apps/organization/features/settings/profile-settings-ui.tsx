"use client";

import { Skeleton } from "@workspace/ui/components/skeleton";

export function EmptyState({
  icon,
  text,
  onAdd,
}: {
  icon: React.ReactNode;
  text: string;
  onAdd: () => void;
}) {
  return (
    <button
      type="button"
      onClick={onAdd}
      className="flex w-full flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border/50 py-5 transition-all hover:border-border hover:bg-muted/20"
    >
      <span className="text-muted-foreground/30">{icon}</span>
      <span className="text-xs text-muted-foreground">{text}</span>
    </button>
  );
}

const SECTION_COL = "md:grid-cols-[240px_1fr]";

export function ProfileSettingsSkeleton() {
  return (
    <div>
      {/* Avatar */}
      <div className="flex items-center gap-4 pb-6">
        <Skeleton className="size-14 shrink-0 rounded-full" />
        <div className="space-y-2">
          <Skeleton className="h-4 w-36" />
          <Skeleton className="h-3 w-24" />
        </div>
      </div>

      {/* Personal info */}
      <div className={`grid gap-8 border-t border-border/40 py-6 ${SECTION_COL}`}>
        <div className="space-y-2">
          <Skeleton className="h-4 w-32" />
          <Skeleton className="h-3 w-44" />
        </div>
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-9 w-full rounded-md" />
          ))}
        </div>
      </div>

      {/* Contacts / Employment / Education */}
      {[3, 2, 2].map((count, i) => (
        <div key={i} className={`grid gap-8 border-t border-border/40 py-6 ${SECTION_COL}`}>
          <div className="space-y-2">
            <Skeleton className="h-4 w-28" />
            <Skeleton className="h-3 w-40" />
          </div>
          <div className="space-y-2">
            <div className="flex justify-end">
              <Skeleton className="h-7 w-24 rounded-md" />
            </div>
            {Array.from({ length: count }).map((_, j) => (
              <Skeleton key={j} className="h-12 w-full rounded-lg" />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
