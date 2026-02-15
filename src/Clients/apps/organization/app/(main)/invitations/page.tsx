import type { Metadata } from "next";

import { InvitationsPage } from "@/features/invitations/invitations-page";

export const metadata: Metadata = {
  title: "Edvantix - Приглашения",
};

export default function Page() {
  return <InvitationsPage />;
}
