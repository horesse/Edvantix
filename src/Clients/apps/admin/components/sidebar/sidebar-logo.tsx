import Link from "next/link";

import { cn } from "@workspace/ui/lib/utils";

interface SidebarLogoProps {
  className?: string;
}

export function SidebarLogo({ className }: Readonly<SidebarLogoProps>) {
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
      <div className="min-w-0">
        <span className="text-foreground block text-base leading-none font-bold tracking-tight">
          Edvantix
        </span>
        <span className="text-muted-foreground block text-[10px] font-medium tracking-wider uppercase">
          Admin
        </span>
      </div>
    </Link>
  );
}
