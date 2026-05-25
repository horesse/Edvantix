"use client";

import useOrganization from "@workspace/api-hooks/organization/useOrganization";

import { useOrganization as useOrgContext } from "@/components/organization/provider";

import { OrgSettingsSkeleton } from "./components/org-settings-skeleton";
import { OrgSettingsForm } from "./org-settings-form";

export function OrgSettingsPage() {
  const { currentOrg, canManage } = useOrgContext();
  const orgId = currentOrg?.id ?? "";
  const { data: org, isLoading } = useOrganization(orgId);

  if (!canManage) {
    return (
      <p className="text-muted-foreground py-8 text-center text-sm">
        Доступ запрещён. Только владелец или менеджер могут изменять настройки.
      </p>
    );
  }

  if (isLoading) return <OrgSettingsSkeleton />;
  if (!org)
    return (
      <p className="text-muted-foreground py-8 text-center text-sm">
        Выберите организацию
      </p>
    );

  return <OrgSettingsForm org={org} />;
}
