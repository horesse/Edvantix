"use client";

import { useEffect } from "react";
import { useSearchParams } from "next/navigation";

import { Loader2 } from "lucide-react";

import { signIn } from "@/lib/auth-client";

export default function LoginPage() {
  const searchParams = useSearchParams();
  const callbackUrl = searchParams.get("callbackUrl") || "/";

  useEffect(() => {
    const login = async () => {
      await signIn.oauth2({
        providerId: "keycloak",
        callbackURL: callbackUrl,
      });
    };

    login();
  }, [callbackUrl]);

  return (
    <div className="space-y-4 text-center">
      <Loader2 className="text-primary mx-auto size-12 animate-spin" />
      <h1 className="text-2xl font-semibold">Перенаправление на страницу входа...</h1>
      <p className="text-muted-foreground">
        Пожалуйста, подождите, пока мы перенаправим вас на страницу авторизации.
      </p>
    </div>
  );
}
