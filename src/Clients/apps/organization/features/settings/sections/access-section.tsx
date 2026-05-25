import Link from "next/link";

import { ChevronRight, Shield } from "lucide-react";

import useRolesSummary from "@workspace/api-hooks/organization/useRolesSummary";
import { Badge } from "@workspace/ui/components/badge";

import { useOrganization } from "@/components/organization/provider";

import { SectionHeader } from "../components/section-header";

export function AccessSection() {
  const { currentOrg } = useOrganization();
  const orgId = currentOrg?.id ?? "";
  const { data: summary } = useRolesSummary(orgId, {
    enabled: Boolean(orgId),
  });

  const rolesCount = summary?.totalRoles ?? 0;
  const membersCount = summary?.assignedMembersCount ?? 0;
  const preview = summary?.roleNamesPreview ?? [];

  return (
    <section>
      <SectionHeader
        Icon={Shield}
        title="Доступы"
        subtitle="Кто что может делать в системе"
      />
      <Link href="/organization/roles" className="block">
        <div className="group flex cursor-pointer items-center gap-[18px] rounded-2xl border border-slate-200 bg-white px-6 py-5 shadow-[0_1px_3px_rgba(15,23,42,0.04)] transition-all duration-150 hover:border-indigo-200 hover:shadow-[0_4px_16px_rgba(79,70,229,0.10)]">
          <div className="flex size-11 shrink-0 items-center justify-center rounded-[12px] bg-indigo-50 text-indigo-700">
            <Shield className="size-[22px]" />
          </div>

          <div className="min-w-0 flex-1">
            <div className="mb-1 flex items-center gap-2.5">
              <h3 className="text-[15px] font-semibold text-slate-900">
                Роли и права
              </h3>
              {rolesCount > 0 && (
                <Badge variant="secondary">{rolesCount} ролей</Badge>
              )}
            </div>
            <div className="text-[13px] text-slate-500">
              {preview.length > 0
                ? preview.join(", ")
                : "Шаблоны прав для сотрудников"}
            </div>
          </div>

          <div className="flex shrink-0 items-center gap-1.5 text-[12px] whitespace-nowrap text-slate-500 tabular-nums">
            {membersCount > 0 && (
              <span>
                <strong className="font-semibold text-slate-900">
                  {membersCount}
                </strong>{" "}
                сотрудника назначены
              </span>
            )}
          </div>

          <ChevronRight className="size-4 shrink-0 text-slate-300 transition-colors duration-150 group-hover:text-indigo-400" />
        </div>
      </Link>
    </section>
  );
}
