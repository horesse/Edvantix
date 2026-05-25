"use client";

import { useRouter } from "next/navigation";

import { ChevronRight, Lock } from "lucide-react";

import type { RoleDto } from "@workspace/types/organization";

import { declMembers, getRoleAvatarColors } from "./roles-constants";

interface RoleCardProps {
  role: RoleDto;
  orgId: string;
}

export function RoleCard({ role, orgId: _orgId }: Readonly<RoleCardProps>) {
  const router = useRouter();
  const colors = getRoleAvatarColors(role.name);

  const granted = role.permissionsCount;
  const total = role.totalPermissionsCount;
  const pct = total > 0 ? Math.round((granted / total) * 100) : 0;

  return (
    <button
      type="button"
      onClick={() => router.push(`/organization/roles/${role.id}`)}
      className="group flex w-full cursor-pointer items-center gap-4 rounded-2xl border border-slate-200 bg-white p-[18px_20px] text-left transition-all hover:border-indigo-200 hover:shadow-md focus-visible:ring-2 focus-visible:ring-indigo-400 focus-visible:outline-none"
    >
      {/* Avatar */}
      <div
        className="flex size-11 shrink-0 items-center justify-center rounded-xl text-[17px] font-bold"
        style={{ background: colors.bg, color: colors.fg }}
      >
        {role.name.charAt(0)}
      </div>

      {/* Name + description */}
      <div className="min-w-0 flex-1">
        <div className="mb-0.5 flex items-center gap-2">
          <span className="text-[15px] font-semibold text-slate-900">
            {role.name}
          </span>
          {role.isSystem && (
            <span className="inline-flex items-center gap-1 rounded-full bg-slate-100 px-2 py-0.5 text-[11px] font-medium text-slate-500">
              <Lock className="size-2.5" />
              системная
            </span>
          )}
        </div>
        <p className="line-clamp-1 text-[13px] leading-relaxed text-slate-500">
          {role.description ?? "—"}
        </p>
      </div>

      {/* Permissions bar */}
      <div
        className="flex shrink-0 flex-col items-end gap-1"
        style={{ minWidth: 140 }}
      >
        <span className="text-[13px] font-medium text-slate-900">
          {granted}{" "}
          <span className="font-normal text-slate-400">из {total} прав</span>
        </span>
        <div className="h-1.5 w-36 overflow-hidden rounded-full bg-slate-100">
          <div
            className="h-full rounded-full"
            style={{
              width: `${pct}%`,
              background: "linear-gradient(90deg, #6366f1, #818cf8)",
            }}
          />
        </div>
        <span className="text-[11.5px] text-slate-500">
          {role.membersCount} {declMembers(role.membersCount)}
        </span>
      </div>

      <ChevronRight className="size-4.5 shrink-0 text-slate-300 transition-transform group-hover:translate-x-0.5" />
    </button>
  );
}
