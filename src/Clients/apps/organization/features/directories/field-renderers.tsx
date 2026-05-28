"use client";

import type { UseFormReturn } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Textarea } from "@workspace/ui/components/textarea";
import { cn } from "@workspace/ui/lib/utils";

import { COLOR_DOTS } from "./color-palette";
import type { DirectoryField } from "./directory-config";

interface FieldRendererProps {
  field: DirectoryField;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  form: UseFormReturn<any>;
  mode?: "create" | "edit";
}

/** Рендерит одно поле формы справочника по его дескриптору. */
export function FieldRenderer({
  field: f,
  form,
  mode,
}: Readonly<FieldRendererProps>) {
  switch (f.kind) {
    case "text":
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                {f.label}
                {f.required && <span className="ml-1 text-red-500">*</span>}
                {f.hint && (
                  <span className="ml-auto text-xs text-slate-400">
                    {f.hint}
                  </span>
                )}
              </FormLabel>
              <FormControl>
                <Input
                  {...field}
                  placeholder={f.placeholder}
                  maxLength={f.maxLength}
                  autoComplete="off"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );

    case "code": {
      if (f.showOnlyInEdit && mode !== "edit") return null;
      const isReadonly = f.readonlyInEdit && mode === "edit";
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                {f.label}
                {f.hint && !isReadonly && (
                  <span className="ml-auto text-xs text-slate-400">
                    {f.hint}
                  </span>
                )}
                {isReadonly && (
                  <span className="ml-auto text-xs text-slate-400">
                    не изменяется
                  </span>
                )}
              </FormLabel>
              <FormControl>
                <Input
                  {...field}
                  readOnly={isReadonly}
                  className={cn(
                    "font-mono tracking-widest uppercase",
                    isReadonly && "cursor-default bg-slate-50 text-slate-500",
                  )}
                  maxLength={f.maxLength ?? 8}
                  onChange={
                    isReadonly
                      ? undefined
                      : (e) =>
                          field.onChange(
                            e.target.value
                              .toUpperCase()
                              .slice(0, f.maxLength ?? 8),
                          )
                  }
                  autoComplete="off"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );
    }

    case "textarea":
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                {f.label}
                {f.hint && (
                  <span className="ml-auto text-xs text-slate-400">
                    {f.hint}
                  </span>
                )}
              </FormLabel>
              <FormControl>
                <Textarea
                  {...field}
                  rows={f.rows ?? 4}
                  maxLength={f.maxLength}
                  className="resize-y"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );

    case "color":
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>{f.label}</FormLabel>
              <FormControl>
                <div className="flex flex-wrap gap-2">
                  {Object.entries(COLOR_DOTS).map(([key, hex]) => {
                    const active = field.value === key;
                    return (
                      <button
                        key={key}
                        type="button"
                        aria-label={key}
                        onClick={() => field.onChange(key)}
                        className={cn(
                          "flex size-8 items-center justify-center rounded-lg border-2 bg-white transition-shadow",
                          active && "ring-2 ring-offset-1",
                        )}
                        style={{ borderColor: active ? hex : "#e2e8f0" }}
                      >
                        <span
                          className="size-3.5 rounded-full"
                          style={{ background: hex }}
                        />
                      </button>
                    );
                  })}
                </div>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );

    case "enumSelect":
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>{f.label}</FormLabel>
              <Select
                value={String(field.value ?? "")}
                onValueChange={(v) => field.onChange(v)}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {f.options.map((opt) => (
                    <SelectItem
                      key={String(opt.value)}
                      value={String(opt.value)}
                    >
                      {opt.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />
      );

    case "number":
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>{f.label}</FormLabel>
              <FormControl>
                <div className="relative">
                  <Input
                    {...field}
                    type="number"
                    min={f.min}
                    max={f.max}
                    onChange={(e) => field.onChange(e.target.valueAsNumber)}
                    className={f.suffix ? "pr-10" : undefined}
                  />
                  {f.suffix && (
                    <span className="pointer-events-none absolute inset-y-0 right-3 flex items-center text-sm text-slate-400">
                      {f.suffix}
                    </span>
                  )}
                </div>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );

    case "statusToggle": {
      if (f.showOnlyInEdit && mode !== "edit") return null;
      return (
        <FormField
          control={form.control}
          name={f.name}
          render={({ field }) => (
            <FormItem>
              <FormLabel>{f.label}</FormLabel>
              <FormControl>
                <div className="grid h-[38px] grid-cols-2 gap-1 rounded-lg bg-slate-100 p-1">
                  {(
                    [
                      { value: false, label: "Активный" },
                      { value: true, label: "Архив" },
                    ] as const
                  ).map((opt) => {
                    const active = field.value === opt.value;
                    return (
                      <button
                        key={String(opt.value)}
                        type="button"
                        onClick={() => field.onChange(opt.value)}
                        className={cn(
                          "rounded-md text-sm font-medium transition-all",
                          active
                            ? "bg-white text-slate-900 shadow-sm"
                            : "text-slate-500 hover:text-slate-700",
                        )}
                      >
                        {opt.label}
                      </button>
                    );
                  })}
                </div>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      );
    }

    case "row": {
      const visibleChildren = f.children.filter((child) => {
        if (
          "showOnlyInEdit" in child &&
          child.showOnlyInEdit &&
          mode !== "edit"
        )
          return false;
        return true;
      });
      return (
        <div
          className={cn(
            "grid gap-3",
            visibleChildren.length > 1 ? "grid-cols-2" : "",
          )}
        >
          {visibleChildren.map((child) => (
            <FieldRenderer
              key={child.name}
              field={child}
              form={form}
              mode={mode}
            />
          ))}
        </div>
      );
    }

    case "switch":
      return null;

    default:
      return null;
  }
}
