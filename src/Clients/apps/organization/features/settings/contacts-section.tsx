"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  Globe,
  Loader2,
  Mail,
  MoreHorizontal,
  Phone,
  Plus,
  Trash2,
} from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useUpdateContacts from "@workspace/api-hooks/profiles/useUpdateContacts";
import type { Contact, OwnProfileDetails } from "@workspace/types/profile";
import { ContactType } from "@workspace/types/profile";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import {
  type ContactInput,
  contactSchema,
} from "@workspace/validations/profile/settings";

const contactTypeLabels: Record<ContactType, string> = {
  [ContactType.Email]: "Email",
  [ContactType.Phone]: "Телефон",
  [ContactType.Uri]: "Веб-сайт",
  [ContactType.Other]: "Другое",
};

const contactTypeIcons: Record<ContactType, React.ReactNode> = {
  [ContactType.Email]: <Mail className="size-4" />,
  [ContactType.Phone]: <Phone className="size-4" />,
  [ContactType.Uri]: <Globe className="size-4" />,
  [ContactType.Other]: <MoreHorizontal className="size-4" />,
};

type ContactsSectionProps = {
  profile: OwnProfileDetails;
};

export function ContactsSection({ profile }: ContactsSectionProps) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const contacts = profile.contacts ?? [];

  const updateMutation = useUpdateContacts({
    onSuccess: () => {
      toast.success("Контакты обновлены");
    },
    onError: () => {
      toast.error("Не удалось обновить контакты");
    },
  });

  function handleRemoveContact(index: number) {
    const updated = contacts.filter((_, i) => i !== index);
    updateMutation.mutate(updated);
  }

  function handleAddContact(data: ContactInput) {
    const updated = [
      ...contacts,
      {
        type: data.type,
        value: data.value,
        description: data.description || null,
      },
    ];
    updateMutation.mutate(updated, {
      onSuccess: () => {
        setDialogOpen(false);
        toast.success("Контакт добавлен");
      },
    });
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="font-medium">Контакты</h3>
          <p className="text-muted-foreground text-sm">Способы связи с вами</p>
        </div>
        <Button variant="outline" size="sm" onClick={() => setDialogOpen(true)}>
          <Plus className="size-4" />
          <span className="hidden sm:inline">Добавить</span>
        </Button>
      </div>

      {contacts.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-8">
          <Mail className="text-muted-foreground/50 mb-3 size-10" />
          <p className="text-muted-foreground text-sm">Контакты не добавлены</p>
          <Button
            variant="link"
            size="sm"
            className="mt-1"
            onClick={() => setDialogOpen(true)}
          >
            Добавить первый контакт
          </Button>
        </div>
      ) : (
        <div className="divide-border/50 divide-y rounded-lg border">
          {contacts.map((contact, index) => (
            <ContactRow
              key={`${contact.type}-${contact.value}`}
              contact={contact}
              onRemove={() => handleRemoveContact(index)}
              isLoading={updateMutation.isPending}
            />
          ))}
        </div>
      )}

      <AddContactDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={handleAddContact}
        isLoading={updateMutation.isPending}
      />
    </div>
  );
}

function ContactRow({
  contact,
  onRemove,
  isLoading,
}: {
  contact: Contact;
  onRemove: () => void;
  isLoading: boolean;
}) {
  return (
    <div className="group hover:bg-muted/50 flex items-center gap-3 px-4 py-3 transition-colors">
      <div className="text-muted-foreground bg-muted flex size-9 shrink-0 items-center justify-center rounded-lg">
        {contactTypeIcons[contact.type]}
      </div>
      <div className="min-w-0 flex-1">
        <div className="flex items-center gap-2">
          <span className="truncate text-sm font-medium">{contact.value}</span>
          <Badge variant="secondary" className="shrink-0 text-[10px]">
            {contactTypeLabels[contact.type]}
          </Badge>
        </div>
        {contact.description && (
          <p className="text-muted-foreground truncate text-xs">
            {contact.description}
          </p>
        )}
      </div>
      <Button
        variant="ghost"
        size="sm"
        className="text-muted-foreground hover:text-destructive h-8 w-8 shrink-0 p-0 opacity-0 transition-opacity group-hover:opacity-100"
        onClick={onRemove}
        disabled={isLoading}
      >
        <Trash2 className="size-4" />
      </Button>
    </div>
  );
}

function AddContactDialog({
  open,
  onOpenChange,
  onSubmit,
  isLoading,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: ContactInput) => void;
  isLoading: boolean;
}) {
  const form = useForm<ContactInput>({
    resolver: zodResolver(contactSchema),
    defaultValues: {
      type: ContactType.Email,
      value: "",
      description: "",
    },
  });

  function handleSubmit(data: ContactInput) {
    onSubmit(data);
    form.reset();
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Новый контакт</DialogTitle>
          <DialogDescription>Добавьте способ связи с вами</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Тип</FormLabel>
                  <Select
                    onValueChange={(v) => field.onChange(Number(v))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Выберите тип" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(contactTypeLabels).map(([key, label]) => (
                        <SelectItem key={key} value={key}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="value"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Значение</FormLabel>
                  <FormControl>
                    <Input placeholder="example@mail.com" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Описание{" "}
                    <span className="text-muted-foreground font-normal">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder="Рабочий email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading && <Loader2 className="size-4 animate-spin" />}
                <span>Добавить</span>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
