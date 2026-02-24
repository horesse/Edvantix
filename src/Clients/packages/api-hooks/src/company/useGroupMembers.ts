import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient, {
  type GroupMembersQuery,
} from "@workspace/api-client/company/company";
import type { GroupMemberModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export default function useGroupMembers(
  groupId: string,
  query?: Omit<GroupMembersQuery, "groupId">,
  options?: Omit<
    UseQueryOptions<PagedResult<GroupMemberModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.groupMembers(groupId, query),
    queryFn: () =>
      companyApiClient.getGroupMembers({
        groupId,
        ...query,
      }),
    enabled: Boolean(groupId),
    ...options,
  });
}
