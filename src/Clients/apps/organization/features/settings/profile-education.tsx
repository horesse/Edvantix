"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";

import { EducationLevel } from "@workspace/types/profile";
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
import { formatDateRange } from "@workspace/utils/format";
import { educationSchema } from "@workspace/validations/profile";

import type { EducationInput } from "./profile-settings-schema";

const educationLevelLabels: Record<EducationLevel, string> = {
  [EducationLevel.Preschool]: "Дошкольное",
  [EducationLevel.GeneralSecondary]: "Общее среднее",
  [EducationLevel.VocationalTechnical]: "Профессионально-техническое",
  [EducationLevel.SecondarySpecialized]: "Среднее специальное",
  [EducationLevel.HigherBachelor]: "Высшее (I ступень)",
  [EducationLevel.HigherMaster]: "Высшее (II ступень)",
  [EducationLevel.Postgraduate]: "Послевузовское",
  [EducationLevel.AdditionalChildren]: "Доп. образование детей",
  [EducationLevel.AdditionalAdults]: "Доп. образование взрослых",
  [EducationLevel.Special]: "Специальное",
};

export function EducationCard({
  field,
  onRemove,
}: {
  field: EducationInput & { id: string };
  onRemove: () => void;
}) {
  const levelLabel = field.level
    ? (educationLevelLabels[field.level as EducationLevel] ?? "Не указано")
    : "Не указано";

  return (
    <div className="group relative rounded-lg border border-border/40 p-3.5 transition-colors hover:bg-muted/20">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="text-sm font-medium">{field.institution}</p>
          {field.specialty && (
            <p className="mt-0.5 text-xs text-muted-foreground">
              {field.specialty}
            </p>
          )}
          <div className="mt-1.5 flex flex-wrap items-center gap-2">
            <Badge
              variant="secondary"
              className="rounded-sm px-1.5 py-0 text-[10px] font-normal"
            >
              {levelLabel}
            </Badge>
            {field.dateStart && (
              <span className="text-xs text-muted-foreground/60">
                {formatDateRange(field.dateStart, field.dateEnd || null)}
              </span>
            )}
          </div>
        </div>
        <button
          type="button"
          onClick={onRemove}
          className="shrink-0 text-transparent transition-colors group-hover:text-muted-foreground/40 hover:!text-destructive"
          aria-label="Удалить образование"
        >
          <Trash2 className="size-3.5" />
        </button>
      </div>
    </div>
  );
}

export function EducationDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: EducationInput) => void;
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
    onAppend(data);
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Новое образование</DialogTitle>
          <DialogDescription>
            Укажите информацию о вашем образовании
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="institution"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Учебное заведение</FormLabel>
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
                    <span className="font-normal text-muted-foreground">
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
                  <FormLabel>Уровень образования</FormLabel>
                  <Select
                    onValueChange={(v) => field.onChange(Number(v))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Выберите уровень" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(educationLevelLabels).map(
                        ([key, label]) => (
                          <SelectItem key={key} value={key}>
                            {label}
                          </SelectItem>
                        ),
                      )}
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
                name="dateEnd"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Дата окончания{" "}
                      <span className="font-normal text-muted-foreground">
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
