"use client";

import { Building2, Check, UserPlus } from "lucide-react";
import type { Control } from "react-hook-form";
import { useWatch } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@workspace/ui/components/form";
import { cn } from "@workspace/ui/lib/utils";
import type { CreateOrganizationInput } from "@workspace/validations/company";

import { FieldHint, FieldLabel } from "../components/field-hint";
import { InfoCallout } from "../components/info-callout";
import { StepHeader } from "../components/step-header";
import { LEGAL_FORM_DATA } from "../constants";

interface StepLegalFormProps {
  control: Control<CreateOrganizationInput>;
}

export function StepLegalForm({ control }: StepLegalFormProps) {
  const legalFormValue = useWatch({ control, name: "legalForm" });
  const selectedEntry = LEGAL_FORM_DATA.find((e) => e.value === legalFormValue);

  return (
    <div className="flex flex-col gap-7">
      <StepHeader
        eyebrow="Шаг 1 из 4"
        title="Какая у вас форма собственности?"
        subtitle="От этого зависит, какие документы и отчёты мы подготовим. Переключиться можно позже в настройках."
      />

      <FormField
        control={control}
        name="legalForm"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label="Форма собственности" required />
            <FormControl>
              <div className="grid grid-cols-2 gap-2.5">
                {LEGAL_FORM_DATA.map((entry) => {
                  const isActive = field.value === entry.value;
                  return (
                    <button
                      key={entry.value}
                      type="button"
                      onClick={() => field.onChange(entry.value)}
                      className={cn(
                        "flex flex-col gap-1 rounded-xl border px-3.5 py-3.5 text-left font-sans transition-all",
                        isActive
                          ? "border-brand-600 bg-brand-50/40 shadow-[0_0_0_3px_rgba(79,70,229,0.12)]"
                          : "border-border bg-card hover:border-brand-200 hover:bg-slate-50/60",
                      )}
                    >
                      <div className="flex items-center justify-between gap-2">
                        <span
                          className={cn(
                            "inline-flex items-center rounded-md px-2 py-0.5 text-[12px] font-bold tracking-tight",
                            isActive
                              ? "bg-brand-600 text-white"
                              : "bg-slate-100 text-slate-600",
                          )}
                        >
                          {entry.tag}
                        </span>
                        {isActive && (
                          <div className="bg-brand-600 flex size-[18px] items-center justify-center rounded-full">
                            <Check
                              className="size-3 text-white"
                              strokeWidth={3}
                            />
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
            </FormControl>
            <FieldHint
              error={fieldState.error?.message}
              hint={
                !fieldState.error
                  ? "Выберите правовую форму вашей организации"
                  : undefined
              }
            />
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />

      {selectedEntry && (
        <InfoCallout
          variant={selectedEntry.isLegalEntity ? "primary" : "neutral"}
          icon={selectedEntry.isLegalEntity ? Building2 : UserPlus}
          title={
            selectedEntry.isLegalEntity
              ? "Регистрируется как юридическое лицо"
              : "Регистрируется как физическое лицо"
          }
          description={
            selectedEntry.isLegalEntity
              ? "Для документов потребуются реквизиты: УНП, расчётный счёт, юридический адрес. Их можно заполнить позже."
              : "Для ИП достаточно паспортных данных и УНП — реквизитный блок упростим."
          }
        />
      )}
    </div>
  );
}
