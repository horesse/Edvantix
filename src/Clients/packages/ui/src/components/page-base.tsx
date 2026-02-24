import * as React from "react";

import { cn } from "../lib/utils";

export interface PageBaseProps extends React.HTMLAttributes<HTMLDivElement> {
  padding?: "sm" | "md" | "lg";
}

const PageBase = React.forwardRef<HTMLDivElement, PageBaseProps>(
  ({ className, padding = "lg", children, ...props }, ref) => {
    const paddingClasses = {
      sm: "p-4",
      md: "p-6",
      lg: "p-8",
    };

    return (
      <div
        ref={ref}
        className={cn(
          "bg-background h-screen overflow-hidden",
          paddingClasses[padding],
          className,
        )}
        {...props}
      >
        {children}
      </div>
    );
  },
);
PageBase.displayName = "PageBase";

export { PageBase };
