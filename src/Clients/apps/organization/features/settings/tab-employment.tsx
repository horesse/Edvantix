"use client";

import { useEffect, useState } from "react";

import { Briefcase, Loader2, Plus } from "lucide-react";
import { toast } from "sonner";

import useUpdateEmployment from "@workspace/api-hooks/profiles/useUpdateEmployment";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";

import { EmploymentCard, EmploymentDialog } from "./profile-employment";
import type { EmploymentInput } from "./profile-settings-schema";
import { EmptyState } from "./profile-settings-ui";

function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

export function TabEmployment({
  profile,
  onDirtyChange,
}: {
  profile: OwnProfileDetails;
  onDirtyChange?: (dirty: boolean) => void;
}) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [employments, setEmployments] = useState<EmploymentInput[]>(() =>
    profile.employmentHistories.map((e) => ({
      workplace: e.workplace,
      position: e.position,
      startDate: toDateString(e.startDate),
      endDate: toDateString(e.endDate),
      description: e.description ?? "",
    })),
  );
  const [savedSnapshot, setSavedSnapshot] = useState(employments);

  const mutation = useUpdateEmployment({
    onSuccess: (data) => {
      toast.success("Опыт работы сохранён");
      const server = data.employmentHistories.map((e) => ({
        workplace: e.workplace,
        position: e.position,
        startDate: toDateString(e.startDate),
        endDate: toDateString(e.endDate),
        description: e.description ?? "",
      }));
      setSavedSnapshot(server);
    },
    onError: () => toast.error("Не удалось сохранить опыт работы"),
  });

  function handleAppend(data: EmploymentInput) {
    setEmployments((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setEmployments((prev) => prev.filter((_, i) => i !== index));
  }

  function handleSubmit() {
    mutation.mutate({
      employmentHistories: employments.map((e) => ({
        workplace: e.workplace,
        position: e.position,
        startDate: e.startDate,
        endDate: e.endDate || null,
        description: e.description || null,
      })),
    });
  }

  const isDirty = JSON.stringify(employments) !== JSON.stringify(savedSnapshot);
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
          История вашей трудовой деятельности
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

      {employments.length === 0 ? (
        <EmptyState
          icon={<Briefcase className="size-5" />}
          text="Места работы не добавлены"
          onAdd={() => setDialogOpen(true)}
        />
      ) : (
        <div>
          {employments.map((emp, index) => (
            <EmploymentCard
              key={`${emp.workplace}-${emp.startDate}-${index}`}
              field={{ ...emp, id: String(index) }}
              onRemove={() => handleRemove(index)}
              isLast={index === employments.length - 1}
            />
          ))}
        </div>
      )}

      <EmploymentDialog
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
