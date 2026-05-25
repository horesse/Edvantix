import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type {
  OrganizationMemberDto,
  OrganizationMembersQuery,
} from "@workspace/types/organization";
import type { PagedResult } from "@workspace/types/shared";

import { organizationKeys } from "../keys";

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
    queryKey: organizationKeys.members(orgId, query),
    queryFn: () => organizationApiClient.getMembers(query),
    enabled: Boolean(orgId),
    ...options,
  });
}
