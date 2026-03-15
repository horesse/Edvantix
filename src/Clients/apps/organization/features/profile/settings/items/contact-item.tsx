"use client";

import { Globe, Mail, MoreHorizontal, Phone, Trash2 } from "lucide-react";

import { ContactType } from "@workspace/types/profile";
import { cn } from "@workspace/ui/lib/utils";

import type { ContactInput } from "../schema";

export const contactTypeMeta: Record<
  ContactType,
  { label: string; icon: typeof Mail; color: string }
> = {
  [ContactType.Email]: {
    label: "Email",
    icon: Mail,
    color: "bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400",
  },
  [ContactType.Phone]: {
    label: "Телефон",
    icon: Phone,
    color:
      "bg-emerald-100 text-emerald-600 dark:bg-emerald-900/30 dark:text-emerald-400",
  },
  [ContactType.Uri]: {
    label: "Веб-сайт",
    icon: Globe,
    color:
      "bg-violet-100 text-violet-600 dark:bg-violet-900/30 dark:text-violet-400",
  },
  [ContactType.Other]: {
    label: "Другое",
    icon: MoreHorizontal,
    color: "bg-muted text-muted-foreground",
  },
};

export function ContactItem({
  contact,
  onRemove,
}: {
  contact: ContactInput;
  onRemove: () => void;
}) {
  const meta =
    contactTypeMeta[contact.type as ContactType] ??
    contactTypeMeta[ContactType.Other];
  const Icon = meta.icon;

  return (
    <div className="group border-border bg-muted/30 mb-2 flex items-center gap-3 rounded-xl border p-3">
      <span
        className={cn(
          "flex size-8 shrink-0 items-center justify-center rounded-lg",
          meta.color,
        )}
      >
        <Icon className="size-4" />
      </span>
      <div className="min-w-0 flex-1">
        <p className="text-foreground truncate text-sm font-medium">
          {contact.value}
        </p>
        <p className="text-muted-foreground truncate text-xs">
          {meta.label}
          {contact.description ? ` · ${contact.description}` : ""}
        </p>
      </div>
      <button
        type="button"
        onClick={onRemove}
        aria-label="Удалить"
        className="text-muted-foreground/50 hover:bg-destructive/10 hover:text-destructive flex size-7 shrink-0 items-center justify-center rounded-lg opacity-0 transition-all group-hover:opacity-100"
      >
        <Trash2 className="size-4" />
      </button>
    </div>
  );
}
