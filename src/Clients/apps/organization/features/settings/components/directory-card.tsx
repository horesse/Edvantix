import Link from "next/link";

import { ChevronRight, Clock } from "lucide-react";
import type { LucideIcon } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

import { declRecords, relativeDate } from "../lib/declension";

interface DirectoryCardProps {
  readonly code: string;
  readonly name: string;
  readonly description: string;
  readonly Icon: LucideIcon;
  readonly badge?: string | null;
  readonly activeCount: number;
  readonly archivedCount: number;
  readonly lastModifiedAt?: string | null;
  readonly href?: string;
  readonly isAvailable: boolean;
}

export function DirectoryCard({
  name,
  description,
  Icon,
  badge,
  activeCount,
  archivedCount,
  lastModifiedAt,
  href,
  isAvailable,
}: DirectoryCardProps) {
  const relative = relativeDate(lastModifiedAt);
  const isClickable = isAvailable && Boolean(href);

  const inner = (
    <div
      className={cn(
        "group flex flex-col rounded-[14px] border border-slate-200 bg-white p-5 shadow-[0_1px_2px_rgba(15,23,42,0.03)] transition-all duration-150",
        isClickable &&
          "cursor-pointer hover:border-indigo-200 hover:shadow-[0_4px_16px_rgba(79,70,229,0.10),0_0_0_1px_rgba(79,70,229,0.06)]",
        !isClickable && "cursor-default opacity-[0.92]",
      )}
    >
      {/* Header */}
      <div className="mb-3 flex items-center gap-3">
        <div
          className={cn(
            "flex size-9 shrink-0 items-center justify-center rounded-[10px] text-indigo-700 transition-colors duration-150",
            "bg-indigo-50/80",
            isClickable && "group-hover:bg-indigo-100",
          )}
        >
          <Icon className="size-[18px]" />
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex items-center gap-2">
            <h3 className="truncate text-[14.5px] font-semibold tracking-[-0.01em] text-slate-900">
              {name}
            </h3>
            {badge && (
              <span className="rounded bg-slate-100 px-1.5 py-[2px] text-[10px] font-semibold tracking-[0.06em] text-slate-500 uppercase">
                {badge}
              </span>
            )}
          </div>
        </div>
        <ChevronRight
          className={cn(
            "size-4 shrink-0 text-slate-300 transition-colors duration-150",
            isClickable && "group-hover:text-indigo-400",
          )}
        />
      </div>

      {/* Description */}
      <p className="mb-3.5 min-h-9 text-[12.5px] leading-relaxed text-slate-500">
        {description}
      </p>

      {/* Footer stats */}
      <div className="flex items-center gap-3 border-t border-slate-50 pt-2.5 text-xs text-slate-400 tabular-nums">
        <span className="inline-flex items-baseline gap-1">
          <strong className="text-[13px] font-semibold text-slate-900">
            {activeCount}
          </strong>
          {declRecords(activeCount)}
          {archivedCount > 0 && (
            <span className="ml-1 text-slate-300">
              +{archivedCount} в архиве
            </span>
          )}
        </span>
        {relative && (
          <>
            <span className="text-slate-200">·</span>
            <span className="inline-flex items-center gap-1">
              <Clock className="size-[11px] text-slate-300" />
              {relative}
            </span>
          </>
        )}
      </div>
    </div>
  );

  if (isClickable && href) {
    return <Link href={href}>{inner}</Link>;
  }

  return inner;
}
