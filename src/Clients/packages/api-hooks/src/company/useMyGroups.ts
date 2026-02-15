import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient, {
  type MyGroupsQuery,
} from "@workspace/api-client/company/company";
import type { GroupSummaryModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export default function useMyGroups(
  query?: MyGroupsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<GroupSummaryModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.myGroups(query),
    queryFn: () => companyApiClient.getMyGroups(query),
    ...options,
  });
}
