import * as React from "react";

import { AnimatePresence } from "framer-motion";

import { cn } from "../lib/utils";

export interface IslandLayoutProps extends React.HTMLAttributes<HTMLDivElement> {
  columns?: 2 | 3;
  centerIsland?: React.ReactNode;
  showCenter?: boolean;
}

const IslandLayout = React.forwardRef<HTMLDivElement, IslandLayoutProps>(
  (
    {
      className,
      children,
      columns = 2,
      centerIsland,
      showCenter = false,
      ...props
    },
    ref,
  ) => {
    const childrenArray = React.Children.toArray(children);

    if (columns === 3 || (columns === 2 && showCenter && centerIsland)) {
      return (
        <div
          ref={ref}
          className={cn(
            "grid gap-4",
            "grid-cols-1",
            "lg:grid-cols-[1fr_auto_1fr]",
            "md:grid-cols-2",
            className,
          )}
          {...props}
        >
          <div className="flex flex-col gap-4">{childrenArray[0]}</div>
          <AnimatePresence mode="wait">
            {showCenter && centerIsland && (
              <div className="hidden w-100 flex-col gap-4 lg:flex">
                {centerIsland}
              </div>
            )}
          </AnimatePresence>
          <div className="flex flex-col gap-4">{childrenArray[1]}</div>
        </div>
      );
    }

    return (
      <div
        ref={ref}
        className={cn("grid gap-4", "grid-cols-1", "lg:grid-cols-2", className)}
        {...props}
      >
        {childrenArray}
      </div>
    );
  },
);
IslandLayout.displayName = "IslandLayout";

export type IslandColumnProps = React.HTMLAttributes<HTMLDivElement>;

const IslandColumn = React.forwardRef<HTMLDivElement, IslandColumnProps>(
  ({ className, children, ...props }, ref) => (
    <div ref={ref} className={cn("flex flex-col gap-4", className)} {...props}>
      {children}
    </div>
  ),
);
IslandColumn.displayName = "IslandColumn";

export { IslandLayout, IslandColumn };
