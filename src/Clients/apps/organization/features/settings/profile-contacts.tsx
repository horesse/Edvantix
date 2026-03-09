"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  Globe,
  Mail,
  MoreHorizontal,
  MoreVertical,
  Phone,
  Trash2,
} from "lucide-react";
import { useForm } from "react-hook-form";

import { ContactType } from "@workspace/types/profile";
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
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
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
import { cn } from "@workspace/ui/lib/utils";
import { contactSchema } from "@workspace/validations/profile";

import type { ContactInput } from "./profile-settings-schema";

const contactTypeMeta: Record<
  ContactType,
  { label: string; icon: typeof Mail; color: string }
> = {
  [ContactType.Email]: {
    label: "Email",
    icon: Mail,
    color: "bg-blue-500/10 text-blue-600 dark:text-blue-400",
  },
  [ContactType.Phone]: {
    label: "Телефон",
    icon: Phone,
    color: "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400",
  },
  [ContactType.Uri]: {
    label: "Веб-сайт",
    icon: Globe,
    color: "bg-violet-500/10 text-violet-600 dark:text-violet-400",
  },
  [ContactType.Other]: {
    label: "Другое",
    icon: MoreHorizontal,
    color: "bg-muted text-muted-foreground",
  },
};

export function ContactRow({
  field,
  onRemove,
}: {
  field: ContactInput & { id: string };
  onRemove: () => void;
}) {
  const meta = contactTypeMeta[field.type];
  const Icon = meta.icon;

  return (
    <div className="group hover:bg-muted/40 flex items-center gap-3 rounded-lg px-3 py-2.5 transition-colors">
      <span
        className={cn(
          "flex size-8 shrink-0 items-center justify-center rounded-lg",
          meta.color,
        )}
      >
        <Icon className="size-3.5" />
      </span>

      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-medium">{field.value}</p>
        <p className="text-muted-foreground truncate text-xs">
          {meta.label}
          {field.description ? ` · ${field.description}` : ""}
        </p>
      </div>

      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <button
            type="button"
            className="text-muted-foreground/50 hover:bg-muted hover:text-foreground flex size-7 shrink-0 items-center justify-center rounded-md opacity-0 transition-all group-hover:opacity-100"
            aria-label="Действия"
          >
            <MoreVertical className="size-3.5" />
          </button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-36">
          <DropdownMenuItem
            onClick={onRemove}
            className="text-destructive focus:text-destructive"
          >
            <Trash2 className="size-3.5" />
            Удалить
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
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
                      {Object.entries(contactTypeMeta).map(([key, meta]) => (
                        <SelectItem key={key} value={key}>
                          <span className="flex items-center gap-2">
                            <meta.icon className="text-muted-foreground size-3.5" />
                            {meta.label}
                          </span>
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
