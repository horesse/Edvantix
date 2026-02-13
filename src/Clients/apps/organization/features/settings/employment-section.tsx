"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Briefcase, Loader2, Plus, Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useUpdateEmployment from "@workspace/api-hooks/profiles/useUpdateEmployment";
import type {
  EmploymentHistory,
  OwnProfileDetails,
} from "@workspace/types/profile";
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
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type EmploymentInput,
  employmentSchema,
} from "@workspace/validations/profile";

function formatDateRange(start: string, end?: string | null): string {
  const options: Intl.DateTimeFormatOptions = {
    month: "short",
    year: "numeric",
  };
  const startStr = new Date(start).toLocaleDateString("ru-RU", options);
  const endStr = end
    ? new Date(end).toLocaleDateString("ru-RU", options)
    : "настоящее время";
  return `${startStr} — ${endStr}`;
}

type EmploymentSectionProps = {
  profile: OwnProfileDetails;
};

export function EmploymentSection({ profile }: EmploymentSectionProps) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const items = profile.employmentHistories ?? [];

  const updateMutation = useUpdateEmployment({
    onSuccess: () => {
      toast.success("Опыт работы обновлён");
    },
    onError: () => {
      toast.error("Не удалось обновить опыт работы");
    },
  });

  function handleRemove(index: number) {
    const updated = items.filter((_, i) => i !== index);
    updateMutation.mutate(updated);
  }

  function handleAdd(data: EmploymentInput) {
    const updated = [
      ...items,
      {
        companyName: data.companyName,
        position: data.position,
        startDate: data.startDate,
        endDate: data.endDate || null,
        description: data.description || null,
      },
    ];
    updateMutation.mutate(updated, {
      onSuccess: () => {
        setDialogOpen(false);
        toast.success("Место работы добавлено");
      },
    });
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="font-medium">Опыт работы</h3>
          <p className="text-muted-foreground text-sm">
            Ваша трудовая деятельность
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={() => setDialogOpen(true)}>
          <Plus className="size-4" />
          <span className="hidden sm:inline">Добавить</span>
        </Button>
      </div>

      {items.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-8">
          <Briefcase className="text-muted-foreground/50 mb-3 size-10" />
          <p className="text-muted-foreground text-sm">
            Места работы не добавлены
          </p>
          <Button
            variant="link"
            size="sm"
            className="mt-1"
            onClick={() => setDialogOpen(true)}
          >
            Добавить первое место работы
          </Button>
        </div>
      ) : (
        <div className="space-y-3">
          {items.map((item, index) => (
            <EmploymentCard
              key={`${item.companyName}-${item.startDate}`}
              item={item}
              onRemove={() => handleRemove(index)}
              isLoading={updateMutation.isPending}
            />
          ))}
        </div>
      )}

      <AddEmploymentDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={handleAdd}
        isLoading={updateMutation.isPending}
      />
    </div>
  );
}

function EmploymentCard({
  item,
  onRemove,
  isLoading,
}: {
  item: EmploymentHistory;
  onRemove: () => void;
  isLoading: boolean;
}) {
  return (
    <div className="group hover:bg-muted/30 relative rounded-lg border p-4 transition-colors">
      <div className="flex gap-3">
        <div className="bg-primary/10 text-primary mt-0.5 flex size-10 shrink-0 items-center justify-center rounded-lg">
          <Briefcase className="size-5" />
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex items-start justify-between gap-2">
            <div className="min-w-0">
              <p className="truncate font-medium">{item.position}</p>
              <p className="text-muted-foreground truncate text-sm">
                {item.companyName}
              </p>
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
          <p className="text-muted-foreground mt-1 text-xs">
            {formatDateRange(item.startDate, item.endDate)}
          </p>
          {item.description && (
            <p className="text-muted-foreground mt-2 text-sm leading-relaxed">
              {item.description}
            </p>
          )}
        </div>
      </div>
    </div>
  );
}

function AddEmploymentDialog({
  open,
  onOpenChange,
  onSubmit,
  isLoading,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: EmploymentInput) => void;
  isLoading: boolean;
}) {
  const form = useForm<EmploymentInput>({
    resolver: zodResolver(employmentSchema),
    defaultValues: {
      companyName: "",
      position: "",
      startDate: "",
      endDate: "",
      description: "",
    },
  });

  function handleSubmit(data: EmploymentInput) {
    onSubmit(data);
    form.reset();
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
              name="companyName"
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
                      <span className="text-muted-foreground font-normal">
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
                    <span className="text-muted-foreground font-normal">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Опишите ваши обязанности и достижения..."
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
