import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type {
  OrganizationMemberDto,
  OrganizationMembersQuery,
} from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

/**
 * Возвращает участников текущей организации (берётся из localStorage selectedOrgId).
 * @param orgId  ID организации — используется только как часть query key для кэширования.
 */
export default function useOrganizationMembers(
  orgId: string,
  query?: OrganizationMembersQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<OrganizationMemberDto>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.members(orgId, query),
    queryFn: () => companyApiClient.getMembers(query),
    enabled: Boolean(orgId),
    ...options,
  });
}
