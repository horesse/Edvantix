"use client";

import { useEffect, useRef, useState } from "react";

import { Check, Clock } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

// ── Stage types ───────────────────────────────────────────────────────────────

export type LoadingStage = "auth" | "session" | "profile";

type StepId = 1 | 2 | 3;

interface StepDef {
  id: StepId;
  label: string;
  activeSubtitle: string;
  doneSubtitle: string;
}

interface StageConfig {
  /** Text shown under the main heading, changes per stage. */
  subtitle: string;
  /** Progress bar starts at this value when entering the stage. */
  progressFrom: number;
  /** Progress bar animates toward this value while in the stage. */
  progressTo: number;
  /** Which step card is currently spinning. */
  activeStep: StepId;
  /** Steps rendered as "done" (green checkmark). */
  doneSteps: Array<StepId>;
}

// ── Config ────────────────────────────────────────────────────────────────────

/**
 * Three visual steps that match the auth flow:
 *   1. Credential check  (AuthGuard isLoading)
 *   2. Session setup     (token fetch after auth)
 *   3. Profile load      (ProfileGuard isLoading)
 */
const STEPS: StepDef[] = [
  {
    id: 1,
    label: "Проверка учётных данных",
    activeSubtitle: "Отправляем запрос на сервер…",
    doneSubtitle: "Данные подтверждены",
  },
  {
    id: 2,
    label: "Настройка сессии",
    activeSubtitle: "Создаём защищённый токен…",
    doneSubtitle: "Сессия установлена",
  },
  {
    id: 3,
    label: "Загрузка профиля",
    activeSubtitle: "Получаем данные пользователя…",
    doneSubtitle: "Профиль загружен",
  },
];

const STAGE_CONFIG: Record<LoadingStage, StageConfig> = {
  auth: {
    subtitle: "Проверяем ваши данные…",
    progressFrom: 5,
    progressTo: 30,
    activeStep: 1,
    doneSteps: [],
  },
  session: {
    subtitle: "Настраиваем сессию…",
    progressFrom: 38,
    progressTo: 65,
    activeStep: 2,
    doneSteps: [1],
  },
  profile: {
    subtitle: "Загружаем ваш профиль…",
    progressFrom: 72,
    progressTo: 95,
    activeStep: 3,
    doneSteps: [1, 2],
  },
};

// ── Sub-components ────────────────────────────────────────────────────────────

/** Gradient-stroke spinning ring with Edvantix logo in the center. */
function SpinnerLogo() {
  return (
    <div className="relative flex size-20 items-center justify-center">
      {/* Background circle */}
      <div className="border-primary/10 bg-primary/5 absolute inset-0 rounded-full border-2" />

      {/* Rotating gradient ring */}
      <svg
        className="absolute inset-0 size-full"
        style={{ animation: "spin 1.1s linear infinite" }}
        viewBox="0 0 80 80"
        fill="none"
        aria-hidden="true"
      >
        <circle
          cx="40"
          cy="40"
          r="36"
          stroke="url(#edv-spin-grad)"
          strokeWidth="4"
          strokeLinecap="round"
          strokeDasharray="140 86"
        />
        <defs>
          {/* stopColor as CSS property — CSS variables don't work in SVG presentation attributes */}
          <linearGradient id="edv-spin-grad" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop
              offset="0%"
              style={{ stopColor: "var(--primary)", stopOpacity: 0 }}
            />
            <stop
              offset="100%"
              style={{ stopColor: "var(--primary)", stopOpacity: 1 }}
            />
          </linearGradient>
        </defs>
      </svg>

      {/* Edvantix layers icon — absolute so it renders above the spinning ring */}
      <div
        className="absolute inset-0 flex items-center justify-center"
        style={{ zIndex: 1 }}
      >
        <svg
          width="24"
          height="24"
          fill="none"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path
            d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"
            style={{ stroke: "var(--primary)" }}
            strokeWidth="1.8"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>
    </div>
  );
}

/** Mini spinner for the active step card. */
function MiniSpinner() {
  return (
    <svg
      className="text-primary size-4"
      style={{ animation: "spin 0.9s linear infinite" }}
      fill="none"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <circle
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        strokeWidth="3"
        strokeDasharray="40 22"
        strokeLinecap="round"
      />
    </svg>
  );
}

type StepStatus = "done" | "active" | "pending";

interface StepCardProps {
  step: StepDef;
  status: StepStatus;
  /** Stagger delay in ms so cards animate in sequentially. */
  delay: number;
  mounted: boolean;
}

function getStepSubtitle(step: StepDef, status: StepStatus): string {
  if (status === "done") return step.doneSubtitle;
  if (status === "active") return step.activeSubtitle;
  return "Ожидание…";
}

function StepCard({ step, status, delay, mounted }: Readonly<StepCardProps>) {
  return (
    <div
      className={cn(
        "transition-all duration-300",
        mounted ? "translate-y-0 opacity-100" : "translate-y-2 opacity-0",
      )}
      style={{ transitionDelay: `${delay}ms` }}
    >
      <div className="bg-card border-border flex items-center gap-3 rounded-xl border p-3.5 shadow-sm">
        {/* Status icon */}
        <div
          className={cn(
            "flex size-7 shrink-0 items-center justify-center rounded-full",
            status === "done" && "bg-emerald-50",
            status === "active" && "bg-primary/10",
            status === "pending" && "bg-muted",
          )}
        >
          {status === "done" && (
            <Check className="size-4 text-emerald-500" strokeWidth={2.5} />
          )}
          {status === "active" && <MiniSpinner />}
          {status === "pending" && (
            <Clock className="text-muted-foreground/40 size-4" />
          )}
        </div>

        {/* Text */}
        <div className="min-w-0 flex-1">
          <p
            className={cn(
              "text-sm font-medium",
              status === "pending"
                ? "text-muted-foreground/50"
                : "text-foreground",
            )}
          >
            {step.label}
          </p>
          <p
            className={cn(
              "mt-0.5 text-xs",
              status === "done" && "text-emerald-500",
              status === "active" && "text-muted-foreground",
              status === "pending" && "text-muted-foreground/30",
            )}
          >
            {getStepSubtitle(step, status)}
          </p>
        </div>
      </div>
    </div>
  );
}

// ── Main component ────────────────────────────────────────────────────────────

interface LoadingScreenProps {
  stage: LoadingStage;
}

/**
 * Full-screen auth loading overlay with animated progress bar and step cards.
 *
 * Stages map to auth flow steps:
 *   "auth"    → checking session (AuthGuard)
 *   "session" → fetching access token (AuthGuard post-auth)
 *   "profile" → loading profile (ProfileGuard)
 *
 * Progress animates smoothly within each stage's range via requestAnimationFrame.
 */
export function LoadingScreen({ stage }: Readonly<LoadingScreenProps>) {
  const config = STAGE_CONFIG[stage];
  const [progress, setProgress] = useState(config.progressFrom);
  const [subtitle, setSubtitle] = useState(config.subtitle);
  const [subtitleVisible, setSubtitleVisible] = useState(true);
  const [mounted, setMounted] = useState(false);
  const rafRef = useRef<ReturnType<typeof setInterval> | null>(null);

  // Stagger entrance animation on mount
  useEffect(() => {
    const t = setTimeout(() => setMounted(true), 30);
    return () => clearTimeout(t);
  }, []);

  // Animate progress bar when stage changes
  useEffect(() => {
    const {
      progressFrom,
      progressTo,
      subtitle: newSubtitle,
    } = STAGE_CONFIG[stage];

    setProgress(progressFrom);

    // Fade subtitle
    setSubtitleVisible(false);
    const subtitleTimer = setTimeout(() => {
      setSubtitle(newSubtitle);
      setSubtitleVisible(true);
    }, 180);

    // Animate progress toward target over ~1.4s
    const DURATION_MS = 1400;
    const INTERVAL_MS = 40;
    const steps = DURATION_MS / INTERVAL_MS;
    const increment = (progressTo - progressFrom) / steps;
    let current = progressFrom;

    rafRef.current = setInterval(() => {
      current += increment;
      if (current >= progressTo) {
        current = progressTo;
        if (rafRef.current !== null) clearInterval(rafRef.current);
      }
      setProgress(Math.round(current));
    }, INTERVAL_MS);

    return () => {
      if (rafRef.current !== null) clearInterval(rafRef.current);
      clearTimeout(subtitleTimer);
    };
  }, [stage]);

  return (
    <>
      {/* CSS keyframe for the spinner (injected once) */}
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>

      <div className="bg-muted/20 flex min-h-screen items-center justify-center p-6 sm:p-10">
        <div className="w-full max-w-[400px]">
          {/* Spinner + heading */}
          <div className="mb-10 flex flex-col items-center text-center">
            <div className="mb-6">
              <SpinnerLogo />
            </div>
            <h1 className="text-foreground text-xl font-bold tracking-tight">
              Выполняется вход
            </h1>
            <p
              className={cn(
                "text-muted-foreground mt-1.5 text-sm transition-opacity duration-200",
                subtitleVisible ? "opacity-100" : "opacity-0",
              )}
            >
              {subtitle}
            </p>
          </div>

          {/* Progress bar */}
          <div className="mb-8">
            <div className="mb-2 flex items-center justify-between">
              <span className="text-muted-foreground text-xs font-medium">
                Прогресс
              </span>
              <span className="text-primary text-xs font-semibold tabular-nums">
                {progress}%
              </span>
            </div>
            <div className="bg-muted h-1.5 w-full overflow-hidden rounded-full">
              <div
                className="bg-primary h-full rounded-full transition-[width] duration-500 ease-out"
                style={{ width: `${progress}%` }}
              />
            </div>
          </div>

          {/* Step cards */}
          <div className="space-y-3" role="status" aria-live="polite">
            {STEPS.map((step, idx) => {
              const getStatus = (): StepStatus => {
                if (config.doneSteps.includes(step.id)) return "done";
                if (config.activeStep === step.id) return "active";
                return "pending";
              };
              const status = getStatus();

              return (
                <StepCard
                  key={step.id}
                  step={step}
                  status={status}
                  delay={idx * 80}
                  mounted={mounted}
                />
              );
            })}
          </div>
        </div>
      </div>
    </>
  );
}
