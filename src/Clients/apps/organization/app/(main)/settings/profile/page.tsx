import type { Metadata } from "next";

import { ProfileSettings } from "@/features/settings/profile-settings";

export const metadata: Metadata = {
  title: "Профиль",
};

export default function ProfileSettingsPage() {
  return <ProfileSettings />;
}
