import { Skeleton } from "@workspace/ui/components/skeleton";

export function OrgSettingsSkeleton() {
  return (
    <div className="space-y-5">
      {[3, 4, 2].map((rows, i) => (
        <div
          key={i}
          className="border-border overflow-hidden rounded-2xl border bg-white"
        >
          <div className="flex items-center gap-3.5 border-b px-6 py-4">
            <Skeleton className="size-9 rounded-[10px]" />
            <div className="space-y-1.5">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-3 w-44" />
            </div>
          </div>
          <div className="space-y-4 px-6 py-5">
            {Array.from({ length: rows }).map((_, j) => (
              <div key={j} className="space-y-1.5">
                <Skeleton className="h-3 w-24" />
                <Skeleton className="h-10 w-full" />
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
