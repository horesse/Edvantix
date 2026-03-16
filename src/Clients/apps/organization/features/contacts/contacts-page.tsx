"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  Globe,
  Loader2,
  Mail,
  MoreHorizontal,
  Pencil,
  Phone,
  Plus,
  Trash2,
} from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useAddOrganizationContact from "@workspace/api-hooks/company/useAddOrganizationContact";
import useDeleteOrganizationContact from "@workspace/api-hooks/company/useDeleteOrganizationContact";
import useOrganizationContacts from "@workspace/api-hooks/company/useOrganizationContacts";
import useUpdateOrganizationContact from "@workspace/api-hooks/company/useUpdateOrganizationContact";
import type { OrganizationContactModel } from "@workspace/types/company";
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
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  type OrganizationContactInput,
  organizationContactSchema,
} from "@workspace/validations/company";

import { PageHeader } from "@/components/layout/page-header";
import { useOrganization } from "@/components/organization/provider";
import { contactTypeLabels } from "@/lib/company-options";

const contactTypeIcons: Record<ContactType, React.ReactNode> = {
  [ContactType.Email]: <Mail className="size-4" />,
  [ContactType.Phone]: <Phone className="size-4" />,
  [ContactType.Uri]: <Globe className="size-4" />,
  [ContactType.Other]: <MoreHorizontal className="size-4" />,
};

export function ContactsPage() {
  const { currentOrg, canManage } = useOrganization();
  const orgId = currentOrg?.id ?? "";
  const { data, isLoading } = useOrganizationContacts(orgId);
  const contacts = data?.items ?? [];

  const [addOpen, setAddOpen] = useState(false);
  const [editContact, setEditContact] =
    useState<OrganizationContactModel | null>(null);

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-4">
      <PageHeader
        title="Контакты"
        actions={
          canManage && (
            <Button size="sm" onClick={() => setAddOpen(true)}>
              <Plus className="size-4" />
              Добавить
            </Button>
          )
        }
      />

      {isLoading ? (
        <div className="space-y-2">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-16 w-full" />
          ))}
        </div>
      ) : contacts.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
          <Mail className="text-muted-foreground/50 mb-3 size-10" />
          <p className="text-muted-foreground text-sm">Контакты не добавлены</p>
          {canManage && (
            <Button
              variant="link"
              size="sm"
              className="mt-1"
              onClick={() => setAddOpen(true)}
            >
              Добавить первый контакт
            </Button>
          )}
        </div>
      ) : (
        <div className="divide-border/50 divide-y rounded-lg border">
          {contacts.map((contact) => (
            <ContactRow
              key={contact.id}
              contact={contact}
              orgId={orgId}
              canManage={canManage}
              onEdit={() => setEditContact(contact)}
            />
          ))}
        </div>
      )}

      <AddContactDialog
        orgId={orgId}
        open={addOpen}
        onOpenChange={setAddOpen}
      />
      <EditContactDialog
        orgId={orgId}
        contact={editContact}
        onClose={() => setEditContact(null)}
      />
    </div>
  );
}

function ContactRow({
  contact,
  orgId,
  canManage,
  onEdit,
}: {
  contact: OrganizationContactModel;
  orgId: string;
  canManage: boolean;
  onEdit: () => void;
}) {
  const deleteMutation = useDeleteOrganizationContact({
    onSuccess: () => toast.success("Контакт удалён"),
    onError: () => toast.error("Не удалось удалить контакт"),
  });

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
      {canManage && (
        <div className="flex shrink-0 gap-1 opacity-0 transition-opacity group-hover:opacity-100">
          <Button
            variant="ghost"
            size="sm"
            className="text-muted-foreground h-8 w-8 p-0"
            onClick={onEdit}
          >
            <Pencil className="size-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            className="text-muted-foreground hover:text-destructive h-8 w-8 p-0"
            onClick={() =>
              deleteMutation.mutate({ orgId, contactId: contact.id })
            }
            disabled={deleteMutation.isPending}
          >
            <Trash2 className="size-4" />
          </Button>
        </div>
      )}
    </div>
  );
}

function AddContactDialog({
  orgId,
  open,
  onOpenChange,
}: {
  orgId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const form = useForm<OrganizationContactInput>({
    resolver: zodResolver(organizationContactSchema),
    defaultValues: {
      type: ContactType.Email,
      value: "",
      description: "",
    },
  });

  const mutation = useAddOrganizationContact({
    onSuccess: () => {
      toast.success("Контакт добавлен");
      onOpenChange(false);
      form.reset();
    },
    onError: () => toast.error("Не удалось добавить контакт"),
  });

  function handleSubmit(data: OrganizationContactInput) {
    mutation.mutate({
      orgId,
      request: {
        type: data.type,
        value: data.value,
        description: data.description || null,
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Новый контакт</DialogTitle>
          <DialogDescription>
            Добавьте контактную информацию организации
          </DialogDescription>
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
                    <Input placeholder="info@school.ru" {...field} />
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
                    <Input placeholder="Приёмная" {...field} />
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
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending && (
                  <Loader2 className="size-4 animate-spin" />
                )}
                Добавить
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

function EditContactDialog({
  orgId,
  contact,
  onClose,
}: {
  orgId: string;
  contact: OrganizationContactModel | null;
  onClose: () => void;
}) {
  const form = useForm<OrganizationContactInput>({
    resolver: zodResolver(organizationContactSchema),
    values: contact
      ? {
          type: contact.type,
          value: contact.value,
          description: contact.description ?? "",
        }
      : undefined,
  });

  const mutation = useUpdateOrganizationContact({
    onSuccess: () => {
      toast.success("Контакт обновлён");
      onClose();
    },
    onError: () => toast.error("Не удалось обновить контакт"),
  });

  function handleSubmit(data: OrganizationContactInput) {
    if (!contact) return;
    mutation.mutate({
      orgId,
      contactId: contact.id,
      request: {
        type: data.type,
        value: data.value,
        description: data.description || null,
      },
    });
  }

  return (
    <Dialog open={contact !== null} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Редактировать контакт</DialogTitle>
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
                        <SelectValue />
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
                    <Input {...field} />
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
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button type="button" variant="outline" onClick={onClose}>
                Отмена
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending && (
                  <Loader2 className="size-4 animate-spin" />
                )}
                Сохранить
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
