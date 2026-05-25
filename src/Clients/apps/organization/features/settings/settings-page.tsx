"use client";

import { useMemo, useState } from "react";

import { Search, Settings, X } from "lucide-react";

import useDirectoriesCatalog from "@workspace/api-hooks/organization/useDirectoriesCatalog";
import useOrganizationSummary from "@workspace/api-hooks/organization/useOrganizationSummary";
import useRolesSummary from "@workspace/api-hooks/organization/useRolesSummary";

import { PageLayout } from "@/components/layout/page-layout";
import { useOrganization } from "@/components/organization/provider";

import { PageBreadcrumb } from "@/components/layout/page-breadcrumb";

import { EmptySearchPage } from "./components/empty-search";
import { SettingsSkeleton } from "./components/settings-skeleton";
import { PLATFORM_ITEMS } from "./constants";
import { AccessSection } from "./sections/access-section";
import { DirectoriesSection } from "./sections/directories-section";
import { OrgSection } from "./sections/org-section";
import { PlatformSection } from "./sections/platform-section";

export function SettingsPage() {
  const { currentOrg } = useOrganization();
  const orgId = currentOrg?.id ?? "";

  const [query, setQuery] = useState("");

  const { isLoading: orgLoading } = useOrganizationSummary(orgId, {
    enabled: Boolean(orgId),
  });
  const { data: directories, isLoading: dirsLoading } = useDirectoriesCatalog(
    orgId,
    { enabled: Boolean(orgId) },
  );
  const { isLoading: rolesLoading } = useRolesSummary(orgId, {
    enabled: Boolean(orgId),
  });

  const isLoading = orgLoading || dirsLoading || rolesLoading;

  const q = query.trim().toLowerCase();

  const dirsFiltered = useMemo(
    () =>
      (directories ?? []).filter((d) => {
        if (!q) return true;
        return (
          d.name.toLowerCase().includes(q) ||
          d.description.toLowerCase().includes(q)
        );
      }),
    [directories, q],
  );

  const platformFiltered = useMemo(
    () =>
      PLATFORM_ITEMS.filter((p) => {
        if (!q) return true;
        return (
          p.name.toLowerCase().includes(q) ||
          p.description.toLowerCase().includes(q)
        );
      }),
    [q],
  );

  if (!currentOrg) {
    return (
      <p className="text-muted-foreground py-16 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  const orgMatches =
    !q ||
    "организация".includes(q) ||
    (currentOrg.shortName ?? "").toLowerCase().includes(q) ||
    currentOrg.fullLegalName.toLowerCase().includes(q);

  const rolesMatch =
    !q ||
    "роли".includes(q) ||
    "доступы".includes(q) ||
    "права".includes(q);

  const sectionVisible = {
    org: orgMatches,
    directories: !q || dirsFiltered.length > 0,
    access: rolesMatch,
    platform: !q || platformFiltered.length > 0,
  };

  const anyVisible =
    sectionVisible.org ||
    sectionVisible.directories ||
    sectionVisible.access ||
    sectionVisible.platform;

  return (
    <PageLayout
      header={
        <div className="space-y-[18px]">
          <PageBreadcrumb
            items={
              currentOrg
                ? [{ label: currentOrg.shortName ?? currentOrg.fullLegalName, href: "/" }]
                : []
            }
            currentPage="Настройки"
          />
          {/* Page title row */}
          <div className="flex items-center gap-[18px]">
            <div className="flex size-12 shrink-0 items-center justify-center rounded-[12px] bg-indigo-50 text-indigo-700">
              <Settings className="size-6" />
            </div>
            <div className="min-w-0 flex-1">
              <h1 className="text-2xl font-bold tracking-[-0.02em] text-slate-900">
                Настройки
              </h1>
              <p className="mt-0.5 text-[13.5px] text-slate-500">
                Управление организацией, справочниками и подключёнными сервисами
              </p>
            </div>
          </div>

          {/* Search */}
          <div className="relative max-w-[480px]">
            <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
            <input
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Поиск по настройкам — справочники, разделы, опции…"
              className="focus:border-primary focus:ring-primary/20 h-10 w-full rounded-[10px] border border-slate-200 bg-white pr-10 pl-[38px] text-[14px] outline-none focus:ring-3"
            />
            {query && (
              <button
                type="button"
                onClick={() => setQuery("")}
                className="absolute right-2 top-1/2 flex size-6 -translate-y-1/2 items-center justify-center rounded-md text-slate-400 transition-colors hover:bg-slate-100"
              >
                <X className="size-3.5" />
              </button>
            )}
          </div>
        </div>
      }
    >
      {isLoading ? (
        <SettingsSkeleton />
      ) : (
        <div className="mx-auto flex max-w-[1180px] flex-col gap-8">
          {sectionVisible.org && <OrgSection />}

          {sectionVisible.directories && (
            <DirectoriesSection items={dirsFiltered} query={query} />
          )}

          {sectionVisible.access && <AccessSection />}

          {sectionVisible.platform && (
            <PlatformSection items={platformFiltered} />
          )}

          {!anyVisible && <EmptySearchPage query={query} />}
        </div>
      )}
    </PageLayout>
  );
}
