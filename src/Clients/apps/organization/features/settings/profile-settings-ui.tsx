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
      className="border-border/50 hover:border-border hover:bg-muted/20 flex w-full flex-col items-center justify-center gap-2 rounded-lg border border-dashed py-5 transition-all"
    >
      <span className="text-muted-foreground/30">{icon}</span>
      <span className="text-muted-foreground text-xs">{text}</span>
    </button>
  );
}

export function ProfileSettingsSkeleton() {
  return (
    <div className="space-y-6">
      {/* Avatar */}
      <div className="flex items-center gap-5">
        <Skeleton className="size-16 shrink-0 rounded-full" />
        <div className="space-y-2">
          <Skeleton className="h-4 w-36" />
          <Skeleton className="h-3 w-24" />
        </div>
      </div>

      {/* Vertical tabs layout */}
      <div className="flex flex-col gap-6 md:flex-row md:gap-8">
        {/* Sidebar skeleton */}
        <div className="flex w-full shrink-0 flex-row gap-1 md:w-48 md:flex-col md:gap-0.5">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-8 w-full rounded-md" />
          ))}
        </div>

        {/* Content skeleton */}
        <div className="min-w-0 flex-1 space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            {Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="space-y-2">
                <Skeleton className="h-3 w-20" />
                <Skeleton className="h-9 w-full rounded-md" />
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
