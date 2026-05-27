import { cn } from "@workspace/ui/lib/utils";

interface StatusBadgeProps {
  isArchived: boolean;
  className?: string;
}

/** Бейдж статуса элемента справочника: «Активный» / «В архиве». */
export function StatusBadge({ isArchived, className }: Readonly<StatusBadgeProps>) {
  if (isArchived) {
    return (
      <span
        className={cn(
          "inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium",
          "bg-slate-100 text-slate-500",
          className,
        )}
      >
        <span className="size-1.5 rounded-full bg-slate-400" />
        В архиве
      </span>
    );
  }

  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium",
        "bg-emerald-100 text-emerald-700",
        className,
      )}
    >
      <span className="size-1.5 rounded-full bg-emerald-500" />
      Активный
    </span>
  );
}
