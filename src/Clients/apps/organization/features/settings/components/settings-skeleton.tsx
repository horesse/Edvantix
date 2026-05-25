import { Skeleton } from "@workspace/ui/components/skeleton";

export function SettingsSkeleton() {
  return (
    <div className="flex max-w-[1180px] flex-col gap-8">
      {/* Org section skeleton */}
      <div>
        <Skeleton className="mb-3.5 h-5 w-48 rounded-md" />
        <Skeleton className="h-[172px] w-full rounded-2xl" />
      </div>

      {/* Directories section skeleton */}
      <div>
        <Skeleton className="mb-3.5 h-5 w-56 rounded-md" />
        <div className="grid gap-3.5 [grid-template-columns:repeat(auto-fill,minmax(280px,1fr))]">
          {Array.from({ length: 8 }, (_, i) => (
            <Skeleton key={`dir-skeleton-${i}`} className="h-[148px] rounded-[14px]" />
          ))}
        </div>
      </div>

      {/* Access section skeleton */}
      <div>
        <Skeleton className="mb-3.5 h-5 w-32 rounded-md" />
        <Skeleton className="h-20 w-full rounded-2xl" />
      </div>

      {/* Platform section skeleton */}
      <div>
        <Skeleton className="mb-3.5 h-5 w-40 rounded-md" />
        <div className="grid gap-3.5 [grid-template-columns:repeat(auto-fill,minmax(280px,1fr))]">
          {Array.from({ length: 6 }, (_, i) => (
            <Skeleton key={`plat-skeleton-${i}`} className="h-[118px] rounded-[14px]" />
          ))}
        </div>
      </div>
    </div>
  );
}
