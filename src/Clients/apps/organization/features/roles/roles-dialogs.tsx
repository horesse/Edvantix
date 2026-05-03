"use client";

import { useState } from "react";

import { Loader2 } from "lucide-react";
import { toast } from "sonner";

import useCreateRole from "@workspace/api-hooks/company/useCreateRole";
import useDeleteRole from "@workspace/api-hooks/company/useDeleteRole";
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

// ── Create role ────────────────────────────────────────────────────────────────

interface CreateRoleDialogProps {
  orgId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CreateRoleDialog({
  orgId,
  open,
  onOpenChange,
}: Readonly<CreateRoleDialogProps>) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const mutation = useCreateRole({
    onSuccess: () => {
      toast.success("Роль создана");
      onOpenChange(false);
      setName("");
      setDescription("");
    },
    onError: () => toast.error("Не удалось создать роль"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    mutation.mutate({
      orgId,
      request: {
        name: name.trim(),
        description: description.trim() || null,
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Создать роль</DialogTitle>
          <DialogDescription>
            Новая роль будет доступна для назначения участникам организации
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="create-role-name">
              Название <span className="text-destructive">*</span>
            </Label>
            <Input
              id="create-role-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Например, Методист"
              required
              maxLength={100}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="create-role-desc">
              Описание{" "}
              <span className="text-muted-foreground text-xs">
                (необязательно)
              </span>
            </Label>
            <Input
              id="create-role-desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Короткое пояснение — кому назначается эта роль"
              maxLength={300}
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
            <Button type="submit" disabled={mutation.isPending || !name.trim()}>
              {mutation.isPending && (
                <Loader2 className="size-4 animate-spin" />
              )}
              Создать
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// ── Delete role ───────────────────────────────────────────────────────────────

interface DeleteRoleDialogProps {
  orgId: string;
  roleId: string;
  roleName: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onDeleted?: () => void;
}

export function DeleteRoleDialog({
  orgId,
  roleId,
  roleName,
  open,
  onOpenChange,
  onDeleted,
}: Readonly<DeleteRoleDialogProps>) {
  const mutation = useDeleteRole({
    onSuccess: () => {
      toast.success("Роль удалена");
      onOpenChange(false);
      onDeleted?.();
    },
    onError: () => toast.error("Не удалось удалить роль"),
  });

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Удалить роль?</AlertDialogTitle>
          <AlertDialogDescription>
            Роль <strong>{roleName}</strong> будет безвозвратно удалена.
            Участники с этой ролью останутся в организации, но останутся без
            роли.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Отмена</AlertDialogCancel>
          <AlertDialogAction
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            onClick={() => mutation.mutate({ orgId, roleId })}
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
