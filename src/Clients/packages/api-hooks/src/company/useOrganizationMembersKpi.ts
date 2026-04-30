import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationMembersKpiDto } from "@workspace/types/company";

import { companyKeys } from "../keys";

/**
 * Возвращает KPI-статистику участников текущей организации:
 * общее количество, активные, архивные и удалённые.
 *
 * @param orgId ID организации — используется только как часть query key для кэширования.
 */
export default function useOrganizationMembersKpi(
  orgId: string,
  options?: Omit<
    UseQueryOptions<OrganizationMembersKpiDto>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.membersKpi(orgId),
    queryFn: () => companyApiClient.getMembersKpi(),
    enabled: Boolean(orgId),
    ...options,
  });
}
