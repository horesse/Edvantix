"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Check, GraduationCap, X } from "lucide-react";
import { useForm } from "react-hook-form";

import { EducationLevel } from "@workspace/types/profile";
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

import { educationLevelLabels } from "../items/education-item";
import type { EducationInput } from "../schema";
import { educationSchema } from "../schema";

export function EducationDialog({
  open,
  onOpenChange,
  onAdd,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAdd: (data: EducationInput) => void;
}) {
  const form = useForm<EducationInput>({
    resolver: zodResolver(educationSchema),
    defaultValues: {
      institution: "",
      specialty: "",
      dateStart: "",
      dateEnd: "",
      level: undefined,
    },
  });

  function handleSubmit(data: EducationInput) {
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
        className="gap-0 overflow-hidden p-0 sm:max-w-lg"
      >
        <div className="border-border flex items-center justify-between border-b px-6 py-4">
          <div className="flex items-center gap-3">
            <div className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-teal-100 dark:bg-teal-900/30">
              <GraduationCap className="size-4 text-teal-600 dark:text-teal-400" />
            </div>
            <div>
              <DialogTitle className="text-foreground text-sm font-semibold">
                Новое образование
              </DialogTitle>
              <DialogDescription className="text-muted-foreground text-xs">
                Укажите информацию об образовании
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
            <div className="max-h-[60vh] space-y-4 overflow-y-auto px-6 py-5">
              <FormField
                control={form.control}
                name="institution"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Учебное заведение{" "}
                      <span className="text-destructive">*</span>
                    </FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Название учебного заведения"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="specialty"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Специальность{" "}
                      <span className="text-muted-foreground font-normal">
                        (необязательно)
                      </span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="Информатика" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="level"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Уровень образования{" "}
                      <span className="text-destructive">*</span>
                    </FormLabel>
                    <Select
                      onValueChange={(v) =>
                        field.onChange(Number(v) as EducationLevel)
                      }
                      value={
                        field.value != null ? String(field.value) : undefined
                      }
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Выберите уровень" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {(
                          Object.entries(educationLevelLabels) as [
                            string,
                            string,
                          ][]
                        ).map(([key, label]) => (
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
              <div className="grid gap-4 sm:grid-cols-2">
                <FormField
                  control={form.control}
                  name="dateStart"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Дата начала <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input type="date" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="dateEnd"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Дата окончания{" "}
                        <span className="text-muted-foreground font-normal">
                          (пусто = учусь)
                        </span>
                      </FormLabel>
                      <FormControl>
                        <Input type="date" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
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
                  Сохранить
                </button>
              </div>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
