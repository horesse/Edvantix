import type { Metadata } from "next";

import { ProfileSettings } from "@/features/profile/settings/profile-settings";

export const metadata: Metadata = {
  title: "Профиль",
};

export default function ProfileSettingsPage() {
  return <ProfileSettings />;
}
