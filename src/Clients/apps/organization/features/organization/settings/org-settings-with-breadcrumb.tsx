"use client";

import { PageBreadcrumb } from "@/components/layout/page-breadcrumb";
import { PageLayout } from "@/components/layout/page-layout";

import { OrgSettingsPage } from "./org-settings-page";

const BREADCRUMB_ITEMS = [
  { label: "Настройки", href: "/organization/settings" },
] as const;

export function OrgSettingsWithBreadcrumb() {
  return (
    <PageLayout
      header={
        <PageBreadcrumb
          items={[...BREADCRUMB_ITEMS]}
          currentPage="Организация"
        />
      }
    >
      <OrgSettingsPage />
    </PageLayout>
  );
}
