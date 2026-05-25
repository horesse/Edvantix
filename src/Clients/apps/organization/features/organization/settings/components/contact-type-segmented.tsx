import { ContactType } from "@workspace/types/organization";
import { cn } from "@workspace/ui/lib/utils";

import { CONTACT_TYPE_DATA } from "../../constants";

interface ContactTypeSegmentedProps {
  value: ContactType;
  onChange: (v: ContactType) => void;
}

export function ContactTypeSegmented({
  value,
  onChange,
}: Readonly<ContactTypeSegmentedProps>) {
  return (
    <div className="overflow-x-auto pb-0.5">
      <div className="inline-flex gap-0.5 rounded-[10px] border border-slate-200 bg-slate-100 p-[3px]">
        {CONTACT_TYPE_DATA.map((c) => {
          const active = value === c.value;
          return (
            <button
              key={c.value}
              type="button"
              onClick={() => onChange(c.value)}
              className={cn(
                "inline-flex items-center gap-1.5 rounded-lg px-3.5 py-[7px] text-[13px] font-medium transition-all",
                active
                  ? "text-foreground bg-white font-semibold shadow-sm"
                  : "text-slate-500 hover:text-slate-700",
              )}
            >
              <c.Icon className="size-3.5 shrink-0" />
              <span className="whitespace-nowrap">{c.short}</span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
