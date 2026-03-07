"use client";

import { useEffect } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import useUpdatePersonalInfo from "@workspace/api-hooks/profiles/useUpdatePersonalInfo";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  birthDateField,
  middleNameField,
  nameField,
} from "@workspace/validations/profile";

const schema = z.object({
  lastName: nameField("Фамилия"),
  firstName: nameField("Имя"),
  middleName: middleNameField,
  birthDate: birthDateField,
});

type FormValues = z.infer<typeof schema>;

function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function TabGeneral({
  profile,
  onDirtyChange,
}: {
  profile: OwnProfileDetails;
  onDirtyChange?: (dirty: boolean) => void;
}) {
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      lastName: profile.lastName,
      firstName: profile.firstName,
      middleName: profile.middleName ?? "",
      birthDate: toDateString(profile.birthDate),
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
    });
  }

  const isDirty = form.formState.isDirty;
  const isPending = mutation.isPending;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="max-w-2xl space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <FormField
              control={form.control}
              name="lastName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Фамилия</FormLabel>
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
                  <FormLabel>Имя</FormLabel>
                  <FormControl>
                    <Input placeholder="Иван" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <FormField
              control={form.control}
              name="middleName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Отчество{" "}
                    <span className="text-muted-foreground/50">(необяз.)</span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder="Иванович" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
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
          </div>
        </div>

        <div className="flex items-center justify-between border-t border-border/40 pt-4">
          {isDirty && !isPending ? (
            <p className="text-xs text-muted-foreground">
              Есть несохранённые изменения
            </p>
          ) : (
            <span />
          )}
          <Button type="submit" disabled={isPending || !isDirty} size="sm">
            {isPending && <Loader2 className="size-3.5 animate-spin" />}
            Сохранить
          </Button>
        </div>
      </form>
    </Form>
  );
}
