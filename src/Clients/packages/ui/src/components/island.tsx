import * as React from "react";

import { type VariantProps, cva } from "class-variance-authority";
import { motion } from "framer-motion";

import { cn } from "../lib/utils";
import { ScrollArea } from "./scroll-area";

const islandVariants = cva("bg-card text-card-foreground transition-shadow", {
  variants: {
    variant: {
      default: "shadow-lg hover:shadow-xl",
      floating: "shadow-2xl",
      flat: "shadow-sm",
      bordered: "border shadow-sm hover:shadow-md",
    },
    padding: {
      none: "",
      sm: "p-4",
      md: "p-6",
      lg: "p-8",
    },
    rounded: {
      none: "rounded-none",
      sm: "rounded-sm",
      md: "rounded-md",
      lg: "rounded-lg",
      xl: "rounded-xl",
      "2xl": "rounded-2xl",
    },
  },
  defaultVariants: {
    variant: "default",
    padding: "md",
    rounded: "2xl",
  },
});

export interface IslandProps
  extends
    React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof islandVariants> {
  asChild?: boolean;
  enableScroll?: boolean;
  scrollClassName?: string;
  animate?: boolean;
}

const Island = React.forwardRef<HTMLDivElement, IslandProps>(
  (
    {
      className,
      variant,
      padding,
      children,
      enableScroll = false,
      scrollClassName,
      animate = false,
      ...props
    },
    ref,
  ) => {
    const content = enableScroll ? (
      <ScrollArea className={cn("h-full", scrollClassName)}>
        {children}
      </ScrollArea>
    ) : (
      children
    );

    if (animate) {
      return (
        <motion.div
          ref={ref}
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          exit={{ opacity: 0, scale: 0.95 }}
          transition={{ duration: 0.2, ease: "easeOut" }}
          className={cn(islandVariants({ variant, padding }), className)}
          style={props.style}
        >
          {content}
        </motion.div>
      );
    }

    return (
      <div
        ref={ref}
        className={cn(islandVariants({ variant, padding }), className)}
        {...props}
      >
        {content}
      </div>
    );
  },
);
Island.displayName = "Island";

const IslandHeader = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("flex flex-col space-y-1.5", className)}
    {...props}
  />
));
IslandHeader.displayName = "IslandHeader";

const IslandTitle = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("text-xl leading-none font-semibold", className)}
    {...props}
  />
));
IslandTitle.displayName = "IslandTitle";

const IslandDescription = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("text-muted-foreground text-sm", className)}
    {...props}
  />
));
IslandDescription.displayName = "IslandDescription";

const IslandContent = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div ref={ref} className={cn("flex-1", className)} {...props} />
));
IslandContent.displayName = "IslandContent";

const IslandFooter = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("flex items-center pt-4", className)}
    {...props}
  />
));
IslandFooter.displayName = "IslandFooter";

export {
  Island,
  IslandHeader,
  IslandTitle,
  IslandDescription,
  IslandContent,
  IslandFooter,
};
