"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Briefcase, Check, X } from "lucide-react";
import { useForm } from "react-hook-form";

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
import { Textarea } from "@workspace/ui/components/textarea";

import type { EmploymentInput } from "../schema";
import { employmentSchema } from "../schema";

export function EmploymentDialog({
  open,
  onOpenChange,
  onAdd,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAdd: (data: EmploymentInput) => void;
}) {
  const [currentlyWorking, setCurrentlyWorking] = useState(false);

  const form = useForm<EmploymentInput>({
    resolver: zodResolver(employmentSchema),
    defaultValues: {
      workplace: "",
      position: "",
      startDate: "",
      endDate: "",
      description: "",
    },
  });

  function handleSubmit(data: EmploymentInput) {
    onAdd(data);
    form.reset();
    setCurrentlyWorking(false);
    onOpenChange(false);
  }

  function handleClose() {
    form.reset();
    setCurrentlyWorking(false);
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
            <div className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-amber-100 dark:bg-amber-900/30">
              <Briefcase className="size-4 text-amber-600 dark:text-amber-400" />
            </div>
            <div>
              <DialogTitle className="text-foreground text-sm font-semibold">
                Добавить место работы
              </DialogTitle>
              <DialogDescription className="text-muted-foreground text-xs">
                Укажите данные об опыте работы
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
                name="workplace"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Организация <span className="text-destructive">*</span>
                    </FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Яндекс, Mail.ru, Школа №42…"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="position"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Должность <span className="text-destructive">*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="Преподаватель" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <div className="grid gap-4 sm:grid-cols-2">
                <FormField
                  control={form.control}
                  name="startDate"
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
                  name="endDate"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Дата окончания</FormLabel>
                      <FormControl>
                        <Input
                          type="date"
                          disabled={currentlyWorking}
                          {...field}
                          value={currentlyWorking ? "" : field.value}
                        />
                      </FormControl>
                      <label className="mt-2 flex cursor-pointer items-center gap-1.5">
                        <input
                          type="checkbox"
                          className="accent-brand-600 size-3.5 rounded"
                          checked={currentlyWorking}
                          onChange={(e) => {
                            setCurrentlyWorking(e.target.checked);
                            if (e.target.checked) field.onChange("");
                          }}
                        />
                        <span className="text-muted-foreground text-xs">
                          По настоящее время
                        </span>
                      </label>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
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
                      <Textarea
                        placeholder="Краткое описание задач и достижений…"
                        rows={3}
                        {...field}
                      />
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
