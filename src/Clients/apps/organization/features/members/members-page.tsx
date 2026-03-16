"use client";

import { useState } from "react";

import {
  Eye,
  Filter,
  GraduationCap,
  Loader2,
  MoreHorizontal,
  Pencil,
  Plus,
  Search,
  Trash2,
  UserCheck,
  UserCog,
  Users,
  X,
} from "lucide-react";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { useOrganization } from "@/components/organization/provider";
import { organizationRoleLabels } from "@/lib/company-options";

// ── Role config ───────────────────────────────────────────────────────────────

const ROLE_TABS = [
  { label: "Все", value: "all" },
  { label: "Ученики", value: String(OrganizationRole.Student) },
  { label: "Учителя", value: String(OrganizationRole.Teacher) },
  { label: "Прочие", value: "other" },
] as const;

type RoleTab = (typeof ROLE_TABS)[number]["value"];

const ROLE_BADGE: Record<
  OrganizationRole,
  { label: string; bg: string; text: string }
> = {
  [OrganizationRole.Owner]: {
    label: "Владелец",
    bg: "bg-primary/10",
    text: "text-primary",
  },
  [OrganizationRole.Manager]: {
    label: "Менеджер",
    bg: "bg-violet-50",
    text: "text-violet-700",
  },
  [OrganizationRole.Teacher]: {
    label: "Учитель",
    bg: "bg-violet-50",
    text: "text-violet-700",
  },
  [OrganizationRole.Student]: {
    label: "Ученик",
    bg: "bg-blue-50",
    text: "text-blue-700",
  },
};

// ── Stat cards ────────────────────────────────────────────────────────────────

interface StatCardProps {
  icon: React.ElementType;
  iconBg: string;
  iconColor: string;
  label: string;
  value: number;
}

function StatCard({
  icon: Icon,
  iconBg,
  iconColor,
  label,
  value,
}: StatCardProps) {
  return (
    <div className="bg-card border-border flex items-center gap-3 rounded-xl border px-4 py-3 shadow-sm">
      <div
        className={cn(
          "flex size-8 shrink-0 items-center justify-center rounded-lg",
          iconBg,
        )}
      >
        <Icon className={cn("size-4", iconColor)} />
      </div>
      <div>
        <p className="text-foreground text-lg leading-none font-bold tabular-nums">
          {value}
        </p>
        <p className="text-muted-foreground mt-0.5 text-[11px]">{label}</p>
      </div>
    </div>
  );
}

// ── Member row ────────────────────────────────────────────────────────────────

function MemberRow({
  member,
  selected,
  onSelect,
  canManage,
  onChangeRole,
  onRemove,
}: {
  member: OrganizationMemberModel;
  selected: boolean;
  onSelect: (id: string, checked: boolean) => void;
  canManage: boolean;
  onChangeRole: (m: OrganizationMemberModel) => void;
  onRemove: (m: OrganizationMemberModel) => void;
}) {
  const displayName =
    member.displayName ?? `Профиль #${member.profileId.slice(0, 8)}`;
  const initials = displayName
    .split(" ")
    .slice(0, 2)
    .map((w) => w[0])
    .join("")
    .toUpperCase();

  const badge = ROLE_BADGE[member.role] ?? ROLE_BADGE[OrganizationRole.Student];

  return (
    <tr className="group border-border hover:bg-muted/30 border-b last:border-0">
      {/* Checkbox */}
      <td className="w-10 px-4 py-3">
        <Checkbox
          checked={selected}
          onCheckedChange={(v) => onSelect(member.id, Boolean(v))}
          aria-label={`Выбрать ${displayName}`}
        />
      </td>

      {/* Name + email */}
      <td className="min-w-0 px-2 py-3">
        <div className="flex items-center gap-3">
          <div className="from-primary/60 to-primary flex size-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white">
            {initials}
          </div>
          <div className="min-w-0">
            <p className="text-foreground truncate text-sm font-medium">
              {displayName}
            </p>
            <p className="text-muted-foreground truncate text-xs">
              {member.profileId.slice(0, 8)}…
            </p>
          </div>
        </div>
      </td>

      {/* Role */}
      <td className="px-2 py-3">
        <span
          className={cn(
            "inline-block rounded-full px-2.5 py-1 text-[11px] font-semibold",
            badge.bg,
            badge.text,
          )}
        >
          {badge.label}
        </span>
      </td>

      {/* Group (placeholder) */}
      <td className="hidden px-2 py-3 sm:table-cell">
        <span className="text-muted-foreground text-xs">—</span>
      </td>

      {/* Status */}
      <td className="hidden px-2 py-3 md:table-cell">
        <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-2.5 py-1 text-[11px] font-semibold text-emerald-700">
          <span className="size-1.5 rounded-full bg-emerald-500" />
          Активен
        </span>
      </td>

      {/* Date */}
      <td className="hidden px-2 py-3 lg:table-cell">
        <span className="text-muted-foreground text-xs">
          {new Date(member.joinedAt).toLocaleDateString("ru-RU")}
        </span>
      </td>

      {/* Actions */}
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
                    <UserCog className="size-4" />
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

// ── Skeleton rows ─────────────────────────────────────────────────────────────

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
                <Skeleton className="h-3 w-32" />
                <Skeleton className="h-2.5 w-20" />
              </div>
            </div>
          </td>
          <td className="px-2 py-3">
            <Skeleton className="h-5 w-16 rounded-full" />
          </td>
          <td className="hidden px-2 py-3 sm:table-cell">
            <Skeleton className="h-3 w-24" />
          </td>
          <td className="hidden px-2 py-3 md:table-cell">
            <Skeleton className="h-5 w-16 rounded-full" />
          </td>
          <td className="hidden px-2 py-3 lg:table-cell">
            <Skeleton className="h-3 w-20" />
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
  const [activeTab, setActiveTab] = useState<RoleTab>("all");
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [changeRoleMember, setChangeRoleMember] =
    useState<OrganizationMemberModel | null>(null);
  const [removeMember, setRemoveMember] =
    useState<OrganizationMemberModel | null>(null);

  const orgId = currentOrg?.id ?? "";
  const { data, isLoading } = useOrganizationMembers(orgId, {
    pageIndex: page,
    pageSize,
  });

  const members = data?.items ?? [];
  const total = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  // client-side filter by search and tab
  const filtered = members.filter((m) => {
    const name = (m.displayName ?? "").toLowerCase();
    const matchSearch = !search || name.includes(search.toLowerCase());
    const matchTab =
      activeTab === "all"
        ? true
        : activeTab === String(OrganizationRole.Student)
          ? m.role === OrganizationRole.Student
          : activeTab === String(OrganizationRole.Teacher)
            ? m.role === OrganizationRole.Teacher
            : m.role !== OrganizationRole.Student &&
              m.role !== OrganizationRole.Teacher;
    return matchSearch && matchTab;
  });

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

  const stats = [
    {
      icon: Users,
      iconBg: "bg-blue-50",
      iconColor: "text-blue-600",
      label: "Учеников",
      value: 248,
    },
    {
      icon: GraduationCap,
      iconBg: "bg-violet-50",
      iconColor: "text-violet-600",
      label: "Учителей",
      value: 32,
    },
    {
      icon: UserCheck,
      iconBg: "bg-emerald-50",
      iconColor: "text-emerald-600",
      label: "Активных",
      value: 289,
    },
    {
      icon: Loader2,
      iconBg: "bg-amber-50",
      iconColor: "text-amber-600",
      label: "Ожидают",
      value: 23,
    },
    {
      icon: X,
      iconBg: "bg-rose-50",
      iconColor: "text-rose-500",
      label: "Заблокировано",
      value: 0,
    },
  ];

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
            Пригласить
          </Button>
        )}
      </div>

      {/* ── Stat cards ── */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-5">
        {stats.map((s) => (
          <StatCard key={s.label} {...s} />
        ))}
      </div>

      {/* ── Table card ── */}
      <div className="bg-card border-border rounded-2xl border shadow-sm">
        {/* Toolbar */}
        <div className="border-border flex flex-wrap items-center gap-3 border-b px-5 py-3">
          {/* Role tabs */}
          <div className="bg-muted/50 flex items-center gap-0.5 rounded-lg p-1">
            {ROLE_TABS.map((tab) => (
              <button
                key={tab.value}
                onClick={() => setActiveTab(tab.value)}
                className={cn(
                  "rounded-md px-3 py-1.5 text-xs font-medium transition-colors",
                  activeTab === tab.value
                    ? "bg-background text-foreground shadow-sm"
                    : "text-muted-foreground hover:text-foreground",
                )}
              >
                {tab.label}
              </button>
            ))}
          </div>

          {/* Search */}
          <div className="relative min-w-48 flex-1">
            <Search className="text-muted-foreground absolute top-1/2 left-3 size-3.5 -translate-y-1/2" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Поиск участника…"
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
                  Участник
                </th>
                <th className="text-muted-foreground px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Роль
                </th>
                <th className="text-muted-foreground hidden px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase sm:table-cell">
                  Группа
                </th>
                <th className="text-muted-foreground hidden px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase md:table-cell">
                  Статус
                </th>
                <th className="text-muted-foreground hidden px-2 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase lg:table-cell">
                  Вступил
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
                    colSpan={7}
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

// ── Dialogs (logic unchanged, restyled) ───────────────────────────────────────

function AddMemberDialog({
  orgId,
  open,
  onOpenChange,
}: {
  orgId: string;
  open: boolean;
  onOpenChange: (v: boolean) => void;
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

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Добавить участника</DialogTitle>
          <DialogDescription>
            Укажите ID профиля и роль нового участника
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (profileId.trim())
              mutation.mutate({
                orgId,
                request: { profileId: profileId.trim(), role },
              });
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
  orgId: string;
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

  return (
    <Dialog open={member !== null} onOpenChange={(o) => !o && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Изменить роль</DialogTitle>
          <DialogDescription>
            {member?.displayName ?? `Профиль #${member?.profileId}`}
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (member)
              mutation.mutate({
                orgId,
                memberId: member.id,
                request: { newRole },
              });
          }}
          className="space-y-4"
        >
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
  orgId: string;
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
    <AlertDialog open={member !== null} onOpenChange={(o) => !o && onClose()}>
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
