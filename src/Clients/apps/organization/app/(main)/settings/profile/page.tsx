"use client";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { AvatarSection } from "@/features/settings/avatar-section";
import { ProfileForm } from "@/features/settings/profile-form";

function ProfileSkeleton() {
  return (
    <div className="space-y-6">
      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center gap-4">
            <Skeleton className="size-20 rounded-full sm:size-24" />
            <div className="space-y-2">
              <Skeleton className="h-5 w-36" />
              <Skeleton className="h-4 w-20" />
            </div>
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <Skeleton className="h-5 w-48" />
          <Skeleton className="h-4 w-64" />
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
          </div>
          <Skeleton className="h-10 w-full" />
          <div className="grid gap-4 sm:grid-cols-2">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default function ProfileSettingsPage() {
  const { data: profile, isLoading } = useProfileDetails();

  if (isLoading) {
    return <ProfileSkeleton />;
  }

  if (!profile) {
    return null;
  }

  const fullName = [profile.lastName, profile.firstName, profile.middleName]
    .filter(Boolean)
    .join(" ");

  return (
    <div className="space-y-6">
      <Card>
        <CardContent className="pt-6">
          <AvatarSection avatarUrl={profile.avatarUrl} fullName={fullName} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Личная информация</CardTitle>
          <CardDescription>Основные данные вашего профиля</CardDescription>
        </CardHeader>
        <CardContent>
          <ProfileForm profile={profile} />
        </CardContent>
      </Card>
    </div>
  );
}
