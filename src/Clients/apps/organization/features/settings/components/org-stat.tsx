import { cn } from "@workspace/ui/lib/utils";

interface OrgStatProps {
  readonly label: string;
  readonly value: string | null | undefined;
  readonly hint?: string | null;
  readonly mono?: boolean;
}

export function OrgStat({ label, value, hint, mono }: OrgStatProps) {
  return (
    <div className="border-slate-50 px-6 py-4 [&:not(:last-child)]:border-r">
      <div className="mb-1.5 text-[11px] font-semibold tracking-[0.06em] text-slate-400 uppercase">
        {label}
      </div>
      <div
        className={cn(
          "truncate text-[13.5px] font-medium text-slate-900",
          mono && "font-mono",
        )}
      >
        {value ?? "—"}
      </div>
      {hint && (
        <div className="mt-0.5 text-[11.5px] text-slate-400">{hint}</div>
      )}
    </div>
  );
}
