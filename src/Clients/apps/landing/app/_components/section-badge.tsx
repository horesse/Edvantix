import type { ReactNode } from "react";

import type { LucideIcon } from "lucide-react";

/** Shared h2 className used across section headings. */
export const SECTION_H2_CLASS =
  "text-card-foreground mb-4 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl";

/** Large primary CTA button — Hero and Cta sections. */
export const PRIMARY_BTN_CLASS =
  "bg-primary hover:bg-primary/90 text-primary-foreground shadow-primary/25 hover:shadow-primary/40 focus-visible:ring-ring focus-visible:ring-offset-background h-12 px-8 text-base shadow-xl transition-all duration-300 hover:scale-[1.02] focus-visible:ring-2 focus-visible:ring-offset-2";

/** Outline secondary button — Hero and Cta sections. */
export const OUTLINE_BTN_CLASS =
  "border-border bg-muted/30 hover:bg-muted/60 text-foreground hover:border-border focus-visible:ring-ring h-12 px-8 text-base transition-all duration-300 focus-visible:ring-2";

/** Pill-shaped primary-coloured badge used at the top of each section. */
export function SectionBadge({
  icon: Icon,
  children,
}: {
  icon: LucideIcon;
  children: ReactNode;
}) {
  return (
    <div className="border-primary/20 bg-primary/5 text-primary mb-4 inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-medium">
      <Icon className="h-3 w-3" aria-hidden="true" />
      {children}
    </div>
  );
}
