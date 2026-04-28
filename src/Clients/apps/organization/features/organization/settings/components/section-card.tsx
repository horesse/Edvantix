import type { LucideIcon } from "lucide-react";

interface SectionCardProps {
  icon: LucideIcon;
  title: string;
  subtitle?: string;
  rightSlot?: React.ReactNode;
  children: React.ReactNode;
}

export function SectionCard({
  icon: Icon,
  title,
  subtitle,
  rightSlot,
  children,
}: Readonly<SectionCardProps>) {
  return (
    <section className="border-border overflow-hidden rounded-2xl border bg-white">
      <header className="flex items-center gap-3.5 border-b border-slate-100/80 px-6 py-4">
        <div className="flex size-9 shrink-0 items-center justify-center rounded-[10px] bg-indigo-50 text-indigo-600">
          <Icon className="size-[18px]" />
        </div>
        <div className="min-w-0 flex-1">
          <h2 className="text-[15px] font-semibold tracking-tight text-slate-900">
            {title}
          </h2>
          {subtitle && (
            <p className="mt-0.5 text-[12.5px] text-slate-500">{subtitle}</p>
          )}
        </div>
        {rightSlot}
      </header>
      <div className="px-6 py-5">{children}</div>
    </section>
  );
}
