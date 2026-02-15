import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { InvitationModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export type PendingInvitationsQuery = {
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
  search?: string;
};

export default function usePendingInvitationsPaginated(
  orgId: number,
  query?: PendingInvitationsQuery,
  options?: Omit<
    UseQueryOptions<PagedResult<InvitationModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.pendingInvitationsPaginated(orgId, query),
    queryFn: async () => {
      const allInvitations = await companyApiClient.getPendingInvitations(orgId);

      let filteredInvitations = allInvitations;
      if (query?.search) {
        const searchLower = query.search.toLowerCase();
        filteredInvitations = allInvitations.filter(
          (inv) =>
            inv.inviteeEmail?.toLowerCase().includes(searchLower) ||
            inv.inviteeProfileId?.toString().includes(searchLower),
        );
      }

      if (query?.orderBy) {
        filteredInvitations = [...filteredInvitations].sort((a, b) => {
          const aVal = a[query.orderBy as keyof InvitationModel];
          const bVal = b[query.orderBy as keyof InvitationModel];
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
      const items = filteredInvitations.slice(start, start + pageSize);

      return {
        items,
        totalCount: filteredInvitations.length,
      };
    },
    enabled: orgId > 0,
    ...options,
  });
}
