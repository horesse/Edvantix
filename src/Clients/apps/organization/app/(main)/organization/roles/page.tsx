import type { Metadata } from "next";

import { RolesPage } from "@/features/roles/roles-page";

export const metadata: Metadata = {
  title: "Edvantix - Роли и права",
};

export default function Page() {
  return <RolesPage />;
}
