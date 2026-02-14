"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Clock, Loader2, Plus, Trash2, X } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useCancelInvitation from "@workspace/api-hooks/company/useCancelInvitation";
import useCreateInvitation from "@workspace/api-hooks/company/useCreateInvitation";
import usePendingInvitations from "@workspace/api-hooks/company/usePendingInvitations";
import type { InvitationModel } from "@workspace/types/company";
import { OrganizationRole } from "@workspace/types/company";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@workspace/ui/components/tabs";
import {
  type CreateInvitationInput,
  createInvitationSchema,
} from "@workspace/validations/company";

import { useOrganization } from "@/components/organization-provider";
import { organizationRoleLabels } from "@/lib/company-options";

import { IncomingInvitationsSection } from "./incoming-invitations-section";

export function InvitationsPage() {
  const { currentOrg, canManage } = useOrganization();

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-2xl font-bold">Приглашения</h1>
        <p className="text-muted-foreground text-sm">
          Управление приглашениями организации
        </p>
      </div>

      <Tabs defaultValue={canManage ? "outgoing" : "incoming"}>
        <TabsList>
          {canManage && (
            <TabsTrigger value="outgoing">Отправленные</TabsTrigger>
          )}
          <TabsTrigger value="incoming">Входящие</TabsTrigger>
        </TabsList>
        {canManage && (
          <TabsContent value="outgoing" className="mt-4">
            <OutgoingInvitations orgId={currentOrg.id} />
          </TabsContent>
        )}
        <TabsContent value="incoming" className="mt-4">
          <IncomingInvitationsSection />
        </TabsContent>
      </Tabs>
    </div>
  );
}

function OutgoingInvitations({ orgId }: { orgId: number }) {
  const [createOpen, setCreateOpen] = useState(false);
  const { data: invitations = [], isLoading } = usePendingInvitations(orgId);

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Button onClick={() => setCreateOpen(true)}>
          <Plus className="size-4" />
          Пригласить
        </Button>
      </div>

      {isLoading ? (
        <div className="space-y-2">
          <Skeleton className="h-16 w-full" />
          <Skeleton className="h-16 w-full" />
        </div>
      ) : invitations.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
          <p className="text-muted-foreground text-sm">
            Нет ожидающих приглашений
          </p>
        </div>
      ) : (
        <div className="space-y-2">
          {invitations.map((inv) => (
            <InvitationRow key={inv.id} invitation={inv} orgId={orgId} />
          ))}
        </div>
      )}

      <CreateInvitationDialog
        orgId={orgId}
        open={createOpen}
        onOpenChange={setCreateOpen}
      />
    </div>
  );
}

function InvitationRow({
  invitation,
  orgId,
}: {
  invitation: InvitationModel;
  orgId: number;
}) {
  const cancelMutation = useCancelInvitation({
    onSuccess: () => toast.success("Приглашение отменено"),
    onError: () => toast.error("Не удалось отменить приглашение"),
  });

  const expiresAt = new Date(invitation.expiresAt).toLocaleDateString("ru-RU");
  const target =
    invitation.inviteeEmail ?? `Профиль #${invitation.inviteeProfileId}`;

  return (
    <div className="flex items-center gap-4 rounded-lg border p-4">
      <div className="min-w-0 flex-1">
        <p className="font-medium">{target}</p>
        <div className="text-muted-foreground flex items-center gap-2 text-sm">
          <Badge variant="outline">
            {organizationRoleLabels[invitation.role]}
          </Badge>
          <span className="flex items-center gap-1">
            <Clock className="size-3" />
            до {expiresAt}
          </span>
        </div>
      </div>
      <Button
        variant="ghost"
        size="sm"
        onClick={() =>
          cancelMutation.mutate({ orgId, invitationId: invitation.id })
        }
        disabled={cancelMutation.isPending}
      >
        {cancelMutation.isPending ? (
          <Loader2 className="size-4 animate-spin" />
        ) : (
          <X className="size-4" />
        )}
        <span className="hidden sm:inline">Отменить</span>
      </Button>
    </div>
  );
}

function CreateInvitationDialog({
  orgId,
  open,
  onOpenChange,
}: {
  orgId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const form = useForm<CreateInvitationInput>({
    resolver: zodResolver(createInvitationSchema),
    defaultValues: {
      inviteeEmail: "",
      role: OrganizationRole.Student,
      ttlDays: 7,
    },
  });

  const mutation = useCreateInvitation({
    onSuccess: () => {
      toast.success("Приглашение отправлено");
      onOpenChange(false);
      form.reset();
    },
    onError: () => toast.error("Не удалось отправить приглашение"),
  });

  function handleSubmit(data: CreateInvitationInput) {
    mutation.mutate({
      orgId,
      request: {
        inviteeEmail: data.inviteeEmail || null,
        inviteeProfileId: data.inviteeProfileId || null,
        role: data.role,
        ttlDays: data.ttlDays,
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Новое приглашение</DialogTitle>
          <DialogDescription>
            Пригласите участника по email или ID профиля
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="inviteeEmail"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input
                      type="email"
                      placeholder="user@example.com"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="role"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Роль</FormLabel>
                  <Select
                    onValueChange={(v) => field.onChange(Number(v))}
                    value={String(field.value)}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(organizationRoleLabels)
                        .filter(
                          ([key]) => Number(key) !== OrganizationRole.Owner,
                        )
                        .map(([key, label]) => (
                          <SelectItem key={key} value={key}>
                            {label}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="ttlDays"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Срок действия (дней)</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      min={1}
                      max={30}
                      {...field}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending && (
                  <Loader2 className="size-4 animate-spin" />
                )}
                Отправить
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
