"use client";

import { useEffect, useMemo, useState } from "react";

import { useRouter } from "next/navigation";

import {
  AlertTriangle,
  Check,
  ChevronDown,
  Lock,
  Search,
  Trash2,
} from "lucide-react";
import { toast } from "sonner";

import useRole from "@workspace/api-hooks/company/useRole";
import useUpdateRole from "@workspace/api-hooks/company/useUpdateRole";
import type { FeatureDto } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";

import { PageLayout } from "@/components/layout/page-layout";
import { useOrganization } from "@/components/organization/provider";

import { FEATURE_META, getRoleAvatarColors } from "./roles-constants";
import { DeleteRoleDialog } from "./roles-dialogs";

interface RoleEditPageProps {
  roleId: string;
}

export function RoleEditPage({ roleId }: Readonly<RoleEditPageProps>) {
  const router = useRouter();
  const { currentOrg, canManage } = useOrganization();
  const orgId = currentOrg?.id ?? "";

  const { data: role, isLoading } = useRole(orgId, roleId);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [permSearch, setPermSearch] = useState("");
  const [collapsed, setCollapsed] = useState<Set<string>>(new Set());
  const [deleteOpen, setDeleteOpen] = useState(false);

  // Sync form fields when role data loads
  useEffect(() => {
    if (role) {
      setName(role.name);
      setDescription(role.description ?? "");
    }
  }, [role]);

  const hasChanges =
    role !== undefined &&
    (name.trim() !== role.name ||
      (description.trim() || null) !== role.description);

  const update = useUpdateRole({
    onSuccess: () => toast.success("Роль сохранена"),
    onError: () => toast.error("Не удалось сохранить роль"),
  });

  function handleSave() {
    if (!role) return;
    update.mutate({
      orgId,
      roleId: role.id,
      request: {
        name: name.trim(),
        description: description.trim() || null,
      },
    });
  }

  function handleReset() {
    if (!role) return;
    setName(role.name);
    setDescription(role.description ?? "");
  }

  const q = permSearch.trim().toLowerCase();
  const visibleFeatures = useMemo(
    () =>
      (role?.features ?? [])
        .map((feat) => ({
          feat,
          permissions: q
            ? feat.permissions.filter(
                (p) =>
                  p.name.toLowerCase().includes(q) ||
                  feat.name.toLowerCase().includes(q),
              )
            : feat.permissions,
        }))
        .filter(({ permissions }) => permissions.length > 0),
    [role, q],
  );

  const totalGranted = useMemo(
    () =>
      (role?.features ?? []).reduce(
        (sum, f) => sum + f.permissions.filter((p) => p.isActive).length,
        0,
      ),
    [role],
  );

  const pct =
    role && role.totalPermissionsCount > 0
      ? Math.round((totalGranted / role.totalPermissionsCount) * 100)
      : 0;

  function toggleCollapse(code: string) {
    setCollapsed((prev) => {
      const next = new Set(prev);
      if (next.has(code)) next.delete(code);
      else next.add(code);

      return next;
    });
  }

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-16 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  const colors = role
    ? getRoleAvatarColors(role.name)
    : { bg: "#f1f5f9", fg: "#475569" };

  return (
    <>
      <PageLayout
        back={{ href: "/organization/roles", label: "Роли и права" }}
        header={
          isLoading ? (
            <RoleEditHeaderSkeleton />
          ) : role ? (
            <div className="space-y-3">
              {/* Back */}
              <Button
                variant="ghost"
                size="sm"
                className="text-muted-foreground -ml-2 h-7 gap-1.5 px-2 text-xs"
                onClick={() => router.push("/organization/roles")}
              >
                ← Роли и права
              </Button>
              {/* Header */}
              <div className="flex items-center gap-4">
                <div
                  className="flex size-12 shrink-0 items-center justify-center rounded-xl text-lg font-bold"
                  style={{ background: colors.bg, color: colors.fg }}
                >
                  {(name || role.name).charAt(0)}
                </div>
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2">
                    <h1 className="text-xl font-bold tracking-tight">
                      {role.name}
                    </h1>
                    {role.isSystem && (
                      <span className="inline-flex items-center gap-1 rounded-full bg-slate-100 px-2 py-0.5 text-[11px] font-medium text-slate-500">
                        <Lock className="size-2.5" />
                        системная
                      </span>
                    )}
                  </div>
                  <p className="text-muted-foreground text-sm">
                    {role.membersCount} участников · {totalGranted} из{" "}
                    {role.totalPermissionsCount} прав ({pct}%)
                  </p>
                </div>
                {!role.isSystem && canManage && (
                  <Button
                    variant="outline"
                    size="sm"
                    className="text-destructive hover:text-destructive border-red-200 hover:bg-red-50"
                    onClick={() => setDeleteOpen(true)}
                  >
                    <Trash2 className="size-4" />
                    Удалить роль
                  </Button>
                )}
              </div>
            </div>
          ) : null
        }
      >
        {isLoading && (
          <div className="space-y-5">
            <Skeleton className="h-36 w-full rounded-2xl" />
            <Skeleton className="h-96 w-full rounded-2xl" />
          </div>
        )}

        {role && (
          <div className="space-y-5 pb-24">
            {/* Owner read-only banner */}
            {role.isOwner && (
              <div className="flex items-start gap-2.5 rounded-xl border border-amber-200/60 bg-amber-50/60 px-4 py-3">
                <AlertTriangle className="mt-0.5 size-4 shrink-0 text-amber-700" />
                <p className="text-sm leading-relaxed text-amber-800">
                  Роль «{role.name}» имеет полный доступ ко всем разделам
                  системы и не может быть изменена.
                </p>
              </div>
            )}

            {/* Basic info */}
            <section className="rounded-2xl border border-slate-200 bg-white p-5">
              <h2 className="mb-4 text-sm font-semibold">Основные сведения</h2>
              <div className="space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="role-name">
                    Название роли <span className="text-destructive">*</span>
                  </Label>
                  <Input
                    id="role-name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    disabled={role.isOwner || !canManage}
                    maxLength={100}
                  />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="role-desc">
                    Описание
                    <span className="text-muted-foreground ml-1 text-xs">
                      — кому назначается эта роль
                    </span>
                  </Label>
                  <Textarea
                    id="role-desc"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    disabled={role.isOwner || !canManage}
                    rows={3}
                    maxLength={300}
                  />
                </div>
              </div>
            </section>

            {/* Permissions */}
            <PermissionsSection
              features={role.features}
              visibleFeatures={visibleFeatures}
              search={permSearch}
              onSearchChange={setPermSearch}
              collapsed={collapsed}
              onToggleCollapse={toggleCollapse}
            />
          </div>
        )}
      </PageLayout>

      {/* Sticky save bar */}
      {role && (
        <SaveBar
          visible={hasChanges && !role.isOwner && canManage}
          saving={update.isPending}
          onSave={handleSave}
          onReset={handleReset}
        />
      )}

      {/* Delete dialog */}
      {role && (
        <DeleteRoleDialog
          orgId={orgId}
          roleId={role.id}
          roleName={role.name}
          open={deleteOpen}
          onOpenChange={setDeleteOpen}
          onDeleted={() => router.push("/organization/roles")}
        />
      )}
    </>
  );
}

// ── Permissions section ────────────────────────────────────────────────────────

interface PermissionsSectionProps {
  features: readonly FeatureDto[];
  visibleFeatures: {
    feat: FeatureDto;
    permissions: FeatureDto["permissions"];
  }[];
  search: string;
  onSearchChange: (v: string) => void;
  collapsed: Set<string>;
  onToggleCollapse: (code: string) => void;
}

function PermissionsSection({
  features,
  visibleFeatures,
  search,
  onSearchChange,
  collapsed,
  onToggleCollapse,
}: Readonly<PermissionsSectionProps>) {
  return (
    <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white">
      {/* Header */}
      <div className="flex items-center gap-4 border-b border-slate-100 px-5 py-4">
        <div className="min-w-0 flex-1">
          <h2 className="text-sm font-semibold">Права доступа</h2>
          <p className="text-muted-foreground mt-0.5 text-xs">
            Отображает, что участник с этой ролью может делать
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

      {/* Feature groups */}
      <div>
        {visibleFeatures.map(({ feat, permissions }) => {
          const meta = FEATURE_META[feat.code];
          const Icon = meta?.icon;
          const granted = feat.permissions.filter((p) => p.isActive).length;
          const isCollapsed = collapsed.has(feat.code);

          return (
            <div key={feat.code} className="border-t border-slate-50">
              {/* Feature header */}
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
                  {granted} / {feat.permissions.length}
                </span>
                {/* Feature-level toggle (read-only indicator) */}
                <FeatureToggle
                  granted={granted}
                  total={feat.permissions.length}
                />
              </div>

              {/* Permission rows */}
              {!isCollapsed && (
                <div className="px-5 pt-1 pb-3 pl-[70px]">
                  {permissions.map((perm) => (
                    <div
                      key={perm.id}
                      className="flex items-center gap-3 rounded-lg px-2.5 py-2"
                    >
                      {/* Checkbox (read-only) */}
                      <span
                        className="inline-flex size-[18px] shrink-0 items-center justify-center rounded"
                        style={{
                          border: `1.5px solid ${perm.isActive ? "#4f46e5" : "#cbd5e1"}`,
                          background: perm.isActive ? "#4f46e5" : "#fff",
                        }}
                      >
                        {perm.isActive && (
                          <Check
                            className="size-3 text-white"
                            strokeWidth={3}
                          />
                        )}
                      </span>
                      <span
                        className="text-[13px]"
                        style={{
                          color: perm.isActive ? "#0f172a" : "#475569",
                          fontWeight: perm.isActive ? 500 : 400,
                        }}
                      >
                        {perm.name}
                      </span>
                      <code className="ml-auto rounded bg-slate-50 px-2 py-0.5 font-mono text-[11px] text-slate-400">
                        {perm.code}
                      </code>
                    </div>
                  ))}
                </div>
              )}
            </div>
          );
        })}

        {features.length > 0 && visibleFeatures.length === 0 && (
          <p className="text-muted-foreground px-5 py-8 text-center text-sm">
            Права не найдены
          </p>
        )}

        {features.length === 0 && (
          <p className="text-muted-foreground px-5 py-8 text-center text-sm">
            Права не настроены
          </p>
        )}
      </div>
    </section>
  );
}

// ── Feature-level read-only toggle ────────────────────────────────────────────

function FeatureToggle({ granted, total }: { granted: number; total: number }) {
  const allOn = granted === total && total > 0;
  const someOn = granted > 0 && !allOn;
  const bg = allOn ? "#4f46e5" : someOn ? "#818cf8" : "#cbd5e1";

  return (
    <div
      className="relative shrink-0 rounded-full"
      style={{ width: 40, height: 22, background: bg }}
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
    </div>
  );
}

// ── Save bar ──────────────────────────────────────────────────────────────────

interface SaveBarProps {
  visible: boolean;
  saving: boolean;
  onSave: () => void;
  onReset: () => void;
}

function SaveBar({ visible, saving, onSave, onReset }: Readonly<SaveBarProps>) {
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

// ── Loading skeleton ──────────────────────────────────────────────────────────

function RoleEditHeaderSkeleton() {
  return (
    <div className="flex items-center gap-4">
      <Skeleton className="size-12 rounded-xl" />
      <div className="space-y-2">
        <Skeleton className="h-5 w-40" />
        <Skeleton className="h-4 w-56" />
      </div>
    </div>
  );
}
