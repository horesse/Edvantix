"use client";

import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import { Mail, Plus } from "lucide-react";

import type { ContactRequest, OwnProfileDetails } from "@workspace/types/profile";

import { ContactDialog } from "../dialogs/contact-dialog";
import { ContactItem } from "../items/contact-item";
import type { ContactInput } from "../schema";
import type { ContactsSectionHandle } from "../types";

/** Empty state shown when no contacts have been added. */
function ContactsEmptyState({ onAdd }: { onAdd: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center px-4 py-8">
      <div className="relative mb-5">
        <div className="flex size-20 items-center justify-center rounded-2xl border-2 border-dashed border-blue-200 bg-blue-50 dark:border-blue-800/50 dark:bg-blue-950/20">
          <Mail
            className="size-10 text-blue-300 dark:text-blue-600"
            strokeWidth={1.2}
          />
        </div>
        <div className="absolute -top-2 -right-2 flex size-6 items-center justify-center rounded-full bg-blue-400 shadow-sm dark:bg-blue-600">
          <Plus className="size-3 text-white" strokeWidth={2.5} />
        </div>
      </div>
      <p className="text-foreground mb-1 text-sm font-semibold">
        Контакты не добавлены
      </p>
      <p className="text-muted-foreground mb-4 max-w-xs text-center text-xs leading-relaxed">
        Добавьте email, телефон или мессенджеры — ученики смогут с вами
        связаться.
      </p>
      <button
        type="button"
        onClick={onAdd}
        className="inline-flex items-center gap-2 rounded-xl bg-blue-500 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-600 dark:bg-blue-600 dark:hover:bg-blue-700"
      >
        <Plus className="size-4" strokeWidth={2.5} />
        Добавить контакт
      </button>
    </div>
  );
}

export const ContactsSection = forwardRef<
  ContactsSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function ContactsSection({ profile, onDirtyChange }, ref) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [contacts, setContacts] = useState<ContactInput[]>(() =>
    profile.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description ?? "",
    })),
  );
  const [savedSnapshot, setSavedSnapshot] = useState(contacts);

  function handleAppend(data: ContactInput) {
    setContacts((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setContacts((prev) => prev.filter((_, i) => i !== index));
  }

  const isDirty = JSON.stringify(contacts) !== JSON.stringify(savedSnapshot);

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    getPayload(): ContactRequest[] {
      return contacts.map((c) => ({
        type: c.type,
        value: c.value,
        description: c.description || null,
      }));
    },
    acknowledgeServerState() {
      setSavedSnapshot(contacts);
    },
  }));

  return (
    <div className="space-y-3">
      <div className="flex justify-end">
        <button
          type="button"
          onClick={() => setDialogOpen(true)}
          className="border-primary/20 bg-primary/5 text-primary hover:border-primary/40 hover:bg-primary/10 flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-semibold transition-colors"
        >
          <Plus className="size-3.5" />
          Добавить
        </button>
      </div>

      {contacts.length === 0 ? (
        <ContactsEmptyState onAdd={() => setDialogOpen(true)} />
      ) : (
        <div className="space-y-0.5">
          {contacts.map((contact, index) => (
            <ContactItem
              key={`${contact.type}-${contact.value}-${index}`}
              contact={contact}
              onRemove={() => handleRemove(index)}
            />
          ))}
        </div>
      )}

      <ContactDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onAdd={handleAppend}
      />
    </div>
  );
});
