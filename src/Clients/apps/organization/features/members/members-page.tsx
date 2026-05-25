"use client";

import { useState } from "react";

import { UserPlus } from "lucide-react";

import type { OrganizationMemberDto } from "@workspace/types/organization";
import { Button } from "@workspace/ui/components/button";

import { PageLayout } from "@/components/layout/page-layout";
import { useOrganization } from "@/components/organization/provider";

import {
  AddMemberDialog,
  ChangeRoleDialog,
  RemoveMemberDialog,
} from "./members-dialogs";
import { MembersKpiCards } from "./members-kpi-cards";
import { MembersTable } from "./members-table";

export function MembersPage() {
  const { currentOrg, canManage } = useOrganization();

  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [changeRoleMember, setChangeRoleMember] =
    useState<OrganizationMemberDto | null>(null);
  const [removeMember, setRemoveMember] =
    useState<OrganizationMemberDto | null>(null);

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-16 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  const orgId = currentOrg.id;
  const orgName = currentOrg.shortName ?? currentOrg.fullLegalName;

  function handleSelect(id: string, checked: boolean) {
    setSelected((prev) => {
      const next = new Set(prev);
      if (checked) next.add(id);
      else next.delete(id);
      return next;
    });
  }

  function handleSelectAll(checked: boolean, ids: string[]) {
    setSelected(checked ? new Set(ids) : new Set());
  }

  return (
    <PageLayout
      title="Участники"
      description={`Сотрудники и роли в организации «${orgName}»`}
      actions={
        canManage ? (
          <Button size="sm" onClick={() => setAddDialogOpen(true)}>
            <UserPlus className="size-4" />
            Пригласить участника
          </Button>
        ) : undefined
      }
    >
      <MembersKpiCards orgId={orgId} />

      <MembersTable
        orgId={orgId}
        canManage={canManage}
        selected={selected}
        onSelect={handleSelect}
        onSelectAll={handleSelectAll}
        onClearSelection={() => setSelected(new Set())}
        onChangeRole={setChangeRoleMember}
        onRemove={setRemoveMember}
      />

      <AddMemberDialog
        orgId={orgId}
        open={addDialogOpen}
        onOpenChange={setAddDialogOpen}
      />
      <ChangeRoleDialog
        orgId={orgId}
        member={changeRoleMember}
        onClose={() => setChangeRoleMember(null)}
      />
      <RemoveMemberDialog
        orgId={orgId}
        member={removeMember}
        onClose={() => setRemoveMember(null)}
      />
    </PageLayout>
  );
}
