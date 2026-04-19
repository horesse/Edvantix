"use client";

import { Calendar } from "lucide-react";
import type { Control } from "react-hook-form";
import { useWatch } from "react-hook-form";

import {
  ORGANIZATION_TYPE_LABELS,
  OrganizationType,
} from "@workspace/types/company";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import type { CreateOrganizationInput } from "@workspace/validations/company";

import { FieldHint, FieldLabel } from "../components/field-hint";
import { StepHeader } from "../components/step-header";
import { LEGAL_FORM_DATA } from "../constants";

const TODAY = new Date().toISOString().slice(0, 10);

interface StepAboutProps {
  control: Control<CreateOrganizationInput>;
}

export function StepAbout({ control }: StepAboutProps) {
  const legalFormValue = useWatch({ control, name: "legalForm" });
  const lf = LEGAL_FORM_DATA.find((e) => e.value === legalFormValue);

  return (
    <div className="flex flex-col gap-5">
      <StepHeader
        eyebrow="Шаг 2 из 4"
        title="Расскажите об организации"
        subtitle="Эти данные появятся в договорах и кабинете школы."
      />

      <FormField
        control={control}
        name="fullLegalName"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label="Полное наименование" required />
            <FormControl>
              <Input
                {...field}
                placeholder={
                  lf
                    ? `${lf.tag} «Название школы»`
                    : "Полное юридическое наименование"
                }
                aria-invalid={fieldState.invalid}
              />
            </FormControl>
            <FieldHint
              error={fieldState.error?.message}
              hint={
                !fieldState.error
                  ? lf
                    ? `Как в учредительных документах — начните с «${lf.tag}»`
                    : "Как в учредительных документах"
                  : undefined
              }
            />
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />

      <FormField
        control={control}
        name="shortName"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label="Краткое название" optional />
            <FormControl>
              <Input
                {...field}
                placeholder="Например: Школа «Эврика»"
                aria-invalid={fieldState.invalid}
              />
            </FormControl>
            <FieldHint
              error={fieldState.error?.message}
              hint={
                !fieldState.error
                  ? "Используется в интерфейсе и письмах студентам. По умолчанию — первые слова из полного."
                  : undefined
              }
            />
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />

      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="registrationDate"
          render={({ field, fieldState }) => (
            <FormItem>
              <FieldLabel label="Дата регистрации" required />
              <FormControl>
                <div className="relative">
                  <Calendar className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2" />
                  <Input
                    {...field}
                    type="date"
                    max={TODAY}
                    className="pl-9"
                    aria-invalid={fieldState.invalid}
                  />
                </div>
              </FormControl>
              <FieldHint
                error={fieldState.error?.message}
                hint={
                  !fieldState.error
                    ? "Из свидетельства о государственной регистрации"
                    : undefined
                }
              />
              <FormMessage className="hidden" />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="organizationType"
          render={({ field, fieldState }) => (
            <FormItem>
              <FieldLabel label="Тип организации" required />
              <Select
                value={field.value !== undefined ? String(field.value) : ""}
                onValueChange={(v) =>
                  field.onChange(Number(v) as OrganizationType)
                }
              >
                <FormControl>
                  <SelectTrigger aria-invalid={fieldState.invalid}>
                    <SelectValue placeholder="Выберите тип" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {(
                    Object.entries(ORGANIZATION_TYPE_LABELS) as [
                      string,
                      string,
                    ][]
                  ).map(([value, label]) => (
                    <SelectItem key={value} value={value}>
                      {label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FieldHint
                error={fieldState.error?.message}
                hint={
                  !fieldState.error
                    ? "Категория по роду образовательной деятельности"
                    : undefined
                }
              />
              <FormMessage className="hidden" />
            </FormItem>
          )}
        />
      </div>
    </div>
  );
}
