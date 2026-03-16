"use client";

import { forwardRef, useEffect, useImperativeHandle } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Gender, type OwnProfileDetails } from "@workspace/types/profile";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { cn } from "@workspace/ui/lib/utils";
import {
  birthDateField,
  middleNameField,
  nameField,
} from "@workspace/validations/profile";

import { toDateString } from "../schema";
import type { GeneralData, GeneralSectionHandle } from "../types";

const schema = z.object({
  lastName: nameField("Фамилия"),
  firstName: nameField("Имя"),
  middleName: middleNameField,
  birthDate: birthDateField,
  gender: z.nativeEnum(Gender),
});

type FormValues = z.infer<typeof schema>;

const GENDER_OPTIONS = [
  { value: Gender.Female, label: "Женский" },
  { value: Gender.Male, label: "Мужской" },
] as const;

export const GeneralSection = forwardRef<
  GeneralSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function GeneralSection({ profile, onDirtyChange }, ref) {
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      lastName: profile.lastName,
      firstName: profile.firstName,
      middleName: profile.middleName ?? "",
      birthDate: toDateString(profile.birthDate),
      gender: profile.gender ?? Gender.None,
    },
  });

  const isDirty = form.formState.isDirty;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    async getPayload(): Promise<GeneralData | null> {
      const valid = await form.trigger();
      if (!valid) return null;
      const values = form.getValues();
      return {
        firstName: values.firstName,
        lastName: values.lastName,
        middleName: values.middleName || null,
        birthDate: values.birthDate,
      };
    },
    acknowledgeServerState() {
      // form.reset preserves current values as the new baseline,
      // clearing isDirty without fetching from the server again.
      form.reset(form.getValues());
    },
  }));

  return (
    <Form {...form}>
      <form className="space-y-4">
        {/* Name row: Last / First / Middle */}
        <div className="grid gap-4 sm:grid-cols-3">
          <FormField
            control={form.control}
            name="lastName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Фамилия <span className="text-destructive">*</span>
                </FormLabel>
                <FormControl>
                  <Input placeholder="Иванов" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="firstName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Имя <span className="text-destructive">*</span>
                </FormLabel>
                <FormControl>
                  <Input placeholder="Иван" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="middleName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Отчество</FormLabel>
                <FormControl>
                  <Input placeholder="Иванович" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {/* Birth date + Gender */}
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField
            control={form.control}
            name="birthDate"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Дата рождения</FormLabel>
                <FormControl>
                  <Input type="date" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="gender"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Пол</FormLabel>
                <div className="flex h-[42px] gap-2">
                  {GENDER_OPTIONS.map((opt) => (
                    <label
                      key={opt.value}
                      className={cn(
                        "flex flex-1 cursor-pointer items-center justify-center gap-2 rounded-xl border text-sm font-medium transition-colors",
                        field.value === opt.value
                          ? "border-brand-300 bg-brand-50 text-brand-700 dark:border-brand-700 dark:bg-brand-900/20 dark:text-brand-400"
                          : "border-border text-muted-foreground hover:bg-muted",
                      )}
                    >
                      <input
                        type="radio"
                        className="accent-brand-600 size-4"
                        value={opt.value}
                        checked={field.value === opt.value}
                        onChange={() => field.onChange(opt.value)}
                      />
                      {opt.label}
                    </label>
                  ))}
                </div>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
      </form>
    </Form>
  );
});
