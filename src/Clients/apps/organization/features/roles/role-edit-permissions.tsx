"use client";

import { Check, ChevronDown, Search } from "lucide-react";

import type { FeatureDto } from "@workspace/types/organization";

import { FEATURE_META } from "./roles-constants";

interface PermissionsSectionProps {
  visibleFeatures: {
    feat: FeatureDto;
    permissions: FeatureDto["permissions"];
  }[];
  activePerms: Set<string>;
  search: string;
  onSearchChange: (v: string) => void;
  collapsed: Set<string>;
  onToggleCollapse: (code: string) => void;
  onTogglePerm: (id: string) => void;
  onToggleFeature: (feat: FeatureDto) => void;
  readonly: boolean;
}

export function PermissionsSection({
  visibleFeatures,
  activePerms,
  search,
  onSearchChange,
  collapsed,
  onToggleCollapse,
  onTogglePerm,
  onToggleFeature,
  readonly,
}: Readonly<PermissionsSectionProps>) {
  return (
    <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white">
      <div className="flex items-center gap-4 border-b border-slate-100 px-5 py-4">
        <div className="min-w-0 flex-1">
          <h2 className="text-sm font-semibold">Права доступа</h2>
          <p className="text-muted-foreground mt-0.5 text-xs">
            Отметьте, что может делать участник с этой ролью
          </p>
        </div>
        <div className="relative h-[34px] w-64">
          <Search className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 size-3.5 -translate-y-1/2" />
          <input
            value={search}
            onChange={(e) => onSearchChange(e.target.value)}
            placeholder="Поиск по правам"
            className="focus:border-primary focus:ring-primary/20 h-[34px] w-full rounded-lg border border-slate-200 bg-white pr-3 pl-8 text-[13px] outline-none focus:ring-3"
          />
        </div>
      </div>

      <div>
        {visibleFeatures.map(({ feat, permissions }) => {
          const meta = FEATURE_META[feat.code];
          const Icon = meta?.icon;
          const ids = feat.permissions.map((p) => p.id);
          const granted = ids.filter((id) => activePerms.has(id)).length;
          const allOn = granted === ids.length && ids.length > 0;
          const someOn = granted > 0 && !allOn;
          const isCollapsed = collapsed.has(feat.code);

          return (
            <div key={feat.code} className="border-t border-slate-50">
              <div className="flex items-center gap-3.5 bg-slate-50/80 px-5 py-3.5">
                <button
                  type="button"
                  onClick={() => onToggleCollapse(feat.code)}
                  className="flex size-5 items-center justify-center rounded text-slate-400 transition-colors hover:text-slate-600"
                  style={{
                    transform: isCollapsed ? "rotate(-90deg)" : "rotate(0deg)",
                    transition: "transform 0.15s",
                  }}
                >
                  <ChevronDown className="size-4" />
                </button>

                {Icon && (
                  <div className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-indigo-50 text-indigo-600">
                    <Icon className="size-4" />
                  </div>
                )}

                <div className="min-w-0 flex-1">
                  <p className="text-[13.5px] font-semibold text-slate-900">
                    {feat.name}
                  </p>
                  {meta?.description && (
                    <p className="text-xs text-slate-500">{meta.description}</p>
                  )}
                </div>

                <span
                  className="text-muted-foreground shrink-0 text-xs tabular-nums"
                  style={{ minWidth: 48, textAlign: "right" }}
                >
                  {granted} / {ids.length}
                </span>

                <FeatureToggle
                  allOn={allOn}
                  someOn={someOn}
                  disabled={readonly}
                  onChange={() => onToggleFeature(feat)}
                />
              </div>

              {!isCollapsed && (
                <div className="pt-1 pb-3 pl-[70px]">
                  {permissions.map((perm) => {
                    const on = activePerms.has(perm.id);
                    return (
                      <label
                        key={perm.id}
                        className="flex items-center gap-3 rounded-lg px-2.5 py-2 transition-colors"
                        style={{ cursor: readonly ? "not-allowed" : "pointer" }}
                        onMouseEnter={(e) => {
                          if (!readonly)
                            e.currentTarget.style.background = "#f8fafc";
                        }}
                        onMouseLeave={(e) => {
                          e.currentTarget.style.background = "transparent";
                        }}
                      >
                        <PermCheckbox
                          checked={on}
                          disabled={readonly}
                          onChange={() => onTogglePerm(perm.id)}
                        />
                        <span
                          className="text-[13px]"
                          style={{
                            color: on ? "#0f172a" : "#475569",
                            fontWeight: on ? 500 : 400,
                          }}
                        >
                          {perm.name}
                        </span>
                        <code className="ml-auto rounded bg-slate-50 px-2 py-0.5 font-mono text-[11px] text-slate-400">
                          {perm.code}
                        </code>
                      </label>
                    );
                  })}
                </div>
              )}
            </div>
          );
        })}

        {visibleFeatures.length === 0 && (
          <p className="text-muted-foreground px-5 py-8 text-center text-sm">
            {search ? "Права не найдены" : "Права не настроены"}
          </p>
        )}
      </div>
    </section>
  );
}

function FeatureToggle({
  allOn,
  someOn,
  disabled,
  onChange,
}: Readonly<{
  allOn: boolean;
  someOn: boolean;
  disabled: boolean;
  onChange: () => void;
}>) {
  let bg: string;
  if (disabled) {
    bg = "#e2e8f0";
  } else if (allOn) {
    bg = "#4f46e5";
  } else if (someOn) {
    bg = "#818cf8";
  } else {
    bg = "#cbd5e1";
  }

  return (
    <button
      type="button"
      onClick={onChange}
      disabled={disabled}
      className="relative shrink-0 rounded-full transition-colors"
      style={{
        width: 40,
        height: 22,
        background: bg,
        cursor: disabled ? "not-allowed" : "pointer",
      }}
    >
      <span
        className="absolute top-[2px] size-[18px] rounded-full bg-white shadow-sm transition-[left] duration-150"
        style={{ left: allOn || someOn ? 20 : 2 }}
      />
      {someOn && !allOn && (
        <span
          className="absolute rounded-sm bg-indigo-800"
          style={{ top: 9, left: 26, width: 6, height: 2 }}
        />
      )}
    </button>
  );
}

function PermCheckbox({
  checked,
  disabled,
  onChange,
}: Readonly<{
  checked: boolean;
  disabled: boolean;
  onChange: () => void;
}>) {
  return (
    <span
      role="checkbox"
      aria-checked={checked}
      tabIndex={disabled ? -1 : 0}
      onClick={disabled ? undefined : onChange}
      onKeyDown={(e) => {
        if (!disabled && (e.key === " " || e.key === "Enter")) {
          e.preventDefault();
          onChange();
        }
      }}
      className="inline-flex shrink-0 items-center justify-center rounded transition-colors"
      style={{
        width: 18,
        height: 18,
        border: `1.5px solid ${checked ? "#4f46e5" : "#cbd5e1"}`,
        background: checked ? "#4f46e5" : "#fff",
        cursor: disabled ? "not-allowed" : "pointer",
        opacity: disabled ? 0.6 : 1,
      }}
    >
      {checked && <Check className="size-3 text-white" strokeWidth={3} />}
    </span>
  );
}
