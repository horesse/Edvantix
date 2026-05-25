"use client";

import { useEffect, useMemo, useState } from "react";

import { useRouter } from "next/navigation";

import { AlertTriangle } from "lucide-react";
import { toast } from "sonner";

import useRole from "@workspace/api-hooks/organization/useRole";
import useUpdateRole from "@workspace/api-hooks/organization/useUpdateRole";
import type { FeatureDto } from "@workspace/types/organization";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";

import { PageLayout } from "@/components/layout/page-layout";
import { useOrganization } from "@/components/organization/provider";

import { RoleEditHeader, RoleEditHeaderSkeleton } from "./role-edit-header";
import { PermissionsSection } from "./role-edit-permissions";
import { SaveBar } from "./role-edit-save-bar";
import { getRoleAvatarColors } from "./roles-constants";
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
  const [activePerms, setActivePerms] = useState<Set<string>>(new Set());
  const [permSearch, setPermSearch] = useState("");
  const [collapsed, setCollapsed] = useState<Set<string>>(new Set());
  const [deleteOpen, setDeleteOpen] = useState(false);

  useEffect(() => {
    if (role) {
      setName(role.name);
      setDescription(role.description ?? "");
      setActivePerms(
        new Set(
          role.features.flatMap((f) =>
            f.permissions.filter((p) => p.isActive).map((p) => p.id),
          ),
        ),
      );
    }
  }, [role]);

  const allPermIds = useMemo(
    () => role?.features.flatMap((f) => f.permissions.map((p) => p.id)) ?? [],
    [role],
  );

  const hasChanges = useMemo(() => {
    if (!role) return false;
    const origActive = new Set(
      role.features.flatMap((f) =>
        f.permissions.filter((p) => p.isActive).map((p) => p.id),
      ),
    );
    return (
      name.trim() !== role.name ||
      (description.trim() || null) !== role.description ||
      activePerms.size !== origActive.size ||
      [...activePerms].some((id) => !origActive.has(id))
    );
  }, [role, name, description, activePerms]);

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
        permissionIds: [...activePerms],
      },
    });
  }

  function handleReset() {
    if (!role) return;
    setName(role.name);
    setDescription(role.description ?? "");
    setActivePerms(
      new Set(
        role.features.flatMap((f) =>
          f.permissions.filter((p) => p.isActive).map((p) => p.id),
        ),
      ),
    );
  }

  function togglePerm(permId: string) {
    setActivePerms((prev) => {
      const next = new Set(prev);
      if (next.has(permId)) next.delete(permId);
      else next.add(permId);
      return next;
    });
  }

  function toggleFeature(feat: FeatureDto) {
    const ids = feat.permissions.map((p) => p.id);
    const allOn = ids.every((id) => activePerms.has(id));
    setActivePerms((prev) => {
      const next = new Set(prev);
      if (allOn) ids.forEach((id) => next.delete(id));
      else ids.forEach((id) => next.add(id));
      return next;
    });
  }

  function toggleCollapse(code: string) {
    setCollapsed((prev) => {
      const next = new Set(prev);
      if (next.has(code)) next.delete(code);
      else next.add(code);
      return next;
    });
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

  const totalGranted = activePerms.size;
  const pct =
    allPermIds.length > 0
      ? Math.round((totalGranted / allPermIds.length) * 100)
      : 0;

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
  const readonly = !canManage || role?.isOwner;

  const header = isLoading ? (
    <RoleEditHeaderSkeleton />
  ) : role ? (
    <RoleEditHeader
      role={role}
      name={name}
      totalGranted={totalGranted}
      totalPerms={allPermIds.length}
      pct={pct}
      colors={colors}
      canManage={canManage ?? false}
      onBack={() => router.push("/organization/roles")}
      onDeleteOpen={() => setDeleteOpen(true)}
    />
  ) : null;

  return (
    <>
      <PageLayout header={header}>
        {isLoading && (
          <div className="space-y-5">
            <Skeleton className="h-36 w-full rounded-2xl" />
            <Skeleton className="h-96 w-full rounded-2xl" />
          </div>
        )}

        {role && (
          <div className="space-y-5 pb-24">
            {role.isOwner && (
              <div className="flex items-start gap-2.5 rounded-xl border border-amber-200/60 bg-amber-50/60 px-4 py-3">
                <AlertTriangle className="mt-0.5 size-4 shrink-0 text-amber-700" />
                <p className="text-sm leading-relaxed text-amber-800">
                  Роль «{role.name}» имеет полный доступ ко всем разделам
                  системы и не может быть изменена.
                </p>
              </div>
            )}

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
                    disabled={readonly}
                    maxLength={100}
                  />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="role-desc">
                    Описание{" "}
                    <span className="text-muted-foreground ml-1 text-xs">
                      — кому назначается эта роль
                    </span>
                  </Label>
                  <Textarea
                    id="role-desc"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    disabled={readonly}
                    rows={3}
                    maxLength={300}
                  />
                </div>
              </div>
            </section>

            <PermissionsSection
              visibleFeatures={visibleFeatures}
              activePerms={activePerms}
              search={permSearch}
              onSearchChange={setPermSearch}
              collapsed={collapsed}
              onToggleCollapse={toggleCollapse}
              onTogglePerm={togglePerm}
              onToggleFeature={toggleFeature}
              readonly={readonly ?? false}
            />
          </div>
        )}
      </PageLayout>

      <SaveBar
        visible={(hasChanges && !role?.isOwner && canManage) ?? false}
        saving={update.isPending}
        onSave={handleSave}
        onReset={handleReset}
      />

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
