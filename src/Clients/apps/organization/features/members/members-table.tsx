"use client";

import { useEffect, useState } from "react";

import { ChevronLeft, ChevronRight, X } from "lucide-react";

import useOrganizationMembers from "@workspace/api-hooks/company/useOrganizationMembers";
import type { OrganizationMemberDto } from "@workspace/types/company";
import { OrganizationStatus } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import { Checkbox } from "@workspace/ui/components/checkbox";
import {
  Empty,
  EmptyDescription,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
} from "@workspace/ui/components/empty";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { MEMBER_STATUS_OPTIONS } from "./members-constants";
import { FilterDropdown } from "./members-filter-dropdown";
import { MembersTableRow } from "./members-table-row";

// ── Types ─────────────────────────────────────────────────────────────────────

type SortField = "fullName" | "role" | "status" | "lastActivity";
type SortDir = "asc" | "desc";

interface SortState {
  field: SortField;
  dir: SortDir;
}

// ── Skeleton rows ─────────────────────────────────────────────────────────────

function SkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, i) => (
        <tr key={i} className="border-border border-b">
          <td className="w-12 px-4 py-3 sm:px-5">
            <Skeleton className="size-4 rounded" />
          </td>
          <td className="px-2 py-3">
            <div className="flex items-center gap-3">
              <Skeleton className="size-9 rounded-full" />
              <Skeleton className="h-3 w-48" />
            </div>
          </td>
          <td className="hidden px-2 py-3 sm:table-cell">
            <Skeleton className="h-5 w-20 rounded-md" />
          </td>
          <td className="hidden px-2 py-3 md:table-cell">
            <Skeleton className="h-5 w-16 rounded-full" />
          </td>
          <td className="hidden px-2 py-3 lg:table-cell">
            <Skeleton className="h-4 w-20" />
          </td>
          <td className="px-3 py-3" />
        </tr>
      ))}
    </>
  );
}

// ── Sort header ───────────────────────────────────────────────────────────────

interface SortHeaderProps {
  field: SortField;
  sort: SortState;
  onSort: (field: SortField) => void;
  children: React.ReactNode;
  className?: string;
}

function SortHeader({
  field,
  sort,
  onSort,
  children,
  className,
}: Readonly<SortHeaderProps>) {
  const active = sort.field === field;

  return (
    <th
      className={cn(
        "bg-muted/50 border-border border-b px-2 py-3 text-left text-[11px] font-semibold tracking-wider text-slate-500 uppercase",
        className,
      )}
    >
      <button
        type="button"
        onClick={() => onSort(field)}
        className="inline-flex items-center gap-1 hover:text-slate-700"
        style={{ color: active ? "#0f172a" : "#64748b" }}
      >
        {children}
        <svg
          width="12"
          height="12"
          viewBox="0 0 12 12"
          style={{ opacity: active ? 1 : 0.3 }}
        >
          <path
            d="M6 2l3 3H3z"
            fill={active && sort.dir === "desc" ? "#cbd5e1" : "currentColor"}
          />
          <path
            d="M6 10l-3-3h6z"
            fill={active && sort.dir === "asc" ? "#cbd5e1" : "currentColor"}
          />
        </svg>
      </button>
    </th>
  );
}

// ── Selection bar ─────────────────────────────────────────────────────────────

interface SelectionBarProps {
  count: number;
  onClear: () => void;
}

function SelectionBar({ count, onClear }: Readonly<SelectionBarProps>) {
  return (
    <div
      className="border-b px-4 py-2.5"
      style={{
        background: "rgba(79,70,229,0.05)",
        borderColor: "#e0eaff",
      }}
    >
      <div className="flex items-center gap-3">
        <span className="text-sm font-medium" style={{ color: "#4338ca" }}>
          Выбрано: {count}
        </span>
        <div className="h-4 w-px bg-indigo-200" />
        <button
          type="button"
          onClick={onClear}
          className="ml-auto flex items-center gap-1.5 rounded-lg px-2.5 py-1 text-xs font-medium text-slate-500 transition-colors hover:bg-white"
        >
          <X className="size-3" />
          Снять выделение
        </button>
      </div>
    </div>
  );
}

// ── Pagination ────────────────────────────────────────────────────────────────

interface PaginationProps {
  page: number;
  totalPages: number;
  total: number;
  pageSize: number;
  onPageChange: (page: number) => void;
}

function Pagination({
  page,
  totalPages,
  total,
  pageSize,
  onPageChange,
}: Readonly<PaginationProps>) {
  const from = (page - 1) * pageSize + 1;
  const to = Math.min(page * pageSize, total);

  const visiblePages = (() => {
    if (totalPages <= 7)
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    if (page <= 4) return [1, 2, 3, 4, 5, -1, totalPages];
    if (page >= totalPages - 3)
      return [
        1,
        -1,
        totalPages - 4,
        totalPages - 3,
        totalPages - 2,
        totalPages - 1,
        totalPages,
      ];
    return [1, -1, page - 1, page, page + 1, -2, totalPages];
  })();

  return (
    <div className="border-border flex items-center justify-between border-t px-4 py-3 sm:px-5">
      <p className="text-muted-foreground text-xs">
        {from}–{to} из {total}
      </p>
      <div className="flex items-center gap-1">
        <button
          type="button"
          onClick={() => onPageChange(Math.max(1, page - 1))}
          disabled={page === 1}
          className="border-border text-muted-foreground hover:bg-muted flex size-8 items-center justify-center rounded-lg border transition-colors disabled:pointer-events-none disabled:opacity-40"
        >
          <ChevronLeft className="size-4" />
        </button>

        {visiblePages.map((p, i) =>
          p < 0 ? (
            <span
              key={`ellipsis-${i}`}
              className="text-muted-foreground flex size-8 items-center justify-center text-xs"
            >
              …
            </span>
          ) : (
            <button
              key={p}
              type="button"
              onClick={() => onPageChange(p)}
              className={cn(
                "flex size-8 items-center justify-center rounded-lg text-xs font-medium transition-colors",
                p === page
                  ? "bg-primary text-white"
                  : "border-border text-muted-foreground hover:bg-muted border",
              )}
            >
              {p}
            </button>
          ),
        )}

        <button
          type="button"
          onClick={() => onPageChange(Math.min(totalPages, page + 1))}
          disabled={page === totalPages}
          className="border-border text-muted-foreground hover:bg-muted flex size-8 items-center justify-center rounded-lg border transition-colors disabled:pointer-events-none disabled:opacity-40"
        >
          <ChevronRight className="size-4" />
        </button>
      </div>
    </div>
  );
}

// ── Main table component ──────────────────────────────────────────────────────

interface MembersTableProps {
  orgId: string;
  canManage: boolean;
  selected: Set<string>;
  onSelect: (id: string, checked: boolean) => void;
  onSelectAll: (checked: boolean, ids: string[]) => void;
  onClearSelection: () => void;
  onChangeRole: (m: OrganizationMemberDto) => void;
  onRemove: (m: OrganizationMemberDto) => void;
}

const PAGE_SIZE = 20;

export function MembersTable({
  orgId,
  canManage,
  selected,
  onSelect,
  onSelectAll,
  onClearSelection,
  onChangeRole,
  onRemove,
}: Readonly<MembersTableProps>) {
  const [statusFilter, setStatusFilter] = useState<Set<OrganizationStatus>>(
    new Set(),
  );
  const [sort, setSort] = useState<SortState>({
    field: "lastActivity",
    dir: "desc",
  });
  const [page, setPage] = useState(1);

  // Reset page when filter changes
  useEffect(() => {
    setPage(1);
  }, [statusFilter]);

  const activeStatus =
    statusFilter.size === 1 ? [...statusFilter][0] : undefined;

  const { data, isLoading } = useOrganizationMembers(orgId, {
    pageIndex: page,
    pageSize: PAGE_SIZE,
    ...(activeStatus !== undefined ? { status: activeStatus } : {}),
  });

  const members = data?.items ?? [];
  const total = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));

  // Client-side sort of the current page
  const sorted = [...members].sort((a, b) => {
    let cmp: number;
    if (sort.field === "fullName") {
      cmp = a.fullName.localeCompare(b.fullName, "ru");
    } else if (sort.field === "role") {
      cmp = a.role.localeCompare(b.role, "ru");
    } else if (sort.field === "status") {
      cmp = a.status - b.status;
    } else {
      const ta = a.lastActivity ? new Date(a.lastActivity).getTime() : 0;
      const tb = b.lastActivity ? new Date(b.lastActivity).getTime() : 0;
      cmp = ta - tb;
    }
    return sort.dir === "asc" ? cmp : -cmp;
  });

  function handleSort(field: SortField) {
    setSort((s) =>
      s.field === field
        ? { field, dir: s.dir === "asc" ? "desc" : "asc" }
        : { field, dir: "asc" },
    );
  }

  const allOnPageSelected =
    sorted.length > 0 && sorted.every((m) => selected.has(m.id));
  const someOnPageSelected = sorted.some((m) => selected.has(m.id));
  const headerChecked = allOnPageSelected
    ? true
    : someOnPageSelected
      ? ("indeterminate" as const)
      : false;

  return (
    <div className="bg-card border-border overflow-hidden rounded-2xl border shadow-sm">
      {/* Toolbar */}
      <div className="border-border flex flex-wrap items-center gap-3 border-b px-4 py-3 sm:px-5">
        <FilterDropdown
          label="Статус"
          options={MEMBER_STATUS_OPTIONS}
          value={statusFilter}
          onChange={setStatusFilter}
        />

        {statusFilter.size > 0 && (
          <button
            type="button"
            onClick={() => setStatusFilter(new Set())}
            className="text-muted-foreground hover:text-foreground text-sm transition-colors"
          >
            Сбросить
          </button>
        )}

        <div className="ml-auto text-xs text-slate-400">
          Найдено:{" "}
          <strong className="text-foreground font-semibold tabular-nums">
            {isLoading ? "…" : total}
          </strong>
        </div>
      </div>

      {/* Selection bar */}
      {selected.size > 0 && (
        <SelectionBar count={selected.size} onClear={onClearSelection} />
      )}

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr>
              <th className="bg-muted/50 border-border w-12 border-b px-4 py-3 sm:px-5">
                <Checkbox
                  checked={headerChecked}
                  onCheckedChange={(v) =>
                    onSelectAll(
                      Boolean(v),
                      sorted.map((m) => m.id),
                    )
                  }
                  aria-label="Выбрать всех"
                />
              </th>
              <SortHeader field="fullName" sort={sort} onSort={handleSort}>
                Участник
              </SortHeader>
              <SortHeader
                field="role"
                sort={sort}
                onSort={handleSort}
                className="hidden sm:table-cell"
              >
                Роль
              </SortHeader>
              <SortHeader
                field="status"
                sort={sort}
                onSort={handleSort}
                className="hidden md:table-cell"
              >
                Статус
              </SortHeader>
              <SortHeader
                field="lastActivity"
                sort={sort}
                onSort={handleSort}
                className="hidden lg:table-cell"
              >
                Активность
              </SortHeader>
              <th className="bg-muted/50 border-border w-12 border-b px-3 py-3" />
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              <SkeletonRows />
            ) : sorted.length === 0 ? (
              <tr>
                <td colSpan={6} className="px-4 py-16">
                  <Empty className="border-0">
                    <EmptyMedia variant="icon">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      >
                        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                        <circle cx="9" cy="7" r="4" />
                        <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
                        <path d="M16 3.13a4 4 0 0 1 0 7.75" />
                      </svg>
                    </EmptyMedia>
                    <EmptyHeader>
                      <EmptyTitle>Участники не найдены</EmptyTitle>
                      <EmptyDescription>
                        {statusFilter.size > 0
                          ? "Попробуйте изменить или сбросить фильтры"
                          : "В этой организации пока нет участников"}
                      </EmptyDescription>
                    </EmptyHeader>
                    {statusFilter.size > 0 && (
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setStatusFilter(new Set())}
                      >
                        Сбросить фильтры
                      </Button>
                    )}
                  </Empty>
                </td>
              </tr>
            ) : (
              sorted.map((member) => (
                <MembersTableRow
                  key={member.id}
                  member={member}
                  selected={selected.has(member.id)}
                  onSelect={onSelect}
                  canManage={canManage}
                  onChangeRole={onChangeRole}
                  onRemove={onRemove}
                />
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {!isLoading && total > 0 && (
        <Pagination
          page={page}
          totalPages={totalPages}
          total={total}
          pageSize={PAGE_SIZE}
          onPageChange={setPage}
        />
      )}
    </div>
  );
}
