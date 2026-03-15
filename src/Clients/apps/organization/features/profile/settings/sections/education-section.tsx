"use client";

import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import { GraduationCap, Plus } from "lucide-react";

import type {
  EducationRequest,
  OwnProfileDetails,
} from "@workspace/types/profile";

import { EducationDialog } from "../dialogs/education-dialog";
import { EducationItem } from "../items/education-item";
import type { EducationInput } from "../schema";
import { toDateString } from "../schema";
import type { EducationSectionHandle } from "../types";

/** Empty state shown when no education entries have been added. */
function EducationEmptyState({ onAdd }: { onAdd: () => void }) {
  return (
    <div className="flex flex-col items-center rounded-xl border-2 border-dashed border-teal-200 bg-teal-50/40 p-8 text-center dark:border-teal-800/40 dark:bg-teal-950/10">
      {/* Illustration: graduation cap + book stacks */}
      <div className="relative mb-5 select-none">
        <div
          className="relative z-10 flex flex-col items-center"
          style={{ marginBottom: 48 }}
        >
          <GraduationCap
            className="size-14 text-teal-400 dark:text-teal-600"
            strokeWidth={1.2}
          />
        </div>
        {/* Book 3 (bottom) */}
        <div className="absolute bottom-0 left-1/2 h-4 w-24 -translate-x-1/2 rounded-lg bg-teal-300 shadow-sm dark:bg-teal-800" />
        {/* Book 2 (middle) */}
        <div className="absolute bottom-3.5 left-1/2 h-4 w-20 -translate-x-1/2 rounded-lg bg-teal-400 shadow-sm dark:bg-teal-700" />
        {/* Book 1 (top) */}
        <div className="absolute bottom-7 left-1/2 h-4 w-16 -translate-x-1/2 rounded-lg bg-teal-500 shadow-sm dark:bg-teal-600" />
      </div>

      <p className="mb-1.5 text-sm font-semibold text-teal-800 dark:text-teal-300">
        Образование не добавлено
      </p>
      <p className="mb-5 max-w-xs text-xs leading-relaxed text-teal-700/70 dark:text-teal-500">
        Укажите учебные заведения, специальности и уровень образования — это
        важно для подтверждения квалификации.
      </p>

      <button
        type="button"
        onClick={onAdd}
        className="mb-3 inline-flex items-center gap-2 rounded-xl bg-teal-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-teal-700 dark:bg-teal-700 dark:hover:bg-teal-600"
      >
        <Plus className="size-4" strokeWidth={2.5} />
        Добавить образование
      </button>

      {/* Hint chips */}
      <div className="mt-1 flex flex-wrap justify-center gap-1.5">
        {[
          "Высшее образование",
          "Среднее профессиональное",
          "Курсы и сертификаты",
        ].map((chip) => (
          <span
            key={chip}
            className="rounded-full border border-teal-200 bg-teal-100 px-2.5 py-1 text-[11px] text-teal-600 dark:border-teal-800/50 dark:bg-teal-900/30 dark:text-teal-500"
          >
            {chip}
          </span>
        ))}
      </div>
    </div>
  );
}

export const EducationSection = forwardRef<
  EducationSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function EducationSection({ profile, onDirtyChange }, ref) {
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

  function handleAppend(data: EducationInput) {
    setEducations((prev) => [...prev, data]);
  }

  function handleRemove(index: number) {
    setEducations((prev) => prev.filter((_, i) => i !== index));
  }

  const isDirty = JSON.stringify(educations) !== JSON.stringify(savedSnapshot);

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    getPayload(): EducationRequest[] {
      return educations.map((e) => ({
        institution: e.institution,
        specialty: e.specialty || null,
        dateStart: e.dateStart,
        dateEnd: e.dateEnd || null,
        level: e.level,
      }));
    },
    acknowledgeServerState() {
      setSavedSnapshot(educations);
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

      {educations.length === 0 ? (
        <EducationEmptyState onAdd={() => setDialogOpen(true)} />
      ) : (
        <div>
          {educations.map((edu, index) => (
            <EducationItem
              key={`${edu.institution}-${edu.dateStart}-${index}`}
              education={edu}
              onRemove={() => handleRemove(index)}
              isLast={index === educations.length - 1}
            />
          ))}
        </div>
      )}

      <EducationDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onAdd={handleAppend}
      />
    </div>
  );
});
