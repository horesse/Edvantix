import { AlertCircle, Archive } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

interface DangerZoneSectionProps {
  onArchive: () => void;
  isArchiving: boolean;
}

export function DangerZoneSection({
  onArchive,
  isArchiving,
}: Readonly<DangerZoneSectionProps>) {
  return (
    <section className="overflow-hidden rounded-2xl border border-red-200 bg-white">
      <header className="flex items-center gap-3 border-b border-red-100 bg-red-500/[0.03] px-6 py-3.5">
        <AlertCircle className="size-4 text-red-700" />
        <h2 className="text-sm font-semibold text-red-800">Опасная зона</h2>
      </header>
      <div className="px-6 py-4">
        <div className="flex flex-wrap items-center gap-4">
          <div className="min-w-0 flex-1">
            <p className="text-[13.5px] font-medium text-slate-900">
              Архивировать организацию
            </p>
            <p className="mt-0.5 text-[12.5px] text-slate-500">
              Скрыть организацию из списка. Данные сохранятся, восстановить
              можно в течение 90 дней.
            </p>
          </div>
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="shrink-0 border-red-200 text-red-700 hover:bg-red-50"
            onClick={onArchive}
            disabled={isArchiving}
          >
            <Archive className="size-3.5" />
            Архивировать
          </Button>
        </div>
      </div>
    </section>
  );
}
