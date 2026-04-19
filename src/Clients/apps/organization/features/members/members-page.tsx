"use client";

import { useState } from "react";

import {
  Eye,
  Filter,
  Loader2,
  MoreHorizontal,
  Pencil,
  Plus,
  Search,
  Trash2,
  Users,
} from "lucide-react";
import { toast } from "sonner";

import useAddMember from "@workspace/api-hooks/company/useAddMember";
import useOrganizationMembers from "@workspace/api-hooks/company/useOrganizationMembers";
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
import { Checkbox } from "@workspace/ui/components/checkbox";
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
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { useOrganization } from "@/components/organization/provider";

// ── Member row ────────────────────────────────────────────────────────────────

function MemberRow({
  member,
  selected,
  onSelect,
  canManage,
  onChangeRole,
  onRemove,
}: Readonly<{
  member: OrganizationMemberDto;
  selected: boolean;
  onSelect: (id: string, checked: boolean) => void;
  canManage: boolean;
  onChangeRole: (m: OrganizationMemberDto) => void;
  onRemove: (m: OrganizationMemberDto) => void;
}>) {
  const shortId = member.profileId.slice(0, 8);
  const initials = shortId.slice(0, 2).toUpperCase();
  const roleShort = member.organizationMemberRoleId.slice(0, 8);

  return (
    <tr className="group border-border hover:bg-muted/30 border-b last:border-0">
      <td className="w-10 px-4 py-3">
        <Checkbox
          checked={selected}
          onCheckedChange={(v) => onSelect(member.id, Boolean(v))}
          aria-label={`Выбрать участника`}
        />
      </td>

      <td className="min-w-0 px-2 py-3">
        <div className="flex items-center gap-3">
          <div className="from-primary/60 to-primary flex size-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white">
            {initials}
          </div>
          <div className="min-w-0">
            <p className="text-foreground truncate font-mono text-xs">
              {member.profileId}
            </p>
            <p className="text-muted-foreground mt-0.5 text-xs">
              {new Date(member.startDate).toLocaleDateString("ru-RU")}
            </p>
          </div>
        </div>
      </td>

      <td className="px-2 py-3">
        <span className="bg-muted text-muted-foreground inline-block rounded-full px-2.5 py-1 font-mono text-[11px]">
          {roleShort}…
        </span>
      </td>

      <td className="hidden px-2 py-3 md:table-cell">
        {member.endDate ? (
          <span className="inline-flex items-center gap-1.5 rounded-full bg-amber-50 px-2.5 py-1 text-[11px] font-semibold text-amber-700">
            <span className="size-1.5 rounded-full bg-amber-400" />
            до {new Date(member.endDate).toLocaleDateString("ru-RU")}
          </span>
        ) : (
          <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-2.5 py-1 text-[11px] font-semibold text-emerald-700">
            <span className="size-1.5 rounded-full bg-emerald-500" />
            Активен
          </span>
        )}
      </td>

      <td className="w-20 px-2 py-3">
        <div className="flex items-center justify-end gap-1 opacity-0 transition-opacity group-hover:opacity-100">
          <button className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-7 items-center justify-center rounded-lg">
            <Eye className="size-3.5" />
          </button>
          {canManage && (
            <>
              <button
                onClick={() => onChangeRole(member)}
                className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-7 items-center justify-center rounded-lg"
              >
                <Pencil className="size-3.5" />
              </button>
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <button className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-7 items-center justify-center rounded-lg">
                    <MoreHorizontal className="size-3.5" />
                  </button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-44">
                  <DropdownMenuItem onClick={() => onChangeRole(member)}>
                    <Pencil className="size-4" />
                    Изменить роль
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    variant="destructive"
                    onClick={() => onRemove(member)}
                  >
                    <Trash2 className="size-4" />
                    Удалить
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </>
          )}
        </div>
      </td>
    </tr>
  );
}

function SkeletonRows() {
  return (
    <>
      {Array.from({ length: 6 }).map((_, i) => (
        <tr key={i} className="border-border border-b">
          <td className="w-10 px-4 py-3">
            <Skeleton className="size-4 rounded" />
          </td>
          <td className="px-2 py-3">
            <div className="flex items-center gap-3">
              <Skeleton className="size-8 rounded-full" />
              <div className="space-y-1.5">
                <Skeleton className="h-3 w-48" />
                <Skeleton className="h-2.5 w-20" />
              </div>
            </div>
          </td>
          <td className="px-2 py-3">
            <Skeleton className="h-5 w-20 rounded-full" />
          </td>
          <td className="hidden px-2 py-3 md:table-cell">
            <Skeleton className="h-5 w-16 rounded-full" />
          </td>
          <td className="px-2 py-3" />
        </tr>
      ))}
    </>
  );
}

// ── Main ──────────────────────────────────────────────────────────────────────

export function MembersPage() {
  const { currentOrg, canManage } = useOrganization();

  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [changeRoleMember, setChangeRoleMember] =
    useState<OrganizationMemberDto | null>(null);
  const [removeMember, setRemoveMember] =
    useState<OrganizationMemberDto | null>(null);

  const orgId = currentOrg?.id ?? "";
  const { data, isLoading } = useOrganizationMembers(orgId, {
    pageIndex: page,
    pageSize,
  });

  const members = data?.items ?? [];
  const total = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  const filtered = members.filter(
    (m) =>
      !search ||
      m.profileId.toLowerCase().includes(search.toLowerCase()) ||
      m.organizationMemberRoleId.toLowerCase().includes(search.toLowerCase()),
  );

  function toggleSelect(id: string, checked: boolean) {
    setSelected((prev) => {
      const next = new Set(prev);
      if (checked) next.add(id);
      else next.delete(id);
      return next;
    });
  }

  function toggleSelectAll(checked: boolean) {
    setSelected(checked ? new Set(filtered.map((m) => m.id)) : new Set());
  }

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-12 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-5">
      {/* ── Header ── */}
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="text-foreground text-lg font-bold tracking-tight">
            Участники
          </h1>
          <p className="text-muted-foreground mt-0.5 text-sm">
            {total > 0
              ? `${total} участников в организации`
              : "Пока нет участников"}
          </p>
        </div>
        {canManage && (
          <Button size="sm" onClick={() => setAddDialogOpen(true)}>
            <Plus className="size-4" />
            Добавить
          </Button>
        )}
      </div>

      {/* ── Stat card ── */}
      <div className="bg-card border-border inline-flex items-center gap-3 rounded-xl border px-4 py-3 shadow-sm">
        <div className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-blue-50">
          <Users className="size-4 text-blue-600" />
        </div>
        <div>
          <p className="text-foreground text-lg leading-none font-bold tabular-nums">
            {total}
          </p>
          <p className="text-muted-foreground mt-0.5 text-[11px]">Участников</p>
        </div>
      </div>

      {/* ── Table card ── */}
      <div className="bg-card border-border rounded-2xl border shadow-sm">
        {/* Toolbar */}
        <div className="border-border flex flex-wrap items-center gap-3 border-b px-5 py-3">
          <div className="relative min-w-48 flex-1">
            <Search className="text-muted-foreground absolute top-1/2 left-3 size-3.5 -translate-y-1/2" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Поиск по ID профиля…"
              className="h-8 pl-8 text-xs"
            />
          </div>
          <Button variant="outline" size="sm" className="h-8 gap-1.5 text-xs">
            <Filter className="size-3.5" />
            Фильтр
          </Button>
        </div>

        {/* Table */}
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-border border-b">
                <th className="w-10 px-4 py-2.5">
                  <Checkbox
                    checked={
                      filtered.length > 0 && selected.size === filtered.length
                    }
                    onCheckedChange={(v) => toggleSelectAll(Boolean(v))}
                    aria-label="Выбрать всех"
                  />
                </th>
                <th className="text-muted-foreground px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Профиль
                </th>
                <th className="text-muted-foreground px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Роль
                </th>
                <th className="text-muted-foreground hidden px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase md:table-cell">
                  Статус
                </th>
                <th className="w-20 px-2 py-2.5" />
              </tr>
            </thead>
            <tbody>
              {isLoading ? (
                <SkeletonRows />
              ) : filtered.length === 0 ? (
                <tr>
                  <td
                    colSpan={5}
                    className="text-muted-foreground px-4 py-12 text-center text-sm"
                  >
                    {search ? "Участники не найдены" : "Нет участников"}
                  </td>
                </tr>
              ) : (
                filtered.map((member) => (
                  <MemberRow
                    key={member.id}
                    member={member}
                    selected={selected.has(member.id)}
                    onSelect={toggleSelect}
                    canManage={canManage}
                    onChangeRole={setChangeRoleMember}
                    onRemove={setRemoveMember}
                  />
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {!isLoading && total > 0 && (
          <div className="border-border flex items-center justify-between border-t px-5 py-3">
            <p className="text-muted-foreground text-xs">
              {(page - 1) * pageSize + 1}–{Math.min(page * pageSize, total)} из{" "}
              {total}
            </p>
            <div className="flex items-center gap-1">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="border-border text-muted-foreground hover:bg-muted flex size-8 items-center justify-center rounded-lg border transition-colors disabled:opacity-40"
              >
                ‹
              </button>
              {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                const p = i + 1;
                return (
                  <button
                    key={p}
                    onClick={() => setPage(p)}
                    className={cn(
                      "flex size-8 items-center justify-center rounded-lg text-xs font-medium transition-colors",
                      p === page
                        ? "bg-primary text-white"
                        : "border-border text-muted-foreground hover:bg-muted border",
                    )}
                  >
                    {p}
                  </button>
                );
              })}
              {totalPages > 5 && (
                <span className="text-muted-foreground px-1">…</span>
              )}
              <button
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="border-border text-muted-foreground hover:bg-muted flex size-8 items-center justify-center rounded-lg border transition-colors disabled:opacity-40"
              >
                ›
              </button>
            </div>
          </div>
        )}
      </div>

      {/* ── Dialogs ── */}
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

// ── Dialogs ───────────────────────────────────────────────────────────────────

function AddMemberDialog({
  orgId,
  open,
  onOpenChange,
}: Readonly<{
  orgId: string;
  open: boolean;
  onOpenChange: (v: boolean) => void;
}>) {
  const [profileId, setProfileId] = useState("");
  const [roleId, setRoleId] = useState("");
  const [startDate, setStartDate] = useState(
    new Date().toISOString().split("T")[0] ?? "",
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

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Добавить участника</DialogTitle>
          <DialogDescription>
            Укажите ID профиля, роль и дату начала
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (profileId.trim() && roleId.trim() && startDate) {
              mutation.mutate({
                orgId,
                request: {
                  profileId: profileId.trim(),
                  organizationMemberRoleId: roleId.trim(),
                  startDate,
                },
              });
            }
          }}
          className="space-y-4"
        >
          <div className="space-y-2">
            <Label htmlFor="profileId">ID профиля</Label>
            <Input
              id="profileId"
              value={profileId}
              onChange={(e) => setProfileId(e.target.value)}
              placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="roleId">ID роли</Label>
            <Input
              id="roleId"
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="startDate">Дата начала</Label>
            <Input
              id="startDate"
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

function ChangeRoleDialog({
  orgId,
  member,
  onClose,
}: Readonly<{
  orgId: string;
  member: OrganizationMemberDto | null;
  onClose: () => void;
}>) {
  const [newRoleId, setNewRoleId] = useState(
    member?.organizationMemberRoleId ?? "",
  );

  const mutation = useUpdateMember({
    onSuccess: () => {
      toast.success("Роль обновлена");
      onClose();
    },
    onError: () => toast.error("Не удалось обновить роль"),
  });

  return (
    <Dialog open={member !== null} onOpenChange={(o) => !o && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Изменить роль</DialogTitle>
          <DialogDescription className="font-mono text-xs">
            {member?.profileId}
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (member && newRoleId.trim()) {
              mutation.mutate({
                orgId,
                memberId: member.id,
                request: { organizationMemberRoleId: newRoleId.trim() },
              });
            }
          }}
          className="space-y-4"
        >
          <div className="space-y-2">
            <Label>Новый ID роли</Label>
            <Input
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

function RemoveMemberDialog({
  orgId,
  member,
  onClose,
}: Readonly<{
  orgId: string;
  member: OrganizationMemberDto | null;
  onClose: () => void;
}>) {
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
            Профиль{" "}
            <span className="font-mono">{member?.profileId.slice(0, 8)}…</span>{" "}
            будет удалён из организации. Это действие нельзя отменить.
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
