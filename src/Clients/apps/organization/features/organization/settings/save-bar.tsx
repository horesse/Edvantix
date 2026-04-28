import { Check } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Spinner } from "@workspace/ui/components/spinner";

import { pluralRu } from "./schema";

interface SaveBarProps {
  changedCount: number;
  isSaving: boolean;
  onSave: () => void;
  onReset: () => void;
}

export function SaveBar({
  changedCount,
  isSaving,
  onSave,
  onReset,
}: Readonly<SaveBarProps>) {
  return (
    <div className="border-border sticky bottom-0 -mx-4 border-t bg-white px-4 py-3 shadow-[0_-4px_12px_rgba(15,23,42,0.06)] lg:-mx-6 lg:px-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="flex items-center gap-3 text-sm">
          <div className="flex size-8 shrink-0 items-center justify-center rounded-full bg-amber-100">
            <span className="size-2 rounded-full bg-amber-400" />
          </div>
          <div>
            <p className="font-semibold text-slate-900">
              {changedCount}{" "}
              {pluralRu(
                changedCount,
                "несохранённое изменение",
                "несохранённых изменения",
                "несохранённых изменений",
              )}
            </p>
            <p className="text-xs text-slate-500">Сохраните, чтобы применить</p>
          </div>
        </div>
        <div className="flex gap-2">
          <Button
            type="button"
            variant="ghost"
            size="sm"
            onClick={onReset}
            disabled={isSaving}
          >
            Отменить изменения
          </Button>
          <Button type="button" size="sm" onClick={onSave} disabled={isSaving}>
            {isSaving ? (
              <>
                <Spinner className="size-3.5" />
                Сохранение…
              </>
            ) : (
              <>
                <Check className="size-3.5" strokeWidth={2.5} />
                Сохранить изменения
              </>
            )}
          </Button>
        </div>
      </div>
    </div>
  );
}
