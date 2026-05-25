"use client";

import { useState } from "react";

import { MoreVertical, Pencil, Trash2 } from "lucide-react";

import type { OrganizationMemberDto } from "@workspace/types/organization";
import { Checkbox } from "@workspace/ui/components/checkbox";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";

import {
  MEMBER_STATUS_CONFIG,
  getAvatarGradient,
  getRoleColor,
} from "./members-constants";

// ── Helpers ───────────────────────────────────────────────────────────────────

function formatLastActivity(iso: string | null): string {
  if (!iso) return "—";
  const date = new Date(iso);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60_000);
  if (diffMins < 1) return "только что";
  if (diffMins < 60) return `${diffMins} мин назад`;
  const diffHours = Math.floor(diffMins / 60);
  if (diffHours < 24) return `${diffHours} ч назад`;
  const diffDays = Math.floor(diffHours / 24);
  if (diffDays === 1) return "вчера";
  if (diffDays < 30) return `${diffDays} д назад`;
  return date.toLocaleDateString("ru-RU");
}

// ── Status pill ───────────────────────────────────────────────────────────────

function StatusPill({
  status,
}: Readonly<{ status: OrganizationMemberDto["status"] }>) {
  const cfg = MEMBER_STATUS_CONFIG[status];
  return (
    <span
      className="inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs leading-snug font-medium"
      style={{ background: cfg.bg, color: cfg.fg }}
    >
      <span
        className="size-1.5 shrink-0 rounded-full"
        style={{ background: cfg.dot }}
      />
      {cfg.label}
    </span>
  );
}

// ── Role badge ────────────────────────────────────────────────────────────────

function RoleBadge({ role }: Readonly<{ role: string | null | undefined }>) {
  if (!role) {
    return <span className="text-muted-foreground text-xs">—</span>;
  }
  const { bg, fg } = getRoleColor(role);
  return (
    <span
      className="inline-block rounded-md px-2.5 py-1 text-xs font-medium"
      style={{ background: bg, color: fg }}
    >
      {role}
    </span>
  );
}

// ── Avatar ────────────────────────────────────────────────────────────────────

function MemberAvatar({
  fullName,
  avatarUrl,
}: Readonly<{ fullName: string; avatarUrl: string | null }>) {
  const [imgError, setImgError] = useState(false);

  if (avatarUrl && !imgError) {
    return (
      // eslint-disable-next-line @next/next/no-img-element
      <img
        src={avatarUrl}
        alt={fullName}
        onError={() => setImgError(true)}
        className="size-9 shrink-0 rounded-full object-cover"
      />
    );
  }

  const parts = fullName.trim().split(/\s+/);
  const initials =
    ((parts[0]?.[0] ?? "") + (parts[1]?.[0] ?? "")).toUpperCase() ||
    fullName.slice(0, 2).toUpperCase();

  return (
    <div
      className="flex size-9 shrink-0 items-center justify-center rounded-full text-xs font-bold text-white"
      style={{ background: getAvatarGradient(fullName) }}
    >
      {initials}
    </div>
  );
}

// ── Row action menu ───────────────────────────────────────────────────────────

interface RowMenuProps {
  member: OrganizationMemberDto;
  onChangeRole: (m: OrganizationMemberDto) => void;
  onRemove: (m: OrganizationMemberDto) => void;
}

function RowMenu({ member, onChangeRole, onRemove }: Readonly<RowMenuProps>) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          type="button"
          aria-label="Действия"
          className="flex size-8 items-center justify-center rounded-lg text-slate-400 transition-colors hover:bg-slate-100 hover:text-slate-600 data-[state=open]:bg-slate-100 data-[state=open]:text-slate-600"
        >
          <MoreVertical className="size-4" />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-48">
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
  );
}

// ── Table row ─────────────────────────────────────────────────────────────────

interface MembersTableRowProps {
  member: OrganizationMemberDto;
  selected: boolean;
  onSelect: (id: string, checked: boolean) => void;
  canManage: boolean;
  onChangeRole: (m: OrganizationMemberDto) => void;
  onRemove: (m: OrganizationMemberDto) => void;
}

export function MembersTableRow({
  member,
  selected,
  onSelect,
  canManage,
  onChangeRole,
  onRemove,
}: Readonly<MembersTableRowProps>) {
  return (
    <tr
      className="border-border group border-b transition-colors last:border-0"
      style={{
        background: selected ? "rgba(79,70,229,0.03)" : "transparent",
      }}
      onMouseEnter={(e) => {
        if (!selected) e.currentTarget.style.background = "#fafbfc";
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.background = selected
          ? "rgba(79,70,229,0.03)"
          : "transparent";
      }}
    >
      {/* Checkbox */}
      <td className="w-12 px-4 py-3 sm:px-5">
        <Checkbox
          checked={selected}
          onCheckedChange={(v) => onSelect(member.id, Boolean(v))}
          aria-label="Выбрать участника"
        />
      </td>

      {/* Participant */}
      <td className="min-w-0 px-2 py-3">
        <div className="flex items-center gap-3">
          <MemberAvatar
            fullName={member.fullName}
            avatarUrl={member.avatarUrl}
          />
          <div className="min-w-0">
            <p className="text-foreground truncate text-sm font-medium">
              {member.fullName}
            </p>
          </div>
        </div>
      </td>

      {/* Role */}
      <td className="hidden px-2 py-3 sm:table-cell">
        <RoleBadge role={member.role} />
      </td>

      {/* Status */}
      <td className="hidden px-2 py-3 md:table-cell">
        <StatusPill status={member.status} />
      </td>

      {/* Last activity */}
      <td className="hidden px-2 py-3 text-sm text-slate-500 lg:table-cell">
        {formatLastActivity(member.lastActivity)}
      </td>

      {/* Actions */}
      <td className="px-3 py-3 text-right">
        {canManage ? (
          <RowMenu
            member={member}
            onChangeRole={onChangeRole}
            onRemove={onRemove}
          />
        ) : (
          <div className="size-8" />
        )}
      </td>
    </tr>
  );
}
