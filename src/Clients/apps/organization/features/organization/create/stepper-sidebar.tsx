import { Check, Info, Sparkles } from "lucide-react";

import { cn } from "@workspace/ui/lib/utils";

import { WIZARD_STEPS } from "./constants";

function declension(n: number, forms: [string, string, string]): string {
  const abs = Math.abs(n);
  const mod10 = abs % 10;
  const mod100 = abs % 100;
  if (mod10 === 1 && mod100 !== 11) return forms[0];
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20))
    return forms[1];
  return forms[2];
}

interface StepperSidebarProps {
  current: number;
  completed: Set<number>;
  onJump?: (i: number) => void;
}

export function StepperSidebar({
  current,
  completed,
  onJump,
}: StepperSidebarProps) {
  const remaining = Math.max(0, WIZARD_STEPS.length - current);

  return (
    <aside className="border-border bg-card flex w-[280px] shrink-0 flex-col gap-4 overflow-y-auto border-r p-5">
      {/* Header */}
      <div>
        <div className="bg-brand-100 text-brand-700 mb-3 inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-[11px] font-semibold tracking-wide">
          <Sparkles className="size-3" />
          НАСТРОЙКА ШКОЛЫ
        </div>
        <h1 className="text-foreground mb-1 text-xl font-bold tracking-tight">
          Регистрация организации
        </h1>
        <p className="text-muted-foreground text-[13px] leading-relaxed">
          {remaining > 0
            ? `Осталось ${remaining} ${declension(remaining, ["шаг", "шага", "шагов"])} до запуска школы.`
            : "Все шаги выполнены!"}
        </p>
      </div>

      {/* Step list */}
      <ol className="m-0 flex list-none flex-col gap-0.5 p-0">
        {WIZARD_STEPS.map((s, i) => {
          const isDone = completed.has(i);
          const isActive = i === current;
          const isFuture = !isDone && !isActive;
          const canJump = isDone || isActive;

          return (
            <li key={s.id}>
              <button
                type="button"
                disabled={!canJump}
                onClick={() => canJump && onJump?.(i)}
                className={cn(
                  "flex w-full items-start gap-3 rounded-[10px] border border-transparent px-3 py-2.5 text-left font-sans transition-colors",
                  isActive && "bg-brand-50/60 border-brand-100",
                  !isActive && canJump && "hover:bg-muted/60 cursor-pointer",
                  !canJump && "cursor-default",
                )}
              >
                {/* Circle indicator */}
                <div className="relative mt-0.5 shrink-0">
                  <div
                    className={cn(
                      "flex size-7 items-center justify-center rounded-full border text-[13px] font-semibold transition-all",
                      isActive &&
                        "bg-brand-600 border-brand-600 text-white shadow-[0_0_0_4px_rgba(79,70,229,0.12)]",
                      isDone && "bg-brand-100 border-brand-200 text-brand-700",
                      isFuture && "border-slate-200 bg-white text-slate-400",
                    )}
                  >
                    {isDone ? (
                      <Check className="size-3.5" strokeWidth={2.5} />
                    ) : (
                      i + 1
                    )}
                  </div>
                  {i < WIZARD_STEPS.length - 1 && (
                    <div
                      className={cn(
                        "absolute top-[30px] left-[13px] w-0.5",
                        isDone ? "bg-brand-200" : "bg-slate-200",
                      )}
                      style={{ bottom: "-14px" }}
                    />
                  )}
                </div>

                {/* Title + hint */}
                <div className="min-w-0 flex-1 pt-0.5">
                  <div
                    className={cn(
                      "text-[13.5px] leading-tight",
                      isActive && "text-foreground font-semibold",
                      isDone && "font-medium text-slate-600",
                      isFuture && "font-medium text-slate-400",
                    )}
                  >
                    {s.title}
                  </div>
                  <div
                    className={cn(
                      "mt-0.5 text-[12px] leading-tight",
                      isFuture ? "text-slate-300" : "text-muted-foreground",
                    )}
                  >
                    {s.hint}
                  </div>
                </div>
              </button>
            </li>
          );
        })}
      </ol>

      {/* Support card */}
      <div className="mt-auto border-t border-slate-100 pt-4">
        <div className="bg-muted/50 border-border flex gap-2.5 rounded-xl border p-3">
          <div className="border-border bg-card text-brand-600 flex size-8 shrink-0 items-center justify-center rounded-lg border">
            <Info className="size-4" />
          </div>
          <div className="text-muted-foreground min-w-0 flex-1 text-[12px] leading-snug">
            Нужна помощь? Напишите нам — ответим в течение 15 минут.
            <a
              href="mailto:support@edvantix.ru"
              className="text-brand-600 mt-1 block font-medium"
            >
              support@edvantix.ru →
            </a>
          </div>
        </div>
      </div>
    </aside>
  );
}
