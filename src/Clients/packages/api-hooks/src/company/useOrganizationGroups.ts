import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient, {
  type OrganizationGroupsQuery,
} from "@workspace/api-client/company/company";
import type { GroupModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export default function useOrganizationGroups(
  orgId: string,
  query?: Omit<OrganizationGroupsQuery, "organizationId">,
  options?: Omit<
    UseQueryOptions<PagedResult<GroupModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.groups(orgId, query),
    queryFn: () =>
      companyApiClient.getOrganizationGroups({
        organizationId: orgId,
        ...query,
      }),
    enabled: Boolean(orgId),
    ...options,
  });
}
