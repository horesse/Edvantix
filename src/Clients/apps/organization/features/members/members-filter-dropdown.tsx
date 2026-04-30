"use client";

import { useEffect, useRef, useState } from "react";

import { Check, ChevronDown } from "lucide-react";

interface FilterOption<T> {
  value: T;
  label: string;
  dot?: string;
}

interface FilterDropdownProps<T extends string | number> {
  label: string;
  options: readonly FilterOption<T>[];
  value: Set<T>;
  onChange: (next: Set<T>) => void;
}

/** Мультиселект-дропдаун для фильтрации таблицы. */
export function FilterDropdown<T extends string | number>({
  label,
  options,
  value,
  onChange,
}: Readonly<FilterDropdownProps<T>>) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);
  const count = value.size;

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  function toggle(v: T) {
    const next = new Set(value);
    if (next.has(v)) {
      next.delete(v);
    } else {
      next.add(v);
    }
    onChange(next);
  }

  return (
    <div ref={ref} className="relative">
      <button
        type="button"
        onClick={() => setOpen((o) => !o)}
        className="inline-flex h-9 items-center gap-2 rounded-xl border px-3.5 text-sm font-medium transition-all focus:outline-none"
        style={{
          borderColor: count > 0 ? "#c7d6fe" : "#e2e8f0",
          background: count > 0 ? "rgba(79,70,229,0.05)" : "#fff",
          color: count > 0 ? "#4338ca" : "#334155",
        }}
      >
        <span>{label}</span>
        {count > 0 && (
          <span
            className="flex h-5 min-w-[18px] items-center justify-center rounded-full px-1.5 text-[11px] font-semibold text-white tabular-nums"
            style={{ background: "#4f46e5" }}
          >
            {count}
          </span>
        )}
        <ChevronDown
          className="size-3.5 transition-transform"
          style={{
            color: count > 0 ? "#4338ca" : "#94a3b8",
            transform: open ? "rotate(180deg)" : "none",
          }}
        />
      </button>

      {open && (
        <div className="bg-card border-border absolute top-11 left-0 z-20 min-w-[200px] rounded-xl border p-1.5 shadow-lg">
          {options.map((opt) => {
            const checked = value.has(opt.value);
            return (
              <button
                key={String(opt.value)}
                type="button"
                onClick={() => toggle(opt.value)}
                className="hover:bg-muted flex w-full items-center gap-2.5 rounded-lg px-2.5 py-2 text-left text-sm transition-colors"
              >
                <span
                  className="flex size-4 shrink-0 items-center justify-center rounded"
                  style={{
                    border: `1.5px solid ${checked ? "#4f46e5" : "#cbd5e1"}`,
                    background: checked ? "#4f46e5" : "#fff",
                    transition: "all .12s",
                  }}
                >
                  {checked && (
                    <Check className="size-2.5 text-white" strokeWidth={3} />
                  )}
                </span>
                {opt.dot && (
                  <span
                    className="size-1.5 shrink-0 rounded-full"
                    style={{ background: opt.dot }}
                  />
                )}
                <span className="text-foreground flex-1">{opt.label}</span>
              </button>
            );
          })}

          {count > 0 && (
            <>
              <div className="my-1.5 h-px bg-slate-100" />
              <button
                type="button"
                onClick={() => onChange(new Set())}
                className="hover:bg-muted text-muted-foreground w-full rounded-lg px-2.5 py-2 text-left text-xs transition-colors"
              >
                Сбросить
              </button>
            </>
          )}
        </div>
      )}
    </div>
  );
}
