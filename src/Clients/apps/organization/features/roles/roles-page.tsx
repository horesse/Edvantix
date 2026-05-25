"use client";

import { useState } from "react";

import { Plus, Search } from "lucide-react";

import useRoles from "@workspace/api-hooks/organization/useRoles";
import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { PageLayout } from "@/components/layout/page-layout";
import { useOrganization } from "@/components/organization/provider";

import { CreateRoleDialog } from "./roles-dialogs";
import { RoleCard } from "./roles-role-card";

export function RolesPage() {
  const { currentOrg, canManage } = useOrganization();
  const [search, setSearch] = useState("");
  const [createOpen, setCreateOpen] = useState(false);

  const query = search.trim() ? { search: search.trim() } : undefined;
  const { data, isLoading } = useRoles(currentOrg?.id ?? "", query, {
    enabled: Boolean(currentOrg?.id),
  });

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-16 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  const roles = data?.items ?? [];
  const total = data?.totalCount ?? 0;

  return (
    <PageLayout
      title="Роли и права"
      description="Наборы прав для сотрудников — определяют, что участники видят и могут делать"
      actions={
        canManage ? (
          <Button size="sm" onClick={() => setCreateOpen(true)}>
            <Plus className="size-4" />
            Создать роль
          </Button>
        ) : undefined
      }
    >
      {/* Toolbar */}
      <div className="flex items-center gap-3">
        <div className="relative h-9 w-80">
          <Search className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 size-3.5 -translate-y-1/2" />
          <input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Поиск по названию или описанию"
            className="focus:border-primary focus:ring-primary/20 h-9 w-full rounded-lg border border-slate-200 bg-white pr-3 pl-9 text-sm outline-none focus:ring-3"
          />
        </div>
        {!isLoading && (
          <p className="text-muted-foreground ml-auto text-sm">
            Всего ролей: <strong className="text-foreground">{total}</strong>
          </p>
        )}
      </div>

      {/* List */}
      <div className="flex flex-col gap-3">
        {isLoading
          ? Array.from({ length: 4 }, (_, i) => (
              <Skeleton
                key={`skeleton-${i}`}
                className="h-20 w-full rounded-2xl"
              />
            ))
          : roles.map((role) => (
              <RoleCard key={role.id} role={role} orgId={currentOrg.id} />
            ))}

        {!isLoading && roles.length === 0 && (
          <div className="rounded-2xl border border-dashed border-slate-200 py-16 text-center">
            <p className="text-muted-foreground text-sm">
              {search ? "Роли не найдены" : "Роли пока не созданы"}
            </p>
          </div>
        )}
      </div>

      {canManage && (
        <CreateRoleDialog
          orgId={currentOrg.id}
          open={createOpen}
          onOpenChange={setCreateOpen}
        />
      )}
    </PageLayout>
  );
}
