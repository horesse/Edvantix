"use client";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import { Card, CardContent } from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { ContactsSection } from "@/features/settings/contacts-section";

function ContactsSkeleton() {
  return (
    <Card>
      <CardContent className="space-y-4 pt-6">
        <div className="flex items-center justify-between">
          <div className="space-y-1">
            <Skeleton className="h-5 w-28" />
            <Skeleton className="h-4 w-40" />
          </div>
          <Skeleton className="h-8 w-24" />
        </div>
        <div className="divide-border/50 divide-y rounded-lg border">
          {Array.from({ length: 2 }).map((_, i) => (
            <div key={i} className="flex items-center gap-3 px-4 py-3">
              <Skeleton className="size-9 rounded-lg" />
              <div className="space-y-1.5">
                <Skeleton className="h-4 w-40" />
                <Skeleton className="h-3 w-24" />
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}

export default function ContactsSettingsPage() {
  const { data: profile, isLoading } = useProfileDetails();

  if (isLoading) {
    return <ContactsSkeleton />;
  }

  if (!profile) {
    return null;
  }

  return (
    <Card>
      <CardContent className="pt-6">
        <ContactsSection profile={profile} />
      </CardContent>
    </Card>
  );
}
