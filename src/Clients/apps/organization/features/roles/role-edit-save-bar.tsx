"use client";

import { Check } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

interface SaveBarProps {
  visible: boolean;
  saving: boolean;
  onSave: () => void;
  onReset: () => void;
}

export function SaveBar({
  visible,
  saving,
  onSave,
  onReset,
}: Readonly<SaveBarProps>) {
  return (
    <div
      className="fixed inset-x-0 bottom-0 border-t border-slate-200 bg-white px-8 py-3.5 shadow-[0_-4px_12px_rgba(15,23,42,0.06)] transition-transform duration-300"
      style={{
        transform: visible ? "translateY(0)" : "translateY(100%)",
        zIndex: 40,
      }}
    >
      <div className="mx-auto flex max-w-3xl items-center justify-between gap-5">
        <div className="flex items-center gap-2.5 text-sm">
          <span className="size-2 rounded-full bg-amber-400" />
          <strong className="text-slate-900">Несохранённые изменения</strong>
          <span className="text-slate-500">— сохраните, чтобы применить</span>
        </div>
        <div className="flex gap-2.5">
          <Button variant="ghost" size="sm" onClick={onReset} disabled={saving}>
            Отменить
          </Button>
          <Button size="sm" onClick={onSave} disabled={saving}>
            {saving ? (
              <>
                <span className="size-3.5 animate-spin rounded-full border-2 border-white/35 border-t-white" />
                Сохранение…
              </>
            ) : (
              <>
                <Check className="size-4" strokeWidth={2.5} />
                Сохранить
              </>
            )}
          </Button>
        </div>
      </div>
    </div>
  );
}
