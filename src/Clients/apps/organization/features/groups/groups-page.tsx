"use client";

import { useMemo, useState } from "react";

import Link from "next/link";

import type { ColumnDef } from "@tanstack/react-table";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2, MoreHorizontal, Plus, Trash2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useCreateGroup from "@workspace/api-hooks/company/useCreateGroup";
import useDeleteGroup from "@workspace/api-hooks/company/useDeleteGroup";
import useOrganizationGroups from "@workspace/api-hooks/company/useOrganizationGroups";
import type { GroupModel } from "@workspace/types/company";
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
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type CreateGroupInput,
  createGroupSchema,
} from "@workspace/validations/company";

import { FilterTable } from "@/components/filter-table";
import { usePaginatedTable } from "@/hooks/usePaginatedTable";
import { useOrganization } from "@/components/organization-provider";

export function GroupsPage() {
  const { currentOrg, canManage } = useOrganization();
  const [createOpen, setCreateOpen] = useState(false);
  const [deleteGroup, setDeleteGroup] = useState<GroupModel | null>(null);

  const { pageIndex, pageSize, sortingQuery, handlePaginationChange, handleSortingChange } =
    usePaginatedTable();

  const orgId = currentOrg?.id ?? 0;
  const { data, isLoading } = useOrganizationGroups(orgId, {
    pageIndex: pageIndex + 1,
    pageSize,
    ...sortingQuery,
  });

  const columns = useMemo<ColumnDef<GroupModel>[]>(
    () => [
      {
        accessorKey: "name",
        header: "Название",
        cell: ({ row }) => (
          <Link
            href={`/groups/${row.original.id}`}
            className="font-medium hover:underline"
          >
            {row.original.name}
          </Link>
        ),
      },
      {
        accessorKey: "description",
        header: "Описание",
        cell: ({ row }) => (
          <div className="text-muted-foreground max-w-md truncate">
            {row.original.description ?? "—"}
          </div>
        ),
      },
      {
        accessorKey: "membersCount",
        header: "Участники",
        cell: ({ row }) => (
          <div className="tabular-nums">{row.original.membersCount}</div>
        ),
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
                  <DropdownMenuItem asChild>
                    <Link href={`/groups/${row.original.id}`}>Открыть</Link>
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    variant="destructive"
                    onClick={() => setDeleteGroup(row.original)}
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
          <h1 className="text-3xl font-bold tracking-tight">Группы</h1>
          <p className="text-muted-foreground">
            Управление группами организации
          </p>
        </div>
        {canManage && (
          <Button onClick={() => setCreateOpen(true)}>
            <Plus className="size-4" />
            Создать группу
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
        getRowId={(row) => String(row.id)}
      />

      <CreateGroupDialog
        orgId={orgId}
        open={createOpen}
        onOpenChange={setCreateOpen}
      />
      <DeleteGroupDialog
        orgId={orgId}
        group={deleteGroup}
        onClose={() => setDeleteGroup(null)}
      />
    </div>
  );
}

function CreateGroupDialog({
  orgId,
  open,
  onOpenChange,
}: {
  orgId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const form = useForm<CreateGroupInput>({
    resolver: zodResolver(createGroupSchema),
    defaultValues: { name: "", description: "" },
  });

  const mutation = useCreateGroup({
    onSuccess: () => {
      toast.success("Группа создана");
      onOpenChange(false);
      form.reset();
    },
    onError: () => toast.error("Не удалось создать группу"),
  });

  function handleSubmit(data: CreateGroupInput) {
    mutation.mutate({
      orgId,
      request: { name: data.name, description: data.description || null },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Новая группа</DialogTitle>
          <DialogDescription>Создайте группу в организации</DialogDescription>
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
                    <Input placeholder="10-А класс" {...field} />
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
                    <Textarea
                      placeholder="Описание группы"
                      rows={2}
                      {...field}
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
                Создать
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

function DeleteGroupDialog({
  orgId,
  group,
  onClose,
}: {
  orgId: number;
  group: GroupModel | null;
  onClose: () => void;
}) {
  const mutation = useDeleteGroup({
    onSuccess: () => {
      toast.success("Группа удалена");
      onClose();
    },
    onError: () => toast.error("Не удалось удалить группу"),
  });

  return (
    <AlertDialog
      open={group !== null}
      onOpenChange={(open) => !open && onClose()}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Удалить группу?</AlertDialogTitle>
          <AlertDialogDescription>
            Группа «{group?.name}» будет удалена. Это действие нельзя отменить.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Отмена</AlertDialogCancel>
          <AlertDialogAction
            onClick={() => group && mutation.mutate({ id: group.id, orgId })}
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
