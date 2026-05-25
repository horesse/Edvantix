import type { Metadata } from "next";

import { SettingsPage } from "@/features/settings";

export const metadata: Metadata = {
  title: "Edvantix - Настройки",
};

export default function Page() {
  return <SettingsPage />;
}
