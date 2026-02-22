import type { Metadata } from "next";

import { GroupDetailPage } from "@/features/groups/group-detail-page";

export const metadata: Metadata = {
  title: "Edvantix - Группа",
};

type Props = {
  params: Promise<{ groupId: string }>;
};

export default async function Page({ params }: Props) {
  const { groupId } = await params;
  return <GroupDetailPage groupId={groupId} />;
}
