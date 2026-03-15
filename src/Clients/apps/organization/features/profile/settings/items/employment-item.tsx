"use client";

import { Trash2 } from "lucide-react";

import { formatDateRange } from "@workspace/utils/format";

import type { EmploymentInput } from "../schema";

export function EmploymentItem({
  employment,
  onRemove,
  isLast,
}: {
  employment: EmploymentInput;
  onRemove: () => void;
  isLast?: boolean;
}) {
  const initial = employment.workplace?.[0]?.toUpperCase() ?? "?";

  return (
    <div
      className={`group border-border overflow-hidden rounded-xl border ${!isLast ? "mb-3" : ""}`}
    >
      <div className="border-border bg-muted/30 flex items-center justify-between border-b px-4 py-3">
        <div className="flex items-center gap-3">
          <div className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-amber-500">
            <span className="text-xs font-bold text-white">{initial}</span>
          </div>
          <div>
            <p className="text-foreground text-sm font-semibold">
              {employment.workplace}
            </p>
            <p className="text-muted-foreground text-xs">
              {employment.position}
              {employment.startDate &&
                ` · ${formatDateRange(employment.startDate, employment.endDate || null)}`}
            </p>
          </div>
        </div>
        <button
          type="button"
          onClick={onRemove}
          aria-label="Удалить"
          className="text-muted-foreground/50 hover:bg-destructive/10 hover:text-destructive flex size-7 items-center justify-center rounded-lg opacity-0 transition-all group-hover:opacity-100"
        >
          <Trash2 className="size-4" />
        </button>
      </div>
      {employment.description && (
        <div className="px-4 py-3">
          <p className="text-muted-foreground text-xs leading-relaxed">
            {employment.description}
          </p>
        </div>
      )}
    </div>
  );
}
