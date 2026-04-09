import type { Metadata } from "next";

import { ProfilesPage } from "@/features/profiles/profiles-page";

export const metadata: Metadata = {
  title: "Edvantix Admin — Профили",
};

export default function Page() {
  return <ProfilesPage />;
}
