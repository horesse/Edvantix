import Link from "next/link";

import { cn } from "@workspace/ui/lib/utils";

interface SidebarLogoProps {
  className?: string;
}

/** Edvantix brand logo with icon + wordmark. */
export function SidebarLogo({ className }: SidebarLogoProps) {
  return (
    <Link href="/" className={cn("flex items-center gap-2.5", className)}>
      <div className="bg-primary flex size-8 shrink-0 items-center justify-center rounded-lg">
        <svg width="18" height="18" fill="none" viewBox="0 0 24 24">
          <path
            d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"
            stroke="white"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>
      <span className="text-foreground text-base font-bold tracking-tight">
        Edvantix
      </span>
    </Link>
  );
}
