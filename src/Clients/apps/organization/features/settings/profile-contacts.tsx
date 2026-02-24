"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Globe, Mail, MoreHorizontal, Phone, Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";

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
import { contactSchema } from "@workspace/validations/profile";

import type { ContactInput } from "./profile-settings-schema";

const contactTypeLabels: Record<ContactType, string> = {
  [ContactType.Email]: "Email",
  [ContactType.Phone]: "Телефон",
  [ContactType.Uri]: "Веб-сайт",
  [ContactType.Other]: "Другое",
};

const contactTypeIcons: Record<ContactType, React.ReactNode> = {
  [ContactType.Email]: <Mail className="size-3.5" />,
  [ContactType.Phone]: <Phone className="size-3.5" />,
  [ContactType.Uri]: <Globe className="size-3.5" />,
  [ContactType.Other]: <MoreHorizontal className="size-3.5" />,
};

export function ContactRow({
  field,
  onRemove,
}: {
  field: ContactInput & { id: string };
  onRemove: () => void;
}) {
  return (
    <div className="group flex items-center gap-3 rounded-md px-2 py-2.5 transition-colors hover:bg-muted/30">
      <span className="shrink-0 text-muted-foreground">
        {contactTypeIcons[field.type]}
      </span>
      <div className="min-w-0 flex-1">
        <span className="truncate text-sm">{field.value}</span>
        {field.description && (
          <p className="truncate text-xs text-muted-foreground/60">
            {field.description}
          </p>
        )}
      </div>
      <Badge
        variant="secondary"
        className="shrink-0 rounded-sm px-1.5 py-0 text-[10px] font-normal"
      >
        {contactTypeLabels[field.type]}
      </Badge>
      <button
        type="button"
        onClick={onRemove}
        className="ml-0.5 shrink-0 text-transparent transition-colors group-hover:text-muted-foreground/40 hover:!text-destructive"
        aria-label="Удалить контакт"
      >
        <Trash2 className="size-3.5" />
      </button>
    </div>
  );
}

export function ContactDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: ContactInput) => void;
}) {
  const form = useForm<ContactInput>({
    resolver: zodResolver(contactSchema),
    defaultValues: { type: ContactType.Email, value: "", description: "" },
  });

  function handleSubmit(data: ContactInput) {
    onAppend(data);
    form.reset();
    onOpenChange(false);
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
                    <span className="font-normal text-muted-foreground">
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
                size="sm"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit" size="sm">
                Добавить
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
