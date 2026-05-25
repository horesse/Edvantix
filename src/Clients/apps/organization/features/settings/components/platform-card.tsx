import type { PlatformItem } from "../constants";
import { PLATFORM_TONE_COLORS } from "../constants";

interface PlatformCardProps {
  readonly item: PlatformItem;
}

export function PlatformCard({ item }: PlatformCardProps) {
  const Icon = item.icon;
  const colors = PLATFORM_TONE_COLORS[item.tone];

  return (
    <div className="group cursor-pointer rounded-[14px] border border-slate-200 bg-white p-[18px] opacity-[0.95] shadow-[0_1px_2px_rgba(15,23,42,0.03)] transition-all duration-150 hover:shadow-[0_4px_12px_rgba(15,23,42,0.05)]">
      {/* Header */}
      <div className="mb-3 flex items-center gap-3">
        <div
          className="flex size-[34px] shrink-0 items-center justify-center rounded-[10px]"
          style={{ background: colors.bg, color: colors.fg }}
        >
          <Icon className="size-[17px]" />
        </div>
        <h3 className="min-w-0 flex-1 text-[14px] font-semibold text-slate-900">
          {item.name}
        </h3>
        <span className="rounded border border-dashed border-slate-200 bg-slate-50 px-1.5 py-[2px] text-[10px] font-semibold tracking-[0.06em] text-slate-400 uppercase">
          скоро
        </span>
      </div>

      {/* Description */}
      <p className="min-h-[34px] text-[12.5px] leading-relaxed text-slate-500">
        {item.description}
      </p>

      {/* Meta */}
      {item.meta && (
        <div className="mt-3 border-t border-slate-50 pt-2.5 text-[12px] font-medium text-slate-600">
          {item.meta}
        </div>
      )}
    </div>
  );
}
