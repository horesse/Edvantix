"use client";

import type React from "react";
import { useState } from "react";

import { useRouter } from "next/navigation";

import {
  Bell,
  Filter,
  Loader2,
  MoreHorizontal,
  Pencil,
  Search,
  Shield,
  ShieldAlert,
  ShieldCheck,
  UserX,
  Users,
} from "lucide-react";
import { toast } from "sonner";

import useAdminProfiles from "@workspace/api-hooks/admin/useAdminProfiles";
import useBlockProfile from "@workspace/api-hooks/admin/useBlockProfile";
import useSendAdminNotification from "@workspace/api-hooks/admin/useSendAdminNotification";
import useUnblockProfile from "@workspace/api-hooks/admin/useUnblockProfile";
import type { AdminProfileDto } from "@workspace/types/admin";
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
import { Textarea } from "@workspace/ui/components/textarea";
import { cn } from "@workspace/ui/lib/utils";

// ── Stat card ─────────────────────────────────────────────────────────────────

interface StatCardProps {
  icon: React.ElementType;
  iconBg: string;
  iconColor: string;
  label: string;
  value: number | string;
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

// ── Skeleton rows ─────────────────────────────────────────────────────────────

function SkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, i) => (
        <tr key={i} className="border-border border-b">
          <td className="px-4 py-3">
            <div className="flex items-center gap-3">
              <Skeleton className="size-8 rounded-full" />
              <div className="space-y-1.5">
                <Skeleton className="h-3 w-36" />
                <Skeleton className="h-2.5 w-24" />
              </div>
            </div>
          </td>
          <td className="px-3 py-3">
            <Skeleton className="h-3 w-28" />
          </td>
          <td className="px-3 py-3">
            <Skeleton className="h-5 w-20 rounded-full" />
          </td>
          <td className="px-3 py-3">
            <Skeleton className="h-3 w-28" />
          </td>
          <td className="w-20 px-3 py-3" />
        </tr>
      ))}
    </>
  );
}

// ── Profile row ───────────────────────────────────────────────────────────────

function ProfileRow({
  profile,
  onBlock,
  onUnblock,
  onNotify,
  onEdit,
}: {
  profile: AdminProfileDto;
  onBlock: (p: AdminProfileDto) => void;
  onUnblock: (p: AdminProfileDto) => void;
  onNotify: (p: AdminProfileDto) => void;
  onEdit: (id: string) => void;
}) {
  const initials = profile.fullName
    .split(" ")
    .slice(0, 2)
    .map((w) => w[0])
    .join("")
    .toUpperCase();

  const lastLogin = profile.lastLoginAt
    ? new Date(profile.lastLoginAt).toLocaleString("ru-RU", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      })
    : "Никогда";

  return (
    <tr className="group border-border hover:bg-muted/30 border-b last:border-0">
      {/* Name */}
      <td className="min-w-0 px-4 py-3">
        <button
          type="button"
          onClick={() => onEdit(profile.id)}
          className="flex items-center gap-3 text-left"
        >
          <div className="from-primary/60 to-primary flex size-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white">
            {initials}
          </div>
          <div className="min-w-0">
            <p className="text-foreground truncate text-sm font-medium hover:underline">
              {profile.fullName}
            </p>
            <p className="text-muted-foreground truncate text-xs">
              @{profile.userName}
            </p>
          </div>
        </button>
      </td>

      {/* Account ID */}
      <td className="hidden px-3 py-3 lg:table-cell">
        <span className="text-muted-foreground font-mono text-xs">
          {profile.accountId.slice(0, 8)}…
        </span>
      </td>

      {/* Status */}
      <td className="px-3 py-3">
        {profile.isBlocked ? (
          <span className="inline-flex items-center gap-1.5 rounded-full bg-rose-50 px-2.5 py-1 text-[11px] font-semibold text-rose-700">
            <span className="size-1.5 rounded-full bg-rose-500" />
            Заблокирован
          </span>
        ) : (
          <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-2.5 py-1 text-[11px] font-semibold text-emerald-700">
            <span className="size-1.5 rounded-full bg-emerald-500" />
            Активен
          </span>
        )}
      </td>

      {/* Last login */}
      <td className="hidden px-3 py-3 md:table-cell">
        <span className="text-muted-foreground text-xs">{lastLogin}</span>
      </td>

      {/* Actions */}
      <td className="w-20 px-3 py-3">
        <div className="flex items-center justify-end gap-1 opacity-0 transition-opacity group-hover:opacity-100">
          <button
            onClick={() => onNotify(profile)}
            className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-7 items-center justify-center rounded-lg"
            title="Отправить уведомление"
          >
            <Bell className="size-3.5" />
          </button>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button className="text-muted-foreground hover:bg-muted hover:text-foreground flex size-7 items-center justify-center rounded-lg">
                <MoreHorizontal className="size-3.5" />
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
              <DropdownMenuItem onClick={() => onEdit(profile.id)}>
                <Pencil className="size-4" />
                Редактировать
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => onNotify(profile)}>
                <Bell className="size-4" />
                Отправить уведомление
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              {profile.isBlocked ? (
                <DropdownMenuItem onClick={() => onUnblock(profile)}>
                  <ShieldCheck className="size-4" />
                  Разблокировать
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  variant="destructive"
                  onClick={() => onBlock(profile)}
                >
                  <ShieldAlert className="size-4" />
                  Заблокировать
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </td>
    </tr>
  );
}

// ── Send notification dialog ──────────────────────────────────────────────────

function SendNotificationDialog({
  profile,
  onClose,
}: {
  profile: AdminProfileDto | null;
  onClose: () => void;
}) {
  const [title, setTitle] = useState("");
  const [message, setMessage] = useState("");
  const [type, setType] = useState("0");

  const send = useSendAdminNotification({
    onSuccess: () => {
      toast.success("Уведомление отправлено");
      onClose();
      setTitle("");
      setMessage("");
    },
    onError: () => toast.error("Не удалось отправить уведомление"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!profile) return;
    send.mutate({
      profileId: profile.id,
      request: { title, message, type: Number(type) },
    });
  }

  return (
    <Dialog open={!!profile} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Отправить уведомление</DialogTitle>
          <DialogDescription>
            Получатель: <strong>{profile?.fullName}</strong>
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="notif-type">Тип</Label>
            <Select value={type} onValueChange={setType}>
              <SelectTrigger id="notif-type">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="0">Информация</SelectItem>
                <SelectItem value="1">Успех</SelectItem>
                <SelectItem value="2">Предупреждение</SelectItem>
                <SelectItem value="3">Ошибка</SelectItem>
                <SelectItem value="6">Системное</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div className="space-y-2">
            <Label htmlFor="notif-title">Заголовок</Label>
            <Input
              id="notif-title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Заголовок уведомления"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="notif-message">Сообщение</Label>
            <Textarea
              id="notif-message"
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              placeholder="Текст уведомления"
              rows={3}
              required
            />
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Отмена
            </Button>
            <Button type="submit" disabled={send.isPending}>
              {send.isPending && (
                <Loader2 className="mr-2 size-4 animate-spin" />
              )}
              Отправить
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// ── Block confirmation dialog ─────────────────────────────────────────────────

function BlockConfirmDialog({
  profile,
  onConfirm,
  onClose,
  isPending,
}: {
  profile: AdminProfileDto | null;
  onConfirm: () => void;
  onClose: () => void;
  isPending: boolean;
}) {
  return (
    <AlertDialog open={!!profile} onOpenChange={(open) => !open && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Заблокировать пользователя?</AlertDialogTitle>
          <AlertDialogDescription>
            Пользователь <strong>{profile?.fullName}</strong> (@
            {profile?.userName}) не сможет войти в систему. Вы можете
            разблокировать его позже.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Отмена</AlertDialogCancel>
          <AlertDialogAction
            onClick={onConfirm}
            disabled={isPending}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            {isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
            Заблокировать
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

// ── Unblock confirmation dialog ───────────────────────────────────────────────

function UnblockConfirmDialog({
  profile,
  onConfirm,
  onClose,
  isPending,
}: {
  profile: AdminProfileDto | null;
  onConfirm: () => void;
  onClose: () => void;
  isPending: boolean;
}) {
  return (
    <AlertDialog open={!!profile} onOpenChange={(open) => !open && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Разблокировать пользователя?</AlertDialogTitle>
          <AlertDialogDescription>
            Пользователь <strong>{profile?.fullName}</strong> (@
            {profile?.userName}) снова сможет войти в систему.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Отмена</AlertDialogCancel>
          <AlertDialogAction onClick={onConfirm} disabled={isPending}>
            {isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
            Разблокировать
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

// ── Filter tabs ───────────────────────────────────────────────────────────────

const STATUS_TABS = [
  { label: "Все", value: "all" },
  { label: "Активные", value: "active" },
  { label: "Заблокированные", value: "blocked" },
] as const;

type StatusTab = (typeof STATUS_TABS)[number]["value"];

// ── Main page ─────────────────────────────────────────────────────────────────

export function ProfilesPage() {
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [activeTab, setActiveTab] = useState<StatusTab>("all");
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const router = useRouter();
  const [notifyTarget, setNotifyTarget] = useState<AdminProfileDto | null>(
    null,
  );
  const [blockTarget, setBlockTarget] = useState<AdminProfileDto | null>(null);
  const [unblockTarget, setUnblockTarget] = useState<AdminProfileDto | null>(
    null,
  );

  const isBlocked =
    activeTab === "blocked" ? true : activeTab === "active" ? false : undefined;

  const { data, isLoading } = useAdminProfiles({
    pageIndex: page,
    pageSize,
    search: debouncedSearch || undefined,
    isBlocked,
  });

  const profiles = data?.items ?? [];
  const total = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  const blockMutation = useBlockProfile({
    onSuccess: () => {
      toast.success("Пользователь заблокирован");
      setBlockTarget(null);
    },
    onError: () => toast.error("Не удалось заблокировать"),
  });

  const unblockMutation = useUnblockProfile({
    onSuccess: () => {
      toast.success("Блокировка снята");
      setUnblockTarget(null);
    },
    onError: () => toast.error("Не удалось разблокировать"),
  });

  // Debounce search input to avoid excessive API calls
  function handleSearchChange(e: React.ChangeEvent<HTMLInputElement>) {
    const v = e.target.value;
    setSearch(v);
    setPage(1);
    setTimeout(() => setDebouncedSearch(v), 400);
  }

  function handleTabChange(tab: StatusTab) {
    setActiveTab(tab);
    setPage(1);
  }

  return (
    <div className="space-y-6">
      {/* ── Header ── */}
      <div>
        <h1 className="text-foreground text-2xl font-bold tracking-tight">
          Управление профилями
        </h1>
        <p className="text-muted-foreground mt-1 text-sm">
          {total > 0 ? `${total} профилей в системе` : "Нет профилей"}
        </p>
      </div>

      {/* ── Stats ── */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4">
        <StatCard
          icon={Users}
          iconBg="bg-blue-50"
          iconColor="text-blue-600"
          label="Всего профилей"
          value={total}
        />
        <StatCard
          icon={ShieldCheck}
          iconBg="bg-emerald-50"
          iconColor="text-emerald-600"
          label="Активных"
          value={profiles.filter((p) => !p.isBlocked).length}
        />
        <StatCard
          icon={UserX}
          iconBg="bg-rose-50"
          iconColor="text-rose-500"
          label="Заблокированных"
          value={profiles.filter((p) => p.isBlocked).length}
        />
        <StatCard
          icon={Shield}
          iconBg="bg-violet-50"
          iconColor="text-violet-600"
          label="Страница"
          value={`${page} / ${totalPages}`}
        />
      </div>

      {/* ── Table card ── */}
      <div className="bg-card border-border rounded-2xl border shadow-sm">
        {/* Toolbar */}
        <div className="border-border flex flex-wrap items-center gap-3 border-b px-5 py-3">
          {/* Status tabs */}
          <div className="bg-muted/50 flex items-center gap-0.5 rounded-lg p-1">
            {STATUS_TABS.map((tab) => (
              <button
                key={tab.value}
                onClick={() => handleTabChange(tab.value)}
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
              onChange={handleSearchChange}
              placeholder="Поиск по имени или логину…"
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
                <th className="text-muted-foreground px-4 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Пользователь
                </th>
                <th className="text-muted-foreground hidden px-3 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase lg:table-cell">
                  ID аккаунта
                </th>
                <th className="text-muted-foreground px-3 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Статус
                </th>
                <th className="text-muted-foreground hidden px-3 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase md:table-cell">
                  Последний вход
                </th>
                <th className="w-20 px-3 py-2.5" />
              </tr>
            </thead>
            <tbody>
              {isLoading ? (
                <SkeletonRows />
              ) : profiles.length === 0 ? (
                <tr>
                  <td
                    colSpan={5}
                    className="text-muted-foreground px-4 py-12 text-center text-sm"
                  >
                    {search ? "Профили не найдены" : "Нет профилей"}
                  </td>
                </tr>
              ) : (
                profiles.map((profile) => (
                  <ProfileRow
                    key={profile.id}
                    profile={profile}
                    onBlock={setBlockTarget}
                    onUnblock={setUnblockTarget}
                    onNotify={setNotifyTarget}
                    onEdit={(id) => router.push(`/profiles/${id}`)}
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
                const startPage = Math.max(
                  1,
                  Math.min(page - 2, totalPages - 4),
                );
                const p = startPage + i;
                if (p > totalPages) return null;
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
      <SendNotificationDialog
        profile={notifyTarget}
        onClose={() => setNotifyTarget(null)}
      />
      <BlockConfirmDialog
        profile={blockTarget}
        onConfirm={() => blockTarget && blockMutation.mutate(blockTarget.id)}
        onClose={() => setBlockTarget(null)}
        isPending={blockMutation.isPending}
      />
      <UnblockConfirmDialog
        profile={unblockTarget}
        onConfirm={() =>
          unblockTarget && unblockMutation.mutate(unblockTarget.id)
        }
        onClose={() => setUnblockTarget(null)}
        isPending={unblockMutation.isPending}
      />
    </div>
  );
}
