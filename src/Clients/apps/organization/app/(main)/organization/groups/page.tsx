import type { Metadata } from "next";

import { GroupsPage } from "@/features/groups/groups-page";

export const metadata: Metadata = {
  title: "Edvantix - Группы",
};

export default function Page() {
  return <GroupsPage />;
}
