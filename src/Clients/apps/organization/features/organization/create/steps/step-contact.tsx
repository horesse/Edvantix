"use client";

import type { Control } from "react-hook-form";
import { useWatch } from "react-hook-form";

import { ContactType } from "@workspace/types/company";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { Textarea } from "@workspace/ui/components/textarea";
import { cn } from "@workspace/ui/lib/utils";
import type { CreateOrganizationInput } from "@workspace/validations/company";

import { FieldHint, FieldLabel } from "../components/field-hint";
import { StepHeader } from "../components/step-header";
import { CONTACT_TYPE_DATA } from "../constants";

interface StepContactProps {
  control: Control<CreateOrganizationInput>;
}

export function StepContact({ control }: StepContactProps) {
  const contactTypeValue = useWatch({ control, name: "primaryContactType" });
  const descLen = (
    useWatch({ control, name: "primaryContactDescription" }) ?? ""
  ).length;
  const ct = CONTACT_TYPE_DATA.find((c) => c.value === contactTypeValue);
  const ContactIcon = ct?.Icon;

  return (
    <div className="flex flex-col gap-5">
      <StepHeader
        eyebrow="Шаг 3 из 4"
        title="Как с вами связаться?"
        subtitle="Основной контакт — канал для уведомлений о платежах, новых студентах и системных сообщениях."
      />

      {/* Segmented contact type selector */}
      <FormField
        control={control}
        name="primaryContactType"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label="Канал связи" required />
            <FormControl>
              <div className="overflow-x-auto pb-0.5">
                <div className="inline-flex gap-0.5 rounded-[10px] border border-slate-200 bg-slate-100 p-[3px]">
                  {CONTACT_TYPE_DATA.map((c) => {
                    const isActive = field.value === c.value;
                    return (
                      <button
                        key={c.value}
                        type="button"
                        onClick={() => field.onChange(c.value as ContactType)}
                        className={cn(
                          "inline-flex items-center gap-1.5 rounded-lg px-3.5 py-[7px] text-[13px] font-medium transition-all",
                          isActive
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
            </FormControl>
            <FieldHint error={fieldState.error?.message} />
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />

      {/* Contact value */}
      <FormField
        control={control}
        name="primaryContactValue"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label={ct?.label ?? "Контакт"} required />
            <FormControl>
              <div className="relative">
                {ContactIcon && (
                  <div className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 -translate-y-1/2">
                    <ContactIcon className="size-3.5" />
                  </div>
                )}
                <Input
                  {...field}
                  type={ct?.inputType ?? "text"}
                  placeholder={ct?.placeholder ?? ""}
                  className={cn(ct && "pl-9")}
                  aria-invalid={fieldState.invalid}
                />
              </div>
            </FormControl>
            <FieldHint
              error={fieldState.error?.message}
              hint={!fieldState.error ? ct?.hint : undefined}
            />
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />

      {/* Description / comment */}
      <FormField
        control={control}
        name="primaryContactDescription"
        render={({ field, fieldState }) => (
          <FormItem>
            <FieldLabel label="Комментарий" required />
            <FormControl>
              <Textarea
                {...field}
                placeholder="Например: основной рабочий email директора, проверяется с 9:00 до 18:00 в будни"
                maxLength={500}
                aria-invalid={fieldState.invalid}
              />
            </FormControl>
            <div className="mt-1.5 flex items-start justify-between">
              <FieldHint
                error={fieldState.error?.message}
                hint={
                  !fieldState.error
                    ? "Краткое описание — кому и когда звонить/писать. Видно только сотрудникам школы."
                    : undefined
                }
              />
              <span className="text-muted-foreground shrink-0 text-[11px] tabular-nums">
                {descLen} / 500
              </span>
            </div>
            <FormMessage className="hidden" />
          </FormItem>
        )}
      />
    </div>
  );
}
