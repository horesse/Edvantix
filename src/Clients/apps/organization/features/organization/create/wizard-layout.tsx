import type { ReactNode } from "react";

import { ArrowLeft, ArrowRight, Check, ChevronRight } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

import { WIZARD_STEPS } from "./constants";
import { StepperSidebar } from "./stepper-sidebar";

// ── Breadcrumb ────────────────────────────────────────────────────────────────

function BreadcrumbBar() {
  return (
    <div className="border-border bg-card flex items-center gap-2 border-b px-8 py-[18px] text-[13px]">
      <span className="text-muted-foreground">Организация</span>
      <ChevronRight className="size-3.5 text-slate-300" />
      <span className="text-foreground font-medium">Регистрация</span>
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
    <div className="border-border bg-card flex items-center justify-between border-t px-12 py-4">
      <div className="text-muted-foreground text-[13px]">
        Шаг{" "}
        <strong className="text-foreground font-semibold">{step + 1}</strong> из{" "}
        {WIZARD_STEPS.length}
      </div>
      <div className="flex gap-2.5">
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

        <div className="flex-1 overflow-y-auto">
          <div className="mx-auto max-w-[720px] px-12 py-10">{children}</div>
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
