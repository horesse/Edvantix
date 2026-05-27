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
}

/** Рендерит одно поле формы справочника по его дескриптору. */
export function FieldRenderer({
  field: f,
  form,
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

    case "code":
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
                <Input
                  {...field}
                  className="font-mono tracking-widest uppercase"
                  maxLength={f.maxLength ?? 8}
                  onChange={(e) =>
                    field.onChange(
                      e.target.value.toUpperCase().slice(0, f.maxLength ?? 8),
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

    case "switch":
      return null;

    default:
      return null;
  }
}
