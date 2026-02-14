import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { GroupMemberModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useGroupMembers(
  groupId: number,
  options?: Omit<UseQueryOptions<GroupMemberModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.groupMembers(groupId),
    queryFn: () => companyApiClient.getGroupMembers(groupId),
    enabled: groupId > 0,
    ...options,
  });
}
