import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { RolesSummaryDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/** Возвращает сводку ролей организации для страницы настроек. */
export default function useRolesSummary(
  orgId: string,
  options?: Omit<UseQueryOptions<RolesSummaryDto>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: organizationKeys.rolesSummary(orgId),
    queryFn: () => organizationApiClient.getRolesSummary(),
    enabled: Boolean(orgId),
    ...options,
  });
}
