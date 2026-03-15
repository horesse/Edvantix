import type React from "react";

interface SectionCardProps {
  iconBg: string;
  iconColor: string;
  icon: React.ElementType;
  title: string;
  subtitle: string;
  action?: React.ReactNode;
  children: React.ReactNode;
}

export function SectionCard({
  icon: Icon,
  iconBg,
  iconColor,
  title,
  subtitle,
  action,
  children,
}: SectionCardProps) {
  return (
    <div className="bg-card border-border rounded-2xl border p-6 shadow-sm">
      <div className="mb-5 flex items-center justify-between">
        <div className="flex items-center gap-2.5">
          <div
            className={`flex size-8 shrink-0 items-center justify-center rounded-xl ${iconBg}`}
          >
            <Icon className={`size-4 ${iconColor}`} />
          </div>
          <div>
            <p className="text-foreground text-sm font-semibold">{title}</p>
            <p className="text-muted-foreground text-xs">{subtitle}</p>
          </div>
        </div>
        {action}
      </div>
      {children}
    </div>
  );
}
