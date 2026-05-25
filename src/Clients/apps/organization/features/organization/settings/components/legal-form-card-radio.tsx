import { Check } from "lucide-react";

import { LegalForm } from "@workspace/types/organization";
import { cn } from "@workspace/ui/lib/utils";

import { LEGAL_FORM_DATA } from "../../constants";

interface LegalFormCardRadioProps {
  value: LegalForm;
  onChange: (v: LegalForm) => void;
}

export function LegalFormCardRadio({
  value,
  onChange,
}: Readonly<LegalFormCardRadioProps>) {
  return (
    <div className="grid grid-cols-1 gap-2.5 sm:grid-cols-2">
      {LEGAL_FORM_DATA.map((entry) => {
        const active = value === entry.value;
        return (
          <button
            key={entry.value}
            type="button"
            onClick={() => onChange(entry.value)}
            className={cn(
              "flex flex-col gap-1 rounded-xl border px-3.5 py-3.5 text-left font-sans transition-all",
              active
                ? "border-brand-600 bg-brand-50/40 shadow-[0_0_0_3px_rgba(79,70,229,0.12)]"
                : "border-border bg-card hover:border-brand-200 hover:bg-slate-50/60",
            )}
          >
            <div className="flex items-center justify-between gap-2">
              <span
                className={cn(
                  "inline-flex items-center rounded-md px-2 py-0.5 text-[12px] font-bold tracking-tight",
                  active
                    ? "bg-brand-600 text-white"
                    : "bg-slate-100 text-slate-600",
                )}
              >
                {entry.tag}
              </span>
              {active && (
                <div className="bg-brand-600 flex size-[18px] items-center justify-center rounded-full">
                  <Check className="size-3 text-white" strokeWidth={3} />
                </div>
              )}
            </div>
            <span className="text-[13px] leading-tight text-slate-600">
              {entry.label}
            </span>
          </button>
        );
      })}
    </div>
  );
}
