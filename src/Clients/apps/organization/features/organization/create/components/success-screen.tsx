import { ArrowRight, Check } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

interface SuccessScreenProps {
  name: string;
  onDashboard: () => void;
}

export function SuccessScreen({ name, onDashboard }: SuccessScreenProps) {
  return (
    <div className="flex max-w-[420px] flex-col items-center gap-4 text-center">
      <div className="flex size-[72px] items-center justify-center rounded-full bg-emerald-100">
        <div className="flex size-[52px] items-center justify-center rounded-full bg-emerald-500 shadow-[0_8px_24px_rgba(16,185,129,0.35)]">
          <Check className="size-7 text-white" strokeWidth={3} />
        </div>
      </div>

      <div>
        <h2 className="text-foreground text-2xl font-bold tracking-tight">
          Организация зарегистрирована
        </h2>
        <p className="text-muted-foreground mx-auto mt-2 max-w-[380px] text-[14.5px] leading-relaxed">
          <strong className="text-foreground font-semibold">{name}</strong>{" "}
          появилась в вашем кабинете. Теперь добавьте первый курс и пригласите
          преподавателей.
        </p>
      </div>

      <div className="mt-2 flex flex-col gap-2 sm:flex-row sm:gap-2.5">
        <Button variant="outline" size="sm">
          Добавить курс
        </Button>
        <Button size="sm" onClick={onDashboard}>
          Перейти в кабинет
          <ArrowRight className="size-4" />
        </Button>
      </div>
    </div>
  );
}
