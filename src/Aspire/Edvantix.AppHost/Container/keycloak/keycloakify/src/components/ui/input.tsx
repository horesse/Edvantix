import * as React from "react";

import { cn } from "@/lib/utils";

export interface InputProps extends React.ComponentProps<"input"> {
    /** Показать состояние ошибки (красная рамка) */
    hasError?: boolean;
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
    ({ className, type, hasError = false, ...props }, ref) => {
        return (
            <input
                type={type}
                ref={ref}
                className={cn(
                    "w-full px-3.5 py-2.5 text-sm rounded-lg border bg-white",
                    "text-slate-900 placeholder:text-slate-400 outline-none",
                    "transition-all duration-150",
                    "disabled:cursor-not-allowed disabled:opacity-50",
                    hasError
                        ? "border-red-500 shadow-[0_0_0_3px_rgba(239,68,68,0.1)] focus:border-red-500 focus:shadow-[0_0_0_3px_rgba(239,68,68,0.1)]"
                        : "border-slate-300 focus:border-[#4f46e5] focus:shadow-[0_0_0_3px_rgba(79,70,229,0.12)]",
                    className
                )}
                {...props}
            />
        );
    }
);
Input.displayName = "Input";

export { Input };
