import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type {
  OrganizationDto,
  OrganizationsQuery,
} from "@workspace/types/organization";
import type { PagedResult } from "@workspace/types/shared";

import { organizationKeys } from "../keys";

/** Возвращает постраничный список организаций с опциональной фильтрацией. */
export default function useOrganizations(
  query?: OrganizationsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<OrganizationDto>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: organizationKeys.organizations(query),
    queryFn: () => organizationApiClient.getOrganizations(query),
    ...options,
  });
}
