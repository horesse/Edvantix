"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { GraduationCap, Loader2, Plus, Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
import type {
  Education,
  EducationRequest,
  OwnProfileDetails,
} from "@workspace/types/profile";
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
import {
  type EducationInput,
  educationSchema,
} from "@workspace/validations/profile";

import { buildProfileUpdateRequest } from "@/lib/profile-update";

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

type EducationSectionProps = {
  profile: OwnProfileDetails;
};

function toEducationRequest(education: Education): EducationRequest {
  return {
    institution: education.institution,
    specialty: education.specialty ?? null,
    dateStart: education.dateStart,
    dateEnd: education.dateEnd ?? null,
    level: education.educationLevel,
  };
}

export function EducationSection({ profile }: EducationSectionProps) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const items = profile.educations;

  const updateMutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Образование обновлено");
    },
    onError: () => {
      toast.error("Не удалось обновить образование");
    },
  });

  function handleRemove(index: number) {
    const updated = items
      .filter((_, i) => i !== index)
      .map(toEducationRequest);
    updateMutation.mutate(
      buildProfileUpdateRequest(profile, { educations: updated }),
    );
  }

  function handleAdd(data: EducationInput) {
    const updated = [
      ...items.map(toEducationRequest),
      {
        institution: data.institution,
        specialty: data.specialty || null,
        dateStart: data.dateStart,
        dateEnd: data.dateEnd || null,
        level: data.level,
      },
    ];
    updateMutation.mutate(
      buildProfileUpdateRequest(profile, { educations: updated }),
      {
        onSuccess: () => {
          setDialogOpen(false);
          toast.success("Образование добавлено");
        },
      },
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="font-medium">Образование</h3>
          <p className="text-muted-foreground text-sm">
            Ваше образование и квалификации
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={() => setDialogOpen(true)}>
          <Plus className="size-4" />
          <span className="hidden sm:inline">Добавить</span>
        </Button>
      </div>

      {items.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-8">
          <GraduationCap className="text-muted-foreground/50 mb-3 size-10" />
          <p className="text-muted-foreground text-sm">
            Образование не добавлено
          </p>
          <Button
            variant="link"
            size="sm"
            className="mt-1"
            onClick={() => setDialogOpen(true)}
          >
            Добавить образование
          </Button>
        </div>
      ) : (
        <div className="space-y-3">
          {items.map((item, index) => (
            <EducationCard
              key={`${item.institution}-${item.dateStart}`}
              item={item}
              onRemove={() => handleRemove(index)}
              isLoading={updateMutation.isPending}
            />
          ))}
        </div>
      )}

      <AddEducationDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={handleAdd}
        isLoading={updateMutation.isPending}
      />
    </div>
  );
}

function EducationCard({
  item,
  onRemove,
  isLoading,
}: {
  item: Education;
  onRemove: () => void;
  isLoading: boolean;
}) {
  const levelLabel = educationLevelLabels[item.educationLevel] ?? "Не указано";

  return (
    <div className="group hover:bg-muted/30 relative rounded-lg border p-4 transition-colors">
      <div className="flex gap-3">
        <div className="bg-primary/10 text-primary mt-0.5 flex size-10 shrink-0 items-center justify-center rounded-lg">
          <GraduationCap className="size-5" />
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex items-start justify-between gap-2">
            <div className="min-w-0">
              <p className="truncate font-medium">{item.institution}</p>
              {item.specialty && (
                <p className="text-muted-foreground truncate text-sm">
                  {item.specialty}
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
          <div className="mt-2 flex flex-wrap items-center gap-2">
            <Badge variant="secondary" className="text-[10px]">
              {levelLabel}
            </Badge>
            <span className="text-muted-foreground text-xs">
              {formatDateRange(item.dateStart, item.dateEnd)}
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}

function AddEducationDialog({
  open,
  onOpenChange,
  onSubmit,
  isLoading,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: EducationInput) => void;
  isLoading: boolean;
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
    onSubmit(data);
    form.reset();
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
