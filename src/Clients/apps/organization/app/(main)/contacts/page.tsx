import type { Metadata } from "next";

import { ContactsPage } from "@/features/contacts/contacts-page";

export const metadata: Metadata = {
  title: "Edvantix - Контакты организации",
};

export default function Page() {
  return <ContactsPage />;
}
