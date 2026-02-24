"use client";

import type React from "react";

import Link from "next/link";

import { ShieldAlert } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { useIsAdmin } from "@/hooks/use-realm-roles";
import { signIn, useSession } from "@/lib/auth-client";

type AdminGuardProps = {
  children: React.ReactNode;
};

export function AdminGuard({ children }: AdminGuardProps) {
  const { data: session, isPending } = useSession();
  const isAdmin = useIsAdmin();

  if (isPending) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center">
        <div className="w-full max-w-sm space-y-3">
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-4 w-3/4" />
          <Skeleton className="h-4 w-1/2" />
        </div>
      </div>
    );
  }

  if (!session) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center px-4">
        <div className="space-y-4 text-center">
          <ShieldAlert className="text-muted-foreground mx-auto h-12 w-12" />
          <h2 className="text-xl font-semibold">Authentication required</h2>
          <p className="text-muted-foreground">
            Please sign in to access the admin panel.
          </p>
          <Button onClick={() => void signIn.social({ provider: "keycloak" })}>
            Sign in
          </Button>
        </div>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center px-4">
        <div className="space-y-4 text-center">
          <ShieldAlert className="text-destructive mx-auto h-12 w-12" />
          <h2 className="text-xl font-semibold">Access denied</h2>
          <p className="text-muted-foreground">
            You do not have permission to access the admin panel.
          </p>
          <Button variant="outline" asChild>
            <Link href="/">Back to blog</Link>
          </Button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
