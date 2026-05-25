import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { OrganizationSummaryDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/** Возвращает сводку организации для страницы настроек. */
export default function useOrganizationSummary(
  orgId: string,
  options?: Omit<
    UseQueryOptions<OrganizationSummaryDto>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: organizationKeys.organizationSummary(orgId),
    queryFn: () => organizationApiClient.getOrganizationSummary(),
    enabled: Boolean(orgId),
    ...options,
  });
}
