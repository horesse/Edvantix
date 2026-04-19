import type { ReactNode } from "react";

import { cn } from "@workspace/ui/lib/utils";

interface ReviewSectionProps {
  title: string;
  onEdit: () => void;
  children: ReactNode;
}

export function ReviewSection({ title, onEdit, children }: ReviewSectionProps) {
  return (
    <div className="bg-card border-border overflow-hidden rounded-2xl border">
      <div className="border-border flex items-center justify-between border-b px-5 py-3.5">
        <h3 className="text-foreground text-[14px] font-semibold">{title}</h3>
        <button
          type="button"
          onClick={onEdit}
          className="text-brand-600 hover:bg-brand-50 rounded-md px-2 py-1 text-[12.5px] font-medium transition-colors"
        >
          Изменить
        </button>
      </div>
      <div className="divide-border/50 divide-y px-5">{children}</div>
    </div>
  );
}

interface ReviewRowProps {
  label: string;
  value?: ReactNode;
  empty?: string;
}

export function ReviewRow({ label, value, empty }: ReviewRowProps) {
  return (
    <div className="grid grid-cols-[180px_1fr] gap-4 py-[7px] text-[13.5px]">
      <div className="text-muted-foreground">{label}</div>
      <div
        className={cn(
          "font-medium",
          value ? "text-foreground" : "text-slate-300",
        )}
      >
        {value ??
          (empty ? (
            <span className="font-normal text-slate-400">{empty}</span>
          ) : (
            "—"
          ))}
      </div>
    </div>
  );
}
