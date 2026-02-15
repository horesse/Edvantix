import type { Metadata } from "next";

import { OrgSettingsPage } from "@/features/organization/org-settings-page";

export const metadata: Metadata = {
  title: "Edvantix - Настройки организации",
};

export default function Page() {
  return <OrgSettingsPage />;
}
