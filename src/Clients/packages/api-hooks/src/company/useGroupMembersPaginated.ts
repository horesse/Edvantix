import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { GroupMemberModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export type GroupMembersQuery = {
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
  search?: string;
};

export default function useGroupMembersPaginated(
  groupId: number,
  query?: GroupMembersQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<GroupMemberModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.groupMembersPaginated(groupId, query),
    queryFn: async () => {
      const allMembers = await companyApiClient.getGroupMembers(groupId);

      let filteredMembers = allMembers;
      if (query?.search) {
        const searchLower = query.search.toLowerCase();
        filteredMembers = allMembers.filter((m) =>
          m.displayName?.toLowerCase().includes(searchLower),
        );
      }

      if (query?.orderBy) {
        filteredMembers = [...filteredMembers].sort((a, b) => {
          const aVal = a[query.orderBy as keyof GroupMemberModel];
          const bVal = b[query.orderBy as keyof GroupMemberModel];
          if (aVal === bVal) return 0;
          if (aVal == null) return 1;
          if (bVal == null) return -1;
          const result = aVal > bVal ? 1 : -1;
          return query.isDescending ? -result : result;
        });
      }

      const pageSize = query?.pageSize ?? 10;
      const pageIndex = query?.pageIndex ?? 1;
      const start = (pageIndex - 1) * pageSize;
      const items = filteredMembers.slice(start, start + pageSize);

      return {
        items,
        totalCount: filteredMembers.length,
      };
    },
    enabled: groupId > 0,
    ...options,
  });
}
