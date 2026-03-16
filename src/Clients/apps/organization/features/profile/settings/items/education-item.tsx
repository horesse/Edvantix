"use client";

import { Trash2 } from "lucide-react";

import { EducationLevel } from "@workspace/types/profile";
import { formatDateRange } from "@workspace/utils/format";

import type { EducationInput } from "../schema";

export const educationLevelLabels: Record<EducationLevel, string> = {
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

export function EducationItem({
  education,
  onRemove,
  isLast,
}: {
  education: EducationInput;
  onRemove: () => void;
  isLast?: boolean;
}) {
  const levelLabel = education.level
    ? (educationLevelLabels[education.level as EducationLevel] ?? "Не указано")
    : "Не указано";
  const initial = education.institution?.[0]?.toUpperCase() ?? "?";

  return (
    <div
      className={`group border-border overflow-hidden rounded-xl border ${!isLast ? "mb-3" : ""}`}
    >
      <div className="border-border bg-muted/30 flex items-center justify-between border-b px-4 py-3">
        <div className="flex items-center gap-3">
          <div className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-teal-100 dark:bg-teal-900/30">
            <span className="text-xs font-bold text-teal-700 dark:text-teal-400">
              {initial}
            </span>
          </div>
          <div>
            <p className="text-foreground text-sm font-semibold">
              {education.institution}
            </p>
            <p className="text-muted-foreground text-xs">
              {levelLabel}
              {education.specialty && ` · ${education.specialty}`}
              {education.dateStart &&
                ` · ${formatDateRange(education.dateStart, education.dateEnd || null)}`}
            </p>
          </div>
        </div>
        <button
          type="button"
          onClick={onRemove}
          aria-label="Удалить"
          className="text-muted-foreground/50 hover:bg-destructive/10 hover:text-destructive flex size-7 items-center justify-center rounded-lg opacity-0 transition-all group-hover:opacity-100"
        >
          <Trash2 className="size-4" />
        </button>
      </div>
    </div>
  );
}
