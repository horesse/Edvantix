"use client";

import { Mail } from "lucide-react";

import { PageHeader } from "@/components/layout/page-header";

export function InvitationsPage() {
  return (
    <div className="space-y-4">
      <PageHeader title="Приглашения" />
      <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-16">
        <Mail className="text-muted-foreground/40 mb-3 size-10" />
        <p className="text-muted-foreground text-sm font-medium">
          Раздел в разработке
        </p>
        <p className="text-muted-foreground/60 mt-1 text-xs">
          Система приглашений будет доступна в ближайшем обновлении
        </p>
      </div>
    </div>
  );
}
