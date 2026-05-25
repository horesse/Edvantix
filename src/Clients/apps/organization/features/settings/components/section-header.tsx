import type { LucideIcon } from "lucide-react";

interface SectionHeaderProps {
  readonly Icon: LucideIcon;
  readonly title: string;
  readonly subtitle?: string;
  readonly action?: React.ReactNode;
}

export function SectionHeader({
  Icon,
  title,
  subtitle,
  action,
}: SectionHeaderProps) {
  return (
    <div className="mb-3.5 flex items-end gap-4">
      <div className="min-w-0 flex-1">
        <div className="mb-1 inline-flex items-center gap-2 text-[11px] font-semibold tracking-[0.08em] text-slate-500 uppercase">
          <Icon className="size-3.5 text-slate-500" />
          {title}
        </div>
        {subtitle && (
          <div className="text-[13.5px] text-slate-400">{subtitle}</div>
        )}
      </div>
      {action}
    </div>
  );
}
