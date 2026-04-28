import { FieldHint, FieldLabel } from "../../components/field-hint";
import { ChangedBadge } from "./changed-badge";

interface FieldRowProps {
  label: string;
  required?: boolean;
  optional?: boolean;
  hint?: string;
  changed?: boolean;
  error?: string;
  children: React.ReactNode;
}

export function FieldRow({
  label,
  required,
  optional,
  hint,
  changed,
  error,
  children,
}: Readonly<FieldRowProps>) {
  return (
    <div className="space-y-1.5">
      <div className="flex flex-wrap items-center gap-2">
        <FieldLabel label={label} required={required} optional={optional} />
        {changed && <ChangedBadge />}
      </div>
      {children}
      <FieldHint hint={!error ? hint : undefined} error={error} />
    </div>
  );
}
