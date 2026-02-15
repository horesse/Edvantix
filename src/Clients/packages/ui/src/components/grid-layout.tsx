import * as React from "react";

import { cn } from "../lib/utils";

export interface GridLayoutProps extends React.HTMLAttributes<HTMLDivElement> {
  gap?: "sm" | "md" | "lg";
}

const GridLayout = React.forwardRef<HTMLDivElement, GridLayoutProps>(
  ({ className, gap = "md", children, ...props }, ref) => {
    const gapClasses = {
      sm: "gap-3",
      md: "gap-4",
      lg: "gap-6",
    };

    return (
      <div
        ref={ref}
        className={cn(
          "grid h-full grid-cols-1 lg:grid-cols-[auto_1fr]",
          gapClasses[gap],
          className,
        )}
        {...props}
      >
        {children}
      </div>
    );
  },
);
GridLayout.displayName = "GridLayout";

export interface MainAreaProps extends React.HTMLAttributes<HTMLDivElement> {
  gap?: "sm" | "md" | "lg";
}

const MainArea = React.forwardRef<HTMLDivElement, MainAreaProps>(
  ({ className, gap = "md", children, ...props }, ref) => {
    const gapClasses = {
      sm: "gap-3",
      md: "gap-4",
      lg: "gap-5",
    };

    return (
      <div
        ref={ref}
        className={cn("flex h-full flex-col overflow-hidden", gapClasses[gap], className)}
        {...props}
      >
        {children}
      </div>
    );
  },
);
MainArea.displayName = "MainArea";

export interface ContentAreaProps extends React.HTMLAttributes<HTMLDivElement> {}

const ContentArea = React.forwardRef<HTMLDivElement, ContentAreaProps>(
  ({ className, children, ...props }, ref) => (
    <div
      ref={ref}
      className={cn(
        "flex-1 overflow-y-auto rounded-2xl bg-card p-4 shadow-sm lg:p-5",
        className,
      )}
      {...props}
    >
      {children}
    </div>
  ),
);
ContentArea.displayName = "ContentArea";

export { GridLayout, MainArea, ContentArea };
