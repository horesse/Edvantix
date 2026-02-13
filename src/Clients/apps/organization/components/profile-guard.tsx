"use client";

import { useEffect } from "react";

import { useRouter } from "next/navigation";

import useOwnProfile from "@workspace/api-hooks/profiles/useOwnProfile";

import { LoadingScreen } from "./auth-guard";

type AxiosLikeError = { response?: { status?: number } };

export function ProfileGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { data, isLoading, error } = useOwnProfile({
    retry: false,
  });

  useEffect(() => {
    if (!isLoading && error) {
      const axiosError = error as AxiosLikeError;
      if (axiosError.response?.status === 404) {
        router.push("/profile/register");
      }
    }
  }, [error, isLoading, router]);

  if (isLoading) {
    return (
      <LoadingScreen
        title="Загрузка профиля"
        description="Пожалуйста, подождите..."
      />
    );
  }

  if (error) {
    const axiosError = error as AxiosLikeError;
    if (axiosError.response?.status === 404) {
      return (
        <LoadingScreen
          title="Перенаправление"
          description="Перенаправление на страницу регистрации профиля..."
        />
      );
    }
  }

  return <>{children}</>;
}
