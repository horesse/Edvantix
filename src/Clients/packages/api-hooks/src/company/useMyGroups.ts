import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { GroupSummaryModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useMyGroups(
  options?: Omit<UseQueryOptions<GroupSummaryModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.myGroups(),
    queryFn: () => companyApiClient.getMyGroups(),
    ...options,
  });
}
