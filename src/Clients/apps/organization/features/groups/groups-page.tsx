"use client";

import { useState } from "react";

import Link from "next/link";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2, Plus, Trash2, UsersRound } from "lucide-react";
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
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
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
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type CreateGroupInput,
  createGroupSchema,
} from "@workspace/validations/company";

import { useOrganization } from "@/components/organization-provider";

export function GroupsPage() {
  const { currentOrg, canManage } = useOrganization();
  const [createOpen, setCreateOpen] = useState(false);
  const [deleteGroup, setDeleteGroup] = useState<GroupModel | null>(null);

  const orgId = currentOrg?.id ?? 0;
  const { data: groups = [], isLoading } = useOrganizationGroups(orgId);

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Группы</h1>
          <p className="text-muted-foreground text-sm">
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

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
      ) : groups.length === 0 ? (
        <div className="border-border/50 flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
          <UsersRound className="text-muted-foreground/50 mb-3 size-10" />
          <p className="text-muted-foreground text-sm">Нет групп</p>
          {canManage && (
            <Button
              variant="link"
              size="sm"
              className="mt-1"
              onClick={() => setCreateOpen(true)}
            >
              Создать первую группу
            </Button>
          )}
        </div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {groups.map((group) => (
            <GroupCard
              key={group.id}
              group={group}
              canManage={canManage}
              onDelete={() => setDeleteGroup(group)}
            />
          ))}
        </div>
      )}

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

function GroupCard({
  group,
  canManage,
  onDelete,
}: {
  group: GroupModel;
  canManage: boolean;
  onDelete: () => void;
}) {
  return (
    <Card className="group hover:bg-muted/50 relative transition-colors">
      <Link href={`/groups/${group.id}`}>
        <CardHeader className="pb-2">
          <CardTitle className="text-base">{group.name}</CardTitle>
          {group.description && (
            <CardDescription className="line-clamp-2">
              {group.description}
            </CardDescription>
          )}
        </CardHeader>
        <CardContent>
          <p className="text-muted-foreground text-sm">
            {group.membersCount}{" "}
            {pluralize(
              group.membersCount,
              "участник",
              "участника",
              "участников",
            )}
          </p>
        </CardContent>
      </Link>
      {canManage && (
        <Button
          variant="ghost"
          size="sm"
          className="text-muted-foreground hover:text-destructive absolute top-2 right-2 size-8 p-0 opacity-0 transition-opacity group-hover:opacity-100"
          onClick={(e) => {
            e.preventDefault();
            onDelete();
          }}
        >
          <Trash2 className="size-4" />
        </Button>
      )}
    </Card>
  );
}

function pluralize(count: number, one: string, few: string, many: string) {
  const mod10 = count % 10;
  const mod100 = count % 100;
  if (mod10 === 1 && mod100 !== 11) return one;
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return few;
  return many;
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
