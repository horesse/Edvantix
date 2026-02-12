"use client";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import { Card, CardContent } from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { EducationSection } from "@/features/settings/education-section";
import { EmploymentSection } from "@/features/settings/employment-section";

function CareerSkeleton() {
  return (
    <div className="space-y-6">
      <Card>
        <CardContent className="space-y-4 pt-6">
          <div className="flex items-center justify-between">
            <div className="space-y-1">
              <Skeleton className="h-5 w-32" />
              <Skeleton className="h-4 w-48" />
            </div>
            <Skeleton className="h-8 w-24" />
          </div>
          <Skeleton className="h-28 w-full rounded-lg" />
        </CardContent>
      </Card>
      <Card>
        <CardContent className="space-y-4 pt-6">
          <div className="flex items-center justify-between">
            <div className="space-y-1">
              <Skeleton className="h-5 w-32" />
              <Skeleton className="h-4 w-48" />
            </div>
            <Skeleton className="h-8 w-24" />
          </div>
          <Skeleton className="h-28 w-full rounded-lg" />
        </CardContent>
      </Card>
    </div>
  );
}

export default function CareerSettingsPage() {
  const { data: profile, isLoading } = useProfileDetails();

  if (isLoading) {
    return <CareerSkeleton />;
  }

  if (!profile) {
    return null;
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardContent className="pt-6">
          <EmploymentSection profile={profile} />
        </CardContent>
      </Card>

      <Card>
        <CardContent className="pt-6">
          <EducationSection profile={profile} />
        </CardContent>
      </Card>
    </div>
  );
}
