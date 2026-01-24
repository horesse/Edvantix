"use client";

import { useEffect } from "react";

import { useRouter } from "next/navigation";

import useOwnProfile from "@workspace/api-hooks/profiles/useOwnProfile";
import { Card, CardContent } from "@workspace/ui/components/card";
import { Spinner } from "@workspace/ui/components/spinner";

export function ProfileGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { data, isLoading, error } = useOwnProfile({
    retry: false,
  });

  useEffect(() => {
    if (!isLoading && error) {
      const axiosError = error as { response?: { status?: number } };
      if (axiosError.response?.status === 404) {
        router.push("/profile/register");
      }
    }
  }, [error, isLoading, router]);

  if (isLoading) {
    return (
      <div className="from-background to-muted/20 flex h-screen items-center justify-center bg-linear-to-br">
        <Card
          className="border-muted/50 w-full max-w-md shadow-lg"
          role="status"
          aria-live="polite"
          aria-busy="true"
        >
          <CardContent className="space-y-6 pt-6 pb-6">
            <div className="flex justify-center" aria-hidden="true">
              <Spinner className="size-8" />
            </div>
            <div className="space-y-2 text-center">
              <h1 className="text-foreground text-xl font-semibold">
                Загрузка профиля
              </h1>
              <p className="text-muted-foreground text-sm">
                Пожалуйста, подождите...
              </p>
            </div>
            <span className="sr-only">Загрузка профиля, пожалуйста, подождите</span>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (error) {
    const axiosError = error as { response?: { status?: number } };
    if (axiosError.response?.status === 404) {
      return (
        <div className="from-background to-muted/20 flex h-screen items-center justify-center bg-linear-to-br">
          <Card
            className="border-muted/50 w-full max-w-md shadow-lg"
            role="status"
            aria-live="polite"
            aria-busy="true"
          >
            <CardContent className="space-y-6 pt-6 pb-6">
              <div className="flex justify-center" aria-hidden="true">
                <Spinner className="size-8" />
              </div>
              <div className="space-y-2 text-center">
                <h1 className="text-foreground text-xl font-semibold">
                  Перенаправление
                </h1>
                <p className="text-muted-foreground text-sm">
                  Перенаправление на страницу регистрации профиля...
                </p>
              </div>
              <span className="sr-only">Перенаправление на страницу регистрации</span>
            </CardContent>
          </Card>
        </div>
      );
    }
  }

  return <>{children}</>;
}
