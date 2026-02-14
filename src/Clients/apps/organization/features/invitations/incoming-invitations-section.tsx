"use client";

import { Check, Clock, Mail, X } from "lucide-react";
import { toast } from "sonner";

import useAcceptInvitation from "@workspace/api-hooks/company/useAcceptInvitation";
import useDeclineInvitation from "@workspace/api-hooks/company/useDeclineInvitation";
import useMyInvitations from "@workspace/api-hooks/company/useMyInvitations";
import type { InvitationModel } from "@workspace/types/company";
import { InvitationStatus } from "@workspace/types/company";
import { Badge } from "@workspace/ui/components/badge";
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
  invitationStatusLabels,
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
          <CardTitle className="text-base">Входящие приглашения</CardTitle>
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
        <CardTitle className="text-base">Входящие приглашения</CardTitle>
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
    <div className="flex items-center gap-4 rounded-lg border p-4">
      <div className="bg-primary/10 flex size-10 shrink-0 items-center justify-center rounded-lg">
        <Mail className="text-primary size-5" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="font-medium">
          {invitation.organizationName ?? "Организация"}
        </p>
        <div className="text-muted-foreground flex items-center gap-2 text-sm">
          <span>Роль: {roleLabel}</span>
          <span className="text-muted-foreground/50">·</span>
          <span className="flex items-center gap-1">
            <Clock className="size-3" />
            до {expiresAt}
          </span>
        </div>
      </div>
      <div className="flex gap-2">
        <Button
          size="sm"
          variant="outline"
          disabled={isPending}
          onClick={() => declineMutation.mutate(invitation.token)}
        >
          <X className="size-4" />
          <span className="hidden sm:inline">Отклонить</span>
        </Button>
        <Button
          size="sm"
          disabled={isPending}
          onClick={() => acceptMutation.mutate(invitation.token)}
        >
          <Check className="size-4" />
          <span className="hidden sm:inline">Принять</span>
        </Button>
      </div>
    </div>
  );
}
