import {
  AlertCircle,
  Briefcase,
  Building2,
  Info,
  UserPlus,
} from "lucide-react";

import { LegalForm } from "@workspace/types/company";

import { LEGAL_FORM_DATA } from "../../constants";
import { ChangedBadge } from "../components/changed-badge";
import { FieldRow } from "../components/field-row";
import { LegalFormCardRadio } from "../components/legal-form-card-radio";
import { SectionCard } from "../components/section-card";

interface LegalFormSectionProps {
  value: LegalForm;
  onChange: (v: LegalForm) => void;
  changed: boolean;
  error?: string;
  submitAttempted: boolean;
}

export function LegalFormSection({
  value,
  onChange,
  changed,
  error,
  submitAttempted,
}: Readonly<LegalFormSectionProps>) {
  const entry = LEGAL_FORM_DATA.find((e) => e.value === value);

  return (
    <SectionCard
      icon={Briefcase}
      title="Правовая форма"
      subtitle="Влияет на реквизиты, отчёты и шаблоны договоров"
      rightSlot={changed ? <ChangedBadge /> : undefined}
    >
      <FieldRow
        label="Форма собственности"
        required
        error={submitAttempted ? error : undefined}
      >
        <LegalFormCardRadio value={value} onChange={onChange} />
      </FieldRow>

      <div className="mt-4 flex items-center gap-2.5 rounded-xl border border-slate-200 bg-slate-50 px-3.5 py-3 text-[13px]">
        <Info className="size-[15px] shrink-0 text-slate-400" />
        <span className="text-slate-600">
          Статус:{" "}
          <strong className="text-slate-900">
            {entry?.isLegalEntity ? "Юридическое лицо" : "Физическое лицо"}
          </strong>{" "}
          · определяется автоматически
        </span>
      </div>

      {changed && (
        <div className="mt-3 flex gap-2.5 rounded-xl border border-amber-200 bg-amber-50 px-3.5 py-3">
          <AlertCircle className="mt-0.5 size-4 shrink-0 text-amber-800" />
          <p className="text-[12.5px] leading-relaxed text-amber-800">
            Смена формы собственности затронет формирование договоров и отчётов.
            Потребуется дополнительное подтверждение перед сохранением.
          </p>
        </div>
      )}
    </SectionCard>
  );
}
