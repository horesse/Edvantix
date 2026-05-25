import { Calendar, FileText } from "lucide-react";
import type { UseFormRegister } from "react-hook-form";

import {
  ORGANIZATION_TYPE_LABELS,
  OrganizationType,
} from "@workspace/types/organization";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";

import { FieldRow } from "../components/field-row";
import { SectionCard } from "../components/section-card";
import type { EditFormValues } from "../schema";

interface BasicInfoSectionProps {
  register: UseFormRegister<EditFormValues>;
  orgTypeValue: OrganizationType;
  onOrgTypeChange: (v: OrganizationType) => void;
  changedFields: Partial<Record<keyof EditFormValues, boolean>>;
  errors: Partial<Record<keyof EditFormValues, { message?: string }>>;
  submitAttempted: boolean;
}

export function BasicInfoSection({
  register,
  orgTypeValue,
  onOrgTypeChange,
  changedFields,
  errors,
  submitAttempted,
}: Readonly<BasicInfoSectionProps>) {
  return (
    <SectionCard
      icon={FileText}
      title="Основные сведения"
      subtitle="Данные, которые появятся в документах и интерфейсе"
    >
      <div className="space-y-5">
        <FieldRow
          label="Полное наименование"
          required
          hint="Как в учредительных документах"
          changed={!!changedFields.fullLegalName}
          error={submitAttempted ? errors.fullLegalName?.message : undefined}
        >
          <Input {...register("fullLegalName")} />
        </FieldRow>

        <FieldRow
          label="Краткое название"
          optional
          hint="Используется в интерфейсе и письмах"
          changed={!!changedFields.shortName}
        >
          <Input {...register("shortName")} />
        </FieldRow>

        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
          <FieldRow
            label="Дата регистрации"
            required
            hint="Из свидетельства о регистрации"
            changed={!!changedFields.registrationDate}
            error={
              submitAttempted ? errors.registrationDate?.message : undefined
            }
          >
            <div className="relative">
              <Calendar className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
              <Input
                type="date"
                max={new Date().toISOString().slice(0, 10)}
                className="pl-9"
                {...register("registrationDate")}
              />
            </div>
          </FieldRow>

          <FieldRow
            label="Тип организации"
            required
            hint="Категория по роду деятельности"
            changed={!!changedFields.organizationType}
            error={
              submitAttempted ? errors.organizationType?.message : undefined
            }
          >
            <Select
              value={String(orgTypeValue)}
              onValueChange={(v) =>
                onOrgTypeChange(Number(v) as OrganizationType)
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Выберите тип" />
              </SelectTrigger>
              <SelectContent>
                {(
                  Object.entries(ORGANIZATION_TYPE_LABELS) as [string, string][]
                ).map(([val, label]) => (
                  <SelectItem key={val} value={val}>
                    {label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FieldRow>
        </div>
      </div>
    </SectionCard>
  );
}
