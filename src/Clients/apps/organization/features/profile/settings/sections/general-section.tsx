"use client";

import { forwardRef, useEffect, useImperativeHandle } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import useUpdatePersonalInfo from "@workspace/api-hooks/profiles/useUpdatePersonalInfo";
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
import type { SectionHandle } from "../types";

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
  SectionHandle,
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

  const mutation = useUpdatePersonalInfo({
    onSuccess: (data) => {
      toast.success("Личная информация сохранена");
      form.reset({
        lastName: data.lastName,
        firstName: data.firstName,
        middleName: data.middleName ?? "",
        birthDate: toDateString(data.birthDate),
        gender: data.gender ?? Gender.None,
      });
    },
    onError: () => toast.error("Не удалось сохранить"),
  });

  function onSubmit(data: FormValues) {
    mutation.mutate({
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: data.middleName || null,
      birthDate: data.birthDate,
      gender: data.gender,
    });
  }

  const isDirty = form.formState.isDirty;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    submit: () => void form.handleSubmit(onSubmit)(),
  }));

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
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
