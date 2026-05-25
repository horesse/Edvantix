import type { Metadata } from "next";

import { OrgSettingsWithBreadcrumb } from "@/features/organization/settings/org-settings-with-breadcrumb";

export const metadata: Metadata = {
  title: "Edvantix — Настройки организации",
};

export default function Page() {
  return <OrgSettingsWithBreadcrumb />;
}
