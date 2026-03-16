"use client";

import { TriangleAlert } from "lucide-react";

import type { OwnProfileDetails } from "@workspace/types/profile";

export function computeCompleteness(profile: {
  firstName?: string | null;
  lastName?: string | null;
  avatarUrl?: string | null;
  bio?: string | null;
  contacts?: unknown[];
  educations?: unknown[];
  employmentHistories?: unknown[];
}): number {
  const checks = [
    Boolean(profile.firstName),
    Boolean(profile.lastName),
    Boolean(profile.avatarUrl),
    Boolean(profile.bio),
    (profile.contacts?.length ?? 0) > 0,
    (profile.educations?.length ?? 0) > 0,
    (profile.employmentHistories?.length ?? 0) > 0,
  ];
  return Math.round((checks.filter(Boolean).length / checks.length) * 100);
}

export function CompletenessCard({ percent }: { percent: number }) {
  if (percent >= 60) return null;

  return (
    <div className="rounded-2xl border border-amber-200 bg-amber-50 p-4 dark:border-amber-800/40 dark:bg-amber-950/20">
      <div className="flex items-start gap-3">
        <div className="mt-0.5 flex size-7 shrink-0 items-center justify-center rounded-lg bg-amber-100 dark:bg-amber-900/40">
          <TriangleAlert className="size-3.5 text-amber-600 dark:text-amber-500" />
        </div>
        <div className="min-w-0 flex-1">
          <p className="text-xs font-semibold text-amber-800 dark:text-amber-400">
            Профиль заполнен на {percent}%
          </p>
          <p className="mt-1 text-[11px] leading-relaxed text-amber-700 dark:text-amber-500/80">
            Добавьте образование и опыт работы — ученики больше доверяют
            подробным профилям.
          </p>
          <div className="mt-2 h-1.5 overflow-hidden rounded-full bg-amber-200 dark:bg-amber-900/60">
            <div
              className="h-full rounded-full bg-amber-500 transition-all duration-300 dark:bg-amber-600"
              style={{ width: `${percent}%` }}
            />
          </div>
        </div>
      </div>
    </div>
  );
}

/** Alias for backward compatibility — same component as CompletenessCard. */
export const ProfileCompletenessCard = CompletenessCard;
