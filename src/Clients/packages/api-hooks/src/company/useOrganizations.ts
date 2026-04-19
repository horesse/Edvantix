import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type {
  OrganizationDto,
  OrganizationsQuery,
} from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

/** Возвращает постраничный список организаций с опциональной фильтрацией. */
export default function useOrganizations(
  query?: OrganizationsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<OrganizationDto>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.organizations(query),
    queryFn: () => companyApiClient.getOrganizations(query),
    ...options,
  });
}
