import type { Metadata } from "next";

import { RoleEditPage } from "@/features/roles/role-edit-page";

export const metadata: Metadata = {
  title: "Edvantix - Редактирование роли",
};

interface Props {
  params: Promise<{ roleId: string }>;
}

export default async function Page({ params }: Props) {
  const { roleId } = await params;

  return <RoleEditPage roleId={roleId} />;
}
