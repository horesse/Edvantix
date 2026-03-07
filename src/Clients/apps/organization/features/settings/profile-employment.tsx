"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { MoreVertical, Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";

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
import { Textarea } from "@workspace/ui/components/textarea";
import { formatDateRange } from "@workspace/utils/format";
import { employmentSchema } from "@workspace/validations/profile";

import type { EmploymentInput } from "./profile-settings-schema";

export function EmploymentCard({
  field,
  onRemove,
  isLast,
}: {
  field: EmploymentInput & { id: string };
  onRemove: () => void;
  isLast?: boolean;
}) {
  return (
    <div className="group relative flex gap-3">
      {/* Timeline line + dot */}
      <div className="flex flex-col items-center pt-1.5">
        <div className="size-2 shrink-0 rounded-full bg-primary/70 ring-2 ring-background" />
        {!isLast && <div className="mt-1 w-px flex-1 bg-border/60" />}
      </div>

      {/* Content */}
      <div className="flex-1 pb-6">
        <div className="flex items-start justify-between gap-2">
          <div className="min-w-0">
            <p className="text-sm font-medium leading-tight">
              {field.position}
            </p>
            <p className="mt-0.5 text-xs text-muted-foreground">
              {field.workplace}
            </p>
            {field.startDate && (
              <p className="mt-1 text-xs text-muted-foreground/60">
                {formatDateRange(field.startDate, field.endDate || null)}
              </p>
            )}
            {field.description && (
              <p className="mt-2 max-w-lg text-xs leading-relaxed text-muted-foreground/80">
                {field.description}
              </p>
            )}
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button
                type="button"
                className="flex size-7 shrink-0 items-center justify-center rounded-md text-muted-foreground/50 opacity-0 transition-all hover:bg-muted hover:text-foreground group-hover:opacity-100"
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
      </div>
    </div>
  );
}

export function EmploymentDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: EmploymentInput) => void;
}) {
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
    onAppend(data);
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Новое место работы</DialogTitle>
          <DialogDescription>
            Укажите информацию о вашем опыте работы
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="workplace"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Организация</FormLabel>
                  <FormControl>
                    <Input placeholder="ООО «Компания»" {...field} />
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
                  <FormLabel>Должность</FormLabel>
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
                    <FormLabel>Дата начала</FormLabel>
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
                    <FormLabel>
                      Дата окончания{" "}
                      <span className="font-normal text-muted-foreground">
                        (пусто = по н.в.)
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
                    <Textarea
                      placeholder="Опишите ваши обязанности и достижения…"
                      rows={3}
                      {...field}
                    />
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
