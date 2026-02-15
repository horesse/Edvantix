import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { GroupModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export type OrganizationGroupsQuery = {
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
  search?: string;
};

export default function useOrganizationGroupsPaginated(
  orgId: number,
  query?: OrganizationGroupsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<GroupModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.groupsPaginated(orgId, query),
    queryFn: async () => {
      const allGroups = await companyApiClient.getOrganizationGroups(orgId);

      let filteredGroups = allGroups;
      if (query?.search) {
        const searchLower = query.search.toLowerCase();
        filteredGroups = allGroups.filter(
          (g) =>
            g.name.toLowerCase().includes(searchLower) ||
            g.description?.toLowerCase().includes(searchLower),
        );
      }

      if (query?.orderBy) {
        filteredGroups = [...filteredGroups].sort((a, b) => {
          const aVal = a[query.orderBy as keyof GroupModel];
          const bVal = b[query.orderBy as keyof GroupModel];
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
      const items = filteredGroups.slice(start, start + pageSize);

      return {
        items,
        totalCount: filteredGroups.length,
      };
    },
    enabled: orgId > 0,
    ...options,
  });
}
