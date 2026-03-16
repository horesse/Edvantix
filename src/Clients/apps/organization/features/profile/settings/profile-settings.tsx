"use client";

import { useCallback, useRef, useState } from "react";

import Link from "next/link";
import { useRouter } from "next/navigation";

import {
  Briefcase,
  Check,
  ChevronLeft,
  ChevronRight,
  Clock,
  GraduationCap,
  Mail,
  Pencil,
  User,
} from "lucide-react";
import { toast } from "sonner";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { AvatarCard } from "./cards/avatar-card";
import {
  CompletenessCard,
  computeCompleteness,
} from "./cards/completeness-card";
import { SectionCard } from "./cards/section-card";
import { SubjectsCard } from "./cards/subjects-card";
import { BioSection } from "./sections/bio-section";
import { ContactsSection } from "./sections/contacts-section";
import { EducationSection } from "./sections/education-section";
import { EmploymentSection } from "./sections/employment-section";
import { GeneralSection } from "./sections/general-section";
import type {
  BioSectionHandle,
  ContactsSectionHandle,
  EducationSectionHandle,
  EmploymentSectionHandle,
  GeneralSectionHandle,
  SkillsSectionHandle,
} from "./types";

// ── Skeleton ──────────────────────────────────────────────────────────────────

function ProfileSettingsSkeleton() {
  return (
    <div className="flex flex-col gap-5 lg:flex-row lg:items-start">
      <div className="flex w-full shrink-0 flex-col gap-4 lg:w-72">
        <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
          <Skeleton className="mx-auto size-28 rounded-2xl" />
          <Skeleton className="mx-auto mt-4 h-24 w-full rounded-xl" />
        </div>
        <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
          <div className="space-y-2">
            {[1, 2, 3].map((i) => (
              <Skeleton key={i} className="h-8 w-full rounded-full" />
            ))}
          </div>
        </div>
      </div>
      <div className="min-w-0 flex-1 space-y-4">
        {[1, 2, 3].map((i) => (
          <div
            key={i}
            className="bg-card border-border rounded-2xl border p-6 shadow-sm"
          >
            <div className="mb-5 flex items-center gap-2.5">
              <Skeleton className="size-8 rounded-xl" />
              <div className="space-y-1.5">
                <Skeleton className="h-3.5 w-32" />
                <Skeleton className="h-3 w-48" />
              </div>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              {[1, 2, 3, 4].map((j) => (
                <div key={j} className="space-y-2">
                  <Skeleton className="h-3 w-20" />
                  <Skeleton className="h-10 w-full rounded-xl" />
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

// ── Main component ────────────────────────────────────────────────────────────

export function ProfileSettings() {
  const { data: profile, isLoading } = useProfileDetails();
  const router = useRouter();

  const generalRef = useRef<GeneralSectionHandle>(null);
  const contactsRef = useRef<ContactsSectionHandle>(null);
  const educationRef = useRef<EducationSectionHandle>(null);
  const employmentRef = useRef<EmploymentSectionHandle>(null);
  const bioRef = useRef<BioSectionHandle>(null);
  const skillsRef = useRef<SkillsSectionHandle>(null);

  const [dirtyMap, setDirtyMap] = useState({
    general: false,
    contacts: false,
    education: false,
    employment: false,
    bio: false,
    skills: false,
  });

  const hasUnsaved = Object.values(dirtyMap).some(Boolean);

  // Stable per-key handlers — each created once, no new references on re-render.
  const onGeneralDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, general: d })),
    [],
  );
  const onContactsDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, contacts: d })),
    [],
  );
  const onEducationDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, education: d })),
    [],
  );
  const onEmploymentDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, employment: d })),
    [],
  );
  const onBioDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, bio: d })),
    [],
  );
  const onSkillsDirty = useCallback(
    (d: boolean) => setDirtyMap((p) => ({ ...p, skills: d })),
    [],
  );

  const mutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Профиль сохранён");
      // Let all sections know the current state is now "saved"
      generalRef.current?.acknowledgeServerState();
      contactsRef.current?.acknowledgeServerState();
      educationRef.current?.acknowledgeServerState();
      employmentRef.current?.acknowledgeServerState();
      bioRef.current?.acknowledgeServerState();
      skillsRef.current?.acknowledgeServerState();
    },
    onError: () => toast.error("Не удалось сохранить профиль"),
  });

  async function handleSaveAll() {
    const generalData = await generalRef.current?.getPayload();
    if (!generalData) {
      // Validation failed — form will show errors
      return;
    }

    mutation.mutate({
      firstName: generalData.firstName,
      lastName: generalData.lastName,
      middleName: generalData.middleName,
      birthDate: generalData.birthDate,
      bio: bioRef.current?.getPayload() ?? null,
      contacts: contactsRef.current?.getPayload() ?? [],
      employmentHistories: employmentRef.current?.getPayload() ?? [],
      educations: educationRef.current?.getPayload() ?? [],
      skills: skillsRef.current?.getPayload() ?? [],
    });
  }

  const fullName = profile
    ? [profile.firstName, profile.lastName].filter(Boolean).join(" ")
    : null;

  // ── Loading skeleton ────────────────────────────────────────────────────────
  if (isLoading || !profile) {
    return (
      <div>
        {/* Skeleton sticky header */}
        <div className="border-border bg-card -mx-4 -mt-4 mb-6 flex items-center gap-2 border-b px-6 py-3 lg:-mx-6 lg:-mt-6">
          <Skeleton className="h-4 w-20" />
          <Skeleton className="mx-1 size-3" />
          <Skeleton className="h-4 w-32" />
          <div className="ml-auto flex gap-2">
            <Skeleton className="h-9 w-20 rounded-lg" />
            <Skeleton className="h-9 w-28 rounded-lg" />
          </div>
        </div>
        <div className="mx-auto max-w-6xl">
          <ProfileSettingsSkeleton />
        </div>
      </div>
    );
  }

  const completeness = computeCompleteness(profile);

  return (
    <div>
      {/* ── Sticky header ──────────────────────────────────────────────────────
          -mt cancels the inner wrapper's pt so header sits at main's outer top.
          -mx cancels the wrapper's px for full-bleed width.
          sticky top-0 is relative to main (scroll container, no padding). ── */}
      <div className="border-border bg-card sticky top-0 z-10 -mx-4 -mt-4 mb-5 flex items-center gap-2 border-b px-6 py-3 shadow-sm lg:-mx-6 lg:-mt-6">
        {/* Breadcrumb */}
        <button
          type="button"
          onClick={() => router.back()}
          className="text-muted-foreground hover:text-foreground flex items-center gap-1 text-sm transition-colors"
        >
          <ChevronLeft className="size-4" />
          Назад
        </button>

        <ChevronRight className="text-muted-foreground/30 size-3.5" />
        <Link
          href="/settings/profile"
          className="text-muted-foreground hover:text-foreground text-sm transition-colors"
        >
          {fullName}
        </Link>
        <ChevronRight className="text-muted-foreground/30 size-3.5" />
        <span className="text-foreground text-sm font-semibold">
          Редактирование
        </span>

        {hasUnsaved && (
          <span className="ml-2 inline-flex items-center gap-1.5 rounded-full border border-amber-200 bg-amber-50 px-2.5 py-1 text-[11px] font-medium text-amber-700 dark:border-amber-800/50 dark:bg-amber-950/30 dark:text-amber-400">
            <span className="size-1.5 rounded-full bg-amber-500 dark:bg-amber-400" />
            Несохранённые изменения
          </span>
        )}

        <div className="ml-auto flex gap-2">
          <button
            type="button"
            onClick={() => router.back()}
            className="border-border text-muted-foreground hover:bg-muted rounded-lg border px-4 py-2 text-sm font-medium transition-colors"
          >
            Отмена
          </button>
          <button
            type="button"
            onClick={() => void handleSaveAll()}
            disabled={!hasUnsaved || mutation.isPending}
            className="bg-primary text-primary-foreground hover:bg-primary/90 flex items-center gap-1.5 rounded-lg px-4 py-2 text-sm font-semibold shadow-sm transition-colors disabled:opacity-40"
          >
            <Check className="size-4" />
            Сохранить
          </button>
        </div>
      </div>

      {/* ── Centered content grid ─────────────────────────────────────────── */}
      <div className="mx-auto max-w-6xl">
        <div className="flex flex-col gap-5 lg:flex-row lg:items-start">
          {/* ── Left panel ── */}
          <div className="flex w-full shrink-0 flex-col gap-4 lg:w-72">
            <AvatarCard profile={profile} />
            <SubjectsCard
              ref={skillsRef}
              profile={profile}
              onDirtyChange={onSkillsDirty}
            />
            <CompletenessCard percent={completeness} />
          </div>

          {/* ── Right: stacked section cards ── */}
          <div className="flex min-w-0 flex-1 flex-col gap-4">
            <SectionCard
              icon={User}
              iconBg="bg-brand-100 dark:bg-brand-900/20"
              iconColor="text-brand-600 dark:text-brand-400"
              title="Личная информация"
              subtitle="ФИО, дата рождения, пол"
            >
              <GeneralSection
                ref={generalRef}
                profile={profile}
                onDirtyChange={onGeneralDirty}
              />
            </SectionCard>

            <SectionCard
              icon={Mail}
              iconBg="bg-blue-100 dark:bg-blue-900/30"
              iconColor="text-blue-600 dark:text-blue-400"
              title="Контактные данные"
              subtitle={
                profile.contacts.length > 0
                  ? `${profile.contacts.length} ${profile.contacts.length === 1 ? "запись" : "записей"}`
                  : "Нет записей"
              }
            >
              <ContactsSection
                ref={contactsRef}
                profile={profile}
                onDirtyChange={onContactsDirty}
              />
            </SectionCard>

            <SectionCard
              icon={Briefcase}
              iconBg="bg-amber-100 dark:bg-amber-900/30"
              iconColor="text-amber-600 dark:text-amber-400"
              title="Опыт работы"
              subtitle={
                profile.employmentHistories.length > 0
                  ? `${profile.employmentHistories.length} ${profile.employmentHistories.length === 1 ? "запись" : "записей"}`
                  : "Нет записей"
              }
            >
              <EmploymentSection
                ref={employmentRef}
                profile={profile}
                onDirtyChange={onEmploymentDirty}
              />
            </SectionCard>

            <SectionCard
              icon={GraduationCap}
              iconBg="bg-teal-100 dark:bg-teal-900/30"
              iconColor="text-teal-600 dark:text-teal-400"
              title="Образование"
              subtitle={
                profile.educations.length > 0
                  ? `${profile.educations.length} ${profile.educations.length === 1 ? "запись" : "записей"}`
                  : "Нет записей"
              }
            >
              <EducationSection
                ref={educationRef}
                profile={profile}
                onDirtyChange={onEducationDirty}
              />
            </SectionCard>

            <SectionCard
              icon={Pencil}
              iconBg="bg-emerald-100 dark:bg-emerald-900/30"
              iconColor="text-emerald-600 dark:text-emerald-400"
              title="О себе"
              subtitle="Видно ученикам и коллегам"
            >
              <BioSection
                ref={bioRef}
                profile={profile}
                onDirtyChange={onBioDirty}
              />
            </SectionCard>

            {/* ── Bottom bar ── */}
            <div className="border-border bg-card flex items-center justify-between rounded-2xl border px-5 py-4 shadow-sm">
              <p className="text-muted-foreground flex items-center gap-1.5 text-xs">
                <Clock className="size-3.5" />
                Изменения применяются ко всему профилю
              </p>
              <div className="flex gap-2">
                <button
                  type="button"
                  onClick={() => router.back()}
                  className="border-border text-muted-foreground hover:bg-muted rounded-xl border px-5 py-2.5 text-sm font-medium transition-colors"
                >
                  Отмена
                </button>
                <button
                  type="button"
                  onClick={() => void handleSaveAll()}
                  disabled={!hasUnsaved || mutation.isPending}
                  className="bg-primary text-primary-foreground hover:bg-primary/90 flex items-center gap-2 rounded-xl px-5 py-2.5 text-sm font-semibold shadow-sm transition-colors disabled:opacity-40"
                >
                  <Check className="size-4" />
                  Сохранить изменения
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
