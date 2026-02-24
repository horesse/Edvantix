"use client";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";

import { PageHeader } from "@/components/page-header";

import { ProfileEditor } from "./profile-editor";
import { ProfileSettingsSkeleton } from "./profile-settings-ui";

export function ProfileSettings() {
  const { data: profile, isLoading } = useProfileDetails();

  return (
    <div className="space-y-6">
      <PageHeader title="Профиль" />
      {isLoading || !profile ? (
        <ProfileSettingsSkeleton />
      ) : (
        <ProfileEditor profile={profile} />
      )}
    </div>
  );
}
