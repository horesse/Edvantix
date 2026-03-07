"use client";

import { useEffect, useState } from "react";

import { Briefcase, Loader2, Plus } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import { zodResolver } from "@hookform/resolvers/zod";

import useUpdateEmployment from "@workspace/api-hooks/profiles/useUpdateEmployment";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
import { Form } from "@workspace/ui/components/form";
import { employmentSchema } from "@workspace/validations/profile";

import { EmploymentCard, EmploymentDialog } from "./profile-employment";
import type { EmploymentInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

const formSchema = z.object({
  employmentHistories: z.array(employmentSchema),
});

type FormValues = z.infer<typeof formSchema>;

function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function TabEmployment({ profile }: { profile: OwnProfileDetails }) {
  const [dialogOpen, setDialogOpen] = useState(false);

  const mutation = useUpdateEmployment({
    onSuccess: () => {
      toast.success("Опыт работы сохранён");
      form.reset(form.getValues());
    },
    onError: () => toast.error("Не удалось сохранить опыт работы"),
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      employmentHistories: profile.employmentHistories.map((e) => ({
        workplace: e.workplace,
        position: e.position,
        startDate: toDateString(e.startDate),
        endDate: toDateString(e.endDate),
        description: e.description ?? "",
      })),
    },
  });

  useEffect(() => {
    if (!form.formState.isDirty) {
      form.reset({
        employmentHistories: profile.employmentHistories.map((e) => ({
          workplace: e.workplace,
          position: e.position,
          startDate: toDateString(e.startDate),
          endDate: toDateString(e.endDate),
          description: e.description ?? "",
        })),
      });
    }
  }, [profile]); // eslint-disable-line react-hooks/exhaustive-deps

  const employmentsArray = useFieldArray({
    control: form.control,
    name: "employmentHistories",
  });

  function onSubmit(data: FormValues) {
    mutation.mutate({
      employmentHistories: data.employmentHistories.map((e) => ({
        workplace: e.workplace,
        position: e.position,
        startDate: e.startDate,
        endDate: e.endDate || null,
        description: e.description || null,
      })),
    });
  }

  const isDirty = form.formState.isDirty;
  const isPending = mutation.isPending;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">
            История вашей трудовой деятельности
          </p>
          <Button
            type="button"
            variant="ghost"
            size="sm"
            onClick={() => setDialogOpen(true)}
            className="h-7 gap-1.5 px-2.5 text-xs text-muted-foreground hover:text-foreground"
          >
            <Plus className="size-3" />
            Добавить
          </Button>
        </div>

        {employmentsArray.fields.length === 0 ? (
          <EmptyState
            icon={<Briefcase className="size-5" />}
            text="Места работы не добавлены"
            onAdd={() => setDialogOpen(true)}
          />
        ) : (
          <div>
            {employmentsArray.fields.map((field, index) => (
              <EmploymentCard
                key={field.id}
                field={field}
                onRemove={() => employmentsArray.remove(index)}
                isLast={index === employmentsArray.fields.length - 1}
              />
            ))}
          </div>
        )}

        <EmploymentDialog
          open={dialogOpen}
          onOpenChange={setDialogOpen}
          onAppend={(data: EmploymentInput) => employmentsArray.append(data)}
        />

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
