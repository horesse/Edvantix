"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Check, Mail, X } from "lucide-react";
import { useForm } from "react-hook-form";

import { ContactType } from "@workspace/types/profile";
import {
  Dialog,
  DialogContent,
  DialogDescription,
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

import { contactTypeMeta } from "../items/contact-item";
import type { ContactInput } from "../schema";
import { contactSchema } from "../schema";

export function ContactDialog({
  open,
  onOpenChange,
  onAdd,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAdd: (data: ContactInput) => void;
}) {
  const form = useForm<ContactInput>({
    resolver: zodResolver(contactSchema),
    defaultValues: { type: ContactType.Email, value: "", description: "" },
  });

  function handleSubmit(data: ContactInput) {
    onAdd(data);
    form.reset();
    onOpenChange(false);
  }

  function handleClose() {
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(v) => {
        if (!v) handleClose();
      }}
    >
      <DialogContent
        showCloseButton={false}
        className="gap-0 overflow-hidden p-0 sm:max-w-md"
      >
        <div className="border-border flex items-center justify-between border-b px-6 py-4">
          <div className="flex items-center gap-3">
            <div className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-blue-100 dark:bg-blue-900/30">
              <Mail className="size-4 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <DialogTitle className="text-foreground text-sm font-semibold">
                Новый контакт
              </DialogTitle>
              <DialogDescription className="text-muted-foreground text-xs">
                Добавьте способ связи с вами
              </DialogDescription>
            </div>
          </div>
          <button
            type="button"
            onClick={handleClose}
            className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-8 items-center justify-center rounded-lg transition-colors"
          >
            <X className="size-4" />
          </button>
        </div>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)}>
            <div className="space-y-4 px-6 py-5">
              <FormField
                control={form.control}
                name="type"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Тип</FormLabel>
                    <Select
                      onValueChange={(v) =>
                        field.onChange(Number(v) as ContactType)
                      }
                      value={String(field.value ?? ContactType.Email)}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Выберите тип" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {(
                          Object.entries(contactTypeMeta) as [
                            string,
                            (typeof contactTypeMeta)[ContactType],
                          ][]
                        ).map(([key, meta]) => (
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
                    <FormLabel>
                      Значение <span className="text-destructive">*</span>
                    </FormLabel>
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
            </div>
            <div className="border-border bg-muted/30 flex items-center justify-between border-t px-6 py-4">
              <p className="text-muted-foreground text-xs">
                * — обязательные поля
              </p>
              <div className="flex gap-2">
                <button
                  type="button"
                  onClick={handleClose}
                  className="border-border text-muted-foreground hover:bg-background rounded-xl border px-4 py-2 text-sm font-medium transition-colors"
                >
                  Отмена
                </button>
                <button
                  type="submit"
                  className="bg-primary text-primary-foreground hover:bg-primary/90 flex items-center gap-1.5 rounded-xl px-4 py-2 text-sm font-semibold shadow-sm transition-colors"
                >
                  <Check className="size-3.5" />
                  Добавить
                </button>
              </div>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
