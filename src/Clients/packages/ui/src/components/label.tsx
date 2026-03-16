"use client";

import * as React from "react";

import * as LabelPrimitive from "@radix-ui/react-label";

import { cn } from "@workspace/ui/lib/utils";

function Label({
  className,
  ...props
}: React.ComponentProps<typeof LabelPrimitive.Root>) {
  return (
    <LabelPrimitive.Root
      data-slot="label"
      className={cn(
        /* text-xs / font-medium / text-muted-foreground matches design: labels above form fields are 12px, slate-500 in light / slate-400 in dark */
        "text-muted-foreground flex items-center gap-2 text-xs leading-none font-medium select-none group-data-[disabled=true]:pointer-events-none group-data-[disabled=true]:opacity-50 peer-disabled:cursor-not-allowed peer-disabled:opacity-50",
        className,
      )}
      {...props}
    />
  );
}

export { Label };
