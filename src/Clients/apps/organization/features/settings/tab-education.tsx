"use client";

import { useEffect, useState } from "react";

import { GraduationCap, Loader2, Plus } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import { zodResolver } from "@hookform/resolvers/zod";

import useUpdateEducation from "@workspace/api-hooks/profiles/useUpdateEducation";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
import { Form } from "@workspace/ui/components/form";
import { educationSchema } from "@workspace/validations/profile";

import { EducationCard, EducationDialog } from "./profile-education";
import type { EducationInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

const formSchema = z.object({
  educations: z.array(educationSchema),
});

type FormValues = z.infer<typeof formSchema>;

function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function TabEducation({ profile }: { profile: OwnProfileDetails }) {
  const [dialogOpen, setDialogOpen] = useState(false);

  const mutation = useUpdateEducation({
    onSuccess: () => {
      toast.success("Образование сохранено");
      form.reset(form.getValues());
    },
    onError: () => toast.error("Не удалось сохранить образование"),
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      educations: profile.educations.map((e) => ({
        institution: e.institution,
        specialty: e.specialty ?? "",
        dateStart: toDateString(e.dateStart),
        dateEnd: toDateString(e.dateEnd),
        level: e.educationLevel,
      })),
    },
  });

  useEffect(() => {
    if (!form.formState.isDirty) {
      form.reset({
        educations: profile.educations.map((e) => ({
          institution: e.institution,
          specialty: e.specialty ?? "",
          dateStart: toDateString(e.dateStart),
          dateEnd: toDateString(e.dateEnd),
          level: e.educationLevel,
        })),
      });
    }
  }, [profile]); // eslint-disable-line react-hooks/exhaustive-deps

  const educationsArray = useFieldArray({
    control: form.control,
    name: "educations",
  });

  function onSubmit(data: FormValues) {
    mutation.mutate({
      educations: data.educations.map((e) => ({
        institution: e.institution,
        specialty: e.specialty || null,
        dateStart: e.dateStart,
        dateEnd: e.dateEnd || null,
        level: e.level,
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
            Учебные заведения и уровни образования
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

        {educationsArray.fields.length === 0 ? (
          <EmptyState
            icon={<GraduationCap className="size-5" />}
            text="Образование не добавлено"
            onAdd={() => setDialogOpen(true)}
          />
        ) : (
          <div>
            {educationsArray.fields.map((field, index) => (
              <EducationCard
                key={field.id}
                field={field}
                onRemove={() => educationsArray.remove(index)}
                isLast={index === educationsArray.fields.length - 1}
              />
            ))}
          </div>
        )}

        <EducationDialog
          open={dialogOpen}
          onOpenChange={setDialogOpen}
          onAppend={(data: EducationInput) => educationsArray.append(data)}
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
