import Link from "next/link";

import { ArrowRight, Building2, Info } from "lucide-react";

import useOrganizationSummary from "@workspace/api-hooks/organization/useOrganizationSummary";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";

import { useOrganization } from "@/components/organization/provider";

import { OrgStat } from "../components/org-stat";
import { SectionHeader } from "../components/section-header";
import { ORG_EDIT_ROUTE } from "../constants";
import { relativeDate } from "../lib/declension";

export function OrgSection() {
  const { currentOrg } = useOrganization();
  const orgId = currentOrg?.id ?? "";
  const { data: summary } = useOrganizationSummary(orgId, {
    enabled: Boolean(orgId),
  });

  const displayName = summary?.shortName ?? summary?.fullLegalName ?? currentOrg?.shortName ?? currentOrg?.fullLegalName;
  const initial = displayName?.replace(/[«»"]/g, "").trim().charAt(0) ?? "?";

  const lastModifiedLabel = summary?.lastModified?.at
    ? relativeDate(summary.lastModified.at)
    : null;

  return (
    <section>
      <SectionHeader
        Icon={Building2}
        title="Организация"
        subtitle="Юридические данные, контакты и фирменные документы"
      />
      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-[0_1px_3px_rgba(15,23,42,0.04)]">
        {/* Top bar */}
        <div className="flex items-center gap-5 border-b border-slate-50 bg-gradient-to-br from-indigo-50/50 to-violet-50/40 px-6 py-6">
          {/* Avatar */}
          <div className="flex size-[60px] shrink-0 items-center justify-center rounded-[14px] bg-gradient-to-br from-indigo-500 to-violet-500 text-[22px] font-bold text-white shadow-[0_4px_14px_rgba(99,102,241,0.3)]">
            {initial}
          </div>

          <div className="min-w-0 flex-1">
            <div className="mb-1 flex items-center gap-2.5">
              <h2 className="text-[18px] font-bold tracking-[-0.02em] text-slate-900">
                {displayName ?? "—"}
              </h2>
              {summary?.isLegalEntity === false && (
                <Badge variant="secondary">ИП</Badge>
              )}
            </div>
            <div className="text-[13px] text-slate-500">
              {summary?.fullLegalName}
            </div>
          </div>

          <Link href={ORG_EDIT_ROUTE} className="shrink-0">
            <Button variant="default" size="sm">
              Редактировать
              <ArrowRight className="size-3.5" />
            </Button>
          </Link>
        </div>

        {/* Stats grid */}
        <div className="grid grid-cols-4">
          <OrgStat
            label="Тип организации"
            value={summary ? String(summary.organizationType) : null}
          />
          <OrgStat
            label="Сотрудников"
            value={summary ? String(summary.membersCount) : null}
          />
          <OrgStat
            label="Основной контакт"
            value={summary?.primaryContact?.value ?? null}
            mono
          />
          <OrgStat
            label="Изменено"
            value={lastModifiedLabel}
            hint={summary?.lastModified?.byDisplayName ?? undefined}
          />
        </div>

        {/* Footer link */}
        <div className="flex items-center gap-3.5 border-t border-slate-50 bg-slate-50/60 px-6 py-3.5 text-[13px] text-slate-500">
          <Info className="size-[15px] text-slate-400" />
          <span>Сотрудники организации</span>
          <span className="text-slate-300">·</span>
          <Link
            href="/organization/members"
            className="font-medium text-indigo-600 hover:underline"
          >
            Управление профилями и приглашениями →
          </Link>
        </div>
      </div>
    </section>
  );
}
