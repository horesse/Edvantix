"use client";

import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import { Briefcase, Plus } from "lucide-react";

import type {
  EmploymentHistoryRequest,
  OwnProfileDetails,
} from "@workspace/types/profile";

import { EmploymentDialog } from "../dialogs/employment-dialog";
import { EmploymentItem } from "../items/employment-item";
import type { EmploymentInput } from "../schema";
import { toDateString } from "../schema";
import type { EmploymentSectionHandle } from "../types";

/** Empty state shown when no employment history has been added. */
function EmploymentEmptyState({ onAdd }: { onAdd: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center px-4 py-8">
      <div className="relative mb-5">
        <div className="flex size-20 items-center justify-center rounded-2xl border-2 border-dashed border-amber-200 bg-amber-50 dark:border-amber-800/50 dark:bg-amber-950/20">
          <Briefcase
            className="size-10 text-amber-300 dark:text-amber-600"
            strokeWidth={1.2}
          />
        </div>
        <div className="absolute -top-2 -right-2 flex size-6 items-center justify-center rounded-full bg-amber-400 shadow-sm dark:bg-amber-600">
          <Plus className="size-3 text-white" strokeWidth={2.5} />
        </div>
      </div>
      <p className="text-foreground mb-1 text-sm font-semibold">
        Опыт работы не указан
      </p>
      <p className="text-muted-foreground mb-4 max-w-xs text-center text-xs leading-relaxed">
        Расскажите о своём профессиональном пути — это повышает доверие учеников
        и помогает при поиске работы.
      </p>
      <button
        type="button"
        onClick={onAdd}
        className="inline-flex items-center gap-2 rounded-xl bg-amber-500 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-amber-600 dark:bg-amber-600 dark:hover:bg-amber-700"
      >
        <Plus className="size-4" strokeWidth={2.5} />
        Добавить место работы
      </button>
    </div>
  );
}

export const EmploymentSection = forwardRef<
  EmploymentSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function EmploymentSection({ profile, onDirtyChange }, ref) {
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

  function handleAppend(data: EmploymentInput) {
    setEmployments((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setEmployments((prev) => prev.filter((_, i) => i !== index));
  }

  const isDirty = JSON.stringify(employments) !== JSON.stringify(savedSnapshot);

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    getPayload(): EmploymentHistoryRequest[] {
      return employments.map((e) => ({
        workplace: e.workplace,
        position: e.position,
        startDate: e.startDate,
        endDate: e.endDate || null,
        description: e.description || null,
      }));
    },
    acknowledgeServerState() {
      setSavedSnapshot(employments);
    },
  }));

  return (
    <div className="space-y-3">
      <div className="flex justify-end">
        <button
          type="button"
          onClick={() => setDialogOpen(true)}
          className="border-primary/20 bg-primary/5 text-primary hover:border-primary/40 hover:bg-primary/10 flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-semibold transition-colors"
        >
          <Plus className="size-3.5" />
          Добавить
        </button>
      </div>

      {employments.length === 0 ? (
        <EmploymentEmptyState onAdd={() => setDialogOpen(true)} />
      ) : (
        <div>
          {employments.map((emp, index) => (
            <EmploymentItem
              key={`${emp.workplace}-${emp.startDate}-${index}`}
              employment={emp}
              onRemove={() => handleRemove(index)}
              isLast={index === employments.length - 1}
            />
          ))}
        </div>
      )}

      <EmploymentDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onAdd={handleAppend}
      />
    </div>
  );
});
