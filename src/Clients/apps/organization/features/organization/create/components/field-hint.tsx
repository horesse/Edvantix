import { AlertCircle } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

interface FieldHintProps {
  hint?: string;
  error?: string;
}

export function FieldHint({ hint, error }: FieldHintProps) {
  if (!hint && !error) return null;
  return (
    <div
      className={cn(
        "mt-1.5 flex items-start gap-1.5 text-[12px] leading-tight",
        error ? "text-destructive" : "text-muted-foreground",
      )}
    >
      {error && <AlertCircle className="mt-px size-3 shrink-0" />}
      <span>{error ?? hint}</span>
    </div>
  );
}

interface FieldLabelProps {
  label: string;
  required?: boolean;
  optional?: boolean;
}

export function FieldLabel({ label, required, optional }: FieldLabelProps) {
  return (
    <div className="mb-1.5 flex items-baseline gap-2">
      <span className="text-foreground text-[13px] font-medium">
        {label}
        {required && <span className="text-destructive ml-0.5">*</span>}
      </span>
      {optional && (
        <span className="text-muted-foreground text-[11px]">необязательно</span>
      )}
    </div>
  );
}
