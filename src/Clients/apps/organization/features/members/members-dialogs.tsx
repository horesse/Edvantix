"use client";

import { useState } from "react";

import { Loader2 } from "lucide-react";
import { toast } from "sonner";

import useAddMember from "@workspace/api-hooks/company/useAddMember";
import useRemoveMember from "@workspace/api-hooks/company/useRemoveMember";
import useUpdateMember from "@workspace/api-hooks/company/useUpdateMember";
import type { OrganizationMemberDto } from "@workspace/types/company";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@workspace/ui/components/alert-dialog";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";

// ── Add member ────────────────────────────────────────────────────────────────

interface AddMemberDialogProps {
  orgId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function AddMemberDialog({
  orgId,
  open,
  onOpenChange,
}: Readonly<AddMemberDialogProps>) {
  const [profileId, setProfileId] = useState("");
  const [roleId, setRoleId] = useState("");
  const [startDate, setStartDate] = useState(
    () => new Date().toISOString().split("T")[0] ?? "",
  );

  const mutation = useAddMember({
    onSuccess: () => {
      toast.success("Участник добавлен");
      onOpenChange(false);
      setProfileId("");
      setRoleId("");
    },
    onError: () => toast.error("Не удалось добавить участника"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!profileId.trim() || !roleId.trim() || !startDate) return;
    mutation.mutate({
      orgId,
      request: {
        profileId: profileId.trim(),
        organizationMemberRoleId: roleId.trim(),
        startDate,
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Пригласить участника</DialogTitle>
          <DialogDescription>
            Укажите ID профиля, роль и дату начала участия
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="add-profileId">ID профиля</Label>
            <Input
              id="add-profileId"
              value={profileId}
              onChange={(e) => setProfileId(e.target.value)}
              placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="add-roleId">ID роли</Label>
            <Input
              id="add-roleId"
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="add-startDate">Дата начала</Label>
            <Input
              id="add-startDate"
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              required
            />
          </div>
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
              Добавить
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// ── Change role ───────────────────────────────────────────────────────────────

interface ChangeRoleDialogProps {
  orgId: string;
  member: OrganizationMemberDto | null;
  onClose: () => void;
}

export function ChangeRoleDialog({
  orgId,
  member,
  onClose,
}: Readonly<ChangeRoleDialogProps>) {
  const [newRoleId, setNewRoleId] = useState("");

  const mutation = useUpdateMember({
    onSuccess: () => {
      toast.success("Роль обновлена");
      onClose();
    },
    onError: () => toast.error("Не удалось обновить роль"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!member || !newRoleId.trim()) return;
    mutation.mutate({
      orgId,
      memberId: member.id,
      request: { organizationMemberRoleId: newRoleId.trim() },
    });
  }

  return (
    <Dialog open={member !== null} onOpenChange={(o) => !o && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Изменить роль</DialogTitle>
          <DialogDescription>{member?.fullName}</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="change-roleId">Новый ID роли</Label>
            <Input
              id="change-roleId"
              value={newRoleId}
              onChange={(e) => setNewRoleId(e.target.value)}
              placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
              required
            />
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Отмена
            </Button>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending && (
                <Loader2 className="size-4 animate-spin" />
              )}
              Сохранить
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// ── Remove member ─────────────────────────────────────────────────────────────

interface RemoveMemberDialogProps {
  orgId: string;
  member: OrganizationMemberDto | null;
  onClose: () => void;
}

export function RemoveMemberDialog({
  orgId,
  member,
  onClose,
}: Readonly<RemoveMemberDialogProps>) {
  const mutation = useRemoveMember({
    onSuccess: () => {
      toast.success("Участник удалён");
      onClose();
    },
    onError: () => toast.error("Не удалось удалить участника"),
  });

  return (
    <AlertDialog open={member !== null} onOpenChange={(o) => !o && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Удалить участника?</AlertDialogTitle>
          <AlertDialogDescription>
            <strong>{member?.fullName}</strong> будет удалён из организации. Это
            действие нельзя отменить.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Отмена</AlertDialogCancel>
          <AlertDialogAction
            onClick={() =>
              member && mutation.mutate({ orgId, memberId: member.id })
            }
            disabled={mutation.isPending}
          >
            {mutation.isPending && <Loader2 className="size-4 animate-spin" />}
            Удалить
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
