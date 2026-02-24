"use client";

import { useEffect, useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Briefcase, GraduationCap, Loader2, Mail, Plus } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";
import { toast } from "sonner";

import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
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

import { AvatarBlock } from "./profile-avatar";
import { ContactDialog, ContactRow } from "./profile-contacts";
import { EducationCard, EducationDialog } from "./profile-education";
import { EmploymentCard, EmploymentDialog } from "./profile-employment";
import {
  buildUpdateRequest,
  getDefaultValues,
  profileFormSchema,
  type ProfileFormValues,
} from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

// Two-column section grid: 240px description | 1fr content
const SECTION = "grid gap-8 border-t border-border/40 py-6 md:grid-cols-[240px_1fr]";

function SectionMeta({
  title,
  description,
}: {
  title: string;
  description: string;
}) {
  return (
    <div>
      <p className="text-sm font-medium">{title}</p>
      <p className="mt-1 text-xs text-muted-foreground">{description}</p>
    </div>
  );
}

export function ProfileEditor({ profile }: { profile: OwnProfileDetails }) {
  const [contactDialog, setContactDialog] = useState(false);
  const [employmentDialog, setEmploymentDialog] = useState(false);
  const [educationDialog, setEducationDialog] = useState(false);

  const updateMutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Профиль сохранён");
      // Reset dirty state without changing values — server has accepted the data.
      form.reset(form.getValues());
    },
    onError: () => {
      toast.error("Не удалось сохранить профиль");
    },
  });

  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileFormSchema),
    defaultValues: getDefaultValues(profile),
  });

  // Sync form with server state, but only when the form is clean to avoid wiping unsaved changes.
  useEffect(() => {
    if (!form.formState.isDirty) {
      form.reset(getDefaultValues(profile));
    }
  }, [profile]); // eslint-disable-line react-hooks/exhaustive-deps

  const contactsArray = useFieldArray({ control: form.control, name: "contacts" });
  const employmentsArray = useFieldArray({ control: form.control, name: "employmentHistories" });
  const educationsArray = useFieldArray({ control: form.control, name: "educations" });

  function onSubmit(data: ProfileFormValues) {
    updateMutation.mutate(buildUpdateRequest(data));
  }

  const isDirty = form.formState.isDirty;
  const isPending = updateMutation.isPending;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>

        {/* ── Avatar ── */}
        <div className="pb-6">
          <AvatarBlock profile={profile} getFormValues={form.getValues} />
        </div>

        {/* ── Personal info ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Личная информация"
            description="Ваше имя и дата рождения"
          />
          <div className="space-y-3">
            <div className="grid gap-3 sm:grid-cols-2">
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
            <div className="grid gap-3 sm:grid-cols-2">
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
        </section>

        {/* ── Contacts ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Контакты"
            description="Email, телефон и другие способы связи"
          />
          <div className="space-y-2">
            <div className="flex justify-end">
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={() => setContactDialog(true)}
                className="h-7 gap-1.5 px-2.5 text-xs text-muted-foreground hover:text-foreground"
              >
                <Plus className="size-3" />
                Добавить
              </Button>
            </div>
            {contactsArray.fields.length === 0 ? (
              <EmptyState
                icon={<Mail className="size-5" />}
                text="Способы связи не добавлены"
                onAdd={() => setContactDialog(true)}
              />
            ) : (
              <div className="space-y-0.5">
                {contactsArray.fields.map((field, index) => (
                  <ContactRow
                    key={field.id}
                    field={field}
                    onRemove={() => contactsArray.remove(index)}
                  />
                ))}
              </div>
            )}
          </div>
          <ContactDialog
            open={contactDialog}
            onOpenChange={setContactDialog}
            onAppend={(data) => contactsArray.append(data)}
          />
        </section>

        {/* ── Employment ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Опыт работы"
            description="История вашей трудовой деятельности"
          />
          <div className="space-y-2">
            <div className="flex justify-end">
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={() => setEmploymentDialog(true)}
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
                onAdd={() => setEmploymentDialog(true)}
              />
            ) : (
              <div className="space-y-2">
                {employmentsArray.fields.map((field, index) => (
                  <EmploymentCard
                    key={field.id}
                    field={field}
                    onRemove={() => employmentsArray.remove(index)}
                  />
                ))}
              </div>
            )}
          </div>
          <EmploymentDialog
            open={employmentDialog}
            onOpenChange={setEmploymentDialog}
            onAppend={(data) => employmentsArray.append(data)}
          />
        </section>

        {/* ── Education ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Образование"
            description="Учебные заведения и уровни образования"
          />
          <div className="space-y-2">
            <div className="flex justify-end">
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={() => setEducationDialog(true)}
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
                onAdd={() => setEducationDialog(true)}
              />
            ) : (
              <div className="space-y-2">
                {educationsArray.fields.map((field, index) => (
                  <EducationCard
                    key={field.id}
                    field={field}
                    onRemove={() => educationsArray.remove(index)}
                  />
                ))}
              </div>
            )}
          </div>
          <EducationDialog
            open={educationDialog}
            onOpenChange={setEducationDialog}
            onAppend={(data) => educationsArray.append(data)}
          />
        </section>

        {/* ── Save bar ── */}
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
            Сохранить изменения
          </Button>
        </div>

      </form>
    </Form>
  );
}
