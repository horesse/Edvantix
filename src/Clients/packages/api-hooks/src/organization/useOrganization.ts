import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { OrganizationDetailDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/** Возвращает полные данные организации по её ID. */
export default function useOrganization(
  id: string,
  options?: Omit<
    UseQueryOptions<OrganizationDetailDto>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: organizationKeys.organization(id),
    queryFn: () => organizationApiClient.getOrganization(id),
    enabled: Boolean(id),
    ...options,
  });
}
