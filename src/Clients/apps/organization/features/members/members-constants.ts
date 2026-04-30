import { Archive, CircleCheck, Trash2, Users } from "lucide-react";

import { OrganizationStatus } from "@workspace/types/company";

export const MEMBER_STATUS_CONFIG: Record<
  OrganizationStatus,
  { label: string; bg: string; fg: string; dot: string }
> = {
  [OrganizationStatus.Active]: {
    label: "Активен",
    bg: "#d1fae5",
    fg: "#047857",
    dot: "#10b981",
  },
  [OrganizationStatus.Archived]: {
    label: "Архив",
    bg: "#f1f5f9",
    fg: "#475569",
    dot: "#94a3b8",
  },
  [OrganizationStatus.Deleted]: {
    label: "Удалён",
    bg: "#fee2e2",
    fg: "#b91c1c",
    dot: "#ef4444",
  },
};

export const MEMBER_STATUS_OPTIONS = [
  { value: OrganizationStatus.Active, label: "Активен", dot: "#10b981" },
  { value: OrganizationStatus.Archived, label: "Архив", dot: "#94a3b8" },
  { value: OrganizationStatus.Deleted, label: "Удалён", dot: "#ef4444" },
];

export const KPI_ITEMS = [
  {
    status: null as OrganizationStatus | null,
    label: "Всего",
    icon: Users,
    iconBg: "bg-slate-100",
    iconColor: "text-slate-500",
    delta: "участников",
  },
  {
    status: OrganizationStatus.Active,
    label: "Активные",
    icon: CircleCheck,
    iconBg: "bg-emerald-50",
    iconColor: "text-emerald-600",
    delta: "в организации",
  },
  {
    status: OrganizationStatus.Archived,
    label: "Архив",
    icon: Archive,
    iconBg: "bg-indigo-50",
    iconColor: "text-indigo-600",
    delta: "неактивных",
  },
  {
    status: OrganizationStatus.Deleted,
    label: "Удалены",
    icon: Trash2,
    iconBg: "bg-amber-50",
    iconColor: "text-amber-600",
    delta: "требуют внимания",
  },
] as const;

// ── Avatar gradients ──────────────────────────────────────────────────────────

const AVATAR_GRADIENTS = [
  "linear-gradient(135deg, #ec4899, #f43f5e)",
  "linear-gradient(135deg, #3b82f6, #06b6d4)",
  "linear-gradient(135deg, #10b981, #22c55e)",
  "linear-gradient(135deg, #f97316, #f59e0b)",
  "linear-gradient(135deg, #8b5cf6, #a855f7)",
  "linear-gradient(135deg, #14b8a6, #06b6d4)",
  "linear-gradient(135deg, #6366f1, #3b82f6)",
  "linear-gradient(135deg, #f43f5e, #ec4899)",
] as const;

function hashString(s: string): number {
  let hash = 0;
  for (let i = 0; i < s.length; i++) {
    hash = (hash * 31 + s.charCodeAt(i)) >>> 0;
  }
  return hash;
}

export function getAvatarGradient(name: string): string {
  return AVATAR_GRADIENTS[hashString(name) % AVATAR_GRADIENTS.length]!;
}

// ── Role colors (deterministic by role code) ──────────────────────────────────

const ROLE_PALETTES: ReadonlyArray<{ bg: string; fg: string }> = [
  { bg: "rgba(139,92,246,0.12)", fg: "#6d28d9" }, // violet
  { bg: "rgba(79,70,229,0.12)", fg: "#4338ca" }, // indigo
  { bg: "rgba(14,165,233,0.12)", fg: "#0369a1" }, // blue
  { bg: "rgba(20,184,166,0.12)", fg: "#0f766e" }, // teal
  { bg: "rgba(245,158,11,0.14)", fg: "#92400e" }, // amber
  { bg: "#f1f5f9", fg: "#475569" }, // slate
  { bg: "rgba(236,72,153,0.12)", fg: "#9d174d" }, // pink
  { bg: "rgba(16,185,129,0.12)", fg: "#047857" }, // emerald
];

export function getRoleColor(role: string): { bg: string; fg: string } {
  return ROLE_PALETTES[hashString(role) % ROLE_PALETTES.length]!;
}
