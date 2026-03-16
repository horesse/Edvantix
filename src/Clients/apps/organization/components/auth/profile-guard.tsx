"use client";

import { useEffect } from "react";

import { useRouter } from "next/navigation";

import useOwnProfile from "@workspace/api-hooks/profiles/useOwnProfile";

import { LoadingScreen } from "./loading-screen";

type AxiosLikeError = { response?: { status?: number } };

/**
 * Guards routes that require a completed profile.
 * Redirects to /profile/register when the API returns 404 (profile not found).
 */
export function ProfileGuard({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const router = useRouter();
  const { isLoading, error } = useOwnProfile({ retry: false });

  useEffect(() => {
    if (!isLoading && error) {
      const axiosError = error as AxiosLikeError;
      if (axiosError.response?.status === 404) {
        router.push("/profile/register");
      }
    }
  }, [error, isLoading, router]);

  if (isLoading) {
    return <LoadingScreen stage="profile" />;
  }

  if (error) {
    const axiosError = error as AxiosLikeError;
    if (axiosError.response?.status === 404) {
      // Redirect is in progress via useEffect; show profile stage while navigating.
      return <LoadingScreen stage="profile" />;
    }
  }

  return <>{children}</>;
}
