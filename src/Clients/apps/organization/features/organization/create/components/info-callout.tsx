import type { ElementType } from "react";

import { cn } from "@workspace/ui/lib/utils";

type CalloutVariant = "primary" | "success" | "neutral";

const VARIANT_STYLES: Record<
  CalloutVariant,
  { wrapper: string; iconBg: string; iconColor: string }
> = {
  primary: {
    wrapper: "bg-brand-50/50 border-brand-200",
    iconBg: "bg-brand-100",
    iconColor: "text-brand-600",
  },
  success: {
    wrapper: "bg-emerald-50 border-emerald-200",
    iconBg: "bg-emerald-100",
    iconColor: "text-emerald-700",
  },
  neutral: {
    wrapper: "bg-muted/50 border-border",
    iconBg: "bg-slate-100",
    iconColor: "text-slate-500",
  },
};

interface InfoCalloutProps {
  variant?: CalloutVariant;
  icon: ElementType;
  title: string;
  description?: string;
}

export function InfoCallout({
  variant = "primary",
  icon: Icon,
  title,
  description,
}: InfoCalloutProps) {
  const s = VARIANT_STYLES[variant];
  return (
    <div className={cn("flex gap-3 rounded-xl border p-3.5", s.wrapper)}>
      <div
        className={cn(
          "flex size-8 shrink-0 items-center justify-center rounded-lg",
          s.iconBg,
        )}
      >
        <Icon className={cn("size-4", s.iconColor)} />
      </div>
      <div className="min-w-0 flex-1">
        <div className="text-foreground text-[13.5px] font-semibold">
          {title}
        </div>
        {description && (
          <div className="text-muted-foreground mt-0.5 text-[13px] leading-relaxed">
            {description}
          </div>
        )}
      </div>
    </div>
  );
}
