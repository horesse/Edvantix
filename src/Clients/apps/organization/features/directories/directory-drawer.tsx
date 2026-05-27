"use client";

import { useEffect } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { X } from "lucide-react";
import type { DefaultValues, Resolver } from "react-hook-form";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import type {
  DirectoryItemBase,
  DirectoryUsageDto,
} from "@workspace/types/organization";
import { Button } from "@workspace/ui/components/button";
import { Form } from "@workspace/ui/components/form";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@workspace/ui/components/sheet";

import type { DirectoryConfig } from "./directory-config";
import { FieldRenderer } from "./field-renderers";

interface DirectoryDrawerProps<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
> {
  config: DirectoryConfig<TItem, TForm>;
  orgId: string;
  open: boolean;
  mode: "create" | "edit";
  item?: TItem | null;
  onClose: () => void;
  onCreate: (orgId: string, request: unknown) => Promise<unknown>;
  onUpdate: (orgId: string, id: string, request: unknown) => Promise<unknown>;
}

/** Slide-over drawer для создания/редактирования элемента справочника. */
export function DirectoryDrawer<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
>({
  config,
  orgId,
  open,
  mode,
  item,
  onClose,
  onCreate,
  onUpdate,
}: Readonly<DirectoryDrawerProps<TItem, TForm>>) {
  const form = useForm<TForm>({
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(config.schema as any) as Resolver<TForm>,
    defaultValues: config.defaults as DefaultValues<TForm>,
  });

  const { reset } = form;
  useEffect(() => {
    if (open) {
      if (mode === "edit" && item) {
        reset(config.fromItem(item) as Parameters<typeof reset>[0]);
      } else {
        reset(config.defaults as Parameters<typeof reset>[0]);
      }
    }
  }, [open, mode, item, reset, config]);

  async function handleSubmit(values: TForm) {
    try {
      if (mode === "create") {
        await onCreate(orgId, config.toCreate(values));
        toast.success(`${config.singular} создан`);
      } else if (item) {
        await onUpdate(orgId, item.id, config.toUpdate(values, item));
        toast.success(`${config.singular} обновлён`);
      }
      onClose();
    } catch {
      toast.error("Не удалось сохранить. Попробуйте ещё раз.");
    }
  }

  const usageCards: DirectoryUsageDto[] =
    mode === "edit" && item
      ? (config.usageCards?.(item) ?? (item.usage ? [...item.usage] : []))
      : [];

  const Icon = config.icon;
  const title =
    mode === "edit"
      ? `Изменить ${config.singular}`
      : `Новый ${config.singular}`;

  return (
    <Sheet open={open} onOpenChange={(v) => !v && onClose()}>
      <SheetContent className="flex w-[460px] flex-col gap-0 p-0 sm:max-w-[460px]">
        <SheetHeader className="flex-row items-center gap-3 border-b px-6 py-4">
          <div className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-indigo-50 text-indigo-600">
            <Icon className="size-5" />
          </div>
          <div className="min-w-0 flex-1">
            <SheetTitle className="text-base">{title}</SheetTitle>
            <p className="mt-0.5 text-xs text-slate-400">
              Справочник «{config.plural}»
            </p>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="ml-auto flex size-8 shrink-0 items-center justify-center rounded-lg text-slate-400 transition-colors hover:bg-slate-100 hover:text-slate-600"
            aria-label="Закрыть"
          >
            <X className="size-4" />
          </button>
        </SheetHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="flex min-h-0 flex-1 flex-col"
          >
            <div className="flex-1 space-y-5 overflow-y-auto px-6 py-5">
              {config.fields.map((f) => (
                <FieldRenderer key={f.name} field={f} form={form} />
              ))}

              {usageCards.length > 0 && (
                <div className="rounded-xl border bg-slate-50 p-4">
                  <p className="mb-3 text-[11px] font-semibold tracking-wider text-slate-500 uppercase">
                    Где используется
                  </p>
                  <div
                    className="grid gap-2"
                    style={{
                      gridTemplateColumns: `repeat(${Math.min(usageCards.length, 3)}, 1fr)`,
                    }}
                  >
                    {usageCards.map((card) => (
                      <div key={card.label}>
                        <p className="text-lg font-semibold text-slate-900 tabular-nums">
                          {card.count}
                        </p>
                        <p className="text-xs text-slate-500">{card.label}</p>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>

            <div className="flex items-center justify-end gap-2 border-t px-6 py-4">
              <Button type="button" variant="outline" onClick={onClose}>
                Отмена
              </Button>
              <Button type="submit" disabled={form.formState.isSubmitting}>
                {mode === "edit" ? "Сохранить" : "Создать"}
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
}
