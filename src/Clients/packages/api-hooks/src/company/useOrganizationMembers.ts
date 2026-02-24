import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient, {
  type OrganizationMembersQuery,
} from "@workspace/api-client/company/company";
import type { OrganizationMemberModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export default function useOrganizationMembers(
  orgId: string,
  query?: Omit<OrganizationMembersQuery, "organizationId">,
  options?: Omit<
    UseQueryOptions<PagedResult<OrganizationMemberModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.members(orgId, query),
    queryFn: () =>
      companyApiClient.getMembers({
        organizationId: orgId,
        ...query,
      }),
    enabled: Boolean(orgId),
    ...options,
  });
}
