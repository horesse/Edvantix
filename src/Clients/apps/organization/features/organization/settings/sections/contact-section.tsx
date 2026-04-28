import { Mail } from "lucide-react";
import type { UseFormRegister } from "react-hook-form";

import { ContactType } from "@workspace/types/company";
import { Input } from "@workspace/ui/components/input";
import { Textarea } from "@workspace/ui/components/textarea";

import { CONTACT_TYPE_DATA } from "../../constants";
import { ChangedBadge } from "../components/changed-badge";
import { ContactTypeSegmented } from "../components/contact-type-segmented";
import { FieldRow } from "../components/field-row";
import { SectionCard } from "../components/section-card";
import type { EditFormValues } from "../schema";

interface ContactSectionProps {
  register: UseFormRegister<EditFormValues>;
  contactTypeValue: ContactType;
  onContactTypeChange: (v: ContactType) => void;
  contactDescriptionValue: string;
  onContactDescriptionChange: (v: string) => void;
  changedFields: Partial<Record<keyof EditFormValues, boolean>>;
  errors: Partial<Record<keyof EditFormValues, { message?: string }>>;
  submitAttempted: boolean;
}

export function ContactSection({
  register,
  contactTypeValue,
  onContactTypeChange,
  contactDescriptionValue,
  onContactDescriptionChange,
  changedFields,
  errors,
  submitAttempted,
}: Readonly<ContactSectionProps>) {
  const ctMeta =
    CONTACT_TYPE_DATA.find((c) => c.value === contactTypeValue) ??
    CONTACT_TYPE_DATA[0]!;
  const hasContactChanges =
    changedFields.contactType ??
    changedFields.contactValue ??
    changedFields.contactDescription;

  return (
    <SectionCard
      icon={Mail}
      title="Основной контакт"
      subtitle="Канал связи для уведомлений и системных сообщений"
      rightSlot={hasContactChanges ? <ChangedBadge /> : undefined}
    >
      <div className="space-y-5">
        <FieldRow label="Канал связи" required>
          <ContactTypeSegmented
            value={contactTypeValue}
            onChange={onContactTypeChange}
          />
        </FieldRow>

        <FieldRow
          label={ctMeta.label}
          required
          hint={ctMeta.hint}
          changed={!!changedFields.contactValue}
          error={submitAttempted ? errors.contactValue?.message : undefined}
        >
          <div className="relative">
            <ctMeta.Icon className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 size-3.5 -translate-y-1/2" />
            <Input
              type={ctMeta.inputType}
              placeholder={ctMeta.placeholder}
              className="pl-9"
              {...register("contactValue")}
            />
          </div>
        </FieldRow>

        <FieldRow
          label="Комментарий"
          optional
          hint="Кому и когда писать/звонить. Видно только сотрудникам."
          changed={!!changedFields.contactDescription}
          error={
            submitAttempted ? errors.contactDescription?.message : undefined
          }
        >
          <Textarea
            maxLength={500}
            className="min-h-20 resize-y"
            value={contactDescriptionValue}
            onChange={(e) => onContactDescriptionChange(e.target.value)}
          />
          <p className="mt-1 text-right text-[11px] text-slate-400 tabular-nums">
            {contactDescriptionValue.length} / 500
          </p>
        </FieldRow>
      </div>
    </SectionCard>
  );
}
