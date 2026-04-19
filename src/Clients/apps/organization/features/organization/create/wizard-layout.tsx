import type { ReactNode } from "react";

import { ArrowLeft, ArrowRight, Check, ChevronRight } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { cn } from "@workspace/ui/lib/utils";

import { WIZARD_STEPS } from "./constants";
import { StepperSidebar } from "./stepper-sidebar";

// ── Breadcrumb ────────────────────────────────────────────────────────────────

function BreadcrumbBar() {
  return (
    <div className="border-border bg-card flex items-center gap-2 border-b px-4 py-[18px] text-[13px] sm:px-8">
      <span className="text-muted-foreground">Организация</span>
      <ChevronRight className="size-3.5 text-slate-300" />
      <span className="text-foreground font-medium">Регистрация</span>
    </div>
  );
}

// ── Mobile step bar (below lg) ────────────────────────────────────────────────

interface MobileStepBarProps {
  step: number;
  completed: Set<number>;
  onJump?: (i: number) => void;
}

function MobileStepBar({ step, completed, onJump }: MobileStepBarProps) {
  const currentStep = WIZARD_STEPS[step];

  return (
    <div className="border-border bg-card border-b px-4 py-3 lg:hidden">
      <div className="mb-2 flex items-center justify-between">
        <span className="text-foreground text-[13px] font-semibold">
          {currentStep?.title}
        </span>
        <span className="text-muted-foreground text-[12px]">
          {step + 1} / {WIZARD_STEPS.length}
        </span>
      </div>
      <div className="flex items-center gap-2.5">
        <div className="relative h-1.5 flex-1 overflow-hidden rounded-full bg-slate-100">
          <div
            className="bg-brand-600 absolute inset-y-0 left-0 rounded-full transition-all duration-300 ease-out"
            style={{ width: `${((step + 1) / WIZARD_STEPS.length) * 100}%` }}
          />
        </div>
        <div className="flex shrink-0 items-center gap-1">
          {WIZARD_STEPS.map((s, i) => {
            const isDone = completed.has(i);
            const isActive = i === step;
            const canJump = isDone || isActive;
            return (
              <button
                key={s.id}
                type="button"
                aria-label={s.title}
                disabled={!canJump}
                onClick={() => canJump && onJump?.(i)}
                className={cn(
                  "h-1.5 rounded-full transition-all duration-200",
                  isActive
                    ? "bg-brand-600 w-5"
                    : isDone
                      ? "bg-brand-300 w-1.5"
                      : "w-1.5 bg-slate-200",
                  canJump ? "cursor-pointer" : "cursor-default",
                )}
              />
            );
          })}
        </div>
      </div>
    </div>
  );
}

// ── Footer ────────────────────────────────────────────────────────────────────

interface WizardFooterProps {
  step: number;
  isLast: boolean;
  isPending: boolean;
  onBack: () => void;
  onNext: () => void;
  onCancel: () => void;
}

function WizardFooter({
  step,
  isLast,
  isPending,
  onBack,
  onNext,
  onCancel,
}: WizardFooterProps) {
  return (
    <div className="border-border bg-card flex items-center justify-between border-t px-4 py-3 sm:px-8 sm:py-4 lg:px-12">
      <div className="text-muted-foreground hidden text-[13px] sm:block">
        Шаг{" "}
        <strong className="text-foreground font-semibold">{step + 1}</strong> из{" "}
        {WIZARD_STEPS.length}
      </div>
      <div className="flex w-full justify-end gap-2 sm:w-auto sm:gap-2.5">
        <Button type="button" variant="ghost" size="sm" onClick={onCancel}>
          Отмена
        </Button>
        {step > 0 && (
          <Button type="button" variant="outline" size="sm" onClick={onBack}>
            <ArrowLeft className="size-4" />
            Назад
          </Button>
        )}
        <Button type="button" size="sm" onClick={onNext} disabled={isPending}>
          {isLast ? (
            <>
              {isPending ? "Сохранение…" : "Зарегистрировать"}
              <Check className="size-4" strokeWidth={2.5} />
            </>
          ) : (
            <>
              Далее
              <ArrowRight className="size-4" />
            </>
          )}
        </Button>
      </div>
    </div>
  );
}

// ── Layout shell ──────────────────────────────────────────────────────────────

interface WizardLayoutProps {
  step: number;
  completed: Set<number>;
  isLast: boolean;
  isPending: boolean;
  onBack: () => void;
  onNext: () => void;
  onCancel: () => void;
  onJump: (i: number) => void;
  children: ReactNode;
}

/**
 * Full-screen wizard shell: stepper sidebar + breadcrumb + scrollable content + footer.
 * Uses negative margins to escape the (main) layout padding.
 */
export function WizardLayout({
  step,
  completed,
  isLast,
  isPending,
  onBack,
  onNext,
  onCancel,
  onJump,
  children,
}: WizardLayoutProps) {
  return (
    <div className="-mx-4 -my-4 flex h-screen overflow-hidden lg:-mx-6 lg:-my-6">
      <StepperSidebar current={step} completed={completed} onJump={onJump} />

      <div className="flex min-w-0 flex-1 flex-col overflow-hidden">
        <BreadcrumbBar />
        <MobileStepBar step={step} completed={completed} onJump={onJump} />

        <div className="flex-1 overflow-y-auto">
          <div className="mx-auto max-w-[720px] px-4 py-6 sm:px-8 sm:py-8 lg:px-12 lg:py-10">
            {children}
          </div>
        </div>

        <WizardFooter
          step={step}
          isLast={isLast}
          isPending={isPending}
          onBack={onBack}
          onNext={onNext}
          onCancel={onCancel}
        />
      </div>
    </div>
  );
}
