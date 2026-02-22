"use client";

import { useMemo, useState } from "react";

import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import type { ColumnDef } from "@tanstack/react-table";
import {
  ArrowLeft,
  Loader2,
  MoreHorizontal,
  Pencil,
  Plus,
  Trash2,
  UserCog,
} from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useAddGroupMember from "@workspace/api-hooks/company/useAddGroupMember";
import useDeleteGroup from "@workspace/api-hooks/company/useDeleteGroup";
import useGroup from "@workspace/api-hooks/company/useGroup";
import useGroupMembers from "@workspace/api-hooks/company/useGroupMembers";
import useRemoveGroupMember from "@workspace/api-hooks/company/useRemoveGroupMember";
import useUpdateGroup from "@workspace/api-hooks/company/useUpdateGroup";
import useUpdateGroupMemberRole from "@workspace/api-hooks/company/useUpdateGroupMemberRole";
import type { GroupMemberModel } from "@workspace/types/company";
import { GroupRole } from "@workspace/types/company";
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
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type UpdateGroupInput,
  updateGroupSchema,
} from "@workspace/validations/company";

import { FilterTable } from "@/components/filter-table";
import { useOrganization } from "@/components/organization-provider";
import { usePaginatedTable } from "@/hooks/usePaginatedTable";
import { groupRoleLabels } from "@/lib/company-options";

type GroupDetailPageProps = {
  groupId: number;
};

export function GroupDetailPage({ groupId }: GroupDetailPageProps) {
  const router = useRouter();
  const { currentOrg, canManage } = useOrganization();
  const { data: group, isLoading: groupLoading } = useGroup(groupId);

  const {
    pageIndex,
    pageSize,
    sortingQuery,
    handlePaginationChange,
    handleSortingChange,
  } = usePaginatedTable();

  const { data: membersData, isLoading: membersLoading } = useGroupMembers(
    groupId,
    {
      pageIndex: pageIndex + 1,
      pageSize,
      ...sortingQuery,
    },
  );

  const [editOpen, setEditOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [addMemberOpen, setAddMemberOpen] = useState(false);
  const [changeRoleMember, setChangeRoleMember] =
    useState<GroupMemberModel | null>(null);
  const [removeMember, setRemoveMember] = useState<GroupMemberModel | null>(
    null,
  );

  const orgId = currentOrg?.id ?? 0;

  const columns = useMemo<ColumnDef<GroupMemberModel>[]>(
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
        cell: ({ row }) => (
          <Badge variant="outline">{groupRoleLabels[row.original.role]}</Badge>
        ),
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

  if (groupLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-4 w-96" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  if (!group) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Группа не найдена
      </p>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => router.push("/groups")}
        >
          <ArrowLeft className="size-4" />
          Группы
        </Button>
      </div>

      <div className="bg-muted/50 overflow-hidden rounded-xl border-0 shadow-sm">
        <div className="p-6">
          <div className="flex items-start justify-between">
            <div className="space-y-1">
              <h2 className="text-2xl font-bold">{group.name}</h2>
              {group.description && (
                <p className="text-muted-foreground">{group.description}</p>
              )}
            </div>
            {canManage && (
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setEditOpen(true)}
                >
                  <Pencil className="size-4" />
                  Редактировать
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setDeleteOpen(true)}
                >
                  <Trash2 className="size-4" />
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>

      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div className="space-y-1">
            <h2 className="text-2xl font-bold">Участники группы</h2>
            <p className="text-muted-foreground text-sm">
              Управление участниками группы
            </p>
          </div>
          {canManage && (
            <Button size="sm" onClick={() => setAddMemberOpen(true)}>
              <Plus className="size-4" />
              Добавить
            </Button>
          )}
        </div>

        <FilterTable
          columns={columns}
          data={membersData?.items ?? []}
          totalItems={membersData?.totalItems ?? 0}
          pageIndex={pageIndex}
          pageSize={pageSize}
          isLoading={membersLoading}
          onPaginationChange={handlePaginationChange}
          onSortingChange={handleSortingChange}
          getRowId={(row) => row.id}
        />
      </div>

      <EditGroupDialog
        group={group}
        orgId={orgId}
        open={editOpen}
        onOpenChange={setEditOpen}
      />
      <DeleteGroupConfirm
        group={group}
        orgId={orgId}
        open={deleteOpen}
        onClose={() => setDeleteOpen(false)}
        onDeleted={() => router.push("/groups")}
      />
      <AddGroupMemberDialog
        groupId={groupId}
        open={addMemberOpen}
        onOpenChange={setAddMemberOpen}
      />
      <ChangeGroupMemberRoleDialog
        groupId={groupId}
        member={changeRoleMember}
        onClose={() => setChangeRoleMember(null)}
      />
      <RemoveGroupMemberDialog
        groupId={groupId}
        member={removeMember}
        onClose={() => setRemoveMember(null)}
      />
    </div>
  );
}

function EditGroupDialog({
  group,
  orgId,
  open,
  onOpenChange,
}: {
  group: { id: number; name: string; description?: string | null };
  orgId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const form = useForm<UpdateGroupInput>({
    resolver: zodResolver(updateGroupSchema),
    defaultValues: {
      name: group.name,
      description: group.description ?? "",
    },
  });

  const mutation = useUpdateGroup({
    onSuccess: () => {
      toast.success("Группа обновлена");
      onOpenChange(false);
    },
    onError: () => toast.error("Не удалось обновить группу"),
  });

  function handleSubmit(data: UpdateGroupInput) {
    mutation.mutate({
      id: group.id,
      orgId,
      request: { name: data.name, description: data.description || null },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Редактировать группу</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Название</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Описание{" "}
                    <span className="text-muted-foreground font-normal">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Textarea rows={2} {...field} />
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
                Сохранить
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

function DeleteGroupConfirm({
  group,
  orgId,
  open,
  onClose,
  onDeleted,
}: {
  group: { id: number; name: string };
  orgId: number;
  open: boolean;
  onClose: () => void;
  onDeleted: () => void;
}) {
  const mutation = useDeleteGroup({
    onSuccess: () => {
      toast.success("Группа удалена");
      onClose();
      onDeleted();
    },
    onError: () => toast.error("Не удалось удалить группу"),
  });

  return (
    <AlertDialog open={open} onOpenChange={(o) => !o && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Удалить группу?</AlertDialogTitle>
          <AlertDialogDescription>
            Группа «{group.name}» будет удалена. Это действие нельзя отменить.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Отмена</AlertDialogCancel>
          <AlertDialogAction
            onClick={() => mutation.mutate({ id: group.id, orgId })}
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

function AddGroupMemberDialog({
  groupId,
  open,
  onOpenChange,
}: {
  groupId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const [profileId, setProfileId] = useState("");
  const [role, setRole] = useState<GroupRole>(GroupRole.Student);

  const mutation = useAddGroupMember({
    onSuccess: () => {
      toast.success("Участник добавлен в группу");
      onOpenChange(false);
      setProfileId("");
    },
    onError: () => toast.error("Не удалось добавить участника"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const id = Number(profileId);
    if (id > 0) {
      mutation.mutate({ groupId, request: { profileId: id, role } });
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Добавить участника</DialogTitle>
          <DialogDescription>
            Укажите ID профиля и роль в группе
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="groupProfileId">ID профиля</Label>
            <Input
              id="groupProfileId"
              type="number"
              min={1}
              value={profileId}
              onChange={(e) => setProfileId(e.target.value)}
              required
            />
          </div>
          <div className="space-y-2">
            <Label>Роль</Label>
            <Select
              value={String(role)}
              onValueChange={(v) => setRole(Number(v) as GroupRole)}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(groupRoleLabels).map(([key, label]) => (
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

function ChangeGroupMemberRoleDialog({
  groupId,
  member,
  onClose,
}: {
  groupId: number;
  member: GroupMemberModel | null;
  onClose: () => void;
}) {
  const [newRole, setNewRole] = useState<GroupRole>(
    member?.role ?? GroupRole.Student,
  );

  const mutation = useUpdateGroupMemberRole({
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
        groupId,
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
              onValueChange={(v) => setNewRole(Number(v) as GroupRole)}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(groupRoleLabels).map(([key, label]) => (
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

function RemoveGroupMemberDialog({
  groupId,
  member,
  onClose,
}: {
  groupId: number;
  member: GroupMemberModel | null;
  onClose: () => void;
}) {
  const mutation = useRemoveGroupMember({
    onSuccess: () => {
      toast.success("Участник удалён из группы");
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
            удалён из группы.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Отмена</AlertDialogCancel>
          <AlertDialogAction
            onClick={() =>
              member && mutation.mutate({ groupId, memberId: member.id })
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
