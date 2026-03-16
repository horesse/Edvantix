"use client";

import { useState } from "react";

import { useRouter } from "next/navigation";

import { cn } from "@workspace/ui/lib/utils";

import { SidebarLogo } from "@/components/sidebar/sidebar-logo";

import { AVATAR_OPTIONS } from "./schema";
import type {
  AvatarStepValues,
  PersonalStepValues,
  ProfileSetupValues,
} from "./schema";
import { StepAvatar } from "./step-avatar";
import { StepPersonal } from "./step-personal";

// ── Types ────────────────────────────────────────────────────────────────────

type Step = 1 | 2;

// ── Step indicator ───────────────────────────────────────────────────────────

interface StepIndicatorProps {
  current: Step;
  steps: { label: string }[];
}

function StepIndicator({ current, steps }: StepIndicatorProps) {
  return (
    <div className="space-y-1.5">
      <div className="flex items-center justify-center gap-1.5">
        {steps.map((_, idx) => {
          const stepNum = (idx + 1) as Step;
          const isDone = stepNum < current;
          const isActive = stepNum === current;
          return (
            <div
              key={idx}
              className={cn(
                "h-2 rounded-full transition-all duration-300",
                isDone
                  ? "w-2 bg-green-500"
                  : isActive
                    ? "bg-primary w-6"
                    : "bg-muted-foreground/30 w-2",
              )}
            />
          );
        })}
      </div>
      <div className="flex justify-center gap-8">
        {steps.map((step, idx) => {
          const stepNum = (idx + 1) as Step;
          return (
            <span
              key={idx}
              className={cn(
                "text-xs",
                stepNum === current
                  ? "text-primary font-medium"
                  : "text-muted-foreground",
              )}
            >
              {step.label}
            </span>
          );
        })}
      </div>
    </div>
  );
}

// ── Default values ────────────────────────────────────────────────────────────

const defaultAvatarValues: AvatarStepValues = {
  avatarType: "preset",
  presetValue: AVATAR_OPTIONS[0].value,
};

const defaultPersonalValues: PersonalStepValues = {
  lastName: "",
  firstName: "",
  patronymic: "",
  birthDate: "",
  gender: "male",
  countryCode: "+375",
  phone: "",
};

const STEPS = [{ label: "Аватар" }, { label: "Данные" }];

// ── Page ──────────────────────────────────────────────────────────────────────

interface ProfileSetupPageProps {
  /**
   * Called when the user completes step 2 and clicks "Сохранить и войти".
   * Receives the full form values including avatar data and personal info.
   * `avatarFile` is the raw File selected by the user, or null if a preset was chosen.
   * Should return a Promise that resolves on success or rejects with an error message.
   */
  onSubmit?: (
    values: ProfileSetupValues,
    avatarFile: File | null,
  ) => Promise<void>;
}

/**
 * Multi-step profile setup wizard.
 *
 * Step 1: choose avatar (preset emoji or uploaded photo).
 * Step 2: personal data (name, birth date, gender, phone).
 *
 * Sits outside the (main) layout — no sidebar, minimal header.
 * Pass an `onSubmit` handler to wire in the real API call from the page layer.
 */
export function ProfileSetupPage({ onSubmit }: ProfileSetupPageProps) {
  const router = useRouter();
  const [step, setStep] = useState<Step>(1);
  const [avatarValues, setAvatarValues] =
    useState<AvatarStepValues>(defaultAvatarValues);
  const [uploadedDataUrl, setUploadedDataUrl] = useState<string | null>(null);
  const [uploadedFile, setUploadedFile] = useState<File | null>(null);
  const [personalValues, setPersonalValues] = useState<PersonalStepValues>(
    defaultPersonalValues,
  );
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit() {
    setError(null);

    if (!personalValues.lastName.trim()) {
      setError("Пожалуйста, введите фамилию.");
      return;
    }

    if (!personalValues.firstName.trim()) {
      setError("Пожалуйста, введите имя.");
      return;
    }

    if (!personalValues.birthDate) {
      setError("Пожалуйста, укажите дату рождения.");
      return;
    }

    setIsSubmitting(true);

    try {
      const payload: ProfileSetupValues = {
        ...avatarValues,
        ...personalValues,
      };

      if (onSubmit) {
        await onSubmit(payload, uploadedFile);
      } else {
        // Default: just navigate to dashboard (useful for Storybook / tests)
        router.push("/");
      }
    } catch (err) {
      const message =
        err instanceof Error
          ? err.message
          : "Не удалось сохранить профиль. Попробуйте ещё раз.";
      setError(message);
      console.error("[profile-setup] submit error:", err);
    } finally {
      setIsSubmitting(false);
    }
  }

  function handleSkip() {
    router.push("/");
  }

  return (
    <div className="bg-muted/30 min-h-screen">
      {/* Minimal header */}
      <header className="bg-background/90 border-border fixed inset-x-0 top-0 z-20 border-b backdrop-blur-sm">
        <div className="mx-auto flex h-14 max-w-5xl items-center justify-between px-6">
          <SidebarLogo />
          <span className="text-muted-foreground text-xs">
            Настройка профиля
          </span>
        </div>
      </header>

      {/* Content */}
      <main className="flex min-h-screen items-center justify-center px-4 pt-24 pb-10">
        <div className="w-full max-w-[520px]">
          {/* Heading + progress */}
          <div className="mb-8 space-y-5 text-center">
            <div className="bg-primary/5 border-primary/20 mx-auto flex size-14 items-center justify-center rounded-2xl border">
              <svg width="28" height="28" fill="none" viewBox="0 0 24 24">
                <path
                  d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                  stroke="hsl(var(--primary))"
                  strokeWidth="1.8"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </div>
            <div>
              <h1 className="text-foreground text-2xl font-bold tracking-tight">
                Настройте свой профиль
              </h1>
              <p className="text-muted-foreground mt-1.5 text-sm">
                Это займёт меньше минуты
              </p>
            </div>
            <StepIndicator current={step} steps={STEPS} />
          </div>

          {/* Card */}
          <div className="bg-card border-border overflow-hidden rounded-2xl border shadow-sm">
            {step === 1 && (
              <StepAvatar
                values={avatarValues}
                uploadedDataUrl={uploadedDataUrl}
                onChange={setAvatarValues}
                onUploadChange={(file, dataUrl) => {
                  setUploadedFile(file);
                  setUploadedDataUrl(dataUrl);
                }}
                onNext={() => setStep(2)}
              />
            )}
            {step === 2 && (
              <StepPersonal
                values={personalValues}
                onChange={setPersonalValues}
                onBack={() => setStep(1)}
                onSubmit={handleSubmit}
                isSubmitting={isSubmitting}
                error={error}
              />
            )}
          </div>

          {/* Skip link */}
          <p className="mt-4 text-center text-sm">
            <button
              type="button"
              onClick={handleSkip}
              className="text-muted-foreground hover:text-foreground transition-colors"
            >
              Пропустить пока →
            </button>
          </p>
        </div>
      </main>
    </div>
  );
}
