"use client";

import { useEffect, useState } from "react";

import { GraduationCap, Loader2, Plus } from "lucide-react";
import { toast } from "sonner";

import useUpdateEducation from "@workspace/api-hooks/profiles/useUpdateEducation";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";

import { EducationCard, EducationDialog } from "./profile-education";
import type { EducationInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function TabEducation({
  profile,
  onDirtyChange,
}: {
  profile: OwnProfileDetails;
  onDirtyChange?: (dirty: boolean) => void;
}) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [educations, setEducations] = useState<EducationInput[]>(() =>
    profile.educations.map((e) => ({
      institution: e.institution,
      specialty: e.specialty ?? "",
      dateStart: toDateString(e.dateStart),
      dateEnd: toDateString(e.dateEnd),
      level: e.educationLevel,
    })),
  );
  const [savedSnapshot, setSavedSnapshot] = useState(educations);

  const mutation = useUpdateEducation({
    onSuccess: (data) => {
      toast.success("Образование сохранено");
      const server = data.educations.map((e) => ({
        institution: e.institution,
        specialty: e.specialty ?? "",
        dateStart: toDateString(e.dateStart),
        dateEnd: toDateString(e.dateEnd),
        level: e.educationLevel,
      }));
      setSavedSnapshot(server);
    },
    onError: () => toast.error("Не удалось сохранить образование"),
  });

  function handleAppend(data: EducationInput) {
    setEducations((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setEducations((prev) => prev.filter((_, i) => i !== index));
  }

  function handleSubmit() {
    mutation.mutate({
      educations: educations.map((e) => ({
        institution: e.institution,
        specialty: e.specialty || null,
        dateStart: e.dateStart,
        dateEnd: e.dateEnd || null,
        level: e.level,
      })),
    });
  }

  const isDirty = JSON.stringify(educations) !== JSON.stringify(savedSnapshot);
  const isPending = mutation.isPending;

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        handleSubmit();
      }}
      className="space-y-4"
    >
      <div className="flex items-center justify-between">
        <p className="text-muted-foreground text-sm">
          Учебные заведения и уровни образования
        </p>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setDialogOpen(true)}
          className="text-muted-foreground hover:text-foreground h-7 gap-1.5 px-2.5 text-xs"
        >
          <Plus className="size-3" />
          Добавить
        </Button>
      </div>

      {educations.length === 0 ? (
        <EmptyState
          icon={<GraduationCap className="size-5" />}
          text="Образование не добавлено"
          onAdd={() => setDialogOpen(true)}
        />
      ) : (
        <div>
          {educations.map((edu, index) => (
            <EducationCard
              key={`${edu.institution}-${edu.dateStart}-${index}`}
              field={{ ...edu, id: String(index) }}
              onRemove={() => handleRemove(index)}
              isLast={index === educations.length - 1}
            />
          ))}
        </div>
      )}

      <EducationDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onAppend={handleAppend}
      />

      <div className="border-border/40 flex items-center justify-between border-t pt-4">
        {isDirty && !isPending ? (
          <p className="text-muted-foreground text-xs">
            Есть несохранённые изменения
          </p>
        ) : (
          <span />
        )}
        <Button type="submit" disabled={isPending || !isDirty} size="sm">
          {isPending && <Loader2 className="size-3.5 animate-spin" />}
          Сохранить
        </Button>
      </div>
    </form>
  );
}
