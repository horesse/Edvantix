"use client";

import { Check, Clock, Mail, X } from "lucide-react";
import { toast } from "sonner";

import useAcceptInvitation from "@workspace/api-hooks/company/useAcceptInvitation";
import useDeclineInvitation from "@workspace/api-hooks/company/useDeclineInvitation";
import useMyInvitations from "@workspace/api-hooks/company/useMyInvitations";
import type { InvitationModel } from "@workspace/types/company";
import { InvitationStatus } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";

import {
  organizationRoleLabels,
  parseOrganizationRole,
} from "@/lib/company-options";

export function IncomingInvitationsSection() {
  const { data: invitations = [], isLoading } = useMyInvitations();

  const pending = invitations.filter(
    (inv) => inv.status === InvitationStatus.Pending,
  );

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Входящие приглашения</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <Skeleton className="h-16 w-full" />
          <Skeleton className="h-16 w-full" />
        </CardContent>
      </Card>
    );
  }

  if (pending.length === 0) return null;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Входящие приглашения</CardTitle>
        <CardDescription>
          Приглашения в организации, ожидающие вашего ответа
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
        {pending.map((invitation) => (
          <IncomingInvitationCard key={invitation.id} invitation={invitation} />
        ))}
      </CardContent>
    </Card>
  );
}

function IncomingInvitationCard({
  invitation,
}: {
  invitation: InvitationModel;
}) {
  const acceptMutation = useAcceptInvitation({
    onSuccess: () => toast.success("Приглашение принято"),
    onError: () => toast.error("Не удалось принять приглашение"),
  });

  const declineMutation = useDeclineInvitation({
    onSuccess: () => toast.success("Приглашение отклонено"),
    onError: () => toast.error("Не удалось отклонить приглашение"),
  });

  const isPending = acceptMutation.isPending || declineMutation.isPending;
  const role = parseOrganizationRole(String(invitation.role));
  const roleLabel = role
    ? organizationRoleLabels[role]
    : String(invitation.role);
  const expiresAt = new Date(invitation.expiresAt).toLocaleDateString("ru-RU");

  return (
    <div className="flex items-center gap-3 rounded-md border p-3">
      <div className="bg-primary/10 flex size-8 shrink-0 items-center justify-center rounded-md">
        <Mail className="text-primary size-4" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="text-sm font-medium">
          {invitation.organizationName ?? "Организация"}
        </p>
        <div className="text-muted-foreground flex items-center gap-1.5 text-xs">
          <span>{roleLabel}</span>
          <span className="text-muted-foreground/40">·</span>
          <span className="flex items-center gap-1">
            <Clock className="size-3" />
            до {expiresAt}
          </span>
        </div>
      </div>
      <div className="flex gap-1.5">
        <Button
          size="sm"
          variant="ghost"
          disabled={isPending}
          onClick={() => declineMutation.mutate(invitation.token)}
        >
          <X className="size-3.5" />
          <span className="hidden sm:inline">Отклонить</span>
        </Button>
        <Button
          size="sm"
          disabled={isPending}
          onClick={() => acceptMutation.mutate(invitation.token)}
        >
          <Check className="size-3.5" />
          <span className="hidden sm:inline">Принять</span>
        </Button>
      </div>
    </div>
  );
}
