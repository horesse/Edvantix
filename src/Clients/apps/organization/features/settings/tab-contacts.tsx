"use client";

import { useEffect, useState } from "react";

import { Loader2, Mail, Plus } from "lucide-react";
import { toast } from "sonner";

import useUpdateContacts from "@workspace/api-hooks/profiles/useUpdateContacts";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";

import { ContactDialog, ContactRow } from "./profile-contacts";
import type { ContactInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

export function TabContacts({
  profile,
  onDirtyChange,
}: {
  profile: OwnProfileDetails;
  onDirtyChange?: (dirty: boolean) => void;
}) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [contacts, setContacts] = useState<ContactInput[]>(() =>
    profile.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description ?? "",
    })),
  );
  const [savedSnapshot, setSavedSnapshot] = useState(contacts);

  const mutation = useUpdateContacts({
    onSuccess: (data) => {
      toast.success("Контакты сохранены");
      const server = data.contacts.map((c) => ({
        type: c.type,
        value: c.value,
        description: c.description ?? "",
      }));
      setSavedSnapshot(server);
    },
    onError: () => toast.error("Не удалось сохранить контакты"),
  });

  function handleAppend(data: ContactInput) {
    setContacts((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setContacts((prev) => prev.filter((_, i) => i !== index));
  }

  function handleSubmit() {
    mutation.mutate({
      contacts: contacts.map((c) => ({
        type: c.type,
        value: c.value,
        description: c.description || null,
      })),
    });
  }

  const isDirty = JSON.stringify(contacts) !== JSON.stringify(savedSnapshot);
  const isPending = mutation.isPending;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        handleSubmit();
      }}
      className="space-y-4"
    >
      <div className="flex items-center justify-between">
        <p className="text-muted-foreground text-sm">
          Email, телефон и другие способы связи
        </p>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setDialogOpen(true)}
          className="text-muted-foreground hover:text-foreground h-7 gap-1.5 px-2.5 text-xs"
        >
          <Plus className="size-3" />
          Добавить
        </Button>
      </div>

      {contacts.length === 0 ? (
        <EmptyState
          icon={<Mail className="size-5" />}
          text="Способы связи не добавлены"
          onAdd={() => setDialogOpen(true)}
        />
      ) : (
        <div className="space-y-0.5">
          {contacts.map((contact, index) => (
            <ContactRow
              key={`${contact.type}-${contact.value}-${index}`}
              field={{ ...contact, id: String(index) }}
              onRemove={() => handleRemove(index)}
            />
          ))}
        </div>
      )}

      <ContactDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onAppend={handleAppend}
      />

      <div className="border-border/40 flex items-center justify-between border-t pt-4">
        {isDirty && !isPending ? (
          <p className="text-muted-foreground text-xs">
            Есть несохранённые изменения
          </p>
        ) : (
          <span />
        )}
        <Button type="submit" disabled={isPending || !isDirty} size="sm">
          {isPending && <Loader2 className="size-3.5 animate-spin" />}
          Сохранить{" "}
        </Button>
      </div>
    </form>
  );
}
