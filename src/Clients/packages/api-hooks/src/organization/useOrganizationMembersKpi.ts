import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { OrganizationMembersKpiDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

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
    queryKey: organizationKeys.membersKpi(orgId),
    queryFn: () => organizationApiClient.getMembersKpi(),
    enabled: Boolean(orgId),
    ...options,
  });
}
