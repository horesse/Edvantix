"use client";

import { useEffect, useState } from "react";

import { Loader2, Mail, Plus } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import { zodResolver } from "@hookform/resolvers/zod";

import useUpdateContacts from "@workspace/api-hooks/profiles/useUpdateContacts";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
import { Form } from "@workspace/ui/components/form";
import { contactSchema } from "@workspace/validations/profile";

import { ContactDialog, ContactRow } from "./profile-contacts";
import type { ContactInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

const formSchema = z.object({
  contacts: z.array(contactSchema),
});

type FormValues = z.infer<typeof formSchema>;

export function TabContacts({ profile }: { profile: OwnProfileDetails }) {
  const [dialogOpen, setDialogOpen] = useState(false);

  const mutation = useUpdateContacts({
    onSuccess: () => {
      toast.success("Контакты сохранены");
      form.reset(form.getValues());
    },
    onError: () => toast.error("Не удалось сохранить контакты"),
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      contacts: profile.contacts.map((c) => ({
        type: c.type,
        value: c.value,
        description: c.description ?? "",
      })),
    },
  });

  useEffect(() => {
    if (!form.formState.isDirty) {
      form.reset({
        contacts: profile.contacts.map((c) => ({
          type: c.type,
          value: c.value,
          description: c.description ?? "",
        })),
      });
    }
  }, [profile]); // eslint-disable-line react-hooks/exhaustive-deps

  const contactsArray = useFieldArray({
    control: form.control,
    name: "contacts",
  });

  function onSubmit(data: FormValues) {
    mutation.mutate({
      contacts: data.contacts.map((c) => ({
        type: c.type,
        value: c.value,
        description: c.description || null,
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
            Email, телефон и другие способы связи
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

        {contactsArray.fields.length === 0 ? (
          <EmptyState
            icon={<Mail className="size-5" />}
            text="Способы связи не добавлены"
            onAdd={() => setDialogOpen(true)}
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

        <ContactDialog
          open={dialogOpen}
          onOpenChange={setDialogOpen}
          onAppend={(data: ContactInput) => contactsArray.append(data)}
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
