"use client";

import { useMemo, useRef, useState } from "react";

import { Download, Inbox, Info, Plus, Upload } from "lucide-react";
import { toast } from "sonner";

import useArchiveDirectoryItem from "@workspace/api-hooks/organization/useArchiveDirectoryItem";
import useCreateDirectoryItem from "@workspace/api-hooks/organization/useCreateDirectoryItem";
import useDirectoryList from "@workspace/api-hooks/organization/useDirectoryList";
import useReorderDirectory from "@workspace/api-hooks/organization/useReorderDirectory";
import useRestoreDirectoryItem from "@workspace/api-hooks/organization/useRestoreDirectoryItem";
import useUpdateDirectoryItem from "@workspace/api-hooks/organization/useUpdateDirectoryItem";
import type { DirectoryItemBase } from "@workspace/types/organization";
import { Button } from "@workspace/ui/components/button";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@workspace/ui/components/tooltip";

import { PageBreadcrumb } from "@/components/layout/page-breadcrumb";
import { useOrganization } from "@/components/organization/provider";

import type { DirectoryConfig } from "./directory-config";
import { DirectoryDrawer } from "./directory-drawer";
import { DirectoryTable } from "./directory-table";

const DEFAULT_PAGE_SIZE = 20;

interface DirectoryPageProps<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
> {
  config: DirectoryConfig<TItem, TForm>;
}

/** Generic-страница справочника. Принимает конфиг и рендерит полный UI. */
export function DirectoryPage<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
>({ config }: Readonly<DirectoryPageProps<TItem, TForm>>) {
  const { currentOrg } = useOrganization();
  const orgId = currentOrg?.id ?? "";

  const [tab, setTab] = useState<"active" | "archived">("active");
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [pageIndex, setPageIndex] = useState(0);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<"create" | "edit">("create");
  const [editItem, setEditItem] = useState<TItem | null>(null);

  const searchTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  function handleSearchChange(value: string) {
    setSearch(value);
    setPageIndex(0);
    if (searchTimerRef.current) clearTimeout(searchTimerRef.current);
    searchTimerRef.current = setTimeout(() => setDebouncedSearch(value), 350);
  }

  const query = useMemo(
    () => ({
      search: debouncedSearch || undefined,
      includeArchived: tab === "archived",
      pageIndex,
      pageSize: DEFAULT_PAGE_SIZE,
    }),
    [debouncedSearch, tab, pageIndex],
  );

  const { data: listData, isLoading } = useDirectoryList<TItem>(
    orgId,
    config.code,
    query,
  );

  const activeQuery = useMemo(
    () => ({ includeArchived: false, pageSize: 1000 }),
    [],
  );
  const archivedQuery = useMemo(
    () => ({ includeArchived: true, pageSize: 1000 }),
    [],
  );
  const { data: allActive } = useDirectoryList<TItem>(
    orgId,
    config.code,
    activeQuery,
  );
  const { data: allArchived } = useDirectoryList<TItem>(
    orgId,
    config.code,
    archivedQuery,
  );

  const activeCount = allActive?.totalCount ?? 0;
  const archivedCount = (allArchived?.totalCount ?? 0) - activeCount;

  const items = listData?.items ?? [];
  const totalCount = listData?.totalCount ?? 0;
  const totalPages = Math.ceil(totalCount / DEFAULT_PAGE_SIZE);

  const createMutation = useCreateDirectoryItem<unknown>(config.code);
  const updateMutation = useUpdateDirectoryItem<unknown>(config.code);
  const archiveMutation = useArchiveDirectoryItem(config.code);
  const restoreMutation = useRestoreDirectoryItem(config.code);
  const reorderMutation = useReorderDirectory(config.code);

  function openCreate() {
    setDrawerMode("create");
    setEditItem(null);
    setDrawerOpen(true);
  }

  function openEdit(item: TItem) {
    setDrawerMode("edit");
    setEditItem(item);
    setDrawerOpen(true);
  }

  async function handleCreate(orgIdArg: string, request: unknown) {
    await createMutation.mutateAsync({ orgId: orgIdArg, request });
  }

  async function handleUpdate(orgIdArg: string, id: string, request: unknown) {
    await updateMutation.mutateAsync({ orgId: orgIdArg, id, request });
  }

  async function handleArchive(item: TItem) {
    try {
      await archiveMutation.mutateAsync({ orgId, id: item.id });
    } catch (error) {
      toast.error("Не удалось архивировать");
      throw error;
    }
  }

  async function handleRestore(item: TItem) {
    try {
      await restoreMutation.mutateAsync({ orgId, id: item.id });
    } catch (error) {
      toast.error("Не удалось восстановить");
      throw error;
    }
  }

  function handleReorder(orderedIds: string[]) {
    reorderMutation.mutate(
      { orgId, request: { orderedIds } },
      { onError: () => toast.error("Не удалось сохранить порядок") },
    );
  }

  const Icon = config.icon;

  return (
    // Вырываемся из внешнего px-4/lg:px-6 основного лейаута чтобы breadcrumb
    // и header шли от края до края, как в дизайне.
    <div className="-mx-4 -mt-4 flex flex-col gap-0 lg:-mx-6 lg:-mt-6">
      {/* Breadcrumb */}
      <div className="border-b bg-white px-6 py-3.5 lg:px-8">
        <PageBreadcrumb
          items={[
            { label: "Настройки", href: "/organization/settings" },
            { label: "Справочники", href: "/organization/settings" },
          ]}
          currentPage={config.plural}
        />
      </div>

      {/* Page header */}
      <div className="border-b bg-white px-6 py-6 lg:px-8">
        <div className="flex items-center gap-4">
          <div className="flex size-11 shrink-0 items-center justify-center rounded-xl bg-indigo-50 text-indigo-700">
            <Icon className="size-[22px]" />
          </div>
          <h1 className="min-w-0 flex-1 text-2xl font-bold tracking-tight text-slate-900">
            {config.plural}
          </h1>
          <div className="flex shrink-0 items-center gap-2">
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span>
                    <Button variant="outline" size="sm" disabled>
                      <Upload className="size-4" />
                      Импорт
                    </Button>
                  </span>
                </TooltipTrigger>
                <TooltipContent>Скоро</TooltipContent>
              </Tooltip>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span>
                    <Button variant="outline" size="sm" disabled>
                      <Download className="size-4" />
                      Экспорт
                    </Button>
                  </span>
                </TooltipTrigger>
                <TooltipContent>Скоро</TooltipContent>
              </Tooltip>
            </TooltipProvider>
            <Button size="sm" onClick={openCreate}>
              <Plus className="size-4" />
              Добавить {config.singular}
            </Button>
          </div>
        </div>
        <p className="mt-3 max-w-2xl text-sm text-slate-500 lg:pl-[60px]">
          {config.description}
        </p>
      </div>

      {/* Body */}
      <div className="flex-1 overflow-y-auto px-6 py-5 pb-12 lg:px-8">
        <div className="mx-auto max-w-[1180px] space-y-4">
          {/* Tabs + search + counters */}
          <div className="flex items-center gap-4">
            {/* Tabs */}
            <div className="flex gap-1 rounded-xl border bg-white p-1">
              {(
                [
                  ["active", "Активные", activeCount],
                  ["archived", "Архив", archivedCount],
                ] as const
              ).map(([value, label, count]) => {
                const active = tab === value;
                return (
                  <button
                    key={value}
                    type="button"
                    onClick={() => {
                      setTab(value);
                      setPageIndex(0);
                    }}
                    className={`inline-flex items-center gap-2 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
                      active
                        ? "bg-indigo-600 text-white"
                        : "text-slate-500 hover:text-slate-700"
                    }`}
                  >
                    {label}
                    <span
                      className={`rounded-full px-1.5 py-0.5 text-[11px] font-semibold tabular-nums ${
                        active
                          ? "bg-white/20 text-white"
                          : "bg-slate-100 text-slate-500"
                      }`}
                    >
                      {count}
                    </span>
                  </button>
                );
              })}
            </div>

            {/* Search */}
            <div className="relative max-w-xs flex-1">
              <svg
                className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                strokeWidth={2}
              >
                <circle cx="11" cy="11" r="8" />
                <path d="m21 21-4.35-4.35" />
              </svg>
              <input
                type="search"
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                placeholder={`Поиск по ${config.plural.toLowerCase()}`}
                className="h-9 w-full rounded-xl border bg-white pr-3 pl-9 text-sm transition-shadow outline-none focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
              />
            </div>

            {/* Counters */}
            <div className="ml-auto text-sm text-slate-500">
              Показано:{" "}
              <strong className="text-slate-900">
                {isLoading ? "…" : items.length}
              </strong>
              <span className="mx-2 text-slate-300">·</span>
              Всего:{" "}
              <strong className="text-slate-900">
                {isLoading ? "…" : totalCount}
              </strong>
            </div>
          </div>

          {/* Table or empty state */}
          {!isLoading && items.length === 0 ? (
            <div className="flex flex-col items-center justify-center rounded-xl border border-dashed bg-white py-14 text-center">
              <div className="mb-4 flex size-14 items-center justify-center rounded-2xl bg-indigo-50 text-indigo-500">
                <Inbox className="size-7" />
              </div>
              <p className="mb-1.5 text-base font-semibold text-slate-900">
                {search ? "Ничего не найдено" : "Список пуст"}
              </p>
              <p className="mb-5 max-w-xs text-sm text-slate-500">
                {search
                  ? `По запросу «${search}» нет совпадений. Попробуйте изменить запрос.`
                  : `Здесь будут все ${config.plural.toLowerCase()}. Добавьте первую запись, чтобы начать.`}
              </p>
              {!search && (
                <Button size="sm" onClick={openCreate}>
                  <Plus className="size-4" />
                  Добавить {config.singular}
                </Button>
              )}
            </div>
          ) : (
            <DirectoryTable
              config={config}
              items={items}
              activeTab={tab}
              onEdit={openEdit}
              onArchive={handleArchive}
              onRestore={handleRestore}
              onReorder={handleReorder}
            />
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between text-sm text-slate-500">
              <span>
                {pageIndex * DEFAULT_PAGE_SIZE + 1}–
                {Math.min((pageIndex + 1) * DEFAULT_PAGE_SIZE, totalCount)} из{" "}
                {totalCount}
              </span>
              <div className="flex items-center gap-1">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={pageIndex === 0}
                  onClick={() => setPageIndex((p) => p - 1)}
                >
                  ←
                </Button>
                <span className="px-2 tabular-nums">
                  {pageIndex + 1} / {totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={pageIndex >= totalPages - 1}
                  onClick={() => setPageIndex((p) => p + 1)}
                >
                  →
                </Button>
              </div>
            </div>
          )}

          {/* Footer hint */}
          <div className="flex items-center gap-2 text-xs text-slate-400">
            <Info className="size-3.5 shrink-0" />
            Порядок записей определяет, как они отображаются в выпадающих
            списках и фильтрах. Удалить можно только записи, которые ни в чём не
            используются — иначе переведите в «Архив».
          </div>
        </div>
      </div>

      <DirectoryDrawer
        config={config}
        orgId={orgId}
        open={drawerOpen}
        mode={drawerMode}
        item={editItem}
        onClose={() => setDrawerOpen(false)}
        onCreate={handleCreate}
        onUpdate={handleUpdate}
        onArchive={handleArchive}
        onRestore={handleRestore}
      />
    </div>
  );
}
