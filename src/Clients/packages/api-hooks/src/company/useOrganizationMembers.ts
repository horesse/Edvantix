import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationMemberModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useOrganizationMembers(
  orgId: number,
  options?: Omit<
    UseQueryOptions<OrganizationMemberModel[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.members(orgId),
    queryFn: () => companyApiClient.getMembers(orgId),
    enabled: orgId > 0,
    ...options,
  });
}
