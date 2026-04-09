import type { Metadata } from "next";

import { ProfileEditPage } from "@/features/profiles/profile-edit-page";

export const metadata: Metadata = {
  title: "Edvantix Admin — Редактирование профиля",
};

export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  return <ProfileEditPage profileId={id} />;
}
