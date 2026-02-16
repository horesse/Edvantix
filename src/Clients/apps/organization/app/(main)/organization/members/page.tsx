import type { Metadata } from "next";

import { MembersPage } from "@/features/members/members-page";

export const metadata: Metadata = {
  title: "Edvantix - Участники",
};

export default function Page() {
  return <MembersPage />;
}
