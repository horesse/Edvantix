"use client";

import { useEffect, useState } from "react";

import { useRouter } from "next/navigation";

import { Card, CardContent } from "@workspace/ui/components/card";
import { Spinner } from "@workspace/ui/components/spinner";

import { useUserContext } from "@/hooks/use-user-context";
import { getAccessToken, signIn } from "@/lib/auth-client";
import { AUTH } from "@/lib/constants";

function LoadingScreen({
  title,
  description,
}: {
  title: string;
  description: string;
}) {
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
            <h1 className="text-foreground text-xl font-semibold">{title}</h1>
            <p className="text-muted-foreground text-sm">{description}</p>
          </div>
          <span className="sr-only">{description}</span>
        </CardContent>
      </Card>
    </div>
  );
}

export function AuthGuard({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useUserContext();
  const router = useRouter();
  const [tokenError, setTokenError] = useState<string | null>(null);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      signIn.social({
        provider: AUTH.PROVIDER,
        callbackURL: AUTH.CALLBACK_URL,
      });
    }
  }, [isAuthenticated, isLoading, router]);

  useEffect(() => {
    let isMounted = true;

    const fetchAndStoreToken = async () => {
      if (!isAuthenticated) return;

      try {
        const result = await getAccessToken({ providerId: AUTH.PROVIDER });

        if (!isMounted) return;

        if (result.data?.accessToken) {
          if (typeof window !== "undefined") {
            localStorage.setItem("access_token", result.data.accessToken);
          }
        } else {
          setTokenError("Failed to retrieve access token");
          console.error("Access token not found in response");
        }
      } catch (error) {
        if (!isMounted) return;

        const errorMessage =
          error instanceof Error ? error.message : "Unknown error";
        setTokenError(errorMessage);
        console.error("Error fetching access token:", error);
      }
    };

    fetchAndStoreToken();

    return () => {
      isMounted = false;
    };
  }, [isAuthenticated]);

  if (isLoading) {
    return (
      <LoadingScreen
        title="Loading Dashboard"
        description="Please wait while we verify your credentials..."
      />
    );
  }

  if (!isAuthenticated) {
    return (
      <LoadingScreen
        title="Authentication Required"
        description="Redirecting to secure login..."
      />
    );
  }

  if (tokenError) {
    console.warn("Token error:", tokenError);
  }

  return <>{children}</>;
}
