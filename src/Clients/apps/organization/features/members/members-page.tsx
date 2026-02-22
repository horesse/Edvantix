"use client";

import { useMemo, useState } from "react";

import type { ColumnDef } from "@tanstack/react-table";
import { Loader2, MoreHorizontal, Plus, Trash2, UserCog } from "lucide-react";
import { toast } from "sonner";

import useAddMember from "@workspace/api-hooks/company/useAddMember";
import useOrganizationMembers from "@workspace/api-hooks/company/useOrganizationMembers";
import useRemoveMember from "@workspace/api-hooks/company/useRemoveMember";
import useUpdateMemberRole from "@workspace/api-hooks/company/useUpdateMemberRole";
import type { OrganizationMemberModel } from "@workspace/types/company";
import { OrganizationRole } from "@workspace/types/company";
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
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";

import { FilterTable } from "@/components/filter-table";
import { useOrganization } from "@/components/organization-provider";
import { usePaginatedTable } from "@/hooks/usePaginatedTable";
import { organizationRoleLabels } from "@/lib/company-options";

export function MembersPage() {
  const { currentOrg, canManage } = useOrganization();
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [changeRoleMember, setChangeRoleMember] =
    useState<OrganizationMemberModel | null>(null);
  const [removeMember, setRemoveMember] =
    useState<OrganizationMemberModel | null>(null);

  const {
    pageIndex,
    pageSize,
    sortingQuery,
    handlePaginationChange,
    handleSortingChange,
  } = usePaginatedTable();

  const orgId = currentOrg?.id ?? 0;
  const { data, isLoading } = useOrganizationMembers(orgId, {
    pageIndex: pageIndex + 1,
    pageSize,
    ...sortingQuery,
  });

  const columns = useMemo<ColumnDef<OrganizationMemberModel>[]>(
    () => [
      {
        accessorKey: "displayName",
        header: "Имя",
        cell: ({ row }) => (
          <div className="font-medium">
            {row.original.displayName ?? `Профиль #${row.original.profileId}`}
          </div>
        ),
      },
      {
        accessorKey: "role",
        header: "Роль",
        cell: ({ row }) => <RoleBadge role={row.original.role} />,
      },
      {
        accessorKey: "joinedAt",
        header: "Дата вступления",
        cell: ({ row }) =>
          new Date(row.original.joinedAt).toLocaleDateString("ru-RU"),
      },
      {
        id: "actions",
        cell: ({ row }) =>
          canManage ? (
            <div className="flex justify-end">
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="sm" className="size-8 p-0">
                    <MoreHorizontal className="size-4" />
                    <span className="sr-only">Открыть меню</span>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem
                    onClick={() => setChangeRoleMember(row.original)}
                  >
                    <UserCog className="size-4" />
                    Изменить роль
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    variant="destructive"
                    onClick={() => setRemoveMember(row.original)}
                  >
                    <Trash2 className="size-4" />
                    Удалить
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          ) : null,
      },
    ],
    [canManage],
  );

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Участники</h1>
          <p className="text-muted-foreground">
            Управление участниками организации
          </p>
        </div>
        {canManage && (
          <Button onClick={() => setAddDialogOpen(true)}>
            <Plus className="size-4" />
            Добавить
          </Button>
        )}
      </div>

      <FilterTable
        columns={columns}
        data={data?.items ?? []}
        totalItems={data?.totalItems ?? 0}
        pageIndex={pageIndex}
        pageSize={pageSize}
        isLoading={isLoading}
        onPaginationChange={handlePaginationChange}
        onSortingChange={handleSortingChange}
        getRowId={(row) => row.id}
      />

      <AddMemberDialog
        orgId={orgId}
        open={addDialogOpen}
        onOpenChange={setAddDialogOpen}
      />
      <ChangeRoleDialog
        orgId={orgId}
        member={changeRoleMember}
        onClose={() => setChangeRoleMember(null)}
      />
      <RemoveMemberDialog
        orgId={orgId}
        member={removeMember}
        onClose={() => setRemoveMember(null)}
      />
    </div>
  );
}

function RoleBadge({ role }: { role: OrganizationRole }) {
  const variant =
    role === OrganizationRole.Owner
      ? "default"
      : role === OrganizationRole.Manager
        ? "secondary"
        : "outline";

  return <Badge variant={variant}>{organizationRoleLabels[role]}</Badge>;
}

function AddMemberDialog({
  orgId,
  open,
  onOpenChange,
}: {
  orgId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const [profileId, setProfileId] = useState("");
  const [role, setRole] = useState<OrganizationRole>(OrganizationRole.Student);

  const mutation = useAddMember({
    onSuccess: () => {
      toast.success("Участник добавлен");
      onOpenChange(false);
      setProfileId("");
    },
    onError: () => toast.error("Не удалось добавить участника"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const id = Number(profileId);
    if (id > 0) {
      mutation.mutate({ orgId, request: { profileId: id, role } });
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Добавить участника</DialogTitle>
          <DialogDescription>
            Укажите ID профиля и роль нового участника
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="profileId">ID профиля</Label>
            <Input
              id="profileId"
              type="number"
              min={1}
              value={profileId}
              onChange={(e) => setProfileId(e.target.value)}
              placeholder="12345"
              required
            />
          </div>
          <div className="space-y-2">
            <Label>Роль</Label>
            <Select
              value={String(role)}
              onValueChange={(v) => setRole(Number(v) as OrganizationRole)}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(organizationRoleLabels)
                  .filter(([key]) => Number(key) !== OrganizationRole.Owner)
                  .map(([key, label]) => (
                    <SelectItem key={key} value={key}>
                      {label}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
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

function ChangeRoleDialog({
  orgId,
  member,
  onClose,
}: {
  orgId: number;
  member: OrganizationMemberModel | null;
  onClose: () => void;
}) {
  const [newRole, setNewRole] = useState<OrganizationRole>(
    member?.role ?? OrganizationRole.Student,
  );

  const mutation = useUpdateMemberRole({
    onSuccess: () => {
      toast.success("Роль обновлена");
      onClose();
    },
    onError: () => toast.error("Не удалось обновить роль"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (member) {
      mutation.mutate({
        orgId,
        memberId: member.id,
        request: { newRole },
      });
    }
  }

  return (
    <Dialog open={member !== null} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Изменить роль</DialogTitle>
          <DialogDescription>
            {member?.displayName ?? `Профиль #${member?.profileId}`}
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Новая роль</Label>
            <Select
              value={String(newRole)}
              onValueChange={(v) => setNewRole(Number(v) as OrganizationRole)}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(organizationRoleLabels)
                  .filter(([key]) => Number(key) !== OrganizationRole.Owner)
                  .map(([key, label]) => (
                    <SelectItem key={key} value={key}>
                      {label}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
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

function RemoveMemberDialog({
  orgId,
  member,
  onClose,
}: {
  orgId: number;
  member: OrganizationMemberModel | null;
  onClose: () => void;
}) {
  const mutation = useRemoveMember({
    onSuccess: () => {
      toast.success("Участник удалён");
      onClose();
    },
    onError: () => toast.error("Не удалось удалить участника"),
  });

  return (
    <AlertDialog
      open={member !== null}
      onOpenChange={(open) => !open && onClose()}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Удалить участника?</AlertDialogTitle>
          <AlertDialogDescription>
            {member?.displayName ?? `Профиль #${member?.profileId}`} будет
            удалён из организации. Это действие нельзя отменить.
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
